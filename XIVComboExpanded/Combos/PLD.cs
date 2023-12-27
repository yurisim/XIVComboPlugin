using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class PLD
{
    public const byte ClassID = 1;
    public const byte JobID = 19;

    public const uint
        FastBlade = 9,
        RiotBlade = 15,
        ShieldBash = 16,
        FightOrFlight = 20,
        RageOfHalone = 21,
        CircleOfScorn = 23,
        ShieldLob = 24,
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
        Atonement = 16460,
        Expiacion = 25747,
        BladeOfFaith = 25748,
        BladeOfTruth = 25749,
        BladeOfValor = 25750;

    public static class Buffs
    {
        public const ushort
            FightOrFlight = 76,
            Requiescat = 1368,
            SwordOath = 1902,
            DivineMight = 2673,
            ConfiteorReady = 3019;
    }

    public static class Debuffs
    {
        public const ushort
            GoringBlade = 725;
    }

    public static class Levels
    {
        public const byte
            FightOrFlight = 2,
            RiotBlade = 4,
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
            Atonement = 76,
            Confiteor = 80,
            Expiacion = 86,
            BladeOfFaith = 90,
            BladeOfTruth = 90,
            BladeOfValor = 90;
    }
}

internal abstract class PaladinCombo : CustomCombo
{
    protected bool HasMp(uint spell)
    {
        if (actionID == PLD.FastBlade)
        {
            //if (IsEnabled(CustomComboPreset.PaladinGoringBladeAtonementFeature))
            //{
            //    if (level >= PLD.Levels.Atonement && HasEffect(PLD.Buffs.SwordOath) && lastComboMove != PLD.FastBlade && lastComboMove != PLD.RiotBlade)
            //        return PLD.Atonement;
            //}

            var goringBlade = FindTargetEffect(PLD.Debuffs.GoringBlade);

            var fightOrFlightCD = GetCooldown(PLD.FightOrFlight).CooldownRemaining;

            if (GCDClipCheck(actionID))
            {
                if (IsOffCooldown(PLD.FightOrFlight)
                    && (level < PLD.Levels.GoringBlade 
                        || lastComboMove == PLD.RiotBlade
                        || TargetHasEffect(PLD.Debuffs.GoringBlade)))
                {
                    return PLD.FightOrFlight;
                }

                if (level >= PLD.Levels.Requiescat
                    && IsOffCooldown(PLD.Requiescat)
                    && (HasEffect(PLD.Buffs.FightOrFlight) || fightOrFlightCD >= 15))
                {
                    return PLD.Requiescat;
                }

                if (level >= PLD.Levels.CircleOfScorn
                    && IsOffCooldown(PLD.CircleOfScorn)
                    && HasTarget()
                    && GetTargetDistance() <= 5
                    && (HasEffect(PLD.Buffs.FightOrFlight) || fightOrFlightCD >= 7.5))
                {
                    return PLD.CircleOfScorn;
                }

                if (level >= PLD.Levels.SpiritsWithin
                    && IsOffCooldown(PLD.SpiritsWithin)
                    && (HasEffect(PLD.Buffs.FightOrFlight) || fightOrFlightCD >= 7.5))
                {
                    return OriginalHook(PLD.SpiritsWithin);
                }
            }

            if (HasEffect(PLD.Buffs.Requiescat) 
                && goringBlade is not null)
            {
                return PLD.HolySpirit;
            }

            if (comboTime > 0 && InMeleeRange())
            {

                if (lastComboMove == PLD.RiotBlade
                    && level >= PLD.Levels.GoringBlade
                    && (goringBlade is null || (goringBlade is not null && goringBlade.RemainingTime <= 5)))
                {
                    return PLD.GoringBlade;
                }

                if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.RageOfHalone)
                    // Royal Authority
                    return OriginalHook(PLD.RageOfHalone);

                if (lastComboMove == PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                    return PLD.RiotBlade;
            }

            return PLD.FastBlade;
        }

        return actionID;
    }
}

internal class PaladinProminence : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PldAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.TotalEclipse)
        {
            var fightOrFlightCD = GetCooldown(PLD.FightOrFlight).CooldownRemaining;

            if (GCDClipCheck(actionID))
            {
                if (IsOffCooldown(PLD.FightOrFlight)
                    && (lastComboMove == PLD.TotalEclipse 
                        || HasEffect(PLD.Buffs.Requiescat)))
                {
                    return PLD.FightOrFlight;
                }

                if (level >= PLD.Levels.Requiescat
                    && IsOffCooldown(PLD.Requiescat)
                    && (HasEffect(PLD.Buffs.FightOrFlight) || fightOrFlightCD >= 15))
                {
                    return PLD.Requiescat;
                }

                if (level >= PLD.Levels.CircleOfScorn
                    && IsOffCooldown(PLD.CircleOfScorn)
                    && HasTarget()
                    && GetTargetDistance() <= 5
                    && (HasEffect(PLD.Buffs.FightOrFlight) || fightOrFlightCD >= 7.5))
                {
                    return PLD.CircleOfScorn;
                }

                if (level >= PLD.Levels.SpiritsWithin
                    && IsOffCooldown(PLD.SpiritsWithin)
                    && (HasEffect(PLD.Buffs.FightOrFlight) || fightOrFlightCD >= 7.5))
                {
                    return OriginalHook(PLD.SpiritsWithin);
                }

                var gauge = GetJobGauge<PLDGauge>();

                if (level >= PLD.Levels.Sheltron
                    && gauge.OathGauge == 100)
                {
                    return PLD.Sheltron;
                }
            }

            if (HasEffect(PLD.Buffs.Requiescat) 
                && level >= PLD.Levels.HolyCircle 
                && GetTargetDistance() <= 5)
            {
                return PLD.HolyCircle;
            }

            if (comboTime > 0)
            {
                if (lastComboMove == PLD.TotalEclipse && level >= PLD.Levels.Prominence)
                {
                    if (IsEnabled(CustomComboPreset.PaladinProminenceDivineMightFeature))
                    {
                        if (HasEffect(PLD.Buffs.DivineMight))
                        {
                            if (level >= PLD.Levels.HolyCircle && this.HasMp(PLD.HolyCircle))
                                return PLD.HolyCircle;
                        }
                    }

                    return PLD.Prominence;
                }
            }

            return PLD.TotalEclipse;
        }

        return actionID;
    }
}

internal class PaladinHolySpiritHolyCircle : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinConfiteorFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.HolySpirit || actionID == PLD.HolyCircle)
        {
            if (level >= PLD.Levels.Confiteor)
            {
                var original = OriginalHook(PLD.Confiteor);
                if (original != PLD.Confiteor)
                    return original;

                if (HasEffect(PLD.Buffs.ConfiteorReady))
                    return PLD.Confiteor;
            }
        }

        return actionID;
    }
}

internal class PaladinHolySpirit : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinHolySpiritLevelSyncFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.HolySpirit && level < PLD.Levels.HolySpirit)
          return PLD.ShieldLob;
        return actionID;
    }
}

internal class PaladinFightOrFlight : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PldAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.FightOrFlight)
        {
            if (IsEnabled(CustomComboPreset.PaladinFightOrFlightGoringBladeFeature))
            {
                if (level >= PLD.Levels.GoringBlade && HasEffect(PLD.Buffs.FightOrFlight) && IsOffCooldown(PLD.GoringBlade))
                    return PLD.GoringBlade;
            }
        }

        return actionID;
    }
}

internal class PaladinRequiescat : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PldAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.Requiescat)
        {
            if (IsEnabled(CustomComboPreset.PaladinRequiescatCombo))
            {
                // Prioritize Goring Blade over the Confiteor combo.  While Goring Blade deals less damage (700p) than
                // most of the Confiteor combo (900p -> 700p -> 800p -> 900p), Goring Blade uniquely requires melee
                // range to cast, while the entire Confiteor combo chain does not.  Since Requiescat also requires
                // melee range to cast, the most reliable time that the player will be in melee range during the Req
                // buff is immediately following the usage of Req.  This minimizes potential losses and potential
                // cooldown drift if the player is forced out of melee range during the Confiteor combo and is unable
                // to return to melee range by the time it is completed.
                //
                // Since Goring Blade, the entire Confiteor combo, *and* one additional GCD (typically Holy Spirit) fits
                // within even the shortest of party buffs (15s ones like Battle Litany), this should not result in a
                // net reduction in potency, and *may* in fact increase it if someone is slightly late in applying
                // their party buffs, as it shifts the high-potency Confiteor cast back into the party buff window by a
                // single GCD.
                if (IsEnabled(CustomComboPreset.PaladinRequiescatFightOrFlightFeature) && IsEnabled(CustomComboPreset.PaladinFightOrFlightGoringBladeFeature))
                {
                    if (level >= PLD.Levels.GoringBlade && IsOffCooldown(PLD.GoringBlade))
                    {
                        if (IsOnCooldown(PLD.FightOrFlight) && (level < PLD.Levels.Requiescat || IsOnCooldown(PLD.Requiescat)))
                            return PLD.GoringBlade;
                    }
                }

                if (level >= PLD.Levels.Confiteor)
                {
                    // Blade combo
                    var original = OriginalHook(PLD.Confiteor);
                    if (original != PLD.Confiteor)
                        return original;

                    if (HasEffect(PLD.Buffs.ConfiteorReady))
                        return PLD.Confiteor;
                }

                if (level >= PLD.Levels.Requiescat && HasEffect(PLD.Buffs.Requiescat))
                    return PLD.HolySpirit;
            }

            if (IsEnabled(CustomComboPreset.PaladinRequiescatFightOrFlightFeature))
            {
                if (level >= PLD.Levels.FightOrFlight)
                {
                    if (level < PLD.Levels.Requiescat)
                        return PLD.FightOrFlight;

                    // Prefer FoF if it is off cooldown, or if it will be ready sooner than Requiescat.  In practice, this
                    // means that Req should only be returned if FoF is on cooldown and Req is not, ie. immediately after
                    // FoF is cast.  This ensures that the button shows the action that will next be available for use in
                    // that hotbar slot, rather than swapping to FoF at the last instant when FoF comes off cooldown a
                    // a single weave slot earlier than Req.
                    return CalcBestAction(PLD.FightOrFlight, PLD.FightOrFlight, PLD.Requiescat);
                }
            }
        }

        return actionID;
    }
}

internal class PaladinSpiritsWithinCircleOfScorn : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinScornfulSpiritsFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.SpiritsWithin || actionID == PLD.Expiacion || actionID == PLD.CircleOfScorn)
        {
            if (level >= PLD.Levels.Expiacion)
                return CalcBestAction(actionID, PLD.Expiacion, PLD.CircleOfScorn);

            if (level >= PLD.Levels.CircleOfScorn)
                return CalcBestAction(actionID, PLD.SpiritsWithin, PLD.CircleOfScorn);

            return PLD.SpiritsWithin;
        }

        return actionID;
    }
}

internal class PaladinShieldBash : PaladinCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinShieldBashFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.ShieldBash)
        {
            if (level >= PLD.Levels.LowBlow && IsOffCooldown(PLD.LowBlow))
                return PLD.LowBlow;
        }

        return actionID;
    }
}
