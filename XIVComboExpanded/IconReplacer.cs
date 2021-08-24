using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Plugin;
using Dalamud.Utility;
using XIVComboExpandedPlugin.Combos;

namespace XIVComboExpandedPlugin
{
    internal class IconReplacer
    {
        private readonly XIVComboExpandedPlugin Plugin;
        private readonly PluginAddressResolver Address;
        private readonly XIVComboExpandedConfiguration Configuration;

        private delegate ulong IsIconReplaceableDelegate(uint actionID);
        private delegate uint GetIconDelegate(IntPtr actionManager, uint actionID);
        private delegate IntPtr GetActionCooldownSlotDelegate(IntPtr actionManager, int cooldownGroup);

        private readonly Hook<IsIconReplaceableDelegate> IsIconReplaceableHook;
        private readonly Hook<GetIconDelegate> GetIconHook;

        private readonly GetActionCooldownSlotDelegate GetActionCooldownSlot;
        private IntPtr ActionManager = IntPtr.Zero;

        private readonly HashSet<uint> CustomIds = new();
        private List<CustomCombo> CustomCombos = new();

        public IconReplacer(XIVComboExpandedPlugin plugin)
        {
            Plugin = plugin;
            Configuration = plugin.Configuration;

            Address = new PluginAddressResolver();
            Address.Setup();

            CustomCombo.Initialize(this, Configuration);
            UpdateCustomCombos();

            UpdateEnabledActionIDs();

            GetActionCooldownSlot = Marshal.GetDelegateForFunctionPointer<GetActionCooldownSlotDelegate>(Address.GetActionCooldown);

            GetIconHook = new Hook<GetIconDelegate>(Address.GetIcon, GetIconDetour);
            IsIconReplaceableHook = new Hook<IsIconReplaceableDelegate>(Address.IsIconReplaceable, IsIconReplaceableDetour);

            GetIconHook.Enable();
            IsIconReplaceableHook.Enable();

        }

        internal void Dispose()
        {
            GetIconHook.Dispose();
            IsIconReplaceableHook.Dispose();
        }

        private void UpdateCustomCombos()
        {
            CustomCombos = Assembly.GetAssembly(typeof(CustomCombo))!.GetTypes()
                .Where(t => t.BaseType == typeof(CustomCombo))
                .Select(t => Activator.CreateInstance(t))
                .Cast<CustomCombo>()
                .ToList();
        }

        /// <summary>
        /// Maps to <see cref="XIVComboExpandedConfiguration.EnabledActions"/>, these actions can potentially update their icon per the user configuration.
        /// </summary>
        public void UpdateEnabledActionIDs()
        {
            var actionIDs = Enum
                .GetValues(typeof(CustomComboPreset))
                .Cast<CustomComboPreset>()
                .Select(preset => preset.GetAttribute<CustomComboInfoAttribute>())
                .OfType<CustomComboInfoAttribute>()
                .SelectMany(comboInfo => comboInfo.ActionIDs)
                .Concat(Configuration.DancerDanceCompatActionIDs)
                .ToHashSet();

            CustomIds.Clear();
            CustomIds.UnionWith(actionIDs);
        }

        public uint GetNewAction(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            foreach (var combo in CustomCombos)
            {
                if (combo.TryInvoke(actionID, lastComboMove, comboTime, level, out var newActionID))
                    return newActionID;
            }
            return OriginalHook(actionID);
        }

        private ulong IsIconReplaceableDetour(uint actionID) => 1;

        /// <summary>
        /// Replace an ability with another ability
        /// actionID is the original ability to be "used"
        /// Return either actionID (itself) or a new Action table ID as the
        /// ability to take its place.
        /// I tend to make the "combo chain" button be the last move in the combo
        /// For example, Souleater combo on DRK happens by dragging Souleater
        /// onto your bar and mashing it.
        /// </summary>
        private uint GetIconDetour(IntPtr actionManager, uint actionID)
        {
            try
            {
                ActionManager = actionManager;

                if (LocalPlayer == null || !CustomIds.Contains(actionID))
                    return OriginalHook(actionID);

                return GetNewAction(actionID, LastComboMove, ComboTime, LocalPlayer.Level);
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Don't crash the game");
                return GetIconHook.Original(actionManager, actionID);
            }
        }

        #region Getters

        internal bool HasCondition(ConditionFlag flag) => Plugin.Condition[flag];

        internal PlayerCharacter? LocalPlayer => Plugin.ClientState.LocalPlayer;

        internal GameObject? CurrentTarget => Plugin.TargetManager.Target;

        internal uint LastComboMove => (uint)Marshal.ReadInt32(Address.LastComboMove);

        internal float ComboTime => Marshal.PtrToStructure<float>(Address.ComboTimer);

        internal T GetJobGauge<T>() where T : JobGaugeBase => Plugin.JobGauges.Get<T>();

        internal uint OriginalHook(uint actionID) => GetIconHook.Original(ActionManager, actionID);

        #endregion

        #region Effects

        internal bool HasEffect(short effectId) => FindEffect(effectId) is not null;

        internal bool TargetHasEffect(short effectId) => FindTargetEffect(effectId) is not null;

        internal Status? FindEffect(short effectId) => FindEffect(effectId, LocalPlayer, null);

        internal Status? FindTargetEffect(short effectId) => FindEffect(effectId, CurrentTarget, LocalPlayer?.ObjectId);

        internal static Status? FindEffect(short effectId, GameObject? obj, uint? sourceID)
        {
            if (obj is null)
                return null;

            if (obj is not BattleChara chara)
                return null;

            foreach (var status in chara.StatusList)
            {
                if (status.StatusId == effectId)
                    if (!sourceID.HasValue || status.SourceID == sourceID)
                        return status;
            }

            return null;
        }

        #endregion

        #region Cooldowns

        private readonly Dictionary<uint, byte> CooldownGroups = new();

        private byte GetCooldownGroup(uint actionID)
        {
            if (CooldownGroups.TryGetValue(actionID, out var cooldownGroup))
                return cooldownGroup;

            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()!;
            var row = sheet.GetRow(actionID);

            return CooldownGroups[actionID] = row!.CooldownGroup;
        }

        internal CooldownData GetCooldown(uint actionID)
        {
            var cooldownGroup = GetCooldownGroup(actionID);
            if (ActionManager == IntPtr.Zero)
                return new CooldownData() { ActionID = actionID };

            var cooldownPtr = GetActionCooldownSlot(ActionManager, cooldownGroup - 1);
            return Marshal.PtrToStructure<CooldownData>(cooldownPtr);
        }

        #endregion
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct CooldownData
    {
        [FieldOffset(0x0)] public bool IsCooldown;
        [FieldOffset(0x4)] public uint ActionID;
        [FieldOffset(0x8)] public float CooldownElapsed;
        [FieldOffset(0xC)] public float CooldownTotal;

        public float CooldownRemaining => IsCooldown ? CooldownTotal - CooldownElapsed : 0;
    }
}
