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
        SpiritsWithin = 29,
        Sheltron = 3542,
        GoringBlade = 3538,
        RoyalAuthority = 3539,
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
            Requiescat = 1368,
            SwordOath = 1902,
            BladeOfFaithReady = 3019;
    }

    public static class Debuffs
    {
        public const ushort
            GoringBlade = 725;
    }

    public static class Levels
    {
        public const byte
            RiotBlade = 4,
            LowBlow = 12,
            SpiritsWithin = 30,
            Sheltron = 35,
            CircleOfScorn = 50,
            RageOfHalone = 26,
            Prominence = 40,
            GoringBlade = 54,
            RoyalAuthority = 60,
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

internal class PaladinGoringBlade : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PldAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.GoringBlade)
        {
            if (IsEnabled(CustomComboPreset.PaladinGoringBladeAtonementFeature))
            {
                if (level >= PLD.Levels.Atonement && HasEffect(PLD.Buffs.SwordOath) && lastComboMove != PLD.FastBlade && lastComboMove != PLD.RiotBlade)
                    return PLD.Atonement;
            }

            if (level >= PLD.Levels.CircleOfScorn
                && IsOffCooldown(PLD.CircleOfScorn)
                && GCDClipCheck(actionID))
            {

                if (IsOffCooldown(PLD.FightOrFlight) && GCDClipCheck(actionID))
                {
                    return PLD.FightOrFlight;
                }

                return PLD.CircleOfScorn;
            }

            if (level >= PLD.Levels.SpiritsWithin
                && IsOffCooldown(PLD.SpiritsWithin)
                && GCDClipCheck(actionID))
            {

                if (IsOffCooldown(PLD.FightOrFlight) && GCDClipCheck(actionID))
                {
                    return PLD.FightOrFlight;
                }

                return PLD.SpiritsWithin;
            }

            var goringBlade = FindTargetEffect(PLD.Debuffs.GoringBlade);

            if (level >= PLD.Levels.Requiescat
                && IsOffCooldown(PLD.Requiescat)
                && GCDClipCheck(actionID))
            {

                if (IsOffCooldown(PLD.FightOrFlight) && GCDClipCheck(actionID))
                {
                    return PLD.FightOrFlight;
                }

                return PLD.Requiescat;
            }

            if (HasEffect(PLD.Buffs.Requiescat) 
                && goringBlade != null)
            {
                return PLD.HolySpirit;
            }

            if (comboTime > 0)
            {

                if (lastComboMove == PLD.RiotBlade
                    && level >= PLD.Levels.GoringBlade
                    && (goringBlade == null || goringBlade?.RemainingTime <= 5))
                {

                    if (IsOffCooldown(PLD.FightOrFlight) && GCDClipCheck(actionID))
                    {
                        return PLD.FightOrFlight;
                    }

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

internal class PaladinRoyalAuthority : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PldAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.RageOfHalone || actionID == PLD.RoyalAuthority)
        {
            if (IsEnabled(CustomComboPreset.PaladinRoyalAuthorityAtonementFeature))
            {
                if (level >= PLD.Levels.Atonement && HasEffect(PLD.Buffs.SwordOath) && lastComboMove != PLD.FastBlade && lastComboMove != PLD.RiotBlade)
                    return PLD.Atonement;
            }

            if (IsEnabled(CustomComboPreset.PaladinRoyalAuthorityCombo))
            {
                if (comboTime > 0)
                {
                    if (lastComboMove == PLD.RiotBlade && level >= PLD.Levels.RageOfHalone)
                        // Royal Authority
                        return OriginalHook(PLD.RageOfHalone);

                    if (lastComboMove == PLD.FastBlade && level >= PLD.Levels.RiotBlade)
                        return PLD.RiotBlade;
                }

                return PLD.FastBlade;
            }
        }

        return actionID;
    }
}

internal class PaladinProminence : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinProminenceCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.Prominence)
        {
            if (level >= PLD.Levels.CircleOfScorn
                && IsOffCooldown(PLD.CircleOfScorn)
                && GCDClipCheck(actionID))
            {

                if (IsOffCooldown(PLD.FightOrFlight) && GCDClipCheck(actionID))
                {
                    return PLD.FightOrFlight;
                }

                return PLD.CircleOfScorn;
            }

            if (level >= PLD.Levels.SpiritsWithin
                && IsOffCooldown(PLD.SpiritsWithin)
                && GCDClipCheck(actionID))
            {

                if (IsOffCooldown(PLD.FightOrFlight) && GCDClipCheck(actionID))
                {
                    return PLD.FightOrFlight;
                }

                return PLD.SpiritsWithin;
            }

            if (level >= PLD.Levels.Requiescat
                && IsOffCooldown(PLD.Requiescat)
                && GCDClipCheck(actionID))
            {

                if (IsOffCooldown(PLD.FightOrFlight) && GCDClipCheck(actionID))
                {
                    return PLD.FightOrFlight;
                }
                
                return PLD.Requiescat;
            }

            var gauge = GetJobGauge<PLDGauge>();


            if (level >= PLD.Levels.Sheltron
                && gauge.OathGauge == 100
                && GCDClipCheck(actionID))
            {
                return PLD.Sheltron;
            }

            if (HasEffect(PLD.Buffs.Requiescat) && level >= PLD.Levels.HolyCircle)
            {
                return PLD.HolyCircle;
            }

            if (comboTime > 0)
            {
                if (lastComboMove == PLD.TotalEclipse && level >= PLD.Levels.Prominence)
                    return PLD.Prominence;
            }

            return PLD.TotalEclipse;
        }

        return actionID;
    }
}

internal class PaladinHolySpiritHolyCircle : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinConfiteorFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.HolySpirit || actionID == PLD.HolyCircle)
        {
            if (lastComboMove == PLD.BladeOfTruth && level >= PLD.Levels.BladeOfValor)
                return PLD.BladeOfValor;

            if (lastComboMove == PLD.BladeOfFaith && level >= PLD.Levels.BladeOfTruth)
                return PLD.BladeOfTruth;

            if (level >= PLD.Levels.BladeOfFaith && HasEffect(PLD.Buffs.BladeOfFaithReady))
                return PLD.BladeOfFaith;

            if (level >= PLD.Levels.Confiteor)
            {
                var requiescat = FindEffect(PLD.Buffs.Requiescat);
                if (requiescat != null && (requiescat.StackCount == 1 || LocalPlayer?.CurrentMp < 2000))
                    return PLD.Confiteor;
            }
        }

        return actionID;
    }
}

internal class PaladinRequiescat : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PaladinRequiescatCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == PLD.Requiescat)
        {
            if (lastComboMove == PLD.BladeOfTruth && level >= PLD.Levels.BladeOfValor)
                return PLD.BladeOfValor;

            if (lastComboMove == PLD.BladeOfFaith && level >= PLD.Levels.BladeOfTruth)
                return PLD.BladeOfTruth;

            if (level >= PLD.Levels.BladeOfFaith && HasEffect(PLD.Buffs.BladeOfFaithReady))
                return PLD.BladeOfFaith;

            if (level >= PLD.Levels.Confiteor)
            {
                var requiescat = FindEffect(PLD.Buffs.Requiescat);
                if (requiescat != null)
                    return PLD.Confiteor;
            }
        }

        return actionID;
    }
}

internal class PaladinSpiritsWithinCircleOfScorn : CustomCombo
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

internal class PaladinShieldBash : CustomCombo
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
