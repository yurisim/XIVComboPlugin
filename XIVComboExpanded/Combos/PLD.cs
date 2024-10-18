using Dalamud.Game.ClientState.JobGauge.Types;
using Lumina.Excel.GeneratedSheets;

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
            Intervene = 74,
            Atonement = 76,
            Supplication = 76,
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

            var hasRaidBuffs = HasRaidBuffs(1);

            var goringBladeReady = FindEffect(PLD.Buffs.GoringBladeReady);
            var flightOrFight = FindEffect(PLD.Buffs.FightOrFlight);

            var gauge = GetJobGauge<PLDGauge>();

            var canUseAtonement =
                level >= PLD.Levels.Atonement && CanUseAction(OriginalHook(PLD.Atonement));

            var inMeleeRange = InMeleeRange();

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
                                || canUseAtonement
                                || hasRaidBuffs
                            ):
                        return PLD.FightOrFlight;
                    case >= PLD.Levels.Requiescat
                        when IsOffCooldown(OriginalHook(PLD.Requiescat))
                            && (flightOrFight is not null || fightOrFlightCD >= 15 || hasRaidBuffs):
                        return OriginalHook(PLD.Requiescat);
                    case >= PLD.Levels.CircleOfScorn
                        when IsOffCooldown(PLD.CircleOfScorn)
                            && HasTarget()
                            && GetTargetDistance() <= 5
                            && (
                                flightOrFight is not null || fightOrFlightCD >= 7.5 || hasRaidBuffs
                            ):
                        return PLD.CircleOfScorn;
                    case >= PLD.Levels.SpiritsWithin
                        when IsOffCooldown(OriginalHook(PLD.SpiritsWithin))
                            && inMeleeRange
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
                && inMeleeRange
                && actionID is PLD.FastBlade
                && (GetCooldown(PLD.FightOrFlight).CooldownElapsed >= 5 || hasRaidBuffs)
            )
                return PLD.GoringBlade;

            if (
                level >= PLD.Levels.HolySpirit
                && (
                    HasEffect(PLD.Buffs.Requiescat)
                    || (
                        HasEffect(PLD.Buffs.DivineMight)
                        && (
                            !inMeleeRange
                            || lastComboMove == PLD.RiotBlade
                            || lastComboMove == PLD.Prominence
                        )
                    )
                )
            )
            {
                if (level >= PLD.Levels.Confiteor && HasEffect(PLD.Buffs.ConfiteorReady))
                {
                    return OriginalHook(PLD.Confiteor);
                }

                if (level >= PLD.Levels.HolyCircle && actionID is PLD.TotalEclipse)
                {
                    return PLD.HolyCircle;
                }

                return PLD.HolySpirit;
            }

            if (canUseAtonement && inMeleeRange && actionID is PLD.FastBlade)
            {
                return OriginalHook(PLD.Atonement);
            }

            if (comboTime > 0)
            {
                if (actionID is PLD.FastBlade && inMeleeRange)
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

//internal class PaladinFightOrFlight : PaladinCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PldAny;

//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == PLD.FightOrFlight)
//        {
//            if (IsEnabled(CustomComboPreset.PaladinFightOrFlightGoringBladeFeature))
//            {
//                if (
//                    level >= PLD.Levels.GoringBlade
//                    && HasEffect(PLD.Buffs.FightOrFlight)
//                    && IsOffCooldown(PLD.GoringBlade)
//                )
//                    return PLD.GoringBlade;
//            }
//        }

//        return actionID;
//    }
//}

//internal class PaladinRequiescat : PaladinCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PldAny;

//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == PLD.Requiescat)
//        {
//            if (IsEnabled(CustomComboPreset.PaladinRequiescatCombo))
//            {
//                // Prioritize Goring Blade over the Confiteor combo.  While Goring Blade deals less damage (700p) than
//                // most of the Confiteor combo (900p -> 700p -> 800p -> 900p), Goring Blade uniquely requires melee
//                // range to cast, while the entire Confiteor combo chain does not.  Since Requiescat also requires
//                // melee range to cast, the most reliable time that the player will be in melee range during the Req
//                // buff is immediately following the usage of Req.  This minimizes potential losses and potential
//                // cooldown drift if the player is forced out of melee range during the Confiteor combo and is unable
//                // to return to melee range by the time it is completed.
//                //
//                // Since Goring Blade, the entire Confiteor combo, *and* one additional GCD (typically Holy Spirit) fits
//                // within even the shortest of party buffs (15s ones like Battle Litany), this should not result in a
//                // net reduction in potency, and *may* in fact increase it if someone is slightly late in applying
//                // their party buffs, as it shifts the high-potency Confiteor cast back into the party buff window by a
//                // single GCD.
//                if (
//                    IsEnabled(CustomComboPreset.PaladinRequiescatFightOrFlightFeature)
//                    && IsEnabled(CustomComboPreset.PaladinFightOrFlightGoringBladeFeature)
//                )
//                {
//                    if (level >= PLD.Levels.GoringBlade && IsOffCooldown(PLD.GoringBlade))
//                    {
//                        if (
//                            IsOnCooldown(PLD.FightOrFlight)
//                            && (level < PLD.Levels.Requiescat || IsOnCooldown(PLD.Requiescat))
//                        )
//                            return PLD.GoringBlade;
//                    }
//                }

//                if (IsEnabled(CustomComboPreset.PaladinRequiescatCombo))
//                {
//                    if (level >= PLD.Levels.Confiteor)
//                    {
//                        // Blade combo
//                        var original = OriginalHook(PLD.Confiteor);
//                        if (original != PLD.Confiteor)
//                            return original;

//                        if (HasEffect(PLD.Buffs.BladeOfHonorReady))
//                            return OriginalHook(PLD.Imperator);

//                        if (HasEffect(PLD.Buffs.ConfiteorReady))
//                            return OriginalHook(PLD.Confiteor);
//                    }

//                    // This should only occur if the user is below the level for the full 4-part Confiteor combo (level 90), as after that level, all 4
//                    // stacks of Requiescat will be consumed by the Confiteor combo.
//                    if (level >= PLD.Levels.Requiescat && HasEffect(PLD.Buffs.Requiescat))
//                        return PLD.HolySpirit;
//                }

//                if (IsEnabled(CustomComboPreset.PaladinRequiescatFightOrFlightFeature))
//                {
//                    if (level >= PLD.Levels.FightOrFlight)
//                    {
//                        if (level < PLD.Levels.Requiescat)
//                            return PLD.FightOrFlight;

//                        // Prefer FoF if it is off cooldown, or if it will be ready sooner than Requiescat.  In practice, this
//                        // means that Req should only be returned if FoF is on cooldown and Req is not, ie. immediately after
//                        // FoF is cast.  This ensures that the button shows the action that will next be available for use in
//                        // that hotbar slot, rather than swapping to FoF at the last instant when FoF comes off cooldown a
//                        // a single weave slot earlier than Req.
//                        return CalcBestAction(PLD.FightOrFlight, PLD.FightOrFlight, PLD.Requiescat);
//                    }
//                }
//            }

//            return actionID;
//        }
//    }
//}
