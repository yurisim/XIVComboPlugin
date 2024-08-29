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

            if (GCDClipCheck(actionID))
            {
                // Casts hypercharge if heat is over 50
                if (
                    level >= MCH.Levels.Wildfire
                    && InCombat()
                    && HasTarget()
                    && IsOffCooldown(MCH.Wildfire)
                    && gauge.IsOverheated
                )
                {
                    return MCH.Wildfire;
                }

                if (
                    level >= MCH.Levels.RookOverdrive
                    && HasTarget()
                    && gauge.Battery >= 50
                    && (
                        gauge.Battery >= 100
                        || HasRaidBuffs()
                        || (
                            gauge.Battery > 80
                            && (
                                (level < MCH.Levels.AirAnchor && IsOffCooldown(MCH.HotShot))
                                || (level >= MCH.Levels.AirAnchor && IsOffCooldown(MCH.AirAnchor))
                                || (level >= MCH.Levels.Chainsaw && IsOffCooldown(MCH.Chainsaw))
                            )
                        )
                    )
                )
                {
                    return OriginalHook(MCH.RookAutoturret);
                }

                // Casts hypercharge if heat is over 50
                if (
                    level >= MCH.Levels.BarrelStabilizer
                    && InCombat()
                    && IsOffCooldown(MCH.BarrelStabilizer)
                    && (HasRaidBuffs() || IsOffCooldown(MCH.Wildfire))
                )
                {
                    return MCH.BarrelStabilizer;
                }

                var gaussRoundCharges = GetRemainingCharges(OriginalHook(MCH.GaussRound));
                var ricochetCharges = GetRemainingCharges(OriginalHook(MCH.Ricochet));

                if (
                    level >= MCH.Levels.Ricochet
                    && HasCharges(MCH.Ricochet)
                    && HasTarget()
                    && ricochetCharges >= gaussRoundCharges
                )
                {
                    return OriginalHook(MCH.Ricochet);
                }
                else if (HasCharges(MCH.GaussRound))
                {
                    return OriginalHook(MCH.GaussRound);
                }

                // Casts hypercharge if heat is over 50
                if (
                    level >= MCH.Levels.Hypercharge
                    && InCombat()
                    && HasTarget()
                    && IsOffCooldown(MCH.Hypercharge)
                    && (gauge.Heat >= 50 || HasEffect(MCH.Buffs.HyperchargeReady))
                    && (gauge.Heat >= 90 || TargetHasEffect(MCH.Debuffs.Wildfire) || HasRaidBuffs())
                )
                {
                    return MCH.Hypercharge;
                }
            }

            if (
                level >= MCH.Levels.Drill
                && HasCharges(MCH.Drill)
                && (GetCooldown(MCH.Drill).TotalCooldownRemaining <= 5 || HasRaidBuffs())
            )
            {
                // Try to use Reassemble before drill if possible
                if (
                    (IsOffCooldown(MCH.Reassemble) || HasCharges(MCH.Reassemble))
                    && !HasEffect(MCH.Buffs.Reassemble)
                )
                    return MCH.Reassemble;

                return MCH.Drill;
            }

            if (
                level >= MCH.Levels.AirAnchor
                && IsOffCooldown(MCH.AirAnchor)
                && gauge.Battery <= 80
            )
            {
                if (
                    (IsOffCooldown(MCH.Reassemble) || HasCharges(MCH.Reassemble))
                    && !HasEffect(MCH.Buffs.Reassemble)
                )
                    return MCH.Reassemble;

                return MCH.AirAnchor;
            }

            if (level >= MCH.Levels.Chainsaw && IsOffCooldown(MCH.Chainsaw) && gauge.Battery <= 80)
            {
                // Try to use Reassemble before drill if possible
                if (
                    (IsOffCooldown(MCH.Reassemble) || HasCharges(MCH.Reassemble))
                    && !HasEffect(MCH.Buffs.Reassemble)
                )
                    return MCH.Reassemble;

                return MCH.Chainsaw;
            }

            // Hot shot only fires if battery is less than 80, Hot Shot gets upgraded to Air Anchor later
            if (level < MCH.Levels.AirAnchor && IsOffCooldown(MCH.HotShot) && gauge.Battery <= 80)
            {
                if (IsOffCooldown(MCH.Reassemble) && level < MCH.Levels.CleanShot)
                    return MCH.Reassemble;

                return MCH.HotShot;
            }

            if (gauge.IsOverheated && level >= MCH.Levels.HeatBlast)
                return OriginalHook(MCH.HeatBlast);

            if (comboTime > 0)
            {
                if (lastComboMove == MCH.SlugShot && level >= MCH.Levels.CleanShot)
                {
                    if (
                        level < MCH.Levels.Drill
                        && GCDClipCheck(actionID)
                        && IsOffCooldown(MCH.Reassemble)
                    )
                        return MCH.Reassemble;

                    // Heated
                    return OriginalHook(MCH.CleanShot);
                }

                if (lastComboMove == MCH.SplitShot && level >= MCH.Levels.SlugShot)
                    // Heated
                    return OriginalHook(MCH.SlugShot);
            }

            // Heated
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

            if (GCDClipCheck(actionID))
            {
                if (
                    level >= MCH.Levels.RookOverdrive
                    && HasTarget()
                    && gauge.Battery >= 50
                    && (gauge.Battery >= 100 || HasRaidBuffs())
                )
                {
                    return OriginalHook(MCH.RookAutoturret);
                }

                // Casts hypercharge if heat is over 50
                if (
                    level >= MCH.Levels.BarrelStabilizer
                    && IsOffCooldown(MCH.BarrelStabilizer)
                    && InCombat()
                    && gauge.Heat <= 50
                )
                {
                    return MCH.BarrelStabilizer;
                }

                var gaussRoundCharges = GetRemainingCharges(OriginalHook(MCH.GaussRound));
                var ricochetCharges = GetRemainingCharges(OriginalHook(MCH.Ricochet));

                if (
                    level >= MCH.Levels.Ricochet
                    && HasCharges(OriginalHook(MCH.Ricochet))
                    && HasTarget()
                    && ricochetCharges >= gaussRoundCharges
                )
                {
                    return OriginalHook(MCH.Ricochet);
                }
                else if (HasCharges(OriginalHook(MCH.GaussRound)))
                {
                    return OriginalHook(MCH.GaussRound);
                }

                // Casts hypercharge if heat is over 50
                if (
                    level >= MCH.Levels.Hypercharge
                    && HasTarget()
                    && IsOffCooldown(MCH.Hypercharge)
                    && gauge.Heat >= 50
                )
                {
                    return MCH.Hypercharge;
                }
            }

            if (
                level >= MCH.Levels.Bioblaster
                && HasCharges(MCH.Bioblaster)
                && (GetCooldown(MCH.Bioblaster).TotalCooldownRemaining <= 5)
            )
            {
                return MCH.Bioblaster;
            }

            if (level >= MCH.Levels.Chainsaw && IsOffCooldown(MCH.Chainsaw) && gauge.Battery <= 80)
            {
                // Try to use Reassemble before drill if possible
                if (
                    (IsOffCooldown(MCH.Reassemble) || HasCharges(MCH.Reassemble))
                    && !HasEffect(MCH.Buffs.Reassemble)
                )
                    return MCH.Reassemble;

                return MCH.Chainsaw;
            }

            if (gauge.IsOverheated && level >= MCH.Levels.HeatBlast)
            {
                return level >= MCH.Levels.AutoCrossbow ? MCH.AutoCrossbow : MCH.HeatBlast;
            }
        }

        return actionID;
    }
}
