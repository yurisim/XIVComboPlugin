namespace XIVComboExpandedPlugin.Combos;

internal static class ADV
{
    public const byte ClassID = 0;
    public const byte JobID = 0;

    public const uint
        LucidDreaming = 7562,
        AngelWhisper = 18317,
        Swiftcast = 7561;

    public static class Buffs
    {
        public const ushort
            Medicated = 49,
            Swiftcast = 167;
    }

    public static class Debuffs
    {
        public const ushort
            Placeholder = 0;
    }

    public static class Levels
    {
        public const byte
            Swiftcast = 18;
    }
}

internal class SwiftRaiseFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset => CustomComboPreset.AllSwiftcastFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if ((actionID == AST.Ascend && level >= AST.Levels.Ascend) ||
            (actionID == SCH.Ressurection && level >= SCH.Levels.Ressurection) ||
            (actionID == SGE.Egeiro && level >= SGE.Levels.Egeiro) ||
            (actionID == WHM.Raise && level >= WHM.Levels.Raise) ||
            (actionID == RDM.Verraise && level >= RDM.Levels.Verraise && !HasEffect(RDM.Buffs.Dualcast)) ||
            (actionID == BLU.AngelWhisper && level >= BLU.Levels.AngelWhisper))
        {
            if (level >= ADV.Levels.Swiftcast && IsOffCooldown(ADV.Swiftcast))
                return ADV.Swiftcast;
        }

        return actionID;
    }
}
