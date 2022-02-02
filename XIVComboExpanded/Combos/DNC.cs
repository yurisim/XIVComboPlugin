using System;
using System.Linq;

using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class DNC
    {
        public const byte JobID = 38;

        public const uint
            // Single Target
            Cascade = 15989,
            Fountain = 15990,
            ReverseCascade = 15991,
            Fountainfall = 15992,
            // AoE
            Windmill = 15993,
            Bladeshower = 15994,
            RisingWindmill = 15995,
            Bloodshower = 15996,
            // Dancing
            StandardStep = 15997,
            TechnicalStep = 15998,
            Tillana = 25790,
            // Fans
            FanDance1 = 16007,
            FanDance2 = 16008,
            FanDance3 = 16009,
            FanDance4 = 25791,
            // Other
            SaberDance = 16005,
            EnAvant = 16010,
            Devilment = 16011,
            Flourish = 16013,
            Improvisation = 16014,
            StarfallDance = 25792;

        public static class Buffs
        {
            public const ushort
                FlourishingSymmetry = 2693,
                FlourishingFlow = 2694,
                FlourishingFinish = 2698,
                FlourishingStarfall = 2700,
                StandardStep = 1818,
                TechnicalStep = 1819,
                ThreefoldFanDance = 1820,
                FourfoldFanDance = 2699;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Cascade = 1,
                Fountain = 2,
                Windmill = 15,
                StandardStep = 15,
                ReverseCascade = 20,
                Bladeshower = 25,
                RisingWindmill = 35,
                Fountainfall = 40,
                Bloodshower = 45,
                FanDance3 = 66,
                TechnicalStep = 70,
                Flourish = 72,
                Tillana = 82,
                FanDance4 = 86,
                StarfallDance = 90;
        }
    }

    internal class DancerDanceComboCompatibility : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerDanceComboCompatibility;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var actionIDs = Service.Configuration.DancerDanceCompatActionIDs;

            if (actionIDs.Contains(actionID))
            {
                var gauge = GetJobGauge<DNCGauge>();

                if (level >= DNC.Levels.StandardStep && gauge.IsDancing)
                {
                    if (actionID == actionIDs[0] || (actionIDs[0] == 0 && actionID == DNC.Cascade))
                        return OriginalHook(DNC.Cascade);

                    if (actionID == actionIDs[1] || (actionIDs[1] == 0 && actionID == DNC.Flourish))
                        return OriginalHook(DNC.Fountain);

                    if (actionID == actionIDs[2] || (actionIDs[2] == 0 && actionID == DNC.FanDance1))
                        return OriginalHook(DNC.ReverseCascade);

                    if (actionID == actionIDs[3] || (actionIDs[3] == 0 && actionID == DNC.FanDance2))
                        return OriginalHook(DNC.Fountainfall);
                }
            }

            return actionID;
        }
    }

    internal class DancerFanDance12 : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DncAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.FanDance1 || actionID == DNC.FanDance2)
            {
                var gauge = GetJobGauge<DNCGauge>();

                if (IsEnabled(CustomComboPreset.DancerFanDance3Feature))
                {
                    if (IsEnabled(CustomComboPreset.DancerFanDance4Feature))
                    {
                        if (gauge.Feathers == 4)
                        {
                            if (level >= DNC.Levels.FanDance3 && HasEffect(DNC.Buffs.ThreefoldFanDance))
                                return DNC.FanDance3;

                            return actionID;
                        }

                        if (level >= DNC.Levels.FanDance4 && HasEffect(DNC.Buffs.FourfoldFanDance))
                            return DNC.FanDance4;
                    }

                    if (level >= DNC.Levels.FanDance3 && HasEffect(DNC.Buffs.ThreefoldFanDance))
                        return DNC.FanDance3;
                }
            }

            return actionID;
        }
    }

    internal class DancerStandardStepTechnicalStep : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerDanceStepCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.StandardStep)
            {
                var gauge = GetJobGauge<DNCGauge>();

                if (level >= DNC.Levels.StandardStep && gauge.IsDancing && HasEffect(DNC.Buffs.StandardStep))
                {
                    if (gauge.CompletedSteps < 2)
                        return gauge.NextStep;

                    return OriginalHook(DNC.StandardStep);
                }

                return DNC.StandardStep;
            }

            if (actionID == DNC.TechnicalStep)
            {
                var gauge = GetJobGauge<DNCGauge>();

                if (level >= DNC.Levels.TechnicalStep && gauge.IsDancing && HasEffect(DNC.Buffs.TechnicalStep))
                {
                    if (gauge.CompletedSteps < 4)
                        return gauge.NextStep;
                }

                // Tillana
                return OriginalHook(DNC.TechnicalStep);
            }

            return actionID;
        }
    }

    internal class DancerFlourish : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DncAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.Flourish)
            {
                if (IsEnabled(CustomComboPreset.DancerFlourishFan3Feature))
                {
                    if (level >= DNC.Levels.FanDance3 && HasEffect(DNC.Buffs.ThreefoldFanDance))
                        return DNC.FanDance3;
                }

                if (IsEnabled(CustomComboPreset.DancerFlourishFan4Feature))
                {
                    if (level >= DNC.Levels.FanDance4 && HasEffect(DNC.Buffs.FourfoldFanDance))
                        return DNC.FanDance4;
                }

                if (IsEnabled(CustomComboPreset.DancerFlourishFeature))
                {
                    if (level >= DNC.Levels.Flourish && IsOffCooldown(DNC.Flourish))
                    {
                        if (level >= DNC.Levels.Fountainfall && HasEffect(DNC.Buffs.FlourishingFlow))
                            return DNC.Fountainfall;

                        if (level >= DNC.Levels.FanDance4 && HasEffect(DNC.Buffs.FourfoldFanDance))
                            return DNC.FanDance4;

                        if (level >= DNC.Levels.ReverseCascade && HasEffect(DNC.Buffs.FlourishingSymmetry))
                            return DNC.ReverseCascade;

                        if (level >= DNC.Levels.FanDance3 && HasEffect(DNC.Buffs.ThreefoldFanDance))
                            return DNC.FanDance3;
                    }
                }
            }

            return actionID;
        }
    }

    internal class DancerCascadeFountain : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DncAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.Cascade)
            {
                if (IsEnabled(CustomComboPreset.DancerSingleTargetMultibutton))
                {
                    if (level >= DNC.Levels.Fountainfall && HasEffect(DNC.Buffs.FlourishingFlow))
                        return DNC.Fountainfall;

                    if (level >= DNC.Levels.ReverseCascade && HasEffect(DNC.Buffs.FlourishingSymmetry))
                        return DNC.ReverseCascade;

                    if (lastComboMove == DNC.Cascade && level >= DNC.Levels.Fountain)
                        return DNC.Fountain;
                }

                if (IsEnabled(CustomComboPreset.DancerSingleTargetProcs))
                {
                    if (level >= DNC.Levels.ReverseCascade && HasEffect(DNC.Buffs.FlourishingSymmetry))
                        return DNC.ReverseCascade;
                }
            }

            if (actionID == DNC.Fountain)
            {
                if (IsEnabled(CustomComboPreset.DancerSingleTargetProcs))
                {
                    if (level >= DNC.Levels.Fountainfall && HasEffect(DNC.Buffs.FlourishingFlow))
                        return DNC.Fountainfall;
                }
            }

            return actionID;
        }
    }

    internal class DancerWindmillBladeshower : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DncAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.Windmill)
            {
                if (IsEnabled(CustomComboPreset.DancerAoeMultibutton))
                {
                    if (level >= DNC.Levels.Bloodshower && HasEffect(DNC.Buffs.FlourishingFlow))
                        return DNC.Bloodshower;

                    if (level >= DNC.Levels.RisingWindmill && HasEffect(DNC.Buffs.FlourishingSymmetry))
                        return DNC.RisingWindmill;

                    if (lastComboMove == DNC.Windmill && level >= DNC.Levels.Bladeshower)
                        return DNC.Bladeshower;
                }

                if (IsEnabled(CustomComboPreset.DancerAoeProcs))
                {
                    if (level >= DNC.Levels.RisingWindmill && HasEffect(DNC.Buffs.FlourishingSymmetry))
                        return DNC.RisingWindmill;
                }
            }

            if (actionID == DNC.Bladeshower)
            {
                if (IsEnabled(CustomComboPreset.DancerAoeProcs))
                {
                    if (level >= DNC.Levels.Bloodshower && HasEffect(DNC.Buffs.FlourishingFlow))
                        return DNC.Bloodshower;
                }
            }

            return actionID;
        }
    }

    internal class DancerDevilment : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerDevilmentFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.Devilment)
            {
                if (level >= DNC.Levels.StarfallDance && HasEffect(DNC.Buffs.FlourishingStarfall))
                    return DNC.StarfallDance;
            }

            return actionID;
        }
    }
}
