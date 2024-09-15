using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class SAM
{
    public const byte JobID = 34;

    public const uint
        // Single target
        Hakaze = 7477,
        Jinpu = 7478,
        Shifu = 7479,
        Yukikaze = 7480,
        Gekko = 7481,
        Kasha = 7482,
        Enpi = 7486,
        MidareSetsugekka = 7487,
        Higanbana = 7489,
        KaeshiSetsugekka = 16486,
        Gyofu = 36963,
        // AoE
        Fuga = 7483,
        Mangetsu = 7484,
        Oka = 7485,
        TenkaGoken = 7488,
        Fuko = 25780,
        // Iaijutsu and Tsubame
        Iaijutsu = 7867,
        TsubameGaeshi = 16483,
        KaeshiHiganbana = 16484,
        Shoha = 16487,
        // Misc
        MeikyoShisui = 7499,
        HissatsuShinten = 7490,
        HissatsuKyuten = 7491,
        HissatsuSenei = 16481,
        HissatsuGuren = 7496,
        Ikishoten = 16482,
        // Shoha2 = 25779,
        OgiNamikiri = 25781,
        KaeshiNamikiri = 25782,
        Zanshin = 36964;

    public static class Buffs
    {
        public const ushort MeikyoShisui = 1233,
            EyesOpen = 1252,
            Jinpu = 1298,
            Shifu = 1299,
            OgiNamikiriReady = 2959,
            ZanshinReady = 3855;
    }

    public static class Debuffs
    {
        public const ushort Higanabana = 1228;
    }

    public static class Levels
    {
        public const byte Jinpu = 4,
            Shifu = 18,
            Gekko = 30,
            Higanbana = 30,
            Mangetsu = 35,
            Kasha = 40,
            TenkaGoken = 40,
            Oka = 45,
            Yukikaze = 50,
            MidareSetsugekka = 50,
            MeikyoShisui = 50,
            HissatsuShinten = 52,
            HissatsuKyuten = 64,
            Ikishoten = 68,
            HissatsuGuren = 70,
            HissatsuSenei = 72,
            TsubameGaeshi = 76,
            Shoha = 80,
            // Shoha2 = 82,
            Hyosetsu = 86,
            Fuko = 86,
            DoubleMeikyoShisui = 88,
            OgiNamikiri = 90,
            Zanshin = 96;
    }
}

internal class SamuraiYukikaze : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SAM.Hakaze)
        {
            var gauge = GetJobGauge<SAMGauge>();

            var gaugeSen = new[]
            {
                gauge.HasSetsu ? 1 : 0,
                gauge.HasKa ? 1 : 0,
                gauge.HasGetsu ? 1 : 0
            };

            var higanabana = FindTargetEffect(SAM.Debuffs.Higanabana);

            var higanbanaTime =
                (higanabana is null && ShouldRefreshDots())
                || (higanabana is not null && higanabana.RemainingTime <= 6);

            if (GCDClipCheck(actionID))
            {
                var canUseIkishoten =
                    level >= SAM.Levels.Ikishoten && IsOffCooldown(SAM.Ikishoten) && InCombat();

                if (
                    level >= SAM.Levels.MeikyoShisui
                    && (HasCharges(SAM.MeikyoShisui) || IsOffCooldown(SAM.MeikyoShisui))
                    && (
                        HasRaidBuffs()
                        || level < SAM.Levels.DoubleMeikyoShisui
                        || GetCooldown(SAM.MeikyoShisui).CooldownRemaining <= 5
                    )
                )
                    return SAM.MeikyoShisui;

                if (canUseIkishoten && HasRaidBuffs() && gauge.Kenki <= 50) return SAM.Ikishoten;

                if (
                    level >= SAM.Levels.HissatsuGuren
                    && IsOffCooldown(SAM.HissatsuGuren)
                    && IsOnCooldown(SAM.Ikishoten)
                    && gauge.Kenki >= 25
                    && HasEffect(SAM.Buffs.Jinpu)
                )
                    return level >= SAM.Levels.HissatsuSenei
                        ? SAM.HissatsuSenei
                        : SAM.HissatsuGuren;

                if (
                    level >= SAM.Levels.HissatsuShinten
                    && gauge.Kenki >= 25
                    && (
                        gauge.Kenki >= 75
                        || (
                            (canUseIkishoten || GetCooldown(SAM.Ikishoten).CooldownRemaining <= 5)
                            && gauge.Kenki >= 35
                        )
                        || (
                            HasRaidBuffs()
                            && (
                                GetCooldown(SAM.HissatsuGuren).CooldownRemaining >= 15
                                || level < SAM.Levels.HissatsuGuren
                            )
                        )
                    )
                    && HasEffect(SAM.Buffs.Jinpu)
                )
                    return SAM.HissatsuShinten;
            }

            if (OriginalHook(SAM.OgiNamikiri) != SAM.OgiNamikiri) return OriginalHook(SAM.OgiNamikiri);

            if (
                gauge.MeditationStacks == 3
                && (higanbanaTime || HasRaidBuffs() || gaugeSen.Sum() == 3)
            )
                return SAM.Shoha;

            if (
                level >= SAM.Levels.TsubameGaeshi
                && HasCharges(SAM.TsubameGaeshi)
                && (GetCooldown(SAM.TsubameGaeshi).CooldownRemaining <= 15 || HasRaidBuffs())
                && HasEffect(SAM.Buffs.Jinpu)
                && OriginalHook(SAM.TsubameGaeshi) == SAM.KaeshiSetsugekka
            )
                return OriginalHook(SAM.TsubameGaeshi);

            var ogiBuff = FindEffect(SAM.Buffs.OgiNamikiriReady);

            if (level >= SAM.Levels.OgiNamikiri && ogiBuff is not null) return SAM.OgiNamikiri;

            if (gauge.MeditationStacks != 3)
            {
                if (
                    level >= SAM.Levels.Higanbana
                    && !HasEffect(SAM.Buffs.MeikyoShisui)
                    && HasEffect(SAM.Buffs.Jinpu)
                    && HasEffect(SAM.Buffs.Shifu)
                    && gaugeSen.Sum() == 1
                    && (
                        (higanabana is null && ShouldRefreshDots())
                        || (higanabana is not null && higanabana.RemainingTime <= 6)
                    )
                )
                    return OriginalHook(SAM.Iaijutsu);

                if (level >= SAM.Levels.MidareSetsugekka && gaugeSen.Sum() == 3) return OriginalHook(SAM.Iaijutsu);
            }

            if (level >= SAM.Levels.MeikyoShisui && HasEffect(SAM.Buffs.MeikyoShisui))
            {
                if (!gauge.HasGetsu)
                    return SAM.Gekko;
                if (!gauge.HasKa)
                    return SAM.Kasha;
                if (!gauge.HasSetsu)
                    return SAM.Yukikaze;
            }

            // Does not matter
            if (!gauge.HasSetsu)
                return lastComboMove == SAM.Hakaze && level >= SAM.Levels.Yukikaze
                    ? SAM.Yukikaze
                    : SAM.Hakaze;

            // Rear
            if (!gauge.HasGetsu || (!HasEffect(SAM.Buffs.Jinpu) && level >= SAM.Levels.Jinpu))
            {
                if (lastComboMove == SAM.Jinpu && level >= SAM.Levels.Gekko)
                    return SAM.Gekko;

                if (lastComboMove == SAM.Hakaze)
                    return SAM.Jinpu;
            }

            // Flank
            if (!gauge.HasKa || (!HasEffect(SAM.Buffs.Shifu) && level >= SAM.Levels.Shifu))
            {
                if (lastComboMove == SAM.Shifu && level >= SAM.Levels.Kasha)
                    return SAM.Kasha;

                if (lastComboMove == SAM.Hakaze)
                    return SAM.Shifu;
            }
        }

        return actionID;
    }
}

internal class SamuraiMangetsu : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SAM.Fuga || actionID == SAM.Fuko)
        {
            var gauge = GetJobGauge<SAMGauge>();

            var gaugeSen = new[]
            {
                gauge.HasSetsu ? 1 : 0,
                gauge.HasKa ? 1 : 0,
                gauge.HasGetsu ? 1 : 0
            };

            if (
                level >= SAM.Levels.TsubameGaeshi
                && HasCharges(SAM.TsubameGaeshi)
                && (GetCooldown(SAM.TsubameGaeshi).CooldownRemaining <= 15 || HasRaidBuffs())
                && HasEffect(SAM.Buffs.Jinpu)
                && OriginalHook(SAM.TsubameGaeshi) != SAM.TsubameGaeshi
            )
                return OriginalHook(SAM.TsubameGaeshi);

            if (GCDClipCheck(actionID))
            {
                var higanabana = FindTargetEffect(SAM.Debuffs.Higanabana);

                // var higanbanaTime = ((higanabana is null && ShouldRefreshDots())
                //            || (higanabana is not null && higanabana.RemainingTime <= 6));

                //if (gauge.MeditationStacks == 3 && (HasRaidBuffs() || (gaugeSen.Sum() >= 2)))
                //{
                //    return level >= SAM.Levels.Shoha2 ? SAM.Shoha2 : SAM.Shoha;
                //}

                var canUseIkishoten =
                    level >= SAM.Levels.Ikishoten && IsOffCooldown(SAM.Ikishoten) && InCombat();

                if (canUseIkishoten && gauge.Kenki <= 50) return SAM.Ikishoten;

                if (
                    level >= SAM.Levels.MeikyoShisui
                    && (HasCharges(SAM.MeikyoShisui) || IsOffCooldown(SAM.MeikyoShisui))
                    && (
                        HasRaidBuffs()
                        || level < SAM.Levels.DoubleMeikyoShisui
                        || GetCooldown(SAM.MeikyoShisui).CooldownRemaining <= 5
                    )
                )
                    return SAM.MeikyoShisui;

                if (
                    level >= SAM.Levels.HissatsuGuren
                    && IsOffCooldown(SAM.HissatsuGuren)
                    && gauge.Kenki >= 25
                    && HasEffect(SAM.Buffs.Jinpu)
                )
                    return SAM.HissatsuGuren;

                if (
                    level >= SAM.Levels.HissatsuKyuten
                    && gauge.Kenki >= 50
                    && HasEffect(SAM.Buffs.Jinpu)
                )
                    return SAM.HissatsuKyuten;
            }

            if (level >= SAM.Levels.TenkaGoken && gaugeSen.Sum() >= 2 && !this.IsMoving)
                return OriginalHook(SAM.Iaijutsu);

            if (level >= SAM.Levels.MeikyoShisui && HasEffect(SAM.Buffs.MeikyoShisui))
            {
                if (!gauge.HasGetsu)
                    return SAM.Mangetsu;
                if (!gauge.HasKa)
                    return SAM.Oka;
            }

            // Rear
            if ((!gauge.HasGetsu || !HasEffect(SAM.Buffs.Jinpu)) && level >= SAM.Levels.Mangetsu)
                if (lastComboMove == SAM.Fuga || lastComboMove == SAM.Fuko)
                    return SAM.Mangetsu;

            // Flank
            if ((!gauge.HasKa || !HasEffect(SAM.Buffs.Shifu)) && level >= SAM.Levels.Oka)
                if (lastComboMove == SAM.Fuga || lastComboMove == SAM.Fuko)
                    return SAM.Oka;
        }

        return actionID;
    }
}

internal class SamuraiOka : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.SamuraiOkaCombo;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SAM.Oka)
        {
            if (level >= SAM.Levels.MeikyoShisui && HasEffect(SAM.Buffs.MeikyoShisui))
                return SAM.Oka;

            if (comboTime > 0)
                if (
                    (lastComboMove == SAM.Fuga || lastComboMove == SAM.Fuko)
                    && level >= SAM.Levels.Oka
                )
                    return SAM.Oka;

            // Fuko
            return OriginalHook(SAM.Fuga);
        }

        return actionID;
    }
}

internal class SamuraiTsubame : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SAM.TsubameGaeshi)
        {
            var gauge = GetJobGauge<SAMGauge>();

            if (IsEnabled(CustomComboPreset.SamuraiTsubameGaeshiShohaFeature))
                if (level >= SAM.Levels.Shoha && gauge.MeditationStacks >= 3)
                    return SAM.Shoha;

            if (IsEnabled(CustomComboPreset.SamuraiTsubameGaeshiIaijutsuFeature))
            {
                if (level >= SAM.Levels.TsubameGaeshi && gauge.Sen == Sen.NONE)
                    return OriginalHook(SAM.TsubameGaeshi);

                return OriginalHook(SAM.Iaijutsu);
            }
        }

        return actionID;
    }
}

internal class SamuraiIaijutsu : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SAM.Iaijutsu)
        {
            var gauge = GetJobGauge<SAMGauge>();

            if (IsEnabled(CustomComboPreset.SamuraiIaijutsuShohaFeature))
                if (level >= SAM.Levels.Shoha && gauge.MeditationStacks >= 3)
                    return SAM.Shoha;

            if (IsEnabled(CustomComboPreset.SamuraiIaijutsuTsubameGaeshiFeature))
            {
                if (level >= SAM.Levels.TsubameGaeshi && gauge.Sen == Sen.NONE)
                    return OriginalHook(SAM.TsubameGaeshi);

                return OriginalHook(SAM.Iaijutsu);
            }
        }

        return actionID;
    }
}

internal class SamuraiShinten : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SAM.HissatsuShinten)
        {
            var gauge = GetJobGauge<SAMGauge>();

            if (IsEnabled(CustomComboPreset.SamuraiShintenZanshinFeature))
                if (level >= SAM.Levels.Zanshin && HasEffect(SAM.Buffs.ZanshinReady))
                    return SAM.Zanshin;

            if (IsEnabled(CustomComboPreset.SamuraiShintenShohaFeature))
                if (level >= SAM.Levels.Shoha && gauge.MeditationStacks >= 3)
                    return SAM.Shoha;

            if (IsEnabled(CustomComboPreset.SamuraiShintenSeneiFeature))
            {
                if (level >= SAM.Levels.HissatsuSenei && IsAvailable(SAM.HissatsuSenei))
                    return SAM.HissatsuSenei;

                if (IsEnabled(CustomComboPreset.SamuraiSeneiGurenFeature))
                    if (
                        level >= SAM.Levels.HissatsuGuren
                        && level < SAM.Levels.HissatsuSenei
                        && IsOffCooldown(SAM.HissatsuGuren)
                    )
                        return SAM.HissatsuGuren;
            }
        }

        return actionID;
    }
}

internal class SamuraiSenei : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SAM.HissatsuSenei)
            if (IsEnabled(CustomComboPreset.SamuraiSeneiGurenFeature))
                if (level >= SAM.Levels.HissatsuGuren && level < SAM.Levels.HissatsuSenei)
                    return SAM.HissatsuGuren;

        return actionID;
    }
}

internal class SamuraiKyuten : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SAM.HissatsuKyuten)
        {
            var gauge = GetJobGauge<SAMGauge>();

            if (IsEnabled(CustomComboPreset.SamuraiKyutenZanshinFeature))
                if (level >= SAM.Levels.Zanshin && HasEffect(SAM.Buffs.ZanshinReady))
                    return SAM.Zanshin;

            if (IsEnabled(CustomComboPreset.SamuraiKyutenShohaFeature))
                if (level >= SAM.Levels.Shoha && gauge.MeditationStacks >= 3)
                    return SAM.Shoha;

            if (IsEnabled(CustomComboPreset.SamuraiKyutenGurenFeature))
                if (level >= SAM.Levels.HissatsuGuren && IsAvailable(SAM.HissatsuGuren))
                    return SAM.HissatsuGuren;
        }

        return actionID;
    }
}

internal class SamuraiIkishoten : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.SamuraiIkishotenNamikiriFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SAM.Ikishoten)
            if (level >= SAM.Levels.OgiNamikiri)
            {
                var gauge = GetJobGauge<SAMGauge>();

                if (IsEnabled(CustomComboPreset.SamuraiIkishotenShohaFeature))
                    if (level >= SAM.Levels.Shoha && gauge.MeditationStacks >= 3)
                        return SAM.Shoha;

                if (gauge.Kaeshi == Kaeshi.NAMIKIRI)
                    return SAM.KaeshiNamikiri;

                if (HasEffect(SAM.Buffs.OgiNamikiriReady))
                    return SAM.OgiNamikiri;
            }

        return actionID;
    }
}