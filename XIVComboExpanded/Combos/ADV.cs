namespace XIVComboExpandedPlugin.Combos;

internal static class ADV
{
    public const byte ClassID = 0;
    public const byte JobID = 0;

    public const uint LucidDreaming = 7562,
        Provoke = 7533,
        Shirk = 7537,
        Peloton = 7557,
        Feint = 7549,
        Addle = 7560,
        Swiftcast = 7561,
        AngelWhisper = 18317,
        VariantRaise2 = 29734;

    public static class Buffs
    {
        public const ushort Medicated = 49,
            Swiftcast = 167;
    }

    public static class Debuffs
    {
        public const ushort Reprisal = 1193,
            Feint = 1195,
            Addle = 1203;
    }

    public static class Levels
    {
        public const byte Swiftcast = 18,
            Addle = 8,
            LucidDreaming = 14,
            Feint = 22,
            VariantRaise2 = 90;
    }
}

internal class SwiftRaiseFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset => CustomComboPreset.AdvSwiftcastFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (
            (actionID == AST.Ascend && level >= AST.Levels.Ascend)
            || (actionID == SCH.Resurrection && level >= SCH.Levels.Resurrection)
            || (actionID == SGE.Egeiro && level >= SGE.Levels.Egeiro)
            || (actionID == WHM.Raise && level >= WHM.Levels.Raise)
            || (
                actionID == RDM.Verraise
                && level >= RDM.Levels.Verraise
                && !HasEffect(RDM.Buffs.Dualcast)
            )
            || (actionID == BLU.AngelWhisper && level >= BLU.Levels.AngelWhisper)
        )
            if (level >= ADV.Levels.Swiftcast && IsAvailable(ADV.Swiftcast))
                return ADV.Swiftcast;

        return actionID;
    }
}

internal class VariantRaiseFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset =>
        CustomComboPreset.AdvVariantRaiseFeature;

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (
            (actionID == AST.Ascend && level >= AST.Levels.Ascend)
            || (actionID == SCH.Resurrection && level >= SCH.Levels.Resurrection)
            || (actionID == SGE.Egeiro && level >= SGE.Levels.Egeiro)
            || (actionID == WHM.Raise && level >= WHM.Levels.Raise)
            || (
                actionID == RDM.Verraise
                && level >= RDM.Levels.Verraise
                && !HasEffect(RDM.Buffs.Dualcast)
            )
            || (actionID == BLU.AngelWhisper && level >= BLU.Levels.AngelWhisper)
        )
            // Per Splatoon:
            // 1069: solo
            // 1075: group
            // 1076: savage
            if (level >= ADV.Levels.VariantRaise2 && CurrentTerritory == 1075u)
                return ADV.VariantRaise2;

        return actionID;
    }
}
