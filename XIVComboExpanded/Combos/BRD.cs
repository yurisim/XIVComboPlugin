using System.Linq;
using System.Net.NetworkInformation;
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
        NaturesMinne = 7408,
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
            RadiantFinale = 2964,
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
            NaturesMinne = 66,
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
        if (actionID is BRD.HeavyShot or BRD.BurstShot)
        {
            var gauge = GetJobGauge<BRDGauge>();

            var ragingStrikesCD = GetCooldown(BRD.RagingStrikes).CooldownRemaining;

            var raidBuffs = HasRaidBuffs();

            if (GCDClipCheck(actionID) && InCombat() && HasTarget())
            {
                if (level >= BRD.Levels.PitchPerfect // Be the right level
                    && gauge.Song == Song.WANDERER // be the right song
                    && (gauge.Repertoire == 3 || (gauge.Repertoire >= 1 && gauge.SongTimer <= 3000))
                   )
                    return BRD.PitchPerfect;

                switch (level)
                {
                    case >= BRD.Levels.WanderersMinuet when
                        IsOffCooldown(BRD.WanderersMinuet)
                        && (gauge.Song == Song.ARMY || gauge.Song == Song.NONE)
                        && gauge.SongTimer <= 3000:
                        return BRD.WanderersMinuet;
                    case >= BRD.Levels.MagesBallad when
                        IsOffCooldown(BRD.MagesBallad)
                        && (gauge.Song == Song.WANDERER || gauge.Song == Song.NONE)
                        && gauge.SongTimer <= 3000:
                        return BRD.MagesBallad;
                    case >= BRD.Levels.ArmysPaeon when
                        IsOffCooldown(BRD.ArmysPaeon)
                        && (gauge.Song == Song.MAGE || gauge.Song == Song.NONE)
                        && ((gauge.SongTimer <= 12000 && level >= BRD.Levels.WanderersMinuet)
                            || (gauge.SongTimer <= 3000 && level < BRD.Levels.WanderersMinuet)):
                        return BRD.ArmysPaeon;
                    case >= BRD.Levels.BattleVoice when
                        HasEffect(BRD.Buffs.RagingStrikes)
                        && IsOffCooldown(BRD.BattleVoice):
                        return BRD.BattleVoice;
                    case >= BRD.Levels.RadiantFinale when
                        IsOffCooldown(BRD.RadiantFinale)
                        && gauge.Coda.Length >= 1
                        && HasEffect(BRD.Buffs.RagingStrikes):
                        return BRD.RadiantFinale;
                    case >= BRD.Levels.RagingStrikes when
                        IsOffCooldown(BRD.RagingStrikes)
                        && HasRaidBuffs():
                        return BRD.RagingStrikes;
                    case >= BRD.Levels.EmpyrealArrow when
                        IsOffCooldown(BRD.EmpyrealArrow):
                        return BRD.EmpyrealArrow;
                    case >= BRD.Levels.Bloodletter when
                        HasCharges(OriginalHook(BRD.Bloodletter))
                        && (HasEffect(BRD.Buffs.RagingStrikes)
                            || HasRaidBuffs()
                            || GetCooldown(OriginalHook(BRD.Bloodletter)).TotalCooldownRemaining <= 10):
                        return OriginalHook(BRD.Bloodletter);

                    case >= BRD.Levels.Barrage when
                        IsOffCooldown(BRD.Barrage)
                        && (HasEffect(BRD.Buffs.RagingStrikes)
                            || ragingStrikesCD >= 18
                            || HasRaidBuffs()):
                        return BRD.Barrage;

                    case >= BRD.Levels.Sidewinder when
                        IsOffCooldown(BRD.Sidewinder)
                        && ragingStrikesCD >= 9:
                        return BRD.Sidewinder;

                    case >= BRD.Levels.NaturesMinne when
                        IsOffCooldown(BRD.NaturesMinne)
                        && LocalPlayerPercentage() <= 0.50:
                        return BRD.NaturesMinne;
                }
            }


            if (HasEffect(BRD.Buffs.Barrage)) return OriginalHook(BRD.StraightShot);

            var refreshTime = 5;

            var causticDots = new[] {
                FindTargetEffect(BRD.Debuffs.CausticBite),
                FindTargetEffect(BRD.Debuffs.VenomousBite)
                };

            var stormDots = new[] {
                FindTargetEffect(BRD.Debuffs.Stormbite),
                FindTargetEffect(BRD.Debuffs.Windbite)
                };

            var combinedDots = causticDots.Concat(stormDots);

            if (level >= BRD.Levels.IronJaws && combinedDots.Any(x => x?.RemainingTime <= (raidBuffs ? 26 : refreshTime)))
                return BRD.IronJaws;

            if (ShouldUseDots())
            {
                if (causticDots.All(x => x is null || x.RemainingTime <= refreshTime))
                    return OriginalHook(BRD.VenomousBite);

                if (stormDots.All(x => x is null || x.RemainingTime <= refreshTime))
                    return OriginalHook(BRD.Windbite);
            }

            var blastArrow = FindEffect(BRD.Buffs.BlastShotReady);

            if (level >= BRD.Levels.BlastShot
                && blastArrow is not null
                && (blastArrow.RemainingTime <= 6 || raidBuffs))
                return BRD.BlastArrow;

            var radiantEncore = FindEffect(BRD.Buffs.RadiantEncoreReady);

            if (level >= BRD.Levels.RadiantEncore
                && radiantEncore is not null
                && (radiantEncore.RemainingTime <= 20 || raidBuffs)
                )
            {
                return BRD.RadiantEncore;
            }

            var resonantArrowReady = FindEffect(BRD.Buffs.ResonantArrowReady);

            if (level >= BRD.Levels.ResonantArrow
                && resonantArrowReady is not null
                && (resonantArrowReady.RemainingTime <= 10 || raidBuffs)
                )
            {
                return BRD.ResonantArrow;
            }

            if (level >= BRD.Levels.ApexArrow
                && gauge.SoulVoice >= 80
                && (HasEffect(BRD.Buffs.RagingStrikes)
                    || (gauge.SoulVoice == 100
                        && gauge.Song != Song.ARMY
                        && IsOnCooldown(BRD.RagingStrikes))
                    || raidBuffs))
                return BRD.ApexArrow;

            if (level >= BRD.Levels.StraightShot
                && CanUseAction(OriginalHook(BRD.StraightShot)))
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
            var ragingStrikesCD = GetCooldown(BRD.RagingStrikes).CooldownRemaining;

            if (GCDClipCheck(actionID))
            {
                if (level >= BRD.Levels.PitchPerfect // Be the right level
                    && gauge.Song == Song.WANDERER // be the right song
                    && (gauge.Repertoire == 3 || (gauge.Repertoire >= 1 && gauge.SongTimer <= 3000))
                   )
                    return BRD.PitchPerfect;

                switch (level)
                {
                    case >= BRD.Levels.WanderersMinuet when
                        IsOffCooldown(BRD.WanderersMinuet)
                        && (gauge.Song == Song.ARMY || gauge.Song == Song.NONE)
                        && gauge.SongTimer <= 3000:
                        return BRD.WanderersMinuet;
                    case >= BRD.Levels.MagesBallad when
                        IsOffCooldown(BRD.MagesBallad)
                        && (gauge.Song == Song.WANDERER || gauge.Song == Song.NONE)
                        && gauge.SongTimer <= 3000:
                        return BRD.MagesBallad;
                    case >= BRD.Levels.ArmysPaeon when
                        IsOffCooldown(BRD.ArmysPaeon)
                        && (gauge.Song == Song.MAGE || gauge.Song == Song.NONE)
                        && ((gauge.SongTimer <= 12000 && level >= BRD.Levels.WanderersMinuet)
                            || (gauge.SongTimer <= 3000 && level < BRD.Levels.WanderersMinuet)):
                        return BRD.ArmysPaeon;
                    case >= BRD.Levels.RagingStrikes when
                        IsOffCooldown(BRD.RagingStrikes)
                        && HasRaidBuffs():
                        return BRD.RagingStrikes;
                    case >= BRD.Levels.BattleVoice when
                        HasEffect(BRD.Buffs.RagingStrikes)
                        && IsOffCooldown(BRD.BattleVoice):
                        return BRD.BattleVoice;
                    case >= BRD.Levels.RadiantFinale when
                        IsOffCooldown(BRD.RadiantFinale)
                        && gauge.Coda.Length >= 1
                        && (HasRaidBuffs() || HasEffect(BRD.Buffs.RagingStrikes)):
                        return BRD.RadiantFinale;
                    case >= BRD.Levels.EmpyrealArrow when
                        IsOffCooldown(BRD.EmpyrealArrow):
                        return BRD.EmpyrealArrow;
                    case >= BRD.Levels.Barrage when
                        IsOffCooldown(BRD.Barrage)
                        && (HasEffect(BRD.Buffs.RagingStrikes)
                            || ragingStrikesCD >= 18
                            || HasRaidBuffs()):
                        return BRD.Barrage;
                    case >= BRD.Levels.RainOfDeath when
                        IsOffCooldown(BRD.RainOfDeath)
                            || HasCharges(BRD.RainOfDeath):
                        return OriginalHook(BRD.RainOfDeath);
                    case >= BRD.Levels.Sidewinder when IsOffCooldown(BRD.Sidewinder):
                        return BRD.Sidewinder;
                    case >= BRD.Levels.NaturesMinne when
                        IsOffCooldown(BRD.NaturesMinne)
                        && LocalPlayerPercentage() <= 0.50:
                        return BRD.NaturesMinne;
                }

            }

            if (level >= BRD.Levels.ApexArrow
                && gauge.SoulVoice >= 80
                    )
                return BRD.ApexArrow;

            var radiantEncore = FindEffect(BRD.Buffs.RadiantEncoreReady);

            if (level >= BRD.Levels.RadiantEncore
                && radiantEncore is not null
                && (radiantEncore.RemainingTime <= 10)
                )
            {
                return BRD.RadiantEncore;
            }

            var resonantArrowReady = FindEffect(BRD.Buffs.ResonantArrowReady);

            if (level >= BRD.Levels.ResonantArrow
                && resonantArrowReady is not null
                && (resonantArrowReady.RemainingTime <= 10 || HasRaidBuffs())
                )
            {
                return BRD.ResonantArrow;
            }

            if (level >= BRD.Levels.BlastShot && HasEffect(BRD.Buffs.BlastShotReady))
                return BRD.BlastArrow;

            if (level >= BRD.Levels.WideVolley && HasEffect(BRD.Buffs.HawksEye))
            {
                return OriginalHook(BRD.WideVolley);
            }
        }

        return actionID;
    }
}
