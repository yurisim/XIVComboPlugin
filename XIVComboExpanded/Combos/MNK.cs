using System;
using System.Linq;

using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class MNK
{
    public const byte ClassID = 2;
    public const byte JobID = 20;

    public const uint
        Bootshine = 53,
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
        HowlingFist = 25763,
        MasterfulBlitz = 25764,
        RiddleOfWind = 25766,
        SteelPeak = 25761,
        ShadowOfTheDestroyer = 25767;

    public static class Buffs
    {
        public const ushort
            OpoOpoForm = 107,
            RaptorForm = 108,
            CoerlForm = 109,
            PerfectBalance = 110,
            LeadenFist = 1861,
            FormlessFist = 2513,
            DisciplinedFist = 3001;
    }

    public static class Debuffs
    {
        public const ushort
            Demolish = 246;
    }

    public static class Levels
    {
        public const byte
            Bootshine = 1,
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

internal class MonkAoECombo : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkAoECombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.ArmOfTheDestroyer)
        {
            var gauge = GetJobGauge<MNKGauge>();

            if (HasEffect(MNK.Buffs.DisciplinedFist))
            {
                if (gauge.Chakra >= 5 && GCDClipCheck(actionID))
                {
                    if (level >= MNK.Levels.RiddleOfFire
                        && GCDClipCheck(actionID)
                        && IsOffCooldown(MNK.RiddleOfFire))
                    {
                        return MNK.RiddleOfFire;
                    }
                    


                    return level >= MNK.Levels.HowlingFist ? OriginalHook(MNK.HowlingFist) : OriginalHook(MNK.SteelPeak);
                }
            }

            var perfectBalance = FindEffect(MNK.Buffs.PerfectBalance);

            if (HasEffect(MNK.Buffs.CoerlForm)
                || (!gauge.Nadi.HasFlag(Nadi.SOLAR) && perfectBalance?.StackCount >= 3)
                || (gauge.Nadi.HasFlag(Nadi.SOLAR)
                    && gauge.Nadi.HasFlag(Nadi.LUNAR)
                    && HasEffect(MNK.Buffs.PerfectBalance)))
            {
                if (level >= MNK.Levels.Rockbreaker)
                {
                    return MNK.Rockbreaker;
                }
            }

            if (HasEffect(MNK.Buffs.RaptorForm)
                || (!gauge.Nadi.HasFlag(Nadi.SOLAR) && perfectBalance?.StackCount >= 2))
            {

                if (level >= MNK.Levels.FourPointFury)
                {
                    return MNK.FourPointFury;
                }
            }

            if (HasEffect(MNK.Buffs.DisciplinedFist))
            {
                if (level >= MNK.Levels.MasterfulBlitz
                    && (CanUseAction(MNK.FlintStrike)
                        || CanUseAction(MNK.ElixirField)
                        || CanUseAction(MNK.TornadoKick)
                        ))
                {
                    if (level >= MNK.Levels.RiddleOfFire
                        && GCDClipCheck(actionID)
                        && IsOffCooldown(MNK.RiddleOfFire))
                        return MNK.RiddleOfFire;

                    return OriginalHook(MNK.MasterfulBlitz);
                }


                if (level >= MNK.Levels.PerfectBalance
                    && GCDClipCheck(actionID)
                    && GetRemainingCharges(MNK.PerfectBalance) > 0
                    && !HasEffect(MNK.Buffs.PerfectBalance))
                {
                    return MNK.PerfectBalance;

                }
            }

            if (HasEffect(MNK.Buffs.OpoOpoForm)
                || (!gauge.Nadi.HasFlag(Nadi.SOLAR) || perfectBalance?.StackCount >= 1)
                )
            {
                return MNK.ArmOfTheDestroyer;
            }

        }

        return actionID;
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

            if (level >= MNK.Levels.Meditation
                && gauge.Chakra < 5
                && (!InMeleeRange()
                    || !InCombat()))
                return MNK.Meditation;

            if (level >= MNK.Levels.FormShift
                && !HasEffect(MNK.Buffs.FormlessFist)
                && !HasEffect(MNK.Buffs.PerfectBalance)
                && (!InMeleeRange()
                    || !InCombat()))
            {
                return MNK.FormShift;
            }

            if (HasEffect(MNK.Buffs.DisciplinedFist))
            {
                if (gauge.Chakra >= 5 && GCDClipCheck(actionID))
                {
                    if (level >= MNK.Levels.RiddleOfFire
                        && GCDClipCheck(actionID)
                        && IsOffCooldown(MNK.RiddleOfFire))
                        return MNK.RiddleOfFire;

                    return OriginalHook(MNK.SteelPeak);
                }
            }


            //if (level >= MNK.Levels.MasterfulBlitz && !gauge.BeastChakra.Contains(BeastChakra.NONE))
            //    return OriginalHook(MNK.MasterfulBlitz);

            var perfectBalance = FindEffect(MNK.Buffs.PerfectBalance);
            
            var demolish = FindTargetEffect(MNK.Debuffs.Demolish);
            if (HasEffect(MNK.Buffs.CoerlForm)
                || (!gauge.Nadi.HasFlag(Nadi.SOLAR) && perfectBalance?.StackCount >= 3)
                || (demolish == null && HasEffect(MNK.Buffs.FormlessFist)))
            {
                if ((demolish == null
                        || demolish?.RemainingTime <= 6)
                    && level >= MNK.Levels.Demolish)
                {
                    return MNK.Demolish;
                }
                else
                {
                    return MNK.SnapPunch;
                }
            }
            
            var disciplinedFist = FindEffect(MNK.Buffs.DisciplinedFist);

            if (HasEffect(MNK.Buffs.RaptorForm)
                || (!gauge.Nadi.HasFlag(Nadi.SOLAR) && perfectBalance?.StackCount >= 2)
                || (disciplinedFist == null && HasEffect(MNK.Buffs.FormlessFist))
                )
            {
                if ((disciplinedFist == null
                        || disciplinedFist?.RemainingTime <= 6)
                    && level >= MNK.Levels.TwinSnakes)
                {
                    return MNK.TwinSnakes;
                }
                else
                {
                    return MNK.TrueStrike;
                }
            }

            if (HasEffect(MNK.Buffs.DisciplinedFist))
            {
                if (level >= MNK.Levels.MasterfulBlitz
                    && (CanUseAction(MNK.FlintStrike)
                        || CanUseAction(MNK.ElixirField)
                        || CanUseAction(MNK.TornadoKick)
                        ))
                {
                    if (level >= MNK.Levels.RiddleOfFire
                        && GCDClipCheck(actionID)
                        && IsOffCooldown(MNK.RiddleOfFire))
                        return MNK.RiddleOfFire;

                    return OriginalHook(MNK.MasterfulBlitz);
                }


                if (level >= MNK.Levels.PerfectBalance
                    && GetRemainingCharges(MNK.PerfectBalance) > 0
                    && !HasEffect(MNK.Buffs.PerfectBalance)
                    && TargetHasEffect(MNK.Debuffs.Demolish))
                {
                    return MNK.PerfectBalance;

                }
            }

            if (HasEffect(MNK.Buffs.OpoOpoForm)
                || (!gauge.Nadi.HasFlag(Nadi.LUNAR) || perfectBalance?.StackCount >= 1)
                || (gauge.Nadi.HasFlag(Nadi.SOLAR) && gauge.Nadi.HasFlag(Nadi.LUNAR))
                )
            {
                if (!HasEffect(MNK.Buffs.LeadenFist) && level >= MNK.Levels.DragonKick)
                {
                    return MNK.DragonKick;
                }
                else
                {
                    return MNK.Bootshine;
                }
            }

        }

        return actionID;
    }
}

internal class MonkPerfectBalance : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkPerfectBalanceFeature;

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
                    return CalcBestAction(actionID, MNK.RiddleOfFire, MNK.Brotherhood, MNK.RiddleOfWind);

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
