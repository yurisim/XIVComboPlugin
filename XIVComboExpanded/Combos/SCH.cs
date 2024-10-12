using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;
using Newtonsoft.Json.Converters;

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
        BanefulImpaction = 37012,
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
            Galvanize2 = 3087,
            Catalyze = 1918,
            Dissipation = 791,
            Recitation = 1896,
            ImpactImminent = 3882,
            SeraphicVeil = 1917,
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
            Broil = 54,
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
            Expedient = 90,
            BanefulImpaction = 92;

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

            var whisperingDawnCD = GetCooldown(SCH.WhisperingDawn);
            var feyBlessingCD = GetCooldown(SCH.FeyBlessing);
            var summonSeraphCD = GetCooldown(SCH.SummonSeraph);

            var doDissipation =
                whisperingDawnCD.CooldownRemaining >= 30
                && (feyBlessingCD.CooldownRemaining >= 30 || level < SCH.Levels.FeyBlessing)
                && (summonSeraphCD.CooldownRemaining >= 30 || level < SCH.Levels.SummonSeraph)
                && (aetherflowCD >= 8 || level < SCH.Levels.Aetherflow)
                && CanUseAction(SCH.Dissipation)
                && level >= SCH.Levels.Dissipation
                && gauge.SeraphTimer < 1;

            var impactImminent = FindEffect(SCH.Buffs.ImpactImminent);

            var threshold = 0.85;

            var localPlayer = LocalPlayerPercentage();

            if (GCDClipCheck(actionID))
            {
                switch (level)
                {
                    case >= SCH.Levels.ChainStratagem when
                        IsOffCooldown(SCH.ChainStratagem)
                        && HasRaidBuffs(2):
                        return SCH.ChainStratagem;

                    case >= SCH.Levels.FeyBlessing when
                        CanUseAction(SCH.FeyBlessing)
                        && IsOffCooldown(SCH.FeyBlessing)
                        && ((localPlayer <= threshold) || (actionID is SCH.ArtOfWar && TargetOfTargetHPercentage() <= threshold))
                        && !HasEffect(SCH.Buffs.SacredSoil)
                        && !HasEffect(SCH.Buffs.WhisperingDawn):
                        return SCH.FeyBlessing;

                    case >= SCH.Levels.WhisperingDawn when
                        CanUseAction(OriginalHook(SCH.WhisperingDawn))
                        && IsOffCooldown(SCH.WhisperingDawn)
                        && ((localPlayer <= threshold) || (actionID is SCH.ArtOfWar && TargetOfTargetHPercentage() <= threshold))
                        && !HasEffect(SCH.Buffs.SacredSoil):
                        return SCH.WhisperingDawn;

                    case >= SCH.Levels.SummonSeraph when
                        CanUseAction(SCH.SummonSeraph)
                        && IsOffCooldown(SCH.SummonSeraph)
                        && actionID is SCH.ArtOfWar && TargetOfTargetHPercentage() <= threshold
                        && !HasEffect(SCH.Buffs.SacredSoil)
                        && !HasEffect(SCH.Buffs.WhisperingDawn):
                        return SCH.SummonSeraph;

                    case >= SCH.Levels.SacredSoil when
                        CanUseAction(SCH.SacredSoil)
                        && !IsMoving
                        && actionID is SCH.ArtOfWar
                        && TargetOfTargetHPercentage() <= threshold
                        && !HasEffect(SCH.Buffs.WhisperingDawn):
                        return SCH.SacredSoil;

                    case >= SCH.Levels.Consolation when
                        gauge.SeraphTimer > 0
                        && HasCharges(SCH.Consolation)
                        && CanUseAction(SCH.Consolation)
                        && (GetRemainingCharges(SCH.Consolation) == 2
                            || (localPlayer <= threshold + 0.1 && !HasEffect(SCH.Buffs.SeraphicVeil))
                            || (actionID is SCH.ArtOfWar && TargetOfTargetHPercentage() <= threshold && !TargetHasEffect(SCH.Buffs.SeraphicVeil))
                            || gauge.SeraphTimer <= 5000):
                        return SCH.Consolation;

                    case >= SCH.Levels.Excogitation when
                        TargetOfTargetHPercentage() <= 0.6
                        && IsOffCooldown(SCH.Excogitation)
                        && gauge.Aetherflow >= 2:
                        return SCH.Excogitation;

                    case >= SCH.Levels.Aetherflow when
                        gauge.Aetherflow >= 1
                        && ((aetherflowCD <= 7.5 && aetherflowCD / gauge.Aetherflow <= 2.5)
                            || IsOffCooldown(SCH.Aetherflow)
                            || doDissipation):
                        return SCH.EnergyDrain;

                    case >= SCH.Levels.BanefulImpaction when
                        impactImminent is not null
                        && (HasRaidBuffs(2) || impactImminent.RemainingTime <= 20):
                        return SCH.BanefulImpaction;

                    case >= SCH.Levels.Aetherflow when
                        CanUseAction(SCH.Aetherflow)
                        && gauge.Aetherflow == 0
                        && IsOffCooldown(SCH.Aetherflow):
                        return SCH.Aetherflow;

                    case >= SCH.Levels.Dissipation when
                        doDissipation
                        && gauge.Aetherflow == 0
                        && IsOffCooldown(SCH.Dissipation):
                        return SCH.Dissipation;

                    case >= ADV.Levels.LucidDreaming when InCombat() && IsOffCooldown(ADV.LucidDreaming) &&
                        LocalPlayer?.CurrentMp <= 8000:
                        return ADV.LucidDreaming;

                    case >= SCH.Levels.Aetherpact:
                        if (gauge.FairyGauge >= 30
                            && TargetOfTargetHPercentage() <= 0.80
                            && OriginalHook(SCH.Aetherpact) == SCH.Aetherpact
                            && !HasEffect(SCH.Buffs.Dissipation)
                            && gauge.SeraphTimer == 0)
                            return OriginalHook(SCH.Aetherpact);

                        if (TargetOfTargetHPercentage() >= 0.95
                            && OriginalHook(SCH.Aetherpact) != SCH.Aetherpact)
                            return OriginalHook(SCH.Aetherpact);
                        break;
                }

            }

            if (InCombat() && actionID != SCH.ArtOfWar && ShouldUseDots())
            {
                var dots = new[]
                {
                    FindTargetEffect(SCH.Debuffs.Bio),
                    FindTargetEffect(SCH.Debuffs.Bio2),
                    FindTargetEffect(SCH.Debuffs.Biolysis)
                };

                if (dots.All(x => x is null || x.RemainingTime <= 4 || x.RemainingTime <= 8 && IsMoving))
                    return OriginalHook(SCH.Bio);
            }

            if (level >= SCH.Levels.ArtOfWar && level < SCH.Levels.Broil)
            {
                return OriginalHook(SCH.ArtOfWar);
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
                return SCH.FeyBlessing;

            if (
                level >= SCH.Levels.Indomitability
                && IsOffCooldown(SCH.Indomitability)
                && gauge.Aetherflow >= 1
            )
                return SCH.Indomitability;

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

            if (level >= SCH.Levels.Protraction && IsOffCooldown(SCH.Protraction)) return SCH.Protraction;

            if (
                level >= SCH.Levels.Excogitation
                && gauge.Aetherflow >= 1
                && IsOffCooldown(SCH.Excogitation)
            )
                return SCH.Excogitation;

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
            if (level >= SCH.Levels.Recitation && IsOffCooldown(SCH.Recitation)) return SCH.Recitation;

            if (
                level >= SCH.Levels.DeploymentTactics
                && (TargetHasEffect(SCH.Buffs.Catalyze) || TargetHasEffect(SCH.Buffs.Galvanize2))
                && IsOffCooldown(SCH.DeploymentTactics)
            )
                return SCH.DeploymentTactics;

            var cd = GetCooldown(SCH.DeploymentTactics);

            if (cd.IsCooldown && cd.CooldownElapsed <= 1) return OriginalHook(SCH.Ruin);
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
            if (HasEffect(SCH.Buffs.Seraphism))
                return OriginalHook(SCH.EmergencyTactics);

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
            if (level < SCH.Levels.Adloquium)
                return SCH.Physick;

        return actionID;
    }
}