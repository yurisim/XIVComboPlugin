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
        Oracle = 37029,
        SunSign = 37031,
        UmbralDraw = 37018;

    public static class Buffs
    {
        public const ushort Divination = 1878,
            TheBalance = 3887,
            Divining = 3893,
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
            EssentialDignity = 15,
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
            Oracle = 92,
            SunSign = 100,
            Horoscope = 76;
    }
}

internal class AstrologianMalefic : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID is AST.Malefic or AST.Gravity)
        {
            var gauge = GetJobGauge<ASTGauge>();

            var tankPercentage = TargetOfTargetHPercentage();

            var threshold = 0.80;

            var localPlayer = LocalPlayerPercentage();

            var needToUseCards = GetCooldown(OriginalHook(AST.AstralDraw)).CooldownRemaining <= 15;

            var noTankCDs = FindTargetOfTargetEffectAny(WAR.Buffs.Holmgang) is null;

            var divining = FindEffect(AST.Buffs.Divining);

            var raidbuffs = HasRaidBuffs(2);

            if (GCDClipCheck(actionID))
            {
                switch (level)
                {
                    case >= AST.Levels.MinorArcana
                        when gauge.DrawnCrownCard == CardType.LADY
                            && (
                                localPlayer <= 0.95
                                || needToUseCards
                                || (actionID is AST.Gravity && tankPercentage <= 0.75 && noTankCDs)
                            ):
                        return OriginalHook(AST.MinorArcanaDT);
                    case >= AST.Levels.SunSign when CanUseAction(AST.SunSign):
                        return AST.SunSign;

                    case >= AST.Levels.Oracle
                        when divining is not null
                            && (
                                divining.RemainingTime <= 15 || raidbuffs || actionID is AST.Gravity
                            ):
                        return AST.Oracle;

                    case >= AST.Levels.CelestialOpposition
                        when IsOffCooldown(OriginalHook(AST.CelestialOpposition))
                            && OriginalHook(AST.EarthlyStar) is AST.EarthlyStar
                            && (
                                (localPlayer <= threshold)
                                || (
                                    actionID is AST.Gravity
                                    && TargetOfTargetHPercentage() <= threshold
                                )
                            ):
                        return OriginalHook(AST.CelestialOpposition);

                    case >= AST.Levels.EarthlyStar
                        when OriginalHook(AST.EarthlyStar) != AST.EarthlyStar
                            && (
                                localPlayer <= 0.85
                                || (
                                    actionID is AST.Gravity
                                    && TargetOfTargetHPercentage() <= threshold
                                )
                            )
                            && GetCooldown(AST.EarthlyStar).CooldownRemaining <= 50:
                        return OriginalHook(AST.EarthlyStar);

                    case >= AST.Levels.EssentialDignity
                        when noTankCDs
                            && (
                                HasCharges(AST.EssentialDignity)
                                || IsOffCooldown(AST.EssentialDignity)
                            )
                            && (
                                (
                                    GetCooldown(
                                        OriginalHook(AST.EssentialDignity)
                                    ).TotalCooldownRemaining <= 15
                                    && tankPercentage <= threshold - 0.15
                                )
                                || tankPercentage <= threshold - 0.2
                            ):
                        return AST.EssentialDignity;

                    case >= AST.Levels.Exaltation
                        when noTankCDs
                            && IsOffCooldown(AST.Exaltation)
                            && tankPercentage <= threshold - 0.1:
                        return AST.Exaltation;

                    case >= AST.Levels.CelestialIntersection
                        when noTankCDs
                            && (
                                HasCharges(AST.CelestialIntersection)
                                || IsOffCooldown(AST.CelestialIntersection)
                            )
                            && (
                                (
                                    GetCooldown(
                                        OriginalHook(AST.CelestialIntersection)
                                    ).TotalCooldownRemaining <= 10
                                    && tankPercentage <= threshold
                                )
                                || tankPercentage <= threshold - 0.2
                            ):
                        return AST.CelestialIntersection;

                    case >= AST.Levels.Astrodyne
                        when IsOffCooldown(AST.Divination) && HasRaidBuffs(2):
                        return AST.Divination;

                    case >= AST.Levels.MinorArcana
                        when gauge.DrawnCrownCard == CardType.LORD
                            && (HasRaidBuffs(2) || needToUseCards || actionID is AST.Gravity)
                            && InCombat():
                        return OriginalHook(AST.MinorArcanaDT);

                    case >= AST.Levels.AstralDraw
                        when IsOffCooldown(OriginalHook(AST.AstralDraw))
                            && IsOriginal(AST.Play1)
                            && IsOriginal(AST.Play2)
                            && IsOriginal(AST.Play3):
                        return OriginalHook(AST.AstralDraw);

                    case >= ADV.Levels.LucidDreaming
                        when IsOffCooldown(ADV.LucidDreaming) && LocalPlayer?.CurrentMp <= 7500:
                        return ADV.LucidDreaming;
                }
            }

            if (InCombat() && TargetIsEnemy() && ShouldUseDots() && actionID is not AST.Gravity)
            {
                var debuffs = new[]
                {
                    FindTargetEffect(AST.Debuffs.Combust),
                    FindTargetEffect(AST.Debuffs.Combust2),
                    FindTargetEffect(AST.Debuffs.Combust3),
                };

                if (
                    debuffs.All(x =>
                        x is null || x.RemainingTime <= 3 || x.RemainingTime <= 6 && IsMoving
                    )
                )
                    return OriginalHook(AST.Combust);
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
                return AST.Exaltation;

            if (level >= AST.Levels.Exaltation)
                return CalcBestAction(actionID, AST.CelestialIntersection, AST.Exaltation);
        }

        return actionID;
    }
}
