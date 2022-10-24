using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class AST
{
    public const byte JobID = 33;

    public const uint
        Draw = 3590,
        Redraw = 3593,
        Benefic = 3594,
        Malefic = 3596,
        EssentialDignity = 3614,
        Malefic2 = 3598,
        Helios = 3600,
        AspectedHelios = 3601,
        Ascend = 3603,
        Lightspeed = 3606,
        Benefic2 = 3610,
        Synastry = 3612,
        CollectiveUnconscious = 3613,
        Gravity = 3615,
        Balance = 4401,
        Bole = 4404,
        Arrow = 4402,
        Spear = 4403,
        Ewer = 4405,
        Spire = 4406,
        EarthlyStar = 7439,
        Malefic3 = 7442,
        MinorArcana = 7443,
        SleeveDraw = 7448,
        Divination = 16552,
        CelestialIntersection = 16556,
        CelestialOpposition = 16553,
        Combust3 = 16554,
        Malefic4 = 16555,
        Horoscope = 16557,
        NeutralSect = 16559,
        Play = 17055,
        CrownPlay = 25869,
        Astrodyne = 25870,
        FallMalefic = 25871,
        Gravity2 = 25872,
        Exaltation = 25873,
        Macrocosmos = 25874;

    public static class Buffs
    {
        public const ushort
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
        public const ushort
            CombustIII = 1881;
    }

    public static class Levels
    {
        public const byte
            Ascend = 12,
            Benefic2 = 26,
            Draw = 30,
            AspectedHelios = 42,
            Redraw = 40,
            Astrodyne = 50,
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
        //if (actionID == AST.AspectedHelios)
        //{
        //    if (level >= AST.Levels.Horoscope && IsOffCooldown(AST.Horoscope))
        //    {
        //        return AST.Horoscope;
        //    }

        //    return HasEffect(AST.Buffs.AspectedHelios) || level < AST.Levels.AspectedHelios ? AST.Helios : AST.AspectedHelios;
        //}

        if (actionID == AST.Malefic 
            || actionID == AST.Malefic2 
            || actionID == AST.Malefic3 
            || actionID == AST.Malefic4 
            || actionID == AST.FallMalefic)
        {
            var gauge = GetJobGauge<ASTGauge>();


            //if (level >= AST.Levels.EarthlyStar
            //    && IsOffCooldown(AST.EarthlyStar))
            //{
            //    return OriginalHook(AST.EarthlyStar);
            //}

            var tarPercentage = TargetOfTargetHPercentage();

            var playerPercentage = LocalPlayerPercentage();

            if (GCDClipCheck(actionID))
            {
                if (FindTargetOfTargetEffectAny(WAR.Buffs.Holmgang) is null)
                {
                    if (tarPercentage <= 0.75
                    && (IsOffCooldown(AST.EssentialDignity) 
                        || HasCharges(AST.EssentialDignity)))
                    {
                        return AST.EssentialDignity;
                    }

                    if (level >= AST.Levels.CelestialIntersection
                    && HasCharges(AST.CelestialIntersection)
                    && (GetRemainingCharges(AST.CelestialIntersection) >= 2
                        || tarPercentage <= 0.5
                        || GetCooldown(AST.CelestialIntersection).CooldownRemaining <= 5))
                    {
                        return AST.CelestialIntersection;
                    }
                }

                if (level >= AST.Levels.Astrodyne 
                    && IsOffCooldown(AST.Divination) 
                    && HasRaidBuffs()) return AST.Divination;

                if ((OriginalHook(AST.MinorArcana) != AST.MinorArcana)
                    && ((GetCooldown(AST.MinorArcana).CooldownRemaining <= 5 && gauge.DrawnCrownCard != CardType.NONE)
                        || (gauge.DrawnCrownCard == CardType.LADY && LocalPlayerPercentage() <= 0.85)))
                    return OriginalHook(AST.MinorArcana);

                if (OriginalHook(AST.EarthlyStar) != AST.EarthlyStar
                    && LocalPlayerPercentage() <= 0.85
                    && GetCooldown(AST.EarthlyStar).CooldownRemaining <= 50
                    )
                {
                    return OriginalHook(AST.EarthlyStar);
                }

                if (level >= AST.Levels.Astrodyne 
                    && CanUseAction(AST.Astrodyne)) 
                    return AST.Astrodyne;

                if (level >= AST.Levels.MinorArcana
                    && InCombat()
                    && IsOffCooldown(AST.MinorArcana) 
                    && gauge.DrawnCrownCard == CardType.NONE) 
                    return AST.MinorArcana;

                if (level >= AST.Levels.Draw 
                    && gauge.DrawnCard == CardType.NONE 
                    && HasCharges(AST.Draw))
                    return AST.Draw;

                if (IsOffCooldown(ADV.LucidDreaming) 
                    && LocalPlayer?.CurrentMp <= 8000)
                    return ADV.LucidDreaming;

                var seals = gauge.Seals;
                if (level >= AST.Levels.Redraw
                    && CanUseAction(AST.Redraw)
                    && seals != null)
                {

                    var drawnCard = gauge.DrawnCard;

                    switch (drawnCard)
                    {
                        case CardType.BALANCE:
                        case CardType.BOLE:
                            if (gauge.ContainsSeal(SealType.SUN)) return AST.Redraw;
                            break;
                        case CardType.EWER:
                        case CardType.ARROW:
                            if (gauge.ContainsSeal(SealType.MOON)) return AST.Redraw;
                            break;
                        case CardType.SPIRE:
                        case CardType.SPEAR:
                            if (gauge.ContainsSeal(SealType.CELESTIAL)) return AST.Redraw;
                            break;
                        default:
                            break;

                    }

                }
            }


            if (FindTargetEffect(AST.Debuffs.CombustIII)?.RemainingTime <= 4)
                return AST.Combust3;
        }

        return actionID;
    }
}

internal class AstrologianGravity : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == AST.Gravity || actionID == AST.Gravity2)
        {
            var gauge = GetJobGauge<ASTGauge>();

            if (GCDClipCheck(actionID))
            {

                if (level >= AST.Levels.Astrodyne
                    && CanUseAction(AST.Astrodyne))
                    return AST.Astrodyne;

                if (level >= AST.Levels.MinorArcana
                    && IsOffCooldown(AST.MinorArcana)
                    && gauge.DrawnCrownCard == CardType.NONE)
                    return AST.MinorArcana;

                if (level >= AST.Levels.Draw
                    && gauge.DrawnCard == CardType.NONE
                    && HasCharges(AST.Draw))
                    return AST.Draw;

                if (IsOffCooldown(ADV.LucidDreaming)
                    && LocalPlayer?.CurrentMp <= 8000)
                    return ADV.LucidDreaming;

                var seals = gauge.Seals;
                if (level >= AST.Levels.Redraw
                    && CanUseAction(AST.Redraw)
                    && seals != null)
                {

                    var drawnCard = gauge.DrawnCard;

                    switch (drawnCard)
                    {
                        case CardType.BALANCE:
                        case CardType.BOLE:
                            if (gauge.ContainsSeal(SealType.SUN)) return AST.Redraw;
                            break;
                        case CardType.EWER:
                        case CardType.ARROW:
                            if (gauge.ContainsSeal(SealType.MOON)) return AST.Redraw;
                            break;
                        case CardType.SPIRE:
                        case CardType.SPEAR:
                            if (gauge.ContainsSeal(SealType.CELESTIAL)) return AST.Redraw;
                            break;
                        default:
                            break;

                    }

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
            if (level >= AST.Levels.Exaltation
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

internal class AstrologianPlay : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == AST.Play)
        {
            var gauge = GetJobGauge<ASTGauge>();

            if (IsEnabled(CustomComboPreset.AstrologianPlayAstrodyneFeature))
            {
                if (level >= AST.Levels.Astrodyne && !gauge.ContainsSeal(SealType.NONE))
                    return AST.Astrodyne;
            }

            if (IsEnabled(CustomComboPreset.AstrologianPlayDrawFeature))
            {
                if (IsEnabled(CustomComboPreset.AstrologianPlayDrawAstrodyneFeature))
                {
                    var draw = GetCooldown(AST.Draw);

                    if (level >= AST.Levels.Astrodyne && !gauge.ContainsSeal(SealType.NONE) && draw.RemainingCharges == 0)
                        return AST.Astrodyne;
                }

                if (level >= AST.Levels.Draw && gauge.DrawnCard == CardType.NONE)
                    return AST.Draw;
            }
        }

        return actionID;
    }
}

internal class AstrologianDraw : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianDrawLockoutFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == AST.Draw)
        {
            var gauge = GetJobGauge<ASTGauge>();

            if (gauge.DrawnCard != CardType.NONE)
                // Malefic4
                return OriginalHook(AST.Malefic);
        }

        return actionID;
    }
}
internal class AstrologianBenefic2 : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianBeneficSyncFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == AST.Benefic2)
        {
            if (level < AST.Levels.Benefic2)
                return AST.Benefic;
        }

        return actionID;
    }
}