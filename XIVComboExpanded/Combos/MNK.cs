using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class MNK
{
    public const byte ClassID = 2;
    public const byte JobID = 20;

    public const uint Bootshine = 53,
        TrueStrike = 54,
        SnapPunch = 56,
        TwinSnakes = 61,
        ArmOfTheDestroyer = 62,
        Demolish = 66,
        PerfectBalance = 69,
        Rockbreaker = 70,
        DragonKick = 74,
        FormShift = 4262,
        RiddleOfFire = 7395,
        Brotherhood = 7396,
        FourPointFury = 16473,
        Enlightenment = 16474,
        HowlingFist = 25763,
        MasterfulBlitz = 25764,
        RiddleOfWind = 25766,
        ShadowOfTheDestroyer = 25767,
        EarthsReply = 36944,
        SteeledMeditation = 36942,
        EnlightenedMeditation = 36943,
        LeapingOpo = 36945,
        RisingRaptor = 36946,
        PouncingCoeurl = 36947;

    public static class Buffs
    {
        public const ushort OpoOpoForm = 107,
            RaptorForm = 108,
            CoerlForm = 109,
            PerfectBalance = 110,
            EarthsRumination = 3841,
            Brotherhood = 1185,
            RiddleOfFire = 1181,
            FormlessFist = 2513,
            DisciplinedFist = 3001;
    }

    public static class Debuffs
    {
        public const ushort Demolish = 246;
    }

    public static class Levels
    {
        public const byte SnapPunch = 6,
            TwinSnakes = 18,
            ArmOfTheDestroyer = 26,
            Rockbreaker = 30,
            Demolish = 30,
            FourPointFury = 45,
            HowlingFist = 40,
            DragonKick = 50,
            PerfectBalance = 50,
            FormShift = 52,
            TheForbiddenChakra = 54,
            EnhancedPerfectBalance = 60,
            MasterfulBlitz = 60,
            RiddleOfEarth = 64,
            RiddleOfFire = 68,
            Brotherhood = 70,
            Enlightenment = 70,
            RiddleOfWind = 72,
            EnlightenedMeditation = 74,
            ShadowOfTheDestroyer = 82;
    }
}

internal class MonkDragonKick : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.Bootshine)
        {
            var gauge = GetJobGauge<MNKGauge>();
            var riddleOfFire = FindEffect(MNK.Buffs.RiddleOfFire);
            var doesNotHaveSolar = !gauge.Nadi.HasFlag(Nadi.SOLAR);
            var hasSolarLunar = gauge.Nadi.HasFlag(Nadi.SOLAR) && gauge.Nadi.HasFlag(Nadi.LUNAR);

            if (GCDClipCheck(actionID)
                && InCombat()
                && HasTarget())
            {
                if (gauge.Chakra >= 5
                    && (level < MNK.Levels.RiddleOfFire
                        || IsOnCooldown(MNK.RiddleOfFire))
                    && InCombat()
                    && HasTarget())
                    return OriginalHook(MNK.SteeledMeditation);

                switch (level)
                {
                    case >= MNK.Levels.PerfectBalance when
                        GetRemainingCharges(MNK.PerfectBalance) >= 1
                        && (gauge.CoeurlFury != 0 || doesNotHaveSolar || hasSolarLunar)
                        && !HasEffect(MNK.Buffs.FormlessFist)
                        && OriginalHook(MNK.MasterfulBlitz) == MNK.MasterfulBlitz
                        && !HasEffect(MNK.Buffs.PerfectBalance)
                        && (riddleOfFire?.RemainingTime >= 8
                            || HasRaidBuffs()
                            || GetCooldown(MNK.PerfectBalance).TotalCooldownRemaining <= 6
                            || level < MNK.Levels.RiddleOfFire):
                        return MNK.PerfectBalance;
                    case >= MNK.Levels.RiddleOfFire when
                        InMeleeRange()
                        && IsOffCooldown(MNK.RiddleOfFire)
                        && (HasEffect(MNK.Buffs.Brotherhood)
                            || HasRaidBuffs()
                            || level < MNK.Levels.Brotherhood
                            || GetCooldown(OriginalHook(MNK.Brotherhood)).CooldownRemaining >= 10):
                        return MNK.RiddleOfFire;
                    case >= MNK.Levels.RiddleOfWind when
                        IsOffCooldown(MNK.RiddleOfWind)
                        && GetCooldown(MNK.RiddleOfFire).CooldownRemaining > 9
                        && InMeleeRange():
                        return MNK.RiddleOfWind;
                    case >= MNK.Levels.RiddleOfEarth when
                        LocalPlayerPercentage() != 1
                        && HasEffect(MNK.Buffs.EarthsRumination):
                        return MNK.EarthsReply;
                }
            }

            if (GetTargetDistance() >= 7 || !InCombat() || !HasTarget())
            {
                if (gauge.Chakra < 5)
                    return MNK.SteeledMeditation;

                if (level >= MNK.Levels.FormShift
                    && !HasEffect(MNK.Buffs.FormlessFist)
                    && !HasEffect(MNK.Buffs.PerfectBalance))
                    return MNK.FormShift;
            }

            if (level >= MNK.Levels.MasterfulBlitz
                && !HasEffect(MNK.Buffs.PerfectBalance)
                && OriginalHook(MNK.MasterfulBlitz) != MNK.MasterfulBlitz
                && (riddleOfFire != null
                    || HasRaidBuffs()
                    || gauge.BlitzTimeRemaining <= 4
                    || GetCooldown(MNK.RiddleOfFire).CooldownRemaining >= 15
                    || level < MNK.Levels.RiddleOfFire))
                return OriginalHook(MNK.MasterfulBlitz);

            if (HasEffect(MNK.Buffs.RaptorForm)
                || (HasEffect(MNK.Buffs.PerfectBalance)
                    && level >= MNK.Levels.MasterfulBlitz
                    && ((!gauge.Nadi.HasFlag(Nadi.SOLAR) && !gauge.BeastChakra.Contains(BeastChakra.RAPTOR))
                        || (gauge.Nadi.HasFlag(Nadi.SOLAR)
                            && gauge.Nadi.HasFlag(Nadi.LUNAR)
                            && gauge.RaptorFury == 0)))
                || (HasEffect(MNK.Buffs.FormlessFist)
                    && gauge.RaptorFury == 0))
            {
                if (gauge.RaptorFury == 0 && level >= MNK.Levels.TwinSnakes)
                    return MNK.TwinSnakes;
                return MNK.TrueStrike;
            }

            if (HasEffect(MNK.Buffs.CoerlForm)
                || (HasEffect(MNK.Buffs.PerfectBalance)
                    && level >= MNK.Levels.MasterfulBlitz
                    && ((!gauge.Nadi.HasFlag(Nadi.SOLAR) && !gauge.BeastChakra.Contains(BeastChakra.COEURL))
                        || (gauge.Nadi.HasFlag(Nadi.SOLAR)
                            && gauge.Nadi.HasFlag(Nadi.LUNAR)
                            && gauge.CoeurlFury == 0)))
                || (HasEffect(MNK.Buffs.FormlessFist)
                    && gauge.CoeurlFury == 0))
            {
                if (gauge.CoeurlFury == 0 && level >= MNK.Levels.Demolish)
                    return MNK.Demolish;
                return MNK.SnapPunch;
            }

            if (gauge.OpoOpoFury == 0 && level >= MNK.Levels.DragonKick)
                return MNK.DragonKick;
            return MNK.Bootshine;
        }


        return actionID;
    }
}

internal class MonkAoECombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkMonkeyMode;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.ArmOfTheDestroyer)
        {
            var gauge = GetJobGauge<MNKGauge>();
            var disciplinedFist = FindEffect(MNK.Buffs.DisciplinedFist);
            var riddleOfFire = FindEffect(MNK.Buffs.RiddleOfFire);

            // NO GCDs
            if (GCDClipCheck(actionID))
            {
                if (
                    level >= MNK.Levels.PerfectBalance
                    && GetRemainingCharges(MNK.PerfectBalance) >= 1
                    && InCombat()
                    && HasTarget()
                    && !HasEffect(MNK.Buffs.FormlessFist)
                    && disciplinedFist is not null
                    && OriginalHook(MNK.MasterfulBlitz) == MNK.MasterfulBlitz
                    && (
                        riddleOfFire?.RemainingTime >= 8
                        || GetCooldown(MNK.PerfectBalance).ChargeCooldownRemaining <= 6
                        || GetRemainingCharges(MNK.PerfectBalance) == 2
                        || GetCooldown(MNK.RiddleOfFire).CooldownRemaining < 10
                        || level < MNK.Levels.RiddleOfFire
                    )
                    && !HasEffect(MNK.Buffs.PerfectBalance)
                )
                    return MNK.PerfectBalance;

                if (
                    level >= MNK.Levels.RiddleOfFire
                    && InCombat()
                    && HasTarget()
                    && IsOffCooldown(MNK.RiddleOfFire)
                )
                    return MNK.RiddleOfFire;

                if (
                    level >= MNK.Levels.RiddleOfWind
                    && IsOffCooldown(MNK.RiddleOfWind)
                    && GetCooldown(MNK.RiddleOfFire).CooldownRemaining > 12
                )
                    return MNK.RiddleOfWind;
            }

            var perfectBalance = FindEffect(MNK.Buffs.PerfectBalance);

            if (
                level >= MNK.Levels.MasterfulBlitz
                && !HasEffect(MNK.Buffs.PerfectBalance)
                && disciplinedFist != null
                && (
                    riddleOfFire != null
                    || gauge.BlitzTimeRemaining <= 4
                    || GetCooldown(MNK.RiddleOfFire).CooldownRemaining >= 15
                    || level < MNK.Levels.RiddleOfFire
                )
                && OriginalHook(MNK.MasterfulBlitz) != MNK.MasterfulBlitz
            )
                return OriginalHook(MNK.MasterfulBlitz);

            if (
                HasEffect(MNK.Buffs.RaptorForm)
                || (perfectBalance?.StackCount >= 3 && !gauge.Nadi.HasFlag(Nadi.SOLAR))
            )
                return level >= MNK.Levels.FourPointFury ? MNK.FourPointFury : MNK.TwinSnakes;

            if (
                HasEffect(MNK.Buffs.CoerlForm)
                || (perfectBalance?.StackCount >= 2 && !gauge.Nadi.HasFlag(Nadi.SOLAR))
            )
                return level >= MNK.Levels.Rockbreaker ? MNK.Rockbreaker : MNK.SnapPunch;

            if (HasEffect(MNK.Buffs.OpoOpoForm) || perfectBalance?.StackCount >= 1)
                return OriginalHook(MNK.ArmOfTheDestroyer);
        }

        // Blitz
        //if (level >= MNK.Levels.MasterfulBlitz && !gauge.BeastChakra.Contains(BeastChakra.NONE))
        //    return OriginalHook(MNK.MasterfulBlitz);

        if (level >= MNK.Levels.PerfectBalance && HasEffect(MNK.Buffs.PerfectBalance))
            // Solar
            //if (level >= MNK.Levels.EnhancedPerfectBalance && !gauge.Nadi.HasFlag(Nadi.SOLAR))
            //{
            //    if (level >= MNK.Levels.FourPointFury && !gauge.BeastChakra.Contains(BeastChakra.RAPTOR))
            //        return MNK.FourPointFury;
            //    if (level >= MNK.Levels.Rockbreaker && !gauge.BeastChakra.Contains(BeastChakra.COEURL))
            //        return MNK.Rockbreaker;
            //    if (level >= MNK.Levels.ArmOfTheDestroyer && !gauge.BeastChakra.Contains(BeastChakra.OPOOPO))
            //        // Shadow of the Destroyer
            //        return OriginalHook(MNK.ArmOfTheDestroyer);
            //    return level >= MNK.Levels.ShadowOfTheDestroyer
            //        ? MNK.ShadowOfTheDestroyer
            //        : MNK.Rockbreaker;
            //}
            // Lunar.  Also used if we have both Nadi as Tornado Kick/Phantom Rush isn't picky, or under 60.
            return level >= MNK.Levels.ShadowOfTheDestroyer
                ? MNK.ShadowOfTheDestroyer
                : MNK.Rockbreaker;

        // FPF with FormShift
        if (level >= MNK.Levels.FormShift && HasEffect(MNK.Buffs.FormlessFist))
            if (
                level < MNK.Levels.SnapPunch
                || FindTargetEffect(MNK.Debuffs.Demolish) == null
                || FindTargetEffect(MNK.Debuffs.Demolish)?.RemainingTime < 6.0
            )
                return MNK.Demolish;

        // 1-2-3 combo
        if (level >= MNK.Levels.FourPointFury && HasEffect(MNK.Buffs.RaptorForm))
            return MNK.FourPointFury;

        if (level >= MNK.Levels.ArmOfTheDestroyer && HasEffect(MNK.Buffs.OpoOpoForm))
            // Shadow of the Destroyer
            return OriginalHook(MNK.ArmOfTheDestroyer);

        if (level >= MNK.Levels.Rockbreaker && HasEffect(MNK.Buffs.CoerlForm))
            return MNK.Rockbreaker;

        // Shadow of the Destroyer
        return OriginalHook(MNK.ArmOfTheDestroyer);
    }
}

internal class MonkPerfectBalance : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.MonkPerfectBalanceFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.PerfectBalance)
        {
            var gauge = GetJobGauge<MNKGauge>();

            if (!gauge.BeastChakra.Contains(BeastChakra.NONE) && level >= MNK.Levels.MasterfulBlitz)
                // Chakra actions
                return OriginalHook(MNK.MasterfulBlitz);
        }

        return actionID;
    }
}

// internal class MonkRiddleOfFire : CustomCombo
// {
//     protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkAny;

//     protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//     {
//         if (actionID == MNK.RiddleOfFire)
//         {
//             var brotherhood = IsEnabled(CustomComboPreset.MonkRiddleOfFireBrotherhood);
//             var wind = IsEnabled(CustomComboPreset.MonkRiddleOfFireWind);

//             if (brotherhood && wind)
//             {
//                 if (level >= MNK.Levels.RiddleOfWind)
//                     return CalcBestAction(
//                         actionID,
//                         MNK.RiddleOfFire,
//                         MNK.Brotherhood,
//                         MNK.RiddleOfWind
//                     );

//                 if (level >= MNK.Levels.Brotherhood)
//                     return CalcBestAction(actionID, MNK.RiddleOfFire, MNK.Brotherhood);

//                 return actionID;
//             }

//             if (brotherhood)
//             {
//                 if (level >= MNK.Levels.Brotherhood)
//                     return CalcBestAction(actionID, MNK.RiddleOfFire, MNK.Brotherhood);

//                 return actionID;
//             }

//             if (wind)
//             {
//                 if (level >= MNK.Levels.RiddleOfWind)
//                     return CalcBestAction(actionID, MNK.RiddleOfFire, MNK.RiddleOfWind);

//                 return actionID;
//             }
//         }

//         return actionID;
//     }
// }