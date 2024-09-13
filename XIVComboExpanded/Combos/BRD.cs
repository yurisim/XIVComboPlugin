using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class BRD
{
    public const byte ClassID = 5;
    public const byte JobID = 23;

    public const uint HeavyShot = 97,
        StraightShot = 98,
        VenomousBite = 100,
        RagingStrikes = 101,
        QuickNock = 106,
        Barrage = 107,
        Bloodletter = 110,
        Windbite = 113,
        MagesBallad = 114,
        ArmysPaeon = 116,
        RainOfDeath = 117,
        BattleVoice = 118,
        EmpyrealArrow = 3558,
        WanderersMinuet = 3559,
        IronJaws = 3560,
        Sidewinder = 3562,
        PitchPerfect = 7404,
        CausticBite = 7406,
        Stormbite = 7407,
        RefulgentArrow = 7409,
        BurstShot = 16495,
        ApexArrow = 16496,
        Shadowbite = 16494,
        Ladonsbite = 25783,
        BlastArrow = 25784,
        RadiantFinale = 25785,
        WideVolley = 36974,
        HeartbreakShot = 36975,
        ResonantArrow = 36976,
        RadiantEncore = 36977;

    public static class Buffs
    {
        public const ushort RagingStrikes = 125,
            Barrage = 128,
            BattleVoice = 141,
            WanderersMinuet = 2009,
            BlastShotReady = 2692,
            ShadowbiteReady = 3002,
            HawksEye = 3861,
            ResonantArrowReady = 3862,
            RadiantEncoreReady = 3863;
    }

    public static class Debuffs
    {
        public const ushort VenomousBite = 124,
            Windbite = 129,
            CausticBite = 1200,
            Stormbite = 1201;
    }

    public static class Levels
    {
        public const byte StraightShot = 2,
            RagingStrikes = 4,
            VenomousBite = 6,
            Bloodletter = 12,
            WideVolley = 25,
            MagesBallad = 30,
            Windbite = 30,
            Barrage = 38,
            ArmysPaeon = 40,
            RainOfDeath = 45,
            BattleVoice = 50,
            WanderersMinuet = 52,
            PitchPerfect = 52,
            EmpyrealArrow = 54,
            IronJaws = 56,
            Sidewinder = 60,
            BiteUpgrade = 64,
            RefulgentArrow = 70,
            Shadowbite = 72,
            BurstShot = 76,
            ApexArrow = 80,
            Ladonsbite = 82,
            EnhancedBloodLetter = 84,
            BlastShot = 86,
            RadiantFinale = 90,
            HeartbreakShot = 92,
            ResonantArrow = 96,
            RadiantEncore = 100;
    }
}

internal class BardHeavyShot : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BrdAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.HeavyShot || actionID == BRD.BurstShot)
        {
            var gauge = GetJobGauge<BRDGauge>();

            var ragingStrikesCD = GetCooldown(BRD.RagingStrikes).CooldownRemaining;

            if (GCDClipCheck(actionID))
            {
                var bloodLetterCD = GetCooldown(BRD.Bloodletter).TotalCooldownRemaining;


                if (GCDClipCheck(actionID))
                {
                    if (level >= BRD.Levels.PitchPerfect // Be the right level
                        && gauge.Song == Song.WANDERER // be the right song
                        && (gauge.Repertoire == 3 || (gauge.Repertoire >= 1 && gauge.SongTimer <= 3000))
                        )
                    {
                        return BRD.PitchPerfect;
                    }


                    if (InCombat())
                    {
                        switch (level)
                        {
                            case >= BRD.Levels.WanderersMinuet when IsOffCooldown(BRD.WanderersMinuet) && (gauge.Song == Song.ARMY || gauge.Song == Song.NONE) && gauge.SongTimer <= 3000 && InCombat():
                                return BRD.WanderersMinuet;

                            case >= BRD.Levels.MagesBallad when IsOffCooldown(BRD.MagesBallad) && (gauge.Song == Song.WANDERER || gauge.Song == Song.NONE) && gauge.SongTimer <= 3000 && InCombat():
                                return BRD.MagesBallad;

                            case >= BRD.Levels.ArmysPaeon when IsOffCooldown(BRD.ArmysPaeon) && (gauge.Song == Song.MAGE || gauge.Song == Song.NONE) && ((gauge.SongTimer <= 12000 && level >= BRD.Levels.WanderersMinuet) || (gauge.SongTimer <= 3000 && level < BRD.Levels.WanderersMinuet)):
                                return BRD.ArmysPaeon;
                        }
                    }

                    switch (level)
                    {
                        case >= BRD.Levels.RadiantFinale when IsOffCooldown(BRD.RadiantFinale) && gauge.Coda.Length >= 1 && (HasRaidBuffs() || HasEffect(BRD.Buffs.RagingStrikes)):
                            return BRD.RadiantFinale;

                        case >= BRD.Levels.BattleVoice when HasEffect(BRD.Buffs.RagingStrikes) && IsOffCooldown(BRD.BattleVoice):
                            return BRD.BattleVoice;

                        case >= BRD.Levels.Barrage when IsOffCooldown(BRD.Barrage) && (HasEffect(BRD.Buffs.RagingStrikes) || ragingStrikesCD >= 18):
                            return BRD.Barrage;

                        case >= BRD.Levels.Sidewinder when IsOffCooldown(BRD.Sidewinder) && ragingStrikesCD >= 9:
                            return BRD.Sidewinder;

                        case >= BRD.Levels.EmpyrealArrow when IsOffCooldown(BRD.EmpyrealArrow):
                            return BRD.EmpyrealArrow;

                        case >= BRD.Levels.Bloodletter when HasCharges(BRD.Bloodletter) && (HasEffect(BRD.Buffs.RagingStrikes) || HasRaidBuffs() || (gauge.Song == Song.MAGE && bloodLetterCD <= 22) || (bloodLetterCD <= 15)):
                            return BRD.Bloodletter;
                    }
                }

            }

            if (HasEffect(BRD.Buffs.Barrage))
            {
                return OriginalHook(BRD.StraightShot);
            }

            var refreshTime = 6;

            if (level >= BRD.Levels.IronJaws)
            {
                if (level < BRD.Levels.BiteUpgrade)
                {
                    var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbite = FindTargetEffect(BRD.Debuffs.Windbite);

                    if (
                        venomous?.RemainingTime <= refreshTime
                        || windbite?.RemainingTime <= refreshTime
                    )
                        return BRD.IronJaws;
                }

                var caustic = FindTargetEffect(BRD.Debuffs.CausticBite);
                var stormbite = FindTargetEffect(BRD.Debuffs.Stormbite);

                if (
                    caustic?.RemainingTime <= refreshTime
                    || stormbite?.RemainingTime <= refreshTime
                )
                    return BRD.IronJaws;
            }
            else
            {
                var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
                var windbite = FindTargetEffect(BRD.Debuffs.Windbite);

                if (venomous?.RemainingTime <= refreshTime)
                    return BRD.VenomousBite;

                if (windbite?.RemainingTime <= refreshTime)
                    return BRD.Windbite;
            }

            if (level >= BRD.Levels.BlastShot && HasEffect(BRD.Buffs.BlastShotReady))
                return BRD.BlastArrow;

            if (
                level >= BRD.Levels.ApexArrow
                && (
                    (
                        gauge.SoulVoice >= 80
                        && HasEffect(BRD.Buffs.RagingStrikes)
                        && HasEffect(BRD.Buffs.BattleVoice)
                    ) || (gauge.SoulVoice == 100 && ragingStrikesCD >= 12)
                )
            )
                return BRD.ApexArrow;

            if (level >= BRD.Levels.StraightShot && HasEffect(BRD.Buffs.HawksEye))
                // Refulgent Arrow
                return OriginalHook(BRD.StraightShot);
        }

        return actionID;
    }
}

internal class BardIronJaws : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BrdAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.IronJaws)
        {
            var gauge = GetJobGauge<BRDGauge>();

            if (GCDClipCheck(actionID))
            {
                if (
                    level >= BRD.Levels.WanderersMinuet
                    && IsOffCooldown(BRD.WanderersMinuet)
                    && (gauge.Song == Song.ARMY || gauge.Song == Song.NONE)
                    && gauge.SongTimer <= 3000
                )
                    return BRD.WanderersMinuet;

                if (
                    level >= BRD.Levels.MagesBallad
                    && IsOffCooldown(BRD.MagesBallad)
                    && (gauge.Song == Song.WANDERER || gauge.Song == Song.NONE)
                    && gauge.SongTimer <= 3000
                )
                    return BRD.MagesBallad;

                if (
                    level >= BRD.Levels.ArmysPaeon
                    && IsOffCooldown(BRD.ArmysPaeon)
                    && (gauge.Song == Song.MAGE || gauge.Song == Song.NONE)
                    && (
                        (gauge.SongTimer <= 12000 && level >= BRD.Levels.WanderersMinuet)
                        || (gauge.SongTimer <= 3000 && level < BRD.Levels.WanderersMinuet)
                    )
                )
                    return BRD.ArmysPaeon;
            }

            if (level < BRD.Levels.Windbite)
                return BRD.VenomousBite;

            if (level < BRD.Levels.IronJaws)
            {
                var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
                var windbite = FindTargetEffect(BRD.Debuffs.Windbite);

                if (venomous is null)
                    return BRD.VenomousBite;

                if (windbite is null)
                    return BRD.Windbite;

                if (venomous?.RemainingTime < windbite?.RemainingTime)
                    return BRD.VenomousBite;

                return BRD.Windbite;
            }

            if (level < BRD.Levels.BiteUpgrade)
            {
                var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                var windbite = TargetHasEffect(BRD.Debuffs.Windbite);

                if (venomous && windbite)
                    return BRD.IronJaws;

                if (windbite)
                    return BRD.VenomousBite;

                return BRD.Windbite;
            }

            var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
            var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);

            if (caustic && stormbite)
                return BRD.IronJaws;

            if (stormbite)
                return BRD.CausticBite;

            return BRD.Stormbite;
        }

        return actionID;
    }
}

internal class BardQuickNock : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BrdAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.QuickNock || actionID == BRD.Ladonsbite)
        {
            var gauge = GetJobGauge<BRDGauge>();

            if (GCDClipCheck(actionID))
            {
                if (
                    level >= BRD.Levels.PitchPerfect // Be the right level
                    && gauge.Song == Song.WANDERER // be the right song
                    && (gauge.Repertoire == 3 || (gauge.Repertoire >= 1 && gauge.SongTimer <= 2500))
                ) // You either have all stacks or you have 1 stack and the song timer is under 2500ms
                {
                    return BRD.PitchPerfect;
                }

                if (
                    level >= BRD.Levels.WanderersMinuet
                    && IsOffCooldown(BRD.WanderersMinuet)
                    && (gauge.Song == Song.ARMY || gauge.Song == Song.NONE)
                    && gauge.SongTimer <= 3000
                )
                    return BRD.WanderersMinuet;

                if (
                    level >= BRD.Levels.MagesBallad
                    && IsOffCooldown(BRD.MagesBallad)
                    && (gauge.Song == Song.WANDERER || gauge.Song == Song.NONE)
                    && gauge.SongTimer <= 3000
                )
                    return BRD.MagesBallad;

                if (
                    level >= BRD.Levels.ArmysPaeon
                    && IsOffCooldown(BRD.ArmysPaeon)
                    && (gauge.Song == Song.MAGE || gauge.Song == Song.NONE)
                    && (
                        (gauge.SongTimer <= 12000 && level >= BRD.Levels.WanderersMinuet)
                        || (gauge.SongTimer <= 3000 && level < BRD.Levels.WanderersMinuet)
                    )
                )
                    return BRD.ArmysPaeon;

                if (IsOffCooldown(BRD.RainOfDeath) || HasCharges(BRD.RainOfDeath))
                {
                    return BRD.RainOfDeath;
                }

                if (level >= BRD.Levels.Sidewinder && IsOffCooldown(BRD.Sidewinder))
                {
                    return BRD.Sidewinder;
                }

                if (level >= BRD.Levels.EmpyrealArrow && IsOffCooldown(BRD.EmpyrealArrow))
                {
                    return BRD.EmpyrealArrow;
                }
            }

            if (level >= BRD.Levels.ApexArrow && gauge.SoulVoice >= 80)
                return BRD.ApexArrow;

            if (level >= BRD.Levels.Shadowbite && HasEffect(BRD.Buffs.ShadowbiteReady))
            {
                if (
                    level >= BRD.Levels.Barrage
                    && IsOffCooldown(BRD.Barrage)
                    && HasEffect(BRD.Buffs.RagingStrikes)
                )
                    return BRD.Barrage;

                return BRD.Shadowbite;
            }
        }

        return actionID;
    }
}

internal class BardBloodletter : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BrdAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.Bloodletter)
        {
            var gauge = GetJobGauge<BRDGauge>();

            if (IsEnabled(CustomComboPreset.BardExpiringPerfectBloodletterFeature))
            {
                if (
                    level >= BRD.Levels.PitchPerfect
                    && gauge.Song == Song.WANDERER
                    && gauge.Repertoire >= 1
                )
                {
                    if (gauge.SongTimer <= 2500)
                        return BRD.PitchPerfect;
                }
            }

            if (IsEnabled(CustomComboPreset.BardPerfectBloodletterFeature))
            {
                if (
                    level >= BRD.Levels.PitchPerfect
                    && gauge.Song == Song.WANDERER
                    && gauge.Repertoire == 3
                )
                    return BRD.PitchPerfect;
            }

            if (IsEnabled(CustomComboPreset.BardBloodletterFeature))
            {
                if (level >= BRD.Levels.Sidewinder)
                    return CalcBestAction(
                        actionID,
                        BRD.Bloodletter,
                        BRD.EmpyrealArrow,
                        BRD.Sidewinder
                    );

                if (level >= BRD.Levels.EmpyrealArrow)
                    return CalcBestAction(actionID, BRD.Bloodletter, BRD.EmpyrealArrow);

                if (level >= BRD.Levels.Bloodletter)
                    return BRD.Bloodletter;
            }

            if (IsEnabled(CustomComboPreset.BardBloodRainFeature))
            {
                if (
                    level >= BRD.Levels.RainOfDeath
                    && !TargetHasEffect(BRD.Debuffs.CausticBite)
                    && !TargetHasEffect(BRD.Debuffs.Stormbite)
                    && !TargetHasEffect(BRD.Debuffs.Windbite)
                    && !TargetHasEffect(BRD.Debuffs.VenomousBite)
                )
                {
                    return BRD.RainOfDeath;
                }
            }
        }

        return actionID;
    }
}

internal class BardRainOfDeath : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BrdAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.RainOfDeath)
        {
            var gauge = GetJobGauge<BRDGauge>();

            if (IsEnabled(CustomComboPreset.BardExpiringPerfectRainOfDeathFeature))
            {
                if (
                    level >= BRD.Levels.PitchPerfect
                    && gauge.Song == Song.WANDERER
                    && gauge.Repertoire >= 1
                )
                {
                    if (gauge.SongTimer <= 2500)
                        return BRD.PitchPerfect;
                }
            }

            if (IsEnabled(CustomComboPreset.BardPerfectRainOfDeathFeature))
            {
                if (
                    level >= BRD.Levels.PitchPerfect
                    && gauge.Song == Song.WANDERER
                    && gauge.Repertoire == 3
                )
                    return BRD.PitchPerfect;
            }

            if (IsEnabled(CustomComboPreset.BardRainOfDeathFeature))
            {
                if (level >= BRD.Levels.Sidewinder)
                    return CalcBestAction(
                        actionID,
                        BRD.RainOfDeath,
                        BRD.EmpyrealArrow,
                        BRD.Sidewinder
                    );

                if (level >= BRD.Levels.EmpyrealArrow)
                    return CalcBestAction(actionID, BRD.RainOfDeath, BRD.EmpyrealArrow);

                if (level >= BRD.Levels.RainOfDeath)
                    return BRD.RainOfDeath;
            }
        }

        return actionID;
    }
}

internal class BardSidewinder : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.BardSidewinderFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.Sidewinder)
        {
            if (level >= BRD.Levels.Sidewinder)
                return CalcBestAction(actionID, BRD.EmpyrealArrow, BRD.Sidewinder);
        }

        return actionID;
    }
}

internal class BardEmpyrealArrow : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.BardEmpyrealArrowFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.EmpyrealArrow)
        {
            if (level >= BRD.Levels.Sidewinder)
                return CalcBestAction(actionID, BRD.EmpyrealArrow, BRD.Sidewinder);
        }

        return actionID;
    }
}

internal class BardBarrage : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.BardBarrageFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.Barrage)
        {
            if (
                level >= BRD.Levels.StraightShot
                && HasEffect(BRD.Buffs.HawksEye)
                && !HasEffect(BRD.Buffs.ShadowbiteReady)
            )
                // Refulgent Arrow
                return OriginalHook(BRD.StraightShot);
        }

        return actionID;
    }
}

internal class BardMagesBallad : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.BardCyclingSongFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BRD.MagesBallad)
        {
            const ushort remaining = 40000;
            var gauge = GetJobGauge<BRDGauge>();

            if (level >= BRD.Levels.WanderersMinuet)
            {
                if (gauge.Song == Song.WANDERER && gauge.SongTimer >= remaining)
                    return BRD.WanderersMinuet;

                if (IsAvailable(BRD.WanderersMinuet))
                    return BRD.WanderersMinuet;
            }

            if (level >= BRD.Levels.MagesBallad)
            {
                if (gauge.Song == Song.MAGE && gauge.SongTimer >= remaining)
                    return BRD.MagesBallad;

                if (IsAvailable(BRD.MagesBallad))
                    return BRD.MagesBallad;
            }

            if (level >= BRD.Levels.ArmysPaeon)
            {
                if (gauge.Song == Song.ARMY && gauge.SongTimer >= remaining)
                    return BRD.ArmysPaeon;

                if (IsAvailable(BRD.ArmysPaeon))
                    return BRD.ArmysPaeon;
            }

            // Show the next expected song while on cooldown
            if (level >= BRD.Levels.WanderersMinuet)
                return BRD.WanderersMinuet;

            if (level >= BRD.Levels.MagesBallad)
                return BRD.MagesBallad;

            if (level >= BRD.Levels.ArmysPaeon)
                return BRD.ArmysPaeon;
        }

        return actionID;
    }
}
