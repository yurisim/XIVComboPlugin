using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboExpandedPlugin;
using XIVComboExpandedPlugin.Combos;

namespace XIVComboExpandedestPlugin.Combos
{
    internal static class All
    {
        public const byte JobID = 0;

        public const uint
            Swiftcast = 7561,
            Resurrection = 173,
            Verraise = 7523,
            Raise = 125,
            Ascend = 3603,
            Egeiro = 24287,
            SolidReason = 232,
            AgelessWords = 215,
            WiseToTheWorldMIN = 26521,
            WiseToTheWorldBTN = 26522,
            LowBlow = 7540,
            Interject = 7538,
            Reprisal = 7535,
            LucidDreaming = 7562,
            Cast = 289,
            Hook = 296,
            CastLight = 2135,
            Snagging = 4100,
            SurfaceSlap = 4595,
            Gig = 7632,
            VeteranTrade = 7906,
            NaturesBounty = 7909,
            Salvage = 7910,
            ElectricCurrent = 26872,
            PrizeCatch = 26806;

        public static class Buffs
        {
            public const ushort
                Swiftcast = 167,
                EurekaMoment = 2765;
        }

        public static class Debuffs
        {
            public const ushort
                Reprisal = 1193;
        }

        public static class Levels
        {
            public const byte
                Cast = 1,
                Hook = 1,
                Raise = 12,
                Snagging = 36,
                Gig = 61,
                Salvage = 67,
                VeteranTrade = 63,
                NaturesBounty = 69,
                SurfaceSlap = 71,
                PrizeCatch = 81;
        }
    }

    internal class AllSwiftcastFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AllSwiftcastFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == All.Raise || actionID == All.Resurrection || actionID == All.Ascend || actionID == All.Verraise || actionID == All.Egeiro)
            {
                if ((IsOffCooldown(All.Swiftcast) && !HasEffect(RDM.Buffs.Dualcast))
                    || level <= All.Levels.Raise
                    || (level <= RDM.Levels.Verraise && actionID == All.Verraise))
                    return All.Swiftcast;
            }

            return actionID;
        }
    }

    //internal class AllTankInterruptFeature : CustomCombo
    //{
    //    protected override CustomComboPreset Preset => CustomComboPreset.AllTankInterruptFeature;

    //    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    //    {
    //        if (actionID == All.LowBlow)
    //        {
    //            if (CanInterruptEnemy() && IsActionOffCooldown(All.Interject) && CanUseAction(All.Interject))
    //                return All.Interject;
    //        }

    //        return actionID;
    //    }
    //}

    //internal class AllReprisalLockoutFeature : CustomCombo
    //{
    //    protected override CustomComboPreset Preset => CustomComboPreset.AllReprisalLockoutFeature;

    //    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    //    {
    //        if (actionID == All.Reprisal)
    //        {
    //            var reprisalDebuff = FindTargetEffectAny(All.Debuffs.Reprisal);
    //            if (reprisalDebuff != null && reprisalDebuff.RemainingTime > 3 && IsActionOffCooldown(All.Reprisal)) return SMN.Physick;
    //        }

    //        return actionID;
    //    }
    //}
}