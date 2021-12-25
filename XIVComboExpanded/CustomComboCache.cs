using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Dalamud.Game;
using Dalamud.Game.ClientState.Buddy;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;

namespace XIVComboExpandedPlugin
{
    /// <summary>
    /// Cached conditional combo logic.
    /// </summary>
    internal partial class CustomComboCache : IDisposable
    {
        private const uint InvalidObjectID = 0xE000_0000;

        // Invalidate these
        private readonly Dictionary<(uint StatusID, uint? TargetID, uint? SourceID), Status?> statusCache = new();
        private readonly Dictionary<uint, CooldownData> cooldownCache = new();

        // Do not invalidate these
        private readonly Dictionary<uint, byte> cooldownGroupCache = new();
        private readonly Dictionary<Type, JobGaugeBase> jobGaugeCache = new();

        private readonly GetActionCooldownSlotDelegate getActionCooldownSlot;

        private IntPtr actionManager = IntPtr.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomComboCache"/> class.
        /// </summary>
        public CustomComboCache()
        {
            this.getActionCooldownSlot = Marshal.GetDelegateForFunctionPointer<GetActionCooldownSlotDelegate>(Service.Address.GetActionCooldown);

            Service.Framework.Update += this.Framework_Update;
        }

        private delegate IntPtr GetActionCooldownSlotDelegate(IntPtr actionManager, int cooldownGroup);

        /// <inheritdoc/>
        public void Dispose()
        {
            Service.Framework.Update -= this.Framework_Update;
        }

        /// <summary>
        /// Update the address of the action manager.
        /// </summary>
        /// <param name="address">Action manager address.</param>
        internal void UpdateActionManager(IntPtr address)
        {
            this.actionManager = address;
        }

        /// <summary>
        /// Get a job gauge.
        /// </summary>
        /// <typeparam name="T">Type of job gauge.</typeparam>
        /// <returns>The job gauge.</returns>
        internal T GetJobGauge<T>() where T : JobGaugeBase
        {
            if (!this.jobGaugeCache.TryGetValue(typeof(T), out var gauge))
                gauge = this.jobGaugeCache[typeof(T)] = Service.JobGauges.Get<T>();

            return (T)gauge;
        }

        /// <summary>
        /// Finds a status on the given object.
        /// </summary>
        /// <param name="statusID">Status effect ID.</param>
        /// <param name="obj">Object to look for effects on.</param>
        /// <param name="sourceID">Source object ID.</param>
        /// <returns>Status object or null.</returns>
        internal Status? GetStatus(uint statusID, GameObject? obj, uint? sourceID)
        {
            var key = (statusID, obj?.ObjectId, sourceID);
            if (this.statusCache.TryGetValue(key, out var found))
                return found;

            if (obj is null)
                return this.statusCache[key] = null;

            if (obj is not BattleChara chara)
                return this.statusCache[key] = null;

            foreach (var status in chara.StatusList)
            {
                if (status.StatusId == statusID && (!sourceID.HasValue || status.SourceID == 0 || status.SourceID == InvalidObjectID || status.SourceID == sourceID))
                    return this.statusCache[key] = status;
            }

            return this.statusCache[key] = null;
        }

        /// <summary>
        /// Gets the cooldown data for an action.
        /// </summary>
        /// <param name="actionID">Action ID to check.</param>
        /// <returns>Cooldown data.</returns>
        internal unsafe CooldownData GetCooldown(uint actionID)
        {
            if (this.cooldownCache.TryGetValue(actionID, out var found))
                return found;

            var cooldownGroup = this.GetCooldownGroup(actionID);
            if (this.actionManager == IntPtr.Zero)
                return this.cooldownCache[actionID] = new CooldownData() { ActionID = actionID };

            var cooldownPtr = this.getActionCooldownSlot(this.actionManager, cooldownGroup - 1);
            return this.cooldownCache[actionID] = *(CooldownData*)cooldownPtr;
        }

        private byte GetCooldownGroup(uint actionID)
        {
            if (this.cooldownGroupCache.TryGetValue(actionID, out var cooldownGroup))
                return cooldownGroup;

            var sheet = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()!;
            var row = sheet.GetRow(actionID);

            return this.cooldownGroupCache[actionID] = row!.CooldownGroup;
        }

        private unsafe void Framework_Update(Framework framework)
        {
            this.statusCache.Clear();
            this.cooldownCache.Clear();
        }
    }
}
