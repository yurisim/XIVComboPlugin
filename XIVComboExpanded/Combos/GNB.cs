using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class GNB
    {
        public const byte JobID = 37;

        public const uint
            KeenEdge = 16137,
            NoMercy = 16138,
            BrutalShell = 16139,
            DemonSlice = 16141,
            SolidBarrel = 16145,
            GnashingFang = 16146,
            DemonSlaughter = 16149,
            SonicBreak = 16153,
            Continuation = 16155,
            JugularRip = 16156,
            AbdomenTear = 16157,
            EyeGouge = 16158,
            BowShock = 16159,
            BurstStrike = 16162,
            FatedCircle = 16163,
            Bloodfest = 16164,
            DoubleDown = 25760,
            Hypervelocity = 25759;

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
                BrutalShell = 4,
                SolidBarrel = 26,
                DemonSlaughter = 40,
                SonicBreak = 54,
                BowShock = 62,
                Continuation = 70,
                FatedCircle = 72,
                Bloodfest = 76,
                EnhancedContinuation = 86,
                CartridgeCharge2 = 88,
                DoubleDown = 90;
        }
    }

    internal class GunbreakerSolidBarrelCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.GunbreakerSolidBarrelCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.SolidBarrel)
            {
                if (comboTime > 0)
                {
                    if (lastComboMove == GNB.KeenEdge && level >= GNB.Levels.BrutalShell)
                        return GNB.BrutalShell;

                    if (lastComboMove == GNB.BrutalShell && level >= GNB.Levels.SolidBarrel)
                        return GNB.SolidBarrel;
                }

                return GNB.KeenEdge;
            }

            return actionID;
        }
    }

    internal class GunbreakerGnashingFangContinuation : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.GunbreakerGnashingFangCont;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.GnashingFang)
            {
                if (level >= GNB.Levels.Continuation)
                {
                    if (HasEffect(GNB.Buffs.ReadyToRip))
                        return GNB.JugularRip;

                    if (HasEffect(GNB.Buffs.ReadyToTear))
                        return GNB.AbdomenTear;

                    if (HasEffect(GNB.Buffs.ReadyToGouge))
                        return GNB.EyeGouge;
                }

                // Gnashing Fang > Savage Claw > Wicked Talon
                return OriginalHook(GNB.GnashingFang);
            }

            return actionID;
        }
    }

    internal class GunbreakerBurstStrikeContinuation : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.GunbreakerBurstStrikeCont;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.BurstStrike)
            {
                if (level >= GNB.Levels.EnhancedContinuation && HasEffect(GNB.Buffs.ReadyToBlast))
                    return GNB.Hypervelocity;
            }

            return actionID;
        }
    }

    internal class GunbreakerBowShockSonicBreakFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.GunbreakerBowShockSonicBreakFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.BowShock || actionID == GNB.SonicBreak)
            {
                var bowCd = GetCooldown(GNB.BowShock);
                var sonicCd = GetCooldown(GNB.SonicBreak);

                // Prioritize the original if both are off cooldown
                if (!bowCd.IsCooldown && !sonicCd.IsCooldown)
                    return actionID;

                return bowCd.CooldownRemaining < sonicCd.CooldownRemaining
                    ? GNB.BowShock
                    : GNB.SonicBreak;
            }

            return actionID;
        }
    }

    internal class GunbreakerDemonSlaughterCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.GunbreakerDemonSlaughterCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == GNB.DemonSlaughter)
            {
                if (comboTime > 0 && lastComboMove == GNB.DemonSlice && level >= GNB.Levels.DemonSlaughter)
                {
                    if (IsEnabled(CustomComboPreset.GunbreakerFatedCircleFeature) && level >= GNB.Levels.FatedCircle)
                    {
                        var gauge = GetJobGauge<GNBGauge>();
                        var maxAmmo = level >= GNB.Levels.CartridgeCharge2 ? 3 : 2;

                        if (gauge.Ammo == maxAmmo)
                            return GNB.FatedCircle;
                    }

                    return GNB.DemonSlaughter;
                }

                return GNB.DemonSlice;
            }

            return actionID;
        }
    }

    // internal class GunbreakerBloodfestOvercapFeature : CustomCombo
    // {
    //     protected override CustomComboPreset Preset => CustomComboPreset.GunbreakerBloodfestOvercapFeature;
    //
    //     protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    //     {
    //         if (actionID == GNB.BurstStrike)
    //         {
    //             var gauge = GetJobGauge<GNBGauge>();
    //             if (gauge.NumAmmo == 0 && level >= GNB.Levels.Bloodfest)
    //                 return GNB.Bloodfest;
    //         }
    //
    //         return actionID;
    //     }
    // }

    // internal class GunbreakerNoMercyFeature : CustomCombo
    // {
    //     protected override CustomComboPreset Preset => CustomComboPreset.GunbreakerNoMercyFeature;
    //
    //     protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    //     {
    //         if (actionID == GNB.NoMercy)
    //         {
    //             if (HasEffect(GNB.Buffs.NoMercy))
    //             {
    //                 if (level >= GNB.Levels.BowShock && !TargetHasEffect(GNB.Debuffs.BowShock))
    //                     return GNB.BowShock;
    //
    //                 if (level >= GNB.Levels.SonicBreak)
    //                     return GNB.SonicBreak;
    //             }
    //         }
    //
    //         return actionID;
    //     }
    // }
}
