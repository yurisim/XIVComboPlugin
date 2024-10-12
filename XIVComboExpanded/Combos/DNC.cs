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
        LastDance = 36983,
        FinishingMove = 36984,
        // Fans
        FanDance1 = 16007,
        FanDance2 = 16008,
        FanDance3 = 16009,
        FanDance4 = 25791,
        // Other
        SaberDance = 16005,
        ClosedPosition = 16006,
        EnAvant = 16010,
        Devilment = 16011,
        Flourish = 16013,
        Improvisation = 16014,
        StarfallDance = 25792;

    public static class Buffs
    {
        public const ushort
            ClosedPosition = 1823,
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
            LastDanceReady = 3867,
            FinishingMoveReady = 3868,
            DanceOfTheDawnReady = 3869,
            Devilment = 1825,
            FourfoldFanDance = 2699;
    }

    public static class Debuffs
    {
        public const ushort Placeholder = 0;
    }

    public static class Levels
    {
        public const byte Cascade = 1,
            Fountain = 2,
            Windmill = 15,
            StandardStep = 15,
            ReverseCascade = 20,
            Bladeshower = 25,
            RisingWindmill = 35,
            Fountainfall = 40,
            Bloodshower = 45,
            Devilment = 62,
            ClosedPosition = 60,
            FanDance3 = 66,
            TechnicalStep = 70,
            Flourish = 72,
            SaberDance = 76,
            Tillana = 82,
            FanDance4 = 86,
            StarfallDance = 90,
            LastDance = 92,
            FinishingMove = 96,
            DanceOfTheDawn = 100;
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
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.DncAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DNC.StandardStep)
        {
            var gauge = GetJobGauge<DNCGauge>();

            if (
                level >= DNC.Levels.StandardStep
                && gauge.IsDancing
                && HasEffect(DNC.Buffs.StandardStep)
            )
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

            if (
                level >= DNC.Levels.TechnicalStep
                && gauge.IsDancing
                && HasEffect(DNC.Buffs.TechnicalStep)
            )
                if (gauge.CompletedSteps < 4)
                    return gauge.NextStep;

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

            if (
                level >= DNC.Levels.StandardStep
                && gauge.IsDancing
                && HasEffect(DNC.Buffs.StandardStep)
            )
            {
                if (gauge.CompletedSteps < 2)
                    return gauge.NextStep;

                return OriginalHook(DNC.StandardStep);
            }

            if (GCDClipCheck(actionID))
            {

                if (
                    level >= DNC.Levels.Devilment
                    && IsOffCooldown(DNC.Devilment)
                    && (HasEffect(DNC.Buffs.TechnicalFinish) || HasRaidBuffs(2))
                )
                {
                    return DNC.Devilment;
                }

                if (level >= DNC.Levels.FanDance4 && HasEffect(DNC.Buffs.FourfoldFanDance))
                    return DNC.FanDance4;

                if (level >= DNC.Levels.FanDance3 && HasEffect(DNC.Buffs.ThreefoldFanDance))
                    return DNC.FanDance3;

                if (
                    level >= DNC.Levels.Flourish
                    && gauge.Feathers < 4
                    && IsOffCooldown(DNC.Flourish)
                    && (
                        HasRaidBuffs(2)
                        || GetCooldown(DNC.Devilment).CooldownRemaining >= 5
                    )
                )
                    return DNC.Flourish;

                // Only use feathers if full or you have the devilment buff
                if (gauge.Feathers >= 1
                    && (HasRaidBuffs(2) || gauge.Feathers == 4)
                     )
                    return DNC.FanDance1;
            }

            if (level >= DNC.Levels.StandardStep
                && IsOffCooldown(OriginalHook(DNC.StandardStep))
                && (level < DNC.Levels.FinishingMove
                    || (level >= DNC.Levels.FinishingMove
                        && (IsOnCooldown(DNC.Flourish)
                            || !InCombat()
                            || actionID is DNC.Windmill
                            )
                        )
                    )
                )
                return OriginalHook(DNC.StandardStep);

            if (
                level >= DNC.Levels.Tillana
                && (actionID is DNC.Windmill || HasRaidBuffs(2))
                && gauge.Esprit < 50
                && HasEffect(DNC.Buffs.FlourishingFinish)
            )
                return DNC.Tillana;

            if (
                level >= DNC.Levels.SaberDance
                && CanUseAction(OriginalHook(DNC.SaberDance))
                && (gauge.Esprit >= 85
                    || HasRaidBuffs(2)
                    || HasEffect(DNC.Buffs.FlourishingFinish)
                    || actionID is DNC.Windmill
                )
            )
                return OriginalHook(DNC.SaberDance);

            if (level >= DNC.Levels.LastDance
                && HasEffect(DNC.Buffs.LastDanceReady)
                && (HasRaidBuffs(2)
                    || GetCooldown(DNC.StandardStep).CooldownRemaining <= 10
                    || actionID is DNC.Windmill)
               )
                return DNC.LastDance;

            if (
                level >= DNC.Levels.StarfallDance
                && HasRaidBuffs(2)
                && HasEffect(DNC.Buffs.FlourishingStarfall)
            )
                return DNC.StarfallDance;

            // Single Target
            if (actionID is DNC.Cascade)
            {
                if (
                    level >= DNC.Levels.Fountainfall
                    && (HasEffect(DNC.Buffs.FlourishingFlow) || HasEffect(DNC.Buffs.SilkenFlow))
                )
                    return DNC.Fountainfall;

                if (
                    level >= DNC.Levels.ReverseCascade
                    && (
                        HasEffect(DNC.Buffs.FlourishingSymmetry)
                        || HasEffect(DNC.Buffs.SilkenSymmetry)
                    )
                )
                    return DNC.ReverseCascade;

                if (lastComboMove == DNC.Cascade && level >= DNC.Levels.Fountain)
                    return DNC.Fountain;
            }

            // Aoe
            if (actionID is DNC.Windmill)
            {
                if (
                    level >= DNC.Levels.Bloodshower
                    && (HasEffect(DNC.Buffs.FlourishingFlow) || HasEffect(DNC.Buffs.SilkenFlow))
                )
                    return DNC.Bloodshower;

                if (
                    level >= DNC.Levels.RisingWindmill
                    && (
                        HasEffect(DNC.Buffs.FlourishingSymmetry)
                        || HasEffect(DNC.Buffs.SilkenSymmetry)
                    )
                )
                    return DNC.RisingWindmill;

                if (lastComboMove == DNC.Windmill && level >= DNC.Levels.Bladeshower)
                    return DNC.Bladeshower;
            }
        }

        return actionID;
    }
}