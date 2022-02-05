using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class AST
    {
        public const byte JobID = 33;

        public const uint
            Draw = 3590,
            Benefic = 3594,
            Malefic = 3596,
            Malefic2 = 3598,
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
            CelestialOpposition = 16553,
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
                LordOfCrownsDrawn = 2054,
                LadyOfCrownsDrawn = 2055;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Benefic2 = 26,
                Draw = 30,
                Astrodyne = 50,
                MinorArcana = 70,
                CrownPlay = 70;
        }
    }

    internal class AstrologianMalefic : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianMaleficDrawFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Malefic || actionID == AST.Malefic2 || actionID == AST.Malefic3 || actionID == AST.Malefic4 || actionID == AST.FallMalefic)
            {
                var gauge = GetJobGauge<ASTGauge>();

                if (level >= AST.Levels.Draw && gauge.DrawnCard == CardType.NONE && HasCharges(AST.Draw))
                    return AST.Draw;
            }

            return actionID;
        }
    }

    internal class AstrologianGravity : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianGravityDrawFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Gravity || actionID == AST.Gravity2)
            {
                var gauge = GetJobGauge<ASTGauge>();

                if (level >= AST.Levels.Draw && gauge.DrawnCard == CardType.NONE && HasCharges(AST.Draw))
                    return AST.Draw;
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

    internal class AstrologianMinorArcana : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianMinorArcanaCrownPlayFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.MinorArcana)
            {
                var gauge = GetJobGauge<ASTGauge>();

                if (level >= AST.Levels.CrownPlay && gauge.DrawnCrownCard != CardType.NONE)
                    // Card action
                    return OriginalHook(AST.CrownPlay);
            }

            return actionID;
        }
    }

    internal class AstrologianCrownPlay : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianCrownPlayMinorArcanaFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.CrownPlay)
            {
                var gauge = GetJobGauge<ASTGauge>();

                if (level >= AST.Levels.MinorArcana && gauge.DrawnCrownCard == CardType.NONE)
                    return AST.MinorArcana;
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
}
