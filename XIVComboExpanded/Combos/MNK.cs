using System;
using System.Linq;
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
        ElixirField = 3545,
        FlintStrike = 25882,
        TornadoKick = 3543,
        Meditation = 3546,
        TheForbiddenChakra = 3547,
        FormShift = 4262,
        RiddleOfFire = 7395,
        Brotherhood = 7396,
        FourPointFury = 16473,
        Enlightenment = 16474,
        MasterfulBlitz = 25764,
        RiddleOfWind = 25766,
        SteelPeak = 25761,
        ShadowOfTheDestroyer = 25767;

    public static class Buffs
    {
        public const ushort OpoOpoForm = 107,
            RaptorForm = 108,
            CoerlForm = 109,
            PerfectBalance = 110,
            Brotherhood = 1182,
            RiddleOfFire = 1181,
            LeadenFist = 1861,
            FormlessFist = 2513,
            DisciplinedFist = 3001;
    }

    public static class Debuffs
    {
        public const ushort Demolish = 246;
    }

    public static class Levels
    {
        public const byte Bootshine = 1,
            TrueStrike = 4,
            SnapPunch = 6,
            Meditation = 15,
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
            RiddleOfFire = 68,
            Brotherhood = 70,
            Enlightenment = 70,
            RiddleOfWind = 72,
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

            var brotherhoodCD = GetCooldown(MNK.Brotherhood).CooldownRemaining;

            var disciplinedFist = FindEffect(MNK.Buffs.DisciplinedFist);
            var refreshFist = disciplinedFist == null || disciplinedFist?.RemainingTime <= 5;

            var riddleOfFire = FindEffect(MNK.Buffs.RiddleOfFire);

            var demolish = FindTargetEffect(MNK.Debuffs.Demolish);
            var refreshDemolish = demolish == null || demolish?.RemainingTime <= 5;

            var doesNotHaveSolar = !gauge.Nadi.HasFlag(Nadi.SOLAR);

            var hasSolarLunar = gauge.Nadi.HasFlag(Nadi.SOLAR) && gauge.Nadi.HasFlag(Nadi.LUNAR);

            var medicated = FindEffect(ADV.Buffs.Medicated);

            var medicatedGood = medicated is not null && medicated.RemainingTime >= 8;

            // NO GCDs
            if (GCDClipCheck(actionID))
            {
                if (
                    gauge.Chakra >= 5
                    && (
                        GetCooldown(MNK.RiddleOfFire).CooldownRemaining > 2
                        || level < MNK.Levels.RiddleOfFire
                    )
                    && disciplinedFist != null
                    && InCombat()
                    && HasTarget()
                )
                {
                    return OriginalHook(MNK.SteelPeak);
                }

                if (
                    level >= MNK.Levels.PerfectBalance
                    && GetRemainingCharges(MNK.PerfectBalance) >= 1
                    && InCombat()
                    && HasTarget()
                    && (demolish is not null || doesNotHaveSolar || hasSolarLunar)
                    && !HasEffect(MNK.Buffs.FormlessFist)
                    && disciplinedFist is not null
                    && (disciplinedFist.RemainingTime >= 8 || doesNotHaveSolar || hasSolarLunar)
                    && (OriginalHook(MNK.MasterfulBlitz) == MNK.MasterfulBlitz)
                    && (
                        riddleOfFire?.RemainingTime >= 8
                        || medicatedGood
                        || (GetCooldown(MNK.PerfectBalance).ChargeCooldownRemaining <= 6)
                        || GetRemainingCharges(MNK.PerfectBalance) == 2
                        || GetCooldown(MNK.RiddleOfFire).CooldownRemaining < 10
                        || level < MNK.Levels.RiddleOfFire
                    )
                    && !HasEffect(MNK.Buffs.PerfectBalance)
                )
                {
                    return MNK.PerfectBalance;
                }

                if (
                    level >= MNK.Levels.RiddleOfFire
                    && (brotherhoodCD > 6 || level < MNK.Levels.Brotherhood)
                    && InCombat()
                    && HasTarget()
                    && InMeleeRange()
                    && IsOffCooldown(MNK.RiddleOfFire)
                )
                {
                    return MNK.RiddleOfFire;
                }

                if (
                    level >= MNK.Levels.RiddleOfWind
                    && IsOffCooldown(MNK.RiddleOfWind)
                    && (GetCooldown(MNK.RiddleOfFire).CooldownRemaining > 9)
                    && InCombat()
                    && HasTarget()
                    && InMeleeRange()
                )
                {
                    return MNK.RiddleOfWind;
                }
            }

            // Ranged GCDs
            if (GetTargetDistance() >= 7 || !InCombat() || !HasTarget())
            {
                if (level >= MNK.Levels.Meditation && gauge.Chakra < 5)
                {
                    return MNK.Meditation;
                }

                if (
                    level >= MNK.Levels.FormShift
                    && !HasEffect(MNK.Buffs.FormlessFist)
                    && !HasEffect(MNK.Buffs.PerfectBalance)
                )
                {
                    return MNK.FormShift;
                }
            }

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
                && (OriginalHook(MNK.MasterfulBlitz) != MNK.MasterfulBlitz)
            )
            {
                return OriginalHook(MNK.MasterfulBlitz);
            }

            if (
                HasEffect(MNK.Buffs.RaptorForm)
                || (
                    HasEffect(MNK.Buffs.PerfectBalance)
                    && (level >= MNK.Levels.MasterfulBlitz)
                    && (
                        (
                            !gauge.Nadi.HasFlag(Nadi.SOLAR)
                            && !gauge.BeastChakra.Contains(BeastChakra.RAPTOR)
                        )
                        || (
                            gauge.Nadi.HasFlag(Nadi.SOLAR)
                            && gauge.Nadi.HasFlag(Nadi.LUNAR)
                            && refreshFist
                        )
                    )
                )
                || (HasEffect(MNK.Buffs.FormlessFist) && refreshFist)
            )
            {
                if (refreshFist && level >= MNK.Levels.TwinSnakes)
                {
                    return MNK.TwinSnakes;
                }
                else
                {
                    return MNK.TrueStrike;
                }
            }

            if (
                HasEffect(MNK.Buffs.CoerlForm)
                || (
                    HasEffect(MNK.Buffs.PerfectBalance)
                    && (level >= MNK.Levels.MasterfulBlitz)
                    && (
                        (
                            !gauge.Nadi.HasFlag(Nadi.SOLAR)
                            && !gauge.BeastChakra.Contains(BeastChakra.COEURL)
                        )
                        || (
                            gauge.Nadi.HasFlag(Nadi.SOLAR)
                            && gauge.Nadi.HasFlag(Nadi.LUNAR)
                            && refreshDemolish
                        )
                    )
                )
                || (HasEffect(MNK.Buffs.FormlessFist) && refreshDemolish)
            )
            {
                if (refreshDemolish && level >= MNK.Levels.Demolish)
                {
                    return MNK.Demolish;
                }
                else
                {
                    return MNK.SnapPunch;
                }
            }

            if (!HasEffect(MNK.Buffs.LeadenFist))
            {
                return MNK.DragonKick;
            }
            else
            {
                return MNK.Bootshine;
            }
        }

        return actionID;
    }
}

internal class MonkAoECombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkAny;

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
                    && (OriginalHook(MNK.MasterfulBlitz) == MNK.MasterfulBlitz)
                    && (
                        riddleOfFire?.RemainingTime >= 8
                        || (GetCooldown(MNK.PerfectBalance).ChargeCooldownRemaining <= 6)
                        || GetRemainingCharges(MNK.PerfectBalance) == 2
                        || GetCooldown(MNK.RiddleOfFire).CooldownRemaining < 10
                        || level < MNK.Levels.RiddleOfFire
                    )
                    && !HasEffect(MNK.Buffs.PerfectBalance)
                )
                {
                    return MNK.PerfectBalance;
                }

                if (
                    level >= MNK.Levels.RiddleOfFire
                    && InCombat()
                    && HasTarget()
                    && IsOffCooldown(MNK.RiddleOfFire)
                )
                {
                    return MNK.RiddleOfFire;
                }

                if (
                    level >= MNK.Levels.RiddleOfWind
                    && IsOffCooldown(MNK.RiddleOfWind)
                    && (GetCooldown(MNK.RiddleOfFire).CooldownRemaining > 12)
                )
                {
                    return MNK.RiddleOfWind;
                }

                if (
                    gauge.Chakra >= 5
                    && (
                        GetCooldown(MNK.RiddleOfFire).CooldownRemaining > 2
                        || level < MNK.Levels.RiddleOfFire
                    )
                    && disciplinedFist != null
                    && InCombat()
                    && HasTarget()
                )
                {
                    // Idk why howling fist isn't working
                    return level >= MNK.Levels.Enlightenment
                        ? OriginalHook(MNK.Enlightenment)
                        : OriginalHook(MNK.SteelPeak);
                }
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
                && (OriginalHook(MNK.MasterfulBlitz) != MNK.MasterfulBlitz)
            )
            {
                return OriginalHook(MNK.MasterfulBlitz);
            }

            if (
                HasEffect(MNK.Buffs.RaptorForm)
                || (perfectBalance?.StackCount >= 3 && !gauge.Nadi.HasFlag(Nadi.SOLAR))
            )
            {
                return (level >= MNK.Levels.FourPointFury) ? MNK.FourPointFury : MNK.TwinSnakes;
            }

            if (
                HasEffect(MNK.Buffs.CoerlForm)
                || (perfectBalance?.StackCount >= 2 && !gauge.Nadi.HasFlag(Nadi.SOLAR))
            )
            {
                return (level >= MNK.Levels.Rockbreaker) ? MNK.Rockbreaker : MNK.SnapPunch;
            }

            if (HasEffect(MNK.Buffs.OpoOpoForm) || (perfectBalance?.StackCount >= 1))
            {
                return OriginalHook(MNK.ArmOfTheDestroyer);
            }
        }

        return actionID;
    }
}

internal class MonkSnapPunch : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.SnapPunch)
        {
            if (IsEnabled(CustomComboPreset.MonkSnapPunchFeature))
            {
                if (
                    level < MNK.Levels.SnapPunch
                    || FindTargetEffect(MNK.Debuffs.Demolish) == null
                    || FindTargetEffect(MNK.Debuffs.Demolish)?.RemainingTime < 6.0
                )
                    return MNK.Demolish;
            }
        }

        return actionID;
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

internal class MonkRiddleOfFire : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.RiddleOfFire)
        {
            var brotherhood = IsEnabled(CustomComboPreset.MonkRiddleOfFireBrotherhood);
            var wind = IsEnabled(CustomComboPreset.MonkRiddleOfFireWind);

            if (brotherhood && wind)
            {
                if (level >= MNK.Levels.RiddleOfWind)
                    return CalcBestAction(
                        actionID,
                        MNK.RiddleOfFire,
                        MNK.Brotherhood,
                        MNK.RiddleOfWind
                    );

                if (level >= MNK.Levels.Brotherhood)
                    return CalcBestAction(actionID, MNK.RiddleOfFire, MNK.Brotherhood);

                return actionID;
            }

            if (brotherhood)
            {
                if (level >= MNK.Levels.Brotherhood)
                    return CalcBestAction(actionID, MNK.RiddleOfFire, MNK.Brotherhood);

                return actionID;
            }

            if (wind)
            {
                if (level >= MNK.Levels.RiddleOfWind)
                    return CalcBestAction(actionID, MNK.RiddleOfFire, MNK.RiddleOfWind);

                return actionID;
            }
        }

        return actionID;
    }
}
