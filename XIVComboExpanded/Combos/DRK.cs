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
        TheBlackestNight = 7393,
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
        public const ushort BloodWeapon = 742,
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
        public const byte SyphonStrike = 2,
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
            TheBlackestNight = 70,
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
        if (actionID is DRK.HardSlash or DRK.Unleash)
        {
            var gauge = GetJobGauge<DRKGauge>();
            var raidBuffs = HasRaidBuffs(1);

            if (GCDClipCheck(actionID))
            {
                switch (level)
                {
                    case >= DRK.Levels.TheBlackestNight when 
                        actionID is DRK.Unleash
                        && IsOffCooldown(DRK.TheBlackestNight)
                        && LocalPlayer?.CurrentMp >= 5000
                        && LocalPlayerPercentage() <= 0.85:
                        return DRK.TheBlackestNight;
                    case >= DRK.Levels.FloodOfDarkness
                        when (
                            LocalPlayer?.CurrentMp >= 9000
                            || gauge.HasDarkArts
                            || (LocalPlayer?.CurrentMp >= 6000 && raidBuffs)
                        ):
                        return level >= DRK.Levels.EdgeOfDarkness && actionID is not DRK.Unleash
                            ? OriginalHook(DRK.EdgeOfDarkness)
                            : OriginalHook(DRK.FloodOfDarkness);
                    case >= DRK.Levels.BloodWeapon when IsOffCooldown(DRK.BloodWeapon):
                        return OriginalHook(DRK.BloodWeapon);
                    case >= DRK.Levels.AbyssalDrain when IsOffCooldown(DRK.AbyssalDrain):
                        return level >= DRK.Levels.CarveAndSpit && actionID is DRK.HardSlash
                            ? DRK.CarveAndSpit
                            : DRK.AbyssalDrain;
                }
            }
            if (
                level >= DRK.Levels.Bloodspiller
                && (gauge.Blood >= 50 || HasEffect(DRK.Buffs.Delirium))
                && (HasEffect(DRK.Buffs.Delirium) || gauge.Blood >= 70 || raidBuffs)
                // && (lastComboMove is DRK.SyphonStrike or DRK.Unleash || HasEffect(DRK.Buffs.BloodWeapon))
            )
            {
                return level >= DRK.Levels.Quietus && actionID is DRK.Unleash
                    ? DRK.Quietus
                    : DRK.Bloodspiller;
            }

            if (comboTime > 0)
            {
                if (actionID is DRK.HardSlash)
                {
                    if (lastComboMove == DRK.SyphonStrike && level >= DRK.Levels.Souleater)
                        return DRK.Souleater;

                    if (lastComboMove == DRK.HardSlash && level >= DRK.Levels.SyphonStrike)
                        return DRK.SyphonStrike;
                }

                if (actionID is DRK.Unleash)
                {
                    if (lastComboMove == DRK.Unleash && level >= DRK.Levels.StalwartSoul)
                        return DRK.StalwartSoul;
                }
            }

            return actionID is DRK.HardSlash ? DRK.HardSlash : DRK.Unleash;
        }

        return actionID;
    }
}
