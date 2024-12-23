using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace XIVComboExpandedPlugin.Combos;

internal static class PCT
{
    public const byte JobID = 42;

    public const uint FireRed = 34650,
        AeroGreen = 34651,
        WaterBlue = 34652,
        BlizzardCyan = 34653,
        EarthYellow = 34654,
        ThunderMagenta = 34655,
        ExtraFireRed = 34656,
        ExtraAeroGreen = 34657,
        ExtraWaterBlue = 34658,
        ExtraBlizzardCyan = 34659,
        ExtraEarthYellow = 34660,
        ExtraThunderMagenta = 34661,
        HolyWhite = 34662,
        LivingMuse = 35347,
        CometBlack = 34663,
        PomMotif = 34664,
        WingMotif = 34665,
        ClawMotif = 34666,
        MawMotif = 34667,
        HammerMotif = 34668,
        StarrySkyMotif = 34669,
        PomMuse = 34670,
        WingedMuse = 34671,
        ClawedMuse = 34672,
        FangedMuse = 34673,
        StrikingMuse = 34674,
        StarryMuse = 34675,
        MogOftheAges = 34676,
        Retribution = 34677,
        HammerStamp = 34678,
        HammerBrush = 34679,
        PolishingHammer = 34680,
        StarPrism = 34681,
        SteelMuse = 35348,
        SubtractivePalette = 34683,
        Smudge = 34684,
        TemperaCoat = 34685,
        TemperaGrassa = 34686,
        ScenicMuse = 35349,
        RainbowDrip = 34688,
        CreatureMotif = 34689,
        WeaponMotif = 34690,
        LandscapeMotif = 34691,
        CreatureMotifDrawn = 35347,
        WeaponMotifDrawn = 35348,
        LandscapeMotifDrawn = 35349;

    public static class Buffs
    {
        public const ushort SubtractivePalette = 3674,
            StarryMuse = 3685,
            Chroma2Ready = 3675,
            Chroma3Ready = 3676,
            RainbowReady = 3679,
            HammerTime = 3680,
            Inspiration = 3689,
            StarPrismReady = 3681,
            Hyperphantasia = 3688,
            ArtisticInstallation = 3689,
            SubtractiveSpectrum = 3690,
            Monochrome = 3691;
    }

    public static class Debuffs
    {
        public const ushort Placeholder = 0;
    }

    private static class Levels
    {
        public const byte Smudge = 20,
            ExtraFireRed = 25,
            CreatureMotif = 30,
            PomMotif = 30,
            WingMotif = 30,
            PomMuse = 30,
            WingedMuse = 30,
            MogOftheAges = 30,
            ExtraAeroGreen = 35,
            ExtraWaterBlue = 45,
            HammerMotif = 50,
            HammerStamp = 50,
            WeaponMotif = 50,
            StrikingMuse = 50,
            SubtractivePalette = 60,
            BlizzardCyan = 60,
            EarthYellow = 60,
            ThunderMagenta = 60,
            ExtraBlizzardCyan = 60,
            ExtraEarthYellow = 60,
            ExtraThunderMagenta = 60,
            StarrySkyMotif = 70,
            LandscapeMotif = 70,
            MiracleWhite = 80,
            HammerBrush = 86,
            PolishingHammer = 86,
            TemperaGrassa = 88,
            CometBlack = 90,
            RainbowDrip = 92,
            ClawMotif = 96,
            MawMotif = 96,
            ClawedMuse = 96,
            FangedMuse = 96,
            StarryMuse = 70,
            Retribution = 96,
            StarPrism = 100;
    }

    internal class PictomancerSTCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PctAny;

        protected override uint Invoke(
            uint actionID,
            uint lastComboMove,
            float comboTime,
            byte level
        )
        {
            var gauge = GetJobGauge<PCTGauge>();

            if (actionID is FireRed or HolyWhite)
            {
                var hasRaidBuffs = HasRaidBuffs(2);

                var cooldownBuffs =
                    HasEffect(Buffs.StarryMuse) || hasRaidBuffs || level < Levels.StarryMuse;

                var canUsePalette =
                    (HasEffect(Buffs.SubtractiveSpectrum) || gauge.PalleteGauge >= 50)
                    && (cooldownBuffs || gauge.PalleteGauge >= 100);

                var starryCD = GetCooldown(StarryMuse).TotalCooldownRemaining;

                var needToUseHammer =
                    GetCooldown(OriginalHook(SteelMuse)).TotalCooldownRemaining < starryCD;

                var needToUseLivingMuse =
                    GetCooldown(OriginalHook(LivingMuse)).TotalCooldownRemaining < starryCD;

                var hyperphantasia = FindEffect(Buffs.Hyperphantasia);

                if (
                    HasEffect(Buffs.SubtractiveSpectrum)
                    && CanUseAction(SubtractivePalette)
                    && !HasEffect(Buffs.Monochrome)
                )
                {
                    return SubtractivePalette;
                }

                if (GCDClipCheck(actionID))
                {
                    var reprisal = FindTargetEffectAny(ADV.Debuffs.Reprisal);
                    var reprisalFound = reprisal is not null && reprisal.RemainingTime >= 11;

                    switch (level)
                    {
                        case >= Levels.SubtractivePalette
                            when canUsePalette
                                && CanUseAction(SubtractivePalette)
                                && !CanUseAction(OriginalHook(CometBlack)):
                            return SubtractivePalette;
                        // case >= Levels.StarryMuse
                        //     when hasRaidBuffs
                        //         && gauge.LandscapeMotifDrawn
                        //         && IsOffCooldown(StarryMuse):
                        //     return StarryMuse;
                        case >= ADV.Levels.Addle
                            when IsOffCooldown(ADV.Addle)
                                && !TargetHasEffectAny(ADV.Debuffs.Addle)
                                && reprisalFound:
                            return ADV.Addle;
                        case >= Levels.WeaponMotif
                            when IsAvailable(OriginalHook(SteelMuse))
                                && gauge.WeaponMotifDrawn
                                && InCombat()
                                && (level < Levels.StarryMuse || starryCD >= 10)
                                && (
                                    GetCooldown(OriginalHook(SteelMuse)).TotalCooldownRemaining
                                        <= 10
                                    || hasRaidBuffs
                                    || HasEffect(Buffs.StarryMuse)
                                    || level < Levels.HammerBrush
                                // || needToUseHammer
                                ):
                            return OriginalHook(SteelMuse);
                        case >= Levels.MogOftheAges
                            when CanUseAction(OriginalHook(MogOftheAges))
                                && IsOffCooldown(OriginalHook(MogOftheAges))
                                && (
                                    (
                                        gauge.CreatureFlags.HasFlag(CreatureFlags.Pom)
                                        && !gauge.CreatureFlags.HasFlag(CreatureFlags.Wings)
                                    )
                                    || gauge.CreatureFlags.HasFlag(CreatureFlags.Claw)
                                    || cooldownBuffs
                                ):
                            return OriginalHook(MogOftheAges);
                        case >= Levels.CreatureMotif
                            when IsAvailable(OriginalHook(LivingMuse))
                                && gauge.CreatureMotifDrawn
                                && InCombat()
                                && (
                                    cooldownBuffs
                                    || starryCD > 10
                                    || (
                                        (
                                            !gauge.CreatureFlags.HasFlag(CreatureFlags.Pom)
                                            || gauge.CreatureFlags.HasFlag(CreatureFlags.Wings)
                                        ) && !gauge.CreatureFlags.HasFlag(CreatureFlags.Claw)
                                    )
                                )
                                && (cooldownBuffs || needToUseLivingMuse):
                            return OriginalHook(LivingMuse);
                        case >= ADV.Levels.Swiftcast
                            when hyperphantasia is null
                                && !HasEffect(Buffs.HammerTime)
                                && !HasEffect(Buffs.RainbowReady)
                                && cooldownBuffs
                                && HasCharges(OriginalHook(LivingMuse))
                                && !gauge.CreatureMotifDrawn
                                && IsOffCooldown(ADV.Swiftcast):
                            return ADV.Swiftcast;
                        case >= ADV.Levels.LucidDreaming
                            when InCombat()
                                && IsOffCooldown(ADV.LucidDreaming)
                                && LocalPlayer?.CurrentMp <= 7000:
                            return ADV.LucidDreaming;
                    }
                }

                if (
                    HasEffect(Buffs.StarPrismReady)
                    && (HasEffect(Buffs.Inspiration) || !HasEffect(Buffs.Hyperphantasia))
                    && (
                        hyperphantasia?.StackCount < 3
                        // || IsMoving
                        || FindEffect(Buffs.StarPrismReady)?.RemainingTime <= 15
                    )
                )
                {
                    return StarPrism;
                }

                if (
                    level >= Levels.HammerStamp
                    && HasEffect(Buffs.HammerTime)
                    && !HasEffect(Buffs.Inspiration)
                )
                {
                    return OriginalHook(HammerStamp);
                }

                if (HasEffect(Buffs.RainbowReady))
                    return RainbowDrip;

                if (
                    CanUseAction(CometBlack)
                    && (actionID is HolyWhite || canUsePalette || cooldownBuffs)
                )
                    return CometBlack;

                var availableSkill = new (
                    uint Level,
                    uint skill,
                    float CD,
                    bool MotifNeeded,
                    uint motifSkill
                )[]
                {
                    (
                        Levels.LandscapeMotif,
                        ScenicMuse,
                        GetCooldown(OriginalHook(ScenicMuse)).TotalCooldownRemaining,
                        !(gauge.LandscapeMotifDrawn || HasEffect(Buffs.StarryMuse)),
                        LandscapeMotif
                    ),
                    (
                        Levels.WeaponMotif,
                        SteelMuse,
                        GetCooldown(OriginalHook(SteelMuse)).TotalCooldownRemaining,
                        !(gauge.WeaponMotifDrawn || HasEffect(Buffs.HammerTime)),
                        WeaponMotif
                    ),
                    (
                        Levels.CreatureMotif,
                        LivingMuse,
                        GetCooldown(OriginalHook(LivingMuse)).TotalCooldownRemaining,
                        !gauge.CreatureMotifDrawn,
                        CreatureMotif
                    ),
                }
                    .Where(s => s.Level <= level && s.MotifNeeded)
                    .OrderBy(s => s.CD);

                var swiftCast = HasEffect(ADV.Buffs.Swiftcast);

                var quickSkill = availableSkill.Select(s => s.motifSkill);

                if (swiftCast)
                {
                    if (
                        quickSkill.Contains(CreatureMotif)
                        && HasCharges(LivingMuse)
                        && cooldownBuffs
                    )
                        return CreatureMotif;

                    if (quickSkill.Contains(WeaponMotif) && HasCharges(SteelMuse))
                        return WeaponMotif;

                    return quickSkill.FirstOrDefault();
                }

                if (actionID is HolyWhite)
                {
                    if (IsOffCooldown(ADV.Swiftcast) && quickSkill.Any() && GCDClipCheck(actionID))
                    {
                        return ADV.Swiftcast;
                    }

                    if (gauge.Paint >= 2)
                    {
                        return HolyWhite;
                    }
                }

                if (actionID is FireRed && !HasEffect(Buffs.StarryMuse))
                {
                    var filteredskills = availableSkill
                        .Where(s =>
                            GetCooldown(OriginalHook(s.skill)).TotalCooldownRemaining <= 30
                            || (
                                s.skill is not ScenicMuse
                                && GetCooldown(OriginalHook(ScenicMuse)).TotalCooldownRemaining
                                    <= 20
                            )
                            || (TargetHasLowLife() && IsAvailable(OriginalHook(s.skill)))
                            || !InCombat()
                        )
                        .Select(s => s.motifSkill)
                        .FirstOrDefault();

                    if (filteredskills != default)
                        return OriginalHook(filteredskills);
                }

                return HasEffect(Buffs.SubtractivePalette)
                    ? OriginalHook(BlizzardCyan)
                    : OriginalHook(FireRed);
            }

            return actionID;
        }
    }

    internal class PictomancerAoECombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.PctAny;

        protected override uint Invoke(
            uint actionID,
            uint lastComboMove,
            float comboTime,
            byte level
        )
        {
            var gauge = GetJobGauge<PCTGauge>();

            if (actionID == ExtraFireRed)
            {
                if (GCDClipCheck(actionID))
                    switch (level)
                    {
                        case >= Levels.MogOftheAges
                            when CanUseAction(OriginalHook(MogOftheAges))
                                && IsOffCooldown(MogOftheAges):
                            return OriginalHook(MogOftheAges);
                        case >= Levels.WeaponMotif
                            when gauge.WeaponMotifDrawn
                                && InCombat()
                                && IsAvailable(OriginalHook(SteelMuse)):
                            return OriginalHook(SteelMuse);
                        case >= Levels.CreatureMotif
                            when IsAvailable(OriginalHook(LivingMuse)) && gauge.CreatureMotifDrawn:
                            return OriginalHook(LivingMuse);
                        case >= Levels.SubtractivePalette
                            when (gauge.PalleteGauge >= 50 || HasEffect(Buffs.SubtractiveSpectrum))
                                && !HasEffect(Buffs.SubtractivePalette):
                            return SubtractivePalette;
                        case >= 15
                            when InCombat()
                                && IsOffCooldown(ADV.LucidDreaming)
                                && LocalPlayer?.CurrentMp <= 8000:
                            return ADV.LucidDreaming;
                    }

                if (HasEffect(Buffs.StarPrismReady))
                    return StarPrism;

                if (CanUseAction(OriginalHook(CometBlack)))
                    return OriginalHook(CometBlack);

                if (HasEffect(Buffs.RainbowReady))
                    return RainbowDrip;

                if (
                    level >= Levels.HammerStamp
                    && !HasEffect(Buffs.Inspiration)
                    && HasEffect(Buffs.HammerTime)
                )
                    return OriginalHook(HammerStamp);

                var skills = new (
                    uint Level,
                    bool hasCharges,
                    bool MotifNeeded,
                    uint Skill,
                    float CD
                )[]
                {
                    (
                        Levels.CreatureMotif,
                        HasCharges(OriginalHook(LivingMuse)),
                        !gauge.CreatureMotifDrawn,
                        CreatureMotif,
                        GetCooldown(OriginalHook(LivingMuse)).TotalCooldownRemaining
                    ),
                    (
                        Levels.WeaponMotif,
                        HasCharges(OriginalHook(SteelMuse)),
                        !(gauge.WeaponMotifDrawn || HasEffect(Buffs.HammerTime)),
                        WeaponMotif,
                        GetCooldown(OriginalHook(SteelMuse)).TotalCooldownRemaining
                    ),
                    (
                        Levels.LandscapeMotif,
                        HasCharges(OriginalHook(ScenicMuse)),
                        !(gauge.LandscapeMotifDrawn || HasEffect(Buffs.StarryMuse)),
                        LandscapeMotif,
                        GetCooldown(OriginalHook(ScenicMuse)).TotalCooldownRemaining
                    ),
                }
                    .Where(s => s.Level <= level && s.hasCharges && s.MotifNeeded)
                    .OrderBy(s => s.CD)
                    .Select(s => s.Skill)
                    .FirstOrDefault();

                if (HasEffect(Buffs.SubtractivePalette))
                    return OriginalHook(ExtraBlizzardCyan);

                if (skills != default)
                    return OriginalHook(skills);

                if (gauge.Paint >= 3)
                    return HolyWhite;

                return actionID;
            }

            return actionID;
        }
    }

    internal class PictMotifFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.PctAny;

        protected override uint Invoke(
            uint actionID,
            uint lastComboMove,
            float comboTime,
            byte level
        )
        {
            return actionID switch
            {
                CreatureMotif when OriginalHook(LivingMuse) != LivingMuse && InCombat() =>
                    OriginalHook(LivingMuse),
                WeaponMotif when OriginalHook(SteelMuse) != SteelMuse => OriginalHook(SteelMuse),
                LandscapeMotif when OriginalHook(ScenicMuse) != ScenicMuse => OriginalHook(
                    ScenicMuse
                ),
                _ => actionID,
            };
        }
    }
}
