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
        UmbralDraw = 37018;

    public static class Buffs
    {
        public const ushort ClarifyingDraw = 2713,
            Divination = 1878,
            AspectedHelios = 836,
            BalanceDrawn = 913,
            BoleDrawn = 914,
            ArrowDrawn = 915,
            SpearDrawn = 916,
            EwerDrawn = 917,
            SpireDawm = 918,
            LordOfCrownsDrawn = 2054,
            LadyOfCrownsDrawn = 2055;
    }

    public static class Debuffs
    {
        public const ushort Combust = 838,
            Combust2 = 838,
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

            var needToUseCards = GetCooldown(OriginalHook(AST.AstralDraw)).CooldownRemaining <= 10;

            if (GCDClipCheck(actionID))
            {
                if (FindTargetOfTargetEffectAny(WAR.Buffs.Holmgang) is null)
                {
                    if (
                        tarPercentage <= threshold - 0.1
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
                    level >= AST.Levels.CelestialOpposition
                    && IsOffCooldown(OriginalHook(AST.CelestialOpposition))
                    && myHP <= threshold
                )
                {
                    return OriginalHook(AST.CelestialOpposition);
                }

                if (
                    level >= AST.Levels.Astrodyne
                    && IsOffCooldown(AST.Divination)
                    && HasRaidBuffs()
                )
                {
                    return AST.Divination;
                }

                // if (
                //     (OriginalHook(AST.Buffs.LordOfCrownsDrawn) != AST.MinorArcana)
                //     && (
                //         (
                //             GetCooldown(AST.MinorArcana).CooldownRemaining <= 5
                //             && gauge.DrawnCrownCard != CardType.NONE
                //         )
                //         || (
                //             gauge.DrawnCrownCard == CardType.LADY && LocalPlayerPercentage() <= 0.85
                //         )
                //     )
                // )
                //     return OriginalHook(AST.MinorArcana);


                if (level >= AST.Levels.AstralDraw)
                {
                    if (
                        gauge.DrawnCrownCard == CardType.LORD
                        && level >= AST.Levels.MinorArcana
                        && (HasRaidBuffs() || needToUseCards)
                    )
                    {
                        return OriginalHook(AST.MinorArcanaDT);
                    }
                }

                if (level >= AST.Levels.AstralDraw && IsOffCooldown(OriginalHook(AST.AstralDraw)))
                {
                    return OriginalHook(AST.AstralDraw);
                }

                if (
                    OriginalHook(AST.EarthlyStar) != AST.EarthlyStar
                    && LocalPlayerPercentage() <= 0.85
                    && GetCooldown(AST.EarthlyStar).CooldownRemaining <= 50
                )
                {
                    return OriginalHook(AST.EarthlyStar);
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

//internal class AstrologianGravity : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianPlayDrawFeature;

//    // private Dictionary<CardType, SealType> CardSeals { get; } =
//    //     new Dictionary<CardType, SealType>
//    //     {
//    //         { CardType.BALANCE, SealType.SUN },
//    //         { CardType.BOLE, SealType.SUN },
//    //         { CardType.ARROW, SealType.MOON },
//    //         { CardType.EWER, SealType.MOON },
//    //         { CardType.SPEAR, SealType.CELESTIAL },
//    //         { CardType.SPIRE, SealType.CELESTIAL },
//    //     };

//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == AST.Gravity || actionID == AST.Gravity2)
//        {
//            var gauge = GetJobGauge<ASTGauge>();

//            var threshold = 0.80;

//            var myHP = LocalPlayerPercentage();

//            var tarPercentage = TargetOfTargetHPercentage();


//            if (GCDClipCheck(actionID))
//            {
//                if (FindTargetOfTargetEffectAny(WAR.Buffs.Holmgang) is null)
//                {
//                    // Exaltation
//                    if (
//                        level >= AST.Levels.Exaltation
//                        && IsOffCooldown(AST.Exaltation)
//                        && tarPercentage <= threshold + 0.1
//                    )
//                    {
//                        return AST.Exaltation;
//                    }

//                    if (
//                        tarPercentage <= threshold
//                        && (IsOffCooldown(AST.EssentialDignity) || HasCharges(AST.EssentialDignity))
//                    )
//                    {
//                        return AST.EssentialDignity;
//                    }

//                    if (
//                        level >= AST.Levels.CelestialIntersection
//                        && (
//                            HasCharges(AST.CelestialIntersection)
//                            || IsOffCooldown(AST.CelestialIntersection)
//                        )
//                        && (
//                            (
//                                GetRemainingCharges(AST.CelestialIntersection) >= 2
//                                && tarPercentage <= threshold + 0.1
//                            )
//                            || tarPercentage <= threshold
//                        )
//                    )
//                    {
//                        return AST.CelestialIntersection;
//                    }
//                }


//                if (level >= AST.Levels.Astrodyne && CanUseAction(AST.Astrodyne))
//                    return AST.Astrodyne;

//                if (
//                    level >= AST.Levels.MinorArcana
//                    && IsOffCooldown(AST.MinorArcana)
//                    && (
//                        gauge.DrawnCrownCard == CardType.NONE
//                        || gauge.DrawnCrownCard == CardType.LORD
//                    )
//                )
//                    return AST.MinorArcana;

//                if (
//                    level >= AST.Levels.Draw
//                    && gauge.DrawnCard == CardType.NONE
//                    && HasCharges(AST.Draw)
//                )
//                    return AST.Draw;

//                if (IsOffCooldown(ADV.LucidDreaming) && LocalPlayer?.CurrentMp <= 8000)
//                    return ADV.LucidDreaming;

//                if (
//                    level >= AST.Levels.CelestialOpposition
//                    && IsOffCooldown(OriginalHook(AST.CelestialOpposition))
//                    && myHP <= threshold
//                )
//                {
//                    return OriginalHook(AST.CelestialOpposition);
//                }

//                var seals = gauge.Seals;
//                if (level >= AST.Levels.Redraw && CanUseAction(AST.Redraw) && seals != null)
//                {
//                    var drawnCard = gauge.DrawnCard;

//                    switch (drawnCard)
//                    {
//                        case CardType.BALANCE:
//                        case CardType.BOLE:
//                            if (gauge.ContainsSeal(SealType.SUN))
//                                return AST.Redraw;
//                            break;
//                        case CardType.EWER:
//                        case CardType.ARROW:
//                            if (gauge.ContainsSeal(SealType.MOON))
//                                return AST.Redraw;
//                            break;
//                        case CardType.SPIRE:
//                        case CardType.SPEAR:
//                            if (gauge.ContainsSeal(SealType.CELESTIAL))
//                                return AST.Redraw;
//                            break;
//                        default:
//                            break;
//                    }
//                }
//            }
//        }

//        return actionID;
//    }
//}

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

            return CalcBestAction(actionID, AST.Exaltation, AST.CelestialIntersection);
        }

        return actionID;
    }
}


//internal class AstrologianArcana : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } =
//        CustomComboPreset.AstrologianBeneficSyncFeature;

//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        var gauge = GetJobGauge<ASTGauge>();

//        if (actionID == AST.Helios)
//        {
//            if (IsEnabled(CustomComboPreset.AstrologianHeliosArcanaFeature) && gauge.DrawnCrownCard == CardType.LADY && level >= AST.Levels.MinorArcana)
//                return OriginalHook(AST.MinorArcanaDT);
//        }

//        return actionID;
//    }
//}
