using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class RPR
{
    public const byte JobID = 39;

    public const uint
        // Single Target
        Slice = 24373,
        WaxingSlice = 24374,
        InfernalSlice = 24375,

        // AoE
        SpinningScythe = 24376,
        NightmareScythe = 24377,
        WhorlOfDeath = 24379,
        GrimReaping = 24397,

        // Soul Reaver
        Gibbet = 24382,
        Gallows = 24383,
        Guillotine = 24384,
        BloodStalk = 24389,
        UnveiledGibbet = 24390,
        UnveiledGallows = 24391,
        GrimSwathe = 24392,
        Gluttony = 24393,
        VoidReaping = 24395,
        CrossReaping = 24396,

        // Generators
        SoulSlice = 24380,
        SoulScythe = 24381,
        // Sacrifice


        ArcaneCircle = 24405,
        PlentifulHarvest = 24385,
        // Shroud


        Enshroud = 24394,
        Communio = 24398,
        LemuresSlice = 24399,
        LemuresScythe = 24400,
        // Misc


        ShadowOfDeath = 24378,
        Harpe = 24386,
        Soulsow = 24387,
        HarvestMoon = 24388,
        HellsIngress = 24401,
        HellsEgress = 24402,
        Regress = 24403;

    public static class Buffs
    {
        public const ushort
            EnhancedHarpe = 2845,
            SoulReaver = 2587,
            EnhancedGibbet = 2588,
            EnhancedGallows = 2589,
            EnhancedVoidReaping = 2590,
            EnhancedCrossReaping = 2591,
            ImmortalSacrifice = 2592,
            Enshrouded = 2593,
            Soulsow = 2594,
            ArcaneCircle = 2599,
            BloodsownCircle = 2972,
            Threshold = 2595;
    }

    public static class Debuffs
    {
        public const ushort
            DeathsDesign = 2586;
    }

    public static class Levels
    {
        public const byte
            WaxingSlice = 5,
            HellsIngress = 20,
            HellsEgress = 20,
            SpinningScythe = 25,
            InfernalSlice = 30,
            WhorlOfDeath = 35,
            NightmareScythe = 45,
            BloodStalk = 50,
            GrimSwathe = 55,
            SoulSlice = 60,
            SoulScythe = 65,
            SoulReaver = 70,
            ArcaneCircle = 72,
            Regress = 74,
            Gluttony = 76,
            Enshroud = 80,
            Soulsow = 82,
            HarvestMoon = 82,
            EnhancedShroud = 86,
            LemuresScythe = 86,
            PlentifulHarvest = 88,
            Communio = 90;
    }
}

internal class ReaperSlice : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RPR.Slice)
        {
            var gauge = GetJobGauge<RPRGauge>();

            var deathsDesign = FindTargetEffect(RPR.Debuffs.DeathsDesign);

            var needSoulSlice = level >= RPR.Levels.SoulSlice
                            && HasCharges(RPR.SoulSlice)
                            && (GetRemainingCharges(RPR.SoulSlice) >= 2
                                || (GetCooldown(RPR.SoulSlice).ChargeCooldownRemaining <= 6)
                                || HasEffect(RPR.Buffs.ArcaneCircle)
                                || HasRaidBuffs());

            if (GCDClipCheck(actionID) && !HasEffect(RPR.Buffs.SoulReaver))
            {
                if (level >= RPR.Levels.ArcaneCircle
                    && IsOffCooldown(RPR.ArcaneCircle)
                    && (gauge.LemureShroud <= 2 || HasRaidBuffs()))
                {
                    return RPR.ArcaneCircle;
                }

                if (level >= RPR.Levels.Enshroud
                    && gauge.Shroud >= 50
                    && gauge.EnshroudedTimeRemaining == 0
                    && (gauge.Shroud >= 80
                        || HasEffect(RPR.Buffs.ArcaneCircle)
                        || HasRaidBuffs()
                        || HasEffect(RPR.Buffs.ImmortalSacrifice))
                    && IsOffCooldown(RPR.Enshroud))
                {
                    return RPR.Enshroud;
                }

                if (level >= RPR.Levels.Gluttony
                    && IsOffCooldown(RPR.Gluttony)
                    && gauge.Shroud <= 80
                    && gauge.EnshroudedTimeRemaining == 0
                    && (HasEffect(RPR.Buffs.ArcaneCircle)
                        || HasRaidBuffs()
                        || GetCooldown(RPR.ArcaneCircle).CooldownRemaining >= 3)
                    && gauge.Soul >= 50)
                {
                    return RPR.Gluttony;
                }

                if (level >= RPR.Levels.BloodStalk
                    && ((gauge.Soul >= 50
                        && gauge.Shroud <= 90
                        && (gauge.Soul >= 90
                            || needSoulSlice

                            || HasEffect(RPR.Buffs.ArcaneCircle)
                            || HasRaidBuffs()))

                    || gauge.VoidShroud >= 2))
                {
                    return OriginalHook(RPR.BloodStalk);
                }
            }

            if (level >= RPR.Levels.Communio && gauge.LemureShroud == 1)
                return RPR.Communio;

            if ((!HasEffect(RPR.Buffs.EnhancedGibbet) && HasEffect(RPR.Buffs.SoulReaver))
                || (!HasEffect(RPR.Buffs.EnhancedVoidReaping) && gauge.EnshroudedTimeRemaining > 0))
            {
                return OriginalHook(RPR.Gallows);
            }

            if ((HasEffect(RPR.Buffs.EnhancedGibbet) && HasEffect(RPR.Buffs.SoulReaver))
                || (HasEffect(RPR.Buffs.EnhancedVoidReaping) && gauge.EnshroudedTimeRemaining > 0))
            {
                return OriginalHook(RPR.Gibbet);
            }

            if (level >= RPR.Levels.Soulsow)
            {
                if ((!InCombat() && !HasEffect(RPR.Buffs.Soulsow))
                        || (InCombat() && HasEffect(RPR.Buffs.Soulsow) && HasRaidBuffs()))
                {
                    return OriginalHook(RPR.Soulsow);
                }
            }

            if (level >= RPR.Levels.PlentifulHarvest
                && gauge.Shroud <= 50
                && gauge.EnshroudedTimeRemaining == 0
                && !HasEffect(RPR.Buffs.SoulReaver)
                && !HasEffect(RPR.Buffs.BloodsownCircle)
                && HasEffect(RPR.Buffs.ImmortalSacrifice))
            {
                return RPR.PlentifulHarvest;
            }

            if ((deathsDesign is null && ShouldRefreshDots())

                    || (deathsDesign is not null && deathsDesign.RemainingTime <= 15))
            {
                return RPR.ShadowOfDeath;
            }

            if (needSoulSlice
                && gauge.Soul <= 50)
                return RPR.SoulSlice;

            if (comboTime > 0)
            {
                if (lastComboMove == RPR.WaxingSlice && level >= RPR.Levels.InfernalSlice)
                    return RPR.InfernalSlice;

                if (lastComboMove == RPR.Slice && level >= RPR.Levels.WaxingSlice)
                    return RPR.WaxingSlice;
            }

            return RPR.Slice;
        }

        return actionID;
    }
}

internal class ReaperScythe : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RPR.NightmareScythe)
        {
            var gauge = GetJobGauge<RPRGauge>();

            var deathsDesign = FindTargetEffect(RPR.Debuffs.DeathsDesign);

            var doSoulScythe = level >= RPR.Levels.SoulScythe
                && HasCharges(RPR.SoulScythe);

            if (GCDClipCheck(actionID) && !HasEffect(RPR.Buffs.SoulReaver))
            {
                if (level >= RPR.Levels.ArcaneCircle
                    && IsOffCooldown(RPR.ArcaneCircle)
                    && HasRaidBuffs())
                {
                    return RPR.ArcaneCircle;
                }

                if (level >= RPR.Levels.Gluttony
                    && IsOffCooldown(RPR.Gluttony)
                    && gauge.Soul >= 50)
                {
                    return RPR.Gluttony;
                }

                if (level >= RPR.Levels.BloodStalk
                    && (gauge.Soul >= 50 || gauge.VoidShroud >= 2))
                {
                    return level >= RPR.Levels.GrimSwathe ? OriginalHook(RPR.GrimSwathe) : OriginalHook(RPR.BloodStalk);
                }

                if (level >= RPR.Levels.Enshroud
                    && IsOffCooldown(RPR.Enshroud)
                    && (gauge.Shroud >= 50))
                {
                    return RPR.Enshroud;
                }
            }

            if (level >= RPR.Levels.PlentifulHarvest
                && gauge.Shroud <= 50
                && gauge.EnshroudedTimeRemaining == 0
                && !HasEffect(RPR.Buffs.SoulReaver)
                && !HasEffect(RPR.Buffs.BloodsownCircle)
                && HasEffect(RPR.Buffs.ImmortalSacrifice))
            {
                return RPR.PlentifulHarvest;
            }

            if (level >= RPR.Levels.Soulsow)
            {
                if ((!InCombat() && !HasEffect(RPR.Buffs.Soulsow))
                        || (InCombat() && HasEffect(RPR.Buffs.Soulsow) && deathsDesign is not null))
                {
                    return OriginalHook(RPR.Soulsow);
                }
            }

            if (level >= RPR.Levels.Communio && gauge.LemureShroud == 1)
                return RPR.Communio;

            if (HasEffect(RPR.Buffs.SoulReaver) || gauge.EnshroudedTimeRemaining > 0)
            {
                return OriginalHook(RPR.Guillotine);
            }

            if ((deathsDesign is null && ShouldRefreshDots())
                    || (deathsDesign is not null && deathsDesign.RemainingTime <= 15))
            {
                return RPR.WhorlOfDeath;
            }

            if (doSoulScythe && gauge.Soul <= 50) return RPR.SoulScythe;

            if (comboTime > 0)
            {
                if (lastComboMove == RPR.SpinningScythe && level >= RPR.Levels.NightmareScythe)
                    return RPR.NightmareScythe;
            }

            return RPR.SpinningScythe;
        }

        return actionID;
    }
}

internal class ReaperShadowOfDeath : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RPR.ShadowOfDeath)
        {
            var gauge = GetJobGauge<RPRGauge>();

            if (IsEnabled(CustomComboPreset.ReaperShadowSoulsowFeature))
            {
                if (level >= RPR.Levels.Soulsow && !InCombat() && !HasTarget() && !HasEffect(RPR.Buffs.Soulsow))
                    return RPR.Soulsow;
            }

            if (level >= RPR.Levels.Enshroud && gauge.EnshroudedTimeRemaining > 0)
            {
                if (IsEnabled(CustomComboPreset.ReaperShadowLemuresFeature))
                {
                    if (level >= RPR.Levels.EnhancedShroud && gauge.VoidShroud >= 2)
                        return RPR.LemuresSlice;
                }

                if (IsEnabled(CustomComboPreset.ReaperShadowCommunioFeature))
                {
                    if (level >= RPR.Levels.Communio && gauge.LemureShroud == 1)
                        return RPR.Communio;
                }
            }

            if ((level >= RPR.Levels.SoulReaver && HasEffect(RPR.Buffs.SoulReaver)) ||
                (level >= RPR.Levels.Enshroud && gauge.EnshroudedTimeRemaining > 0))
            {
                if (IsEnabled(CustomComboPreset.ReaperShadowGallowsFeature))
                    // Cross Reaping
                    return OriginalHook(RPR.Gallows);

                if (IsEnabled(CustomComboPreset.ReaperShadowGibbetFeature))
                    // Void Reaping
                    return OriginalHook(RPR.Gibbet);
            }
        }

        return actionID;
    }
}

internal class ReaperSoulSlice : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RPR.SoulSlice)
        {
            var gauge = GetJobGauge<RPRGauge>();

            if (level >= RPR.Levels.Enshroud && gauge.EnshroudedTimeRemaining > 0)
            {
                if (IsEnabled(CustomComboPreset.ReaperSoulLemuresFeature))
                {
                    if (level >= RPR.Levels.EnhancedShroud && gauge.VoidShroud >= 2)
                        return RPR.LemuresSlice;
                }

                if (IsEnabled(CustomComboPreset.ReaperSoulCommunioFeature))
                {
                    if (level >= RPR.Levels.Communio && gauge.LemureShroud == 1)
                        return RPR.Communio;
                }
            }

            if ((level >= RPR.Levels.SoulReaver && HasEffect(RPR.Buffs.SoulReaver)) ||
                (level >= RPR.Levels.Enshroud && gauge.EnshroudedTimeRemaining > 0))
            {
                if (IsEnabled(CustomComboPreset.ReaperSoulGallowsFeature))
                    // Cross Reaping
                    return OriginalHook(RPR.Gallows);

                if (IsEnabled(CustomComboPreset.ReaperSoulGibbetFeature))
                    // Void Reaping
                    return OriginalHook(RPR.Gibbet);
            }

            if (IsEnabled(CustomComboPreset.ReaperSoulOvercapFeature))
            {
                if (IsEnabled(CustomComboPreset.ReaperBloodStalkGluttonyFeature))
                {
                    if (level >= RPR.Levels.Gluttony && gauge.Soul >= 50 && gauge.EnshroudedTimeRemaining == 0 && IsOffCooldown(RPR.Gluttony))
                        return RPR.Gluttony;
                }

                if (level >= RPR.Levels.BloodStalk && gauge.Soul > 50 && gauge.EnshroudedTimeRemaining == 0)
                    // Unveiled Gibbet and Gallows
                    return OriginalHook(RPR.BloodStalk);
            }
        }

        return actionID;
    }
}

internal class ReaperSoulScythe : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RPR.SoulScythe)
        {
            var gauge = GetJobGauge<RPRGauge>();

            if (IsEnabled(CustomComboPreset.ReaperSoulScytheOvercapFeature))
            {
                if (IsEnabled(CustomComboPreset.ReaperGrimSwatheGluttonyFeature))
                {
                    if (level >= RPR.Levels.Gluttony && gauge.Soul >= 50 && gauge.EnshroudedTimeRemaining == 0 && IsOffCooldown(RPR.Gluttony))
                        return RPR.Gluttony;
                }

                if (level >= RPR.Levels.GrimSwathe && gauge.Soul > 50 && gauge.EnshroudedTimeRemaining == 0)
                    return RPR.GrimSwathe;
            }
        }

        return actionID;
    }
}

internal class ReaperBloodStalk : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RPR.BloodStalk)
        {
            var gauge = GetJobGauge<RPRGauge>();

            if (IsEnabled(CustomComboPreset.ReaperBloodStalkGluttonyFeature))
            {
                if (level >= RPR.Levels.Gluttony && gauge.Soul >= 50 && IsOffCooldown(RPR.Gluttony))
                    return RPR.Gluttony;
            }
        }

        return actionID;
    }
}

internal class ReaperGrimSwathe : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RPR.GrimSwathe)
        {
            var gauge = GetJobGauge<RPRGauge>();

            if (IsEnabled(CustomComboPreset.ReaperGrimSwatheGluttonyFeature))
            {
                if (level >= RPR.Levels.Gluttony && gauge.Soul >= 50 && IsOffCooldown(RPR.Gluttony))
                    return RPR.Gluttony;
            }
        }

        return actionID;
    }
}

internal class ReaperGibbetGallowsGuillotine : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RPR.Gibbet || actionID == RPR.Gallows)
        {
            var gauge = GetJobGauge<RPRGauge>();

            if ((level >= RPR.Levels.SoulReaver && HasEffect(RPR.Buffs.SoulReaver)) ||
                (level >= RPR.Levels.Enshroud && gauge.EnshroudedTimeRemaining > 0))
            {
                if (IsEnabled(CustomComboPreset.ReaperLemuresSoulReaverFeature))
                {
                    if (level >= RPR.Levels.EnhancedShroud && gauge.VoidShroud >= 2)
                        return RPR.LemuresSlice;
                }

                if (IsEnabled(CustomComboPreset.ReaperCommunioSoulReaverFeature))
                {
                    if (level >= RPR.Levels.Communio && gauge.LemureShroud == 1)
                        return RPR.Communio;
                }

                if (IsEnabled(CustomComboPreset.ReaperEnhancedEnshroudedFeature))
                {
                    if (HasEffect(RPR.Buffs.EnhancedVoidReaping))
                        return RPR.VoidReaping;

                    if (HasEffect(RPR.Buffs.EnhancedCrossReaping))
                        return RPR.CrossReaping;
                }

                if (IsEnabled(CustomComboPreset.ReaperEnhancedSoulReaverFeature))
                {
                    if (HasEffect(RPR.Buffs.EnhancedGibbet))
                        // Void Reaping
                        return OriginalHook(RPR.Gibbet);

                    if (HasEffect(RPR.Buffs.EnhancedGallows))
                        // Cross Reaping
                        return OriginalHook(RPR.Gallows);
                }
            }
        }

        if (actionID == RPR.Guillotine)
        {
            var gauge = GetJobGauge<RPRGauge>();

            if (level >= RPR.Levels.Enshroud && gauge.EnshroudedTimeRemaining > 0)
            {
                if (IsEnabled(CustomComboPreset.ReaperLemuresSoulReaverFeature))
                {
                    if (level >= RPR.Levels.LemuresScythe && gauge.VoidShroud >= 2)
                        return RPR.LemuresScythe;
                }

                if (IsEnabled(CustomComboPreset.ReaperCommunioSoulReaverFeature))
                {
                    if (level >= RPR.Levels.Communio && gauge.LemureShroud == 1)
                        return RPR.Communio;
                }
            }
        }

        return actionID;
    }
}

internal class ReaperEnshroud : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RPR.Enshroud)
        {
            var gauge = GetJobGauge<RPRGauge>();

            if (IsEnabled(CustomComboPreset.ReaperEnshroudCommunioFeature))
            {
                if (level >= RPR.Levels.Communio && gauge.EnshroudedTimeRemaining > 0)
                    return RPR.Communio;
            }
        }

        return actionID;
    }
}

internal class ReaperArcaneCircle : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RPR.ArcaneCircle)
        {
            if (IsEnabled(CustomComboPreset.ReaperHarvestFeature))
            {
                if (level >= RPR.Levels.PlentifulHarvest && HasEffect(RPR.Buffs.ImmortalSacrifice))
                    return RPR.PlentifulHarvest;
            }
        }

        return actionID;
    }
}

internal class ReaperHellsIngressEgress : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RPR.HellsEgress || actionID == RPR.HellsIngress)
        {
            if (IsEnabled(CustomComboPreset.ReaperRegressFeature))
            {
                if (level >= RPR.Levels.Regress)
                {
                    if (IsEnabled(CustomComboPreset.ReaperRegressOption))
                    {
                        var threshold = FindEffect(RPR.Buffs.Threshold);

                        if (threshold != null && threshold.RemainingTime <= 8.5)
                            return RPR.Regress;
                    }
                    else
                    {
                        if (HasEffect(RPR.Buffs.Threshold))
                            return RPR.Regress;
                    }
                }
            }
        }

        return actionID;
    }
}

internal class ReaperHarpe : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == RPR.Harpe)
        {
            if (IsEnabled(CustomComboPreset.ReaperHarpeHarvestSoulsowFeature))
            {
                if (level >= RPR.Levels.Soulsow && !HasEffect(RPR.Buffs.Soulsow) && (!InCombat() || !HasTarget()))
                    return RPR.Soulsow;
            }

            if (IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonFeature))
            {
                if (level >= RPR.Levels.HarvestMoon && HasEffect(RPR.Buffs.Soulsow))
                {
                    if (IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonEnhancedFeature))
                    {
                        if (HasEffect(RPR.Buffs.EnhancedHarpe))
                            return RPR.Harpe;
                    }

                    if (IsEnabled(CustomComboPreset.ReaperHarpeHarvestMoonCombatFeature))
                    {
                        if (OutOfCombat())
                            return RPR.Harpe;
                    }

                    return RPR.HarvestMoon;
                }
            }
        }

        return actionID;
    }
}
