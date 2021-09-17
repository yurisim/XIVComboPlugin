using System;
using System.Linq;

using Dalamud.Data;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;

namespace XIVComboExpandedPlugin
{
    /// <summary>
    /// Main plugin implementation.
    /// </summary>
    public sealed partial class XIVComboExpandedPlugin : IDalamudPlugin
    {
        private const string Command = "/pcombo";

        private readonly WindowSystem windowSystem;
        private readonly ConfigWindow configWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="XIVComboExpandedPlugin"/> class.
        /// </summary>
        /// <param name="pluginInterface">Dalamud plugin interface.</param>
        /// <param name="chatGui">Dalamud ChatGui object.</param>
        /// <param name="clientState">Dalamud ClientState object.</param>
        /// <param name="commandManager">Dalamud CommandManager object.</param>
        /// <param name="condition">Dalamud Condition object.</param>
        /// <param name="dataManager">Dalamud DataManager object.</param>
        /// <param name="jobGauges">Dalamud JobGauges object.</param>
        /// <param name="targetManager">Dalamud TargetManager object.</param>
        public XIVComboExpandedPlugin(
            DalamudPluginInterface pluginInterface,
            ChatGui chatGui,
            ClientState clientState,
            CommandManager commandManager,
            Condition condition,
            DataManager dataManager,
            JobGauges jobGauges,
            TargetManager targetManager)
        {
            this.Interface = pluginInterface;
            this.ChatGui = chatGui;
            this.ClientState = clientState;
            this.CommandManager = commandManager;
            this.Condition = condition;
            this.DataManager = dataManager;
            this.JobGauges = jobGauges;
            this.TargetManager = targetManager;

            this.Configuration = pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();

            this.Address = new PluginAddressResolver();
            this.Address.Setup();

            this.IconReplacer = new IconReplacer(this);

            this.configWindow = new(this);
            this.windowSystem = new("XIVComboExpanded");
            this.windowSystem.AddWindow(this.configWindow);

            this.Interface.UiBuilder.OpenConfigUi += this.UiBuilder_OpenConfigUi;
            this.Interface.UiBuilder.Draw += this.windowSystem.Draw;

            this.CommandManager.AddHandler(Command, new CommandInfo(this.OnCommand)
            {
                HelpMessage = "Open a window to edit custom combo settings.",
                ShowInHelp = true,
            });
        }

        /// <inheritdoc/>
        public string Name => "XIV Combo Expanded";

        /// <summary>
        /// Gets the PluginAddressResolver object.
        /// </summary>
        internal PluginAddressResolver Address { get; init; } = null!;

        /// <summary>
        /// Gets the IconReplacer object.
        /// </summary>
        internal IconReplacer IconReplacer { get; init; } = null!;

        /// <summary>
        /// Gets the plugin configuration.
        /// </summary>
        internal PluginConfiguration Configuration { get; init; } = null!;

        /// <summary>
        /// Gets the Dalamud plugin interface.
        /// </summary>
        internal DalamudPluginInterface Interface { get; init; } = null!;

        /// <summary>
        /// Gets the Dalamud ChatGui.
        /// </summary>
        internal ChatGui ChatGui { get; init; } = null!;

        /// <summary>
        /// Gets the Dalamud ClientState object.
        /// </summary>
        internal ClientState ClientState { get; init; } = null!;

        /// <summary>
        /// Gets the Dalamud CommandManager object.
        /// </summary>
        internal CommandManager CommandManager { get; init; } = null!;

        /// <summary>
        /// Gets the Dalamud Condition object.
        /// </summary>
        internal Condition Condition { get; init; } = null!;

        /// <summary>
        /// Gets the Dalamud DataManager object.
        /// </summary>
        internal DataManager DataManager { get; init; } = null!;

        /// <summary>
        /// Gets the Dalamud JobGauges object.
        /// </summary>
        internal JobGauges JobGauges { get; init; } = null!;

        /// <summary>
        /// Gets the Dalamud TargetManager object.
        /// </summary>
        internal TargetManager TargetManager { get; init; } = null!;

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Interface.UiBuilder.Draw -= this.windowSystem.Draw;
            this.Interface.UiBuilder.OpenConfigUi -= this.UiBuilder_OpenConfigUi;

            this.CommandManager.RemoveHandler(Command);
            this.IconReplacer.Dispose();
        }

        /// <summary>
        /// Save the configuration to disk.
        /// </summary>
        internal void SaveConfiguration()
            => this.Interface.SavePluginConfig(this.Configuration);

        private void UiBuilder_OpenConfigUi()
            => this.configWindow.IsOpen = true;

        private void OnCommand(string command, string arguments)
        {
            var argumentsParts = arguments.Split();

            switch (argumentsParts[0])
            {
                case "setall":
                    {
                        foreach (var preset in Enum.GetValues<CustomComboPreset>())
                        {
                            this.Configuration.EnabledActions.Add(preset);
                        }

                        this.ChatGui.Print("All SET");
                        this.SaveConfiguration();
                        break;
                    }

                case "unsetall":
                    {
                        foreach (var preset in Enum.GetValues<CustomComboPreset>())
                        {
                            this.Configuration.EnabledActions.Remove(preset);
                        }

                        this.ChatGui.Print("All UNSET");
                        this.SaveConfiguration();
                        break;
                    }

                case "set":
                    {
                        var targetPreset = argumentsParts[1].ToLowerInvariant();
                        foreach (var preset in Enum.GetValues<CustomComboPreset>())
                        {
                            if (preset.ToString().ToLowerInvariant() != targetPreset)
                                continue;

                            this.Configuration.EnabledActions.Add(preset);
                            this.ChatGui.Print($"{preset} SET");
                        }

                        this.SaveConfiguration();
                        break;
                    }

                case "secrets":
                    {
                        this.Configuration.EnableSecretCombos = !this.Configuration.EnableSecretCombos;

                        this.ChatGui.Print(this.Configuration.EnableSecretCombos
                            ? $"Secret combos are now shown"
                            : $"Secret combos are now hidden");

                        this.SaveConfiguration();
                        break;
                    }

                case "toggle":
                    {
                        var targetPreset = argumentsParts[1].ToLowerInvariant();
                        foreach (var preset in Enum.GetValues<CustomComboPreset>())
                        {
                            if (preset.ToString().ToLowerInvariant() != targetPreset)
                                continue;

                            if (this.Configuration.EnabledActions.Contains(preset))
                            {
                                this.Configuration.EnabledActions.Remove(preset);
                                this.ChatGui.Print($"{preset} UNSET");
                            }
                            else
                            {
                                this.Configuration.EnabledActions.Add(preset);
                                this.ChatGui.Print($"{preset} SET");
                            }
                        }

                        this.SaveConfiguration();
                        break;
                    }

                case "unset":
                    {
                        var targetPreset = argumentsParts[1].ToLowerInvariant();
                        foreach (var preset in Enum.GetValues<CustomComboPreset>())
                        {
                            if (preset.ToString().ToLowerInvariant() != targetPreset)
                                continue;

                            this.Configuration.EnabledActions.Remove(preset);
                            this.ChatGui.Print($"{preset} UNSET");
                        }

                        this.SaveConfiguration();
                        break;
                    }

                case "list":
                    {
                        var filter = argumentsParts.Length > 1
                            ? argumentsParts[1].ToLowerInvariant()
                            : "all";

                        if (filter == "set")
                        {
                            foreach (var preset in Enum.GetValues<CustomComboPreset>()
                                .Select(preset => this.Configuration.IsEnabled(preset)))
                            {
                                this.ChatGui.Print(preset.ToString());
                            }
                        }
                        else if (filter == "unset")
                        {
                            foreach (var preset in Enum.GetValues<CustomComboPreset>()
                                .Select(preset => !this.Configuration.IsEnabled(preset)))
                            {
                                this.ChatGui.Print(preset.ToString());
                            }
                        }
                        else if (filter == "all")
                        {
                            foreach (var preset in Enum.GetValues<CustomComboPreset>())
                            {
                                this.ChatGui.Print(preset.ToString());
                            }
                        }
                        else
                        {
                            this.ChatGui.PrintError("Available list filters: set, unset, all");
                        }

                        break;
                    }

                default:
                    this.configWindow.Toggle();
                    break;
            }

            this.Interface.SavePluginConfig(this.Configuration);
        }
    }
}