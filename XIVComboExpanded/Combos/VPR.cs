﻿using System.Collections;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class VPR
{
    public const byte JobID = 41;

    public const uint
            SteelFangs = 34606,
            ReavingFangs = 34607,
            HuntersSting = 34608,
            SwiftskinsSting = 34609,
            FlankstingStrike = 34610,
            FlanksbaneFang = 34611,
            HindstingStrike = 34612,
            HindsbaneFang = 34613,

            SteelMaw = 34614,
            ReavingMaw = 34615,
            HuntersBite = 34616,
            SwiftskinsBite = 34617,
            JaggedMaw = 34618,
            BloodiedMaw = 34619,

            Vicewinder = 34620,
            HuntersCoil = 34621,
            SwiftskinsCoil = 34622,
            VicePit = 34623,
            HuntersDen = 34624,
            SwiftskinsDen = 34625,

            SerpentsTail = 35920,
            DeathRattle = 34634,
            LastLash = 34635,
            Twinfang = 35921,
            Twinblood = 35922,
            TwinfangBite = 34636,
            TwinfangThresh = 34638,
            TwinbloodBite = 34637,
            TwinbloodThresh = 34639,

            UncoiledFury = 34633,
            UncoiledTwinfang = 34644,
            UncoiledTwinblood = 34645,

            SerpentsIre = 34647,
            Reawaken = 34626,
            FirstGeneration = 34627,
            SecondGeneration = 34628,
            ThirdGeneration = 34629,
            FourthGeneration = 34630,
            Ouroboros = 34631,
            FirstLegacy = 34640,
            SecondLegacy = 34641,
            ThirdLegacy = 34642,
            FourthLegacy = 34643,

            WrithingSnap = 34632,
            Slither = 34646;

    public static class Buffs
    {
        public const ushort
            FlankstungVenom = 3645,
            FlanksbaneVenom = 3646,
            HindstungVenom = 3647,
            HindsbaneVenom = 3648,
            GrimhuntersVenom = 3649,
            GrimskinsVenom = 3650,
            HuntersVenom = 3657,
            SwiftskinsVenom = 3658,
            FellhuntersVenom = 3659,
            FellskinsVenom = 3660,
            PoisedForTwinfang = 3665,
            PoisedForTwinblood = 3666,
            HuntersInstinct = 3668, // Double check, might also be 4120
            Swiftscaled = 3669,     // Might also be 4121
            Reawakened = 3670,
            ReadyToReawaken = 3671,
            HonedSteel = 3672,
            HonedReavers = 3772;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            SteelFangs = 1,
            HuntersSting = 5,
            ReavingFangs = 10,
            WrithingSnap = 15,
            SwiftskinsSting = 20,
            SteelMaw = 25,
            Single3rdCombo = 30, // Includes Flanksting, Flanksbane, Hindsting, and Hindsbane
            ReavingMaw = 35,
            Slither = 40,
            HuntersBite = 40,
            SwiftskinsBite = 45,
            AoE3rdCombo = 50,    // Jagged Maw and Bloodied Maw
            SerpentsTail = 50,
            DeathRattle = 55,
            LastLash = 60,
            Vicewinder = 65,     // Also includes Hunter's Coil and Swiftskin's Coil
            VicePit = 70,        // Also includes Hunter's Den and Swiftskin's Den
            TwinsSingle = 75,    // Twinfang Bite and Twinblood Bite
            TwinsAoE = 80,       // Twinfang Thresh and Twinblood Thresh
            UncoiledFury = 82,
            SerpentsIre = 86,
            EnhancedRattle = 88, // Third stack of Rattling Coil can be accumulated
            Reawaken = 90,       // Also includes First Generation through Fourth Generation
            UncoiledTwins = 92,  // Uncoiled Twinfang and Uncoiled Twinblood
            Ouroboros = 96,      // Also includes a 5th Anguine Tribute stack from Reawaken
            Legacies = 100;      // First through Fourth Legacy
    }
}

internal class ViperFangs : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.SteelFangs)
        {
            var gauge = GetJobGauge<VPRGauge>();
            var maxtribute = level >= VPR.Levels.Ouroboros ? 5 : 4;

            // if (IsEnabled(CustomComboPreset.ViperSteelTailFeature) && OriginalHook(VPR.SerpentsTail) == VPR.DeathRattle && CanUseAction(VPR.DeathRattle))
            //     return VPR.DeathRattle;

            // if (IsEnabled(CustomComboPreset.ViperGenerationLegaciesFeature))
            // {
            //     if (actionID == VPR.SteelFangs && OriginalHook(VPR.SerpentsTail) == VPR.FirstLegacy)
            //         return VPR.FirstLegacy;

            //     if (actionID == VPR.ReavingFangs && OriginalHook(VPR.SerpentsTail) == VPR.SecondLegacy)
            //         return VPR.SecondLegacy;
            // }

            // if (IsEnabled(CustomComboPreset.ViperSteelCoilFeature))
            // {
            //     if (IsEnabled(CustomComboPreset.ViperGenerationLegaciesFeature))
            //     {
            //         if (actionID == VPR.SteelFangs && OriginalHook(VPR.SerpentsTail) == VPR.ThirdLegacy)
            //             return VPR.ThirdLegacy;

            //         if (actionID == VPR.ReavingFangs && OriginalHook(VPR.SerpentsTail) == VPR.FourthLegacy)
            //             return VPR.FourthLegacy;
            //     }

            //     if (IsEnabled(CustomComboPreset.ViperTwinCoilFeature))
            //     {
            //         if (HasEffect(VPR.Buffs.HuntersVenom) && CanUseAction(VPR.TwinfangBite))
            //             return VPR.TwinfangBite;

            //         if (HasEffect(VPR.Buffs.SwiftskinsVenom) && CanUseAction(VPR.TwinbloodBite))
            //             return VPR.TwinbloodBite;

            //         if (OriginalHook(VPR.Twinfang) == VPR.TwinfangBite && CanUseAction(VPR.TwinfangBite))
            //             return VPR.TwinfangBite;

            //         if (OriginalHook(VPR.Twinblood) == VPR.TwinbloodBite && CanUseAction(VPR.TwinbloodBite))
            //             return VPR.TwinbloodBite;
            //     }

            //     if (gauge.AnguineTribute > 0)
            //     {
            //         if (actionID == VPR.SteelFangs)
            //             return gauge.AnguineTribute == maxtribute ? VPR.FirstGeneration : VPR.ThirdGeneration;

            //         if (actionID == VPR.ReavingFangs)
            //             return gauge.AnguineTribute >= maxtribute - 1 ? VPR.SecondGeneration : VPR.FourthGeneration;
            //     }
            //     else
            //     {
            //         if (CanUseAction(VPR.SwiftskinsCoil) || CanUseAction(VPR.HuntersCoil))
            //             return actionID == VPR.SteelFangs ? VPR.HuntersCoil : VPR.SwiftskinsCoil;
            //     }
            // }

            // if (IsEnabled(CustomComboPreset.ViperAutoViceSTFeature) &&
            //     level >= VPR.Levels.Vicewinder && IsOriginal(VPR.ReavingFangs) &&
            //     IsAvailable(VPR.Vicewinder) && IsOriginal(VPR.SerpentsTail))
            //     return VPR.Vicewinder;

            // if (IsEnabled(CustomComboPreset.ViperAutoFangBiteFeature))
            // {
            //     if (OriginalHook(VPR.SteelFangs) == VPR.HindstingStrike)
            //     {
            //         if (HasEffect(VPR.Buffs.HindsbaneVenom))
            //             return VPR.HindsbaneFang;
            //         if (HasEffect(VPR.Buffs.HindstungVenom))
            //             return VPR.HindstingStrike;
            //     }

            //     if (OriginalHook(VPR.SteelFangs) == VPR.FlankstingStrike)
            //     {
            //         if (HasEffect(VPR.Buffs.FlanksbaneVenom))
            //             return VPR.FlanksbaneFang;
            //         if (HasEffect(VPR.Buffs.FlankstungVenom))
            //             return VPR.FlankstingStrike;
            //     }
            // }

            // if (IsEnabled(CustomComboPreset.ViperAutoSteelReavingFeature) &&
            //     OriginalHook(VPR.SteelFangs) == VPR.SteelFangs)
            //     return HasEffect(VPR.Buffs.HonedReavers) ? VPR.ReavingFangs : VPR.SteelFangs;

            if (GCDClipCheck(actionID))
            {
                switch (level)
                {
                    case >= VPR.Levels.SerpentsTail when !IsOriginal(VPR.SerpentsTail):
                        return OriginalHook(VPR.SerpentsTail);
                    case >= VPR.Levels.TwinsSingle when !IsOriginal(VPR.Twinfang):
                        if (CanUseAction(VPR.TwinfangBite))
                        {
                            return HasEffect(VPR.Buffs.SwiftskinsVenom) ? VPR.TwinbloodBite : VPR.TwinfangBite;
                        }
                        break;
                }
            }

            var canUseSSC = CanUseAction(VPR.SwiftskinsCoil);
            var canUseHunters = CanUseAction(VPR.HuntersCoil);



            if (canUseSSC || canUseHunters)
            {
                if (canUseHunters
                    && (!HasEffect(VPR.Buffs.HuntersInstinct)
                        || HasEffect(VPR.Buffs.FlanksbaneVenom)
                        || HasEffect(VPR.Buffs.FlankstungVenom))
                        || !canUseSSC)
                    return VPR.HuntersCoil;

                if (canUseSSC
                    && (HasEffect(VPR.Buffs.HindsbaneVenom)
                        || HasEffect(VPR.Buffs.HindstungVenom)
                        || !canUseHunters))
                    return VPR.SwiftskinsCoil;

                return canUseSSC ? VPR.SwiftskinsCoil : VPR.HuntersCoil;
            }

            if (gauge.RattlingCoilStacks >= 1
                && HasEffect(VPR.Buffs.Swiftscaled)
                && HasEffect(VPR.Buffs.HuntersInstinct)
                && (HasRaidBuffs() || (gauge.RattlingCoilStacks >= 2 && HasCharges(VPR.Vicewinder)))
            )
            {
                return VPR.UncoiledFury;
            }

            if (level >= VPR.Levels.Vicewinder
                && HasCharges(VPR.Vicewinder)
                && HasEffect(VPR.Buffs.Swiftscaled)
                && !canUseSSC
                && !canUseHunters
                && (HasRaidBuffs() || GetCooldown(VPR.Vicewinder).TotalCooldownRemaining <= 5))
            {
                return VPR.Vicewinder;
            }

            // Switch case here for optimization, rather than calling OriginalHook in a lot of places.
            switch (OriginalHook(VPR.SteelFangs))
            {
                // Combo step 1, detect presence of buffs, returned buffed Reavers or SteelFangs
                case VPR.SteelFangs:
                    return HasEffect(VPR.Buffs.HonedReavers) ? VPR.ReavingFangs : VPR.SteelFangs;

                // Combo step 2, prioritize whichever buff we don't have. Starts with Swiftscaled since that speeds up the rotation significantly
                case VPR.HuntersSting:
                    if (level < VPR.Levels.SwiftskinsSting)
                        return VPR.HuntersSting;

                    return HasEffect(VPR.Buffs.FlanksbaneVenom) || HasEffect(VPR.Buffs.FlankstungVenom)
                        ? VPR.HuntersSting
                        : VPR.SwiftskinsSting;

                // Combo step 3, use whichever buff we have, or default to start hindsbane unless otherwise specified
                case VPR.HindstingStrike:
                    if (HasEffect(VPR.Buffs.HindsbaneVenom))
                        return VPR.HindsbaneFang;
                    if (HasEffect(VPR.Buffs.HindstungVenom))
                        return VPR.HindstingStrike;

                    return VPR.HindstingStrike;

                // Combo step 3, flank. Use whichever buff we have, or default to Flanksbane if we're here and buff has fallen off.
                case VPR.FlankstingStrike:
                    if (HasEffect(VPR.Buffs.FlanksbaneVenom))
                        return VPR.FlanksbaneFang;
                    if (HasEffect(VPR.Buffs.FlankstungVenom))
                        return VPR.FlankstingStrike;

                    return VPR.FlankstingStrike;

                // Default return of actionID
                default:
                    return actionID;
            }
        }

        return actionID;
    }
}

/// <summary>
/// This method helps determine the relative positional
/// </summary>
internal class ViperPositionals : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.HuntersCoil || actionID == VPR.SwiftskinsCoil)
        {
            var swiftSkin = CanUseAction(VPR.SwiftskinsCoil);
            var hunter = CanUseAction(VPR.HuntersCoil);

            var hasHindsbane = HasEffect(VPR.Buffs.HindsbaneVenom)
                    || HasEffect(VPR.Buffs.HindstungVenom);

            var hasFlanksbane = HasEffect(VPR.Buffs.FlanksbaneVenom)
                    || HasEffect(VPR.Buffs.FlankstungVenom);

            if ((hasFlanksbane
                    || hunter
                    || !HasEffect(VPR.Buffs.HuntersInstinct))
                && actionID is VPR.HuntersCoil)
            {
                if ((hunter && !hasHindsbane)
                    || !HasEffect(VPR.Buffs.HuntersInstinct)
                    || (hunter && !swiftSkin)
                    || (!swiftSkin && hunter))
                    return VPR.HuntersCoil;

                if (!swiftSkin && !hunter)
                {
                    if (HasEffect(VPR.Buffs.FlanksbaneVenom))
                        return VPR.FlanksbaneFang;
                    if (HasEffect(VPR.Buffs.FlankstungVenom))
                        return VPR.FlankstingStrike;
                }
            }

            if ((hasHindsbane
                    || swiftSkin
                    || HasEffect(VPR.Buffs.HuntersInstinct))
                && actionID is VPR.SwiftskinsCoil)
            {
                if (swiftSkin
                    && (!hasFlanksbane
                        || (swiftSkin && !hunter))
                    && HasEffect(VPR.Buffs.HuntersInstinct))
                    return VPR.SwiftskinsCoil;

                if (!swiftSkin && !hunter)
                {
                    if (HasEffect(VPR.Buffs.HindsbaneVenom))
                        return VPR.HindsbaneFang;
                    return VPR.HindstingStrike;
                }
            }

            return ADV.Swiftcast;
        }

        return actionID;
    }
}

internal class ViperAoE : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.SteelMaw || actionID == VPR.ReavingMaw)
        {
            var gauge = GetJobGauge<VPRGauge>();
            var maxtribute = level >= VPR.Levels.Ouroboros ? 5 : 4;

            // if (IsEnabled(CustomComboPreset.ViperSteelTailFeature) && OriginalHook(VPR.SerpentsTail) == VPR.LastLash && CanUseAction(VPR.LastLash))
            //     return VPR.LastLash;

            // if (IsEnabled(CustomComboPreset.ViperGenerationLegaciesFeature))
            // {
            //     if (actionID == VPR.SteelMaw && OriginalHook(VPR.SerpentsTail) == VPR.FirstLegacy)
            //         return VPR.FirstLegacy;

            //     if (actionID == VPR.ReavingMaw && OriginalHook(VPR.SerpentsTail) == VPR.SecondLegacy)
            //         return VPR.SecondLegacy;
            // }

            // if (IsEnabled(CustomComboPreset.ViperSteelCoilFeature))
            // {
            //     if (IsEnabled(CustomComboPreset.ViperGenerationLegaciesFeature))
            //     {
            //         if (actionID == VPR.SteelMaw && OriginalHook(VPR.SerpentsTail) == VPR.ThirdLegacy)
            //             return VPR.ThirdLegacy;

            //         if (actionID == VPR.ReavingMaw && OriginalHook(VPR.SerpentsTail) == VPR.FourthLegacy)
            //             return VPR.FourthLegacy;
            //     }

            //     if (IsEnabled(CustomComboPreset.ViperTwinCoilFeature))
            //     {
            //         if (HasEffect(VPR.Buffs.FellhuntersVenom) && level >= VPR.Levels.TwinsAoE && CanUseAction(VPR.TwinfangThresh))
            //             return VPR.TwinfangThresh;

            //         if (HasEffect(VPR.Buffs.FellskinsVenom) && level >= VPR.Levels.TwinsAoE && CanUseAction(VPR.TwinbloodThresh))
            //             return VPR.TwinbloodThresh;

            //         if (OriginalHook(VPR.Twinfang) == VPR.TwinfangThresh && CanUseAction(VPR.TwinfangThresh))
            //             return VPR.TwinfangThresh;

            //         if (OriginalHook(VPR.Twinblood) == VPR.TwinbloodThresh && CanUseAction(VPR.TwinbloodThresh))
            //             return VPR.TwinbloodThresh;
            //     }

            //     if (gauge.AnguineTribute > 0)
            //     {
            //         if (actionID == VPR.SteelMaw)
            //             return gauge.AnguineTribute == maxtribute ? VPR.FirstGeneration : VPR.ThirdGeneration;

            //         if (actionID == VPR.ReavingMaw)
            //             return gauge.AnguineTribute >= maxtribute - 1 ? VPR.SecondGeneration : VPR.FourthGeneration;
            //     }
            //     else
            //     {
            //         if (CanUseAction(VPR.SwiftskinsDen) || CanUseAction(VPR.HuntersDen))
            //             return actionID == VPR.SteelMaw ? VPR.HuntersDen : VPR.SwiftskinsDen;
            //     }
            // }

            // if (IsEnabled(CustomComboPreset.ViperAutoViceAoEFeature) &&
            //     level >= VPR.Levels.VicePit && IsOriginal(VPR.ReavingMaw) &&
            //     IsAvailable(VPR.VicePit) && IsOriginal(VPR.SerpentsTail))
            //     return VPR.VicePit;

            // if (IsEnabled(CustomComboPreset.ViperAutoFangBiteFeature))
            // {
            //     if (OriginalHook(VPR.SteelMaw) == VPR.JaggedMaw)
            //     {
            //         if (HasEffect(VPR.Buffs.GrimhuntersVenom) && level >= VPR.Levels.AoE3rdCombo)
            //             return VPR.JaggedMaw;
            //         if (HasEffect(VPR.Buffs.GrimskinsVenom) && level >= VPR.Levels.AoE3rdCombo)
            //             return VPR.BloodiedMaw;
            //     }
            // }

            // if (IsEnabled(CustomComboPreset.ViperAutoSteelReavingFeature) &&
            //     OriginalHook(VPR.SteelMaw) == VPR.SteelMaw)
            //     return (HasEffect(VPR.Buffs.HonedReavers) && level >= VPR.Levels.ReavingMaw) ? VPR.ReavingMaw : VPR.SteelMaw;

            if (GCDClipCheck(actionID))
            {
                switch (level)
                {
                    case >= VPR.Levels.TwinsAoE when !IsOriginal(VPR.Twinfang):
                        if (CanUseAction(VPR.TwinfangThresh))
                        {
                            return HasEffect(VPR.Buffs.FellskinsVenom) ? VPR.TwinbloodThresh : VPR.TwinfangThresh;
                        }
                        break;
                    case >= VPR.Levels.LastLash when !IsOriginal(VPR.SerpentsTail):
                        return OriginalHook(VPR.SerpentsTail);
                }
            }

            var canUseSSC = CanUseAction(VPR.SwiftskinsDen);
            var canUseHunters = CanUseAction(VPR.HuntersDen);

            if (canUseSSC || canUseHunters)
            {
                if (canUseHunters
                    && (!HasEffect(VPR.Buffs.HuntersInstinct)
                        || HasEffect(VPR.Buffs.FlanksbaneVenom)
                        || HasEffect(VPR.Buffs.FlankstungVenom))
                        || !canUseSSC
                        )
                    return VPR.HuntersDen;

                if (canUseSSC
                    && (HasEffect(VPR.Buffs.HindsbaneVenom)
                        || HasEffect(VPR.Buffs.HindstungVenom)
                        || !canUseHunters))
                    return VPR.SwiftskinsDen;

                return canUseSSC ? VPR.SwiftskinsDen : VPR.HuntersDen;
            }

            if (gauge.RattlingCoilStacks >= 1
                && HasEffect(VPR.Buffs.Swiftscaled)
                && HasEffect(VPR.Buffs.HuntersInstinct)
            )
            {
                return VPR.UncoiledFury;
            }

            if (level >= VPR.Levels.VicePit
                && HasCharges(VPR.VicePit)
                && HasEffect(VPR.Buffs.Swiftscaled)
                && !canUseSSC
                && !canUseHunters)
            {
                return VPR.VicePit;
            }

            switch (OriginalHook(VPR.SteelMaw))
            {
                case VPR.SteelMaw:
                    return (HasEffect(VPR.Buffs.HonedReavers) && level >= VPR.Levels.ReavingMaw) ? VPR.ReavingMaw : VPR.SteelMaw;

                case VPR.HuntersBite:
                    if (level >= VPR.Levels.SwiftskinsBite)
                    {
                        var swift = FindEffect(VPR.Buffs.Swiftscaled);
                        var instinct = FindEffect(VPR.Buffs.HuntersInstinct);
                        if (swift is null || swift?.RemainingTime <= instinct?.RemainingTime) // We'd always want to prioritize swift since it speeds up the rotation
                            return VPR.SwiftskinsBite;
                    }
                    return VPR.HuntersBite;
                case VPR.JaggedMaw:
                    if (HasEffect(VPR.Buffs.GrimskinsVenom))
                        return VPR.BloodiedMaw;
                    if (HasEffect(VPR.Buffs.GrimhuntersVenom))
                        return VPR.JaggedMaw;

                    return VPR.JaggedMaw;

                default:
                    return actionID;
            }

        }

        return actionID;
    }
}

internal class ViperCoils : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.HuntersCoil || actionID == VPR.SwiftskinsCoil)
        {
            if (IsEnabled(CustomComboPreset.ViperGenerationLegaciesFeature))
            {
                if (actionID == VPR.HuntersCoil && OriginalHook(VPR.SerpentsTail) == VPR.ThirdLegacy)
                    return VPR.ThirdLegacy;

                if (actionID == VPR.SwiftskinsCoil && OriginalHook(VPR.SerpentsTail) == VPR.FourthLegacy)
                    return VPR.FourthLegacy;
            }

            if (IsEnabled(CustomComboPreset.ViperTwinCoilFeature))
            {
                if (HasEffect(VPR.Buffs.HuntersVenom) && CanUseAction(VPR.TwinfangBite))
                    return VPR.TwinfangBite;

                if (HasEffect(VPR.Buffs.SwiftskinsVenom) && CanUseAction(VPR.TwinbloodBite))
                    return VPR.TwinbloodBite;

                if (OriginalHook(VPR.Twinfang) == VPR.TwinfangBite && CanUseAction(VPR.TwinfangBite))
                    return VPR.TwinfangBite;

                if (OriginalHook(VPR.Twinblood) == VPR.TwinbloodBite && CanUseAction(VPR.TwinbloodBite))
                    return VPR.TwinbloodBite;
            }
        }

        return actionID;
    }
}

internal class ViperDens : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.HuntersDen || actionID == VPR.SwiftskinsDen)
        {
            if (IsEnabled(CustomComboPreset.ViperGenerationLegaciesFeature))
            {
                if (actionID == VPR.HuntersDen && OriginalHook(VPR.SerpentsTail) == VPR.ThirdLegacy)
                    return VPR.ThirdLegacy;

                if (actionID == VPR.SwiftskinsDen && OriginalHook(VPR.SerpentsTail) == VPR.FourthLegacy)
                    return VPR.FourthLegacy;
            }

            if (IsEnabled(CustomComboPreset.ViperTwinCoilFeature))
            {
                if (HasEffect(VPR.Buffs.FellhuntersVenom) && level >= VPR.Levels.TwinsAoE && CanUseAction(VPR.TwinfangThresh))
                    return VPR.TwinfangThresh;

                if (HasEffect(VPR.Buffs.FellskinsVenom) && level >= VPR.Levels.TwinsAoE && CanUseAction(VPR.TwinbloodThresh))
                    return VPR.TwinbloodThresh;

                if (OriginalHook(VPR.Twinfang) == VPR.TwinfangThresh && CanUseAction(VPR.TwinfangThresh))
                    return VPR.TwinfangThresh;

                if (OriginalHook(VPR.Twinblood) == VPR.TwinbloodThresh && CanUseAction(VPR.TwinbloodThresh))
                    return VPR.TwinbloodThresh;
            }
        }

        return actionID;
    }
}

internal class ViperUncoiled : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.UncoiledFury)
        {
            var gauge = GetJobGauge<VPRGauge>();

            if (IsEnabled(CustomComboPreset.ViperUncoiledFollowupFeature))
            {
                if (OriginalHook(VPR.Twinfang) == VPR.UncoiledTwinfang &&
                    HasEffect(VPR.Buffs.PoisedForTwinfang)
                    && CanUseAction(VPR.UncoiledTwinfang))
                    return VPR.UncoiledTwinfang;

                if (OriginalHook(VPR.Twinblood) == VPR.UncoiledTwinblood && CanUseAction(VPR.UncoiledTwinblood))
                    return VPR.UncoiledTwinblood;
            }

            if (IsEnabled(CustomComboPreset.ViperFuryAndIreFeature) && level >= VPR.Levels.SerpentsIre && CanUseAction(VPR.SerpentsIre))
            {
                if (gauge.RattlingCoilStacks == 0)
                    return VPR.SerpentsIre;
            }

            // if (IsEnabled(CustomComboPreset.ViperSnapCoilFeature) && gauge.RattlingCoilStacks == 0)
            // {
            //     return VPR.WrithingSnap;
            // }
        }

        return actionID;
    }
}

internal class ViperVicewinder : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.Vicewinder)
        {
            if (IsEnabled(CustomComboPreset.ViperTwinCoilFeature))
            {
                if (HasEffect(VPR.Buffs.HuntersVenom) && CanUseAction(VPR.TwinfangBite))
                    return VPR.TwinfangBite;

                if (HasEffect(VPR.Buffs.SwiftskinsVenom) && CanUseAction(VPR.TwinbloodBite))
                    return VPR.TwinbloodBite;

                if (OriginalHook(VPR.Twinfang) == VPR.TwinfangBite && CanUseAction(VPR.TwinfangBite))
                    return VPR.TwinfangBite;

                if (OriginalHook(VPR.Twinblood) == VPR.TwinbloodBite && CanUseAction(VPR.TwinbloodBite))
                    return VPR.TwinbloodBite;
            }

            if (IsEnabled(CustomComboPreset.ViperPvPWinderComboFeature))
            {
                var gauge = GetJobGauge<VPRGauge>();
                if (level >= VPR.Levels.Ouroboros && HasEffect(VPR.Buffs.Reawakened) && gauge.AnguineTribute == 1)
                    return VPR.Ouroboros;

                if (IsEnabled(CustomComboPreset.ViperPvPWinderComboStartHuntersFeature) && CanUseAction(VPR.HuntersCoil))
                    return VPR.HuntersCoil;

                if (CanUseAction(VPR.SwiftskinsCoil))
                    return VPR.SwiftskinsCoil;

                if (CanUseAction(VPR.HuntersCoil))
                    return VPR.HuntersCoil;

                return VPR.Vicewinder;
            }
        }

        return actionID;
    }
}

internal class ViperVicepit : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.VicePit)
        {
            if (IsEnabled(CustomComboPreset.ViperTwinCoilFeature))
            {
                if (HasEffect(VPR.Buffs.FellhuntersVenom) && CanUseAction(VPR.TwinfangThresh))
                    return VPR.TwinfangThresh;

                if (HasEffect(VPR.Buffs.FellskinsVenom) && CanUseAction(VPR.TwinbloodThresh))
                    return VPR.TwinbloodThresh;

                if (OriginalHook(VPR.Twinfang) == VPR.TwinfangThresh && CanUseAction(VPR.TwinfangThresh))
                    return VPR.TwinfangThresh;

                if (OriginalHook(VPR.Twinblood) == VPR.TwinbloodThresh && CanUseAction(VPR.TwinbloodThresh))
                    return VPR.TwinbloodThresh;
            }

            if (IsEnabled(CustomComboPreset.ViperPvPPitComboFeature))
            {
                var gauge = GetJobGauge<VPRGauge>();
                if (level >= VPR.Levels.Ouroboros && HasEffect(VPR.Buffs.Reawakened) && gauge.AnguineTribute == 1)
                    return VPR.Ouroboros;

                if (IsEnabled(CustomComboPreset.ViperPvPPitComboStartHuntersFeature) && CanUseAction(VPR.HuntersDen))
                    return VPR.HuntersDen;

                if (CanUseAction(VPR.SwiftskinsDen))
                    return VPR.SwiftskinsDen;

                if (CanUseAction(VPR.HuntersDen))
                    return VPR.HuntersDen;

                return VPR.VicePit;
            }
        }

        return actionID;
    }
}

internal class ViperReawaken : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.Reawaken)
        {
            // if (IsEnabled(CustomComboPreset.ViperReawakenAIOFeature) && HasEffect(VPR.Buffs.Reawakened))
            // {
            //     var gauge = GetJobGauge<VPRGauge>();

            //     if (level >= VPR.Levels.Legacies)
            //     {
            //         var original = OriginalHook(VPR.SerpentsTail);
            //         if (original is VPR.FirstLegacy or
            //                         VPR.SecondLegacy or
            //                         VPR.ThirdLegacy or
            //                         VPR.FourthLegacy)
            //             return original;
            //     }

            //     var maxtribute = level >= VPR.Levels.Ouroboros ? 5 : 4;
            //     if (gauge.AnguineTribute == maxtribute)
            //         return VPR.FirstGeneration;
            //     if (gauge.AnguineTribute == maxtribute - 1)
            //         return VPR.SecondGeneration;
            //     if (gauge.AnguineTribute == maxtribute - 2)
            //         return VPR.ThirdGeneration;
            //     if (gauge.AnguineTribute == maxtribute - 3)
            //         return VPR.FourthGeneration;
            //     if (gauge.AnguineTribute == 1 && level >= VPR.Levels.Ouroboros)
            //         return VPR.Ouroboros;
            // }

            // if (IsEnabled(CustomComboPreset.ViperReawakenIreFeature) && IsAvailable(VPR.SerpentsIre) && CanUseAction(VPR.SerpentsIre))
            // {
            //     return VPR.SerpentsIre;
            // }
        }

        return actionID;
    }
}

internal class ViperoGCDs : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.SerpentsTail)
        {
            if (IsEnabled(CustomComboPreset.ViperMergeSerpentTwinsFeature))
            {
                if (!IsOriginal(VPR.SerpentsTail))
                    return OriginalHook(VPR.SerpentsTail);

                if ((HasEffect(VPR.Buffs.PoisedForTwinfang) ||
                    HasEffect(VPR.Buffs.HuntersVenom) ||
                    HasEffect(VPR.Buffs.FellhuntersVenom)) && CanUseAction(OriginalHook(VPR.Twinfang)))
                    return OriginalHook(VPR.Twinfang);

                if ((HasEffect(VPR.Buffs.PoisedForTwinblood) ||
                    HasEffect(VPR.Buffs.SwiftskinsVenom) ||
                    HasEffect(VPR.Buffs.FellskinsVenom)) && CanUseAction(OriginalHook(VPR.Twinblood)))
                    return OriginalHook(VPR.Twinblood);

                if (!IsOriginal(VPR.Twinfang) && CanUseAction(OriginalHook(VPR.Twinfang)))
                    return OriginalHook(VPR.Twinfang);

                if (!IsOriginal(VPR.Twinblood) && CanUseAction(OriginalHook(VPR.Twinblood)))
                    return OriginalHook(VPR.Twinblood);
            }
        }

        if (actionID == VPR.Twinfang || actionID == VPR.Twinblood)
        {
            if (IsEnabled(CustomComboPreset.ViperMergeTwinsSerpentFeature) && !IsOriginal(VPR.SerpentsTail) && CanUseAction(OriginalHook(VPR.SerpentsTail)))
                return OriginalHook(VPR.SerpentsTail);
        }

        return actionID;
    }
}