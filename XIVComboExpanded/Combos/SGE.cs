using System;
using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class SGE
{
    public const byte JobID = 40;

    public const uint Dosis = 24283,
        Diagnosis = 24284,
        Kardia = 24285,
        Prognosis = 24286,
        Egeiro = 24287,
        Physis = 24288,
        Phlegma = 24289,
        Eukrasia = 24290,
        EukrasianPrognosis = 24292,
        EukrasianDiagnosis = 24291,
        Soteria = 24294,
        Druochole = 24296,
        Dyskrasia = 24297,
        Kerachole = 24298,
        Ixochole = 24299,
        Zoe = 24300,
        Pepsis = 24301,
        Physis2 = 24302,
        Taurochole = 24303,
        Toxikon = 24304,
        Haima = 24305,
        Dyskrasia2 = 24315,
        Phlegma2 = 24307,
        Rhizomata = 24309,
        Holos = 24310,
        Panhaima = 24311,
        Phlegma3 = 24313,
        Krasis = 24317,
        Psyche = 37033,
        Philosophia = 37035,
        Pneuma = 24318;

    public static class Buffs
    {
        public const ushort Physis = 2617,
            Physis2 = 2620,
            Kardion = 2604,
            Kerakeia = 2938,
            Philosophia = 3898,
            Zoe = 2611,
            Eukrasia = 2606;
    }

    public static class Debuffs
    {
        public const ushort EDosis1 = 2614,
            EDosis2 = 2615,
            EDosis3 = 2616,
            EDyskrasia = 3897;
    }

    public static class Levels
    {
        public const byte Dosis = 1,
            Prognosis = 10,
            Egeiro = 12,
            Physis = 20,
            Phlegma = 26,
            EDosis1 = 30,
            Soteria = 35,
            Druochole = 45,
            Dyskrasia = 46,
            Kerachole = 50,
            Ixochole = 52,
            Zoe = 56,
            Physis2 = 60,
            Taurochole = 62,
            Toxikon = 66,
            Haima = 70,
            Phlegma2 = 72,
            Dosis2 = 72,
            EDosis2 = 72,
            Rhizomata = 74,
            Holos = 76,
            EnhancedKerachole = 78,
            Panhaima = 80,
            Phlegma3 = 82,
            EDosis3 = 82,
            Dosis3 = 82,
            Krasis = 86,
            Pneuma = 90,
            Psyche = 92,
            Philosophia = 100;
    }
}

internal class SageDosis : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SgeAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SGE.Dosis)
        {
            var gauge = GetJobGauge<SGEGauge>();

            var myHP = LocalPlayerPercentage();

            var threshold = 0.75;

            var mpThreshold = 9200;

            var raidbuffs = HasRaidBuffs();

            var controlBurst = LocalPlayer?.CurrentMp <= mpThreshold
                            || IsOnCooldown(ADV.LucidDreaming);

            if (GCDClipCheck(actionID))
            {
                var needToUseAddersgall = (gauge.Addersgall == 2 && gauge.AddersgallTimer <= 10) || gauge.Addersgall == 3;
                var targetHPPercent = TargetOfTargetHPercentage();

                switch (level)
                {
                    case >= SGE.Levels.Physis when
                        (!HasEffect(SGE.Buffs.Kerakeia) || myHP <= threshold - 0.25)
                        && IsOffCooldown(OriginalHook(SGE.Physis))
                        && myHP <= threshold:
                        return OriginalHook(SGE.Physis);

                    case >= SGE.Levels.Ixochole when IsOffCooldown(SGE.Ixochole)
                                     && gauge.Addersgall >= 2
                                     && ((!HasEffect(SGE.Buffs.Physis)
                                          && !HasEffect(SGE.Buffs.Physis2)
                                          && !HasEffect(SGE.Buffs.Kerakeia))
                                         || myHP <= threshold - 0.40)
                                     && myHP <= threshold - 0.10:
                        return SGE.Ixochole;

                    case >= SGE.Levels.Soteria when IsOffCooldown(SGE.Soteria)
                                    && ((!HasEffect(SGE.Buffs.Physis)
                                         && !HasEffect(SGE.Buffs.Physis2)
                                         && !HasEffect(SGE.Buffs.Kerakeia))
                                        || myHP <= threshold - 0.25)
                                    && targetHPPercent <= threshold:
                        return SGE.Soteria;

                    case >= SGE.Levels.Krasis when IsOffCooldown(SGE.Krasis)
                                   && ((!HasEffect(SGE.Buffs.Physis)
                                        && !HasEffect(SGE.Buffs.Physis2)
                                        && !HasEffect(SGE.Buffs.Kerakeia))
                                       || myHP <= threshold - 0.15)
                                   && targetHPPercent <= threshold - 0.1:
                        return SGE.Krasis;

                    case >= SGE.Levels.Druochole when 
                        targetHPPercent >= 0.2
                        && gauge.Addersgall >= 2
                        && (targetHPPercent <= threshold - 0.25
                            || (needToUseAddersgall && targetHPPercent <= threshold - 0.15)):
                        return level >= SGE.Levels.Taurochole && IsOffCooldown(SGE.Taurochole)
                            ? SGE.Taurochole
                            : SGE.Druochole;

                    case >= SGE.Levels.Rhizomata when 
                        gauge.Addersgall <= 1
                        && IsOffCooldown(SGE.Rhizomata):
                        return SGE.Rhizomata;

                    case >= SGE.Levels.Psyche when 
                        (controlBurst || raidbuffs)
                        && IsOffCooldown(SGE.Psyche):
                        return SGE.Psyche;

                    case >= ADV.Levels.LucidDreaming when 
                        IsOffCooldown(ADV.LucidDreaming) 
                        && LocalPlayer?.CurrentMp <= 9200:
                        return ADV.LucidDreaming;
                }

            }

            if (InCombat())
            {
                if (InCombat() && TargetIsEnemy() && ShouldUseDots() && actionID is not AST.Gravity)
                {
                    var debuffs = new[]
                    {
                        FindTargetEffect(SGE.Debuffs.EDyskrasia),
                        FindTargetEffect(SGE.Debuffs.EDosis3),
                        FindTargetEffect(SGE.Debuffs.EDosis2),
                        FindTargetEffect(SGE.Debuffs.EDosis1)
                        };

                    if (debuffs.All(x => x is null || x.RemainingTime <= 4 || x.RemainingTime <= 8 && IsMoving))
                    {
                        if (!HasEffect(SGE.Buffs.Eukrasia)) return SGE.Eukrasia;

                        return OriginalHook(SGE.Dosis);
                    }
                }

                if (
                    level >= SGE.Levels.Pneuma
                    && IsOffCooldown(SGE.Pneuma)
                    && (
                        (
                            !HasEffect(SGE.Buffs.Physis)
                            && !HasEffect(SGE.Buffs.Physis2)
                            && !HasEffect(SGE.Buffs.Kerakeia)
                        )
                        || myHP <= threshold - 0.4
                    )
                    && IsOnCooldown(SGE.Ixochole)
                    && !this.IsMoving
                    && myHP <= threshold - 0.2
                )
                    return SGE.Pneuma;

                var charges = GetRemainingCharges(OriginalHook(SGE.Phlegma));

                if (
                    level >= SGE.Levels.Phlegma
                    && GetTargetDistance() <= 6
                    && charges >= 1
                    && controlBurst
                    && (GetCooldown(OriginalHook(SGE.Phlegma)).TotalCooldownRemaining <= 5
                        || raidbuffs
                        || charges >= 2)
                )
                    return OriginalHook(SGE.Phlegma);

                if (this.IsMoving)
                    if (gauge.Addersting >= 1 && level >= SGE.Levels.Toxikon)
                        return OriginalHook(SGE.Toxikon);

                return actionID;
            }

            return actionID;
        }

        return actionID;
    }
}

internal class SageTaurochole : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SgeAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SGE.Taurochole)
        {
            var gauge = GetJobGauge<SGEGauge>();

            if (level >= SGE.Levels.Rhizomata && gauge.Addersgall == 0)
                return SGE.Rhizomata;

            if (level >= SGE.Levels.Taurochole && IsOffCooldown(SGE.Taurochole))
                return SGE.Taurochole;

            return SGE.Druochole;
        }

        return actionID;
    }
}

internal class SageShieldDiagnosis : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SgeAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        // If the action is Diagnosis and we have eukrasia buff then do a zoe cast if it's off cooldown
        if (actionID == SGE.Diagnosis || actionID == SGE.Prognosis)
        {
            // if (GCDClipCheck(actionID))
            // {
            if (IsOffCooldown(SGE.Zoe)
                && level >= SGE.Levels.Zoe
                && HasEffect(SGE.Buffs.Eukrasia)
                )
            {
                return SGE.Zoe;
            }

            // if (IsOffCooldown(SGE.Philosophia)
            //     && level >= SGE.Levels.Philosophia
            //     && HasEffect(SGE.Buffs.Eukrasia)
            //     && !HasEffect(SGE.Buffs.Zoe))
            // {
            //     return SGE.Philosophia;
            // }

            return actionID;
        }

        return actionID;
    }

    internal class SagePhlegma : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SgeAny;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == SGE.Dyskrasia2 || actionID == SGE.Dyskrasia)
            {
                var gauge = GetJobGauge<SGEGauge>();

                var targetHPPercent = TargetOfTargetHPercentage();

                var myHP = LocalPlayerPercentage();

                var threshold = 0.85;

                if (GCDClipCheck(actionID))
                {
                    if (
                        level >= SGE.Levels.Physis
                        && (!HasEffect(SGE.Buffs.Kerakeia) || myHP <= threshold - 0.2)
                        && IsOffCooldown(OriginalHook(SGE.Physis))
                        && myHP <= threshold
                    )
                        return OriginalHook(SGE.Physis);

                    if (
                        level >= SGE.Levels.Ixochole
                        && IsOffCooldown(SGE.Ixochole)
                        && gauge.Addersgall >= 2
                        && (
                            (
                                !HasEffect(SGE.Buffs.Physis)
                                && !HasEffect(SGE.Buffs.Physis2)
                                && !HasEffect(SGE.Buffs.Kerakeia)
                            )
                            || myHP <= threshold - 0.35
                        )
                        && myHP <= threshold - 0.10
                    )
                        return SGE.Ixochole;

                    if (
                        level >= SGE.Levels.Soteria
                        && IsOffCooldown(SGE.Soteria)
                        && (
                            (
                                !HasEffect(SGE.Buffs.Physis)
                                && !HasEffect(SGE.Buffs.Physis2)
                                && !HasEffect(SGE.Buffs.Kerakeia)
                            )
                            || myHP <= threshold - 0.20
                        )
                        && targetHPPercent <= threshold
                    )
                        return SGE.Soteria;

                    if (
                        level >= SGE.Levels.Krasis
                        && IsOffCooldown(SGE.Krasis)
                        && (
                            (
                                !HasEffect(SGE.Buffs.Physis)
                                && !HasEffect(SGE.Buffs.Physis2)
                                && !HasEffect(SGE.Buffs.Kerakeia)
                            )
                            || myHP <= threshold - 0.10
                        )
                        && targetHPPercent <= threshold - 0.1
                    )
                        return SGE.Krasis;

                    if (
                        level >= SGE.Levels.Rhizomata
                        && gauge.Addersgall <= 1
                        && IsOffCooldown(SGE.Rhizomata)
                    )
                        return SGE.Rhizomata;

                    if (level >= SGE.Levels.Psyche && IsOffCooldown(SGE.Psyche)) return SGE.Psyche;

                    // Use Lucid Dreaming if low enough mana
                    if (IsOffCooldown(ADV.LucidDreaming) && LocalPlayer?.CurrentMp <= 8000) return ADV.LucidDreaming;
                }

                if (level >= SGE.Levels.EDosis3 && GetTargetDistance() <= 6 && TargetIsEnemy())
                {

                    if (InCombat() && TargetIsEnemy() && ShouldUseDots() && actionID is not AST.Gravity)
                    {
                        var debuffs = new[]
                        {
                        FindTargetEffect(SGE.Debuffs.EDyskrasia),
                        FindTargetEffect(SGE.Debuffs.EDosis3),
                        FindTargetEffect(SGE.Debuffs.EDosis2),
                        FindTargetEffect(SGE.Debuffs.EDosis1)
                        };

                        if (debuffs.All(x => x is null || x.RemainingTime <= 3 || x.RemainingTime <= 6 && IsMoving))
                        {
                            if (!HasEffect(SGE.Buffs.Eukrasia)) return SGE.Eukrasia;

                            return OriginalHook(SGE.Dyskrasia);
                        }
                    }
                }



                var plegma = OriginalHook(SGE.Phlegma);

                if (GetTargetDistance() <= 6 && HasCharges(plegma) && level >= SGE.Levels.Phlegma) return plegma;

                if (
                    level >= SGE.Levels.Toxikon
                    && HasTarget()
                    && TargetIsEnemy()
                    && gauge.Addersting > 0
                )
                    return OriginalHook(SGE.Toxikon);

                if (gauge.Addersting >= 1 && level >= SGE.Levels.Toxikon) return OriginalHook(SGE.Toxikon);

                return actionID;
            }

            return actionID;
        }
    }
}

internal class SagePhlegma : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SgeAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SGE.Physis)
        {
            if (IsOffCooldown(SGE.Philosophia)
                && level >= SGE.Levels.Philosophia)
            {
                return CalcBestAction(OriginalHook(SGE.Physis), OriginalHook(SGE.Physis), SGE.Philosophia);
            }

            return actionID;
        }

        return actionID;
    }
}
