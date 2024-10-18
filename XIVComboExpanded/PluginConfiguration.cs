using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Configuration;
using Dalamud.Utility;
using Newtonsoft.Json;
using XIVComboExpandedPlugin.Attributes;
using XIVComboExpandedPlugin.Combos;

namespace XIVComboExpandedPlugin;

/// <summary>
///     Plugin configuration.
/// </summary>
[Serializable]
public class PluginConfiguration : IPluginConfiguration
{
    private static readonly HashSet<CustomComboPreset> SecretCombos;
    private static readonly Dictionary<CustomComboPreset, CustomComboPreset[]> ConflictingCombos;
    private static readonly Dictionary<CustomComboPreset, CustomComboPreset?> ParentCombos; // child: parent

    static PluginConfiguration()
    {
        SecretCombos = Enum.GetValues<CustomComboPreset>()
            .Where(preset => preset.GetAttribute<SecretCustomComboAttribute>() != default)
            .ToHashSet();

        ConflictingCombos = Enum.GetValues<CustomComboPreset>()
            .Distinct() // Prevent ArgumentExceptions from adding the same int twice, should not be seen anymore
            .ToDictionary(
                preset => preset,
                preset =>
                    preset.GetAttribute<ConflictingCombosAttribute>()?.ConflictingPresets
                    ?? Array.Empty<CustomComboPreset>()
            );

        ParentCombos = Enum.GetValues<CustomComboPreset>()
            .Distinct() // Prevent ArgumentExceptions from adding the same int twice, should not be seen anymore
            .ToDictionary(
                preset => preset,
                preset => preset.GetAttribute<ParentComboAttribute>()?.ParentPreset
            );
    }

    /// <summary>
    ///     Gets or sets the collection of enabled combos.
    /// </summary>
    [JsonProperty("EnabledActionsV5")]
    public HashSet<CustomComboPreset> EnabledActions { get; set; } = new();

    /// <summary>
    ///     Gets or sets the collection of enabled combos.
    /// </summary>
    [JsonProperty("EnabledActionsV4")]
    public HashSet<CustomComboPreset> EnabledActions4 { get; set; } = new();

    /// <summary>
    ///     Gets or sets a value indicating whether to allow and display secret combos.
    /// </summary>
    [JsonProperty("Debug")]
    public bool EnableSecretCombos { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to hide the children of a feature if it is disabled.
    /// </summary>
    public bool HideChildren { get; set; } = false;

    /// <summary>
    ///     Gets or sets an array of 4 ability IDs to interact with the
    ///     <see cref="CustomComboPreset.DancerDanceComboCompatibility" /> combo.
    /// </summary>
    public uint[] DancerDanceCompatActionIDs { get; set; } =
        { DNC.Cascade, DNC.Flourish, DNC.FanDance1, DNC.FanDance2 };

    /// <summary>
    ///     Gets or sets the configuration version.
    /// </summary>
    public int Version { get; set; } = 5;

    /// <summary>
    ///     Save the configuration to disk.
    /// </summary>
    public void Save()
    {
        Service.Interface.SavePluginConfig(this);
    }

    /// <summary>
    ///     Gets a value indicating whether a preset is enabled.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>The boolean representation.</returns>
    public bool IsEnabled(CustomComboPreset preset)
    {
        return this.EnabledActions.Contains(preset)
            && (this.EnableSecretCombos || !this.IsSecret(preset));
    }

    /// <summary>
    ///     Gets a value indicating whether a preset is secret.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>The boolean representation.</returns>
    public bool IsSecret(CustomComboPreset preset)
    {
        return SecretCombos.Contains(preset);
    }

    /// <summary>
    ///     Gets an array of conflicting combo presets.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>The conflicting presets.</returns>
    public CustomComboPreset[] GetConflicts(CustomComboPreset preset)
    {
        return ConflictingCombos[preset];
    }

    /// <summary>
    ///     Gets the parent combo preset if it exists, or null.
    /// </summary>
    /// <param name="preset">Preset to check.</param>
    /// <returns>The parent preset.</returns>
    public CustomComboPreset? GetParent(CustomComboPreset preset)
    {
        return ParentCombos[preset];
    }
}
