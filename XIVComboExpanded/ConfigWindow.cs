using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;

namespace XIVComboExpandedPlugin
{
    /// <summary>
    /// Plugin configuration window.
    /// </summary>
    internal class ConfigWindow : Window
    {
        private readonly XIVComboExpandedPlugin plugin;
        private readonly Dictionary<string, List<(CustomComboPreset Preset, CustomComboInfoAttribute Info)>> groupedPresets;
        private readonly Vector4 shadedColor = new(0.68f, 0.68f, 0.68f, 1.0f);

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigWindow"/> class.
        /// </summary>
        /// <param name="plugin">The plugin itself.</param>
        public ConfigWindow(XIVComboExpandedPlugin plugin)
            : base("Custom Combo Setup")
        {
            this.plugin = plugin;

            this.RespectCloseHotkey = true;

            this.groupedPresets = Enum
                .GetValues<CustomComboPreset>()
                .Select(preset => (preset, Info: preset.GetAttribute<CustomComboInfoAttribute>()))
                .Where(presetWithInfo => presetWithInfo.Info != null)
                .OrderBy(presetWithInfo => presetWithInfo.Info.JobName)
                .GroupBy(presetWithInfo => presetWithInfo.Info.JobName)
                .ToDictionary(
                    presetWithInfos => presetWithInfos.Key,
                    presetWithInfos => presetWithInfos.ToList());

            this.SizeCondition = ImGuiCond.FirstUseEver;
            this.Size = new Vector2(740, 490);
        }

        /// <inheritdoc/>
        public override void Draw()
        {
            ImGui.Text("This window allows you to enable and disable custom combos to your liking.");

            var showSecrets = this.plugin.Configuration.EnableSecretCombos;
            if (ImGui.Checkbox("Enable secret forbidden knowledge", ref showSecrets))
            {
                this.plugin.Configuration.EnableSecretCombos = showSecrets;
                this.plugin.SaveConfiguration();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Combos too dangerous for the common folk");
                ImGui.EndTooltip();
            }

            ImGui.BeginChild("scrolling", new Vector2(0, -1), true);

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 5));

            var i = 1;

            foreach (var jobName in this.groupedPresets.Keys)
            {
                if (ImGui.CollapsingHeader(jobName))
                {
                    foreach (var (preset, info) in this.groupedPresets[jobName])
                    {
                        var enabled = this.plugin.Configuration.IsEnabled(preset);
                        var secret = this.plugin.Configuration.IsSecret(preset);
                        var conflicts = this.plugin.Configuration.GetConflicts(preset);

                        if (secret && !showSecrets)
                            continue;

                        ImGui.PushItemWidth(200);

                        if (ImGui.Checkbox(info.FancyName, ref enabled))
                        {
                            if (enabled)
                            {
                                this.plugin.Configuration.EnabledActions.Add(preset);
                                foreach (var conflict in conflicts)
                                {
                                    this.plugin.Configuration.EnabledActions.Remove(conflict);
                                }
                            }
                            else
                            {
                                this.plugin.Configuration.EnabledActions.Remove(preset);
                            }

                            this.plugin.IconReplacer.UpdateEnabledActionIDs();
                            this.plugin.SaveConfiguration();
                        }

                        if (secret)
                        {
                            ImGui.SameLine();
                            ImGui.Text("  ");
                            ImGui.SameLine();
                            ImGui.PushFont(UiBuilder.IconFont);
                            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                            ImGui.Text(FontAwesomeIcon.Star.ToIconString());
                            ImGui.PopStyleColor();
                            ImGui.PopFont();

                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted("Secret");
                                ImGui.EndTooltip();
                            }
                        }

                        ImGui.PopItemWidth();

                        ImGui.TextColored(this.shadedColor, $"#{i}: {info.Description}");
                        ImGui.Spacing();

                        if (conflicts.Length > 0)
                        {
                            var conflictText = conflicts.Select(preset =>
                            {
                                var info = preset.GetAttribute<CustomComboInfoAttribute>();
                                return $"\n - {info.FancyName}";
                            }).Aggregate((t1, t2) => $"{t1}{t2}");

                            ImGui.TextColored(this.shadedColor, $"Conflicts with: {conflictText}");
                            ImGui.Spacing();
                        }

                        if (preset == CustomComboPreset.DancerDanceComboCompatibility && enabled)
                        {
                            var actions = this.plugin.Configuration.DancerDanceCompatActionIDs.Cast<int>().ToArray();

                            var inputChanged = false;
                            inputChanged |= ImGui.InputInt("Emboite (Red) ActionID", ref actions[0], 0);
                            inputChanged |= ImGui.InputInt("Entrechat (Blue) ActionID", ref actions[1], 0);
                            inputChanged |= ImGui.InputInt("Jete (Green) ActionID", ref actions[2], 0);
                            inputChanged |= ImGui.InputInt("Pirouette (Yellow) ActionID", ref actions[3], 0);

                            if (inputChanged)
                            {
                                this.plugin.Configuration.DancerDanceCompatActionIDs = actions.Cast<uint>().ToArray();
                                this.plugin.IconReplacer.UpdateEnabledActionIDs();
                                this.plugin.SaveConfiguration();
                            }

                            ImGui.Spacing();
                        }

                        i++;
                    }
                }
                else
                {
                    i += this.groupedPresets[jobName].Count;
                }
            }

            ImGui.PopStyleVar();

            ImGui.EndChild();
        }
    }
}
