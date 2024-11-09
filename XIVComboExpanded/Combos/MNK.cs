using System.Linq;
using System.Reflection;
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
        ForbiddenChakra = 3547,
        FormShift = 4262,
        RiddleOfFire = 7395,
        Brotherhood = 7396,
        FourPointFury = 16473,
        HowlingFist = 25763,
        Mantra = 65,
        MasterfulBlitz = 25764,
        RiddleOfWind = 25766,
        RiddleOfEarth = 7394,
        EarthsReply = 36944,
        FiresReply = 36950,
        SteeledMeditation = 36940,
        WindsReply = 36949;

    public static class Buffs
    {
        public const ushort OpoOpoForm = 107,
            RaptorForm = 108,
            CoerlForm = 109,
            PerfectBalance = 110,
            WindsRumination = 3842,
            EarthsRumination = 3841,
            Brotherhood = 1185,
            RiddleOfFire = 1181,
            FormlessFist = 2513,
            FiresRumination = 3843;
    }

    // public static class Debuffs
    // {
    //     public const ushort Demolish = 246;
    // }

    public static class Levels
    {
        public const byte SnapPunch = 6,
            Meditation = 15,
            TwinSnakes = 18,
            ArmOfTheDestroyer = 26,
            Rockbreaker = 30,
            Demolish = 30,
            FourPointFury = 45,
            Mantra = 42,
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
            RiddleOfWind = 72,
            WindsReply = 96,
            FiresReply = 100;
    }
}

internal class MonkBootshine : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MnkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MNK.Bootshine)
        {
            var gauge = GetJobGauge<MNKGauge>();
            var riddleFireEffect = FindEffect(MNK.Buffs.RiddleOfFire);

            var hasRaidBuffs = HasRaidBuffs(2);

            bool riddleMeDaddy(int? skillTime)
            {
                return (
                        skillTime is not null
                        && GetCooldown(MNK.RiddleOfFire).TotalCooldownRemaining >= skillTime
                    )
                    || riddleFireEffect is not null
                    || hasRaidBuffs
                    || level < MNK.Levels.RiddleOfFire;
            }

            var distance = GetTargetDistance();

            var reprisal = FindTargetEffectAny(ADV.Debuffs.Reprisal);

            var earthsRumination = FindEffect(MNK.Buffs.EarthsRumination);

            if (GCDClipCheck(actionID) && InCombat() && InMeleeRange() && HasTarget())
                switch (level)
                {
                    case >= MNK.Levels.PerfectBalance
                        when GetRemainingCharges(MNK.PerfectBalance) >= 1
                            && !HasEffect(MNK.Buffs.FormlessFist)
                            && OriginalHook(MNK.MasterfulBlitz) == MNK.MasterfulBlitz
                            && !HasEffect(MNK.Buffs.PerfectBalance)
                            && HasEffect(MNK.Buffs.RaptorForm)
                            && (
                                riddleFireEffect?.RemainingTime
                                    >= GetCooldown(actionID).BaseCooldown * 4 // there are 4 actions
                                || GetCooldown(MNK.RiddleOfFire).TotalCooldownRemaining
                                    <= GetCooldown(actionID).BaseCooldown * 3 // use 3 actions before the next riddle
                                || TargetHasLowLife()
                                || GetCooldown(MNK.PerfectBalance).TotalCooldownRemaining <= 5
                            )
                            && (
                                !(gauge.Nadi.HasFlag(Nadi.LUNAR) && gauge.Nadi.HasFlag(Nadi.SOLAR)) // we need to save phantoms for brotherhood, so don't use it if we have both unless...
                                || HasEffect(MNK.Buffs.Brotherhood) // ...we have brotherhood
                                || GetCooldown(MNK.Brotherhood).TotalCooldownRemaining
                                    <= GetCooldown(actionID).BaseCooldown * 3 // ...or we're about to get brotherhood
                            ):
                        return MNK.PerfectBalance;
                    case >= MNK.Levels.Brotherhood
                        when IsOffCooldown(MNK.Brotherhood) && hasRaidBuffs:
                        return MNK.Brotherhood;
                    case >= MNK.Levels.RiddleOfFire
                        when IsOffCooldown(MNK.RiddleOfFire)
                            && (
                                HasEffect(MNK.Buffs.Brotherhood)
                                || hasRaidBuffs
                                || level < MNK.Levels.Brotherhood
                                || GetCooldown(MNK.Brotherhood).CooldownRemaining >= 10
                            ):
                        return MNK.RiddleOfFire;
                    case >= MNK.Levels.Meditation
                        when gauge.Chakra >= 5
                            && (level < MNK.Levels.RiddleOfFire || IsOnCooldown(MNK.RiddleOfFire)):
                        return OriginalHook(MNK.SteeledMeditation);
                    case >= MNK.Levels.RiddleOfWind
                        when IsOffCooldown(MNK.RiddleOfWind) && riddleMeDaddy(10):
                        return MNK.RiddleOfWind;
                    case >= MNK.Levels.RiddleOfEarth
                        when IsOffCooldown(MNK.RiddleOfEarth)
                            && reprisal is not null
                            && reprisal.RemainingTime >= 7:
                        return MNK.RiddleOfEarth;
                    case >= MNK.Levels.Mantra
                        when IsOffCooldown(MNK.Mantra) && LocalPlayerPercentage() <= 0.50:
                        return MNK.Mantra;
                    case >= MNK.Levels.RiddleOfEarth
                        when earthsRumination is not null
                            && (
                                LocalPlayerPercentage() is not 1
                                || earthsRumination.RemainingTime <= 5
                            ):
                        return MNK.EarthsReply;
                    case >= ADV.Levels.Feint
                        when IsOffCooldown(ADV.Feint)
                            && !TargetHasEffectAny(ADV.Debuffs.Feint)
                            && reprisal is not null
                            && reprisal.RemainingTime >= 7:
                        return ADV.Feint;
                }

            if (
                level >= MNK.Levels.FiresReply
                && CanUseAction(MNK.FiresReply)
                && (
                    HasEffect(MNK.Buffs.RaptorForm)
                    || distance >= 5
                    || FindEffect(MNK.Buffs.FiresRumination)?.RemainingTime <= 6
                )
                && !HasEffect(MNK.Buffs.FormlessFist)
            )
                return MNK.FiresReply;

            if (
                level >= MNK.Levels.MasterfulBlitz
                && !HasEffect(MNK.Buffs.PerfectBalance)
                && OriginalHook(MNK.MasterfulBlitz) != MNK.MasterfulBlitz
                && (
                    GetCooldown(MNK.Brotherhood).TotalCooldownElapsed >= 1
                    || level < MNK.Levels.Brotherhood
                )
                && (
                    gauge.BlitzTimeRemaining <= 8500 || riddleFireEffect is not null
                // || hasRaidBuffs
                )
            )
                return OriginalHook(MNK.MasterfulBlitz);

            var windsRumination = FindEffect(MNK.Buffs.WindsRumination);

            if (
                level >= MNK.Levels.WindsReply
                && GetCooldown(MNK.WindsReply).TotalCooldownElapsed >= 1
                && windsRumination is not null
                && (
                    (!HasEffect(MNK.Buffs.FormlessFist) && !HasEffect(MNK.Buffs.PerfectBalance))
                    || windsRumination.RemainingTime <= 6
                    || distance >= 5
                )
            )
            {
                return MNK.WindsReply;
            }

            if (GetTargetDistance() >= 5 || !InCombat() || !HasTarget())
            {
                if (gauge.Chakra < 5)
                    return OriginalHook(MNK.SteeledMeditation);

                if (
                    level >= MNK.Levels.FormShift
                    && !HasEffect(MNK.Buffs.FormlessFist)
                    && !HasEffect(MNK.Buffs.PerfectBalance)
                )
                    return MNK.FormShift;
            }


            if (
                (
                    HasEffect(MNK.Buffs.RaptorForm)
                    || (
                        HasEffect(MNK.Buffs.PerfectBalance)
                        && level >= MNK.Levels.MasterfulBlitz
                        && gauge.Nadi.HasFlag(Nadi.LUNAR)
                        && !(gauge.Nadi.HasFlag(Nadi.LUNAR) && gauge.Nadi.HasFlag(Nadi.SOLAR))
                        && (
                            level < MNK.Levels.Brotherhood
                            // this enables double lunar initially so that we can phantom rush in even windows
                            || GetCooldown(MNK.Brotherhood).CooldownElapsed >= 15
                            || TargetHPercentage() < 0.85
                        )
                        && !gauge.BeastChakra.Contains(BeastChakra.RAPTOR)
                    )
                )
            )
            {
                if (gauge.RaptorFury == 0 && level >= MNK.Levels.TwinSnakes)
                    return MNK.TwinSnakes;

                return OriginalHook(MNK.TrueStrike);
            }

            if (
                (
                    HasEffect(MNK.Buffs.CoerlForm)
                    || (
                        HasEffect(MNK.Buffs.PerfectBalance)
                        && level >= MNK.Levels.MasterfulBlitz
                        && gauge.Nadi.HasFlag(Nadi.LUNAR)
                        && !(gauge.Nadi.HasFlag(Nadi.LUNAR) && gauge.Nadi.HasFlag(Nadi.SOLAR))
                        && (
                            level < MNK.Levels.Brotherhood
                            // this enables double lunar initially so that we can phantom rush in even windows
                            || GetCooldown(MNK.Brotherhood).CooldownElapsed >= 15
                            //
                            || TargetHPercentage() < 0.85
                        )
                        && !gauge.BeastChakra.Contains(BeastChakra.COEURL)
                    )
                )
            )
            {
                if (gauge.CoeurlFury == 0 && level >= MNK.Levels.Demolish)
                    return MNK.Demolish;

                return OriginalHook(MNK.SnapPunch);
            }

            if (gauge.OpoOpoFury == 0 && level >= MNK.Levels.DragonKick)
                return MNK.DragonKick;

            return OriginalHook(MNK.Bootshine);
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

            bool riddleMeDaddy()
            {
                return HasEffect(MNK.Buffs.RiddleOfFire)
                    || HasRaidBuffs(2)
                    || level < MNK.Levels.RiddleOfFire;
            }

            if (GCDClipCheck(actionID) && InCombat())
            {
                if (gauge.Chakra >= 5)
                    return level >= MNK.Levels.HowlingFist
                        ? OriginalHook(MNK.HowlingFist)
                        : OriginalHook(MNK.ForbiddenChakra);

                switch (level)
                {
                    case >= MNK.Levels.PerfectBalance
                        when GetRemainingCharges(MNK.PerfectBalance) >= 1
                            && !HasEffect(MNK.Buffs.FormlessFist)
                            && OriginalHook(MNK.MasterfulBlitz) == MNK.MasterfulBlitz
                            && !HasEffect(MNK.Buffs.PerfectBalance)
                            && (
                                riddleMeDaddy()
                                || GetCooldown(MNK.PerfectBalance).TotalCooldownRemaining <= 8
                            ):
                        return MNK.PerfectBalance;
                    case >= MNK.Levels.RiddleOfFire
                        when InMeleeRange()
                            && (IsOnCooldown(MNK.Brotherhood) || level <= MNK.Levels.Brotherhood)
                            && IsOffCooldown(MNK.RiddleOfFire):
                        return MNK.RiddleOfFire;
                    case >= MNK.Levels.RiddleOfWind
                        when IsOffCooldown(MNK.RiddleOfWind) && InMeleeRange():
                        return MNK.RiddleOfWind;
                    case >= MNK.Levels.RiddleOfEarth
                        when LocalPlayerPercentage() <= 0.98
                            && HasEffect(MNK.Buffs.EarthsRumination):
                        return MNK.EarthsReply;
                }
            }

            if (GetTargetDistance() >= 7 || !InCombat() || !HasTarget())
            {
                if (gauge.Chakra < 5)
                    return OriginalHook(MNK.SteeledMeditation);

                if (
                    level >= MNK.Levels.FormShift
                    && !HasEffect(MNK.Buffs.FormlessFist)
                    && !HasEffect(MNK.Buffs.PerfectBalance)
                )
                    return MNK.FormShift;
            }

            if (
                level >= 100
                && CanUseAction(MNK.FiresReply)
                && (
                    HasEffect(MNK.Buffs.RaptorForm)
                    || FindEffect(MNK.Buffs.FiresRumination)?.RemainingTime <= 8
                )
            )
                return MNK.FiresReply;

            var perfectBalance = FindEffect(MNK.Buffs.PerfectBalance);

            if (
                level >= MNK.Levels.MasterfulBlitz
                && !HasEffect(MNK.Buffs.PerfectBalance)
                && OriginalHook(MNK.MasterfulBlitz) != MNK.MasterfulBlitz
            )
                return OriginalHook(MNK.MasterfulBlitz);

            if (CanUseAction(MNK.WindsReply))
                return MNK.WindsReply;

            if (
                HasEffect(MNK.Buffs.RaptorForm)
                || (
                    perfectBalance?.StackCount >= 3
                    && gauge.Nadi.HasFlag(Nadi.LUNAR)
                    && (
                        level < MNK.Levels.Brotherhood
                        || GetCooldown(MNK.Brotherhood).CooldownElapsed >= 20
                    )
                )
            )
                return level >= MNK.Levels.FourPointFury ? MNK.FourPointFury : MNK.TwinSnakes;

            if (
                HasEffect(MNK.Buffs.CoerlForm)
                || (
                    perfectBalance?.StackCount >= 2
                    && gauge.Nadi.HasFlag(Nadi.LUNAR)
                    && (
                        level < MNK.Levels.Brotherhood
                        || GetCooldown(MNK.Brotherhood).CooldownElapsed >= 20
                    )
                )
            )
                return level >= MNK.Levels.Rockbreaker ? MNK.Rockbreaker : MNK.SnapPunch;

            if (HasEffect(MNK.Buffs.OpoOpoForm) || perfectBalance?.StackCount >= 1)
                return OriginalHook(MNK.ArmOfTheDestroyer);
        }

        return actionID;
    }
}
