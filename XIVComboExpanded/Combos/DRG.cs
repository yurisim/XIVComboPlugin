using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class DRG
    {
        public const byte ClassID = 4;
        public const byte JobID = 22;

        public const uint
            // Single Target
            TrueThrust = 75,
            VorpalThrust = 78,
            Disembowel = 87,
            FullThrust = 84,
            ChaosThrust = 88,
            HeavensThrust = 25771,
            ChaoticSpring = 25772,
            WheelingThrust = 3556,
            FangAndClaw = 3554,
            RaidenThrust = 16479,
            // AoE
            DoomSpike = 86,
            SonicThrust = 7397,
            CoerthanTorment = 16477,
            DraconianFury = 25770,
            // Combined
            Geirskogul = 3555,
            Nastrond = 7400,
            // Jumps
            Jump = 92,
            SpineshatterDive = 95,
            DragonfireDive = 96,
            HighJump = 16478,
            MirageDive = 7399,
            // Dragon
            Stardiver = 16480,
            WyrmwindThrust = 25773;

        public static class Buffs
        {
            public const ushort
                SharperFangAndClaw = 802,
                EnhancedWheelingThrust = 803,
                DiveReady = 1243;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                VorpalThrust = 4,
                Disembowel = 18,
                FullThrust = 26,
                SpineshatterDive = 45,
                DragonfireDive = 50,
                ChaosThrust = 50,
                HeavensThrust = 86,
                ChaoticSpring = 86,
                FangAndClaw = 56,
                WheelingThrust = 58,
                Geirskogul = 60,
                SonicThrust = 62,
                MirageDive = 68,
                LifeOfTheDragon = 70,
                CoerthanTorment = 72,
                HighJump = 74,
                RaidenThrust = 76,
                Stardiver = 80;
        }
    }

    internal class DragoonJump : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DragoonJumpFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRG.Jump || actionID == DRG.HighJump)
            {
                if (level >= DRG.Levels.MirageDive && HasEffect(DRG.Buffs.DiveReady))
                    return DRG.MirageDive;
            }

            return actionID;
        }
    }

    internal class DragoonCoerthanTorment : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrgAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRG.CoerthanTorment)
            {
                var gauge = GetJobGauge<DRGGauge>();

                if (IsEnabled(CustomComboPreset.DragoonCoerthanWyrmwindFeature))
                {
                    if (gauge.FirstmindsFocusCount == 2)
                        return DRG.WyrmwindThrust;
                }

                if (IsEnabled(CustomComboPreset.DragoonCoerthanTormentCombo))
                {
                    if (comboTime > 0)
                    {
                        if (lastComboMove == DRG.SonicThrust && level >= DRG.Levels.CoerthanTorment)
                            return DRG.CoerthanTorment;

                        if ((lastComboMove == DRG.DoomSpike || lastComboMove == DRG.DraconianFury) && level >= DRG.Levels.SonicThrust)
                            return DRG.SonicThrust;
                    }

                    // Draconian Fury
                    return OriginalHook(DRG.DoomSpike);
                }
            }

            return actionID;
        }
    }

    internal class DragoonChaosThrust : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrgAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRG.ChaosThrust || actionID == DRG.ChaoticSpring)
            {
                if (IsEnabled(CustomComboPreset.DragoonFangThrustFeature))
                {
                    if (level >= DRG.Levels.FangAndClaw && (HasEffect(DRG.Buffs.SharperFangAndClaw) || HasEffect(DRG.Buffs.EnhancedWheelingThrust)))
                        return DRG.WheelingThrust;
                }

                if (IsEnabled(CustomComboPreset.DragoonChaosThrustCombo))
                {
                    if (level >= DRG.Levels.FangAndClaw && HasEffect(DRG.Buffs.SharperFangAndClaw))
                        return DRG.FangAndClaw;

                    if (level >= DRG.Levels.WheelingThrust && HasEffect(DRG.Buffs.EnhancedWheelingThrust))
                        return DRG.WheelingThrust;

                    if (comboTime > 0)
                    {
                        if (lastComboMove == DRG.Disembowel && level >= DRG.Levels.ChaosThrust)
                            // ChaoticSpring
                            return OriginalHook(DRG.ChaosThrust);

                        if ((lastComboMove == DRG.TrueThrust || lastComboMove == DRG.RaidenThrust) && level >= DRG.Levels.Disembowel)
                            return DRG.Disembowel;
                    }

                    if (IsEnabled(CustomComboPreset.DragoonChaosThrustComboOption))
                        return DRG.Disembowel;

                    // Vorpal Thrust
                    return OriginalHook(DRG.TrueThrust);
                }
            }

            return actionID;
        }
    }

    internal class DragoonFullThrust : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrgAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRG.FullThrust || actionID == DRG.HeavensThrust)
            {
                if (IsEnabled(CustomComboPreset.DragoonFangThrustFeature))
                {
                    if (level >= DRG.Levels.FangAndClaw && (HasEffect(DRG.Buffs.SharperFangAndClaw) || HasEffect(DRG.Buffs.EnhancedWheelingThrust)))
                        return DRG.FangAndClaw;
                }

                if (IsEnabled(CustomComboPreset.DragoonFullThrustCombo))
                {
                    if (level >= DRG.Levels.WheelingThrust && HasEffect(DRG.Buffs.EnhancedWheelingThrust))
                        return DRG.WheelingThrust;

                    if (level >= DRG.Levels.FangAndClaw && HasEffect(DRG.Buffs.SharperFangAndClaw))
                        return DRG.FangAndClaw;

                    if (comboTime > 0)
                    {
                        if (lastComboMove == DRG.VorpalThrust && level >= DRG.Levels.FullThrust)
                            // Heavens' Thrust
                            return OriginalHook(DRG.FullThrust);

                        if ((lastComboMove == DRG.TrueThrust || lastComboMove == DRG.RaidenThrust) && level >= DRG.Levels.VorpalThrust)
                            return DRG.VorpalThrust;
                    }

                    if (IsEnabled(CustomComboPreset.DragoonFullThrustComboOption))
                        return DRG.VorpalThrust;

                    // Vorpal Thrust
                    return OriginalHook(DRG.TrueThrust);
                }
            }

            return actionID;
        }
    }

    internal class DragoonStardiver : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrgAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRG.Stardiver)
            {
                var gauge = GetJobGauge<DRGGauge>();

                if (IsEnabled(CustomComboPreset.DragoonStardiverNastrondFeature))
                {
                    if (level >= DRG.Levels.Geirskogul && (!gauge.IsLOTDActive || IsOffCooldown(DRG.Nastrond) || IsOnCooldown(DRG.Stardiver)))
                        // Nastrond
                        return OriginalHook(DRG.Geirskogul);
                }

                if (IsEnabled(CustomComboPreset.DragoonStardiverDragonfireDiveFeature))
                {
                    if (level < DRG.Levels.Stardiver || !gauge.IsLOTDActive || IsOnCooldown(DRG.Stardiver) || (IsOffCooldown(DRG.DragonfireDive) && gauge.LOTDTimer > 7.5))
                        return DRG.DragonfireDive;
                }
            }

            return actionID;
        }
    }

    internal class DragoonDives : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DragoonDiveFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DRG.SpineshatterDive || actionID == DRG.DragonfireDive || actionID == DRG.Stardiver)
            {
                if (level >= DRG.Levels.Stardiver)
                {
                    var gauge = GetJobGauge<DRGGauge>();

                    if (gauge.IsLOTDActive)
                        return CalcBestAction(actionID, DRG.SpineshatterDive, DRG.DragonfireDive, DRG.Stardiver);

                    return CalcBestAction(actionID, DRG.SpineshatterDive, DRG.DragonfireDive);
                }

                if (level >= DRG.Levels.DragonfireDive)
                    return CalcBestAction(actionID, DRG.SpineshatterDive, DRG.DragonfireDive);

                if (level >= DRG.Levels.SpineshatterDive)
                    return DRG.SpineshatterDive;
            }

            return actionID;
        }
    }
}
