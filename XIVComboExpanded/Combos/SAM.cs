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
            TsubameGaeshi = 3852,
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
                (higanabana is null && ShouldUseDots())
                || (higanabana is not null && higanabana.RemainingTime <= 6);

            var canUseIkishoten = IsOffCooldown(SAM.Ikishoten) && InCombat();

            var hasRaidBuffs = HasRaidBuffs();

            var jinpu = FindEffect(SAM.Buffs.Jinpu);
            var shifu = FindEffect(SAM.Buffs.Shifu);

            if (GCDClipCheck(actionID))
            {

                var zanshin = FindEffect(SAM.Buffs.ZanshinReady);

                switch (level)
                {
                    case >= SAM.Levels.Shoha when
                        gauge.MeditationStacks == 3
                        && (higanbanaTime
                            || hasRaidBuffs
                            || gaugeSen.Sum() == 3):
                        return SAM.Shoha;
                    case >= SAM.Levels.Zanshin when
                        zanshin is not null
                        && (hasRaidBuffs || zanshin.RemainingTime <= 10):
                        return SAM.Zanshin;
                    case >= SAM.Levels.MeikyoShisui when
                        (HasCharges(SAM.MeikyoShisui) || IsOffCooldown(SAM.MeikyoShisui))
                        && InCombat()
                        // && (!(comboTime > 0) || jinpu is null || shifu is null)
                        && (hasRaidBuffs
                            || GetCooldown(SAM.MeikyoShisui).TotalCooldownRemaining <= 10):
                        return SAM.MeikyoShisui;
                    case >= SAM.Levels.Ikishoten when
                        canUseIkishoten
                        && hasRaidBuffs
                        && gauge.Kenki <= 50:
                        return SAM.Ikishoten;
                    case >= SAM.Levels.HissatsuGuren when
                        IsOffCooldown(SAM.HissatsuGuren)
                        && (IsOnCooldown(SAM.Ikishoten) || level < SAM.Levels.Ikishoten)
                        && gauge.Kenki >= 25
                        && HasEffect(SAM.Buffs.Jinpu):
                        return level >= SAM.Levels.HissatsuSenei
                            ? SAM.HissatsuSenei
                            : SAM.HissatsuGuren;
                    case >= SAM.Levels.HissatsuKyuten when
                        gauge.Kenki >= 25
                        && HasEffect(SAM.Buffs.Jinpu):

                        var skill = level >= SAM.Levels.HissatsuShinten ? OriginalHook(SAM.HissatsuShinten) : OriginalHook(SAM.HissatsuKyuten);

                        if (gauge.Kenki >= 75
                            || (level >= SAM.Levels.Ikishoten && canUseIkishoten && gauge.Kenki >= 35)
                            || (GetCooldown(SAM.Ikishoten).CooldownRemaining <= 6 && gauge.Kenki >= 35)
                            || (hasRaidBuffs && (GetCooldown(SAM.HissatsuGuren).CooldownRemaining >= 5 || level < SAM.Levels.HissatsuGuren)))
                        {
                            return skill;
                        }
                        break;

                }
            }

            if (OriginalHook(SAM.OgiNamikiri) != SAM.OgiNamikiri) return OriginalHook(SAM.OgiNamikiri);

            if (level >= SAM.Levels.TsubameGaeshi
                && CanUseAction(OriginalHook(SAM.TsubameGaeshi)))
                return OriginalHook(SAM.TsubameGaeshi);

            if (level >= SAM.Levels.OgiNamikiri
                && gauge.MeditationStacks is not 3
                && FindEffect(SAM.Buffs.OgiNamikiriReady) is not null)
                return SAM.OgiNamikiri;

            if (gauge.MeditationStacks != 3)
            {
                if (level >= SAM.Levels.Higanbana && !HasEffect(SAM.Buffs.MeikyoShisui) &&
                    HasEffect(SAM.Buffs.Jinpu) && HasEffect(SAM.Buffs.Shifu) && gaugeSen.Sum() == 1 &&
                    ((higanabana is null && ShouldUseDots()) || (higanabana is not null && higanabana.RemainingTime <= 6)))
                    return OriginalHook(SAM.Iaijutsu);

                if (level >= SAM.Levels.MidareSetsugekka && gaugeSen.Sum() == 3)
                    return OriginalHook(SAM.Iaijutsu);
            }

            if (level >= SAM.Levels.MeikyoShisui && HasEffect(SAM.Buffs.MeikyoShisui))
            {
                if (!gauge.HasKa) return SAM.Kasha;
                if (!gauge.HasGetsu) return SAM.Gekko;
                if (!gauge.HasSetsu) return SAM.Yukikaze;
            }

            if (!gauge.HasSetsu && level >= SAM.Levels.Higanbana)
                return lastComboMove == OriginalHook(SAM.Hakaze) && level >= SAM.Levels.Yukikaze ? SAM.Yukikaze : OriginalHook(SAM.Hakaze);

            if (!gauge.HasKa || (shifu is null && level >= SAM.Levels.Shifu) || (jinpu?.RemainingTime > shifu?.RemainingTime && level < SAM.Levels.Higanbana))
            {
                if (lastComboMove == SAM.Shifu && level >= SAM.Levels.Kasha) return SAM.Kasha;
                if (lastComboMove == OriginalHook(SAM.Hakaze)) return SAM.Shifu;
            }

            if (!gauge.HasGetsu || (jinpu is null && level >= SAM.Levels.Jinpu) || (jinpu?.RemainingTime < shifu?.RemainingTime && level < SAM.Levels.Higanbana))
            {
                if (lastComboMove == SAM.Jinpu && level >= SAM.Levels.Gekko) return SAM.Gekko;
                if (lastComboMove == OriginalHook(SAM.Hakaze)) return SAM.Jinpu;
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

            var higanabana = FindTargetEffect(SAM.Debuffs.Higanabana);

            var higanbanaTime =
                (higanabana is null && ShouldUseDots())
                || (higanabana is not null && higanabana.RemainingTime <= 6);

            var canUseIkishoten = IsOffCooldown(SAM.Ikishoten) && InCombat();

            var hasRaidBuffs = HasRaidBuffs();

            var jinpu = FindEffect(SAM.Buffs.Jinpu);
            var shifu = FindEffect(SAM.Buffs.Shifu);

            if (GCDClipCheck(actionID))
            {
                var zanshin = FindEffect(SAM.Buffs.ZanshinReady);

                switch (level)
                {
                    case >= SAM.Levels.Shoha when
                        gauge.MeditationStacks == 3
                        && (higanbanaTime
                            || hasRaidBuffs
                            || gaugeSen.Sum() == 3):
                        return SAM.Shoha;
                    case >= SAM.Levels.Zanshin when
                        zanshin is not null:
                        return SAM.Zanshin;
                    case >= SAM.Levels.MeikyoShisui when
                        (HasCharges(SAM.MeikyoShisui) || IsOffCooldown(SAM.MeikyoShisui))
                        && InCombat()
                        // && (!(comboTime > 0) || jinpu is null || shifu is null)
                        && (hasRaidBuffs
                            || GetCooldown(SAM.MeikyoShisui).TotalCooldownRemaining <= 10):
                        return SAM.MeikyoShisui;
                    case >= SAM.Levels.Ikishoten when
                        canUseIkishoten
                        && hasRaidBuffs
                        && gauge.Kenki <= 50:
                        return SAM.Ikishoten;
                    case >= SAM.Levels.HissatsuGuren when
                        IsOffCooldown(SAM.HissatsuGuren)
                        && (IsOnCooldown(SAM.Ikishoten) || level < SAM.Levels.Ikishoten)
                        && gauge.Kenki >= 25
                        && HasEffect(SAM.Buffs.Jinpu):
                        return level >= SAM.Levels.HissatsuSenei
                            ? SAM.HissatsuSenei
                            : SAM.HissatsuGuren;
                    case >= SAM.Levels.HissatsuShinten when gauge.Kenki >= 25 && HasEffect(SAM.Buffs.Jinpu):
                        if (gauge.Kenki >= 75)
                            return OriginalHook(SAM.HissatsuShinten);

                        if (level >= SAM.Levels.Ikishoten && canUseIkishoten && gauge.Kenki >= 35)
                            return OriginalHook(SAM.HissatsuShinten);

                        if (GetCooldown(SAM.Ikishoten).CooldownRemaining <= 6 && gauge.Kenki >= 35)
                            return OriginalHook(SAM.HissatsuShinten);

                        if (hasRaidBuffs
                            && (GetCooldown(SAM.HissatsuGuren).CooldownRemaining >= 15 || level < SAM.Levels.HissatsuGuren))
                            return OriginalHook(SAM.HissatsuShinten);
                        break;
                    case >= SAM.Levels.HissatsuKyuten when gauge.Kenki >= 25
                                            && (gauge.Kenki >= 75
                                                || (((level >= SAM.Levels.Ikishoten && canUseIkishoten)
                                                     || GetCooldown(SAM.Ikishoten).CooldownRemaining <= 6)
                                                    && gauge.Kenki >= 35)
                                                || (hasRaidBuffs
                                                    && (GetCooldown(SAM.HissatsuGuren).CooldownRemaining >= 15
                                                        || level < SAM.Levels.HissatsuGuren)))
                                            && HasEffect(SAM.Buffs.Shifu):
                        return OriginalHook(SAM.HissatsuKyuten);
                }

                if (level >= SAM.Levels.TsubameGaeshi
                    && CanUseAction(OriginalHook(SAM.TsubameGaeshi)))
                    return OriginalHook(SAM.TsubameGaeshi);

                if (
                    level >= SAM.Levels.HissatsuGuren
                    && IsOffCooldown(SAM.HissatsuGuren)
                    && gauge.Kenki >= 25
                    && HasEffect(SAM.Buffs.Jinpu)
                )
                    return SAM.HissatsuGuren;
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