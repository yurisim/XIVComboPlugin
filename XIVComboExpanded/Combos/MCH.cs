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
        Drill = 16498,
        Bioblaster = 16499,
        AirAnchor = 16500,
        Chainsaw = 25788,
        Excavator = 36981,
        FullMetal = 36982;

    public static class Buffs
    {
        public const ushort
            HyperchargeReady = 3864,
            Overheated = 2688,
            ExcavatorReady = 3865,
            FullMetalPrepared = 3866,
            Reassemble = 851;
    }

    public static class Debuffs
    {
        public const ushort Wildfire = 861;
    }

    public static class Levels
    {
        public const byte SlugShot = 2,
            GaussRound = 15,
            CleanShot = 26,
            Hypercharge = 30,
            HeatBlast = 35,
            RookOverdrive = 40,
            Wildfire = 45,
            Ricochet = 50,
            AutoCrossbow = 52,
            HeatedSplitShot = 54,
            Drill = 58,
            HeatedSlugshot = 60,
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
        if (actionID == MCH.CleanShot || actionID == MCH.HeatedCleanShot)
        {
            var gauge = GetJobGauge<MCHGauge>();

            if (InCombat() && HasTarget())
            {
                var overheated = FindEffect(MCH.Buffs.Overheated);
                var excavatorReady = FindEffect(MCH.Buffs.ExcavatorReady);
                var fullMetal = FindEffect(MCH.Buffs.FullMetalPrepared);

                if (GCDClipCheck(actionID))
                {
                    var gaussRoundCharges = GetRemainingCharges(OriginalHook(MCH.GaussRound));
                    var ricochetCharges = GetRemainingCharges(OriginalHook(MCH.Ricochet));

                    var hyperchargeCDs = new[]
                    {
                level < MCH.Levels.Drill || GetCooldown(MCH.Drill).TotalCooldownRemaining >= 6,
                GetCooldown(OriginalHook(MCH.HotShot)).CooldownRemaining >= 6,
                level < MCH.Levels.Chainsaw || GetCooldown(MCH.Chainsaw).TotalCooldownRemaining >= 6
            };

                    var hyperchargeMe = hyperchargeCDs.Count(x => x is true) >= 2;

                    switch (level)
                    {
                        case >= MCH.Levels.Hypercharge when
                            IsOffCooldown(MCH.Hypercharge)
                            && (gauge.Heat >= 50 || HasEffect(MCH.Buffs.HyperchargeReady))
                            && (hyperchargeMe || TargetHasEffect(MCH.Debuffs.Wildfire))
                            && (gauge.Heat >= 90
                                || TargetHasEffect(MCH.Debuffs.Wildfire)
                                || HasRaidBuffs()
                                || FindEffect(MCH.Buffs.HyperchargeReady)?.RemainingTime <= 10):
                            return MCH.Hypercharge;

                        case >= MCH.Levels.Wildfire when
                            IsOffCooldown(MCH.Wildfire)
                            && overheated is not null
                            && hyperchargeMe
                            && (HasRaidBuffs() || GetCooldown(MCH.BarrelStabilizer).CooldownRemaining >= 100):
                            return MCH.Wildfire;

                        case >= MCH.Levels.BarrelStabilizer when
                            (HasRaidBuffs() || TargetHasEffect(MCH.Debuffs.Wildfire))
                            && IsOffCooldown(MCH.BarrelStabilizer):
                            return MCH.BarrelStabilizer;

                        case >= MCH.Levels.RookOverdrive when
                            CanUseAction(OriginalHook(MCH.RookAutoturret))
                            && (gauge.Battery >= 100
                                || HasRaidBuffs()
                                || (gauge.Battery >= 75
                                    && (IsOffCooldown(OriginalHook(MCH.HotShot))
                                        || (level >= MCH.Levels.Drill && HasCharges(MCH.Drill))
                                        || (level >= MCH.Levels.Chainsaw && IsOffCooldown(MCH.Chainsaw))
                                        || HasEffect(MCH.Buffs.ExcavatorReady)))):
                            return OriginalHook(MCH.RookAutoturret);

                        case >= MCH.Levels.Ricochet when
                            HasCharges(OriginalHook(MCH.Ricochet))
                            && (overheated is not null
                                || GetCooldown(OriginalHook(MCH.Ricochet)).TotalCooldownRemaining <= 35
                                || HasRaidBuffs())
                            && ricochetCharges >= gaussRoundCharges:
                        case >= MCH.Levels.GaussRound when
                            HasCharges(OriginalHook(MCH.GaussRound))
                            && (overheated is not null
                                || GetCooldown(OriginalHook(MCH.GaussRound)).TotalCooldownRemaining <= 35
                                || HasRaidBuffs()):
                            return new[] { OriginalHook(MCH.Ricochet), OriginalHook(MCH.GaussRound) }
                                .MinBy(action => GetCooldown(action).TotalCooldownRemaining);
                    }
                }

                if (overheated is null || fullMetal?.RemainingTime <= 5 || excavatorReady?.RemainingTime <= 5)
                {
                    if (level >= MCH.Levels.Drill
                        && (HasCharges(MCH.Drill) || IsOffCooldown(MCH.Drill))
                        && (GetCooldown(MCH.Drill).TotalCooldownRemaining <= 8 || HasRaidBuffs()))
                    {
                        if ((IsOffCooldown(MCH.Reassemble) || HasCharges(MCH.Reassemble))
                            && GCDClipCheck(actionID)
                            && !HasEffect(MCH.Buffs.Reassemble))
                        {
                            return MCH.Reassemble;
                        }

                        return MCH.Drill;
                    }

                    if (level >= MCH.Levels.FullMetal
                        && fullMetal is not null
                        && (HasRaidBuffs() || fullMetal.RemainingTime <= 15))
                    {
                        return MCH.FullMetal;
                    }

                    if (gauge.Battery <= 80)
                    {
                        if (level >= MCH.Levels.Chainsaw
                            && (IsOffCooldown(OriginalHook(MCH.Chainsaw)) || excavatorReady is not null)
                            && (excavatorReady is null
                                || excavatorReady.RemainingTime <= 15
                                || HasEffect(MCH.Buffs.Reassemble)
                                || HasRaidBuffs()))
                        {
                            if ((IsOffCooldown(MCH.Reassemble) || HasCharges(MCH.Reassemble))
                                && GCDClipCheck(actionID)
                                && !HasEffect(MCH.Buffs.Reassemble))
                            {
                                return MCH.Reassemble;
                            }

                            return OriginalHook(MCH.Chainsaw);
                        }

                        if (IsOffCooldown(OriginalHook(MCH.HotShot)))
                        {
                            if (IsOffCooldown(MCH.Reassemble)
                                && level < MCH.Levels.CleanShot
                                && GCDClipCheck(actionID))
                            {
                                return MCH.Reassemble;
                            }

                            return OriginalHook(MCH.HotShot);
                        }
                    }
                }

                if (overheated is not null && level >= MCH.Levels.HeatBlast)
                {
                    return OriginalHook(MCH.HeatBlast);
                }
            }

            if (comboTime > 0)
            {
                if (lastComboMove == MCH.SlugShot && level >= MCH.Levels.CleanShot)
                {
                    if (level < MCH.Levels.Drill
                        && GCDClipCheck(actionID)
                        && IsOffCooldown(MCH.Reassemble))
                    {
                        return MCH.Reassemble;
                    }

                    return OriginalHook(MCH.CleanShot);
                }

                if (lastComboMove == MCH.SplitShot && level >= MCH.Levels.SlugShot)
                {
                    return OriginalHook(MCH.SlugShot);
                }
            }

            return OriginalHook(MCH.SplitShot);
        }

        return actionID;

    }
}

internal class MachinistWildfire : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.MachinistHyperfireFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == MCH.Hypercharge)
        {
            if (level >= MCH.Levels.Wildfire && IsOffCooldown(MCH.Wildfire) && HasTarget())
                return MCH.Wildfire;

            if (
                level >= MCH.Levels.Wildfire
                && IsOnCooldown(MCH.Hypercharge)
                && !IsOriginal(MCH.Wildfire)
            )
                return MCH.Detonator;
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

            if (InCombat() && HasTarget())
                if (GCDClipCheck(actionID))
                {
                    var gaussRoundCharges = GetRemainingCharges(OriginalHook(MCH.GaussRound));
                    var ricochetCharges = GetRemainingCharges(OriginalHook(MCH.Ricochet));

                    switch (level)
                    {
                        case >= MCH.Levels.Hypercharge when IsOffCooldown(MCH.Hypercharge)
                                                            && (gauge.Heat >= 50 ||
                                                                HasEffect(MCH.Buffs.HyperchargeReady))
                                                            && (gauge.Heat >= 90
                                                                || HasRaidBuffs()):
                            return MCH.Hypercharge;

                        case >= MCH.Levels.RookOverdrive when HasTarget()
                                                              && CanUseAction(OriginalHook(MCH.RookAutoturret))
                                                              && (gauge.Battery >= 100
                                                                  || HasRaidBuffs()
                                                                  || (gauge.Battery >= 75
                                                                      && ((level < MCH.Levels.AirAnchor &&
                                                                           IsOffCooldown(MCH.HotShot))
                                                                          || (level >= MCH.Levels.AirAnchor &&
                                                                              IsOffCooldown(MCH.AirAnchor))
                                                                          || (level >= MCH.Levels.Chainsaw &&
                                                                              IsOffCooldown(
                                                                                  OriginalHook(MCH.Chainsaw)))))):
                            return OriginalHook(MCH.RookAutoturret);

                        case >= MCH.Levels.BarrelStabilizer when
                            (HasRaidBuffs() || TargetHasEffect(MCH.Debuffs.Wildfire))
                            && IsOffCooldown(MCH.BarrelStabilizer):
                            return MCH.BarrelStabilizer;
                        case >= MCH.Levels.Ricochet when HasCharges(OriginalHook(MCH.Ricochet))
                                                         && (overheated is not null
                                                             || GetCooldown(OriginalHook(MCH.Ricochet))
                                                                 .TotalCooldownRemaining <= 20
                                                             || HasRaidBuffs())
                                                         && ricochetCharges >= gaussRoundCharges:
                        case >= MCH.Levels.GaussRound when HasCharges(OriginalHook(MCH.GaussRound))
                                                           && (overheated is not null
                                                               || GetCooldown(OriginalHook(MCH.GaussRound))
                                                                   .TotalCooldownRemaining <= 20
                                                               || HasRaidBuffs()):
                            return new[] { OriginalHook(MCH.Ricochet), OriginalHook(MCH.GaussRound) }
                                .MinBy(actionID => GetCooldown(actionID).TotalCooldownRemaining);
                    }
                }

            if (
                level >= MCH.Levels.Bioblaster
                && HasCharges(MCH.Bioblaster)
                && GetCooldown(MCH.Bioblaster).TotalCooldownRemaining <= 5
            )
                return MCH.Bioblaster;

            if (level >= MCH.Levels.Chainsaw
                && IsOffCooldown(OriginalHook(MCH.Chainsaw))
                && gauge.Battery <= 80)
            {
                // Try to use Reassemble before drill if possible
                if (
                    (IsOffCooldown(MCH.Reassemble) || HasCharges(MCH.Reassemble))
                    && !HasEffect(MCH.Buffs.Reassemble)
                )
                    return MCH.Reassemble;

                return OriginalHook(MCH.Chainsaw);
            }

            var fullMetal = FindEffect(MCH.Buffs.FullMetalPrepared);
            if (level >= MCH.Levels.FullMetal
                && fullMetal is not null
                && (HasRaidBuffs() || fullMetal.RemainingTime <= 10))
                return MCH.FullMetal;

            if (overheated is not null && level >= MCH.Levels.HeatBlast)
                return level >= MCH.Levels.AutoCrossbow ? MCH.AutoCrossbow : MCH.HeatBlast;
        }

        return actionID;
    }
}