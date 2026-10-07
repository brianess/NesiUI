// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System;

namespace Nesi.UI;

/// <summary>
/// Configures the overlay host and its application-provided resources.
/// </summary>
public sealed class NesiApplicationOptions
{
    /// <summary>Gets the native overlay window title.</summary>
    public string Title { get; init; } = "Nesi.UI Application";

    /// <summary>Gets the initial overlay width in pixels.</summary>
    public int Width { get; init; } = 1920;

    /// <summary>Gets the initial overlay height in pixels.</summary>
    public int Height { get; init; } = 1080;

    /// <summary>Gets whether vertical synchronization is enabled.</summary>
    public bool VSync { get; init; }

    /// <summary>Gets the requested frame-rate limit.</summary>
    public int FpsLimit { get; init; } = 180;

    /// <summary>Gets whether the overlay should remain above other windows.</summary>
    public bool TopMost { get; init; } = false;

    /// <summary>Gets the font configuration used by the renderer.</summary>
    public FontOptions Font { get; init; } = new();

    /// <summary>Gets the image files to load under application-defined keys.</summary>
    public AssetOptions Assets { get; init; } = new();

    /// <summary>Gets native window-protection and taskbar options.</summary>
    public WindowProtectionOptions Protection { get; init; } = new();

    /// <summary>Gets the input source updated once per rendered frame.</summary>
    public IInputSource Input { get; init; } = new InputManager();
}

/// <summary>Configures the font loaded by the overlay renderer.</summary>
public sealed class FontOptions
{
    /// <summary>Gets the font file path, or <see langword="null"/> to keep the default font.</summary>
    public string? Path { get; init; }

    /// <summary>Gets the font size in pixels.</summary>
    public int Size { get; init; } = 18;

    /// <summary>
    /// Gets ImGui glyph ranges encoded as start/end pairs and terminated by zero.
    /// </summary>
    public ushort[] GlyphRanges { get; init; } = new ushort[] { 0x0020, 0x00FF, 0 };
}

/// <summary>Configures image files to load and expose through <see cref="AssetManager"/>.</summary>
public sealed class AssetOptions
{
    /// <summary>
    /// Gets image asset keys and their file paths. Missing files are not loaded.
    /// </summary>
    public IReadOnlyDictionary<string, string> Images { get; init; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}

/// <summary>Configures native window visibility and capture-protection behavior.</summary>
public sealed class WindowProtectionOptions
{
    /// <summary>Gets whether the overlay is excluded from supported screen captures.</summary>
    public bool StreamProof { get; init; }

    /// <summary>Gets whether the overlay is shown in the taskbar.</summary>
    public bool ShowInTaskbar { get; init; } = true;
}