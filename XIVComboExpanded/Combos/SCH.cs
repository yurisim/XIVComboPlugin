using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;

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
            SacredSoil = 299,
            WhisperingDawn = 315,
            Galvanize = 297,
            Catalyze = 1918,
            Dissipation = 791,
            Recitation = 1896;
    }

    public static class Debuffs
    {
        public const ushort
            ChainStrategem = 1221,
            Biolysis = 1895;
    }

    public static class Levels
    {
        public const byte
            Ressurection = 12,
            WhisperingDawn = 20,
            FeyIllumination = 40,
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

internal class ScholarSacredSoil : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SchAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SCH.SacredSoil)
        {

            if (level >= SCH.Levels.FeyIllumination
                && IsOffCooldown(OriginalHook(SCH.FeyIllumination))
                && !HasEffect(SCH.Buffs.Dissipation)
                )
                return OriginalHook(SCH.FeyIllumination);

            return actionID;
        }

        return actionID;
    }
}

internal class ScholarEnergyDrain : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SchAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SCH.Broil4 || actionID == SCH.ArtOfWar2 || actionID == SCH.Ruin2)
        {
            var gauge = GetJobGauge<SCHGauge>();

            var aetherflowCD = GetCooldown(SCH.Aetherflow).CooldownRemaining;

            if (GCDClipCheck(actionID))
            {
                var isThereSacredSoil = HasEffect(SCH.Buffs.SacredSoil);

                var isThereRaidDamage = LocalPlayerPercentage() <= 0.90;

                if (level >= SCH.Levels.ChainStratagem 
                    && IsOffCooldown(SCH.ChainStratagem)
                    && HasRaidBuffs())
                {
                    return SCH.ChainStratagem;
                }

                if (level >= SCH.Levels.Consolation
                    && gauge.SeraphTimer > 0
                    && HasCharges(SCH.Consolation)
                    && (GetRemainingCharges(SCH.Consolation) >= 2 || gauge.SeraphTimer <= 10)
                    )
                {
                    return SCH.Consolation;
                }

                if (!HasEffect(SCH.Buffs.Dissipation) && isThereRaidDamage)
                {
                    if (level >= SCH.Levels.FeyBlessing
                    && IsOffCooldown(SCH.FeyBlessing)
                    && gauge.SeraphTimer == 0
                    && (!isThereSacredSoil || LocalPlayerPercentage() <= 0.70))
                    {
                        return SCH.FeyBlessing;
                    }

                    if (level >= SCH.Levels.WhisperingDawn
                        && IsOffCooldown(OriginalHook(SCH.WhisperingDawn))
                        && (!isThereSacredSoil || LocalPlayerPercentage() <= 0.60))
                    {
                        return OriginalHook(SCH.WhisperingDawn);
                    }
                }

                if (TargetOfTargetHPercentage() <= 0.6
                    && level >= SCH.Levels.Excogitation
                    && IsOffCooldown(SCH.Excogitation)
                    && gauge.Aetherflow >= 2)
                {
                    return SCH.Excogitation;
                }

                var doDissipation = IsOnCooldown(SCH.WhisperingDawn)
                                && IsOnCooldown(SCH.FeyBlessing)
                                && IsOnCooldown(SCH.SummonSeraph)
                                && IsOnCooldown(SCH.FeyIllumination)
                                && IsOffCooldown(SCH.Dissipation)
                                && level >= SCH.Levels.Dissipation
                                && gauge.SeraphTimer == 0;

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

                if (level >= SCH.Levels.Aetherpact)
                {
                    if (gauge.FairyGauge >= 20
                         && TargetOfTargetHPercentage() <= 0.75
                         && OriginalHook(SCH.Aetherpact) == SCH.Aetherpact
                         && !HasEffect(SCH.Buffs.Dissipation)
                         && gauge.SeraphTimer == 0)
                    {
                        return SCH.Aetherpact;
                    }

                    if (TargetOfTargetHPercentage() >= 0.95
                         && OriginalHook(SCH.Aetherpact) != SCH.Aetherpact)
                    {
                        return OriginalHook(SCH.Aetherpact);
                    }
                }
            }

            var bio = FindTargetEffect(SCH.Debuffs.Biolysis);

            if (InCombat()
                && actionID != SCH.ArtOfWar2
                && InCombat()
                && TargetIsEnemy()
                && ((bio is not null 
                        && (bio.RemainingTime <= 3 
                            || (bio.RemainingTime <= 6 && IsMoving)))
                        || (bio is null && ShouldRefreshDots())))
            {
                return SCH.Biolysis;
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

            if (level >= SCH.Levels.FeyIllumination
                && IsOffCooldown(OriginalHook(SCH.FeyIllumination))
                && !HasEffect(SCH.Buffs.Dissipation)
                )
            {
                return OriginalHook(SCH.FeyIllumination);
            }

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

            var cd = GetCooldown(SCH.DeploymentTactics);

            if (cd.IsCooldown && cd.CooldownElapsed <= 1)
            {
                return OriginalHook(SCH.Broil4);
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
