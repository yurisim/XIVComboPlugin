using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class AST
{
    public const byte JobID = 33;

    public const uint Ascend = 3603,
        AspectedHelios = 3601,
        AstralDraw = 37017,
        Benefic = 3594,
        Benefic2 = 3610,
        CelestialIntersection = 16556,
        CelestialOpposition = 16553,
        CollectiveUnconscious = 3613,
        Combust = 3599,
        CombinedHelios = 37030,
        Divination = 16552,
        EarthlyStar = 7439,
        EssentialDignity = 3614,
        Exaltation = 25873,
        Gravity = 3615,
        Helios = 3600,
        Lightspeed = 3606,
        Macrocosmos = 25874,
        Malefic = 3596,
        MinorArcanaDT = 37022,
        NeutralSect = 16559,
        Play1 = 37019,
        Play2 = 37020,
        Play3 = 37021,
        Synastry = 3612,
        TheArrow = 37024,
        TheBole = 37027,
        TheEwer = 37028,
        TheSpire = 37025,
        TheBalance = 37023,
        TheSpear = 37026,
        UmbralDraw = 37018;

    public static class Buffs
    {
        public const ushort Divination = 1878,
            AspectedHelios = 836;
    }

    public static class Debuffs
    {
        public const ushort Combust = 838,
            Combust2 = 843,
            Combust3 = 1881;
    }

    public static class Levels
    {
        public const byte Ascend = 12,
            Benefic2 = 26,
            AstralDraw = 30,
            Redraw = 40,
            Astrodyne = 50,
            CelestialOpposition = 60,
            EarthlyStar = 62,
            MinorArcana = 70,
            CrownPlay = 70,
            CelestialIntersection = 74,
            Exaltation = 86,
            Horoscope = 76;
    }
}

internal class AstrologianMalefic : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == AST.Malefic)
        {
            var gauge = GetJobGauge<ASTGauge>();

            var tarPercentage = TargetOfTargetHPercentage();

            var threshold = 0.80;

            var myHP = LocalPlayerPercentage();

            var needToUseCards = GetCooldown(OriginalHook(AST.AstralDraw)).CooldownRemaining <= 15;

            if (GCDClipCheck(actionID))
            {
                if (
                    gauge.DrawnCrownCard == CardType.LADY
                    && level >= AST.Levels.MinorArcana
                    && (myHP <= 0.95 || needToUseCards)
                )
                {
                    return OriginalHook(AST.MinorArcanaDT);
                }

                if (FindTargetOfTargetEffectAny(WAR.Buffs.Holmgang) is null)
                {
                    if (
                        tarPercentage <= threshold - 0.2
                        && (IsOffCooldown(AST.EssentialDignity) || HasCharges(AST.EssentialDignity))
                    )
                    {
                        return AST.EssentialDignity;
                    }

                    if (
                        level >= AST.Levels.CelestialIntersection
                        && (
                            HasCharges(AST.CelestialIntersection)
                            || IsOffCooldown(AST.CelestialIntersection)
                        )
                        && (
                            (
                                GetRemainingCharges(AST.CelestialIntersection) >= 2
                                && tarPercentage <= threshold + 0.1
                            )
                            || tarPercentage <= threshold
                        )
                    )
                    {
                        return AST.CelestialIntersection;
                    }
                }

                if (
                    level >= AST.Levels.Astrodyne
                    && IsOffCooldown(AST.Divination)
                    && HasRaidBuffs()
                )
                {
                    return AST.Divination;
                }

                if (
                    gauge.DrawnCrownCard == CardType.LORD
                    && level >= AST.Levels.MinorArcana
                    && (HasRaidBuffs() || needToUseCards)
                    && InCombat()
                )
                {
                    return OriginalHook(AST.MinorArcanaDT);
                }

                if (
                    OriginalHook(AST.EarthlyStar) != AST.EarthlyStar
                    && LocalPlayerPercentage() <= 0.85
                    && GetCooldown(AST.EarthlyStar).CooldownRemaining <= 50
                )
                {
                    return OriginalHook(AST.EarthlyStar);
                }

                if (
                    level >= AST.Levels.AstralDraw
                    && IsOffCooldown(OriginalHook(AST.AstralDraw))
                    && IsOriginal(AST.Play1)
                    && IsOriginal(AST.Play2)
                    && IsOriginal(AST.Play3)
                )
                {
                    return OriginalHook(AST.AstralDraw);
                }

                if (IsOffCooldown(ADV.LucidDreaming) && LocalPlayer?.CurrentMp <= 8000)
                {
                    return ADV.LucidDreaming;
                }
            }

            if (InCombat() && TargetIsEnemy())
            {
                var combustEffects = new[]
                {
                    FindTargetEffect(AST.Debuffs.Combust),
                    FindTargetEffect(AST.Debuffs.Combust2),
                    FindTargetEffect(AST.Debuffs.Combust3)
                };

                if (!combustEffects.Any(effect => effect?.RemainingTime > 2.8))
                {
                    return OriginalHook(AST.Combust);
                }
            }
        }

        return actionID;
    }
}

internal class AstrologianGravity : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.AstrologianPlayDrawFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == AST.Gravity)
        {
            var gauge = GetJobGauge<ASTGauge>();

            var threshold = 0.80;

            var myHP = LocalPlayerPercentage();

            var tarPercentage = TargetOfTargetHPercentage();

            var needToUseCards = GetCooldown(OriginalHook(AST.AstralDraw)).CooldownRemaining <= 15;

            if (GCDClipCheck(actionID))
            {
                if (FindTargetOfTargetEffectAny(WAR.Buffs.Holmgang) is null)
                {
                    if (
                        gauge.DrawnCrownCard == CardType.LADY
                        && (myHP <= 0.95 || tarPercentage <= 0.75 || needToUseCards)
                    )
                    {
                        return OriginalHook(AST.MinorArcanaDT);
                    }

                    // Exaltation
                    if (
                        level >= AST.Levels.Exaltation
                        && IsOffCooldown(AST.Exaltation)
                        && tarPercentage <= threshold + 0.1
                    )
                    {
                        return AST.Exaltation;
                    }

                    if (
                        tarPercentage <= threshold
                        && (IsOffCooldown(AST.EssentialDignity) || HasCharges(AST.EssentialDignity))
                    )
                    {
                        return AST.EssentialDignity;
                    }

                    if (
                        level >= AST.Levels.CelestialIntersection
                        && (
                            HasCharges(AST.CelestialIntersection)
                            || IsOffCooldown(AST.CelestialIntersection)
                        )
                        && (
                            (
                                GetRemainingCharges(AST.CelestialIntersection) >= 2
                                && tarPercentage <= threshold + 0.1
                            )
                            || tarPercentage <= threshold
                        )
                    )
                    {
                        return AST.CelestialIntersection;
                    }

                    if (
                        gauge.DrawnCrownCard == CardType.LORD
                        && (HasRaidBuffs() || needToUseCards)
                        && InCombat()
                    )
                    {
                        return OriginalHook(AST.MinorArcanaDT);
                    }

                    if (
                        level >= AST.Levels.AstralDraw
                        && IsOriginal(AST.Play1)
                        && IsOriginal(AST.Play2)
                        && IsOriginal(AST.Play3)
                        && IsOffCooldown(OriginalHook(AST.AstralDraw))
                    )
                    {
                        return OriginalHook(AST.AstralDraw);
                    }
                }

                if (IsOffCooldown(ADV.LucidDreaming) && LocalPlayer?.CurrentMp <= 8000)
                    return ADV.LucidDreaming;

                if (
                    level >= AST.Levels.CelestialOpposition
                    && IsOffCooldown(OriginalHook(AST.CelestialOpposition))
                    && myHP <= threshold
                )
                {
                    return OriginalHook(AST.CelestialOpposition);
                }
            }
        }

        return actionID;
    }
}

internal class AstroCelestial : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == AST.CelestialIntersection)
        {
            if (
                level >= AST.Levels.Exaltation
                && IsOffCooldown(AST.Exaltation)
                && GetRemainingCharges(AST.CelestialIntersection) <= 1
            )
            {
                return AST.Exaltation;
            }

            if (level >= AST.Levels.Exaltation)
            {
                return CalcBestAction(actionID, AST.CelestialIntersection, AST.Exaltation);
            }
        }

        return actionID;
    }
}
