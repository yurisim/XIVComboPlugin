using System;
using Dalamud.Game;
using Dalamud.Logging;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace XIVComboExpandedPlugin;

/// <summary>
/// Plugin address resolver.
/// </summary>
internal class PluginAddressResolver : BaseAddressResolver
{
    /// <summary>
    /// Gets the address of the member ComboTimer.
    /// </summary>
    public IntPtr ComboTimer { get; private set; }

    /// <summary>
    /// Gets the address of the member LastComboMove.
    /// </summary>
    public IntPtr LastComboMove => this.ComboTimer + 0x4;

    /// <summary>
    /// Gets the address of fpIsIconReplacable.
    /// </summary>
    public IntPtr IsActionIdReplaceable { get; private set; }

    /// <inheritdoc/>
    protected unsafe override void Setup64Bit(ISigScanner scanner)
    {
        this.ComboTimer = new IntPtr(&ActionManager.Instance()->Combo.Timer);

        this.IsActionIdReplaceable = scanner.ScanText("E8 ?? ?? ?? ?? 84 C0 0F 84 ?? ?? ?? ?? C6 83 ?? ?? ?? ?? ?? 48 8B 5C 24");

        Service.PluginLog.Verbose("===== X I V C O M B O =====");
        Service.PluginLog.Verbose($"{nameof(this.IsActionIdReplaceable)} 0x{this.IsActionIdReplaceable:X}");
        Service.PluginLog.Verbose($"{nameof(this.ComboTimer)}            0x{this.ComboTimer:X}");
        Service.PluginLog.Verbose($"{nameof(this.LastComboMove)}         0x{this.LastComboMove:X}");
    }
}
