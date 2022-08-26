using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class NIN
{
    public const byte ClassID = 29;
    public const byte JobID = 30;

    public const uint
        SpinningEdge = 2240,
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
        FleetingRaiju = 25778;

    public static class Buffs
    {
        public const ushort
            Mudra = 496,
            Kassatsu = 497,
            Doton = 501,
            Suiton = 507,
            Hidden = 614,
            TenChiJin = 1186,
            Bunshin = 1954,
            Meisui = 2689,
            RaijuReady = 2690;
    }

    public static class Debuffs
    {
        public const ushort
            TrickAttack = 3254;
    }

    public static class Levels
    {
        public const byte
            GustSlash = 4,
            Hide = 10,
            Mug = 15,
            TrickAttack = 18,
            AeolianEdge = 26,
            Ninjitsu = 30,
            ChiNormal = 35,
            Assassinate = 40,
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
            TenChiJin = 70,
            Meisui = 72,
            EnhancedKassatsu = 76,
            Bunshin = 80,
            PhantomKamaitachi = 82,
            HollowNozuchi = 86,
            Raiju = 90;
    }
}

internal class NinjaAeolianEdge : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.AeolianEdge)
        {
            var gauge = GetJobGauge<NINGauge>();

            var trickAttackCD = GetCooldown(NIN.TrickAttack).CooldownRemaining;

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

            var upcomingTrickAttack = trickAttackCD <= 10 || IsOffCooldown(NIN.TrickAttack);

            // Only execute this block if GCD is available and NOT if I'm doing a mudra or in TenChiJin
            if (GCDClipCheck(actionID)
                && !HasEffect(NIN.Buffs.TenChiJin)
                && !HasEffect(NIN.Buffs.Mudra)
                )
            {
                if (HasEffect(NIN.Buffs.Suiton)
                    && IsOffCooldown(NIN.TrickAttack))
                {
                    if (level >= NIN.Levels.Kassatsu
                        && IsOffCooldown(NIN.Kassatsu))
                    {
                        return NIN.Kassatsu;
                    }

                    if (level >= NIN.Levels.TrickAttack
                        && (HasEffect(NIN.Buffs.Kassatsu) || GetCooldown(NIN.Kassatsu).CooldownRemaining > 6 || level < NIN.Levels.Kassatsu)
                        && IsOnCooldown(NIN.Mug))
                    {
                        return NIN.TrickAttack;
                    }
                }

                if (level >= NIN.Levels.Bunshin
                    && IsOffCooldown(NIN.Bunshin)
                    && GetCooldown(NIN.TrickAttack).CooldownRemaining > 8
                    && gauge.Ninki >= 50)
                {
                    return NIN.Bunshin;
                }

                if (!HasEffect(NIN.Buffs.Kassatsu))
                {
                    if (level >= NIN.Levels.Meisui
                        && IsOffCooldown(NIN.Meisui)
                        && HasEffect(NIN.Buffs.Suiton)
                        && gauge.Ninki <= 50
                        && GetCooldown(NIN.TrickAttack).CooldownRemaining >= 18)
                        return NIN.Meisui;

                    if (level >= NIN.Levels.Assassinate
                        && IsOffCooldown(OriginalHook(NIN.Assassinate))
                        && (GetCooldown(NIN.TrickAttack).CooldownRemaining > 5 || level < NIN.Levels.Suiton))
                    {
                        return OriginalHook(NIN.Assassinate);
                    }

                    if (level >= NIN.Levels.HellfrogMedium
                        && ((gauge.Ninki >= 90)
                        || (gauge.Ninki >= 50 && (TargetHasEffect(NIN.Debuffs.TrickAttack) || HasEffect(NIN.Buffs.Meisui)))
                        || (IsOffCooldown(NIN.Mug)
                            && gauge.Ninki > 60
                            && level >= NIN.Levels.EnhancedMug)))
                    {
                        return (level >= NIN.Levels.Bhavacakra)
                                ? NIN.Bhavacakra
                                : NIN.HellfrogMedium;
                    }
                }
            }

            // Need to put before instant GCDs to not interrupot mudras.
            if (level >= NIN.Levels.Ninjitsu
                && (GetRemainingCharges(NIN.ChiNormal) >= 2
                    || (GetRemainingCharges(NIN.ChiNormal) >= 1
                        && (GetCooldown(NIN.ChiNormal).ChargeCooldownRemaining <= 6 || TargetHasEffect(NIN.Debuffs.TrickAttack)))
                    || (upcomingTrickAttack
                        && !HasEffect(NIN.Buffs.Suiton)
                        && GetRemainingCharges(NIN.ChiNormal) >= 1)
                    || (HasEffect(NIN.Buffs.Kassatsu) && IsOnCooldown(NIN.TrickAttack))
                    || HasEffect(NIN.Buffs.Mudra)))
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

                if (OriginalHook(NIN.Ninjutsu) == NIN.Fuma
                    && level >= NIN.Levels.ChiNormal)
                    return OriginalHook(NIN.ChiNormal);

                if (upcomingTrickAttack
                    && !HasEffect(NIN.Buffs.Suiton)
                    && level >= NIN.Levels.Suiton
                    && OriginalHook(NIN.Ninjutsu) == NIN.Raiton)
                {
                    return OriginalHook(NIN.JinNormal);
                }

                return OriginalHook(NIN.Ninjutsu);
            }

            if (level >= NIN.Levels.Raiju && HasEffect(NIN.Buffs.RaijuReady))
                return NIN.FleetingRaiju;


            if (level >= NIN.Levels.Huraijin && gauge.HutonTimer == 0)
                return NIN.Huraijin;

            if (level >= NIN.Levels.PhantomKamaitachi
                && OriginalHook(NIN.Bunshin) != NIN.Bunshin)
            {
                return OriginalHook(NIN.Bunshin);
            }

            if (comboTime > 0)
            {
                if (lastComboMove == NIN.GustSlash
                    && level >= NIN.Levels.ArmorCrush
                    && gauge.HutonTimer <= 30000)
                    return NIN.ArmorCrush;

                if (lastComboMove == NIN.GustSlash && level >= NIN.Levels.AeolianEdge)
                    return NIN.AeolianEdge;

                if (lastComboMove == NIN.SpinningEdge && level >= NIN.Levels.GustSlash)
                    return NIN.GustSlash;
            }

            return NIN.SpinningEdge;

        }

        return actionID;
    }
}

internal class NinjaArmorCrush : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.ArmorCrush)
        {
            if (IsEnabled(CustomComboPreset.NinjaArmorCrushRaijuFeature))
            {
                if (level >= NIN.Levels.Raiju && HasEffect(NIN.Buffs.RaijuReady))
                    return NIN.ForkedRaiju;
            }

            if (IsEnabled(CustomComboPreset.NinjaArmorCrushNinjutsuFeature))
            {
                if (level >= NIN.Levels.Ninjitsu && HasEffect(NIN.Buffs.Mudra))
                    return OriginalHook(NIN.Ninjutsu);
            }

            if (IsEnabled(CustomComboPreset.NinjaArmorCrushCombo))
            {
                if (comboTime > 0)
                {
                    if (lastComboMove == NIN.GustSlash && level >= NIN.Levels.ArmorCrush)
                        return NIN.ArmorCrush;

                    if (lastComboMove == NIN.SpinningEdge && level >= NIN.Levels.GustSlash)
                        return NIN.GustSlash;
                }

                return NIN.SpinningEdge;
            }
        }

        return actionID;
    }
}

internal class NinjaHuraijin : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.Huraijin)
        {
            if (level >= NIN.Levels.Raiju && HasEffect(NIN.Buffs.RaijuReady))
            {
                if (IsEnabled(CustomComboPreset.NinjaHuraijinForkedRaijuFeature))
                    return NIN.ForkedRaiju;

                if (IsEnabled(CustomComboPreset.NinjaHuraijinFleetingRaijuFeature))
                    return NIN.FleetingRaiju;
            }

            if (IsEnabled(CustomComboPreset.NinjaHuraijinNinjutsuFeature))
            {
                if (level >= NIN.Levels.Ninjitsu && HasEffect(NIN.Buffs.Mudra))
                    return OriginalHook(NIN.Ninjutsu);
            }

            if (IsEnabled(CustomComboPreset.NinjaHuraijinArmorCrushCombo))
            {
                var gauge = GetJobGauge<NINGauge>();

                if (comboTime > 0 && gauge.HutonTimer > 0)
                {
                    if (lastComboMove == NIN.GustSlash && level >= NIN.Levels.ArmorCrush)
                        return NIN.ArmorCrush;
                }
            }
        }

        return actionID;
    }
}

internal class NinjaHakkeMujinsatsu : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.HakkeMujinsatsu)
        {

            var gauge = GetJobGauge<NINGauge>();

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

            if (level >= NIN.Levels.Ninjitsu
                && !HasEffect(NIN.Buffs.TenChiJin)
                && (GetRemainingCharges(NIN.ChiNormal) >= 2
                    || (GetRemainingCharges(NIN.ChiNormal) >= 1
                        && GetCooldown(NIN.ChiNormal).ChargeCooldownRemaining <= 5)
                    || HasEffect(NIN.Buffs.Kassatsu)
                    || HasEffect(NIN.Buffs.Mudra)))
            {
                if (OriginalHook(NIN.Ninjutsu) == NIN.Ninjutsu)
                    return OriginalHook(NIN.JinNormal);

                if (OriginalHook(NIN.Ninjutsu) == NIN.Fuma)
                    return OriginalHook(NIN.TenNormal);

                //if (level >= NIN.Levels.HollowNozuchi 
                //    && !HasEffect(NIN.Buffs.Kassatsu) 
                //    && !IsMoving 
                //    && !HasEffect(NIN.Buffs.Doton)
                //    && OriginalHook(NIN.Ninjutsu) == NIN.Katon
                //    )
                //{
                //    return OriginalHook(NIN.ChiNormal);
                //}

                return OriginalHook(NIN.Ninjutsu);
            }


            //if (level >= NIN.Levels.Huton
            //    && (level < NIN.Levels.HakkeMujinsatsu || !InCombat())
            //    && gauge.HutonTimer <= 6000
            //    && (GetRemainingCharges(NIN.ChiNormal) >= 1
            //        || HasEffect(NIN.Buffs.Mudra)))
            //{
            //    if (OriginalHook(NIN.Ninjutsu) == NIN.Ninjutsu)
            //        return OriginalHook(NIN.JinNormal);

            //    if (OriginalHook(NIN.Ninjutsu) == NIN.Fuma)
            //        return OriginalHook(NIN.ChiNormal);

            //    if (OriginalHook(NIN.Ninjutsu) == NIN.Raiton)
            //        return OriginalHook(NIN.TenNormal);

            //    return OriginalHook(NIN.Ninjutsu);
            //}

            if (GCDClipCheck(actionID)
                && InCombat()
                && !HasEffect(NIN.Buffs.TenChiJin)
                && !HasEffect(NIN.Buffs.Mudra)
                )
            {

                if (level >= NIN.Levels.Kassatsu
                    && IsOffCooldown(NIN.Kassatsu))
                    return NIN.Kassatsu;

                if (level >= NIN.Levels.Bunshin
                    && IsOffCooldown(NIN.Bunshin)
                    && (gauge.Ninki >= 50))
                {
                    return NIN.Bunshin;
                }

                if (level >= NIN.Levels.HellfrogMedium
                    && gauge.Ninki >= 95)
                {
                    return NIN.HellfrogMedium;
                }

                if (level >= NIN.Levels.Assassinate
                    && IsOffCooldown(OriginalHook(NIN.Assassinate)))
                {
                    return OriginalHook(NIN.Assassinate);
                }
            }

            // FIX
            if (level >= NIN.Levels.PhantomKamaitachi
                && OriginalHook(NIN.Bunshin) != NIN.Bunshin)
            {
                return OriginalHook(NIN.Bunshin);
            }

            if (comboTime > 0)
            {
                if (lastComboMove == NIN.DeathBlossom && level >= NIN.Levels.HakkeMujinsatsu)
                    return NIN.HakkeMujinsatsu;
            }

            return NIN.DeathBlossom;

        }

        return actionID;
    }
}

internal class NinjaKassatsu : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaKassatsuTrickFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.Kassatsu)
        {
            if ((level >= NIN.Levels.Hide && HasEffect(NIN.Buffs.Hidden)) ||
                (level >= NIN.Levels.Suiton && HasEffect(NIN.Buffs.Suiton)))
                return NIN.TrickAttack;
        }

        return actionID;
    }
}

internal class NinjaHide : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaHideMugFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.Hide)
        {
            if (level >= NIN.Levels.Mug && InCombat())
                return NIN.Mug;
        }

        return actionID;
    }
}

internal class NinjaChi : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaKassatsuChiJinFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.ChiNormal)
        {
            if (level >= NIN.Levels.EnhancedKassatsu && HasEffect(NIN.Buffs.Kassatsu))
                return NIN.Jin;
        }

        return actionID;
    }
}

internal class NinjaTenChiJin : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaTCJMeisuiFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == NIN.TenChiJin)
        {
            if (level >= NIN.Levels.Meisui && HasEffect(NIN.Buffs.Suiton))
                return NIN.Meisui;
        }

        return actionID;
    }
}

internal class NinjaNinjitsu : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinAny;

    protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
    {
        if (actionID == NIN.Ninjutsu)
        {
            if (level >= NIN.Levels.Raiju && HasEffect(NIN.Buffs.RaijuReady) && !HasEffect(NIN.Buffs.Mudra))
            {
                if (IsEnabled(CustomComboPreset.NinjaNinjitsuForkedRaijuFeature))
                    return NIN.ForkedRaiju;

                if (IsEnabled(CustomComboPreset.NinjaNinjitsuFleetingRaijuFeature))
                    return NIN.FleetingRaiju;
            }
        }

        return actionID;
    }
}
