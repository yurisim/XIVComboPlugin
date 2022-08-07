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
        Assassinate = 8814,
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
        Raiton = 2267,
        Suiton = 2271,
        ArmorCrush = 3563,
        DreamWithinADream = 3566,
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
            Suiton = 507,
            Hidden = 614,
            Bunshin = 1954,
            RaijuReady = 2690;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
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
            Suiton = 45,
            HakkeMujinsatsu = 52,
            ArmorCrush = 54,
            DreamWithinADream = 56,
            Huraijin = 60,
            Bhavacakra = 68,
            TenChiJin = 70,
            Meisui = 72,
            EnhancedKassatsu = 76,
            Bunshin = 80,
            PhantomKamaitachi = 82,
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

            if (trickAttackCD <= 15 && !HasEffect(NIN.Buffs.Suiton) && level >= NIN.Levels.Suiton)
            {
                if (GetRemainingCharges(NIN.ChiNormal) >= 2)
                    return NIN.TenNormal;

                if (OriginalHook(NIN.Ninjutsu) == NIN.Fuma)
                    return NIN.Chi;

                if (OriginalHook(NIN.Ninjutsu) == NIN.Raiton)
                    return NIN.Jin;

                if (OriginalHook(NIN.Ninjutsu) == NIN.Suiton)
                    return OriginalHook(NIN.Ninjutsu);
            }
            else
            {
                if (GetRemainingCharges(NIN.ChiNormal) >= 2)
                    return NIN.TenNormal;

                if (OriginalHook(NIN.Ninjutsu) == NIN.Fuma)
                    return NIN.Chi;

                if (OriginalHook(NIN.Ninjutsu) == NIN.Raiton)
                    return OriginalHook(NIN.Ninjutsu);
            }

            if (IsEnabled(CustomComboPreset.NinjaAeolianEdgeRaijuFeature))
            {
                if (level >= NIN.Levels.Raiju && HasEffect(NIN.Buffs.RaijuReady))
                    return NIN.FleetingRaiju;
            }

            if (IsEnabled(CustomComboPreset.NinjaAeolianEdgeHutonFeature))
            {
                if (level >= NIN.Levels.Huraijin && gauge.HutonTimer == 0)
                    return NIN.Huraijin;

                if (comboTime > 0)
                {
                    if (lastComboMove == NIN.GustSlash && level >= NIN.Levels.ArmorCrush && gauge.HutonTimer <= 30_000)
                        return NIN.ArmorCrush;
                }
            }

            if (IsEnabled(CustomComboPreset.NinjaAeolianEdgeCombo))
            {
                if (level >= NIN.Levels.TrickAttack && CanUseAction(NIN.TrickAttack) && GCDClipCheck(actionID))
                    return NIN.TrickAttack;

                if (level >= NIN.Levels.Bhavacakra && gauge.Ninki >= 100 && GCDClipCheck(actionID))
                    return NIN.Bhavacakra;

                if (level >= NIN.Levels.DreamWithinADream && IsOffCooldown(NIN.DreamWithinADream) && GCDClipCheck(actionID))
                    return NIN.DreamWithinADream;

                if (comboTime > 0)
                {
                    if (lastComboMove == NIN.GustSlash && level >= NIN.Levels.AeolianEdge)
                        return NIN.AeolianEdge;

                    if (lastComboMove == NIN.SpinningEdge && level >= NIN.Levels.GustSlash)
                        return NIN.GustSlash;
                }

                return NIN.SpinningEdge;
            }
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
            if (IsEnabled(CustomComboPreset.NinjaHakkeMujinsatsuNinjutsuFeature))
            {
                if (level >= NIN.Levels.Ninjitsu && HasEffect(NIN.Buffs.Mudra))
                    return OriginalHook(NIN.Ninjutsu);
            }

            if (IsEnabled(CustomComboPreset.NinjaHakkeMujinsatsuCombo))
            {
                if (comboTime > 0)
                {
                    if (lastComboMove == NIN.DeathBlossom && level >= NIN.Levels.HakkeMujinsatsu)
                        return NIN.HakkeMujinsatsu;
                }

                return NIN.DeathBlossom;
            }
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
