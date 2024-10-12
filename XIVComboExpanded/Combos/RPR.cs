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
                && (GetCooldown(RPR.SoulSlice).TotalCooldownRemaining <= 15
                    || HasEffect(RPR.Buffs.ArcaneCircle)
                    || HasRaidBuffs(2)
                );

            var idealHost = FindEffect(RPR.Buffs.IdealHost);

            if (GCDClipCheck(actionID) && deathsDesign is not null)
            {
                switch (level)
                {
                    case >= RPR.Levels.ArcaneCircle when
                        IsOffCooldown(RPR.ArcaneCircle)
                        && HasRaidBuffs(2):
                        return RPR.ArcaneCircle;

                    case >= RPR.Levels.Sacrificium when
                        CanUseAction(RPR.Sacrificium):
                        return RPR.Sacrificium;

                    case >= RPR.Levels.Gluttony when
                        IsOffCooldown(RPR.Gluttony)
                        && (level < RPR.Levels.ArcaneCircle || IsOnCooldown(RPR.ArcaneCircle))
                        && !HasEffect(RPR.Buffs.SoulReaver)
                        && !HasEffect(RPR.Buffs.Executioner)
                        && gauge.Shroud <= 80
                        && gauge.EnshroudedTimeRemaining == 0
                        && (HasEffect(RPR.Buffs.ArcaneCircle)
                            || HasRaidBuffs(2)
                            || GetCooldown(RPR.ArcaneCircle).CooldownRemaining >= 3)
                        && gauge.Soul >= 50:
                        return RPR.Gluttony;

                    case >= RPR.Levels.BloodStalk when
                        (gauge.Soul >= 50
                        && (level < RPR.Levels.ArcaneCircle || IsOnCooldown(RPR.ArcaneCircle))
                        && !HasEffect(RPR.Buffs.SoulReaver)
                        && !HasEffect(RPR.Buffs.Executioner)
                        && (gauge.Soul >= 90
                             || needSoulSlice
                             || HasEffect(RPR.Buffs.ArcaneCircle)
                             || HasRaidBuffs(2)))
                        || gauge.VoidShroud >= 2:
                        return OriginalHook(RPR.BloodStalk);

                    case >= RPR.Levels.Enshroud when
                        (gauge.Shroud >= 50 || idealHost is not null)
                        && CanUseAction(RPR.Enshroud)
                        && (gauge.Shroud >= 80
                            || idealHost is not null
                            || HasEffect(RPR.Buffs.ArcaneCircle)
                            || HasRaidBuffs(2))
                        && IsOffCooldown(RPR.Enshroud):
                        return RPR.Enshroud;
                }
            }

            if (level >= RPR.Levels.Communio
                && deathsDesign is not null
                && gauge.LemureShroud == 1)
                return RPR.Communio;

            if (HasEffect(RPR.Buffs.SoulReaver)
                || HasEffect(RPR.Buffs.Executioner)
                || gauge.EnshroudedTimeRemaining > 0)
            {
                if ((HasEffect(RPR.Buffs.EnhancedGibbet) && gauge.LemureShroud < 1)
                    || HasEffect(RPR.Buffs.EnhancedVoidReaping))
                    return OriginalHook(RPR.Gibbet);
                return OriginalHook(RPR.Gallows);
            }

            if (level >= RPR.Levels.Soulsow
                && ((!InCombat() && !HasEffect(RPR.Buffs.Soulsow))
                    || (InCombat()
                        && HasEffect(RPR.Buffs.Soulsow)
                        && deathsDesign is not null
                        && (HasRaidBuffs(2) || HasEffect(RPR.Buffs.ArcaneCircle)))))
                return OriginalHook(RPR.Soulsow);

            if (deathsDesign is null || deathsDesign.RemainingTime <= 15)
            {
                if (ShouldUseDots())
                    return RPR.ShadowOfDeath;
            }

            if (deathsDesign is not null)
            {

                var immortalSacrifice = FindEffect(RPR.Buffs.ImmortalSacrifice);

                if (level >= RPR.Levels.PlentifulHarvest
                    && CanUseAction(RPR.PlentifulHarvest)
                    && immortalSacrifice is not null
                   // && (immortalSacrifice.StackCount == PartyList.Length
                   //     || immortalSacrifice.StackCount == 8
                   //     || immortalSacrifice?.RemainingTime <= 25
                   //     )
                   )
                    return RPR.PlentifulHarvest;

                if (needSoulSlice && gauge.Soul <= 50)
                    return RPR.SoulSlice;
            }

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

            if (GCDClipCheck(actionID))
            {
                if (level >= RPR.Levels.BloodStalk && (gauge.Soul >= 50 || gauge.VoidShroud >= 2))
                    return level >= RPR.Levels.GrimSwathe
                        ? OriginalHook(RPR.GrimSwathe)
                        : OriginalHook(RPR.BloodStalk);

                switch (level)
                {
                    case >= RPR.Levels.Sacrificium when
                        CanUseAction(RPR.Sacrificium):
                        return RPR.Sacrificium;

                    case >= RPR.Levels.BloodStalk when
                        !HasEffect(RPR.Buffs.SoulReaver)
                        && !HasEffect(RPR.Buffs.Executioner)
                        && (gauge.Soul >= 50
                            || gauge.VoidShroud >= 2):
                        return OriginalHook(RPR.BloodStalk);
                    case >= RPR.Levels.ArcaneCircle when
                        IsOffCooldown(RPR.ArcaneCircle)
                        && HasRaidBuffs(2):
                        return RPR.ArcaneCircle;

                    case >= RPR.Levels.Enshroud when
                        (gauge.Shroud >= 50 || HasEffect(RPR.Buffs.IdealHost))
                        && gauge.EnshroudedTimeRemaining == 0
                        && IsOffCooldown(RPR.Enshroud):
                        return RPR.Enshroud;

                    case >= RPR.Levels.Gluttony when
                        IsOffCooldown(RPR.Gluttony)
                        && !HasEffect(RPR.Buffs.SoulReaver)
                        && !HasEffect(RPR.Buffs.Executioner)
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

            if (level >= RPR.Levels.Communio && gauge.LemureShroud == 1)
                return RPR.Communio;

            if (deathsDesign is null || deathsDesign.RemainingTime <= 15)
            {
                if (ShouldUseDots())
                    return RPR.WhorlOfDeath;
            }

            if (HasEffect(RPR.Buffs.SoulReaver) || HasEffect(RPR.Buffs.Executioner) || gauge.EnshroudedTimeRemaining > 0)
                return OriginalHook(RPR.Guillotine);

            var immortalSacrifice = FindEffect(RPR.Buffs.ImmortalSacrifice);

            if (level >= RPR.Levels.PlentifulHarvest
                && CanUseAction(RPR.PlentifulHarvest)
                && immortalSacrifice is not null
                && (immortalSacrifice.StackCount == PartyList.Length
                    || immortalSacrifice?.RemainingTime <= 20)
               )
                return RPR.PlentifulHarvest;

            if (doSoulScythe && gauge.Soul <= 50)
                return RPR.SoulScythe;

            if (
                (deathsDesign is null)
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