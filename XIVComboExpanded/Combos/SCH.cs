using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class SCH
{
    public const byte ClassID = 15;
    public const byte JobID = 28;

    public const uint
        Adloquium = 185,
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
        FeyBlessing = 16543,
        SummonSeraph = 16545,
        Consolation = 16546,
        SummonEos = 17215,
        SummonSelene = 17216,
        ArtOfWar2 = 25866,
        Broil4 = 25865,
        Protraction = 25867,
        Expedient = 25868,
        Ruin2 = 17870;

    public static class Buffs
    {
        public const ushort
            Galvanize = 297,
            Catalyze = 1918,
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
            SacredSoil = 50,
            Indomitability = 52,
            DeploymentTactics = 56,
            Dissipation = 60,
            Excogitation = 62,
            ChainStratagem = 66,
            Aetherpact = 70,
            Biolysis = 72,
            Recitation = 74,
            FeyBlessing = 76,
            Consolation = 80,
            SummonSeraph = 80,
            Broil4 = 82,
            Protraction = 86,
            Expedient = 90;
    }
}

internal class ScholarExcogitation : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SchAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SCH.Excogitation)
        {
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

            var doDissipation = IsOnCooldown(SCH.WhisperingDawn)
                            && IsOnCooldown(SCH.FeyBlessing)
                            && IsOnCooldown(SCH.SummonSeraph)
                            && IsOnCooldown(SCH.FeyIllumination)
                            && IsOffCooldown(SCH.Dissipation)
                            && level >= SCH.Levels.Dissipation
                            && gauge.SeraphTimer == 0;


            if (GCDClipCheck(actionID))
            {
                if (level >= SCH.Levels.Aetherflow
                    && gauge.Aetherflow >= 1
                    && (aetherflowCD <= 10 && aetherflowCD / gauge.Aetherflow <= 3
                        || IsOffCooldown(SCH.Aetherflow)
                        || doDissipation
                        )
                    )
                {
                    return SCH.EnergyDrain;
                }

                if (level >= SCH.Levels.Aetherflow
                    && CanUseAction(SCH.Aetherflow)
                    && gauge.Aetherflow == 0
                    && IsOffCooldown(SCH.Aetherflow))
                {
                    return SCH.Aetherflow;
                }

                if (level >= SCH.Levels.Dissipation
                    && doDissipation
                    && gauge.Aetherflow == 0
                    && IsOffCooldown(SCH.Dissipation))
                {
                    return SCH.Dissipation;
                }

                if (InCombat()
                    && IsOffCooldown(ADV.LucidDreaming)
                    && LocalPlayer?.CurrentMp <= 8000)
                {
                    return ADV.LucidDreaming;
                }

                if (level >= SCH.Levels.Aetherpact
                     && gauge.FairyGauge >= 30
                     && TargetOfTargetHPercentage() <= 0.60
                     && OriginalHook(SCH.Aetherpact) == SCH.Aetherpact
                     && !HasEffect(SCH.Buffs.Dissipation)
                     && gauge.SeraphTimer == 0)
                {
                    return SCH.Aetherpact;
                }

                if (level >= SCH.Levels.Aetherpact
                     && TargetOfTargetHPercentage() >= 0.90
                     && OriginalHook(SCH.Aetherpact) != SCH.Aetherpact)
                {
                    return OriginalHook(SCH.Aetherpact);
                }
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
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SchAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SCH.Indomitability)
        {
            var gauge = GetJobGauge<SCHGauge>();

            if (level >= SCH.Levels.Consolation 
                && gauge.SeraphTimer > 0
                && GetRemainingCharges(SCH.Consolation) > 0)
                return SCH.Consolation;

            if (level >= SCH.Levels.FeyBlessing
                && !HasEffect(SCH.Buffs.Dissipation)
                && IsOffCooldown(SCH.FeyBlessing)
                && gauge.SeraphTimer <= 1)
            {
                return SCH.FeyBlessing;
            }

            if (level >= SCH.Levels.Indomitability
                && IsOffCooldown(SCH.Indomitability)
                && gauge.Aetherflow >= 1)
            {
                return SCH.Indomitability;
            }

            return CalcBestAction(actionID, SCH.FeyBlessing);

        }

        return actionID;
    }
}

internal class ScholarExcog : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SchAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SCH.Excogitation)
        {
            var gauge = GetJobGauge<SCHGauge>();

            if (level >= SCH.Levels.Protraction
                && IsOffCooldown(SCH.Protraction))
            {
                return SCH.Protraction;
            }

            if (level >= SCH.Levels.Excogitation
                && gauge.Aetherflow >= 1
                && IsOffCooldown(SCH.Excogitation))
            {
                return SCH.Excogitation;
            }

            return CalcBestAction(actionID, SCH.Protraction);
        }

        return actionID;
    }
}


internal class ScholarAdloCrit : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SchAny;
    
    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SCH.Adloquium)
        {
            if (level >= SCH.Levels.Recitation
                && IsOffCooldown(SCH.Recitation))
            {
                return SCH.Recitation;
            }

            if (level >= SCH.Levels.DeploymentTactics
                && TargetHasEffect(SCH.Buffs.Catalyze)
                && IsOffCooldown(SCH.DeploymentTactics))
            {
                return SCH.DeploymentTactics;
            }
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
