using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class NIN
{
    public const byte ClassID = 29;
    public const byte JobID = 30;

    public const uint SpinningEdge = 2240,
        ThrowingDagger = 2247,
        GustSlash = 2242,
        Hide = 2245,
        Assassinate = 2246,
        Mug = 2248,
        DeathBlossom = 2254,
        AeolianEdge = 2255,
        TrickAttack = 2258,
        Ninjutsu = 2260,
        TenNormal = 2259,
        ChiNormal = 2261,
        JinNormal = 2263,
        Kassatsu = 2264,
        Shukuchi = 2262,
        Fuma = 2265,
        Katon = 2266,
        Raiton = 2267,
        Suiton = 2271,
        ArmorCrush = 3563,
        HellfrogMedium = 7401,
        Bhavacakra = 7402,
        TenChiJin = 7403,
        HakkeMujinsatsu = 16488,
        Meisui = 16489,
        Ten = 18805,
        Chi = 18806,
        Jin = 18807,
        Bunshin = 16493,
        Huraijin = 25876,
        PhantomKamaitachi = 25774,
        ForkedRaiju = 25777,
        FleetingRaiju = 25778,
        Dokumori = 36957;

    public static class Buffs
    {
        public const ushort Mudra = 496,
            Kassatsu = 497,
            Doton = 501,
            TenChiJin = 1186,
            Bunshin = 1954,
            ShadowWalker = 3848,
            Higi = 3850,
            Meisui = 2689,
            PhantomKamaitachi = 2723,
            RaijuReady = 2690;
    }

    public static class Debuffs
    {
        public const ushort Mug = 638,
            Dokumori = 3849,
            KunaisBane = 3906,
            TrickAttack = 3254;
    }

    public static class Levels
    {
        public const byte GustSlash = 4,
            Hide = 10,
            Mug = 15,
            TrickAttack = 18,
            AeolianEdge = 26,
            Ninjitsu = 30,
            ChiNormal = 35,
            Assassinate = 40,
            Shukuchi = 40,
            Suiton = 45,
            Huton = 45,
            Kassatsu = 50,
            HakkeMujinsatsu = 52,
            ArmorCrush = 54,
            DreamWithinADream = 56,
            Huraijin = 60,
            HellfrogMedium = 62,
            EnhancedMug = 66,
            Bhavacakra = 68,
            Dokumori = 66,
            TenChiJin = 70,
            Meisui = 72,
            EnhancedKassatsu = 76,
            Bunshin = 80,
            PhantomKamaitachi = 82,
            KunaisBane = 92,
            HollowNozuchi = 86,
            Raiju = 90;
    }
}

internal class NinjaAeolianEdge : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinAny;

    protected override unsafe uint Invoke(
        uint actionID,
        uint lastComboMove,
        float comboTime,
        byte level
    )
    {
        if (actionID == NIN.AeolianEdge || actionID == NIN.ArmorCrush)
        {
            var gauge = GetJobGauge<NINGauge>();

            // Ten Chi Jin block
            if (HasEffect(NIN.Buffs.TenChiJin))
            {
                if (OriginalHook(NIN.TenNormal) != NIN.TenNormal)
                {
                    return OriginalHook(NIN.TenNormal);
                }

                if (OriginalHook(NIN.ChiNormal) != NIN.ChiNormal)
                {
                    return OriginalHook(NIN.ChiNormal);
                }

                if (OriginalHook(NIN.JinNormal) != NIN.JinNormal)
                {
                    return OriginalHook(NIN.JinNormal);
                }
            }

            var trickThreshold = 15;

            var trickAttackCD = GetCooldown(OriginalHook(NIN.TrickAttack)).CooldownRemaining;

            var upcomingTrickAttack =
                trickAttackCD <= trickThreshold || IsOffCooldown(OriginalHook(NIN.TrickAttack));

            var ninki = gauge.Ninki;

            // Only execute this block if GCD is available and NOT if I'm doing a mudra or in TenChiJin
            if (
                GCDClipCheck(actionID)
                && OriginalHook(NIN.Ninjutsu) == NIN.Ninjutsu
                && !HasEffect(NIN.Buffs.Kassatsu)
                && !HasEffect(NIN.Buffs.TenChiJin)
            )
            {
                if (InMeleeRange())
                {
                    if (
                        level >= NIN.Levels.TrickAttack
                        && HasEffect(NIN.Buffs.ShadowWalker)
                        && IsOffCooldown(OriginalHook(NIN.TrickAttack))
                        && (GetCooldown(OriginalHook(NIN.Mug)).CooldownRemaining >= 10)
                    )
                    {
                        return OriginalHook(NIN.TrickAttack);
                    }
                }

                if (
                    level >= NIN.Levels.Kassatsu
                    && IsOffCooldown(NIN.Kassatsu)
                    && (
                        TargetHasEffect(NIN.Debuffs.TrickAttack)
                        || TargetHasEffect(NIN.Debuffs.KunaisBane)
                        || trickAttackCD >= 6
                    )
                )
                {
                    return NIN.Kassatsu;
                }

                if (
                    level >= NIN.Levels.Bunshin
                    && IsOffCooldown(NIN.Bunshin)
                    && (
                        TargetHasEffect(NIN.Debuffs.TrickAttack)
                        || TargetHasEffect(NIN.Debuffs.KunaisBane)
                        || trickAttackCD >= 9
                    )
                    && ninki >= 50
                )
                {
                    return NIN.Bunshin;
                }

                if (
                    level >= NIN.Levels.Meisui
                    && IsOffCooldown(NIN.Meisui)
                    && HasEffect(NIN.Buffs.ShadowWalker)
                    && ninki <= 50
                    && trickAttackCD >= 20
                )
                    return NIN.Meisui;

                if (
                    level >= NIN.Levels.Assassinate
                    && InMeleeRange()
                    && IsOffCooldown(OriginalHook(NIN.Assassinate))
                    && (trickAttackCD > 5 || level < NIN.Levels.Suiton)
                )
                {
                    return OriginalHook(NIN.Assassinate);
                }

                if (
                    level >= NIN.Levels.HellfrogMedium
                    && InMeleeRange()
                    && ninki >= 50
                    && (
                        ninki >= 80
                        || TargetHasEffect(NIN.Debuffs.TrickAttack)
                        || TargetHasEffect(NIN.Debuffs.KunaisBane)
                        || HasEffect(NIN.Buffs.Meisui)
                        || (
                            level >= NIN.Levels.EnhancedMug
                            && GetCooldown(NIN.Mug).CooldownRemaining <= 5
                        )
                    )
                )
                {
                    return (level >= NIN.Levels.Bhavacakra) ? NIN.Bhavacakra : NIN.HellfrogMedium;
                }
            }

            var phantom = FindEffect(NIN.Buffs.PhantomKamaitachi);

            var phantomTime = phantom is not null ? phantom.RemainingTime : 0;

            if (
                level >= NIN.Levels.PhantomKamaitachi
                && OriginalHook(NIN.Bunshin) != NIN.Bunshin
                && !HasEffect(NIN.Buffs.Mudra)
                && !HasEffect(NIN.Buffs.TenChiJin)
                && !HasEffect(NIN.Buffs.RaijuReady)
                && (
                    TargetHasEffect(NIN.Debuffs.TrickAttack)
                    || TargetHasEffect(NIN.Debuffs.KunaisBane)
                    || phantomTime <= 10
                    || GetTargetDistance() >= 9
                    || trickAttackCD >= phantomTime
                    || HasRaidBuffs()
                )
            )
            {
                return OriginalHook(NIN.Bunshin);
            }

            // Need to put before instant GCDs to not interrupot mudras.

            var startMudra =
                TargetHasEffect(NIN.Debuffs.TrickAttack)
                || TargetHasEffect(NIN.Debuffs.KunaisBane)
                || (
                    (TargetHasEffect(NIN.Debuffs.Mug) || TargetHasEffect(NIN.Debuffs.Dokumori))
                    && IsOnCooldown(OriginalHook(NIN.TrickAttack))
                );

            var continueMudra = HasCharges(NIN.ChiNormal);

            if (
                level >= NIN.Levels.Ninjitsu
                && (
                    OriginalHook(NIN.Ninjutsu) != NIN.Ninjutsu
                    || HasEffect(NIN.Buffs.Kassatsu)
                    || GetRemainingCharges(NIN.ChiNormal) == 2
                    || (
                        continueMudra
                        && (startMudra || upcomingTrickAttack && !HasEffect(NIN.Buffs.ShadowWalker))
                    )
                )
            )
            {
                if (HasEffect(NIN.Buffs.Kassatsu) && level >= NIN.Levels.EnhancedKassatsu)
                {
                    if (OriginalHook(NIN.Ninjutsu) == NIN.Ninjutsu)
                        return OriginalHook(NIN.ChiNormal);
                    if (OriginalHook(NIN.Ninjutsu) == NIN.Fuma)
                        return OriginalHook(NIN.JinNormal);
                    return OriginalHook(NIN.Ninjutsu);
                }

                if (OriginalHook(NIN.Ninjutsu) == NIN.Ninjutsu)
                    return OriginalHook(NIN.TenNormal);

                if (OriginalHook(NIN.Ninjutsu) == NIN.Fuma && level >= NIN.Levels.ChiNormal)
                    return OriginalHook(NIN.ChiNormal);

                if (
                    upcomingTrickAttack
                    && !HasEffect(NIN.Buffs.ShadowWalker)
                    && level >= NIN.Levels.Suiton
                    && OriginalHook(NIN.Ninjutsu) == NIN.Raiton
                )
                    return OriginalHook(NIN.JinNormal);

                return OriginalHook(NIN.Ninjutsu);
            }

            if (level >= NIN.Levels.Raiju && HasEffect(NIN.Buffs.RaijuReady))
                return NIN.FleetingRaiju;

            if (GetTargetDistance() >= 9)
            {
                return NIN.ThrowingDagger;
            }

            if (comboTime > 0)
            {
                if (lastComboMove == NIN.GustSlash && level >= NIN.Levels.AeolianEdge)
                    return actionID;

                if (lastComboMove == NIN.SpinningEdge && level >= NIN.Levels.GustSlash)
                    return NIN.GustSlash;
            }

            return NIN.SpinningEdge;
        }

        return actionID;
    }
}

internal class TenChiJin : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.TenChiJin)
        {
            if (HasEffect(NIN.Buffs.TenChiJin))
            {
                if (OriginalHook(NIN.TenNormal) != NIN.TenNormal)
                {
                    return OriginalHook(NIN.TenNormal);
                }

                if (OriginalHook(NIN.ChiNormal) != NIN.ChiNormal)
                {
                    return OriginalHook(NIN.ChiNormal);
                }

                if (OriginalHook(NIN.JinNormal) != NIN.JinNormal)
                {
                    return OriginalHook(NIN.JinNormal);
                }
            }

            return NIN.TenChiJin;
        }

        return actionID;
    }
}

internal class NinjaHakkeMujinsatsu : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinAny;

    protected override unsafe uint Invoke(
        uint actionID,
        uint lastComboMove,
        float comboTime,
        byte level
    )
    {
        if (actionID == NIN.DeathBlossom)
        {
            var gauge = GetJobGauge<NINGauge>();

            var ninki = gauge.Ninki;

            // var gauge = GetJobGauge<NINGauge>();

            if (HasEffect(NIN.Buffs.TenChiJin))
            {
                if (OriginalHook(NIN.JinNormal) != NIN.JinNormal)
                {
                    return OriginalHook(NIN.JinNormal);
                }

                if (OriginalHook(NIN.TenNormal) != NIN.TenNormal)
                {
                    return OriginalHook(NIN.TenNormal);
                }

                if (OriginalHook(NIN.ChiNormal) != NIN.ChiNormal)
                {
                    return OriginalHook(NIN.ChiNormal);
                }
            }

            var trickThreshold = 15;

            var trickAttackCD = GetCooldown(OriginalHook(NIN.TrickAttack)).CooldownRemaining;

            var upcomingTrickAttack =
                trickAttackCD <= trickThreshold || IsOffCooldown(OriginalHook(NIN.TrickAttack));

            bool CanUseNinjutsu()
            {
                return (level < NIN.Levels.KunaisBane || !CanUseKunai())
                    && (
                        OriginalHook(NIN.Ninjutsu) != NIN.Ninjutsu
                        || HasEffect(NIN.Buffs.Kassatsu)
                        || HasCharges(NIN.ChiNormal)
                        || HasEffect(NIN.Buffs.Mudra)
                    );
            }

            bool CanUseKunai()
            {
                return IsOffCooldown(OriginalHook(NIN.TrickAttack))
                    && HasEffect(NIN.Buffs.ShadowWalker);
            }

            bool ShouldUseChi()
            {
                return upcomingTrickAttack
                    && !HasEffect(NIN.Buffs.Kassatsu)
                    && !HasEffect(NIN.Buffs.ShadowWalker)
                    && OriginalHook(NIN.Ninjutsu) == NIN.Fuma;
            }

            bool ShouldUseTen()
            {
                return OriginalHook(NIN.Ninjutsu) == NIN.Fuma
                    || OriginalHook(NIN.Ninjutsu) == NIN.Raiton;
            }

            if (level >= NIN.Levels.Ninjitsu && CanUseNinjutsu())
            {
                if (OriginalHook(NIN.Ninjutsu) == NIN.Ninjutsu)
                {
                    return OriginalHook(NIN.JinNormal);
                }

                if (ShouldUseChi())
                {
                    return OriginalHook(NIN.ChiNormal);
                }

                if (ShouldUseTen())
                {
                    return OriginalHook(NIN.TenNormal);
                }

                return OriginalHook(NIN.Ninjutsu);
            }

            if (
                GCDClipCheck(actionID)
                && InCombat()
                && !HasEffect(NIN.Buffs.TenChiJin)
                && !HasEffect(NIN.Buffs.Mudra)
            )
            {
                if (level >= NIN.Levels.KunaisBane)
                {
                    if (
                        level >= NIN.Levels.TrickAttack
                        && HasEffect(NIN.Buffs.ShadowWalker)
                        && IsOffCooldown(OriginalHook(NIN.TrickAttack))
                    )
                    {
                        return OriginalHook(NIN.TrickAttack);
                    }
                }

                if (
                    level >= NIN.Levels.Kassatsu
                    && IsOffCooldown(NIN.Kassatsu)
                    && (level < NIN.Levels.KunaisBane || TargetHasEffect(NIN.Debuffs.KunaisBane))
                )
                    return NIN.Kassatsu;

                if (level >= NIN.Levels.Bunshin && IsOffCooldown(NIN.Bunshin) && (ninki >= 50))
                {
                    return NIN.Bunshin;
                }

                if (level >= NIN.Levels.HellfrogMedium && ninki >= 95)
                {
                    return NIN.HellfrogMedium;
                }

                if (level >= NIN.Levels.Assassinate && IsOffCooldown(OriginalHook(NIN.Assassinate)))
                {
                    return OriginalHook(NIN.Assassinate);
                }
            }

            if (level >= NIN.Levels.PhantomKamaitachi && OriginalHook(NIN.Bunshin) != NIN.Bunshin)
            {
                return OriginalHook(NIN.Bunshin);
            }

            if (comboTime > 0)
            {
                if (lastComboMove == NIN.DeathBlossom && level >= NIN.Levels.HakkeMujinsatsu)
                    return NIN.HakkeMujinsatsu;
            }
        }

        return actionID;
    }
}

internal class NinjaHide : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.Shukuchi)
        {
            if (level >= NIN.Levels.Raiju && HasEffect(NIN.Buffs.RaijuReady))
                return NIN.ForkedRaiju;

            if (
                level >= NIN.Levels.Shukuchi
                && (IsOffCooldown(NIN.Shukuchi) || HasCharges(NIN.Shukuchi))
            )
                return NIN.Shukuchi;
        }

        return actionID;
    }
}