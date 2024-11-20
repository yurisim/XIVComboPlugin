using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class PLD
{
    public const byte ClassID = 1;
    public const byte JobID = 19;

    public const uint FastBlade = 9,
        RiotBlade = 15,
        ShieldBash = 16,
        FightOrFlight = 20,
        RageOfHalone = 21,
        CircleOfScorn = 23,
        ShieldLob = 24,
        IronWill = 28,
        SpiritsWithin = 29,
        Sheltron = 3542,
        GoringBlade = 3538,
        RoyalAuthority = 3539,
        Clemency = 3541,
        TotalEclipse = 7381,
        Requiescat = 7383,
        HolySpirit = 7384,
        LowBlow = 7540,
        Prominence = 16457,
        HolyCircle = 16458,
        Confiteor = 16459,
        Intervene = 16461,
        Atonement = 16460,
        Expiacion = 25747,
        BladeOfFaith = 25748,
        BladeOfTruth = 25749,
        BladeOfValor = 25750,
        IronWillRemoval = 32065,
        Supplication = 36918,
        Sepulchre = 36919,
        Imperator = 36921,
        BladeOfHonor = 36922;

    public static class Buffs
    {
        public const ushort FightOrFlight = 76,
            IronWill = 79,
            Requiescat = 1368,
            DivineMight = 2673,
            GoringBladeReady = 3847,
            AtonementReady = 1902,
            SupplicationReady = 3827,
            SepulchreReady = 3828,
            ConfiteorReady = 3019,
            BladeOfHonorReady = 3831;
    }

    public static class Debuffs
    {
        public const ushort GoringBlade = 725;
    }

    public static class Levels
    {
        public const byte FightOrFlight = 2,
            RiotBlade = 4,
            IronWill = 10,
            LowBlow = 12,
            SpiritsWithin = 30,
            Sheltron = 35,
            CircleOfScorn = 50,
            RageOfHalone = 26,
            Prominence = 40,
            GoringBlade = 54,
            RoyalAuthority = 60,
            HolySpirit = 64,
            DivineMagicMastery = 64,
            Requiescat = 68,
            HolyCircle = 72,
            Intervene = 66,
            Atonement = 76,
            Supplication = 76,
            ShieldLob = 15,
            Sepulchre = 76,
            Confiteor = 80,
            Expiacion = 86,
            BladeOfFaith = 90,
            BladeOfHonor = 100;
    }
}

internal class PaladinST : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PldAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID is PLD.FastBlade or PLD.TotalEclipse)
        {
            var fightOrFlightCD = GetCooldown(PLD.FightOrFlight).CooldownRemaining;

            var hasRaidBuffs = HasRaidBuffs(2);

            var goringBladeReady = FindEffect(PLD.Buffs.GoringBladeReady);
            var flightOrFight = FindEffect(PLD.Buffs.FightOrFlight);

            var gauge = GetJobGauge<PLDGauge>();

            var canUseAtonement =
                level >= PLD.Levels.Atonement && CanUseAction(OriginalHook(PLD.Atonement));

            var distance = GetTargetDistance();

            if (GCDClipCheck(actionID) && HasTarget())
            {
                switch (level)
                {
                    case >= PLD.Levels.FightOrFlight
                        when IsOffCooldown(PLD.FightOrFlight)
                            && (
                                (
                                    level < PLD.Levels.RoyalAuthority
                                    && lastComboMove == PLD.RiotBlade
                                )
                                || (
                                    level >= PLD.Levels.RoyalAuthority
                                    && lastComboMove == PLD.RoyalAuthority
                                )
                                || level < PLD.Levels.Prominence
                                    && lastComboMove == PLD.TotalEclipse
                                || level >= PLD.Levels.Prominence && lastComboMove == PLD.Prominence
                                || (canUseAtonement && !HasEffect(PLD.Buffs.SepulchreReady))
                                || hasRaidBuffs
                            ):
                        return PLD.FightOrFlight;
                    case >= PLD.Levels.Requiescat
                        when IsOffCooldown(OriginalHook(PLD.Requiescat))
                            && (HasEffect(PLD.Buffs.FightOrFlight) || hasRaidBuffs):
                        return OriginalHook(PLD.Requiescat);
                    case >= PLD.Levels.CircleOfScorn
                        when IsOffCooldown(PLD.CircleOfScorn)
                            && HasTarget()
                            && distance <= 5
                            && (
                                flightOrFight is not null || fightOrFlightCD >= 7.5 || hasRaidBuffs
                            ):
                        return PLD.CircleOfScorn;
                    case >= PLD.Levels.SpiritsWithin
                        when IsOffCooldown(OriginalHook(PLD.SpiritsWithin))
                            && distance <= 5
                            && (
                                flightOrFight is not null || fightOrFlightCD >= 7.5 || hasRaidBuffs
                            ):
                        return OriginalHook(PLD.SpiritsWithin);
                    case >= PLD.Levels.Sheltron
                        when IsOffCooldown(PLD.Sheltron)
                            && actionID is PLD.TotalEclipse
                            && gauge.OathGauge == 100:
                        return OriginalHook(PLD.Sheltron);
                }
            }

            if (
                level >= PLD.Levels.GoringBlade
                && goringBladeReady is not null
                && distance <= 5
                && actionID is PLD.FastBlade
                && (GetCooldown(PLD.FightOrFlight).CooldownElapsed >= 5 || hasRaidBuffs)
            )
                return PLD.GoringBlade;

            var divineMight = FindEffect(PLD.Buffs.DivineMight);

            if (
                level >= PLD.Levels.HolySpirit
                && LocalPlayer?.CurrentMp > 1000
                && (
                    HasEffect(PLD.Buffs.Requiescat)
                    || (
                        divineMight is not null
                        && (
                            divineMight.RemainingTime <= 6
                            || distance > 5
                            || lastComboMove == PLD.RiotBlade
                            || lastComboMove == PLD.Prominence
                        )
                    )
                )
            )
            {
                if (level >= PLD.Levels.Confiteor && CanUseAction(OriginalHook(PLD.Confiteor)))
                {
                    return OriginalHook(PLD.Confiteor);
                }

                if (level >= PLD.Levels.HolyCircle && actionID is PLD.TotalEclipse)
                {
                    return PLD.HolyCircle;
                }

                return PLD.HolySpirit;
            }

            if (canUseAtonement && distance <= 5 && actionID is PLD.FastBlade)
            {
                return OriginalHook(PLD.Atonement);
            }

            if (comboTime > 0)
            {
                if (actionID is PLD.FastBlade && distance <= 5)
                {
                    if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.RageOfHalone)
                        return OriginalHook(PLD.RageOfHalone);

                    if (lastComboMove == PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                        return PLD.RiotBlade;

                    return PLD.FastBlade;
                }

                if (actionID is PLD.TotalEclipse)
                {
                    if (lastComboMove == PLD.TotalEclipse && level >= PLD.Levels.Prominence)
                        return PLD.Prominence;

                    return PLD.TotalEclipse;
                }
            }

            if (distance > 5)
            {
                if (level >= PLD.Levels.HolySpirit && LocalPlayer?.CurrentMp > 5000 && !IsMoving)
                {
                    return PLD.HolySpirit;
                }

                if (level >= PLD.Levels.ShieldLob)
                    return PLD.ShieldLob;
            }
        }

        return actionID;
    }
}

internal class PaladinHolySpirit : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.PaladinHolySpiritLevelSyncFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.HolySpirit && level < PLD.Levels.HolySpirit)
            return PLD.ShieldLob;
        return actionID;
    }
}
