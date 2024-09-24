using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class WHM
{
    public const byte ClassID = 6;
    public const byte JobID = 24;

    public const uint Cure = 120,
        Aero = 121,
        Medica = 124,
        Stone = 119,
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
        LiturgyOfTheBell = 25862,
        Glare4 = 37009,
        Medica3 = 37010;

    public static class Buffs
    {
        public const ushort Glare4Ready = 3879,
            ThinAir = 1217;
    }

    public static class Debuffs
    {
        public const ushort Dia = 1871,
            Aero = 143,
            Aero2 = 144;
    }

    public static class Levels
    {
        public const byte Aero = 4,
            Aero2 = 46,
            Dia = 72,
            Raise = 12,
            Cure2 = 30,
            PresenceOfMind = 30,
            AfflatusSolace = 52,
            Assize = 56,
            ThinAir = 58,
            Tetragrammaton = 60,
            PlenaryIndulgence = 70,
            AfflatusMisery = 74,
            Aquaveil = 86,
            EnhancedBenison = 88,
            AfflatusRapture = 76,
            Glare4 = 92;
    }
}

internal class WhiteMageAfflatusRapture : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhmAny;

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
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WHM.Holy)
        {
            var gauge = GetJobGauge<WHMGauge>();

            if (level >= WHM.Levels.AfflatusMisery && gauge.BloodLily == 3) return WHM.AfflatusMisery;

            if (level >= WHM.Levels.Glare4 && HasEffect(WHM.Buffs.Glare4Ready)) return WHM.Glare4;
        }

        return actionID;
    }
}

// internal class WhiteMageCure2 : CustomCombo
// {
//     protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhmAny;

//     protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//     {
//         if (actionID == WHM.Cure2)
//         {
//             var gauge = GetJobGauge<WHMGauge>();

//             if (IsEnabled(CustomComboPreset.WhiteMageCureFeature))
//             {
//                 if (level < WHM.Levels.Cure2)
//                     return WHM.Cure;
//             }

//             if (IsEnabled(CustomComboPreset.WhiteMageAfflatusFeature))
//             {
//                 if (IsEnabled(CustomComboPreset.WhiteMageSolaceMiseryFeature))
//                 {
//                     if (level >= WHM.Levels.AfflatusMisery && gauge.BloodLily == 3)
//                     {
//                         if (IsEnabled(CustomComboPreset.WhiteMageSolaceMiseryTargetFeature))
//                         {
//                             if (TargetIsEnemy())
//                                 return WHM.AfflatusMisery;
//                         }
//                         else
//                         {
//                             return WHM.AfflatusMisery;
//                         }
//                     }
//                 }

//                 if (level >= WHM.Levels.AfflatusSolace && gauge.Lily > 0)
//                     return WHM.AfflatusSolace;
//             }
//         }

//         return actionID;
//     }
// }

internal class WhiteMageMedica : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WHM.Medica)
        {
            var gauge = GetJobGauge<WHMGauge>();

            if (level >= WHM.Levels.PlenaryIndulgence && IsOffCooldown(WHM.PlenaryIndulgence))
                return WHM.PlenaryIndulgence;

            if (
                level >= WHM.Levels.AfflatusMisery
                && gauge.BloodLily == 3
                && LocalPlayerPercentage() > 0.90
            )
                return WHM.AfflatusMisery;

            if (level >= WHM.Levels.AfflatusRapture && gauge.Lily > 0) return WHM.AfflatusRapture;
        }

        return actionID;
    }
}

internal class WhiteMageBenison : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WHM.DivineBenison)
        {
            if (
                level >= WHM.Levels.Aquaveil
                && IsOffCooldown(WHM.Aquaveil)
                && GetRemainingCharges(WHM.DivineBenison) <= 1
            )
                return WHM.Aquaveil;

            return CalcBestAction(actionID, WHM.Aquaveil, WHM.DivineBenison);
        }

        return actionID;
    }
}

internal class WhiteMageStoneFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == WHM.Stone)
        {
            var tarOfTarPercentage = TargetOfTargetHPercentage();

            var playerPercentage = LocalPlayerPercentage();

            var gauge = GetJobGauge<WHMGauge>();

            if (GCDClipCheck(actionID))
            {
                if (
                    level >= WHM.Levels.PresenceOfMind
                    && IsOffCooldown(WHM.PresenceOfMind)
                    && HasRaidBuffs()
                )
                    return WHM.PresenceOfMind;

                if (
                    level >= WHM.Levels.Assize
                    && IsOffCooldown(WHM.Assize)
                    && GetTargetDistance() <= 15
                    && (playerPercentage < 1)
                    && (IsOnCooldown(WHM.PresenceOfMind) || HasRaidBuffs())
                )
                    return WHM.Assize;

                if (FindTargetOfTargetEffectAny(WAR.Buffs.Holmgang) is null)
                {
                    if (
                        level >= WHM.Levels.EnhancedBenison
                        && HasCharges(WHM.DivineBenison)
                        && (
                            (
                                GetCooldown(WHM.DivineBenison).CooldownRemaining <= 5
                                && tarOfTarPercentage <= 0.75
                            )
                            || tarOfTarPercentage <= 0.6
                        )
                    )
                        return WHM.DivineBenison;

                    if (
                        level >= WHM.Levels.Tetragrammaton
                        && tarOfTarPercentage <= 0.70
                        && IsOffCooldown(WHM.Tetragrammaton)
                    )
                        return WHM.Tetragrammaton;
                }

                if (
                    HasCondition(ConditionFlag.InCombat)
                    && IsOffCooldown(ADV.LucidDreaming)
                    && LocalPlayer?.CurrentMp <= 8000
                )
                    return ADV.LucidDreaming;
            }

            (ushort Debuff, ushort Level)[] aeroDOT =
            [
                (WHM.Debuffs.Dia, WHM.Levels.Dia),
                (WHM.Debuffs.Aero2, WHM.Levels.Aero2),
                (WHM.Debuffs.Aero, WHM.Levels.Aero)
            ];

            var debuff = FindTargetEffect(aeroDOT.FirstOrDefault(x => x.Level <= level).Debuff);

            var debuffTime = debuff?.RemainingTime;

            if (
                InCombat()
                && (
                    (
                        debuff is not null
                        && (debuffTime <= 3 || (debuff.RemainingTime <= 6 && this.IsMoving))
                    ) || (debuff is null && ShouldUseDots())
                )
            )
                return OriginalHook(WHM.Aero);

            if (level >= WHM.Levels.AfflatusMisery && gauge.BloodLily == 3) return WHM.AfflatusMisery;

            if (gauge.Lily == 3)
            {
                if (playerPercentage <= 0.80 && level >= WHM.Levels.AfflatusRapture) return WHM.AfflatusRapture;

                if (tarOfTarPercentage <= 0.80 && level >= WHM.Levels.AfflatusSolace) return WHM.AfflatusSolace;
            }

            if (level >= WHM.Levels.Glare4 && HasEffect(WHM.Buffs.Glare4Ready))
                return WHM.Glare4;

            return OriginalHook(actionID);
        }

        if (
            (actionID == WHM.Medica2 || actionID == WHM.Cure3)
            && level >= WHM.Levels.ThinAir
            && !HasEffect(WHM.Buffs.ThinAir)
            && GetRemainingCharges(WHM.ThinAir) >= 1
        )
            return WHM.ThinAir;

        if (
            actionID == WHM.Raise
            && level >= WHM.Levels.ThinAir
            && !HasEffect(WHM.Buffs.ThinAir)
            && GetRemainingCharges(WHM.ThinAir) >= 1
        )
            return WHM.ThinAir;

        return actionID;
    }
}