// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using ClickableTransparentOverlay;
using Nesi.UI.Utils;

namespace Nesi.UI;

/// <summary>
/// Loads a font into a ClickableTransparentOverlay overlay.
/// Most application consumers should configure fonts through
/// <see cref="NesiApplicationOptions.Font"/>.
/// </summary>
public class FontManager : IDisposable
{
    private readonly Overlay _overlay;
    private bool _disposed;
    /// <summary>Gets whether the most recent font load succeeded.</summary>
    public bool IsLoaded { get; private set; }

    /// <summary>
    /// Creates a font manager for an overlay host.
    /// </summary>
    /// <param name="overlay">The ClickableTransparentOverlay host to update.</param>
    public FontManager(Overlay overlay)
    {
        _overlay = overlay;
    }

    /// <summary>Loads a font and replaces the overlay's current font.</summary>
    /// <param name="fontPath">Path to a font file.</param>
    /// <param name="fontSize">Font size in pixels.</param>
    /// <param name="ranges">Unicode ranges included in the font atlas.</param>
    public void LoadFont(string fontPath, int fontSize, FontRange[] ranges)
    {
        try
        {
            if (!File.Exists(fontPath))
            {
                Logger.Warning($"Font not found at: {fontPath}, using default");
                return;
            }

            var rangesList = new List<ushort>();
            foreach (var range in ranges)
            {
                rangesList.Add(range.Start);
                rangesList.Add(range.End);
            }
            rangesList.Add(0);

            var rangesArray = rangesList.ToArray();

            _overlay.ReplaceFont(fontPath, fontSize, rangesArray);
            IsLoaded = true;

            Logger.Info($"Font loaded: {fontPath} (size: {fontSize})");
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to load font: {ex.Message}");
        }
    }

    /// <summary>Releases managed resources owned by this manager.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
    }
}

/// <summary>Inclusive start and end code points for a font glyph range.</summary>
/// <param name="Start">The first code point in the range.</param>
/// <param name="End">The last code point in the range.</param>
public record FontRange(ushort Start, ushort End);
