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
        DanceOfTheDawn = 36985,
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
        CuringWaltz = 16015,
        FanDance2 = 16008,
        FanDance3 = 16009,
        FanDance4 = 25791,
        // Other
        SaberDance = 16005,
        ClosedPosition = 16006,
        ShieldSamba = 16012,
        EnAvant = 16010,
        Devilment = 16011,
        Flourish = 16013,
        Improvisation = 16014,
        StarfallDance = 25792;

    public static class Buffs
    {
        public const ushort ClosedPosition = 1823,
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
            FanDance1 = 30,
            RisingWindmill = 35,
            Fountainfall = 40,
            Bloodshower = 45,
            FanDance2 = 50,
            CuringWaltz = 52,
            ShieldSamba = 56,
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
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.DancerDanceComboCompatibility;

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

internal class DancerStandardStepTechnicalStep : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DncAny;

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

                // return OriginalHook(DNC.StandardStep);
            }

            return !HasEffect(DNC.Buffs.StandardStep) && gauge.CompletedSteps < 2
                ? DNC.StandardStep
                : ADV.Peloton;
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

            return DNC.TechnicalStep;
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
            var hasOneRaidBuff = HasRaidBuffs(1);
            var hasTwoRaidBuffs = HasRaidBuffs(2);

            var gauge = GetJobGauge<DNCGauge>();

            var distance = GetTargetDistance();

            var hasStandardStep = HasEffect(DNC.Buffs.StandardStep);
            var hasTechnicalStep = HasEffect(DNC.Buffs.TechnicalStep);

            if (
                level >= DNC.Levels.StandardStep
                && gauge.IsDancing
                && (hasStandardStep || hasTechnicalStep)
            )
            {
                var stepNumber = hasStandardStep ? 2 : 4;

                if (gauge.CompletedSteps < stepNumber)
                    return gauge.NextStep;

                if (distance > 14)
                {
                    return ADV.Swiftcast;
                }

                return hasStandardStep
                    ? OriginalHook(DNC.StandardStep)
                    : OriginalHook(DNC.TechnicalStep);
            }

            if (
                level >= DNC.Levels.Devilment
                && IsOffCooldown(DNC.Devilment)
                && IsOnCooldown(DNC.TechnicalStep)
            )
                return DNC.Devilment;

            if (GCDClipCheck(actionID))
            {
                var reprisal = FindTargetEffectAny(ADV.Debuffs.Reprisal);
                var reprisalFound = reprisal is not null && reprisal.RemainingTime >= 13;

                switch (level)
                {
                    case >= DNC.Levels.FanDance4
                        when HasEffect(DNC.Buffs.FourfoldFanDance) && distance < 15:
                        return DNC.FanDance4;

                    case >= DNC.Levels.FanDance3 when HasEffect(DNC.Buffs.ThreefoldFanDance):
                        return DNC.FanDance3;

                    case >= DNC.Levels.Flourish
                        when gauge.Feathers < 4
                            && IsOffCooldown(DNC.Flourish)
                            && IsOnCooldown(DNC.Devilment):
                        return DNC.Flourish;

                    case >= DNC.Levels.FanDance2
                        when gauge.Feathers >= 1 && distance < 5 && actionID is DNC.Windmill:
                        return DNC.FanDance2;

                    case >= DNC.Levels.FanDance1
                        when gauge.Feathers >= 1 && (hasTwoRaidBuffs || gauge.Feathers == 4):
                        return DNC.FanDance1;

                    case >= DNC.Levels.ShieldSamba
                        when IsOffCooldown(DNC.ShieldSamba) && reprisalFound:
                        return DNC.ShieldSamba;

                    // case >= DNC.Levels.CuringWaltz
                    //     when IsOffCooldown(DNC.CuringWaltz) && LocalPlayerPercentage() <= 0.50:
                    //     return DNC.CuringWaltz;
                }
            }

            if (
                level >= DNC.Levels.TechnicalStep
                && IsOffCooldown(DNC.TechnicalStep)
                && hasTwoRaidBuffs
                && distance < 15
            )
                return DNC.TechnicalStep;

            var lastDanceReady = FindEffect(DNC.Buffs.LastDanceReady);

            if (
                level >= DNC.Levels.LastDance
                && lastDanceReady is not null
                && (
                    hasTwoRaidBuffs
                    || lastDanceReady.RemainingTime <= 5
                    || GetCooldown(OriginalHook(DNC.StandardStep)).CooldownRemaining <= 5
                    || actionID is DNC.Windmill
                )
            )
                return DNC.LastDance;

            if (
                level >= DNC.Levels.StandardStep
                && IsOffCooldown(OriginalHook(DNC.StandardStep))
                && (distance < 15 || !InCombat())
                && (
                    level < DNC.Levels.Flourish
                    || GetCooldown(DNC.Flourish).CooldownRemaining >= 5
                    || !InCombat()
                    || hasStandardStep
                )
                && (
                    GetCooldown(DNC.TechnicalStep).CooldownRemaining >= 11
                    || hasStandardStep
                    || !InCombat()
                    || actionID is DNC.Windmill
                )
            )
                return OriginalHook(DNC.StandardStep);

            if (
                level >= DNC.Levels.Tillana
                && (actionID is DNC.Windmill || hasOneRaidBuff)
                && gauge.Esprit < 50
                && CanUseAction(DNC.Tillana)
                && distance < 15
            )
                return DNC.Tillana;

            if (
                level >= DNC.Levels.DanceOfTheDawn
                && (CanUseAction(DNC.DanceOfTheDawn) || gauge.Esprit == 100)
            )
            {
                return OriginalHook(DNC.SaberDance);
            }

            if (
                level >= DNC.Levels.StarfallDance
                && hasOneRaidBuff
                && HasEffect(DNC.Buffs.FlourishingStarfall)
            )
                return DNC.StarfallDance;

            if (
                level >= DNC.Levels.SaberDance
                && CanUseAction(OriginalHook(DNC.SaberDance))
                && (
                    gauge.Esprit >= 50
                    || hasOneRaidBuff
                    || HasEffect(DNC.Buffs.FlourishingFinish)
                    || actionID is DNC.Windmill
                )
                && (comboTime >= 3 || comboTime == 0) // ensures we don't break combo
            )
            {
                return OriginalHook(DNC.SaberDance);
            }

            // Single Target
            if (actionID is DNC.Cascade)
            {
                if (comboTime >= 5 || comboTime == 0)
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
                }
                if (lastComboMove == DNC.Cascade && level >= DNC.Levels.Fountain)
                    return DNC.Fountain;
            }

            // Aoe
            if (actionID is DNC.Windmill)
            {
                if (comboTime >= 5 || comboTime == 0)
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
                }

                if (lastComboMove == DNC.Windmill && level >= DNC.Levels.Bladeshower)
                    return DNC.Bladeshower;
            }
        }

        return actionID;
    }
}
