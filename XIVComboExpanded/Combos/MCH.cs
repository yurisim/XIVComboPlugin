using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class MCH
{
    public const byte JobID = 31;

    public const uint
        // Single target
        CleanShot = 2873,
        Reassemble = 2876,
        HeatedCleanShot = 7413,
        SplitShot = 2866,
        HeatedSplitShot = 7411,
        SlugShot = 2868,
        HeatedSlugshot = 7412,
        // Charges
        GaussRound = 2874,
        Ricochet = 2890,
        DoubleCheck = 36979,
        Checkmate = 36980,
        // AoE
        SpreadShot = 2870,
        Dismantle = 2887,
        AutoCrossbow = 16497,
        Scattergun = 25786,
        // Rook
        RookAutoturret = 2864,
        RookOverdrive = 7415,
        AutomatonQueen = 16501,
        QueenOverdrive = 16502,
        // Other
        Wildfire = 2878,
        Detonator = 16766,
        Hypercharge = 17209,
        BarrelStabilizer = 7414,
        HeatBlast = 7410,
        BlazingShot = 36978,
        HotShot = 2872,
        Tactician = 16889,
        Drill = 16498,
        Bioblaster = 16499,
        AirAnchor = 16500,
        Chainsaw = 25788,
        Excavator = 36981,
        FullMetal = 36982;

    public static class Buffs
    {
        public const ushort HyperchargeReady = 3864,
            Overheated = 2688,
            Tactician = 1951,
            ExcavatorReady = 3865,
            FullMetalPrepared = 3866,
            Reassemble = 851;
    }

    public static class Debuffs
    {
        public const ushort Wildfire = 861,
            Dismantle = 860;
    }

    public static class Levels
    {
        public const byte SlugShot = 2,
            Reassemble = 10,
            GaussRound = 15,
            CleanShot = 26,
            Hypercharge = 30,
            HeatBlast = 35,
            RookOverdrive = 40,
            Wildfire = 45,
            Ricochet = 50,
            AutoCrossbow = 52,
            HeatedSplitShot = 54,
            Tactician = 56,
            Drill = 58,
            HeatedSlugshot = 60,
            Dismantle = 62,
            HeatedCleanShot = 64,
            BarrelStabilizer = 66,
            Bioblaster = 72,
            BlazingShot = 68,
            ChargedActionMastery = 74,
            AirAnchor = 76,
            QueenOverdrive = 80,
            Chainsaw = 90,
            DoubleCheck = 92,
            CheckMate = 92,
            Excavator = 96,
            FullMetal = 100;
    }
}

internal class MachinistCleanShot : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MchAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID is MCH.SplitShot or MCH.HeatedSplitShot or MCH.SlugShot or MCH.HeatedSlugshot)
        {
            var gauge = GetJobGauge<MCHGauge>();

            var overheated = HasEffect(MCH.Buffs.Overheated);
            var raidbuffs = HasRaidBuffs(1);

            var excavatorReady = FindEffect(MCH.Buffs.ExcavatorReady);
            var fullMetal = FindEffect(MCH.Buffs.FullMetalPrepared);

            var drillReady =
                level >= MCH.Levels.Drill
                && (HasCharges(MCH.Drill) || IsOffCooldown(MCH.Drill))
                && !overheated
                && (GetCooldown(MCH.Drill).TotalCooldownRemaining <= 9 || raidbuffs);

            if (GCDClipCheck(actionID))
            {
                var timeThreshold = 9;

                var nothingBlockingHypercharge = new[]
                {
                    level < MCH.Levels.Drill
                        || GetCooldown(MCH.Drill).TotalCooldownRemaining >= timeThreshold,
                    GetCooldown(OriginalHook(MCH.HotShot)).TotalCooldownRemaining >= timeThreshold,
                    level < MCH.Levels.Chainsaw
                        || GetCooldown(MCH.Chainsaw).TotalCooldownRemaining >= timeThreshold,
                    fullMetal is null,
                    excavatorReady is null,
                    comboTime > 11 || comboTime == 0,
                }.All(x => x is true);

                var hyperchargeReady = HasEffect(MCH.Buffs.HyperchargeReady);

                var canUseHypercharge = gauge.Heat >= 50 || HasEffect(MCH.Buffs.HyperchargeReady);

                // this line is causing weird delays to occur with the icon replacer off GCD
                var dismantleCD = GetCooldown(MCH.Dismantle);
                var tacticianCD = GetCooldown(MCH.Tactician);

                var reprisal = FindTargetEffectAny(ADV.Debuffs.Reprisal);
                var reprisalFound = reprisal is not null && reprisal.RemainingTime >= 11;

                switch (level)
                {
                    case >= MCH.Levels.BarrelStabilizer
                        when raidbuffs
                            && actionID is MCH.SplitShot or MCH.HeatedSplitShot
                            && IsOffCooldown(MCH.BarrelStabilizer):
                        return MCH.BarrelStabilizer;
                    case >= MCH.Levels.Wildfire
                        when IsOffCooldown(MCH.Wildfire)
                            && overheated
                            && (
                                raidbuffs
                                || IsOnCooldown(MCH.BarrelStabilizer)
                                || level < MCH.Levels.BarrelStabilizer
                            ):
                        return MCH.Wildfire;
                    case >= MCH.Levels.RookOverdrive
                        when gauge.Battery >= 50
                            && actionID is MCH.SplitShot or MCH.HeatedSplitShot
                            && CanUseAction(OriginalHook(MCH.RookAutoturret))
                            && (gauge.Battery >= 80 || raidbuffs):
                        return OriginalHook(MCH.RookAutoturret);
                    case >= MCH.Levels.Reassemble
                        when (IsOffCooldown(MCH.Reassemble) || HasCharges(MCH.Reassemble))
                            && (
                                GetCooldown(MCH.Reassemble).TotalCooldownRemaining <= 20
                                || raidbuffs
                            )
                            && !HasEffect(MCH.Buffs.Reassemble)
                            && drillReady:
                        return MCH.Reassemble;
                    case >= MCH.Levels.Tactician
                        when IsOffCooldown(MCH.Tactician)
                            // && !TargetHasEffect(MCH.Debuffs.Dismantle)
                            && (dismantleCD.IsAvailable || dismantleCD.CooldownElapsed >= 10)
                            && reprisalFound:
                    case >= MCH.Levels.Hypercharge
                        when GetCooldown(MCH.Hypercharge).TotalCooldownRemaining <= 1
                            && !overheated
                            && canUseHypercharge
                            && actionID is MCH.SplitShot or MCH.HeatedSplitShot
                            && nothingBlockingHypercharge
                            && (
                                gauge.Heat >= 75
                                || raidbuffs
                                || TargetHasEffect(MCH.Debuffs.Wildfire)
                                || hyperchargeReady
                            ):
                        return MCH.Hypercharge;
                    case >= MCH.Levels.Ricochet
                        when HasCharges(OriginalHook(MCH.Ricochet))
                            && (
                                overheated
                                || GetCooldown(OriginalHook(MCH.Ricochet)).TotalCooldownRemaining
                                    <= 40
                                || raidbuffs
                            ):
                    case >= MCH.Levels.GaussRound
                        when HasCharges(OriginalHook(MCH.GaussRound))
                            && (
                                overheated
                                || GetCooldown(OriginalHook(MCH.GaussRound)).TotalCooldownRemaining
                                    <= 40
                                || raidbuffs
                            ):
                        return new[]
                        {
                            OriginalHook(MCH.Ricochet),
                            OriginalHook(MCH.GaussRound),
                        }.MinBy(action => GetCooldown(action).TotalCooldownRemaining);
                }
            }

            if (
                (!overheated && (comboTime > 4 || comboTime == 0))
                || HasEffect(MCH.Buffs.Reassemble)
            )
            {
                var shouldUseReassemble =
                    (IsOffCooldown(MCH.Reassemble) || HasCharges(MCH.Reassemble))
                    && (GetCooldown(MCH.Reassemble).TotalCooldownRemaining <= 20 || raidbuffs)
                    && GCDClipCheck(actionID)
                    && !HasEffect(MCH.Buffs.Reassemble);

                if (drillReady)
                {
                    return shouldUseReassemble ? MCH.Reassemble : MCH.Drill;
                }

                if (IsOffCooldown(OriginalHook(MCH.HotShot)))
                {
                    return shouldUseReassemble && level >= MCH.Levels.AirAnchor
                        ? MCH.Reassemble
                        : OriginalHook(MCH.HotShot);
                }

                var chainSawReady =
                    level >= MCH.Levels.Chainsaw
                    && (
                        GetCooldown(MCH.Chainsaw).IsAvailable || HasEffect(MCH.Buffs.ExcavatorReady)
                    );

                if (gauge.Battery <= 80 && chainSawReady)
                {
                    return shouldUseReassemble ? MCH.Reassemble : OriginalHook(MCH.Chainsaw);
                }

                if (
                    level >= MCH.Levels.FullMetal
                    && fullMetal is not null
                    && (
                        GetCooldown(MCH.BarrelStabilizer).CooldownElapsed >= 5
                        || gauge.IsRobotActive
                        || raidbuffs
                    )
                )
                {
                    return MCH.FullMetal;
                }
            }

            if (overheated && level >= MCH.Levels.HeatBlast)
            {
                return OriginalHook(MCH.HeatBlast);
            }

            if (comboTime > 0)
            {
                //  DO NOT USE ORIGINALHOOKS HERE IN THE LASTCOMBO MOVE
                if (lastComboMove is MCH.SplitShot && level >= MCH.Levels.SlugShot)
                {
                    return OriginalHook(MCH.SlugShot);
                }

                //  DO NOT USE ORIGINALHOOKS HERE IN THE LASTCOMBO MOVE
                if (lastComboMove is MCH.SlugShot && level >= MCH.Levels.CleanShot)
                {
                    if (
                        level < MCH.Levels.Drill
                        && GCDClipCheck(actionID)
                        && IsOffCooldown(MCH.Reassemble)
                    )
                    {
                        return MCH.Reassemble;
                    }

                    return OriginalHook(MCH.CleanShot);
                }
            }

            return OriginalHook(MCH.SplitShot);
        }

        return actionID;
    }
}

internal class MachinistSpreadShot : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MchAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MCH.SpreadShot || actionID == MCH.Scattergun)
        {
            var gauge = GetJobGauge<MCHGauge>();

            var overheated = FindEffect(MCH.Buffs.Overheated);

            var raidbuffs = HasRaidBuffs(2);

            if (InCombat() && HasTarget())
                if (GCDClipCheck(actionID))
                {
                    var gaussRoundCharges = GetRemainingCharges(OriginalHook(MCH.GaussRound));
                    var ricochetCharges = GetRemainingCharges(OriginalHook(MCH.Ricochet));

                    var timeThreshold = 8;

                    var hyperchargeCDs = new[]
                    {
                        level < MCH.Levels.Chainsaw
                            || GetCooldown(MCH.Chainsaw).TotalCooldownRemaining >= timeThreshold,
                    };

                    var noIncomingCDs = hyperchargeCDs.All(x => x is true);

                    var hyperchargeReady = FindEffect(MCH.Buffs.HyperchargeReady);

                    var hyperchargeElapsed = GetCooldown(MCH.Hypercharge).CooldownElapsed >= 0.5;

                    var canUseHypercharge =
                        gauge.Heat >= 50 || (hyperchargeReady is not null && hyperchargeElapsed);

                    var pleaseUseHypercharge = hyperchargeReady?.RemainingTime <= 15;

                    switch (level)
                    {
                        // case >= MCH.Levels.BarrelStabilizer
                        //     when raidbuffs && IsOffCooldown(MCH.BarrelStabilizer):
                        //     return MCH.BarrelStabilizer;

                        case >= MCH.Levels.Hypercharge
                            when IsOffCooldown(MCH.Hypercharge)
                                && canUseHypercharge
                                && (
                                    gauge.Heat >= 75
                                    || raidbuffs
                                    || TargetHasEffect(MCH.Debuffs.Wildfire)
                                    || (hyperchargeReady is not null && hyperchargeElapsed)
                                ):
                            return MCH.Hypercharge;

                        case >= MCH.Levels.RookOverdrive
                            when gauge.Battery >= 50
                                && CanUseAction(OriginalHook(MCH.RookAutoturret))
                                && (gauge.Battery >= 80 || raidbuffs):
                            return OriginalHook(MCH.RookAutoturret);

                        case >= MCH.Levels.Ricochet
                            when HasCharges(OriginalHook(MCH.Ricochet))
                                && (
                                    overheated is not null
                                    || GetCooldown(
                                        OriginalHook(MCH.Ricochet)
                                    ).TotalCooldownRemaining <= 35
                                    || raidbuffs
                                )
                                && ricochetCharges >= gaussRoundCharges:
                        case >= MCH.Levels.GaussRound
                            when HasCharges(OriginalHook(MCH.GaussRound))
                                && (
                                    overheated is not null
                                    || GetCooldown(
                                        OriginalHook(MCH.GaussRound)
                                    ).TotalCooldownRemaining <= 35
                                    || raidbuffs
                                ):
                            return new[]
                            {
                                OriginalHook(MCH.Ricochet),
                                OriginalHook(MCH.GaussRound),
                            }.MinBy(action => GetCooldown(action).TotalCooldownRemaining);
                    }
                }

            if (
                level >= MCH.Levels.Bioblaster
                && HasCharges(MCH.Bioblaster)
                && GetCooldown(MCH.Bioblaster).TotalCooldownRemaining <= 15
            )
                return MCH.Bioblaster;

            if (gauge.Battery <= 80 || HasEffect(MCH.Buffs.Reassemble))
            {
                var excavatorReady = FindEffect(MCH.Buffs.ExcavatorReady);

                var chainSawReady =
                    level >= MCH.Levels.Chainsaw
                    && (IsOffCooldown(OriginalHook(MCH.Chainsaw)) || excavatorReady is not null);

                if (chainSawReady)
                {
                    if (
                        (IsOffCooldown(MCH.Reassemble) || HasCharges(MCH.Reassemble))
                        && GCDClipCheck(actionID)
                        && !HasEffect(MCH.Buffs.Reassemble)
                    )
                    {
                        return MCH.Reassemble;
                    }

                    return OriginalHook(MCH.Chainsaw);
                }
            }

            var fullMetal = FindEffect(MCH.Buffs.FullMetalPrepared);

            if (
                level >= MCH.Levels.FullMetal
                && fullMetal is not null
                && (
                    GetCooldown(MCH.BarrelStabilizer).CooldownElapsed >= 5
                    || gauge.IsRobotActive
                    || raidbuffs
                )
            )
            {
                return MCH.FullMetal;
            }

            if (overheated is not null && level >= MCH.Levels.HeatBlast)
                return level >= MCH.Levels.AutoCrossbow ? MCH.AutoCrossbow : MCH.HeatBlast;
        }

        return actionID;
    }
}
