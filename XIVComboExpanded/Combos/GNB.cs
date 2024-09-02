using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class GNB
{
    public const byte JobID = 37;

    public const uint DangerZone = 16144,
        KeenEdge = 16137,
        NoMercy = 16138,
        BrutalShell = 16139,
        DemonSlice = 16141,
        RoyalGuard = 16142,
        SolidBarrel = 16145,
        GnashingFang = 16146,
        SavageClaw = 16147,
        WickedTalon = 16150,
        DemonSlaughter = 16149,
        Aurora = 16151,
        SonicBreak = 16153,
        Continuation = 16155,
        JugularRip = 16156,
        AbdomenTear = 16157,
        EyeGouge = 16158,
        BowShock = 16159,
        BurstStrike = 16162,
        FatedCircle = 16163,
        Bloodfest = 16164,
        Hypervelocity = 25759,
        HeartOfCorundum = 25758,
        DoubleDown = 25760,
        RoyalGuardRemoval = 32068,
        FatedBrand = 36936;

    public static class Buffs
    {
        public const ushort NoMercy = 1831,
            RoyalGuard = 1833,
            Aurora = 1835,
            Superbolide = 1836,
            ReadyToRip = 1842,
            ReadyToTear = 1843,
            ReadyToGouge = 1844,
            ReadyToBlast = 2686,
            ReadyToBreak = 3886,
            ReadyToFated = 3839;
    }

    public static class Debuffs
    {
        public const ushort BowShock = 1838;
    }

    public static class Levels
    {
        public const byte NoMercy = 2,
            BrutalShell = 4,
            DangerZone = 18,
            RoyalGuard = 10,
            SolidBarrel = 26,
            BurstStrike = 30,
            DemonSlaughter = 40,
            Aurora = 45,
            SonicBreak = 54,
            GnashingFang = 60,
            BowShock = 62,
            Continuation = 70,
            FatedCircle = 72,
            Bloodfest = 76,
            HeartOfCorundum = 82,
            EnhancedContinuation = 86,
            CartridgeCharge2 = 88,
            DoubleDown = 90;
    }
}

internal class GunbreakerSolidBarrel : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GnbAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == GNB.KeenEdge)
        {
            var noMercyCD = GetCooldown(GNB.NoMercy).CooldownRemaining;
            var gauge = GetJobGauge<GNBGauge>();

            if (
                level >= GNB.Levels.Continuation
                && GNB.Continuation != OriginalHook(GNB.Continuation)
            )
            {
                return OriginalHook(GNB.Continuation);
            }

            var maxAmmo = level >= GNB.Levels.CartridgeCharge2 ? 3 : 2;

            if (GCDClipCheck(actionID))
            {
                if (
                    level >= GNB.Levels.HeartOfCorundum
                    && IsOffCooldown(GNB.HeartOfCorundum)
                    && !HasEffect(GNB.Buffs.Superbolide)
                    && (LocalPlayerPercentage() <= 0.6 || TargetOfTargetHPercentage() <= 0.6)
                )
                {
                    return GNB.HeartOfCorundum;
                }

                if (
                    level >= GNB.Levels.Bloodfest
                    && (
                        HasEffect(GNB.Buffs.NoMercy)
                        || noMercyCD >= 24
                        || lastComboMove == GNB.BrutalShell
                    )
                    && IsOffCooldown(GNB.Bloodfest)
                    && gauge.Ammo == 0
                )
                {
                    return GNB.Bloodfest;
                }

                if (
                    IsOffCooldown(GNB.NoMercy)
                    && (
                        gauge.Ammo >= 1
                        || level < GNB.Levels.BurstStrike
                        || lastComboMove == GNB.BrutalShell
                    )
                )
                {
                    return GNB.NoMercy;
                }

                if (
                    level >= GNB.Levels.DangerZone
                    && (HasEffect(GNB.Buffs.NoMercy) || noMercyCD >= 6)
                    && IsOffCooldown(OriginalHook(GNB.DangerZone))
                )
                {
                    return OriginalHook(GNB.DangerZone);
                }

                if (
                    level >= GNB.Levels.BowShock
                    && IsOffCooldown(GNB.BowShock)
                    && (HasEffect(GNB.Buffs.NoMercy) || noMercyCD >= 12)
                )
                {
                    return GNB.BowShock;
                }

                if (
                    level >= GNB.Levels.Aurora
                    && (IsOffCooldown(GNB.Aurora) || HasCharges(GNB.Aurora))
                    && !HasEffect(GNB.Buffs.Aurora)
                    && (TargetOfTargetHPercentage() <= 0.8)
                )
                {
                    return GNB.Aurora;
                }
            }

            if (CanUseAction(GNB.SavageClaw))
                return GNB.SavageClaw;

            if (CanUseAction(GNB.WickedTalon))
                return GNB.WickedTalon;

            var oneMinCD = 58.80 * 0.2;

            // Weaponskills
            if (level >= GNB.Levels.SonicBreak && HasEffect(GNB.Buffs.ReadyToBreak))
            {
                return GNB.SonicBreak;
            }

            if (
                level >= GNB.Levels.DoubleDown
                && IsOffCooldown(GNB.DoubleDown)
                && gauge.Ammo >= 2
                && GetTargetDistance() <= 5
                && (HasEffect(GNB.Buffs.NoMercy) || noMercyCD >= oneMinCD)
            )
            {
                return GNB.DoubleDown;
            }

            if (
                level < GNB.Levels.DoubleDown
                || (
                    IsOnCooldown(GNB.DoubleDown)
                    && GetCooldown(GNB.DoubleDown).CooldownRemaining >= 5
                )
            )
            {
                if (
                    level >= GNB.Levels.GnashingFang
                    && IsOffCooldown(GNB.GnashingFang)
                    && gauge.Ammo >= 1
                    && (HasEffect(GNB.Buffs.NoMercy) || noMercyCD >= oneMinCD / 2)
                )
                {
                    return GNB.GnashingFang;
                }

                var noMercy = FindEffect(GNB.Buffs.NoMercy);

                var bloodfestOffCD =
                    level >= GNB.Levels.Bloodfest
                    && HasEffect(GNB.Buffs.NoMercy)
                    && IsOffCooldown(GNB.Bloodfest);

                if (
                    gauge.Ammo >= 1
                    && (
                        level < GNB.Levels.GnashingFang
                        || (
                            IsOnCooldown(GNB.GnashingFang)
                            && GetCooldown(GNB.GnashingFang).CooldownRemaining >= 5
                        )
                    )
                    && noMercy is not null
                    && (bloodfestOffCD || (noMercy.RemainingTime < gauge.Ammo * 4))
                )
                {
                    return GNB.BurstStrike;
                }
            }

            // COMBO BLOCK
            if (comboTime > 0)
            {
                if (lastComboMove == GNB.BrutalShell && level >= GNB.Levels.SolidBarrel)
                {
                    if (level >= GNB.Levels.BurstStrike && gauge.Ammo == maxAmmo)
                    {
                        return GNB.BurstStrike;
                    }

                    return GNB.SolidBarrel;
                }

                if (lastComboMove == GNB.KeenEdge && level >= GNB.Levels.BrutalShell)
                    return GNB.BrutalShell;
            }

            return GNB.KeenEdge;
        }

        return actionID;
    }
}


internal class GunbreakerDemonSlaughter : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GnbAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == GNB.DemonSlice)
        {
            var noMercyCD = GetCooldown(GNB.NoMercy).CooldownRemaining;
            var gauge = GetJobGauge<GNBGauge>();

            var maxAmmo = level >= GNB.Levels.CartridgeCharge2 ? 3 : 2;

            if (GCDClipCheck(actionID))
            {
                switch (level)
                {
                    case >= GNB.Levels.Bloodfest when HasEffect(GNB.Buffs.NoMercy) && IsOffCooldown(GNB.Bloodfest) && gauge.Ammo == 0:
                        return GNB.Bloodfest;

                    case >= GNB.Levels.Aurora when (IsOffCooldown(GNB.Aurora) || HasCharges(GNB.Aurora)) && !HasEffect(GNB.Buffs.Aurora) && (TargetOfTargetHPercentage() <= 0.7):
                        return GNB.Aurora;

                    case >= GNB.Levels.BowShock when IsOffCooldown(GNB.BowShock) && (HasEffect(GNB.Buffs.NoMercy) || noMercyCD >= 12):
                        return GNB.BowShock;

                    case >= GNB.Levels.DangerZone when (HasEffect(GNB.Buffs.NoMercy) || noMercyCD >= 6) && IsOffCooldown(GNB.DangerZone):
                        return GNB.DangerZone;

                    case >= GNB.Levels.HeartOfCorundum when IsOffCooldown(GNB.HeartOfCorundum) && !HasEffect(GNB.Buffs.Superbolide) && (LocalPlayerPercentage() <= 0.6 || TargetOfTargetHPercentage() <= 0.6):
                        return GNB.HeartOfCorundum;

                    case >= GNB.Levels.Continuation when GNB.Continuation != OriginalHook(GNB.Continuation):
                        return OriginalHook(GNB.Continuation);

                    case >= 2 when IsOffCooldown(GNB.NoMercy):
                        return GNB.NoMercy;
                }
            }

            var oneMinCD = 58.80 * 0.2;

            var noMercyDuration = FindEffect(GNB.Buffs.NoMercy);

            if (
                level >= GNB.Levels.DoubleDown
                && IsOffCooldown(GNB.DoubleDown)
                && gauge.Ammo >= 2
                && GetTargetDistance() <= 5
                && (HasEffect(GNB.Buffs.NoMercy) || noMercyCD >= oneMinCD)
            )
            {
                return GNB.DoubleDown;
            }

            if (
                level >= GNB.Levels.FatedCircle
                && gauge.Ammo >= 1
                && (level < GNB.Levels.DoubleDown || IsOnCooldown(GNB.DoubleDown))
                && (gauge.Ammo >= maxAmmo || noMercyDuration?.RemainingTime <= gauge.Ammo * 4)
            )
            {
                return GNB.FatedCircle;
            }

            if (
                comboTime > 0
                && lastComboMove == GNB.DemonSlice
                && level >= GNB.Levels.DemonSlaughter
            )
            {
                return GNB.DemonSlaughter;
            }

            return GNB.DemonSlice;
        }

        return actionID;
    }
}