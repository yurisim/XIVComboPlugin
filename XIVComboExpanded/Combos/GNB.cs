using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class GNB
{
    public const byte JobID = 37;

    public const uint

        DangerZone = 16144,
        KeenEdge = 16137,
        NoMercy = 16138,
        BrutalShell = 16139,
        DemonSlice = 16141,
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
        DoubleDown = 25760;

    public static class Buffs
    {
        public const ushort
            NoMercy = 1831,
            ReadyToRip = 1842,
            ReadyToTear = 1843,
            ReadyToGouge = 1844,
            ReadyToBlast = 2686;
    }

    public static class Debuffs
    {
        public const ushort
            BowShock = 1838;
    }

    public static class Levels
    {
        public const byte
            NoMercy = 2,
            BrutalShell = 4,
            DangerZone = 18,
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
            EnhancedContinuation = 86,
            CartridgeCharge2 = 88,
            DoubleDown = 90;
    }
}

internal class GunbreakerSolidBarrel : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerSolidBarrelCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == GNB.SolidBarrel)
        {
            var targetOftarget = GetTargetOfTarget();

            var noMercy = GetCooldown(GNB.NoMercy).CooldownRemaining;
            var gauge = GetJobGauge<GNBGauge>();

            if (GCDClipCheck(actionID))
            {
                if (level >= GNB.Levels.DangerZone
                    && (HasEffect(GNB.Buffs.NoMercy) || noMercy > GetCooldown(GNB.DangerZone).CooldownTotal * 0.2)
                    && IsOffCooldown(GNB.DangerZone))
                {
                    return GNB.DangerZone;
                }

                if (level >= GNB.Levels.BowShock
                    && IsOffCooldown(GNB.BowShock)
                    && (HasEffect(GNB.Buffs.NoMercy) || noMercy > GetCooldown(GNB.BowShock).CooldownTotal * 0.2))
                {
                    return GNB.BowShock;
                }

                if (level >= GNB.Levels.Aurora
                    && IsOffCooldown(GNB.Aurora)
                    && targetOftarget != null
                    && ((float)targetOftarget.CurrentHp / targetOftarget.MaxHp <= 0.8))
                {
                    return GNB.Aurora;
                }

                if (IsOffCooldown(GNB.NoMercy))
                {
                    return GNB.NoMercy;
                }
            }

            if (CanUseAction(GNB.SavageClaw)) return GNB.SavageClaw;

            if (CanUseAction(GNB.WickedTalon)) return GNB.WickedTalon;


            // Weaponskills
            if (level >= GNB.Levels.SonicBreak
                && (HasEffect(GNB.Buffs.NoMercy) || noMercy > GetCooldown(GNB.SonicBreak).CooldownTotal * 0.2)
                && IsOffCooldown(GNB.SonicBreak))
            {
                return GNB.SonicBreak;

            }

            if (level >= GNB.Levels.GnashingFang
                && IsOffCooldown(GNB.GnashingFang)
            && gauge.Ammo >= 1
                && (HasEffect(GNB.Buffs.NoMercy) || noMercy > GetCooldown(GNB.GnashingFang).CooldownTotal * 0.2))
            {
                return GNB.GnashingFang;

            }

            var maxAmmo = level >= GNB.Levels.CartridgeCharge2 ? 3 : 2;

            if (level >= GNB.Levels.BurstStrike
            && (gauge.Ammo >= maxAmmo
            || (gauge.Ammo >= 1
                    && HasEffect(GNB.Buffs.NoMercy))))
            {
                return GNB.BurstStrike;
            }

            // COMBO BLOCK
            if (comboTime > 0)
            {
                if (lastComboMove == GNB.BrutalShell
                    && level >= GNB.Levels.SolidBarrel)
                {
                    if (IsEnabled(CustomComboPreset.GunbreakerBurstStrikeCont))
                    {
                        if (level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast))
                            return GNB.Hypervelocity;
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

internal class GunbreakerGnashingFang : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerGnashingFangCont;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == GNB.GnashingFang)
        {
            if (level >= GNB.Levels.Continuation)
            {
                if (HasEffect(GNB.Buffs.ReadyToGouge))
                    return GNB.EyeGouge;

                if (HasEffect(GNB.Buffs.ReadyToTear))
                    return GNB.AbdomenTear;

                if (HasEffect(GNB.Buffs.ReadyToRip))
                    return GNB.JugularRip;
            }

            // Gnashing Fang > Savage Claw > Wicked Talon
            return OriginalHook(GNB.GnashingFang);
        }

        return actionID;
    }
}

internal class GunbreakerBurstStrikeFatedCircle : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GnbAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == GNB.BurstStrike)
        {
            if (IsEnabled(CustomComboPreset.GunbreakerBurstStrikeCont))
            {
                if (level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast))
                    return GNB.Hypervelocity;
            }
        }

        if (actionID == GNB.BurstStrike || actionID == GNB.FatedCircle)
        {
            var gauge = GetJobGauge<GNBGauge>();

            if (IsEnabled(CustomComboPreset.GunbreakerDoubleDownFeature))
            {
                if (level >= GNB.Levels.DoubleDown && gauge.Ammo >= 2 && IsOffCooldown(GNB.DoubleDown))
                    return GNB.DoubleDown;
            }

            if (IsEnabled(CustomComboPreset.GunbreakerEmptyBloodfestFeature))
            {
                if (level >= GNB.Levels.Bloodfest && gauge.Ammo == 0)
                    return GNB.Bloodfest;
            }
        }

        return actionID;
    }
}

internal class GunbreakerBowShockSonicBreak : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerBowShockSonicBreakFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == GNB.BowShock || actionID == GNB.SonicBreak)
        {
            if (level >= GNB.Levels.BowShock)
                return CalcBestAction(actionID, GNB.BowShock, GNB.SonicBreak);
        }

        return actionID;
    }
}

internal class GunbreakerDemonSlaughter : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerDemonSlaughterCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == GNB.DemonSlaughter)
        {
            var targetOftarget = GetTargetOfTarget();

            var noMercy = GetCooldown(GNB.NoMercy).CooldownRemaining;
            var gauge = GetJobGauge<GNBGauge>();

            if (GCDClipCheck(actionID))
            {
                if (level >= GNB.Levels.DangerZone
                    && (HasEffect(GNB.Buffs.NoMercy) || noMercy > GetCooldown(GNB.DangerZone).CooldownTotal * 0.2)
                    && IsOffCooldown(GNB.DangerZone))
                {
                    return GNB.DangerZone;
                }

                if (level >= GNB.Levels.BowShock
                    && IsOffCooldown(GNB.BowShock)
                    && (HasEffect(GNB.Buffs.NoMercy) || noMercy > GetCooldown(GNB.BowShock).CooldownTotal * 0.2))
                {
                    return GNB.BowShock;
                }

                if (level >= GNB.Levels.Aurora
                    && IsOffCooldown(GNB.Aurora)
                    && targetOftarget != null
                    && ((float)targetOftarget.CurrentHp / targetOftarget.MaxHp <= 0.8))
                {
                    return GNB.Aurora;
                }

                if (IsOffCooldown(GNB.NoMercy))
                {
                    return GNB.NoMercy;
                }
            }

            if (comboTime > 0 
                && lastComboMove == GNB.DemonSlice 
                && level >= GNB.Levels.DemonSlaughter)
            {
                var maxAmmo = level >= GNB.Levels.CartridgeCharge2 ? 3 : 2;

                if (level >= GNB.Levels.FatedCircle 
                    && gauge.Ammo >= 1)
                    return GNB.FatedCircle;

                return GNB.DemonSlaughter;
            }

            return GNB.DemonSlice;
        }

        return actionID;
    }
}

internal class GunbreakerNoMercy : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GnbAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == GNB.NoMercy)
        {
            var gauge = GetJobGauge<GNBGauge>();

            if (IsEnabled(CustomComboPreset.GunbreakerNoMercyDoubleDownFeature))
            {
                if (level >= GNB.Levels.NoMercy && HasEffect(GNB.Buffs.NoMercy))
                {
                    if (level >= GNB.Levels.DoubleDown && gauge.Ammo >= 2 && IsOffCooldown(GNB.DoubleDown))
                        return GNB.DoubleDown;
                }
            }

            if (IsEnabled(CustomComboPreset.GunbreakerNoMercyAlwaysDoubleDownFeature))
            {
                if (level >= GNB.Levels.NoMercy && HasEffect(GNB.Buffs.NoMercy))
                {
                    if (level >= GNB.Levels.DoubleDown)
                        return GNB.DoubleDown;
                }
            }

            if (IsEnabled(CustomComboPreset.GunbreakerNoMercyFeature))
            {
                if (level >= GNB.Levels.NoMercy && HasEffect(GNB.Buffs.NoMercy))
                {
                    if (level >= GNB.Levels.BowShock)
                        return CalcBestAction(GNB.SonicBreak, GNB.SonicBreak, GNB.BowShock);

                    if (level >= GNB.Levels.SonicBreak)
                        return GNB.SonicBreak;
                }
            }
        }

        return actionID;
    }
}
