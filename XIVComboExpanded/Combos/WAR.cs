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
        public const ushort Berserk = 86,
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
        if (actionID is WAR.HeavySwing or WAR.Overpower)
        {
            var gauge = GetJobGauge<WARGauge>();

            var surgingTempest = FindEffect(WAR.Buffs.SurgingTempest);

            var needToInfuriate = GetCooldown(WAR.Infuriate).TotalCooldownRemaining <= 10;

            var raidbuffs = HasRaidBuffs(1);

            if (GCDClipCheck(actionID))
            {
                var localPlayerPercentage = LocalPlayerPercentage();

                switch (level)
                {
                    case >= WAR.Levels.Infuriate
                        when gauge.BeastGauge <= 50
                            && HasCharges(WAR.Infuriate) && (level < WAR.Levels.InnerChaos || !HasEffect(WAR.Buffs.NascentChaos))
                            && (
                                needToInfuriate
                                || raidbuffs
                                || HasEffect(WAR.Buffs.Berserk)
                            )
                            && !HasEffect(WAR.Buffs.InnerRelease):
                        return WAR.Infuriate;

                    case >= WAR.Levels.Upheaval
                        when IsOffCooldown(WAR.Upheaval) && surgingTempest is not null:
                        return WAR.Upheaval;

                    case >= WAR.Levels.Berserk
                        when IsOffCooldown(OriginalHook(WAR.Berserk))
                            && (level < WAR.Levels.StormsEye || surgingTempest is not null)
                            // && (needToInfuriate || level >= WAR.Levels.Infuriate)
                            // && !HasEffect(WAR.Buffs.NascentChaos)
                            :
                        return OriginalHook(WAR.Berserk);

                    case >= WAR.Levels.Equilibrium
                        when IsOffCooldown(WAR.Equilibrium)
                            && localPlayerPercentage <= 0.70
                            && !HasEffect(WAR.Buffs.Holmgang):
                        return WAR.Equilibrium;

                    case >= WAR.Levels.RawIntuition
                        when IsOffCooldown(WAR.RawIntuition)
                            && localPlayerPercentage <= 0.80
                            && actionID is WAR.Overpower
                            && !HasEffect(WAR.Buffs.Holmgang):
                        return OriginalHook(WAR.RawIntuition);
                }
            }

            if (
                level >= WAR.Levels.InnerBeast
                && (surgingTempest is not null || level < WAR.Levels.StormsEye)
                && (
                    gauge.BeastGauge >= 90
                    || (
                        gauge.BeastGauge >= 50
                        && (
                            needToInfuriate
                            // || (actionID is WAR.Overpower && !IsMoving)
                            || raidbuffs
                            || HasEffect(WAR.Buffs.Berserk)
                        )
                    )
                    || HasEffect(WAR.Buffs.InnerRelease)
                )
            )
            {
                return level >= WAR.Levels.SteelCyclone && actionID is WAR.Overpower
                    ? OriginalHook(WAR.SteelCyclone)
                    : OriginalHook(WAR.InnerBeast);
            }

            if (comboTime > 0)
            {
                if (actionID == WAR.HeavySwing)
                {
                    if (lastComboMove == WAR.Maim && level >= WAR.Levels.StormsPath)
                    {
                        if (
                            level >= WAR.Levels.StormsEye
                            && (surgingTempest is null || surgingTempest.RemainingTime < 20)
                        )
                        {
                            return WAR.StormsEye;
                        }

                        return WAR.StormsPath;
                    }

                    if (lastComboMove == WAR.HeavySwing && level >= WAR.Levels.Maim)
                        return WAR.Maim;
                }

                if (actionID is WAR.Overpower && lastComboMove == WAR.Overpower)
                {
                    if (level >= WAR.Levels.MythrilTempest)
                        return WAR.MythrilTempest;
                }
            }

            return actionID;
        }

        return actionID;
    }
}
