using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class SGE
{
    public const byte JobID = 40;

    public const uint
        Dosis = 24283,
        Diagnosis = 24284,
        Kardia = 24285,
        Prognosis = 24286,
        Egeiro = 24287,
        Physis = 24288,
        Phlegma = 24289,
        Eukrasia = 24290,
        EukrasianPrognosis = 24292,
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
        Pneuma = 24318;

    public static class Buffs
    {
        public const ushort
            Physis = 2617,
            Physis2 = 2620,
            Kardion = 2604,
            Kerakeia = 2938,
            Eukrasia = 2606;
    }

    public static class Debuffs
    {
        public const ushort
            EDosis1 = 2614,
            EDosis2 = 2615,
            EDosis3 = 2616;
    }

    public static class Levels
    {
        public const ushort
            Dosis = 1,
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
            Pneuma = 90;
    }
}

internal class SageSoteria : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SgeAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SGE.Dosis)
        {
            var gauge = GetJobGauge<SGEGauge>();

            var myHP = LocalPlayerPercentage();

            var threshold = 0.80;

            if (GCDClipCheck(actionID))
            {
                var targetHPPercent = TargetOfTargetHPercentage();

                if (level >= SGE.Levels.Physis
                    && (!HasEffect(SGE.Buffs.Kerakeia) || myHP <= threshold - 0.25)
                    && IsOffCooldown(OriginalHook(SGE.Physis))
                    && myHP <= threshold)
                {
                    return OriginalHook(SGE.Physis);
                }

                if (level >= SGE.Levels.Ixochole
                    && IsOffCooldown(SGE.Ixochole)
                    && gauge.Addersgall >= 2
                    && ((!HasEffect(SGE.Buffs.Physis)
                        && !HasEffect(SGE.Buffs.Physis2)
                        && !HasEffect(SGE.Buffs.Kerakeia)) || myHP <= threshold - 0.40)
                    && (myHP <= threshold - 0.10))
                {
                    return SGE.Ixochole;
                }

                if (level >= SGE.Levels.Soteria
                    && IsOffCooldown(SGE.Soteria)
                    && ((!HasEffect(SGE.Buffs.Physis)
                        && !HasEffect(SGE.Buffs.Physis2)
                        && !HasEffect(SGE.Buffs.Kerakeia)) || myHP <= threshold - 0.15)
                    && (targetHPPercent <= threshold))
                {
                    return SGE.Soteria;
                }

                if (level >= SGE.Levels.Krasis
                    && IsOffCooldown(SGE.Krasis)
                    && ((!HasEffect(SGE.Buffs.Physis)
                        && !HasEffect(SGE.Buffs.Physis2)
                        && !HasEffect(SGE.Buffs.Kerakeia)) || myHP <= threshold - 0.25)
                    && (targetHPPercent <= threshold - 0.1))
                {
                    return SGE.Krasis;
                }

                var needToUseAddersgall = (gauge.Addersgall == 2 && gauge.AddersgallTimer <= 10)
                        || gauge.Addersgall == 3;

                // Use Druchole if the target of druget is less than 0.7 and we have 3 charges.
                if (level >= SGE.Levels.Druochole
                    && targetHPPercent >= 0.2
                    && gauge.Addersgall >= 2
                    && (targetHPPercent <= threshold - 0.2
                        || (needToUseAddersgall && targetHPPercent <= threshold - 0.1)))
                {
                    return level >= SGE.Levels.Taurochole && IsOffCooldown(SGE.Taurochole)
                        ? SGE.Taurochole
                        : SGE.Druochole;
                }

                if (level >= SGE.Levels.Rhizomata
                    && gauge.Addersgall <= 2
                    && IsOffCooldown(SGE.Rhizomata))
                {
                    return SGE.Rhizomata;
                }

                // Use Lucid Dreaming if low enough mana
                if (IsOffCooldown(ADV.LucidDreaming)
                    && LocalPlayer?.CurrentMp <= 8000)
                {
                    return ADV.LucidDreaming;
                }
            }

            // (ushort Debuff, ushort Level)[] EDosises = new[]
            // {
            //    ( SGE.Debuffs.EDosis3, SGE.Levels.EDosis3 ),
            //    ( SGE.Debuffs.EDosis2, SGE.Levels.EDosis2 ),
            //    ( SGE.Debuffs.EDosis1, SGE.Levels.EDosis1 )
            // };

            if (InCombat())
            {
                var debuff = FindTargetEffect(SGE.Debuffs.EDosis3);

                var debuffTime = debuff?.RemainingTime;

                if (level >= SGE.Levels.EDosis3
                    && ((debuff is not null
                        && (debuffTime <= 3
                            || (debuffTime <= 6 && this.IsMoving)))
                        || (debuff is null && ShouldRefreshDots())))
                {
                    if (!HasEffect(SGE.Buffs.Eukrasia))
                    {
                        return SGE.Eukrasia;
                    }

                    return OriginalHook(SGE.Dosis);
                }

                if (level >= SGE.Levels.Pneuma
                    && IsOffCooldown(SGE.Pneuma)
                    && ((!HasEffect(SGE.Buffs.Physis)
                        && !HasEffect(SGE.Buffs.Physis2)
                        && !HasEffect(SGE.Buffs.Kerakeia)) || myHP <= threshold - 0.4)
                    && IsOnCooldown(SGE.Ixochole)
                    && !this.IsMoving
                    && myHP <= threshold - 0.2)
                {
                    return SGE.Pneuma;
                }

                var plegma = OriginalHook(SGE.Phlegma);

                if (GetTargetDistance() <= 6
                    && HasCharges(plegma)
                    && (GetCooldown(plegma).ChargeCooldownRemaining <= 5
                        // || IsMoving
                        || GetRemainingCharges(plegma) == 2
                        || HasRaidBuffs()))
                {
                    return SGE.Phlegma3;
                }

                if (this.IsMoving)
                {
                    if (gauge.Addersting >= 1
                        && level >= SGE.Levels.Toxikon)
                    {
                        return OriginalHook(SGE.Toxikon);
                    }

                    // if (GetTargetDistance() <= 5
                    //    && HasTarget()
                    //    && level >= SGE.Levels.Dyskrasia
                    //    && TargetIsEnemy())
                    // {
                    //    return level >= SGE.Levels.Phlegma3 ? SGE.Dyskrasia2 : SGE.Dyskrasia;
                    // }
                }

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

            if (IsEnabled(CustomComboPreset.SageTaurocholeRhizomataFeature))
            {
                if (level >= SGE.Levels.Rhizomata && gauge.Addersgall == 0)
                    return SGE.Rhizomata;
            }

            if (IsEnabled(CustomComboPreset.SageTaurocholeDruocholeFeature))
            {
                if (level >= SGE.Levels.Taurochole && IsOffCooldown(SGE.Taurochole))
                    return SGE.Taurochole;

                return SGE.Druochole;
            }
        }

        return actionID;
    }
}

internal class SageZoe : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SgeAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == SGE.Prognosis || actionID == SGE.EukrasianPrognosis)
        {
            if (IsOffCooldown(SGE.Zoe) && level >= SGE.Levels.Zoe)
            {
                return SGE.Zoe;
            }

            // if (actionID == SGE.Pneuma)
            // {
            //    return CalcBestAction(SGE.Pneuma, SGE.Zoe, SGE.Pneuma);
            // }
        }

        return actionID;
    }
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
                if (level >= SGE.Levels.Physis
                    && (!HasEffect(SGE.Buffs.Kerakeia) || myHP <= threshold - 0.3)
                    && IsOffCooldown(OriginalHook(SGE.Physis))
                    && myHP <= threshold - 0.1)
                {
                    return OriginalHook(SGE.Physis);
                }

                if (level >= SGE.Levels.Soteria
                    && IsOffCooldown(SGE.Soteria)
                    && (targetHPPercent <= threshold - 0.15))
                {
                    return SGE.Soteria;
                }

                if (level >= SGE.Levels.Krasis
                    && IsOffCooldown(SGE.Krasis)
                    && (targetHPPercent <= threshold - 0.2))
                {
                    return SGE.Krasis;
                }

                if (level >= SGE.Levels.Druochole
                    && gauge.Addersgall >= 2
                    && (targetHPPercent <= threshold - 0.25))
                {
                    return level >= SGE.Levels.Taurochole && IsOffCooldown(SGE.Taurochole)
                        ? SGE.Taurochole
                        : SGE.Druochole;
                }

                if (level >= SGE.Levels.Rhizomata
                    && gauge.Addersgall <= 2
                    && IsOffCooldown(SGE.Rhizomata))
                {
                    return SGE.Rhizomata;
                }

                // Use Lucid Dreaming if low enough mana
                if (IsOffCooldown(ADV.LucidDreaming)
                    && LocalPlayer?.CurrentMp <= 8000)
                {
                    return ADV.LucidDreaming;
                }
            }

            var plegma = OriginalHook(SGE.Phlegma);

            if (GetTargetDistance() <= 6
                && HasCharges(plegma))
            {
                return OriginalHook(SGE.Phlegma);
            }

            if (level >= SGE.Levels.Toxikon
                && HasTarget()
                && TargetIsEnemy()
                && gauge.Addersting > 0)
            {
                return OriginalHook(SGE.Toxikon);
            }

            if (gauge.Addersting >= 1
                && level >= SGE.Levels.Toxikon)
            {
                return OriginalHook(SGE.Toxikon);
            }

            return actionID;
        }

        return actionID;
    }
}
