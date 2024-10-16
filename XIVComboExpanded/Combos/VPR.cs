using System.Linq;
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
            Swiftscaled = 3669, // Might also be 4121
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
            AoE3rdCombo = 50, // Jagged Maw and Bloodied Maw
            SerpentsTail = 50,
            DeathRattle = 55,
            LastLash = 60,
            Vicewinder = 65, // Also includes Hunter's Coil and Swiftskin's Coil
            VicePit = 70, // Also includes Hunter's Den and Swiftskin's Den
            TwinsSingle = 75, // Twinfang Bite and Twinblood Bite
            TwinsAoE = 80, // Twinfang Thresh and Twinblood Thresh
            UncoiledFury = 82,
            SerpentsIre = 86,
            EnhancedRattle = 88, // Third stack of Rattling Coil can be accumulated
            Reawaken = 90, // Also includes First Generation through Fourth Generation
            UncoiledTwins = 92, // Uncoiled Twinfang and Uncoiled Twinblood
            Ouroboros = 96, // Also includes a 5th Anguine Tribute stack from Reawaken
            Legacies = 100; // First through Fourth Legacy
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
            var maxTribute = level >= VPR.Levels.Ouroboros ? 5 : 4;
            var rattleCount = level >= VPR.Levels.EnhancedRattle ? 3 : 2;
            var raidbuffs = HasRaidBuffs(1);

            var flanksbaneVenom = FindEffect(VPR.Buffs.FlanksbaneVenom);
            var flankstungVenom = FindEffect(VPR.Buffs.FlankstungVenom);

            var hindsbaneVenom = FindEffect(VPR.Buffs.HindsbaneVenom);
            var hindstungVenom = FindEffect(VPR.Buffs.HindstungVenom);

            var huntersVenom = FindEffect(VPR.Buffs.HuntersVenom);
            var swiftskinsVenom = FindEffect(VPR.Buffs.SwiftskinsVenom);

            if (GCDClipCheck(actionID))
                switch (level)
                {
                    case >= VPR.Levels.SerpentsTail when !IsOriginal(VPR.SerpentsTail):
                        return OriginalHook(VPR.SerpentsTail);
                    case >= VPR.Levels.TwinsSingle when !IsOriginal(VPR.Twinfang):
                        if (CanUseAction(VPR.TwinfangBite))
                            return HasEffect(VPR.Buffs.SwiftskinsVenom) ? VPR.TwinbloodBite : VPR.TwinfangBite;
                        if (CanUseAction(VPR.UncoiledTwinfang))
                            return HasEffect(VPR.Buffs.PoisedForTwinblood)
                                ? VPR.UncoiledTwinblood
                                : VPR.UncoiledTwinfang;
                        break;
                    case >= VPR.Levels.SerpentsIre when
                        IsOffCooldown(VPR.SerpentsIre)
                        // && raidbuffs
                        && gauge.RattlingCoilStacks < rattleCount:
                        return VPR.SerpentsIre;
                }

            var canUseSSC = CanUseAction(VPR.SwiftskinsCoil);
            var canUseHunters = CanUseAction(VPR.HuntersCoil);

            if (canUseSSC || canUseHunters)
            {
                if ((canUseHunters
                     && (!HasEffect(VPR.Buffs.HuntersInstinct)
                         || flanksbaneVenom is not null
                         || flankstungVenom is not null))
                    || !canUseSSC)
                    return VPR.HuntersCoil;

                if (canUseSSC
                    && (hindsbaneVenom is not null
                        || hindstungVenom is not null
                        || !canUseHunters))
                    return VPR.SwiftskinsCoil;

                return canUseSSC ? VPR.SwiftskinsCoil : VPR.HuntersCoil;
            }

            if (gauge.AnguineTribute == maxTribute)
                return VPR.FirstGeneration;
            if (gauge.AnguineTribute == maxTribute - 1)
                return VPR.SecondGeneration;
            if (gauge.AnguineTribute == maxTribute - 2)
                return VPR.ThirdGeneration;
            if (gauge.AnguineTribute == maxTribute - 3)
                return VPR.FourthGeneration;
            if (gauge.AnguineTribute == 1 && level >= VPR.Levels.Ouroboros)
                return VPR.Ouroboros;

            var readyToReawaken = FindEffect(VPR.Buffs.ReadyToReawaken);

            if (HasEffect(VPR.Buffs.Swiftscaled)
                && HasEffect(VPR.Buffs.HuntersInstinct)
                && HasTarget()
               )
            {

                var hasPostionalBuff = new[]
                {
                    hindsbaneVenom, hindstungVenom,
                    flanksbaneVenom, flankstungVenom,
                    huntersVenom, swiftskinsVenom
                };

                var noExpiringBuffs = hasPostionalBuff.Any(buff => buff is not null && buff.RemainingTime >= 15);

                if ((gauge.SerpentOffering >= 50 || readyToReawaken is not null)
                    && (gauge.SerpentOffering >= 90
                        || raidbuffs
                        || readyToReawaken?.RemainingTime <= 10)
                    && gauge.AnguineTribute < 1)
                    return VPR.Reawaken;

                if (gauge.RattlingCoilStacks >= 1
                    && noExpiringBuffs
                    && (TargetIsLow() ||
                    (gauge.RattlingCoilStacks == rattleCount
                            && (HasCharges(VPR.Vicewinder)
                                || IsOffCooldown(VPR.SerpentsIre)))))
                    return VPR.UncoiledFury;
            }

            if (level >= VPR.Levels.Vicewinder
                && HasCharges(VPR.Vicewinder)
                && HasEffect(VPR.Buffs.Swiftscaled)
                && !canUseSSC
                && !canUseHunters
                && (raidbuffs || GetCooldown(VPR.Vicewinder).TotalCooldownRemaining <= 12))
                return VPR.Vicewinder;

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

                    return flanksbaneVenom is not null || flankstungVenom is not null
                        ? VPR.HuntersSting
                        : VPR.SwiftskinsSting;

                // Combo step 3, use whichever buff we have, or default to start hindsbane unless otherwise specified
                case VPR.HindstingStrike:
                    if (hindsbaneVenom is not null)
                        return VPR.HindsbaneFang;
                    if (hindstungVenom is not null)
                        return VPR.HindstingStrike;

                    return VPR.HindstingStrike;

                // Combo step 3, flank. Use whichever buff we have, or default to Flanksbane if we're here and buff has fallen off.
                case VPR.FlankstingStrike:
                    if (flanksbaneVenom is not null)
                        return VPR.FlanksbaneFang;
                    if (flankstungVenom is not null)
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
///     This method helps determine the relative positional
/// </summary>
internal class ViperPositionals : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.HuntersCoil || actionID == VPR.SwiftskinsCoil)
        {
            var canUseSwiftSkinCoil = CanUseAction(VPR.SwiftskinsCoil);
            var canUseHuntersCoil = CanUseAction(VPR.HuntersCoil);

            var hasRearBuff = HasEffect(VPR.Buffs.HindsbaneVenom) || HasEffect(VPR.Buffs.HindstungVenom);
            var hasFlankBuff = HasEffect(VPR.Buffs.FlanksbaneVenom) || HasEffect(VPR.Buffs.FlankstungVenom);

            if ((hasFlankBuff
                 || canUseHuntersCoil
                 || (!HasEffect(VPR.Buffs.HuntersInstinct) && level >= VPR.Levels.Vicewinder))
                && actionID is VPR.HuntersCoil)
            {
                // enable if we can use HunterCoil but not if our current buffs want us in the rear
                if ((canUseHuntersCoil && !hasRearBuff)
                    // Enable this position if we need to get the Hunter's Instinct buff
                    || (!HasEffect(VPR.Buffs.HuntersInstinct) && level >= VPR.Levels.Vicewinder)
                    // Enable this position if we have already used the other position
                    || (canUseHuntersCoil && !canUseSwiftSkinCoil))
                    return VPR.HuntersCoil;

                if (!canUseSwiftSkinCoil && !canUseHuntersCoil)
                {
                    if (HasEffect(VPR.Buffs.FlanksbaneVenom))
                        return VPR.FlanksbaneFang;
                    if (HasEffect(VPR.Buffs.FlankstungVenom))
                        return VPR.FlankstingStrike;
                }
            }

            // TODO: Vicewinder is now behaving correctly

            if ((hasRearBuff
                 || canUseSwiftSkinCoil
                 || (HasEffect(VPR.Buffs.HuntersInstinct)
                     && (level >= VPR.Levels.Vicewinder
                         || (!hasRearBuff && !hasFlankBuff)))
                )
                && actionID is VPR.SwiftskinsCoil)
            {
                // Enable this position if we can use SwiftSkinCoil but not if our current buffs want us in the flank
                if (((canUseSwiftSkinCoil && !hasFlankBuff)
                     // Enable this position if we have already used the other position
                     || (canUseSwiftSkinCoil && !canUseHuntersCoil)
                    // Enable this position if we have nothing
                    // || (!hasRearBuff && !hasFlankBuff)
                    )
                    // Enable this position ONLY if we already have the Hunter's Instinct buff
                    && (HasEffect(VPR.Buffs.HuntersInstinct) || level < VPR.Levels.Vicewinder))
                    return VPR.SwiftskinsCoil;

                if (!canUseSwiftSkinCoil && !canUseHuntersCoil)
                {
                    if (HasEffect(VPR.Buffs.HindsbaneVenom))
                        return VPR.HindsbaneFang;
                    if (HasEffect(VPR.Buffs.HindstungVenom))
                        return VPR.HindstingStrike;
                    if (!hasRearBuff && !hasFlankBuff)
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

            if (GCDClipCheck(actionID))
                switch (level)
                {
                    case >= VPR.Levels.TwinsAoE when !IsOriginal(VPR.Twinfang):
                        if (CanUseAction(VPR.TwinfangThresh))
                            return HasEffect(VPR.Buffs.FellskinsVenom) ? VPR.TwinbloodThresh : VPR.TwinfangThresh;
                        if (CanUseAction(VPR.UncoiledTwinfang))
                            return HasEffect(VPR.Buffs.PoisedForTwinblood)
                                ? VPR.UncoiledTwinblood
                                : VPR.UncoiledTwinfang;
                        break;
                    case >= VPR.Levels.LastLash when !IsOriginal(VPR.SerpentsTail):
                        return OriginalHook(VPR.SerpentsTail);
                    case >= VPR.Levels.SerpentsIre when IsOffCooldown(VPR.SerpentsIre):
                        return VPR.SerpentsIre;
                }

            var canUseSSC = CanUseAction(VPR.SwiftskinsDen);
            var canUseHunters = CanUseAction(VPR.HuntersDen);

            if (canUseSSC || canUseHunters)
            {
                if ((canUseHunters
                     && (!HasEffect(VPR.Buffs.HuntersInstinct)
                         || HasEffect(VPR.Buffs.FlanksbaneVenom)
                         || HasEffect(VPR.Buffs.FlankstungVenom)))
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

            var readyToReawaken = FindEffect(VPR.Buffs.ReadyToReawaken);


            if (gauge.AnguineTribute == maxtribute)
                return VPR.FirstGeneration;
            if (gauge.AnguineTribute == maxtribute - 1)
                return VPR.SecondGeneration;
            if (gauge.AnguineTribute == maxtribute - 2)
                return VPR.ThirdGeneration;
            if (gauge.AnguineTribute == maxtribute - 3)
                return VPR.FourthGeneration;
            if (gauge.AnguineTribute == 1 && level >= VPR.Levels.Ouroboros)
                return VPR.Ouroboros;

            if (HasEffect(VPR.Buffs.Swiftscaled) && HasEffect(VPR.Buffs.HuntersInstinct))
            {
                if ((gauge.SerpentOffering >= 50
                     || readyToReawaken is not null)
                    && gauge.AnguineTribute < 1
                    && !canUseSSC
                    && !canUseHunters)
                    return VPR.Reawaken;

                if (gauge.RattlingCoilStacks >= 1) return VPR.UncoiledFury;
            }

            if (level >= VPR.Levels.VicePit
                && HasCharges(VPR.VicePit)
                && HasEffect(VPR.Buffs.Swiftscaled)
                && !canUseSSC
                && !canUseHunters)
                return VPR.VicePit;

            switch (OriginalHook(VPR.SteelMaw))
            {
                case VPR.SteelMaw:
                    return HasEffect(VPR.Buffs.HonedReavers) && level >= VPR.Levels.ReavingMaw
                        ? VPR.ReavingMaw
                        : VPR.SteelMaw;

                case VPR.HuntersBite:
                    if (level >= VPR.Levels.SwiftskinsBite)
                    {
                        var swift = FindEffect(VPR.Buffs.Swiftscaled);
                        var instinct = FindEffect(VPR.Buffs.HuntersInstinct);
                        if (swift is null ||
                            swift?.RemainingTime <=
                            instinct
                                ?.RemainingTime) // We'd always want to prioritize swift since it speeds up the rotation
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

internal class ViperRanged : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == VPR.WrithingSnap)
        {
            var gauge = GetJobGauge<VPRGauge>();
            var maxtribute = level >= VPR.Levels.Ouroboros ? 5 : 4;

            if (GCDClipCheck(actionID))
                switch (level)
                {
                    case >= VPR.Levels.TwinsAoE when !IsOriginal(VPR.Twinfang):
                        if (CanUseAction(VPR.TwinfangThresh))
                            return HasEffect(VPR.Buffs.FellskinsVenom) ? VPR.TwinbloodThresh : VPR.TwinfangThresh;
                        if (CanUseAction(VPR.UncoiledTwinfang))
                            return HasEffect(VPR.Buffs.PoisedForTwinblood)
                                ? VPR.UncoiledTwinblood
                                : VPR.UncoiledTwinfang;
                        break;
                    case >= VPR.Levels.LastLash when !IsOriginal(VPR.SerpentsTail):
                        return OriginalHook(VPR.SerpentsTail);
                    case >= VPR.Levels.SerpentsIre when IsOffCooldown(VPR.SerpentsIre):
                        return VPR.SerpentsIre;
                }

            if (HasEffect(VPR.Buffs.Swiftscaled) && HasEffect(VPR.Buffs.HuntersInstinct))
                if (gauge.RattlingCoilStacks >= 1)
                    return VPR.UncoiledFury;
        }

        return actionID;
    }
}