using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class DRG
{
    public const byte ClassID = 4;
    public const byte JobID = 22;

    public const uint
        // Single Target
        TrueThrust = 75,
        VorpalThrust = 78,
        LifeSurge = 83,
        Disembowel = 87,
        FullThrust = 84,
        LanceCharge = 85,
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
        // BUffs
        BattleLitany = 3557,
        DragonSight = 7398,
        // Dragon
        Stardiver = 16480,
        WyrmwindThrust = 25773;

    public static class Buffs
    {
        public const ushort
            PowerSurge = 2720,
            BattleLitany = 786,
            FangAndClawBared = 802,
            WheelInMotion = 803,
            DiveReady = 1243,
            LanceCharge = 1864;
    }

    public static class Debuffs
    {
        public const ushort
            ChaosThrust = 118,
            ChaoticSpring = 2719;
    }

    public static class Levels
    {
        public const byte
            VorpalThrust = 4,
            Disembowel = 18,
            FullThrust = 26,
            LanceCharge = 30,
            Jump = 30,
            SpineshatterDive = 45,
            DragonfireDive = 50,
            ChaosThrust = 50,
            BattleLitany = 52,
            HeavensThrust = 86,
            ChaoticSpring = 86,
            FangAndClaw = 56,
            WheelingThrust = 58,
            Geirskogul = 60,
            SonicThrust = 62,
            LanceMastery = 64,
            DragonSight = 66,
            MirageDive = 68,
            LifeOfTheDragon = 70,
            CoerthanTorment = 72,
            HighJump = 74,
            RaidenThrust = 76,
            Stardiver = 80,
            WyrmwindThrust = 90;
    }
}

internal class DragoonCoerthanTorment : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrgAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DRG.DoomSpike)
        {
            var gauge = GetJobGauge<DRGGauge>();


            if (GCDClipCheck(actionID))
            {
                var lanceChargeCD = GetCooldown(DRG.LanceCharge).CooldownRemaining;
                var hasLanceCharge = HasEffect(DRG.Buffs.LanceCharge);


                var hasLitany = HasEffect(DRG.Buffs.BattleLitany);
                var litanyCD = GetCooldown(DRG.BattleLitany).CooldownRemaining;

                if (IsOffCooldown(DRG.LanceCharge)
                    && level >= DRG.Levels.LanceCharge)
                {
                    return DRG.LanceCharge;
                }

                if (IsOffCooldown(DRG.DragonSight)
                    && level >= DRG.Levels.DragonSight
                    && (hasLitany || litanyCD > 6))
                {
                    return DRG.DragonSight;
                }

                if (IsOffCooldown(DRG.LifeSurge)
                    && ((lastComboMove == DRG.SonicThrust && level >= DRG.Levels.CoerthanTorment)
                        || (lastComboMove == DRG.DoomSpike && level < DRG.Levels.CoerthanTorment)))
                {
                    return DRG.LifeSurge;
                }

                if ((IsOffCooldown(OriginalHook(DRG.Jump)))
                    && level >= DRG.Levels.Jump
                    && (hasLanceCharge || lanceChargeCD > 3))
                {
                    return OriginalHook(DRG.Jump);
                }

                if (HasEffect(DRG.Buffs.DiveReady)
                    && level >= DRG.Levels.MirageDive
                    && (hasLanceCharge || lanceChargeCD > 3))
                {
                    return DRG.MirageDive;
                }

                if (IsOffCooldown(OriginalHook(DRG.Geirskogul))
                    && level >= DRG.Levels.Geirskogul
                    && (hasLanceCharge || lanceChargeCD > 3))
                {
                    return OriginalHook(DRG.Geirskogul);
                }

                // Optimize with Litany
                if (IsOffCooldown(DRG.DragonfireDive)
                    && level >= DRG.Levels.DragonfireDive)
                {
                    return DRG.DragonfireDive;
                }

                if (IsOffCooldown(DRG.SpineshatterDive)
                    && level >= DRG.Levels.SpineshatterDive
                    && (hasLanceCharge || lanceChargeCD > 6))
                {
                    return DRG.SpineshatterDive;
                }

                if (gauge.FirstmindsFocusCount == 2)
                    return DRG.WyrmwindThrust;

            }



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

        return actionID;
    }
}

internal class DragoonChaosThrust : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrgAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DRG.TrueThrust)
        {

            var gauge = GetJobGauge<DRGGauge>();

            var timeOfRotation = 10.0;
            if (level < DRG.Levels.FangAndClaw)
            {
                timeOfRotation = 10;
            }
            else if (level < DRG.Levels.LanceMastery)
            {
                timeOfRotation = 9;
            }

            var disembowelDuration = FindEffect(DRG.Buffs.PowerSurge)?.RemainingTime;

            if (GCDClipCheck(actionID))
            {
                var lanceChargeCD = GetCooldown(DRG.LanceCharge).CooldownRemaining;
                var hasLanceCharge = HasEffect(DRG.Buffs.LanceCharge);


                var hasLitany = HasEffect(DRG.Buffs.BattleLitany);
                var litanyCD = GetCooldown(DRG.BattleLitany).CooldownRemaining;


                if (IsOffCooldown(DRG.LanceCharge)
                    && level >= DRG.Levels.LanceCharge
                    && (hasLitany || litanyCD > 6 || level < DRG.Levels.BattleLitany))
                {
                    return DRG.LanceCharge;
                }

                if (IsOffCooldown(DRG.DragonSight)
                    && level >= DRG.Levels.DragonSight
                    && (hasLitany || litanyCD > 6))
                {
                    return DRG.DragonSight;
                }

                if (disembowelDuration is not null)
                {
                    if (IsOffCooldown(DRG.LifeSurge)
                        //&& (hasLitany || litanyCD > 2)
                        //&& (hasLanceCharge || lanceChargeCD > 4 || level < DRG.Levels.LanceCharge)
                        && ((lastComboMove == DRG.VorpalThrust && level >= DRG.Levels.FullThrust)
                            || (lastComboMove == DRG.TrueThrust && level < DRG.Levels.FullThrust)))
                    {
                        return DRG.LifeSurge;
                    }

                    if ((IsOffCooldown(OriginalHook(DRG.Jump)))
                        && level >= DRG.Levels.Jump
                        && (hasLanceCharge || lanceChargeCD > 3)
                        && (hasLitany || litanyCD > 2 || level < DRG.Levels.BattleLitany))
                    {
                        return OriginalHook(DRG.Jump);
                    }

                    if (HasEffect(DRG.Buffs.DiveReady)
                        && level >= DRG.Levels.MirageDive
                        && (hasLanceCharge || lanceChargeCD > 3)
                        && (hasLitany || litanyCD > 2 || level < DRG.Levels.BattleLitany))
                    {
                        return DRG.MirageDive;
                    }

                    if (IsOffCooldown(OriginalHook(DRG.Geirskogul))
                        && level >= DRG.Levels.Geirskogul
                        && (hasLanceCharge || lanceChargeCD > 3)
                        && (hasLitany || litanyCD > 2))
                    {
                        return OriginalHook(DRG.Geirskogul);
                    }

                    // Optimize with Litany
                    if (IsOffCooldown(DRG.DragonfireDive)
                        && level >= DRG.Levels.DragonfireDive
                        && (hasLanceCharge || lanceChargeCD > 11)
                        && (hasLitany || litanyCD > 6 || level < DRG.Levels.BattleLitany))
                    {
                        return DRG.DragonfireDive;
                    }

                    if (IsOffCooldown(DRG.SpineshatterDive)
                        && level >= DRG.Levels.SpineshatterDive
                        && (hasLanceCharge || lanceChargeCD > 6)
                        && (hasLitany || litanyCD > 3 || level < DRG.Levels.BattleLitany))
                    {
                        return DRG.SpineshatterDive;
                    }
                }
            }

            if (level >= DRG.Levels.FangAndClaw
                && HasEffect(DRG.Buffs.FangAndClawBared))
                return DRG.FangAndClaw;

            if (level >= DRG.Levels.WheelingThrust
                && HasEffect(DRG.Buffs.WheelInMotion))
                return DRG.WheelingThrust;

            if (comboTime > 0)
            {

                // AM I doing Disembowel or Vorpal Thrust after True Thrust?
                if (lastComboMove == DRG.TrueThrust || lastComboMove == DRG.RaidenThrust)
                {
                    var needToRefreshDisembowel = disembowelDuration is null || disembowelDuration <= timeOfRotation;

                    if (level >= DRG.Levels.Disembowel
                        && needToRefreshDisembowel)
                    {
                        return DRG.Disembowel;
                    }
                    else
                    {
                        return DRG.VorpalThrust;
                    }
                }

                // AM I doing Disembowel or Vorpal Thrust after True Thrust?
                if (lastComboMove == DRG.Disembowel
                    && level >= DRG.Levels.ChaosThrust)
                {
                    return OriginalHook(DRG.ChaosThrust);
                }

                if (lastComboMove == DRG.VorpalThrust
                    && level >= DRG.Levels.FullThrust)
                {

                    return OriginalHook(DRG.FullThrust);
                }
            }

            return OriginalHook(DRG.TrueThrust);
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
                if (level >= DRG.Levels.FangAndClaw && (HasEffect(DRG.Buffs.FangAndClawBared) || HasEffect(DRG.Buffs.WheelInMotion)))
                    return DRG.FangAndClaw;
            }

            if (IsEnabled(CustomComboPreset.DragoonFullThrustCombo))
            {
                if (level >= DRG.Levels.WheelingThrust && HasEffect(DRG.Buffs.WheelInMotion))
                    return DRG.WheelingThrust;

                if (level >= DRG.Levels.FangAndClaw && HasEffect(DRG.Buffs.FangAndClawBared))
                    return DRG.FangAndClaw;

                if (comboTime > 0)
                {
                    if (lastComboMove == DRG.VorpalThrust && level >= DRG.Levels.FullThrust)
                        // Heavens' Thrust
                        return IsOffCooldown(DRG.LifeSurge) ? DRG.LifeSurge : OriginalHook(DRG.FullThrust);

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
                if (level < DRG.Levels.Stardiver || IsOnCooldown(DRG.Stardiver) || (IsOffCooldown(DRG.DragonfireDive) && gauge.LOTDTimer > 7.5))
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

internal class DragoonGierskogul : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DragoonGeirskogulWyrmwindFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DRG.Geirskogul)
        {
            if (level >= DRG.Levels.WyrmwindThrust)
            {
                var gauge = GetJobGauge<DRGGauge>();

                if (gauge.FirstmindsFocusCount == 2)
                {
                    var action = gauge.IsLOTDActive
                        ? DRG.Nastrond
                        : DRG.Geirskogul;

                    if (IsOnCooldown(action))
                        return DRG.WyrmwindThrust;
                }
            }
        }

        return actionID;
    }
}
