using System;
using System.Linq;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using XIVComboExpandedPlugin.Interface;

namespace XIVComboExpandedPlugin;

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
    /// <param name="sigScanner">Dalamud signature scanner.</param>
    /// <param name="gameInteropProvider">Dalamud game interop provider.</param>
    public XIVComboExpandedPlugin(
        IDalamudPluginInterface pluginInterface,
        ISigScanner sigScanner,
        IGameInteropProvider gameInteropProvider
    )
    {
        pluginInterface.Create<Service>();

        Service.Configuration =
            pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
        Service.Address = new PluginAddressResolver();
        Service.Address.Setup((SigScanner)sigScanner);

        if (Service.Configuration.Version == 4)
            this.UpgradeConfig4();

        Service.ComboCache = new CustomComboCache();
        Service.IconReplacer = new IconReplacer(gameInteropProvider);

        this.configWindow = new();
        this.windowSystem = new("XIVComboExpanded");
        this.windowSystem.AddWindow(this.configWindow);

        Service.Interface.UiBuilder.OpenConfigUi += this.OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw += this.windowSystem.Draw;

        Service.CommandManager.AddHandler(
            Command,
            new CommandInfo(this.OnCommand)
            {
                HelpMessage = "Open a window to edit custom combo settings.",
                ShowInHelp = true,
            }
        );
    }

    public string Name => "XIV Combo Expanded";

    /// <inheritdoc/>
    public void Dispose()
    {
        Service.CommandManager.RemoveHandler(Command);

        Service.Interface.UiBuilder.OpenConfigUi -= this.OnOpenConfigUi;
        Service.Interface.UiBuilder.Draw -= this.windowSystem.Draw;

        Service.IconReplacer?.Dispose();
        Service.ComboCache?.Dispose();
    }

    private void OnOpenConfigUi() => this.configWindow.IsOpen = true;

    private void OnCommand(string command, string arguments)
    {
        var argumentsParts = arguments.Split();

        switch (argumentsParts[0])
        {
            case "setall":
            {
                foreach (var preset in Enum.GetValues<CustomComboPreset>())
                {
                    Service.Configuration.EnabledActions.Add(preset);
                }

                Service.ChatGui.Print("All SET");
                Service.Configuration.Save();
                break;
            }

            case "unsetall":
            {
                foreach (var preset in Enum.GetValues<CustomComboPreset>())
                {
                    Service.Configuration.EnabledActions.Remove(preset);
                }

                Service.ChatGui.Print("All UNSET");
                Service.Configuration.Save();
                break;
            }

            case "set":
            {
                var targetPreset = argumentsParts[1].ToLowerInvariant();
                foreach (var preset in Enum.GetValues<CustomComboPreset>())
                {
                    if (preset.ToString().ToLowerInvariant() != targetPreset)
                        continue;

                    Service.Configuration.EnabledActions.Add(preset);
                    Service.ChatGui.Print($"{preset} SET");
                }

                Service.Configuration.Save();
                break;
            }

            case "secrets":
            {
                Service.Configuration.EnableSecretCombos = !Service
                    .Configuration
                    .EnableSecretCombos;

                Service.ChatGui.Print(
                    Service.Configuration.EnableSecretCombos
                        ? $"Secret combos are now shown"
                        : $"Secret combos are now hidden"
                );

                Service.Configuration.Save();
                break;
            }

            case "toggle":
            {
                var targetPreset = argumentsParts[1].ToLowerInvariant();
                foreach (var preset in Enum.GetValues<CustomComboPreset>())
                {
                    if (preset.ToString().ToLowerInvariant() != targetPreset)
                        continue;

                    if (Service.Configuration.EnabledActions.Contains(preset))
                    {
                        Service.Configuration.EnabledActions.Remove(preset);
                        Service.ChatGui.Print($"{preset} UNSET");
                    }
                    else
                    {
                        Service.Configuration.EnabledActions.Add(preset);
                        Service.ChatGui.Print($"{preset} SET");
                    }
                }

                Service.Configuration.Save();
                break;
            }

            case "unset":
            {
                var targetPreset = argumentsParts[1].ToLowerInvariant();
                foreach (var preset in Enum.GetValues<CustomComboPreset>())
                {
                    if (preset.ToString().ToLowerInvariant() != targetPreset)
                        continue;

                    Service.Configuration.EnabledActions.Remove(preset);
                    Service.ChatGui.Print($"{preset} UNSET");
                }

                Service.Configuration.Save();
                break;
            }

            case "list":
            {
                var filter =
                    argumentsParts.Length > 1 ? argumentsParts[1].ToLowerInvariant() : "all";

                if (filter == "set")
                {
                    foreach (
                        var preset in Enum.GetValues<CustomComboPreset>()
                            .Select(preset => Service.Configuration.IsEnabled(preset))
                    )
                    {
                        Service.ChatGui.Print(preset.ToString());
                    }
                }
                else if (filter == "unset")
                {
                    foreach (
                        var preset in Enum.GetValues<CustomComboPreset>()
                            .Select(preset => !Service.Configuration.IsEnabled(preset))
                    )
                    {
                        Service.ChatGui.Print(preset.ToString());
                    }
                }
                else if (filter == "all")
                {
                    foreach (var preset in Enum.GetValues<CustomComboPreset>())
                    {
                        Service.ChatGui.Print(preset.ToString());
                    }
                }
                else
                {
                    Service.ChatGui.PrintError("Available list filters: set, unset, all");
                }

                break;
            }

            default:
                this.configWindow.Toggle();
                break;
        }

        Service.Configuration.Save();
    }

    private void UpgradeConfig4()
    {
        Service.Configuration.Version = 5;
        Service.Configuration.EnabledActions = Service
            .Configuration.EnabledActions4.Select(
                preset =>
                    (int)preset switch
                    {
                        27 => 3301,
                        75 => 3302,
                        73 => 3303,
                        25 => 2501,
                        26 => 2502,
                        56 => 2503,
                        70 => 2504,
                        71 => 2505,
                        110 => 2506,
                        95 => 2507,
                        41 => 2301,
                        42 => 2302,
                        63 => 2303,
                        74 => 2304,
                        33 => 3801,
                        31 => 3802,
                        34 => 3803,
                        43 => 3804,
                        50 => 3805,
                        72 => 3806,
                        103 => 3807,
                        44 => 2201,
                        0 => 2202,
                        1 => 2203,
                        2 => 2204,
                        3 => 3201,
                        4 => 3202,
                        57 => 3203,
                        85 => 3204,
                        20 => 3701,
                        52 => 3702,
                        96 => 3703,
                        97 => 3704,
                        22 => 3705,
                        30 => 3706,
                        83 => 3707,
                        84 => 3708,
                        23 => 3101,
                        24 => 3102,
                        47 => 3103,
                        58 => 3104,
                        66 => 3105,
                        102 => 3106,
                        54 => 2001,
                        82 => 2002,
                        106 => 2003,
                        17 => 3001,
                        18 => 3002,
                        19 => 3003,
                        87 => 3004,
                        88 => 3005,
                        89 => 3006,
                        90 => 3007,
                        91 => 3008,
                        92 => 3009,
                        107 => 3010,
                        108 => 3011,
                        5 => 1901,
                        6 => 1902,
                        59 => 1903,
                        7 => 1904,
                        55 => 1905,
                        86 => 1906,
                        69 => 1907,
                        48 => 3501,
                        49 => 3502,
                        68 => 3503,
                        53 => 3504,
                        93 => 3505,
                        101 => 3506,
                        94 => 3507,
                        11 => 3401,
                        12 => 3402,
                        13 => 3403,
                        14 => 3404,
                        15 => 3405,
                        81 => 3406,
                        60 => 3407,
                        61 => 3408,
                        64 => 3409,
                        65 => 3410,
                        109 => 3411,
                        29 => 2801,
                        37 => 2802,
                        39 => 2701,
                        40 => 2702,
                        8 => 2101,
                        9 => 2102,
                        10 => 2103,
                        78 => 2104,
                        79 => 2105,
                        67 => 2106,
                        104 => 2107,
                        35 => 2401,
                        36 => 2402,
                        76 => 2403,
                        77 => 2404,
                        _ => 0,
                    }
            )
            .Where(id => id != 0)
            .Select(id => (CustomComboPreset)id)
            .ToHashSet();
        Service.Configuration.EnabledActions4 = new();
        Service.Configuration.Save();
    }
}
