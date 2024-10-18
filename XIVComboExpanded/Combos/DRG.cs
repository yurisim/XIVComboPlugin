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
        BarrageThrust = 36954,
        ExplosiveThrust = 36955,
        Drakesbane = 36952,
        BarrageThrust2 = 36954,
        ExplosiveThrust2 = 36955,
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
        DragonfireDive = 96,
        HighJump = 16478,
        MirageDive = 7399,
        // BUffs
        BattleLitany = 3557,
        DragonSight = 7398,
        // Dragon
        Stardiver = 16480,
        RiseOfTheDragon = 36953,
        Starcross = 36956,
        WyrmwindThrust = 25773;

    // Buff abilities
    // LanceCharge = 85,
    // DragonSight = 7398,
    // BattleLitany = 3557;

    public static class Buffs
    {
        public const ushort LifeSurge = 116,
            PowerSurge = 2720,
            BattleLitany = 786,
            LeftEye = 1184,
            FangAndClawBared = 802,
            WheelInMotion = 803,
            LanceCharge = 1864,
            //public const ushort
            //SharperFangAndClaw = 802,
            EnhancedWheelingThrust = 803,
            DiveReady = 1243,
            DraconianFire = 1863;
    }

    public static class Debuffs
    {
        public const ushort ChaosThrust = 118,
            ChaoticSpring = 2719;
    }

    public static class Levels
    {
        public const byte VorpalThrust = 4,
            LifeSurge = 6,
            Disembowel = 18,
            FullThrust = 26,
            LanceCharge = 30,
            Jump = 30,
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
            Drakesbane = 64,
            DragonSight = 66,
            MirageDive = 68,
            LifeOfTheDragon = 70,
            CoerthanTorment = 72,
            HighJump = 74,
            RaidenThrust = 76,
            Stardiver = 80,
            DraconianFury = 82,
            ImprovedLifeSurge = 88,
            WyrmwindThrust = 90,
            RiseOfTheDragon = 92,
            Starcross = 100;
    }
}

internal class DragoonSingleTarget : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrgAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DRG.TrueThrust)
        {
            var gauge = GetJobGauge<DRGGauge>();

            var timeOfRotation = 10.0;
            if (level < DRG.Levels.FangAndClaw)
                timeOfRotation = 10;
            else if (level < DRG.Levels.LanceMastery)
                timeOfRotation = 9;

            var disembowelDuration = FindEffect(DRG.Buffs.PowerSurge)?.RemainingTime;

            if (GCDClipCheck(actionID) && InCombat() && HasTarget())
            {
                static bool doWithLance(int? cooldownAbility)
                {
                    return (
                            cooldownAbility is not null
                            && GetCooldown(DRG.LanceCharge).CooldownRemaining
                                >= cooldownAbility * 0.1
                        )
                        || HasEffect(DRG.Buffs.LanceCharge)
                        || HasRaidBuffs(2);
                }

                switch (level)
                {
                    case >= DRG.Levels.BattleLitany
                        when IsOffCooldown(DRG.BattleLitany) && HasRaidBuffs(2):
                        return DRG.BattleLitany;

                    case >= DRG.Levels.LanceCharge
                        when InMeleeRange()
                            && IsOffCooldown(DRG.LanceCharge)
                            && (
                                HasEffect(DRG.Buffs.BattleLitany)
                                || HasRaidBuffs(2)
                                || level < DRG.Levels.BattleLitany
                                || GetCooldown(DRG.BattleLitany).CooldownRemaining >= 15
                            ):
                        return DRG.LanceCharge;

                    case >= DRG.Levels.Geirskogul
                        when CanUseAction(OriginalHook(DRG.Geirskogul))
                            && IsOffCooldown(OriginalHook(DRG.Geirskogul))
                            && doWithLance(60):
                        return OriginalHook(DRG.Geirskogul);

                    case >= DRG.Levels.WyrmwindThrust
                        when gauge.FirstmindsFocusCount == 2
                            && (
                                OriginalHook(DRG.TrueThrust) != DRG.TrueThrust || doWithLance(null)
                            ):
                        return DRG.WyrmwindThrust;
                }

                if (disembowelDuration is not null)
                    switch (level)
                    {
                        case >= DRG.Levels.LifeSurge
                            when (IsOffCooldown(DRG.LifeSurge) || HasCharges(DRG.LifeSurge))
                                && (
                                    level < DRG.Levels.ImprovedLifeSurge
                                    || GetCooldown(DRG.LifeSurge).TotalCooldownRemaining <= 20
                                    || HasRaidBuffs(2)
                                    || HasEffect(DRG.Buffs.LanceCharge)
                                )
                                && !HasEffect(DRG.Buffs.LifeSurge)
                                && (
                                    (
                                        lastComboMove == OriginalHook(DRG.VorpalThrust)
                                        && level >= DRG.Levels.FullThrust
                                    )
                                    || !(
                                        IsOriginal(DRG.WheelingThrust)
                                        || IsOriginal(DRG.FangAndClaw)
                                    )
                                    || (
                                        lastComboMove == DRG.TrueThrust
                                        && level < DRG.Levels.FullThrust
                                    )
                                ):
                            return DRG.LifeSurge;
                        case >= DRG.Levels.DragonfireDive
                            when IsOffCooldown(DRG.DragonfireDive) && doWithLance(120):
                            return DRG.DragonfireDive;

                        case >= DRG.Levels.Stardiver
                            when gauge.IsLOTDActive
                                && IsOffCooldown(DRG.Stardiver)
                                && doWithLance(gauge.LOTDTimer / 1000 - 5):
                            return DRG.Stardiver;

                        case >= DRG.Levels.Starcross when CanUseAction(DRG.Starcross):
                            return DRG.Starcross;

                        case >= DRG.Levels.Jump
                            when IsOffCooldown(OriginalHook(DRG.Jump))
                                && CanUseAction(OriginalHook(DRG.Jump))
                                && (
                                    doWithLance(30)
                                    || FindEffect(DRG.Buffs.DiveReady)?.RemainingTime <= 8
                                ):
                            return OriginalHook(DRG.Jump);

                        case >= DRG.Levels.RiseOfTheDragon when CanUseAction(DRG.RiseOfTheDragon):
                            return DRG.RiseOfTheDragon;
                    }
            }

            if (comboTime > 0)
            {
                if (lastComboMove == DRG.TrueThrust || lastComboMove == DRG.RaidenThrust)
                {
                    var needToRefreshDisembowel =
                        disembowelDuration is null || disembowelDuration <= timeOfRotation - 2.5;

                    var whichDot =
                        level >= DRG.Levels.ChaoticSpring ? DRG.ChaoticSpring
                        : level >= DRG.Levels.ChaosThrust ? DRG.ChaosThrust
                        : 0;

                    var whichDotEffect =
                        level >= DRG.Levels.ChaoticSpring
                            ? FindTargetEffect(DRG.Debuffs.ChaoticSpring)
                        : level >= DRG.Levels.ChaosThrust
                            ? FindTargetEffect(DRG.Debuffs.ChaosThrust)
                        : null;

                    var refreshDot =
                        whichDot != 0
                        && (
                            (
                                whichDotEffect is not null
                                && whichDotEffect.RemainingTime <= timeOfRotation
                            )
                            || (
                                whichDotEffect is null
                                && ShouldUseDots()
                                && level >= DRG.Levels.ChaosThrust
                            )
                        );

                    if (level >= DRG.Levels.Disembowel && (needToRefreshDisembowel || refreshDot))
                        return OriginalHook(DRG.Disembowel);
                    return OriginalHook(DRG.VorpalThrust);
                }

                // AM I doing Disembowel or Vorpal Thrust after True Thrust?
                if (
                    lastComboMove == OriginalHook(DRG.Disembowel)
                    && level >= DRG.Levels.ChaosThrust
                )
                    return OriginalHook(DRG.ChaosThrust);

                if (
                    lastComboMove == OriginalHook(DRG.VorpalThrust)
                    && level >= DRG.Levels.FullThrust
                )
                    return OriginalHook(DRG.FullThrust);

                if (
                    level >= DRG.Levels.WheelingThrust
                    && lastComboMove == OriginalHook(DRG.ChaosThrust)
                )
                    return DRG.WheelingThrust;

                if (
                    level >= DRG.Levels.FangAndClaw
                    && lastComboMove == OriginalHook(DRG.FullThrust)
                )
                    return DRG.FangAndClaw;

                if (level >= DRG.Levels.Drakesbane)
                {
                    if (!IsOriginal(DRG.WheelingThrust))
                        return OriginalHook(DRG.WheelingThrust);
                    if (!IsOriginal(DRG.FangAndClaw))
                        return OriginalHook(DRG.FangAndClaw);
                }
            }

            return OriginalHook(DRG.TrueThrust);
        }

        return actionID;
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

            var disembowelDuration = FindEffect(DRG.Buffs.PowerSurge)?.RemainingTime;

            if (GCDClipCheck(actionID) && InCombat() && HasTarget())
            {
                static bool doWithLance(int? cooldownAbility)
                {
                    return (
                            cooldownAbility is not null
                            && GetCooldown(DRG.LanceCharge).CooldownRemaining
                                >= cooldownAbility * 0.1
                        )
                        || HasEffect(DRG.Buffs.LanceCharge)
                        || HasRaidBuffs(2);
                }

                switch (level)
                {
                    case >= DRG.Levels.BattleLitany
                        when IsOffCooldown(DRG.BattleLitany) && HasRaidBuffs(2):
                        return DRG.BattleLitany;

                    case >= DRG.Levels.LanceCharge
                        when InMeleeRange()
                            && IsOffCooldown(DRG.LanceCharge)
                            && (
                                HasEffect(DRG.Buffs.BattleLitany)
                                || HasRaidBuffs(2)
                                || level < DRG.Levels.BattleLitany
                                || GetCooldown(DRG.BattleLitany).CooldownRemaining >= 12
                            ):
                        return DRG.LanceCharge;

                    case >= DRG.Levels.Geirskogul
                        when CanUseAction(OriginalHook(DRG.Geirskogul))
                            && IsOffCooldown(OriginalHook(DRG.Geirskogul))
                            && doWithLance(60):
                        return OriginalHook(DRG.Geirskogul);

                    case >= DRG.Levels.WyrmwindThrust
                        when gauge.FirstmindsFocusCount == 2
                            && (
                                OriginalHook(DRG.TrueThrust) != DRG.TrueThrust || doWithLance(null)
                            ):
                        return DRG.WyrmwindThrust;
                }

                if (disembowelDuration is not null)
                    switch (level)
                    {
                        case >= DRG.Levels.LifeSurge
                            when (IsOffCooldown(DRG.LifeSurge) || HasCharges(DRG.LifeSurge))
                                && (
                                    level < DRG.Levels.ImprovedLifeSurge
                                    || GetCooldown(DRG.LifeSurge).TotalCooldownRemaining <= 15
                                    || HasRaidBuffs(2)
                                    || HasEffect(DRG.Buffs.LanceCharge)
                                )
                                && !HasEffect(DRG.Buffs.LifeSurge)
                                && (
                                    (
                                        lastComboMove == DRG.SonicThrust
                                        && level >= DRG.Levels.CoerthanTorment
                                    )
                                    || (
                                        lastComboMove == DRG.DoomSpike
                                        && level < DRG.Levels.CoerthanTorment
                                    )
                                ):
                            return DRG.LifeSurge;
                        case >= DRG.Levels.DragonfireDive
                            when IsOffCooldown(DRG.DragonfireDive) && doWithLance(120):
                            return DRG.DragonfireDive;
                        case >= DRG.Levels.Stardiver
                            when gauge.IsLOTDActive
                                && IsOffCooldown(DRG.Stardiver)
                                && doWithLance(gauge.LOTDTimer / 1000 - 5):
                            return DRG.Stardiver;

                        case >= DRG.Levels.Starcross when CanUseAction(DRG.Starcross):
                            return DRG.Starcross;

                        case >= DRG.Levels.Jump
                            when IsOffCooldown(OriginalHook(DRG.Jump))
                                && CanUseAction(OriginalHook(DRG.Jump))
                                && (
                                    doWithLance(30)
                                    || FindEffect(DRG.Buffs.DiveReady)?.RemainingTime <= 8
                                ):
                            return OriginalHook(DRG.Jump);
                        case >= DRG.Levels.RiseOfTheDragon when CanUseAction(DRG.RiseOfTheDragon):
                            return DRG.RiseOfTheDragon;
                    }
            }

            if (comboTime > 0)
            {
                if (lastComboMove == DRG.SonicThrust && level >= DRG.Levels.CoerthanTorment)
                    return DRG.CoerthanTorment;

                if (
                    (lastComboMove == DRG.DoomSpike || lastComboMove == DRG.DraconianFury)
                    && level >= DRG.Levels.SonicThrust
                )
                    return DRG.SonicThrust;
            }

            // Draconian Fury
            return OriginalHook(DRG.DoomSpike);
        }

        return actionID;
    }
}

internal class DragoonPositionals : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrgAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DRG.WheelingThrust)
            if (lastComboMove == DRG.Disembowel && level >= DRG.Levels.ChaosThrust)
                return OriginalHook(DRG.ChaosThrust);

        return actionID;
    }
}
