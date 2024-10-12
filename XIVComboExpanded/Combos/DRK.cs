using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos;

internal static class DRK
{
    public const byte JobID = 32;

    public const uint HardSlash = 3617,
        Unleash = 3621,
        SyphonStrike = 3623,
        Grit = 3629,
        Unmend = 3624,
        Souleater = 3632,
        BloodWeapon = 3625,
        SaltedEarth = 3639,
        AbyssalDrain = 3641,
        CarveAndSpit = 3643,
        Delirium = 7390,
        Quietus = 7391,
        Bloodspiller = 7392,
        FloodOfDarkness = 16466,
        EdgeOfDarkness = 16467,
        StalwartSoul = 16468,
        FloodOfShadow = 16469,
        EdgeOfShadow = 16470,
        LivingShadow = 16472,
        SaltAndDarkness = 25755,
        Shadowbringer = 25757,
        GritRemoval = 32067,
        ScarletDelirium = 36928,
        Comeuppance = 36929,
        Torcleaver = 36930,
        Impalement = 36931;

    public static class Buffs
    {
        public const ushort
            BloodWeapon = 742,
            Grit = 743,
            Darkside = 751,
            Delirium = 1972,
            WalkingDead = 811,
            ScarletDelirium = 3836;
    }

    public static class Debuffs
    {
        public const ushort Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            SyphonStrike = 2,
            Grit = 10,
            Souleater = 26,
            FloodOfDarkness = 30,
            BloodWeapon = 35,
            EdgeOfDarkness = 40,
            StalwartSoul = 40,
            SaltedEarth = 52,
            AbyssalDrain = 56,
            CarveAndSpit = 60,
            Bloodspiller = 62,
            Quietus = 64,
            Delirium = 68,
            Shadow = 74,
            LivingShadow = 80,
            SaltAndDarkness = 86,
            Shadowbringer = 90,
            ScarletDelirium = 96,
            Comeuppance = 96,
            Torcleaver = 96,
            Impalement = 96;
    }
}

internal class DarkSouleater : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DRK.HardSlash)
        {
            var gauge = GetJobGauge<DRKGauge>();

            if (GCDClipCheck(actionID))
            {
                // Do a check for flood of darkness first since it is a lower level
                if (
                        (
                            level >= DRK.Levels.FloodOfDarkness
                            && (LocalPlayer?.CurrentMp >= 9000 || gauge.HasDarkArts)
                        ) || (LocalPlayer?.CurrentMp >= 6000 && HasRaidBuffs(2))
                    )
                    // If you are high enough level for edge fo darkness then do that instead
                    return level >= DRK.Levels.EdgeOfDarkness
                        ? OriginalHook(DRK.EdgeOfDarkness)
                        : OriginalHook(DRK.FloodOfDarkness);

                if (
                    level >= DRK.Levels.BloodWeapon
                    && IsOffCooldown(DRK.BloodWeapon)
                    && gauge.Blood <= 70
                )
                    return DRK.BloodWeapon;

                if (
                    level >= DRK.Levels.Delirium
                    && IsOffCooldown(DRK.Delirium)
                    && gauge.Blood <= 70
                )
                    return DRK.Delirium;
            }

            if (level >= DRK.Levels.AbyssalDrain && IsOffCooldown(DRK.AbyssalDrain))
                return level >= DRK.Levels.CarveAndSpit ? DRK.CarveAndSpit : DRK.AbyssalDrain;

            if (
                level >= DRK.Levels.Bloodspiller
                && gauge.Blood >= 50
                && (HasEffect(DRK.Buffs.Delirium) || gauge.Blood >= 70 || HasRaidBuffs(2))
            )
                return DRK.Bloodspiller;

            if (comboTime > 0)
            {
                if (lastComboMove == DRK.SyphonStrike && level >= DRK.Levels.Souleater) return DRK.Souleater;

                if (lastComboMove == DRK.HardSlash && level >= DRK.Levels.SyphonStrike)
                    return DRK.SyphonStrike;
            }

            return DRK.HardSlash;
        }

        return actionID;
    }
}

internal class DarkStalwartSoul : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DRK.StalwartSoul)
        {
            var gauge = GetJobGauge<DRKGauge>();

            // Do a check for flood of darkness girst since it is a lower level
            if (
                    level >= DRK.Levels.FloodOfDarkness
                    && (LocalPlayer?.CurrentMp >= 9000 || gauge.HasDarkArts)
                    && GCDClipCheck(actionID)
                )
                // If you are high enough level for edge fo darkness then do that instead
                return OriginalHook(DRK.FloodOfDarkness);

            if (
                level >= DRK.Levels.BloodWeapon
                && GCDClipCheck(actionID)
                && IsOffCooldown(DRK.BloodWeapon)
                && gauge.Blood <= 70
            )
                return DRK.BloodWeapon;

            if (
                level >= DRK.Levels.Delirium
                && GCDClipCheck(actionID)
                && IsOffCooldown(DRK.Delirium)
                && gauge.Blood <= 70
            )
                return DRK.Delirium;

            if (level >= DRK.Levels.AbyssalDrain && IsOffCooldown(DRK.AbyssalDrain)) return DRK.AbyssalDrain;

            if (
                level >= DRK.Levels.Quietus
                && (
                    (level >= DRK.Levels.Delirium && HasEffect(DRK.Buffs.Delirium))
                    || gauge.Blood >= 70
                )
            )
                return DRK.Quietus;

            if (comboTime > 0)
                if (lastComboMove == DRK.Unleash && level >= DRK.Levels.StalwartSoul)
                    return DRK.StalwartSoul;

            return DRK.Unleash;
        }

        return actionID;
    }
}

internal class DarkCarveAndSpitAbyssalDrain : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DRK.CarveAndSpit || actionID == DRK.AbyssalDrain)
            if (IsEnabled(CustomComboPreset.DarkBloodWeaponFeature))
            {
                if (actionID == DRK.AbyssalDrain && level < DRK.Levels.AbyssalDrain)
                    return OriginalHook(DRK.BloodWeapon);

                if (actionID == DRK.CarveAndSpit && level < DRK.Levels.CarveAndSpit)
                    return OriginalHook(DRK.BloodWeapon);

                if (level >= DRK.Levels.BloodWeapon && IsAvailable(DRK.BloodWeapon))
                    return OriginalHook(DRK.BloodWeapon);
            }

        return actionID;
    }
}

internal class DarkQuietusBloodspiller : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DRK.Quietus || actionID == DRK.Bloodspiller)
        {
            var gauge = GetJobGauge<DRKGauge>();

            if (IsEnabled(CustomComboPreset.DarkLivingShadowFeature))
                if (
                    level >= DRK.Levels.LivingShadow
                    && gauge.Blood >= 50
                    && IsOffCooldown(DRK.LivingShadow)
                )
                    return DRK.LivingShadow;
        }

        return actionID;
    }
}

internal class DarkLivingShadow : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DrkAny;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == DRK.LivingShadow)
        {
            var gauge = GetJobGauge<DRKGauge>();

            if (IsEnabled(CustomComboPreset.DarkLivingShadowbringerFeature))
                if (
                    level >= DRK.Levels.Shadowbringer
                    && gauge.ShadowTimeRemaining > 0
                    && HasCharges(DRK.Shadowbringer)
                )
                    return DRK.Shadowbringer;

            if (IsEnabled(CustomComboPreset.DarkLivingShadowbringerHpFeature))
                if (
                    level >= DRK.Levels.Shadowbringer
                    && HasCharges(DRK.Shadowbringer)
                    && IsOnCooldown(DRK.LivingShadow)
                )
                    return DRK.Shadowbringer;
        }

        return actionID;
    }
}