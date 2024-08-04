using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class WAR
{
    public const byte ClassID = 3;
    public const byte JobID = 21;

    public const uint HeavySwing = 31,
        Maim = 37,
        Berserk = 38,
        ThrillOfBattle = 40,
        Overpower = 41,
        StormsPath = 42,
        StormsEye = 45,
        Defiance = 48,
        InnerBeast = 49,
        SteelCyclone = 51,
        Infuriate = 52,
        FellCleave = 3549,
        Decimate = 3550,
        RawIntuition = 3551,
        Equilibrium = 3552,
        Upheaval = 7387,
        InnerRelease = 7389,
        MythrilTempest = 16462,
        ChaoticCyclone = 16463,
        NascentFlash = 16464,
        InnerChaos = 16465,
        Bloodwhetting = 25751,
        PrimalRend = 25753,
        DefianceRemoval = 32066,
        PrimalWrath = 36924,
        PrimalRuination = 36925;

    public static class Buffs
    {
        public const ushort
            Berserk = 86,
            Defiance = 91,
            Holmgang = 409,
            InnerRelease = 1177,
            NascentChaos = 1897,
            PrimalRendReady = 2624,
            SurgingTempest = 2677,
            Wrathful = 3901,
            PrimalRuinationReady = 3834;
    }

    public static class Debuffs
    {
        public const ushort Placeholder = 0;
    }

    public static class Levels
    {
        public const byte Maim = 4,
            Berserk = 6,
            Defiance = 10,
            StormsPath = 26,
            ThrillOfBattle = 30,
            InnerBeast = 35,
            MythrilTempest = 40,
            SteelCyclone = 45,
            StormsEye = 50,
            Infuriate = 50,
            FellCleave = 54,
            RawIntuition = 56,
            Equilibrium = 58,
            Decimate = 60,
            Upheaval = 64,
            InnerRelease = 70,
            ChaoticCyclone = 72,
            MythrilTempestTrait = 74,
            NascentFlash = 76,
            InnerChaos = 80,
            Bloodwhetting = 82,
            PrimalRend = 90,
            PrimalWrath = 96,
            PrimalRuination = 100;
    }
}

internal class WarriorStormsPathCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.HeavySwing)
        {
            var gauge = GetJobGauge<WARGauge>();

            var surgingTempest = FindEffect(WAR.Buffs.SurgingTempest);

            if (GCDClipCheck(actionID))
            {
                var localPlayerPercentage = LocalPlayerPercentage();

                if (
                    level >= WAR.Levels.Infuriate
                    && gauge.BeastGauge <= 50
                    && HasCharges(WAR.Infuriate)
                    && (level < WAR.Levels.InnerChaos || !HasEffect(WAR.Buffs.NascentChaos))
                    && (
                        GetRemainingCharges(WAR.Infuriate) >= 2
                        || GetCooldown(WAR.Infuriate).ChargeCooldownRemaining <= 5
                        || HasRaidBuffs()
                        || HasEffect(WAR.Buffs.Berserk)
                    )
                    && !HasEffect(WAR.Buffs.InnerRelease)
                )
                {
                    return WAR.Infuriate;
                }

                if (
                    IsOffCooldown(WAR.Upheaval)
                    && surgingTempest is not null
                    && level >= WAR.Levels.Upheaval
                )
                {
                    return WAR.Upheaval;
                }

                if (
                    IsOffCooldown(OriginalHook(WAR.Berserk))
                    && (level < WAR.Levels.StormsEye || surgingTempest is not null)
                    && (!HasEffect(WAR.Buffs.NascentChaos) || (level < WAR.Levels.InnerChaos))
                )
                {
                    return OriginalHook(WAR.Berserk);
                }

                if (
                    level >= WAR.Levels.Equilibrium
                    && IsOffCooldown(WAR.Equilibrium)
                    && (localPlayerPercentage <= 0.60)
                )
                {
                    return WAR.Equilibrium;
                }
            }

            if (
                level >= WAR.Levels.InnerBeast
                && (surgingTempest is not null || level < WAR.Levels.StormsEye)
                && (
                    gauge.BeastGauge >= 90
                    || (HasEffect(WAR.Buffs.NascentChaos) && level >= WAR.Levels.InnerChaos)
                    || (
                        gauge.BeastGauge >= 50
                        && (
                            GetRemainingCharges(WAR.Infuriate) >= 2
                            || HasRaidBuffs()
                            || HasEffect(WAR.Buffs.Berserk)
                        )
                    )
                    || HasEffect(WAR.Buffs.InnerRelease)
                )
            )
            {
                return OriginalHook(WAR.InnerBeast);
            }

            if (comboTime > 0)
            {
                if (lastComboMove == WAR.Maim && level >= WAR.Levels.StormsPath)
                {
                    if (
                        level >= WAR.Levels.StormsEye
                        && (surgingTempest is null || surgingTempest?.RemainingTime < 30)
                    )
                        return WAR.StormsEye;

                    return WAR.StormsPath;
                }

                if (lastComboMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                {
                    return WAR.Maim;
                }
            }
        }

        return actionID;
    }
}

internal class WarriorMythrilTempestCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.Overpower)
        {
            var gauge = GetJobGauge<WARGauge>();

            if (GCDClipCheck(actionID))
            {
                var localPlayerPercentage =
                    (LocalPlayer is not null)
                        ? (float)LocalPlayer.CurrentHp / LocalPlayer.MaxHp
                        : 1;

                if (IsOffCooldown(WAR.Upheaval) && level >= WAR.Levels.Upheaval)
                {
                    return WAR.Upheaval;
                }

                if (
                    level >= WAR.Levels.Infuriate
                    && InCombat()
                    && gauge.BeastGauge <= 50
                    && HasCharges(WAR.Infuriate)
                    && (level < WAR.Levels.InnerChaos || !HasEffect(WAR.Buffs.NascentChaos))
                    && (
                        GetRemainingCharges(WAR.Infuriate) >= 2
                        || GetCooldown(WAR.Infuriate).ChargeCooldownRemaining <= 5
                        || HasRaidBuffs()
                        || HasEffect(WAR.Buffs.Berserk)
                    )
                    && !HasEffect(WAR.Buffs.InnerRelease)
                )
                {
                    return WAR.Infuriate;
                }

                if (
                    IsOffCooldown(OriginalHook(WAR.Berserk))
                    && (!HasEffect(WAR.Buffs.NascentChaos) || (level < WAR.Levels.ChaoticCyclone))
                )
                {
                    return OriginalHook(WAR.Berserk);
                }

                if (
                    level >= WAR.Levels.Equilibrium
                    && IsOffCooldown(WAR.Equilibrium)
                    && localPlayerPercentage <= 0.70
                )
                {
                    return WAR.Equilibrium;
                }
            }

            var surgingTempest = FindEffect(WAR.Buffs.SurgingTempest);

            if (
                level >= WAR.Levels.SteelCyclone
                && (surgingTempest is not null || level < WAR.Levels.MythrilTempest)
                && (
                    gauge.BeastGauge >= 80
                    || (HasEffect(WAR.Buffs.NascentChaos) && level >= WAR.Levels.ChaoticCyclone)
                    || (
                        gauge.BeastGauge >= 50
                        && (GetRemainingCharges(WAR.Infuriate) >= 2 || HasEffect(WAR.Buffs.Berserk))
                    )
                    || HasEffect(WAR.Buffs.InnerRelease)
                )
            )
            {
                return OriginalHook(WAR.SteelCyclone);
            }

            if (comboTime > 0)
            {
                if (lastComboMove == WAR.Overpower && level >= WAR.Levels.MythrilTempest)
                {
                    return WAR.MythrilTempest;
                }
            }

            return actionID;
        }

        return actionID;
    }
}

internal class WarriorFellCleaveDecimate : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (
            actionID == WAR.InnerBeast
            || actionID == WAR.FellCleave
            || actionID == WAR.SteelCyclone
            || actionID == WAR.Decimate
        )
        {
            if (IsEnabled(CustomComboPreset.WarriorPrimalBeastFeature))
            {
                if (level >= WAR.Levels.PrimalRend && HasEffect(WAR.Buffs.PrimalRendReady))
                    return WAR.PrimalRend;
            }

            if (IsEnabled(CustomComboPreset.WarriorInfuriateBeastFeature))
            {
                var gauge = GetJobGauge<WARGauge>();

                if (
                    level >= WAR.Levels.Infuriate
                    && gauge.BeastGauge < 50
                    && !HasEffect(WAR.Buffs.InnerRelease)
                )
                    return WAR.Infuriate;
            }
        }

        return actionID;
    }
}

internal class WarriorBerserkInnerRelease : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.WarriorPrimalReleaseFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.Berserk || actionID == WAR.InnerRelease)
        {
            if (level >= WAR.Levels.PrimalWrath && HasEffect(WAR.Buffs.Wrathful))
                return WAR.PrimalWrath;

            if (level >= WAR.Levels.PrimalRuination && HasEffect(WAR.Buffs.PrimalRuinationReady))
                return WAR.PrimalRuination;

            if (level >= WAR.Levels.PrimalRend && HasEffect(WAR.Buffs.PrimalRendReady))
                return WAR.PrimalRend;
        }

        return actionID;
    }
}

internal class WarriorNascentFlashFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.WarriorNascentFlashSyncFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.NascentFlash)
        {
            if (level >= WAR.Levels.NascentFlash)
                return WAR.NascentFlash;

            if (level >= WAR.Levels.RawIntuition)
                return WAR.RawIntuition;
        }

        return actionID;
    }
}

internal class WarriorBloodwhetting : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.Bloodwhetting || actionID == WAR.RawIntuition)
        {
            if (IsEnabled(CustomComboPreset.WarriorHealthyBalancedDietFeature))
            {
                if (level >= WAR.Levels.Bloodwhetting)
                {
                    if (IsCooldownUsable(WAR.Bloodwhetting))
                        return WAR.Bloodwhetting;
                }
                else if (level >= WAR.Levels.RawIntuition)
                {
                    if (IsCooldownUsable(WAR.RawIntuition))
                        return WAR.RawIntuition;
                }

                if (level >= WAR.Levels.ThrillOfBattle && IsCooldownUsable(WAR.ThrillOfBattle))
                    return WAR.ThrillOfBattle;

                if (level >= WAR.Levels.Equilibrium && IsCooldownUsable(WAR.Equilibrium))
                    return WAR.Equilibrium;
            }
        }

        return actionID;
    }
}
