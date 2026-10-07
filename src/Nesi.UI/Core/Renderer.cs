// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using ImGuiNET;
using Nesi.UI.Utils;
using Nesi.UI.Style;

namespace Nesi.UI;

/// <summary>
/// Owns the lifetime of a Nesi.UI overlay application and its public services.
/// </summary>
public sealed class NesiApplication : IDisposable
{
    private readonly Renderer _renderer;
    private readonly NesiApplicationOptions _options;
    /// <summary>Gets the window registry and navigation manager for this application.</summary>
    public WindowManager Windows { get; }

    /// <summary>Gets handles for images configured in the application options.</summary>
    public AssetManager Assets { get; }

    /// <summary>Raised once per frame after the configured input source is updated.</summary>
    public event Action? FrameUpdated;

    /// <summary>Creates an application with the supplied options or their defaults.</summary>
    /// <param name="options">Application and renderer configuration.</param>
    public NesiApplication(NesiApplicationOptions? options = null)
    {
        _options = options ?? new();
        Windows = new WindowManager();
        Assets = new AssetManager();
        _renderer = new Renderer(_options, Windows, Assets, () => FrameUpdated?.Invoke());
    }

    /// <summary>Starts the overlay render loop.</summary>
    public Task RunAsync() => _renderer.Run();

    /// <summary>Releases application windows and renderer resources.</summary>
    public void Dispose()
    {
        Windows.Dispose();
        _renderer.Dispose();
    }

    /// <summary>Enables or disables supported screen-capture exclusion.</summary>
    /// <param name="enabled">Whether capture exclusion should be enabled.</param>
    public void SetStreamProof(bool enabled) => _renderer.SetStreamProof(enabled);

    /// <summary>Changes whether the overlay appears in the taskbar.</summary>
    /// <param name="visible">Whether it should appear in the taskbar.</param>
    public void SetTaskbarVisibility(bool visible) => _renderer.SetTaskbarVisibility(visible);
}

internal sealed class Renderer : ClickableTransparentOverlay.Overlay
{
    private readonly WindowManager _windows;
    private readonly NesiApplicationOptions _options;
    private readonly AssetManager _assets;
    private readonly Action _onFrame;
    private bool _initialized;
    private IntPtr _handle;

    internal Renderer(NesiApplicationOptions options, WindowManager windows, AssetManager assets, Action onFrame)
        : base(ValidateTitle(options.Title), true, options.Width, options.Height)
    {
        _options = options;
        _windows = windows;
        _assets = assets;
        _onFrame = onFrame;
        VSync = options.VSync;
        FPSLimit = options.FpsLimit;
    }

    private static string ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("The application title cannot be empty.", nameof(title));

        return title;
    }

    protected override async Task PostInitialized()
    {
        await base.PostInitialized();
        _handle = window.Handle;
        WindowProtection.SetTopMost(_handle, _options.TopMost);
        SetStreamProof(_options.Protection.StreamProof);
        SetTaskbarVisibility(_options.Protection.ShowInTaskbar);
    }

    internal void SetStreamProof(bool enabled) => WindowProtection.SetStreamProof(_handle, enabled);

    internal void SetTaskbarVisibility(bool visible)
    {
        if (visible) WindowProtection.ShowInTaskbar(_handle);
        else WindowProtection.HideFromTaskbar(_handle);
    }

    protected override void Render()
    {
        if (!_initialized)
        {
            var io = ImGui.GetIO();
            io.ConfigFlags &= ~ImGuiConfigFlags.DockingEnable;
            io.ConfigFlags &= ~ImGuiConfigFlags.NavEnableKeyboard;
            ThemeManager.ApplyCurrentTheme();
            if (_options.Font.Path is { } path && File.Exists(path))
            {
                var glyphRanges = NormalizeGlyphRanges(_options.Font.GlyphRanges);
                ReplaceFont(path, _options.Font.Size, glyphRanges);
            }
            foreach (var image in _options.Assets.Images)
            {
                if (File.Exists(image.Value))
                {
                    AddOrGetImagePointer(image.Value, false, out var handle, out _, out _);
                    _assets.SetHandle(image.Key, handle);
                }
            }
            _initialized = true;
        }
        _options.Input.Update();
        _onFrame();
        _windows.Render();
    }

    private static ushort[] NormalizeGlyphRanges(IReadOnlyList<ushort> ranges)
    {
        if (ranges.Count == 0)
            return new ushort[] { 0x0020, 0x00FF, 0 };

        if (ranges[^1] == 0)
            return ranges.ToArray();

        var normalized = new ushort[ranges.Count + 1];
        for (var index = 0; index < ranges.Count; index++)
            normalized[index] = ranges[index];

        normalized[^1] = 0;
        return normalized;
    }
}
