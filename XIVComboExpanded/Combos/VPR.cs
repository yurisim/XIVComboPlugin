using System.Linq;

using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class VPR
{
    public const byte JobID = 41;

    public const uint SteelFangs = 34606,
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

    /// <summary>
    ///     Selects the coil for the current Vicewinder window. Pure decision shared by the main combo and the HUD.
    /// </summary>
    /// <param name="canUseHunters">Whether Hunters Coil is currently executable.</param>
    /// <param name="canUseSwiftskins">Whether Swiftskins Coil is currently executable.</param>
    /// <param name="huntersMissing">Whether Hunters Instinct is fully missing.</param>
    /// <param name="swiftscaledMissing">Whether Swiftscaled is fully missing.</param>
    /// <param name="huntersExpiring">Whether Hunters Instinct is expiring.</param>
    /// <param name="swiftscaledExpiring">Whether Swiftscaled is expiring.</param>
    /// <param name="hasFlankVenom">Whether a flank venom is held.</param>
    /// <param name="hasHindVenom">Whether a hind venom is held. Hind and empty both default to Swiftskins Coil.</param>
    /// <param name="positionIndependent">Whether positionals are disregarded (omnidirectional target or True North).</param>
    /// <param name="isFlanking">Whether the player is in a flank sector. Front defaults to rear.</param>
    /// <returns>Hunters Coil, Swiftskins Coil, or 0 when neither coil is usable.</returns>
    internal static uint SelectCoil(bool canUseHunters, bool canUseSwiftskins, bool huntersMissing, bool swiftscaledMissing, bool huntersExpiring, bool swiftscaledExpiring, bool hasFlankVenom, bool hasHindVenom, bool positionIndependent, bool isFlanking)
    {
        if (!canUseHunters && !canUseSwiftskins)
            return 0;

        // Forced second coil: only one is executable, so take it regardless of position.
        if (canUseHunters && !canUseSwiftskins)
            return HuntersCoil;

        if (canUseSwiftskins && !canUseHunters)
            return SwiftskinsCoil;

        // First coil: when either buff is fully missing, follow position entirely;
        // the forced second coil covers the other buff. Expiring buffs still override below.
        if (huntersMissing || swiftscaledMissing)
        {
            if (positionIndependent)
                return SwiftskinsCoil;

            return isFlanking ? HuntersCoil : SwiftskinsCoil;
        }

        if (huntersExpiring)
            return HuntersCoil;

        if (swiftscaledExpiring)
            return SwiftskinsCoil;

        // Omnidirectional target or True North: fall back to the venom-driven choice.
        if (positionIndependent)
            return hasFlankVenom ? HuntersCoil : SwiftskinsCoil;

        // Coils grant (not consume) venoms, so match the coil to our position.
        // Front matches neither sector, so default rear with Swiftskins Coil.
        _ = hasHindVenom;

        return isFlanking ? HuntersCoil : SwiftskinsCoil;
    }

    /// <summary>
    ///     Selects the step-3 finisher for the current Reaving combo branch. Pure decision shared by the main combo and the HUD.
    /// </summary>
    /// <param name="rearBranch">Whether the combo is on the rear branch (Hindsting Strike). Flank branch selects the Strike pair otherwise.</param>
    /// <param name="hasFlanksbaneVenom">Whether Flanksbane Venom is held.</param>
    /// <param name="hasFlankstungVenom">Whether Flankstung Venom is held.</param>
    /// <param name="hasHindsbaneVenom">Whether Hindsbane Venom is held.</param>
    /// <param name="hasHindstungVenom">Whether Hindstung Venom is held.</param>
    /// <returns>Flanksbane Fang, Flanksting Strike, Hindsbane Fang, or Hindsting Strike.</returns>
    internal static uint SelectFinisher(bool rearBranch, bool hasFlanksbaneVenom, bool hasFlankstungVenom, bool hasHindsbaneVenom, bool hasHindstungVenom)
    {
        if (rearBranch)
        {
            if (hasHindsbaneVenom)
                return HindsbaneFang;
            if (hasHindstungVenom)
                return HindstingStrike;

            return HindstingStrike;
        }

        if (hasFlanksbaneVenom)
            return FlanksbaneFang;
        if (hasFlankstungVenom)
            return FlankstingStrike;

        return FlankstingStrike;
    }

    public static class Buffs
    {
        public const ushort FlankstungVenom = 3645,
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
        public const ushort Placeholder = 0;
    }

    public static class Levels
    {
        public const byte SteelFangs = 1,
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
            {
                switch (level)
                {
                    case >= VPR.Levels.SerpentsTail when !IsOriginal(VPR.SerpentsTail):
                        return OriginalHook(VPR.SerpentsTail);
                    case >= VPR.Levels.TwinsSingle when !IsOriginal(VPR.Twinfang):
                        if (CanUseAction(VPR.TwinfangBite))
                        {
                            return HasEffect(VPR.Buffs.SwiftskinsVenom)
                                ? VPR.TwinbloodBite
                                : VPR.TwinfangBite;
                        }

                        if (CanUseAction(VPR.UncoiledTwinfang))
                        {
                            return HasEffect(VPR.Buffs.PoisedForTwinblood)
                                ? VPR.UncoiledTwinblood
                                : VPR.UncoiledTwinfang;
                        }

                        break;
                    case >= VPR.Levels.SerpentsIre
                        when IsOffCooldown(VPR.SerpentsIre)
                            // && raidbuffs
                            && gauge.RattlingCoilStacks < rattleCount:
                        return VPR.SerpentsIre;
                }
            }

            var canUseSSC = CanUseAction(VPR.SwiftskinsCoil);
            var canUseHunters = CanUseAction(VPR.HuntersCoil);

            var swiftscaledBuff = FindEffect(VPR.Buffs.Swiftscaled);
            var huntersInstinctBuff = FindEffect(VPR.Buffs.HuntersInstinct);

            var coil = VPR.SelectCoil(
                canUseHunters,
                canUseSSC,
                huntersInstinctBuff is null,
                swiftscaledBuff is null,
                huntersInstinctBuff is not null && huntersInstinctBuff.RemainingTime <= 15,
                swiftscaledBuff is not null && swiftscaledBuff.RemainingTime <= 15,
                flanksbaneVenom is not null || flankstungVenom is not null,
                hindsbaneVenom is not null || hindstungVenom is not null,
                !TargetHasPositionals() || HasEffect(ADV.Buffs.TrueNorth),
                IsFlankingTarget());

            if (coil != 0)
                return coil;

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

            if (
                HasEffect(VPR.Buffs.Swiftscaled)
                && HasEffect(VPR.Buffs.HuntersInstinct)
                && HasTarget())
            {
                var hasPostionalBuff = new[]
                {
                    hindsbaneVenom,
                    hindstungVenom,
                    flanksbaneVenom,
                    flankstungVenom,
                    huntersVenom,
                    swiftskinsVenom,
                };

                var noExpiringBuffs = hasPostionalBuff.Any(buff =>
                    buff is not null && buff.RemainingTime >= 15);

                if (
                    (gauge.SerpentOffering >= 50 || readyToReawaken is not null)
                    && (
                        gauge.SerpentOffering >= 90
                        || raidbuffs
                        || readyToReawaken?.RemainingTime <= 10)
                    && gauge.AnguineTribute < 1)
                    return VPR.Reawaken;

                if (
                    gauge.RattlingCoilStacks >= 1
                    && noExpiringBuffs
                    && (
                        TargetHasLowLife()
                        || (
                            gauge.RattlingCoilStacks == rattleCount
                            && (HasCharges(VPR.Vicewinder) || IsOffCooldown(VPR.SerpentsIre)))))
                    return VPR.UncoiledFury;
            }

            if (
                level >= VPR.Levels.Vicewinder
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

                // Combo step 2, prioritize whichever buff we don't have. Starts with Swiftscaled since that speeds up the rotation significantly.
                // Position only steers this step when buffs are healthy and no venom is held: step 2 fixes the step-3 branch (and hence its positional).
                case VPR.HuntersSting:
                {
                    if (level < VPR.Levels.SwiftskinsSting)
                        return VPR.HuntersSting;

                    // A missing or expiring 10%/15% buff dwarfs the 60-potency positional. Swiftscaled first to preserve the opener.
                    var swiftscaled = FindEffect(VPR.Buffs.Swiftscaled);
                    var huntersInstinct = FindEffect(VPR.Buffs.HuntersInstinct);

                    if (swiftscaled is null || swiftscaled.RemainingTime <= 15)
                        return VPR.SwiftskinsSting;

                    if (huntersInstinct is null || huntersInstinct.RemainingTime <= 15)
                        return VPR.HuntersSting;

                    // A held venom wins even from the wrong sector (440 > 400), so keep its branch.
                    if (flanksbaneVenom is not null || flankstungVenom is not null)
                        return VPR.HuntersSting;

                    if (hindsbaneVenom is not null || hindstungVenom is not null)
                        return VPR.SwiftskinsSting;

                    // Omnidirectional target or True North: keep the buff-driven default.
                    if (!TargetHasPositionals() || HasEffect(ADV.Buffs.TrueNorth))
                        return VPR.SwiftskinsSting;

                    // Otherwise match the branch to our position. Front matches neither sector, so default rear.
                    return IsFlankingTarget() ? VPR.HuntersSting : VPR.SwiftskinsSting;
                }

                // Combo step 3, use whichever buff we have, or default to start hindsbane unless otherwise specified.
                // The branch (and hence positional) was fixed at step 2, so no position check belongs here.
                case VPR.HindstingStrike:
                    return VPR.SelectFinisher(true, flanksbaneVenom is not null, flankstungVenom is not null, hindsbaneVenom is not null, hindstungVenom is not null);

                // Combo step 3, flank. Use whichever buff we have, or default to Flanksbane if we're here and buff has fallen off.
                case VPR.FlankstingStrike:
                    return VPR.SelectFinisher(false, flanksbaneVenom is not null, flankstungVenom is not null, hindsbaneVenom is not null, hindstungVenom is not null);

                // Default return of actionID
                default:
                    return actionID;
            }
        }

        return actionID;
    }
}

/// <summary>
///     Projects the next positional onto HUD slots: each flank/rear slot morphs into its side's upcoming coil or finisher, dims while the other side is next.
/// </summary>
internal class ViperPositionals : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.VprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        bool isFlankSlot = actionID == VPR.HuntersCoil || actionID == VPR.FlankstingStrike || actionID == VPR.FlanksbaneFang;
        bool isRearSlot = actionID == VPR.SwiftskinsCoil || actionID == VPR.HindstingStrike || actionID == VPR.HindsbaneFang;

        if (!isFlankSlot && !isRearSlot)
            return actionID;

        var canUseSwiftskins = CanUseAction(VPR.SwiftskinsCoil);
        var canUseHunters = CanUseAction(VPR.HuntersCoil);

        var swiftscaled = FindEffect(VPR.Buffs.Swiftscaled);
        var huntersInstinct = FindEffect(VPR.Buffs.HuntersInstinct);

        bool hasFlanksbaneVenom = FindEffect(VPR.Buffs.FlanksbaneVenom) is not null;
        bool hasFlankstungVenom = FindEffect(VPR.Buffs.FlankstungVenom) is not null;
        bool hasHindsbaneVenom = FindEffect(VPR.Buffs.HindsbaneVenom) is not null;
        bool hasHindstungVenom = FindEffect(VPR.Buffs.HindstungVenom) is not null;

        uint upcoming = VPR.SelectCoil(
            canUseHunters,
            canUseSwiftskins,
            huntersInstinct is null,
            swiftscaled is null,
            huntersInstinct is not null && huntersInstinct.RemainingTime <= 15,
            swiftscaled is not null && swiftscaled.RemainingTime <= 15,
            hasFlanksbaneVenom || hasFlankstungVenom,
            hasHindsbaneVenom || hasHindstungVenom,
            !TargetHasPositionals() || HasEffect(ADV.Buffs.TrueNorth),
            IsFlankingTarget());

        if (upcoming == 0)
        {
            if (level < VPR.Levels.Single3rdCombo)
                return ADV.Swiftcast;

            switch (OriginalHook(VPR.SteelFangs))
            {
                case VPR.HindstingStrike:
                case VPR.HindsbaneFang:
                    upcoming = VPR.SelectFinisher(true, hasFlanksbaneVenom, hasFlankstungVenom, hasHindsbaneVenom, hasHindstungVenom);
                    break;
                case VPR.FlankstingStrike:
                case VPR.FlanksbaneFang:
                    upcoming = VPR.SelectFinisher(false, hasFlanksbaneVenom, hasFlankstungVenom, hasHindsbaneVenom, hasHindstungVenom);
                    break;
                default:
                {
                    bool flank;
                    if (hasFlanksbaneVenom || hasFlankstungVenom)
                        flank = true;
                    else if (hasHindsbaneVenom || hasHindstungVenom)
                        flank = false;
                    else
                        flank = IsFlankingTarget();

                    upcoming = VPR.SelectFinisher(!flank, hasFlanksbaneVenom, hasFlankstungVenom, hasHindsbaneVenom, hasHindstungVenom);
                    break;
                }
            }
        }

        bool upcomingFlank = upcoming == VPR.HuntersCoil || upcoming == VPR.FlankstingStrike || upcoming == VPR.FlanksbaneFang;

        return isFlankSlot == upcomingFlank ? upcoming : ADV.Swiftcast;
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
            {
                switch (level)
                {
                    case >= VPR.Levels.TwinsAoE when !IsOriginal(VPR.Twinfang):
                        if (CanUseAction(VPR.TwinfangThresh))
                        {
                            return HasEffect(VPR.Buffs.FellskinsVenom)
                                ? VPR.TwinbloodThresh
                                : VPR.TwinfangThresh;
                        }

                        if (CanUseAction(VPR.UncoiledTwinfang))
                        {
                            return HasEffect(VPR.Buffs.PoisedForTwinblood)
                                ? VPR.UncoiledTwinblood
                                : VPR.UncoiledTwinfang;
                        }

                        break;
                    case >= VPR.Levels.LastLash when !IsOriginal(VPR.SerpentsTail):
                        return OriginalHook(VPR.SerpentsTail);
                    case >= VPR.Levels.SerpentsIre when IsOffCooldown(VPR.SerpentsIre):
                        return VPR.SerpentsIre;
                }
            }

            var canUseSSC = CanUseAction(VPR.SwiftskinsDen);
            var canUseHunters = CanUseAction(VPR.HuntersDen);

            if (canUseSSC || canUseHunters)
            {
                if (
                    (
                        canUseHunters
                        && (
                            !HasEffect(VPR.Buffs.HuntersInstinct)
                            || HasEffect(VPR.Buffs.FlanksbaneVenom)
                            || HasEffect(VPR.Buffs.FlankstungVenom))) || !canUseSSC)
                    return VPR.HuntersDen;

                if (
                    canUseSSC
                    && (
                        HasEffect(VPR.Buffs.HindsbaneVenom)
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
                if (
                    (gauge.SerpentOffering >= 50 || readyToReawaken is not null)
                    && gauge.AnguineTribute < 1
                    && !canUseSSC
                    && !canUseHunters)
                    return VPR.Reawaken;

                if (gauge.RattlingCoilStacks >= 1)
                    return VPR.UncoiledFury;
            }

            if (
                level >= VPR.Levels.VicePit
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
            {
                switch (level)
                {
                    case >= VPR.Levels.TwinsAoE when !IsOriginal(VPR.Twinfang):
                        if (CanUseAction(VPR.TwinfangThresh))
                        {
                            return HasEffect(VPR.Buffs.FellskinsVenom)
                                ? VPR.TwinbloodThresh
                                : VPR.TwinfangThresh;
                        }

                        if (CanUseAction(VPR.UncoiledTwinfang))
                        {
                            return HasEffect(VPR.Buffs.PoisedForTwinblood)
                                ? VPR.UncoiledTwinblood
                                : VPR.UncoiledTwinfang;
                        }

                        break;
                    case >= VPR.Levels.LastLash when !IsOriginal(VPR.SerpentsTail):
                        return OriginalHook(VPR.SerpentsTail);
                    case >= VPR.Levels.SerpentsIre when IsOffCooldown(VPR.SerpentsIre):
                        return VPR.SerpentsIre;
                }
            }

            if (HasEffect(VPR.Buffs.Swiftscaled) && HasEffect(VPR.Buffs.HuntersInstinct))
            {
                if (gauge.RattlingCoilStacks >= 1)
                    return VPR.UncoiledFury;
            }
        }

        return actionID;
    }
}
