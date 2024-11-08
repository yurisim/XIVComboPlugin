using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class RDM
{
    public const byte JobID = 35;

    public const uint Jolt = 7503,
        Riposte = 7504,
        Verthunder = 7505,
        Veraero = 7507,
        Scatter = 7509,
        Verfire = 7510,
        Verstone = 7511,
        Zwerchhau = 7512,
        Reprise = 16529,
        Moulinet = 7513,
        Vercure = 7514,
        Redoublement = 7516,
        Fleche = 7517,
        Acceleration = 7518,
        ContreSixte = 7519,
        Embolden = 7520,
        Manafication = 7521,
        Verraise = 7523,
        Jolt2 = 7524,
        Verflare = 7525,
        Verholy = 7526,
        EnchantedRiposte = 7527,
        EnchantedZwerchhau = 7528,
        EnchantedRedoublement = 7529,
        Verthunder2 = 16524,
        Veraero2 = 16525,
        Impact = 16526,
        Engagement = 16527,
        Scorch = 16530,
        Verthunder3 = 25855,
        Veraero3 = 25856,
        Resolution = 25858,
        ViceOfThorns = 37005,
        GrandImpact = 37006,
        Prefulgence = 37007;

    public static class Buffs
    {
        public const ushort Swiftcast = 167,
            VerfireReady = 1234,
            VerstoneReady = 1235,
            Acceleration = 1238,
            Embolden = 1239,
            EmboldenParty = 1297,
            Dualcast = 1249,
            LostChainspell = 2560,
            ThornedFlourish = 3876,
            GrandImpactReady = 3877,
            MagickedSwordPlay = 3875,
            PrefulgenceReady = 3878;
    }

    public static class Debuffs
    {
        public const ushort Placeholder = 0;
    }

    public static class Levels
    {
        public const byte Jolt = 2,
            Verthunder = 4,
            Veraero = 10,
            Scatter = 15,
            Verthunder2 = 18,
            Veraero2 = 22,
            Zwerchhau = 35,
            Engagement = 40,
            Fleche = 45,
            Redoublement = 50,
            Acceleration = 50,
            Moulinent = 52,
            Vercure = 54,
            ContreSixte = 56,
            Embolden = 58,
            Manafication = 60,
            Jolt2 = 62,
            Verraise = 64,
            Impact = 66,
            Verflare = 68,
            Verholy = 70,
            Reprise = 76,
            Scorch = 80,
            Veraero3 = 82,
            Verthunder3 = 82,
            Resolution = 90,
            ViceOfThorns = 92,
            GrandImpact = 96,
            Prefulgence = 100;
    }
}

internal class RedMageVeraeroVerthunder : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RdmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID is RDM.Jolt or RDM.Scatter)
        {
            var gauge = GetJobGauge<RDMGauge>();

            var raidBuffs = HasRaidBuffs(2);

            var mpThreshold = 9300;

            var controlBurst =
                LocalPlayer?.CurrentMp <= mpThreshold || IsOnCooldown(ADV.LucidDreaming);

            var embolden = HasEffect(RDM.Buffs.Embolden);

            // manafication
            if (
                level >= RDM.Levels.Manafication
                && gauge.ManaStacks < 1
                && IsOnCooldown(RDM.Embolden)
                && IsOffCooldown(RDM.Manafication)
            )
                return RDM.Manafication;

            if (GCDClipCheck(actionID))
            {
                switch (level)
                {
                    case >= RDM.Levels.Embolden when IsOffCooldown(RDM.Embolden) && raidBuffs:
                        return RDM.Embolden;
                    case >= RDM.Levels.Fleche
                        when IsOffCooldown(RDM.Fleche)
                            && (
                                level < RDM.Levels.Embolden || controlBurst || embolden || raidBuffs
                            ):
                        return RDM.Fleche;
                    case >= RDM.Levels.ContreSixte
                        when IsOffCooldown(RDM.ContreSixte)
                            && (
                                level < RDM.Levels.Embolden || controlBurst || embolden || raidBuffs
                            ):
                        return RDM.ContreSixte;
                    case >= RDM.Levels.Engagement
                        when InMeleeRange()
                            && HasCharges(RDM.Engagement)
                            && (
                                GetCooldown(RDM.Engagement).CooldownRemaining < 6
                                || HasEffect(RDM.Buffs.Embolden)
                            ):
                        return RDM.Engagement;
                    case >= RDM.Levels.Acceleration
                        when IsOffCooldown(RDM.Acceleration)
                            && (controlBurst || embolden || raidBuffs):
                        return RDM.Acceleration;
                    case >= ADV.Levels.LucidDreaming
                        when IsOffCooldown(ADV.LucidDreaming) && LocalPlayer?.CurrentMp <= 8500:
                        return ADV.LucidDreaming;
                }
            }

            if (level >= RDM.Levels.Scorch && lastComboMove is RDM.Verflare or RDM.Verholy)
                return OriginalHook(RDM.Scorch);

            if (gauge.ManaStacks == 3)
                return gauge.BlackMana < gauge.WhiteMana || level < RDM.Levels.Verholy
                    ? OriginalHook(RDM.Verflare)
                    : OriginalHook(RDM.Verholy);

            var hasSpeedy =
                HasEffect(RDM.Buffs.Dualcast)
                || HasEffect(RDM.Buffs.Acceleration)
                || HasEffect(ADV.Buffs.Swiftcast);

            var swordPlay = FindEffect(RDM.Buffs.MagickedSwordPlay);

            var swordPlayStacks = swordPlay is not null ? swordPlay.StackCount : 0;

            var actualBlack = gauge.BlackMana + swordPlayStacks * 20;
            var actualWhite = gauge.WhiteMana + swordPlayStacks * 20;

            var minimiumGauge =
                20
                + (level >= RDM.Levels.Zwerchhau ? 15 : 0)
                + (level >= RDM.Levels.Redoublement ? 15 : 0);

            var needToReprise = gauge.WhiteMana >= 80 && gauge.BlackMana >= 80 && !hasSpeedy;

            var startMeleeCombo =
                (
                    actualWhite >= minimiumGauge
                    && actualBlack >= minimiumGauge
                    && (HasEffect(RDM.Buffs.Embolden) || raidBuffs)
                ) || needToReprise;

            if (
                level >= RDM.Levels.Reprise
                && actionID is not RDM.Scatter
                && needToReprise
                && !InMeleeRange()
            )
            {
                return OriginalHook(RDM.Reprise);
            }

            if (
                InMeleeRange()
                && (
                    gauge.ManaStacks >= 1 && gauge.ManaStacks < 3
                    || (
                        (lastComboMove is RDM.EnchantedRiposte or RDM.Riposte)
                        && (level >= RDM.Levels.Zwerchhau)
                    )
                    || (
                        (lastComboMove is RDM.EnchantedZwerchhau or RDM.Zwerchhau)
                        && (level >= RDM.Levels.Redoublement)
                    )
                    || (
                        startMeleeCombo
                        && (actionID is not RDM.Scatter || level >= RDM.Levels.Moulinent)
                    )
                )
            )
            {
                if (
                    (lastComboMove == RDM.Zwerchhau || lastComboMove == RDM.EnchantedZwerchhau)
                    && OriginalHook(RDM.Redoublement) != RDM.Redoublement
                    && level >= RDM.Levels.Redoublement
                )
                    // Enchanted
                    return OriginalHook(RDM.Redoublement);

                if (
                    (lastComboMove == RDM.Riposte || lastComboMove == RDM.EnchantedRiposte)
                    && level >= RDM.Levels.Zwerchhau
                    && OriginalHook(RDM.Zwerchhau) != RDM.Zwerchhau
                )
                {
                    return OriginalHook(RDM.Zwerchhau);
                }

                if (actionID is RDM.Scatter)
                {
                    return OriginalHook(RDM.Moulinet);
                }

                if (OriginalHook(RDM.Riposte) != RDM.Riposte)
                {
                    return OriginalHook(RDM.Riposte);
                }
            }

            // Dualcast
            if (hasSpeedy && actionID == RDM.Jolt)
                return gauge.WhiteMana + 7 < gauge.BlackMana
                    ? OriginalHook(RDM.Veraero)
                    : OriginalHook(RDM.Verthunder);

            if (hasSpeedy && actionID == RDM.Scatter)
                return RDM.Scatter;

            if (actionID == RDM.Scatter && level >= RDM.Levels.Verthunder2)
                return gauge.BlackMana < gauge.WhiteMana + 7 || level < RDM.Levels.Veraero2
                    ? OriginalHook(RDM.Verthunder2)
                    : OriginalHook(RDM.Veraero2);

            // Procs
            if (HasEffect(RDM.Buffs.VerfireReady) && actionID is RDM.Jolt)
                return RDM.Verfire;

            if (HasEffect(RDM.Buffs.VerstoneReady) && actionID is RDM.Jolt)
                return RDM.Verstone;

            return OriginalHook(RDM.Jolt);
        }

        return actionID;
    }
}

// internal class RedMageVeraeroVerthunder2 : CustomCombo
// {
//     protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RdmAny;

//     protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//     {
//         if (actionID == RDM.Scatter)
//         {
//             var gauge = GetJobGauge<RDMGauge>();


//             var hasSpeedy =
//                 HasEffect(RDM.Buffs.Dualcast)
//                 || HasEffect(RDM.Buffs.Acceleration)
//                 || HasEffect(ADV.Buffs.Swiftcast);

//             if (
//                     level >= RDM.Levels.Moulinent
//                     && (
//                         (gauge.ManaStacks >= 1 && gauge.ManaStacks < 3)
//                         || (
//                             gauge.WhiteMana >= 60
//                             && gauge.BlackMana >= 60
//                             && HasEffect(RDM.Buffs.Embolden)
//                         )
//                         || (gauge.WhiteMana >= 80 && gauge.BlackMana >= 80 && !hasSpeedy)
//                     )
//                 )
//                 // Enchanted
//                 return OriginalHook(RDM.Moulinet);

//             // Dualcast
//             if (hasSpeedy) return RDM.Scatter;

//             if (level >= RDM.Levels.Verthunder2)
//                 return gauge.BlackMana <= gauge.WhiteMana || level < RDM.Levels.Veraero2
//                     ? OriginalHook(RDM.Verthunder2)
//                     : OriginalHook(RDM.Veraero2);
//         }

//         return actionID;
//     }
