using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class WAR
{
    public const byte ClassID = 3;
    public const byte JobID = 21;

    public const uint
        HeavySwing = 31,
        Maim = 37,
        Berserk = 38,
        ThrillOfBattle = 40,
        Overpower = 41,
        StormsPath = 42,
        StormsEye = 45,
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
        PrimalRend = 25753;

    public static class Buffs
    {
        public const ushort
            Berserk = 86,
            InnerRelease = 1177,
            NascentChaos = 1897,
            PrimalRendReady = 2624,
            SurgingTempest = 2677;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            Maim = 4,
            Berserk = 6,
            StormsPath = 26,
            ThrillOfBattle = 30,
            InnerBeast = 35,
            MythrilTempest = 40,
            StormsEye = 50,
            Infuriate = 50,
            FellCleave = 54,
            RawIntuition = 56,
            Equilibrium = 58,
            Decimate = 60,
            InnerRelease = 70,
            ChaoticCyclone = 72,
            MythrilTempestTrait = 74,
            NascentFlash = 76,
            InnerChaos = 80,
            Bloodwhetting = 82,
            PrimalRend = 90;
    }
}

internal class WarriorStormsPathCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorStormsPathCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.StormsPath)
        {
            var gauge = GetJobGauge<WARGauge>();

            if (IsEnabled(CustomComboPreset.WarriorStormsPathInnerReleaseFeature))
            {
                if (level >= WAR.Levels.InnerRelease && HasEffect(WAR.Buffs.InnerRelease))
                    return WAR.FellCleave;
            }

            if (comboTime > 0)
            {
                if (InCombat() && level >= WAR.Levels.Infuriate && gauge.BeastGauge < 50 && GetRemainingCharges(WAR.Infuriate) > 1 && !HasEffect(WAR.Buffs.InnerRelease) && GCDClipCheck(actionID))
                    return WAR.Infuriate;

                if (IsOffCooldown(WAR.Upheaval) && CanUseAction(WAR.Upheaval) && GCDClipCheck(actionID))
                {
                    return WAR.Upheaval;
                }

                if (lastComboMove == WAR.Maim && level >= WAR.Levels.StormsPath)
                {

                    if (IsEnabled(CustomComboPreset.WarriorStormsPathOvercapFeature))
                    {
                        if (level >= WAR.Levels.InnerBeast && gauge.BeastGauge > 80)
                            // Fell Cleave
                            return OriginalHook(WAR.InnerBeast);
                    }

#if DEBUG
                    var surgingTempest = FindEffect(WAR.Buffs.SurgingTempest);
                    if (surgingTempest is null && level >= WAR.Levels.StormsEye)
                        return WAR.StormsEye;

                    // Medicated + Opener
                    if (HasEffect(ADV.Buffs.Medicated) && surgingTempest?.RemainingTime > 10)
                        return WAR.StormsPath;

                    if (surgingTempest?.RemainingTime < 30 && level >= WAR.Levels.StormsEye)
                        return WAR.StormsEye;
#endif

                    return WAR.StormsPath;
                }

                if (lastComboMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                {
                    if (IsEnabled(CustomComboPreset.WarriorStormsPathOvercapFeature))
                    {
                        if (level >= WAR.Levels.InnerBeast && gauge.BeastGauge > 90)
                            // Fell Cleave
                            return OriginalHook(WAR.InnerBeast);
                    }

                    return WAR.Maim;
                }
            }

            return WAR.HeavySwing;
        }

        return actionID;
    }
}

internal class WarriorStormsEyeCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorStormsEyeCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.StormsEye)
        {
            if (comboTime > 0)
            {
                if (lastComboMove == WAR.Maim && level >= WAR.Levels.StormsEye)
                    return WAR.StormsEye;

                if (lastComboMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                    return WAR.Maim;
            }

            return WAR.HeavySwing;
        }

        return actionID;
    }
}

internal class WarriorMythrilTempestCombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorMythrilTempestCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.MythrilTempest)
        {
            var gauge = GetJobGauge<WARGauge>();

            if (InCombat() && level >= WAR.Levels.Infuriate && gauge.BeastGauge <= 50 && GetRemainingCharges(WAR.Infuriate) >= 1 && !HasEffect(WAR.Buffs.InnerRelease) && GCDClipCheck(actionID))
                return WAR.Infuriate;

            if (level >= WAR.Levels.ChaoticCyclone && CanUseAction(WAR.ChaoticCyclone))
            {
                return WAR.ChaoticCyclone;
            }

            if (IsEnabled(CustomComboPreset.WarriorMythrilTempestInnerReleaseFeature))
            {
                if (level >= WAR.Levels.InnerRelease && HasEffect(WAR.Buffs.InnerRelease))
                    return WAR.Decimate;
            }

            if (comboTime > 0)
            {
                if (lastComboMove == WAR.Overpower && level >= WAR.Levels.MythrilTempest)
                {
                    if (IsEnabled(CustomComboPreset.WarriorMythrilTempestOvercapFeature))
                    {
                        if (level >= WAR.Levels.MythrilTempestTrait && gauge.BeastGauge > 80)
                            return WAR.Decimate;
                    }

                    return WAR.MythrilTempest;
                }
            }


            return WAR.Overpower;
        }

        return actionID;
    }
}

internal class WarriorFellCleaveDecimate : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.InnerBeast || actionID == WAR.FellCleave || actionID == WAR.SteelCyclone || actionID == WAR.Decimate)
        {
            if (IsEnabled(CustomComboPreset.WarriorPrimalBeastFeature))
            {
                if (level >= WAR.Levels.PrimalRend && HasEffect(WAR.Buffs.PrimalRendReady))
                    return WAR.PrimalRend;
            }

            if (IsEnabled(CustomComboPreset.WarriorInfuriateBeastFeature))
            {
                var gauge = GetJobGauge<WARGauge>();

                if (level >= WAR.Levels.Infuriate && gauge.BeastGauge < 50 && !HasEffect(WAR.Buffs.InnerRelease))
                    return WAR.Infuriate;
            }
        }

        return actionID;
    }
}

internal class WarriorBerserkInnerRelease : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorPrimalReleaseFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WAR.Berserk || actionID == WAR.InnerRelease)
        {
            if (level >= WAR.Levels.PrimalRend && HasEffect(WAR.Buffs.PrimalRendReady))
                return WAR.PrimalRend;
        }

        return actionID;
    }
}

internal class WarriorNascentFlashFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorNascentFlashSyncFeature;

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
                    if (IsOffCooldown(WAR.Bloodwhetting))
                        return WAR.Bloodwhetting;
                }
                else if (level >= WAR.Levels.RawIntuition)
                {
                    if (IsOffCooldown(WAR.RawIntuition))
                        return WAR.RawIntuition;
                }

                if (level >= WAR.Levels.ThrillOfBattle && IsOffCooldown(WAR.ThrillOfBattle))
                    return WAR.ThrillOfBattle;

                if (level >= WAR.Levels.Equilibrium && IsOffCooldown(WAR.Equilibrium))
                    return WAR.Equilibrium;
            }
        }

        return actionID;
    }
}
