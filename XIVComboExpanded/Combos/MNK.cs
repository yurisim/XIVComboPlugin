using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class MNK
    {
        public const byte ClassID = 2;
        public const byte JobID = 20;

        public const uint
            Bootshine = 53,
            DragonKick = 74,
            SnapPunch = 56,
            TwinSnakes = 61,
            Demolish = 66,
            ArmOfTheDestroyer = 62,
            Rockbreaker = 70,
            Meditation = 3546,
            FourPointFury = 16473,
            HowlingFist = 25763,
            Enlightenment = 16474;

        public static class Buffs
        {
            public const ushort
                TwinSnakes = 101,
                OpoOpoForm = 107,
                RaptorForm = 108,
                CoerlForm = 109,
                PerfectBalance = 110,
                LeadenFist = 1861,
                FormlessFist = 2513;
        }

        public static class Debuffs
        {
            public const ushort
                Demolish = 246;
        }

        public static class Levels
        {
            public const byte
                Meditation = 15,
                ArmOfTheDestroyer = 26,
                Rockbreaker = 30,
                Demolish = 30,
                FourPointFury = 45,
                HowlingFist = 40,
                DragonKick = 50,
                PerfectBalance = 50,
                FormShift = 52,
                Enlightenment = 70,
                ShadowOfTheDestroyer = 82;
        }
    }

    internal class MonkAoECombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkAoECombo;

        protected internal override uint[] ActionIDs { get; } = new[] { MNK.Rockbreaker };

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MNK.Rockbreaker)
            {
                if (level >= MNK.Levels.PerfectBalance && HasEffect(MNK.Buffs.PerfectBalance))
                    return MNK.Rockbreaker;

                if (level >= MNK.Levels.FormShift && HasEffect(MNK.Buffs.FormlessFist))
                    return MNK.Rockbreaker;

                if (level >= MNK.Levels.ArmOfTheDestroyer && HasEffect(MNK.Buffs.OpoOpoForm))
                    // Shadow of the Destroyer
                    return OriginalHook(MNK.ArmOfTheDestroyer);

                if (level >= MNK.Levels.FourPointFury && HasEffect(MNK.Buffs.RaptorForm))
                    return MNK.FourPointFury;

                if (level >= MNK.Levels.Rockbreaker && HasEffect(MNK.Buffs.CoerlForm))
                    return MNK.Rockbreaker;

                return MNK.ArmOfTheDestroyer;
            }

            return actionID;
        }
    }

    internal class MonkHowlingFistMeditationFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MonkHowlingFistMeditationFeature;

        protected internal override uint[] ActionIDs { get; } = new[] { MNK.HowlingFist };

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == MNK.HowlingFist)
            {
                var gauge = GetJobGauge<MNKGauge>();

                if (level >= MNK.Levels.Meditation && gauge.Chakra < 5)
                    return MNK.Meditation;

                // Enlightenment
                return OriginalHook(MNK.HowlingFist);
            }

            return actionID;
        }
    }
}
