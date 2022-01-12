using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class SCH
    {
        public const byte ClassID = 15;
        public const byte JobID = 28;

        public const uint
            EnergyDrain = 167,
            Aetherflow = 166,
            Lustrate = 189,
            Indomitability = 3583,
            FeyBless = 16543,
            SummonSeraph = 16545,
            Consolation = 16546,
            SummonEos = 17215,
            SummonSelene = 17216;

        public static class Buffs
        {
            public const ushort
                Recitation = 1896;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Aetherflow = 45,
                Consolation = 80,
                SummonSeraph = 80;
        }
    }

    internal class ScholarSeraphConsolationFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ScholarSeraphConsolationFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SCH.FeyBless)
            {
                var gauge = GetJobGauge<SCHGauge>();

                if (level >= SCH.Levels.Consolation && gauge.SeraphTimer > 0)
                    return SCH.Consolation;
            }

            return actionID;
        }
    }

    internal class ScholarEnergyDrain : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ScholarEnergyDrainAetherflowFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SCH.EnergyDrain)
            {
                var gauge = GetJobGauge<SCHGauge>();

                if (level >= SCH.Levels.Aetherflow && gauge.Aetherflow == 0)
                    return SCH.Aetherflow;
            }

            return actionID;
        }
    }

    internal class ScholarLustrate : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ScholarLustrateAetherflowFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SCH.Lustrate)
            {
                var gauge = GetJobGauge<SCHGauge>();

                if (level >= SCH.Levels.Aetherflow && gauge.Aetherflow == 0)
                    return SCH.Aetherflow;
            }

            return actionID;
        }
    }

    internal class ScholarIndomitability : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ScholarIndomAetherflowFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SCH.Indomitability)
            {
                var gauge = GetJobGauge<SCHGauge>();

                if (level >= SCH.Levels.Aetherflow && gauge.Aetherflow == 0 && !HasEffect(SCH.Buffs.Recitation))
                    return SCH.Aetherflow;
            }

            return actionID;
        }
    }

    internal class ScholarSummon : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ScholarSeraphFeature;

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
}
