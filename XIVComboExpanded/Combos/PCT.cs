using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class PCT
{
    public const byte JobID = 42;

    public const uint
        FireRed = 34650,
        AeroGreen = 34651,
        WaterBlue = 34652,
        BlizzardCyan = 34653,
        EarthYellow = 34654,
        ThunderMagenta = 34655,
        ExtraFireRed = 34656,
        ExtraAeroGreen = 34657,
        ExtraWaterBlue = 34658,
        ExtraBlizzardCyan = 34659,
        ExtraEarthYellow = 34660,
        ExtraThunderMagenta = 34661,
        HolyWhite = 34662,
        LivingMuse = 35347,
        CometBlack = 34663,
        PomMotif = 34664,
        WingMotif = 34665,
        ClawMotif = 34666,
        MawMotif = 34667,
        HammerMotif = 34668,
        StarrySkyMotif = 34669,
        PomMuse = 34670,
        WingedMuse = 34671,
        ClawedMuse = 34672,
        FangedMuse = 34673,
        StrikingMuse = 34674,
        StarryMuse = 34675,
        MogOftheAges = 34676,
        Retribution = 34677,
        HammerStamp = 34678,
        HammerBrush = 34679,
        PolishingHammer = 34680,
        StarPrism1 = 34681,
        StarPrism2 = 34682,
        SteelMuse = 35348,
        SubtractivePalette = 34683,
        Smudge = 34684,
        TemperaCoat = 34685,
        TemperaGrassa = 34686,
        ScenicMuse = 35349,
        RainbowDrip = 34688,
        CreatureMotif = 34689,
        WeaponMotif = 34690,
        LandscapeMotif = 34691,
        CreatureMotifDrawn = 35347,
        WeaponMotifDrawn = 35348,
        LandscapeMotifDrawn = 35349;

    public static class Buffs
    {
        public const ushort
            SubtractivePalette = 3674,
            StarryMuse = 3685,
            Chroma2Ready = 3675,
            Chroma3Ready = 3676,
            RainbowReady = 3679,
            HammerTime = 3680,
            StarPrismReady = 3681,
            Installation = 3688,
            ArtisticInstallation = 3689,
            SubtractiveSpectrum = 3690,
            InvertedColors = 3691;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    private static class Levels
    {
        public const byte
            Smudge = 20,
            ExtraFireRed = 25,
            CreatureMotif = 30,
            PomMotif = 30,
            WingMotif = 30,
            PomMuse = 30,
            WingedMuse = 30,
            MogOftheAges = 30,
            ExtraAeroGreen = 35,
            ExtraWaterBlue = 45,
            HammerMotif = 50,
            HammerStamp = 50,
            WeaponMotif = 50,
            StrikingMuse = 50,
            SubtractivePalette = 60,
            BlizzardCyan = 60,
            EarthYellow = 60,
            ThunderMagenta = 60,
            ExtraBlizzardCyan = 60,
            ExtraEarthYellow = 60,
            ExtraThunderMagenta = 60,
            StarrySkyMotif = 70,
            LandscapeMotif = 70,
            MiracleWhite = 80,
            HammerBrush = 86,
            PolishingHammer = 86,
            TemperaGrassa = 88,
            CometBlack = 90,
            RainbowDrip = 92,
            ClawMotif = 96,
            MawMotif = 96,
            ClawedMuse = 96,
            FangedMuse = 96,
            StarryMuse = 70,
            Retribution = 96,
            StarPrism1 = 100,
            StarPrism2 = 100;
    }

    internal class PictomancerSTCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PctAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<PCTGauge>();

            if (actionID is FireRed or HolyWhite)
            {
                if (GCDClipCheck(actionID))
                {
                    switch (level)
                    {
                        case >= Levels.StarryMuse
                            when HasRaidBuffs() && gauge.LandscapeMotifDrawn && IsOffCooldown(StarryMuse):
                            return StarryMuse;
                        case >= Levels.SubtractivePalette
                            when (gauge.PalleteGauge >= 50 || HasEffect(Buffs.SubtractiveSpectrum))
                                 && !HasEffect(Buffs.SubtractivePalette)
                                 && (StarryMuseCD() || gauge.PalleteGauge >= 100):
                            return SubtractivePalette;
                        case >= Levels.WeaponMotif
                            when IsCooldownUsable(OriginalHook(SteelMuse))
                                 && gauge.WeaponMotifDrawn
                                 && InCombat()
                                 && (level >= Levels.StarryMuse && IsOnCooldown(StarryMuse) ||
                                     level < Levels.StarryMuse)
                                 && ((GetCooldown(OriginalHook(SteelMuse)).TotalCooldownRemaining <= 5) || HasRaidBuffs() ||
                                     HasEffect(Buffs.StarryMuse) || level < Levels.HammerBrush):
                            return OriginalHook(SteelMuse);
                        case >= Levels.MogOftheAges
                            when gauge.MooglePortraitReady
                                 && IsOffCooldown(MogOftheAges):
                            return MogOftheAges;
                        case >= Levels.CreatureMotif
                            when IsCooldownUsable(OriginalHook(LivingMuse))
                                 && gauge.CreatureMotifDrawn
                                 && InCombat()
                                 && (level >= Levels.StarryMuse && IsOnCooldown(StarryMuse) ||
                                     level < Levels.StarryMuse)
                                 && (HasRaidBuffs() || (GetCooldown(OriginalHook(LivingMuse)).TotalCooldownRemaining <= 5)
                                 || HasEffect(Buffs.StarryMuse)):
                            return OriginalHook(LivingMuse);
                        case >= 15
                            when InCombat()
                                 && IsOffCooldown(ADV.LucidDreaming)
                                 && LocalPlayer?.CurrentMp <= 8000:
                            return ADV.LucidDreaming;
                    }
                }

                if (
                    level >= Levels.HammerStamp
                    && CanUseAction(OriginalHook(HammerStamp))
                )
                {
                    return OriginalHook(HammerStamp);
                }

                if (HasEffect(Buffs.SubtractivePalette) && actionID is not HolyWhite)
                {
                    return OriginalHook(BlizzardCyan);
                }

                return actionID;

                bool StarryMuseCD()
                {
                    return HasEffect(Buffs.StarryMuse) || level < Levels.StarryMuse || HasRaidBuffs();
                }
            }

            if (actionID is FireRed or BlizzardCyan &&
                IsEnabled(CustomComboPreset.PictomancerRainbowStarter) && !InCombat() &&
                level >= Levels.RainbowDrip)
                return RainbowDrip;

            if (actionID == FireRed || actionID == BlizzardCyan)
            {
                if (IsEnabled(CustomComboPreset.PictomancerStarPrismAutoCombo))
                {
                    if (HasEffect(Buffs.StarPrismReady))
                    {
                        return StarPrism1;
                    }
                }

                if (IsEnabled(CustomComboPreset.PictomancerRainbowAutoCombo))
                {
                    if (HasEffect(Buffs.RainbowReady))
                    {
                        return RainbowDrip;
                    }
                }

                if (IsEnabled(CustomComboPreset.PictomancerAutoMogCombo))
                {
                    if ((gauge.MooglePortraitReady || gauge.MadeenPortraitReady) &&
                        GetRemainingCharges(MogOftheAges) > 0)
                    {
                        return OriginalHook(MogOftheAges);
                    }
                }

                if (IsEnabled(CustomComboPreset.PictomancerSubtractiveAutoCombo) &&
                    !HasEffect(Buffs.SubtractivePalette))
                {
                    if (IsEnabled(CustomComboPreset.PictomancerSubtractiveEarlyAutoCombo)
                        && (gauge.PalleteGauge >= 50 || HasEffect(Buffs.SubtractiveSpectrum)))
                        return SubtractivePalette;

                    if (HasEffect(Buffs.SubtractiveSpectrum) ||
                        (HasEffect(Buffs.Chroma3Ready) && gauge.PalleteGauge == 100))
                        return SubtractivePalette;
                }

                if (IsEnabled(CustomComboPreset.PictomancerHolyAutoCombo))
                {
                    if (gauge.Paint == 5)
                    {
                        if (HasEffect(Buffs.InvertedColors))
                            return CometBlack;
                        return HolyWhite;
                    }
                }

                if (IsEnabled(CustomComboPreset.PictomancerSubtractiveSTCombo))
                {
                    if (!HasEffect(Buffs.SubtractivePalette))
                    {
                        if (HasEffect(Buffs.Chroma2Ready))
                        {
                            return AeroGreen;
                        }

                        if (HasEffect(Buffs.Chroma3Ready))
                        {
                            return WaterBlue;
                        }

                        return FireRed;
                    }
                }
            }

            return actionID;
        }
    }

    internal class PictomancerAoECombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PctAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<PCTGauge>();

            if (actionID == ExtraFireRed)
            {
                if (GCDClipCheck(actionID))
                {
                    switch (level)
                    {
                        case >= Levels.MogOftheAges
                            when CanUseAction(MogOftheAges):
                            return MogOftheAges;
                        case >= Levels.WeaponMotif
                            when gauge.WeaponMotifDrawn
                                 && InCombat()
                                 && IsCooldownUsable(OriginalHook(SteelMuse)):
                            return OriginalHook(SteelMuse);
                        case >= Levels.CreatureMotif
                            when IsCooldownUsable(OriginalHook(LivingMuse))
                                 && gauge.CreatureMotifDrawn:
                            return OriginalHook(LivingMuse);
                        case >= Levels.SubtractivePalette
                            when (gauge.PalleteGauge >= 50 || HasEffect(Buffs.SubtractiveSpectrum))
                                 && !HasEffect(Buffs.SubtractivePalette):
                            return SubtractivePalette;

                    }
                }

                // Hammer Stamp
                if (
                    level >= Levels.HammerStamp
                    && CanUseAction(OriginalHook(HammerStamp)))
                {
                    return OriginalHook(HammerStamp);
                }

                if (HasEffect(Buffs.SubtractivePalette))
                {
                    return OriginalHook(ExtraBlizzardCyan);
                }

                return actionID;
            }


            if ((actionID == ExtraFireRed || actionID == ExtraBlizzardCyan) &&
                IsEnabled(CustomComboPreset.PictomancerRainbowStarter) && !InCombat())
            {
                return RainbowDrip;
            }

            if (actionID == ExtraBlizzardCyan)
            {
                if (IsEnabled(CustomComboPreset.PictomancerSubtractiveAutoCombo) &&
                    !HasEffect(Buffs.SubtractivePalette))
                {
                    if (IsEnabled(CustomComboPreset.PictomancerSubtractiveEarlyAutoCombo)
                        && (gauge.PalleteGauge >= 50 || HasEffect(Buffs.SubtractiveSpectrum)))
                        return SubtractivePalette;

                    if (HasEffect(Buffs.Chroma3Ready) && gauge.PalleteGauge == 100)
                        return SubtractivePalette;
                }

                if (IsEnabled(CustomComboPreset.PictomancerSubtractiveAoECombo))
                {
                    if (!HasEffect(Buffs.SubtractivePalette))
                    {
                        if (actionID == ExtraBlizzardCyan)
                        {
                            if (HasEffect(Buffs.Chroma2Ready) && level >= Levels.ExtraAeroGreen)
                            {
                                return ExtraAeroGreen;
                            }

                            if (HasEffect(Buffs.Chroma3Ready) && level >= Levels.ExtraWaterBlue)
                            {
                                return ExtraWaterBlue;
                            }

                            return OriginalHook(ExtraFireRed);
                        }
                    }
                }
            }

            return actionID;
        }
    }

    // internal class PictomancerMotifs : CustomCombo
    // {
    //     protected internal override CustomComboPreset Preset { get; } =
    //         CustomComboPreset.PctAny;
    //
    //     protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    //     {
    //         if (actionID is CreatureMotif)
    //         {
    //             var gauge = GetJobGauge<PCTGauge>();
    //
    //             var skills = new (uint Level, CooldownData CD, bool MotifNeeded, uint Skill)[]
    //             {
    //                 (Levels.CreatureMotif, GetCooldown(OriginalHook(LivingMuse)), !gauge.CreatureMotifDrawn, CreatureMotif),
    //                 (Levels.WeaponMotif, GetCooldown(SteelMuse), !(gauge.WeaponMotifDrawn || HasEffect(Buffs.HammerTime)), WeaponMotif),
    //                 (Levels.LandscapeMotif, GetCooldown(ScenicMuse), !(gauge.LandscapeMotifDrawn || HasEffect(Buffs.StarryMuse)), LandscapeMotif)
    //             };
    //
    //             var availableSkills = skills
    //                 .Where(s => s.Level <= level)
    //                 .Where(s => s.MotifNeeded)
    //                 .OrderBy(s => s.CD.CooldownRemaining)
    //                 .ThenByDescending(s => s.CD.Available)
    //                 .Select(s => s.Skill)
    //                 .ToArray();
    //
    //             if (availableSkills.Length > 0)
    //             {
    //                 return OriginalHook(availableSkills.First());
    //             }
    //         }
    //
    //         return actionID;
    //     }
    // }

    internal class PictMotifFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.PctAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return actionID switch
            {
                CreatureMotif when OriginalHook(LivingMuse) != LivingMuse && InCombat() => OriginalHook(LivingMuse),
                WeaponMotif when OriginalHook(SteelMuse) != SteelMuse => OriginalHook(SteelMuse),
                LandscapeMotif when OriginalHook(ScenicMuse) != ScenicMuse => OriginalHook(ScenicMuse),
                _ => actionID
            };
        }
    }

    internal class PictomancerSubtractiveAutoCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.PictomancerSubtractiveAutoCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<PCTGauge>();
            if (actionID == WaterBlue || actionID == ExtraWaterBlue)
            {
                if (HasEffect(Buffs.Chroma3Ready) && !HasEffect(Buffs.SubtractivePalette) &&
                    gauge.PalleteGauge == 100)
                {
                    return SubtractivePalette;
                }
            }

            return actionID;
        }
    }

    internal class PictomancerHolyCometCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PictomancerHolyCometCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == HolyWhite)
            {
                if (IsEnabled(CustomComboPreset.PictomancerRainbowHolyCombo) && HasEffect(Buffs.RainbowReady))
                {
                    return RainbowDrip;
                }

                if (HasEffect(Buffs.InvertedColors))
                    return CometBlack;
            }

            return actionID;
        }
    }

    internal class PictomancerCreatureMotifCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PctAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<PCTGauge>();

            if (actionID == CreatureMotif)
            {
                if (IsEnabled(CustomComboPreset.PictomancerCreatureMogCombo))
                {
                    if (gauge.MooglePortraitReady || gauge.MadeenPortraitReady)
                    {
                        if (IsCooldownUsable(MogOftheAges))
                            return OriginalHook(MogOftheAges);
                    }
                }

                if (IsEnabled(CustomComboPreset.PictomancerCreatureMotifCombo))
                {
                    if (actionID == CreatureMotif)
                    {
                        if (OriginalHook(CreatureMotifDrawn) != CreatureMotifDrawn)
                            return OriginalHook(CreatureMotifDrawn);
                    }
                }
            }

            return actionID;
        }
    }

    internal class PictomancerWeaponMotifCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PctAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<PCTGauge>();

            if (actionID == WeaponMotif)
            {
                if (IsEnabled(CustomComboPreset.PictomancerWeaponMotifCombo))
                {
                    if (gauge.WeaponMotifDrawn)
                        return StrikingMuse;
                }

                if (IsEnabled(CustomComboPreset.PictomancerWeaponHammerCombo))
                {
                    if (HasEffect(Buffs.HammerTime))
                    {
                        return OriginalHook(HammerStamp);
                    }
                }
            }

            return actionID;
        }
    }

    internal class PictomancerLandscapeMotifCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PctAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<PCTGauge>();

            if (actionID == LandscapeMotif)
            {
                if (IsEnabled(CustomComboPreset.PictomancerLandscapeMotifCombo))
                {
                    if (gauge.LandscapeMotifDrawn)
                        return StarryMuse;
                }

                if (IsEnabled(CustomComboPreset.PictomancerLandscapePrismCombo))
                {
                    if (HasEffect(Buffs.StarPrismReady))
                    {
                        return OriginalHook(StarPrism1);
                    }
                }
            }

            return actionID;
        }
    }
}