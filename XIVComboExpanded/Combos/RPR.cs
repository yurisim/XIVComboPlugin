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
        Sacrificium = 36969,
        Perfectio = 36973,
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
        public const ushort EnhancedHarpe = 2845,
            SoulReaver = 2587,
            Executioner = 3858,
            EnhancedGibbet = 2588,
            EnhancedGallows = 2589,
            EnhancedVoidReaping = 2590,
            EnhancedCrossReaping = 2591,
            IdealHost = 3905,
            ImmortalSacrifice = 2592,
            Enshrouded = 2593,
            Soulsow = 2594,
            ArcaneCircle = 2599,
            BloodsownCircle = 2972,
            Threshold = 2595,
            Oblatio = 3857, // Sacrificium ready to use
            PerfectioOcculta = 3859, // Turns into Perfectio Parata when Communio is used
            PerfectioParata = 3860; // Perfectio ready to use
    }

    public static class Debuffs
    {
        public const ushort DeathsDesign = 2586;
    }

    public static class Levels
    {
        public const byte WaxingSlice = 5,
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
            Communio = 90,
            Sacrificium = 92,
            Executioner = 96,
            Perfectio = 100;
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

            var needSoulSlice =
                level >= RPR.Levels.SoulSlice
                && HasCharges(RPR.SoulSlice)
                && (GetCooldown(RPR.SoulSlice).TotalCooldownRemaining <= 6
                    || HasEffect(RPR.Buffs.ArcaneCircle)
                    || HasRaidBuffs()
                );

            var idealHost = FindEffect(RPR.Buffs.IdealHost);

            if (GCDClipCheck(actionID) && !HasEffect(RPR.Buffs.SoulReaver))
                switch (level)
                {
                    case >= RPR.Levels.BloodStalk when
                        (gauge.Soul >= 50
                         && (gauge.Soul >= 90
                             || needSoulSlice
                             || HasEffect(RPR.Buffs.ArcaneCircle)
                             || HasRaidBuffs()))
                        || gauge.VoidShroud >= 2:
                        return OriginalHook(RPR.BloodStalk);

                    case >= RPR.Levels.ArcaneCircle when
                        IsOffCooldown(RPR.ArcaneCircle)
                        && HasRaidBuffs():
                        return RPR.ArcaneCircle;

                    case >= RPR.Levels.Enshroud when
                        (gauge.Shroud >= 50 || idealHost is not null)
                        && gauge.EnshroudedTimeRemaining == 0
                        && (gauge.Shroud >= 80
                            || idealHost?.RemainingTime <= 10
                            || HasEffect(RPR.Buffs.ArcaneCircle)
                            || HasRaidBuffs())
                        && IsOffCooldown(RPR.Enshroud):
                        return RPR.Enshroud;

                    case >= RPR.Levels.Gluttony when
                        IsOffCooldown(RPR.Gluttony)
                        && gauge.Shroud <= 80
                        && gauge.EnshroudedTimeRemaining == 0
                        && (HasEffect(RPR.Buffs.ArcaneCircle)
                            || HasRaidBuffs()
                            || GetCooldown(RPR.ArcaneCircle).CooldownRemaining >= 3)
                        && gauge.Soul >= 50:
                        return RPR.Gluttony;
                }

            if (deathsDesign is not null)
            {
                if (level >= RPR.Levels.Communio
                    && gauge.LemureShroud == 1)
                    return RPR.Communio;

                if (level >= RPR.Levels.PlentifulHarvest
                    && gauge.Shroud <= 50
                    && gauge.EnshroudedTimeRemaining == 0
                    && !HasEffect(RPR.Buffs.SoulReaver)
                    && !HasEffect(RPR.Buffs.BloodsownCircle)
                    && HasEffect(RPR.Buffs.ImmortalSacrifice)
                   )
                    return RPR.PlentifulHarvest;

                if (HasEffect(RPR.Buffs.SoulReaver) || gauge.EnshroudedTimeRemaining > 0)
                {
                    if ((HasEffect(RPR.Buffs.EnhancedGibbet) && gauge.LemureShroud < 1)
                        || HasEffect(RPR.Buffs.EnhancedVoidReaping))
                        return OriginalHook(RPR.Gibbet);
                    return OriginalHook(RPR.Gallows);
                }

                if (needSoulSlice && gauge.Soul <= 50)
                    return RPR.SoulSlice;

            }

            if (level >= RPR.Levels.Soulsow
                && ((!InCombat() && !HasEffect(RPR.Buffs.Soulsow))
                    || (InCombat()
                        && HasEffect(RPR.Buffs.Soulsow)
                        && (HasRaidBuffs() || HasEffect(RPR.Buffs.ArcaneCircle)))))
                return OriginalHook(RPR.Soulsow);

            if ((deathsDesign is null && ShouldUseDots())
                || (deathsDesign is not null && deathsDesign.RemainingTime <= 20))
                return RPR.ShadowOfDeath;

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

            var doSoulScythe = level >= RPR.Levels.SoulScythe && HasCharges(RPR.SoulScythe);

            if (GCDClipCheck(actionID) && !HasEffect(RPR.Buffs.SoulReaver))
            {
                if (level >= RPR.Levels.BloodStalk && (gauge.Soul >= 50 || gauge.VoidShroud >= 2))
                    return level >= RPR.Levels.GrimSwathe
                        ? OriginalHook(RPR.GrimSwathe)
                        : OriginalHook(RPR.BloodStalk);

                switch (level)
                {
                    case >= RPR.Levels.BloodStalk when
                        gauge.Soul >= 50
                        || gauge.VoidShroud >= 2:
                        return OriginalHook(RPR.BloodStalk);
                    case >= RPR.Levels.ArcaneCircle when
                        IsOffCooldown(RPR.ArcaneCircle)
                        && HasRaidBuffs():
                        return RPR.ArcaneCircle;

                    case >= RPR.Levels.Enshroud when
                        gauge.Shroud >= 50 || (HasEffect(RPR.Buffs.IdealHost)
                                               && gauge.EnshroudedTimeRemaining == 0
                                               && IsOffCooldown(RPR.Enshroud)):
                        return RPR.Enshroud;

                    case >= RPR.Levels.Gluttony when
                        IsOffCooldown(RPR.Gluttony)
                        && gauge.EnshroudedTimeRemaining == 0
                        && gauge.Soul >= 50:
                        return RPR.Gluttony;
                }
            }


            if (level >= RPR.Levels.Soulsow)
                if (
                    (!InCombat() && !HasEffect(RPR.Buffs.Soulsow))
                    || (InCombat() && HasEffect(RPR.Buffs.Soulsow) && deathsDesign is not null)
                )
                    return OriginalHook(RPR.Soulsow);

            if (
                (deathsDesign is null && ShouldUseDots())
                || (deathsDesign is not null && deathsDesign.RemainingTime <= 15)
            )
                return RPR.WhorlOfDeath;

            if (level >= RPR.Levels.Communio && gauge.LemureShroud == 1)
                return RPR.Communio;

            if (HasEffect(RPR.Buffs.SoulReaver) || gauge.EnshroudedTimeRemaining > 0)
                return OriginalHook(RPR.Guillotine);


            if (
                level >= RPR.Levels.PlentifulHarvest
                && gauge.Shroud <= 50
                && gauge.EnshroudedTimeRemaining == 0
                && !HasEffect(RPR.Buffs.SoulReaver)
                && !HasEffect(RPR.Buffs.BloodsownCircle)
                && HasEffect(RPR.Buffs.ImmortalSacrifice)
            )
                return RPR.PlentifulHarvest;


            if (doSoulScythe && gauge.Soul <= 50)
                return RPR.SoulScythe;

            if (
                (deathsDesign is null && ShouldUseDots())
                || (deathsDesign is not null && deathsDesign.RemainingTime <= 20)
            )
                return RPR.WhorlOfDeath;

            if (comboTime > 0)
                if (lastComboMove == RPR.SpinningScythe && level >= RPR.Levels.NightmareScythe)
                    return RPR.NightmareScythe;

            return RPR.SpinningScythe;
        }

        return actionID;
    }
}

internal class ReaperCommunioPositional : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RprAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID is RPR.Gallows or RPR.Gibbet)
        {
            var gauge = GetJobGauge<RPRGauge>();

            if (level >= RPR.Levels.Communio && gauge.LemureShroud == 1) return RPR.Communio;
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
                if (level >= RPR.Levels.Communio && gauge.EnshroudedTimeRemaining > 0)
                    return RPR.Communio;
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

        return actionID;
    }
}