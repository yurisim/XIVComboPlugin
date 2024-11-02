using System;
using System.Linq;
using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace XIVComboExpandedPlugin.Combos;

internal static class BLM
{
    public const byte ClassID = 7;
    public const byte JobID = 25;

    public const uint Fire = 141,
        Blizzard = 142,
        Thunder = 144,
        Fire2 = 147,
        Transpose = 149,
        Fire3 = 152,
        Thunder3 = 153,
        Blizzard3 = 154,
        Scathe = 156,
        Manafont = 158,
        Freeze = 159,
        Flare = 162,
        LeyLines = 3573,
        Sharpcast = 3574,
        Blizzard4 = 3576,
        Fire4 = 3577,
        BetweenTheLines = 7419,
        Thunder4 = 7420,
        Triplecast = 7421,
        Foul = 7422,
        Amplifier = 25796,
        Thunder2 = 7447,
        Despair = 16505,
        UmbralSoul = 16506,
        Xenoglossy = 16507,
        Blizzard2 = 25793,
        HighFire2 = 25794,
        HighBlizzard2 = 25795,
        Paradox = 25797,
        FlareStar = 36989;

    public static class Buffs
    {
        public const ushort Thunderhead = 3870,
            Firestarter = 165,
            Swiftcast = 167,
            LeyLines = 737,
            Sharpcast = 867,
            Triplecast = 1211,
            EnhancedFlare = 2960;
    }

    public static class Debuffs
    {
        public const ushort Thunder = 161,
            Thunder2 = 162,
            Thunder3 = 163,
            HighThunder = 3871,
            HighThunder2 = 3872,
            Thunder4 = 1210;
    }

    public static class Levels
    {
        public const byte Thunder = 6,
            Blizzard2 = 12,
            Fire2 = 18,
            Thunder2 = 26,
            Manafont = 30,
            Fire3 = 35,
            Blizzard3 = 35,
            Freeze = 40,
            Thunder3 = 45,
            Flare = 50,
            Sharpcast = 54,
            Blizzard4 = 58,
            Fire4 = 60,
            BetweenTheLines = 62,
            Thunder4 = 64,
            Triplecast = 66,
            Foul = 70,
            Despair = 72,
            UmbralSoul = 35,
            Xenoglossy = 80,
            HighFire2 = 82,
            HighBlizzard2 = 82,
            Amplifier = 86,
            EnhancedSharpcast2 = 88,
            Paradox = 90,
            FlareStar = 100;
    }

    public static class MpCosts
    {
        public const ushort Fire2 = 3000,
            Flare = 800;
    }
}

internal class BlackMageFire : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID is BLM.Fire or BLM.Fire2)
        {
            var gauge = GetJobGauge<BLMGauge>();

            var fireCost = GetResourceCost(OriginalHook(BLM.Fire));
            var fire2Cost = GetResourceCost(OriginalHook(BLM.Fire2));

            var playerMP = LocalPlayer?.CurrentMp;
            var hasRaidBuffs = HasRaidBuffs(1);

            var maxPolyglot = level >= BLM.Levels.Xenoglossy ? 2 : 1;

            if (GCDClipCheck(actionID))
            {
                switch (level)
                {
                    case >= BLM.Levels.Triplecast
                        when HasCharges(BLM.Triplecast)
                            && (GetRemainingCharges(BLM.Triplecast) == 2 || hasRaidBuffs)
                            && gauge.UmbralHearts == 3
                            && !HasEffect(BLM.Buffs.Triplecast)
                            && !HasEffect(ADV.Buffs.Swiftcast):
                        return BLM.Triplecast;
                    case >= BLM.Levels.Amplifier
                        when IsOffCooldown(BLM.Amplifier) && gauge.PolyglotStacks != maxPolyglot:
                        return BLM.Amplifier;
                }
            }

            if (
                gauge.PolyglotStacks >= 1
                && (
                    gauge.EnochianTimer <= 10000
                    || hasRaidBuffs
                    || actionID is BLM.Fire2
                    || (
                        level < BLM.Levels.Amplifier
                        || (IsOffCooldown(BLM.Amplifier) && gauge.PolyglotStacks == maxPolyglot)
                    )
                )
                && gauge.ElementTimeRemaining >= 6000
                && (gauge.PolyglotStacks == maxPolyglot || hasRaidBuffs || actionID is BLM.Fire2)
                && level >= BLM.Levels.Foul
            )
            {
                return level >= BLM.Levels.Xenoglossy && actionID is BLM.Fire
                    ? BLM.Xenoglossy
                    : BLM.Foul;
            }

            if (InCombat() && TargetIsEnemy())
            {
                var debuffs = new[]
                {
                    FindTargetEffect(BLM.Debuffs.Thunder),
                    FindTargetEffect(BLM.Debuffs.Thunder3),
                    FindTargetEffect(BLM.Debuffs.HighThunder),
                    FindTargetEffect(BLM.Debuffs.Thunder2),
                    FindTargetEffect(BLM.Debuffs.Thunder4),
                    FindTargetEffect(BLM.Debuffs.HighThunder2),
                };

                if (
                    level >= BLM.Levels.Thunder
                    && HasEffect(BLM.Buffs.Thunderhead)
                    && (actionID is BLM.Fire || level >= BLM.Levels.Thunder2)
                    && gauge.ElementTimeRemaining >= 6000
                    && (
                        debuffs.Any(effect => effect is not null && effect.RemainingTime <= 5)
                        || debuffs.All(effect => effect is null)
                    )
                )
                    return actionID is BLM.Fire ? OriginalHook(BLM.Thunder) : BLM.Thunder2;
            }

            if (gauge.InAstralFire)
            {
                var firestarter = FindEffect(BLM.Buffs.Firestarter);
                var hasLowMP =
                    fireCost > playerMP || (actionID is BLM.Fire2 && fire2Cost > playerMP);

                // Handle low MP situations
                if (hasLowMP)
                {
                    // Try to use Firestarter proc if available for Fire
                    if (firestarter is not null && actionID is BLM.Fire)
                        return BLM.Fire3;

                    // Use Manafont if available
                    if (IsOffCooldown(BLM.Manafont) && level >= BLM.Levels.Manafont)
                        return BLM.Manafont;

                    // Handle single-target Despair
                    if (
                        actionID is BLM.Fire
                        && playerMP > 0
                        && level >= BLM.Levels.Despair
                        && CanUseAction(BLM.Despair)
                    )
                        return BLM.Despair;

                    // Handle AoE Flare
                    if (
                        level >= BLM.Levels.Flare
                        && playerMP < fire2Cost
                        && CanUseAction(BLM.Flare)
                        && (actionID is BLM.Fire2 || level < BLM.Levels.Fire4)
                    )
                    {
                        var hasInstantCast =
                            HasEffect(ADV.Buffs.Swiftcast) || HasEffect(BLM.Buffs.Triplecast);
                        var canCastFlare = playerMP > 0 || IsOffCooldown(BLM.Manafont);

                        // Try to get Swiftcast/Triplecast before Flare
                        if (!hasInstantCast && canCastFlare)
                        {
                            if (IsOffCooldown(ADV.Swiftcast) && level >= ADV.Levels.Swiftcast)
                                return ADV.Swiftcast;

                            if (HasCharges(BLM.Triplecast) && level >= BLM.Levels.Triplecast)
                                return BLM.Triplecast;
                        }
                        // Cast Flare or get MP with Manafont
                        else if (hasInstantCast)
                        {
                            if (playerMP > 0)
                                return BLM.Flare;

                            if (IsOffCooldown(BLM.Manafont))
                                return BLM.Manafont;
                        }
                    }

                    // Transition to ice phase if we can't do anything else
                    return actionID switch
                    {
                        BLM.Fire => level >= BLM.Levels.Blizzard3 ? BLM.Blizzard3 : BLM.Blizzard,
                        BLM.Fire2 => level > BLM.Levels.Blizzard2
                            ? OriginalHook(BLM.Blizzard2)
                            : BLM.Blizzard,
                        _ => actionID,
                    };
                }

                // Handle normal rotation
                if (
                    level >= BLM.Levels.Fire3
                    && firestarter is not null
                    && actionID is BLM.Fire
                    && firestarter.RemainingTime <= 5
                    && !HasEffect(BLM.Buffs.Triplecast)
                )
                {
                    return BLM.Fire3;
                }

                // Handle Astral Fire refresh
                if (gauge.ElementTimeRemaining < 6000 && actionID is BLM.Fire)
                {
                    return level >= BLM.Levels.Fire3 && firestarter is not null
                        ? BLM.Fire3
                        : BLM.Fire;
                }

                // Normal cast priorities
                return actionID switch
                {
                    BLM.Fire => level >= BLM.Levels.Fire4 ? BLM.Fire4 : BLM.Fire,
                    BLM.Fire2 => OriginalHook(BLM.Fire2),
                    _ => actionID,
                };
            }

            if (gauge.InUmbralIce)
            {
                if (
                    level >= BLM.Levels.Fire3
                    && (
                        gauge.UmbralHearts >= 3
                        || (level < BLM.Levels.Blizzard4 && LocalPlayer?.CurrentMp >= 9000)
                    )
                )
                {
                    return actionID is BLM.Fire ? BLM.Fire3 : OriginalHook(BLM.Fire2);
                }

                if (actionID is BLM.Fire)
                {
                    return level >= BLM.Levels.Blizzard4 ? BLM.Blizzard4 : BLM.Blizzard;
                }

                if (actionID is BLM.Fire2)
                {
                    return level >= BLM.Levels.Freeze ? BLM.Freeze : OriginalHook(BLM.Blizzard2);
                }
            }
            if (actionID is BLM.Fire)
            {
                // Occurs for single target rotation
                if (!gauge.InAstralFire && level >= BLM.Levels.Fire3)
                {
                    return (LocalPlayer?.CurrentMp >= 9000) ? BLM.Fire3 : BLM.Blizzard3;
                }
            }

            if (actionID is BLM.Fire2 && !gauge.InAstralFire)
            {
                return LocalPlayer?.CurrentMp >= 9000
                    ? level >= BLM.Levels.Fire2
                        ? OriginalHook(BLM.Fire2)
                        : BLM.Fire
                    : level >= BLM.Levels.Blizzard2
                        ? OriginalHook(BLM.Blizzard2)
                        : BLM.Blizzard;
            }
        }

        return actionID;
    }
}

internal class BlackTranspose : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.BlackTransposeUmbralSoulFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BLM.Transpose)
        {
            var gauge = GetJobGauge<BLMGauge>();
            if (level >= BLM.Levels.UmbralSoul && gauge.IsEnochianActive && gauge.InUmbralIce)
                return BLM.UmbralSoul;
        }

        return actionID;
    }
}

internal class BlackLeyLines : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BLM.LeyLines)
            if (level >= BLM.Levels.BetweenTheLines && HasEffect(BLM.Buffs.LeyLines))
                return BLM.BetweenTheLines;

        return actionID;
    }
}

internal class BlackFire : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.BlackFireFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BLM.Fire)
        {
            var gauge = GetJobGauge<BLMGauge>();

            if (level >= BLM.Levels.Paradox && gauge.IsParadoxActive && gauge.InUmbralIce)
                return BLM.Paradox;

            if (level >= BLM.Levels.Fire3)
            {
                if (IsEnabled(CustomComboPreset.BlackFireOption))
                    if (gauge.AstralFireStacks < 3)
                        return BLM.Fire3;

                if (IsNotEnabled(CustomComboPreset.BlackFireOption2))
                    if (!gauge.InAstralFire)
                        return BLM.Fire3;

                if (HasEffect(BLM.Buffs.Firestarter))
                    return BLM.Fire3;
            }
        }

        return actionID;
    }
}

internal class BlackBlizzard : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlmAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BLM.Blizzard || actionID == BLM.Blizzard3)
        {
            var gauge = GetJobGauge<BLMGauge>();

            if (IsEnabled(CustomComboPreset.BlackSpellsUmbralSoulFeature))
                if (level >= BLM.Levels.UmbralSoul && gauge.InUmbralIce && !HasTarget())
                    return BLM.UmbralSoul;

            if (IsEnabled(CustomComboPreset.BlackBlizzardFeature))
            {
                if (level >= BLM.Levels.Paradox && gauge.IsParadoxActive)
                {
                    //if (
                    //    gauge.InUmbralIce
                    //    || (
                    //        !IsEnabled(CustomComboPreset.BlackBlizzardParadoxOption)
                    //        && LocalPlayer?.CurrentMp >= 1600
                    //    )
                    //)
                    //    return BLM.Paradox;
                }

                if (level >= BLM.Levels.Blizzard3)
                    return BLM.Blizzard3;

                return BLM.Blizzard;
            }
        }

        return actionID;
    }
}

internal class BlackScathe : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } =
        CustomComboPreset.BlackScatheFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == BLM.Scathe)
        {
            var gauge = GetJobGauge<BLMGauge>();

            if (level >= BLM.Levels.Xenoglossy && gauge.PolyglotStacks > 0)
                return BLM.Xenoglossy;
        }

        return actionID;
    }
}
