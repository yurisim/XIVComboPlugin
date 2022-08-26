using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Data;

namespace XIVComboExpandedPlugin.Combos;

internal static class WHM
{
    public const byte ClassID = 6;
    public const byte JobID = 24;

    public const uint
        Cure = 120,
        Medica = 124,
        Stone2 = 127,
        Raise = 125,
        Cure3 = 131,
        Medica2 = 133,
        Cure2 = 135,
        PresenceOfMind = 136,
        Holy = 139,
        Benediction = 140,
        Asylum = 3569,
        Stone3 = 3568,
        Tetragrammaton = 3570,
        Assize = 3571,
        ThinAir = 7430,
        Stone4 = 7431,
        DivineBenison = 7432,
        PlenaryIndulgence = 7433,
        AfflatusSolace = 16531,
        Dia = 16532,
        Glare = 16533,
        AfflatusRapture = 16534,
        AfflatusMisery = 16535,
        Temperance = 16536,
        Glare3 = 25859,
        Holy3 = 25860,
        Aquaveil = 25861,
        LiturgyOfTheBell = 25862;

    public static class Buffs
    {
        public const ushort
            ThinAir = 1217;
    }

    public static class Debuffs
    {
        public const ushort
            Dia = 1871;
    }

    public static class Levels
    {
        public const byte
            Raise = 12,
            Cure2 = 30,
            AfflatusSolace = 52,
            ThinAir = 58,
            Tetragrammaton = 60,
            PlenaryIndulgence = 70,
            AfflatusMisery = 74,
            AfflatusRapture = 76,
            EnhancedBenison = 88;
    }
}

internal class WhiteMageAfflatusSolace : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhiteMageSolaceMiseryFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WHM.AfflatusSolace)
        {
            var gauge = GetJobGauge<WHMGauge>();

            if (level >= WHM.Levels.AfflatusMisery && gauge.BloodLily == 3)
            {
                if (IsEnabled(CustomComboPreset.WhiteMageSolaceMiseryTargetFeature))
                {
                    if (TargetIsEnemy())
                        return WHM.AfflatusMisery;
                }
                else
                {
                    return WHM.AfflatusMisery;
                }
            }
        }

        return actionID;
    }
}

internal class WhiteMageAfflatusRapture : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhiteMageRaptureMiseryFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WHM.AfflatusRapture)
        {
            var gauge = GetJobGauge<WHMGauge>();

            if (level >= WHM.Levels.AfflatusMisery && gauge.BloodLily == 3 && TargetIsEnemy())
                return WHM.AfflatusMisery;
        }

        return actionID;
    }
}

internal class WhiteMageHoly : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhiteMageHolyMiseryFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WHM.Holy || actionID == WHM.Holy3)
        {
            var gauge = GetJobGauge<WHMGauge>();

            if (level >= WHM.Levels.AfflatusMisery && gauge.BloodLily == 3 && TargetIsEnemy())
                return WHM.AfflatusMisery;
        }

        return actionID;
    }
}

internal class WhiteMageCure2 : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WHM.Cure2)
        {
            var gauge = GetJobGauge<WHMGauge>();

            if (IsEnabled(CustomComboPreset.WhiteMageCureFeature))
            {
                if (level < WHM.Levels.Cure2)
                    return WHM.Cure;
            }

            if (IsEnabled(CustomComboPreset.WhiteMageAfflatusFeature))
            {
                if (IsEnabled(CustomComboPreset.WhiteMageSolaceMiseryFeature))
                {
                    if (level >= WHM.Levels.AfflatusMisery && gauge.BloodLily == 3)
                    {
                        if (IsEnabled(CustomComboPreset.WhiteMageSolaceMiseryTargetFeature))
                        {
                            if (TargetIsEnemy())
                                return WHM.AfflatusMisery;
                        }
                        else
                        {
                            return WHM.AfflatusMisery;
                        }
                    }
                }

                if (level >= WHM.Levels.AfflatusSolace && gauge.Lily > 0)
                    return WHM.AfflatusSolace;
            }
        }

        return actionID;
    }
}

internal class WhiteMageMedica : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WHM.Medica)
        {
            var gauge = GetJobGauge<WHMGauge>();

            if (IsEnabled(CustomComboPreset.WhiteMageAfflatusFeature))
            {
                if (IsEnabled(CustomComboPreset.WhiteMageRaptureMiseryFeature))
                {
                    if (level >= WHM.Levels.AfflatusMisery && gauge.BloodLily == 3 && TargetIsEnemy())
                        return WHM.AfflatusMisery;
                }

                if (level >= WHM.Levels.PlenaryIndulgence && IsOffCooldown(WHM.PlenaryIndulgence))
                    return WHM.PlenaryIndulgence;


                if (level >= WHM.Levels.AfflatusRapture && gauge.Lily > 0)
                    return WHM.AfflatusRapture;
            }
        }

        return actionID;
    }
}

internal class WhiteMageDiaFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhmAny;
    
    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WHM.Stone2 || actionID == WHM.Stone3 || actionID == WHM.Stone4 || actionID == WHM.Glare || actionID == WHM.Glare3)
        {
            var targetOfTarget = GetTargetOfTarget();

            var percentage = (targetOfTarget is not null) ? (float)targetOfTarget.CurrentHp / targetOfTarget.MaxHp : 1;
            
            if (GCDClipCheck(actionID))
            {
                if (HasCondition(ConditionFlag.InCombat) 
                    && IsOffCooldown(ADV.LucidDreaming) 
                    && LocalPlayer?.CurrentMp <= 8000)
                    return ADV.LucidDreaming;

                if ((percentage <= 0.80) && (percentage >= 0.10))
                {
                    if (level >= WHM.Levels.Tetragrammaton
                    && IsOffCooldown(WHM.Tetragrammaton))
                    {
                        return WHM.Tetragrammaton;
                    }

                    if (level >= WHM.Levels.EnhancedBenison
                    && GetRemainingCharges(WHM.DivineBenison) >= 2)
                    {
                        return WHM.DivineBenison;
                    }
                }
            }
            
            var diaFound = FindTargetEffect(WHM.Debuffs.Dia);

            // If I'm in combat and the target is an enemy and doesn't have dia, use dia.p
            if (InCombat() && (diaFound?.RemainingTime <= 5 || (diaFound is null && (CurrentTarget as BattleChara)?.MaxHp > 20000000)))
            {
                return WHM.Dia;
            }
        }
       
        if ((actionID == WHM.Medica2 || actionID == WHM.Cure3) 
            && level >= WHM.Levels.ThinAir 
            && !HasEffect(WHM.Buffs.ThinAir) 
            && GetRemainingCharges(WHM.ThinAir) >= 1)
            return WHM.ThinAir;

        if (actionID == WHM.Raise && level >= WHM.Levels.ThinAir && !HasEffect(WHM.Buffs.ThinAir) && GetRemainingCharges(WHM.ThinAir) >= 1)
            return WHM.ThinAir;

        return actionID;
    }
}