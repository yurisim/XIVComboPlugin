using System;
using System.Linq;

using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

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
            FlourishingSymmetry = 3017,
            FlourishingFlow = 3018,
            FlourishingFinish = 2698,
            FlourishingStarfall = 2700,
            SilkenSymmetry = 2693,
            SilkenFlow = 2694,
            StandardStep = 1818,
            TechnicalStep = 1819,
            TechnicalFinish = 1822,
            ThreefoldFanDance = 1820,
            Devilment = 1825,
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
            Devilment = 62,
            FanDance3 = 66,
            TechnicalStep = 70,
            Flourish = 72,
            SaberDance = 76,
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

internal class DancerCascadeFountain : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DncAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DNC.Cascade || actionID == DNC.Windmill)
        {
            var gauge = GetJobGauge<DNCGauge>();

            if (level >= DNC.Levels.StandardStep && gauge.IsDancing && HasEffect(DNC.Buffs.StandardStep))
            {
                if (gauge.CompletedSteps < 2)
                    return gauge.NextStep;

                return OriginalHook(DNC.StandardStep);
            }

            if (level >= DNC.Levels.Devilment
                    && IsOffCooldown(DNC.Devilment)
                    && (HasEffect(DNC.Buffs.TechnicalFinish) || level < DNC.Levels.TechnicalStep))
                return DNC.Devilment;

            if (GCDClipCheck(actionID))
            {

                if (level >= DNC.Levels.FanDance4
                    && HasEffect(DNC.Buffs.FourfoldFanDance))
                    return DNC.FanDance4;

                if (level >= DNC.Levels.FanDance3
                    && HasEffect(DNC.Buffs.ThreefoldFanDance))
                    return DNC.FanDance3;

                if (level >= DNC.Levels.Flourish
                    && gauge.Feathers < 4
                    && IsOffCooldown(DNC.Flourish)
                    && (HasEffect(DNC.Buffs.Devilment) || GetCooldown(DNC.Devilment).CooldownRemaining > 20))
                    return DNC.Flourish;

                // Only use feathers if full or you have the devilment buff
                if (gauge.Feathers == 4
                    || (HasEffect(DNC.Buffs.Devilment) && gauge.Feathers > 0))
                    return DNC.FanDance1;


            }

            if (level >= DNC.Levels.StandardStep && IsOffCooldown(DNC.StandardStep)) return DNC.StandardStep;

            if (level >= DNC.Levels.SaberDance
                    // Use only if you are gonna soon cap the Esprit gauge
                    && (gauge.Esprit >= 85  
                        // Of if the devilment buff is active and you have enough espirit buff.
                        || (HasEffect(DNC.Buffs.Devilment) && gauge.Esprit >= 50)))
                return DNC.SaberDance;

            // From Devilment
            if (level >= DNC.Levels.StarfallDance
                    && HasEffect(DNC.Buffs.Devilment)
                    && HasEffect(DNC.Buffs.FlourishingStarfall))
                return DNC.StarfallDance;

            if (level >= DNC.Levels.Tillana
                    // Use only if you are gonna soon cap the Esprit gauge
                    && HasEffect(DNC.Buffs.Devilment)
                    && HasEffect(DNC.Buffs.FlourishingFinish))
                return DNC.Tillana;

            // Single Target
            if (actionID == DNC.Cascade)
            {
                if (level >= DNC.Levels.Fountainfall
                        && (HasEffect(DNC.Buffs.FlourishingFlow)
                            || HasEffect(DNC.Buffs.SilkenFlow)))
                    return DNC.Fountainfall;

                if (level >= DNC.Levels.ReverseCascade
                        && (HasEffect(DNC.Buffs.FlourishingSymmetry)
                            || HasEffect(DNC.Buffs.SilkenSymmetry)))
                    return DNC.ReverseCascade;

                if (lastComboMove == DNC.Cascade
                    && level >= DNC.Levels.Fountain)
                    return DNC.Fountain;
            }

            // Aoe
            if (actionID == DNC.Windmill)
            {
                if (level >= DNC.Levels.Bloodshower
                    && (HasEffect(DNC.Buffs.FlourishingFlow)
                        || HasEffect(DNC.Buffs.SilkenFlow)))
                    return DNC.Bloodshower;

                if (level >= DNC.Levels.RisingWindmill
                    && (HasEffect(DNC.Buffs.FlourishingSymmetry)
                        || HasEffect(DNC.Buffs.SilkenSymmetry)))
                    return DNC.RisingWindmill;

                if (lastComboMove == DNC.Windmill
                    && level >= DNC.Levels.Bladeshower)
                    return DNC.Bladeshower;
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
