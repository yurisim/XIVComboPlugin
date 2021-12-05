namespace XIVComboExpandedPlugin.Combos
{
    internal static class RPR
    {
        public const byte JobID = 39;

        public const uint
            // Single Target
            Slice = 24373,
            WaxingSlice = 24374,
            InfernalSlice = 24375,
            // AoE
            SpinningScythe = 24376,
            NightmareScythe = 24377,
            // Soul Reaver
            BloodStalk = 24389,
            Gibbet = 24382,
            Gallows = 24383,
            Guillotine = 24384,
            // Sacrifice
            ArcaneCircle = 24405,
            PlentifulHarvest = 24385,
            // Shroud
            Enshroud = 24394,
            Communio = 24398;

        public static class Buffs
        {
            public const ushort
                SoulReaver = 2587,
                ImmortalSacrifice = 2592,
                EnhancedGibbet = 2588,
                EnhancedGallows = 2589,
                EnhancedVoidReaping = 2590,
                EnhancedCrossReaping = 2591,
                Enshrouded = 2593;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                WaxingSlice = 5,
                SpinningScythe = 25,
                InfernalSlice = 30,
                NightmareScythe = 45,
                Enshroud = 80,
                PlentifulHarvest = 88,
                Communio = 90;
        }
    }

    internal class ReaperSliceCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.ReaperSliceCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == RPR.InfernalSlice)
            {
                if (comboTime > 0)
                {
                    if (lastComboMove == RPR.WaxingSlice && level >= RPR.Levels.InfernalSlice)
                        return RPR.InfernalSlice;

                    if (lastComboMove == RPR.Slice && level >= RPR.Levels.WaxingSlice)
                        return RPR.WaxingSlice;
                }

                return RPR.Slice;
            }

            return actionID;
        }
    }

    internal class ReaperScytheCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.ReaperScytheCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == RPR.NightmareScythe)
            {
                if (comboTime > 0)
                {
                    if (lastComboMove == RPR.SpinningScythe && level >= RPR.Levels.NightmareScythe)
                        return RPR.NightmareScythe;
                }

                return RPR.SpinningScythe;
            }

            return actionID;
        }
    }

    internal class ReaperHarvestFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.ReaperHarvestFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == RPR.ArcaneCircle)
            {
                if (level >= RPR.Levels.PlentifulHarvest && HasEffect(RPR.Buffs.ImmortalSacrifice))
                    return RPR.PlentifulHarvest;
            }

            return actionID;
        }
    }

    internal class EnshroudCommunioFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.ReaperEnshroudCommunioFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == RPR.Enshroud)
            {
                if (level >= RPR.Levels.Communio && HasEffect(RPR.Buffs.Enshrouded))
                    return RPR.Communio;
            }

            return actionID;
        }
    }
}