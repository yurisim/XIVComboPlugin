using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class SCH
{
    public const byte ClassID = 15;
    public const byte JobID = 28;

    public const uint Adloquium = 185,
        Bio = 17864,
        Aetherflow = 166,
        EnergyDrain = 167,
        Resurrection = 173,
        SacredSoil = 188,
        Lustrate = 189,
        Physick = 190,
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
        ArtOfWar = 16539,
        Protraction = 25867,
        Expedient = 25868,
        Ruin = 17869,
        Ruin2 = 17870,
        Seraphism = 37014;

    public static class Buffs
    {
        public const ushort SacredSoil = 299,
            WhisperingDawn = 315,
            Galvanize = 297,
            Catalyze = 1918,
            Dissipation = 791,
            Recitation = 1896,
            Seraphism = 3884,
            SeraphismAura = 3885;
    }

    public static class Debuffs
    {
        public const ushort Bio = 179,
            Bio2 = 189,
            ChainStrategem = 1221,
            Biolysis = 1895;
    }

    public static class Levels
    {
        public const byte Resurrection = 12,
            Adloquium = 30,
            WhisperingDawn = 20,
            FeyIllumination = 40,
            Aetherflow = 45,
            Lustrate = 45,
            ArtOfWar = 46,
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
            if (
                level >= SCH.Levels.FeyIllumination
                && IsOnCooldown(SCH.SacredSoil)
                && IsOffCooldown(OriginalHook(SCH.FeyIllumination))
                && !HasEffect(SCH.Buffs.Dissipation)
            )
            {
                return OriginalHook(SCH.FeyIllumination);
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
        if (actionID == SCH.Ruin || actionID == SCH.ArtOfWar || actionID == SCH.Ruin2)
        {
            var gauge = GetJobGauge<SCHGauge>();

            var aetherflowCD = GetCooldown(SCH.Aetherflow).CooldownRemaining;

            if (GCDClipCheck(actionID))
            {
                var isThereSacredSoil = HasEffect(SCH.Buffs.SacredSoil);

                var isThereRaidDamage = LocalPlayerPercentage() <= 0.90;

                if (
                    level >= SCH.Levels.ChainStratagem
                    && IsOffCooldown(SCH.ChainStratagem)
                    && HasRaidBuffs()
                )
                {
                    return SCH.ChainStratagem;
                }

                if (
                    level >= SCH.Levels.Consolation
                    && gauge.SeraphTimer > 0
                    && HasCharges(SCH.Consolation)
                    && (
                        GetRemainingCharges(SCH.Consolation) >= 2
                        || LocalPlayerPercentage() < 0.80
                        || gauge.SeraphTimer <= 5
                    )
                )
                {
                    return OriginalHook(SCH.SummonSeraph);
                }

                if (!HasEffect(SCH.Buffs.Dissipation) && isThereRaidDamage)
                {
                    if (
                        level >= SCH.Levels.FeyBlessing
                        && IsOffCooldown(SCH.FeyBlessing)
                        && gauge.SeraphTimer == 0
                        && (!isThereSacredSoil || LocalPlayerPercentage() <= 0.70)
                    )
                    {
                        return SCH.FeyBlessing;
                    }

                    if (
                        level >= SCH.Levels.WhisperingDawn
                        && IsOffCooldown(OriginalHook(SCH.WhisperingDawn))
                        && (!isThereSacredSoil || LocalPlayerPercentage() <= 0.60)
                    )
                    {
                        return OriginalHook(SCH.WhisperingDawn);
                    }
                }

                if (
                    TargetOfTargetHPercentage() <= 0.5
                    && level >= SCH.Levels.Excogitation
                    && IsOffCooldown(SCH.Excogitation)
                    && gauge.Aetherflow >= 2
                )
                {
                    return SCH.Excogitation;
                }

                var doDissipation =
                    IsOnCooldown(SCH.WhisperingDawn)
                    && IsOnCooldown(SCH.FeyBlessing)
                    && IsOnCooldown(SCH.SummonSeraph)
                    && IsOnCooldown(SCH.FeyIllumination)
                    && IsOffCooldown(SCH.Dissipation)
                    && level >= SCH.Levels.Dissipation
                    && gauge.SeraphTimer == 0;

                if (
                    level >= SCH.Levels.Aetherflow
                    && gauge.Aetherflow >= 1
                    && (
                        (aetherflowCD <= 10 && aetherflowCD / gauge.Aetherflow <= 3)
                        || IsOffCooldown(SCH.Aetherflow)
                        || (HasRaidBuffs() && gauge.Aetherflow >= 3)
                        || doDissipation
                    )
                )
                {
                    return SCH.EnergyDrain;
                }

                if (
                    level >= SCH.Levels.Aetherflow
                    && CanUseAction(SCH.Aetherflow)
                    && gauge.Aetherflow == 0
                    && IsOffCooldown(SCH.Aetherflow)
                )
                {
                    return SCH.Aetherflow;
                }

                if (
                    level >= SCH.Levels.Dissipation
                    && doDissipation
                    && gauge.Aetherflow == 0
                    && IsOffCooldown(SCH.Dissipation)
                )
                {
                    return SCH.Dissipation;
                }

                if (
                    InCombat()
                    && IsOffCooldown(ADV.LucidDreaming)
                    && LocalPlayer?.CurrentMp <= 8000
                )
                {
                    return ADV.LucidDreaming;
                }

                if (level >= SCH.Levels.Aetherpact)
                {
                    if (
                        gauge.FairyGauge >= 30
                        && TargetOfTargetHPercentage() <= 0.80
                        && OriginalHook(SCH.Aetherpact) == SCH.Aetherpact
                        && !HasEffect(SCH.Buffs.Dissipation)
                        && gauge.SeraphTimer == 0
                    )
                    {
                        return OriginalHook(SCH.Aetherpact);
                    }

                    if (
                        TargetOfTargetHPercentage() >= 0.95
                        && OriginalHook(SCH.Aetherpact) != SCH.Aetherpact
                    )
                    {
                        return OriginalHook(SCH.Aetherpact);
                    }
                }
            }

            if (InCombat() && TargetIsEnemy() && actionID != SCH.ArtOfWar && ShouldRefreshDots())
            {
                var combustEffects = new[]
                {
                    FindTargetEffect(SCH.Debuffs.Bio),
                    FindTargetEffect(SCH.Debuffs.Bio2),
                    FindTargetEffect(SCH.Debuffs.Biolysis)
                };

                if (
                    !combustEffects.Any(
                        effect =>
                            effect?.RemainingTime > 2.8
                            || (
                                effect?.RemainingTime is not null
                                && effect.RemainingTime <= 6
                                && this.IsMoving
                            )
                    )
                )
                {
                    return OriginalHook(SCH.Bio);
                }
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

            if (
                level >= SCH.Levels.Consolation
                && gauge.SeraphTimer > 0
                && GetRemainingCharges(SCH.Consolation) > 0
            )
                return SCH.Consolation;

            if (
                level >= SCH.Levels.FeyBlessing
                && !HasEffect(SCH.Buffs.Dissipation)
                && IsOffCooldown(SCH.FeyBlessing)
                && gauge.SeraphTimer <= 1
            )
            {
                return SCH.FeyBlessing;
            }

            if (
                level >= SCH.Levels.Indomitability
                && IsOffCooldown(SCH.Indomitability)
                && gauge.Aetherflow >= 1
            )
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

            if (level >= SCH.Levels.Protraction && IsOffCooldown(SCH.Protraction))
            {
                return SCH.Protraction;
            }

            if (
                level >= SCH.Levels.Excogitation
                && gauge.Aetherflow >= 1
                && IsOffCooldown(SCH.Excogitation)
            )
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
            if (
                level >= SCH.Levels.FeyIllumination
                && IsOffCooldown(OriginalHook(SCH.FeyIllumination))
                && !HasEffect(SCH.Buffs.Dissipation)
            )
            {
                return OriginalHook(SCH.FeyIllumination);
            }

            if (level >= SCH.Levels.Recitation && IsOffCooldown(SCH.Recitation))
            {
                return SCH.Recitation;
            }

            if (
                level >= SCH.Levels.DeploymentTactics
                && TargetHasEffect(SCH.Buffs.Catalyze)
                && IsOffCooldown(SCH.DeploymentTactics)
            )
            {
                return SCH.DeploymentTactics;
            }

            var cd = GetCooldown(SCH.DeploymentTactics);

            if (cd.IsCooldown && cd.CooldownElapsed <= 1)
            {
                return OriginalHook(SCH.Ruin);
            }
        }

        return actionID;
    }
}

internal class ScholarSummon : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.ScholarSeraphFeature;

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

internal class ScholarSeraphism : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.ScholarSeraphismFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SCH.Seraphism)
        {
            if (HasEffect(SCH.Buffs.Seraphism))
                return OriginalHook(SCH.EmergencyTactics);
        }

        return actionID;
    }
}

internal class ScholarAdloquium : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.ScholarAdloquiumSyncFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SCH.Adloquium)
        {
            if (level < SCH.Levels.Adloquium)
                return SCH.Physick;
        }

        return actionID;
    }
}
