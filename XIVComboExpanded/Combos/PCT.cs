﻿using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;

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
            SubstractivePalette = 3674,
            StarryMuse = 3685,
            Chroma2Ready = 3675,
            Chroma3Ready = 3676,
            RainbowReady = 3679,
            HammerReady = 3680,
            StarPrismReady = 3681,
            Installation = 3688,
            ArtisticInstallation = 3689,
            SubstractivePaletteReady = 3690,
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
                            when HasRaidBuffs() && gauge.LandscapeMotifDrawn && IsCooldownUsable(StarryMuse):
                            return StarryMuse;
                        case >= Levels.WeaponMotif
                            when gauge.WeaponMotifDrawn
                                 && IsCooldownUsable(StrikingMuse)
                                 && StarryMuseCD(60):
                            return StrikingMuse;
                        case >= Levels.MogOftheAges
                            when CanUseAction(MogOftheAges)
                                 && StarryMuseCD(80):
                            return MogOftheAges;
                        case >= Levels.CreatureMotif
                            when IsCooldownUsable(OriginalHook(LivingMuse))
                                 && gauge.CreatureMotifDrawn
                                 && StarryMuseCD(40):
                            return OriginalHook(LivingMuse);
                        case >= Levels.SubtractivePalette
                            when gauge.PalleteGauge >= 50
                                 && CanUseAction(SubtractivePalette)
                                 && (
                                     StarryMuseCD(1000)
                                     ||
                                     gauge.PalleteGauge >= 100
                                 ):
                            return SubtractivePalette;
                    }
                }

                if (
                    level >= Levels.HammerStamp
                    && CanUseAction(HammerStamp)
                    && (StarryMuseCD(60) || FindEffect(Buffs.HammerReady)?.RemainingTime <= 10)
                )
                {
                    return HammerStamp;
                }

                if (HasEffect(Buffs.SubstractivePalette) && actionID is not HolyWhite)
                {
                    return OriginalHook(BlizzardCyan);
                }

                return actionID;

                bool StarryMuseCD(float coolDownOfSkill)
                {
                    return HasEffect(Buffs.StarryMuse) || level < Levels.StarryMuse || HasRaidBuffs() ||
                           GetCooldown(StarryMuse).CooldownRemaining > coolDownOfSkill * 0.05;
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
                    !HasEffect(Buffs.SubstractivePalette))
                {
                    if (IsEnabled(CustomComboPreset.PictomancerSubtractiveEarlyAutoCombo)
                        && (gauge.PalleteGauge >= 50 || HasEffect(Buffs.SubstractivePaletteReady)))
                        return SubtractivePalette;

                    if (HasEffect(Buffs.SubstractivePaletteReady) ||
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
                    if (!HasEffect(Buffs.SubstractivePalette))
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
                                 && IsCooldownUsable(StrikingMuse):
                            return StrikingMuse;
                        case >= Levels.CreatureMotif
                            when IsCooldownUsable(OriginalHook(LivingMuse))
                                 && gauge.CreatureMotifDrawn:
                            return OriginalHook(LivingMuse);
                        case >= Levels.SubtractivePalette
                            when gauge.PalleteGauge >= 50
                                 && CanUseAction(SubtractivePalette):
                            return SubtractivePalette;
                    }
                }

                // Hammer Stamp
                if (
                    level >= Levels.HammerStamp
                    && CanUseAction(HammerStamp))
                {
                    return HammerStamp;
                }

                if (HasEffect(Buffs.SubstractivePalette))
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
                    !HasEffect(Buffs.SubstractivePalette))
                {
                    if (IsEnabled(CustomComboPreset.PictomancerSubtractiveEarlyAutoCombo)
                        && (gauge.PalleteGauge >= 50 || HasEffect(Buffs.SubstractivePaletteReady)))
                        return SubtractivePalette;

                    if (HasEffect(Buffs.Chroma3Ready) && gauge.PalleteGauge == 100)
                        return SubtractivePalette;
                }

                if (IsEnabled(CustomComboPreset.PictomancerSubtractiveAoECombo))
                {
                    if (!HasEffect(Buffs.SubstractivePalette))
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

    internal class PictomancerMotifs : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.PctAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            // if (actionID is CreatureMotif)
            // {
            //     var gauge = GetJobGauge<PCTGauge>();

            //     if (level >= Levels.LandscapeMotif && !gauge.LandscapeMotifDrawn && CanUseAction(StarrySkyMotif))
            //     {
            //         return OriginalHook(LandscapeMotif);
            //     }

            //     if (level >= Levels.WeaponMotif && !gauge.WeaponMotifDrawn && !IsOriginal(WeaponMotif))
            //     {
            //         return OriginalHook(WeaponMotif);
            //     }
            // }

            // return actionID;

            if (actionID is CreatureMotif)
            {
                var gauge = GetJobGauge<PCTGauge>();

                var skills = new (uint Level, CooldownData Cooldown, bool Motif, uint Skill)[]
                {
                    (Levels.CreatureMotif, GetCooldown(CreatureMotif), !gauge.CreatureMotifDrawn, CreatureMotif),
                    (Levels.WeaponMotif, GetCooldown(WeaponMotif), !gauge.WeaponMotifDrawn, WeaponMotif),
                    (Levels.LandscapeMotif, GetCooldown(LandscapeMotif), !gauge.LandscapeMotifDrawn, LandscapeMotif)
                };

                var availableSkills = skills
                    .Where(s => s.Level <= level)
                    .Where(s => s.Motif)
                    .OrderBy(s => s.Cooldown.CooldownRemaining)
                    .ThenBy(s => s.Cooldown.RemainingCharges > 0 ? 1 : 0)
                    .Select(s => s.Skill)
                    .ToArray();

                if (availableSkills.Length > 0)
                {
                    return OriginalHook(availableSkills[0]);
                }
            }

            return actionID;
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
                if (HasEffect(Buffs.Chroma3Ready) && !HasEffect(Buffs.SubstractivePalette) &&
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
                    if (HasEffect(Buffs.HammerReady))
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