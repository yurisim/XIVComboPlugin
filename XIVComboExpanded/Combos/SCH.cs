using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class SCH
{
    public const byte ClassID = 15;
    public const byte JobID = 28;

    public const uint
        Aetherflow = 166,
        EnergyDrain = 167,
        Ressurection = 173,
        SacredSoil = 188,
        Lustrate = 189,
        Indomitability = 3583,
        DeploymentTactics = 3585,
        EmergencyTactics = 3586,
        Dissipation = 3587,
        Excogitation = 7434,
        ChainStratagem = 7436,
        Aetherpact = 7437,
        WhisperingDawn = 16537,
        FeyIllumination = 16538,
        Biolysis = 16540,
        Recitation = 16542,
        FeyBless = 16543,
        SummonSeraph = 16545,
        Consolation = 16546,
        SummonEos = 17215,
        SummonSelene = 17216,
        ArtOfWar2 = 25866,
        Broil4 = 25865,
        Ruin2 = 17870;

    public static class Buffs
    {
        public const ushort
            Dissipation = 791,
            Recitation = 1896;
    }

    public static class Debuffs
    {
        public const ushort
            Biolysis = 1895;
    }

    public static class Levels
    {
        public const byte
            Ressurection = 12,
            Aetherflow = 45,
            Lustrate = 45,
            Excogitation = 62,
            ChainStratagem = 66,
            Biolysis = 72,
            Recitation = 74,
            Consolation = 80,
            SummonSeraph = 80,
            Broil4 = 82;
    }
}

internal class ScholarFeyBless : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ScholarSeraphConsolationFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SCH.FeyBless)
        {
            var gauge = GetJobGauge<SCHGauge>();

            if (level >= SCH.Levels.Consolation && gauge.SeraphTimer > 0)
                return SCH.Consolation;
        }

        return actionID;
    }
}

internal class ScholarExcogitation : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SchAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SCH.Excogitation)
        {
            if (IsEnabled(CustomComboPreset.ScholarExcogitationRecitationFeature))
            {
                if (level >= SCH.Levels.Recitation && IsOffCooldown(SCH.Recitation))
                    return SCH.Recitation;
            }

            if (IsEnabled(CustomComboPreset.ScholarExcogitationLustrateFeature))
            {
                if (level < SCH.Levels.Excogitation || IsOnCooldown(SCH.Excogitation))
                    return SCH.Lustrate;
            }
        }

        return actionID;
    }
}

internal class ScholarEnergyDrain : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SchAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SCH.Broil4 || actionID == SCH.ArtOfWar2)
        {
            var gauge = GetJobGauge<SCHGauge>();

            var aetherflowCD = GetCooldown(SCH.Aetherflow).CooldownRemaining;

            if (level >= SCH.Levels.Aetherflow
                && GCDClipCheck(actionID)
                && gauge.Aetherflow >= 1
                && (aetherflowCD <= 10 && aetherflowCD / gauge.Aetherflow <= 3 
                    || IsOffCooldown(SCH.Aetherflow))
                )
            {
                return SCH.EnergyDrain;
            }

            if (level >= SCH.Levels.Aetherflow
                && GCDClipCheck(actionID)
                && gauge.Aetherflow == 0
                && IsOffCooldown(SCH.Aetherflow))
            {
                return SCH.Aetherflow;
            }

            if (InCombat()
                && IsOffCooldown(ADV.LucidDreaming)
                && LocalPlayer?.CurrentMp <= 8000
                && CanUseAction(ADV.LucidDreaming))
            {
                return ADV.LucidDreaming;
            }

            if (InCombat()
                && actionID != SCH.ArtOfWar2
                && TargetIsEnemy()
                && FindTargetEffect(SCH.Debuffs.Biolysis)?.RemainingTime <= 5)
            {
                return SCH.Biolysis;
            }
        }

        return actionID;
    }
}

internal class ScholarLustrate : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SchAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SCH.Lustrate)
        {
            var gauge = GetJobGauge<SCHGauge>();

            if (IsEnabled(CustomComboPreset.ScholarLustrateRecitationFeature))
            {
                if (level >= SCH.Levels.Recitation && IsOffCooldown(SCH.Recitation))
                    return SCH.Recitation;
            }

            if (IsEnabled(CustomComboPreset.ScholarLustrateExcogitationFeature))
            {
                if (level >= SCH.Levels.Excogitation && IsOffCooldown(SCH.Excogitation))
                    return SCH.Excogitation;
            }

            if (IsEnabled(CustomComboPreset.ScholarLustrateAetherflowFeature))
            {
                if (level >= SCH.Levels.Aetherflow && gauge.Aetherflow == 0)
                    return SCH.Aetherflow;
            }
        }

        return actionID;
    }
}

internal class ScholarIndomitability : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ScholarIndomAetherflowFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SCH.Indomitability)
        {
            var gauge = GetJobGauge<SCHGauge>();

            if (level >= SCH.Levels.Aetherflow && gauge.Aetherflow == 0 && !HasEffect(SCH.Buffs.Recitation))
                return SCH.Aetherflow;
        }

        return actionID;
    }
}

internal class ScholarSummon : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ScholarSeraphFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SCH.SummonEos || actionID == SCH.SummonSelene)
        {
            var gauge = GetJobGauge<SCHGauge>();

            if (gauge.SeraphTimer != 0 || HasPetPresent())
                // Consolation
                return OriginalHook(SCH.SummonSeraph);
        }

        return actionID;
    }
}
