using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class SMN
{
    public const byte ClassID = 26;
    public const byte JobID = 27;

    public const uint
        Ruin = 163,
        Ruin2 = 172,
        Ruin3 = 3579,
        Ruin4 = 7426,
        Fester = 181,
        SummonRuby = 25802,
        SummonTopaz = 25803,
        SummonEmerald = 25804,
        Painflare = 3578,
        DreadwyrmTrance = 3581,
        SummonBahamut = 7427,
        EnkindleBahamut = 7429,
        EnergySyphon = 16510,
        Outburst = 16511,
        EnergyDrain = 16508,
        SummonCarbuncle = 25798,
        RadiantAegis = 25799,
        Aethercharge = 25800,
        SearingLight = 25801,
        AstralFlow = 25822,
        TriDisaster = 25826,
        CrimsonCyclone = 25835,
        MountainBuster = 25836,
        Slipstream = 25837,
        CrimsonStrike = 25885,
        Gemshine = 25883,
        PreciousBrilliance = 25884;

    public static class Buffs
    {
        public const ushort
            FurtherRuin = 2701,
            RadiantAegis = 3224,
            SearingLight = 2703,
            IfritsFavor = 2724,
            GarudasFavor = 2725,
            TitansFavor = 2853;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            SummonCarbuncle = 2,
            RadiantAegis = 2,
            Gemshine = 6,
            EnergyDrain = 10,
            Fester = 10,
            SummonEmerald = 22,
            PreciousBrilliance = 26,
            Painflare = 40,
            EnergySyphon = 52,
            Ruin3 = 54,
            AstralFlow = 60,
            Ruin4 = 62,
            SearingLight = 66,
            EnkindleBahamut = 70,
            Rekindle = 80,
            ElementalMastery = 86,
            SummonPhoenix = 80;
    }
}

internal class SummonerFester : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerEDFesterFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SMN.Fester)
        {
            var gauge = GetJobGauge<SMNGauge>();

            if (level >= SMN.Levels.EnergyDrain && !gauge.HasAetherflowStacks)
                return SMN.EnergyDrain;
        }

        return actionID;
    }
}

internal class SummonerPainflare : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerESPainflareFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SMN.Painflare)
        {
            var gauge = GetJobGauge<SMNGauge>();

            if (level >= SMN.Levels.EnergySyphon && !gauge.HasAetherflowStacks)
                return SMN.EnergySyphon;

            if (level >= SMN.Levels.EnergyDrain && !gauge.HasAetherflowStacks)
                return SMN.EnergyDrain;

            if (level < SMN.Levels.Painflare)
                return SMN.Fester;
        }

        return actionID;
    }
}

internal class SummonerRuin : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SmnAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SMN.Ruin 
            || actionID == SMN.Ruin2 
            || actionID == SMN.Outburst 
            || actionID == SMN.TriDisaster)
        {
            var gauge = GetJobGauge<SMNGauge>();

            var hasSearing = HasEffect(SMN.Buffs.SearingLight);

            var searingCD = GetCooldown(SMN.SearingLight).CooldownRemaining;

            var searingElapsed = GetCooldown(SMN.SearingLight).CooldownElapsed;

            if (GCDClipCheck(actionID))
            {
                if (CanUseAction(OriginalHook(SMN.EnkindleBahamut))
                    && ((hasSearing && searingElapsed > 5) || searingCD >= 10 || level < SMN.Levels.SearingLight)
                    && IsOffCooldown(OriginalHook(SMN.EnkindleBahamut)))
                {
                    return OriginalHook(SMN.EnkindleBahamut);
                }

                if (OriginalHook(SMN.AstralFlow) != SMN.AstralFlow
                    && ((hasSearing && searingElapsed > 5) || searingCD >= 10 || level < SMN.Levels.SearingLight)
                    && !gauge.IsGarudaAttuned && !gauge.IsIfritAttuned
                    && IsOffCooldown(OriginalHook(SMN.AstralFlow)))
                {
                    return OriginalHook(SMN.AstralFlow);
                }

                if (IsOffCooldown(SMN.EnergyDrain)
                    && ((hasSearing && searingElapsed > 5) || searingCD >= 2 || level < SMN.Levels.SearingLight))
                {
                    return level >= SMN.Levels.EnergySyphon
                        && (actionID == SMN.Outburst || actionID == SMN.TriDisaster)
                            ? SMN.EnergySyphon
                            : SMN.EnergyDrain;
                }

                if (gauge.HasAetherflowStacks)
                {
                    return level >= SMN.Levels.Painflare 
                        && (actionID == SMN.Outburst || actionID == SMN.TriDisaster) 
                            ? SMN.Painflare 
                            : SMN.Fester;
                }

                if (HasCondition(ConditionFlag.InCombat)
                    && IsOffCooldown(ADV.LucidDreaming)
                    && LocalPlayer?.CurrentMp <= 8000)
                    return ADV.LucidDreaming;

                //if (HasCharges(SMN.RadiantAegis) 
                //    && CanUseAction(SMN.RadiantAegis)
                //    && !HasEffect(SMN.Buffs.RadiantAegis))
                //{
                //    return SMN.RadiantAegis;
                //}
            }

            // Bahamut & Pheonix Summmon
            if (IsOffCooldown(OriginalHook(SMN.Aethercharge)))
            {
                return OriginalHook(SMN.Aethercharge);
            }

            if (level >= SMN.Levels.AstralFlow)
            {
                if (HasEffect(SMN.Buffs.TitansFavor)
                    || (HasEffect(SMN.Buffs.GarudasFavor)
                        && (!IsMoving || HasEffect(ADV.Buffs.Swiftcast))))
                {
                    return OriginalHook(SMN.AstralFlow);
                }

                if ((HasEffect(SMN.Buffs.IfritsFavor) || lastComboMove == SMN.CrimsonCyclone)
                        && InMeleeRange())
                {
                    return OriginalHook(SMN.AstralFlow);
                }
            }

            if (OriginalHook(SMN.Gemshine) != SMN.Gemshine)
            {
                return level >= SMN.Levels.PreciousBrilliance
                    && (actionID == SMN.Outburst || actionID == SMN.TriDisaster)
                        ? OriginalHook(SMN.PreciousBrilliance)
                        : OriginalHook(SMN.Gemshine);
            }

            // RUIN 4
            if (OriginalHook(SMN.Ruin3) != SMN.Ruin3)
            {
                return level >= SMN.Levels.PreciousBrilliance
                    && (actionID == SMN.Outburst || actionID == SMN.TriDisaster)
                        ? OriginalHook(SMN.Outburst)
                        : OriginalHook(SMN.Ruin3);
            }

            if (gauge.IsTitanReady)
            {
                return OriginalHook(SMN.SummonTopaz);
            }

            if (gauge.IsGarudaReady)
            {
                return OriginalHook(SMN.SummonEmerald);
            }

            if (gauge.IsIfritReady)
            {
                if (HasEffect(SMN.Buffs.GarudasFavor) && IsOffCooldown(ADV.Swiftcast)) {
                    return ADV.Swiftcast;
                }

                return OriginalHook(SMN.SummonRuby);
            }

            if (HasEffect(SMN.Buffs.FurtherRuin))
            {
                return SMN.Ruin4;
            }

            return level >= SMN.Levels.PreciousBrilliance
                && (actionID == SMN.Outburst || actionID == SMN.TriDisaster)
                    ? OriginalHook(SMN.Outburst)
                    : OriginalHook(SMN.Ruin);
        }

        return actionID;
    }
}

internal class SummonerOutburstTriDisaster : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SmnAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SMN.Outburst || actionID == SMN.TriDisaster)
        {
            var gauge = GetJobGauge<SMNGauge>();

            if (IsEnabled(CustomComboPreset.SummonerOutburstTitansFavorFeature))
            {
                if (level >= SMN.Levels.ElementalMastery && HasEffect(SMN.Buffs.TitansFavor))
                    return SMN.MountainBuster;
            }

            if (IsEnabled(CustomComboPreset.SummonerOutburstFeature))
            {
                if (level >= SMN.Levels.PreciousBrilliance)
                {
                    if (gauge.IsIfritAttuned || gauge.IsTitanAttuned || gauge.IsGarudaAttuned)
                        return OriginalHook(SMN.PreciousBrilliance);
                }
            }

            if (IsEnabled(CustomComboPreset.SummonerFurtherOutburstFeature))
            {
                if (level >= SMN.Levels.Ruin4 && gauge.SummonTimerRemaining == 0 && gauge.AttunmentTimerRemaining == 0 && HasEffect(SMN.Buffs.FurtherRuin))
                    return SMN.Ruin4;
            }
        }

        return actionID;
    }
}

internal class SummonerGemshinePreciousBrilliance : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SmnAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SMN.Gemshine || actionID == SMN.PreciousBrilliance)
        {
            var gauge = GetJobGauge<SMNGauge>();

            if (IsEnabled(CustomComboPreset.SummonerShinyTitansFavorFeature))
            {
                if (level >= SMN.Levels.ElementalMastery && HasEffect(SMN.Buffs.TitansFavor))
                    return SMN.MountainBuster;
            }

            if (IsEnabled(CustomComboPreset.SummonerShinyEnkindleFeature))
            {
                if (level >= SMN.Levels.EnkindleBahamut && !gauge.IsIfritAttuned && !gauge.IsTitanAttuned && !gauge.IsGarudaAttuned && gauge.SummonTimerRemaining > 0)
                    // Rekindle
                    return OriginalHook(SMN.EnkindleBahamut);
            }

            if (IsEnabled(CustomComboPreset.SummonerFurtherShinyFeature))
            {
                if (level >= SMN.Levels.Ruin4 && gauge.SummonTimerRemaining == 0 && gauge.AttunmentTimerRemaining == 0 && HasEffect(SMN.Buffs.FurtherRuin))
                    return SMN.Ruin4;
            }
        }

        return actionID;
    }
}

internal class SummonerDemiFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SmnAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SMN.Aethercharge || actionID == SMN.DreadwyrmTrance || actionID == SMN.SummonBahamut)
        {
            var gauge = GetJobGauge<SMNGauge>();

            if (IsEnabled(CustomComboPreset.SummonerDemiSearingLightFeature))
            {
                if (level >= SMN.Levels.SearingLight && gauge.IsBahamutReady && InCombat() && IsOffCooldown(SMN.SearingLight))
                    return SMN.SearingLight;
            }

            if (IsEnabled(CustomComboPreset.SummonerDemiEnkindleFeature))
            {
                if (level >= SMN.Levels.EnkindleBahamut && !gauge.IsIfritAttuned && !gauge.IsTitanAttuned && !gauge.IsGarudaAttuned && gauge.SummonTimerRemaining > 0)
                    // Rekindle
                    return OriginalHook(SMN.EnkindleBahamut);
            }
        }

        return actionID;
    }
}

internal class SummonerRadiantCarbundleFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerRadiantCarbuncleFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SMN.RadiantAegis)
        {
            var gauge = GetJobGauge<SMNGauge>();

            if (level >= SMN.Levels.SummonCarbuncle && !HasPetPresent() && gauge.Attunement == 0)
                return SMN.SummonCarbuncle;
        }

        return actionID;
    }
}
