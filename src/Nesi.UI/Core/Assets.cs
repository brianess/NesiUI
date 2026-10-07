// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

namespace Nesi.UI;

/// <summary>
/// Provides access to image texture handles loaded from <see cref="AssetOptions"/>.
/// </summary>
public sealed class AssetManager
{
    private readonly Dictionary<string, IntPtr> _handles =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Tries to retrieve an image handle by its configured key.</summary>
    /// <param name="key">The image key from <see cref="AssetOptions.Images"/>.</param>
    /// <param name="handle">Receives the handle when the image is loaded.</param>
    /// <returns><see langword="true"/> when a loaded image was found.</returns>
    public bool TryGetImage(string key, out IntPtr handle) =>
        _handles.TryGetValue(key, out handle);

    /// <summary>Gets a loaded image handle by key.</summary>
    /// <param name="key">The image key from <see cref="AssetOptions.Images"/>.</param>
    /// <returns>The ImGui texture handle.</returns>
    /// <exception cref="KeyNotFoundException">The key is not loaded.</exception>
    public IntPtr GetImage(string key) =>
        TryGetImage(key, out var handle)
            ? handle
            : throw new KeyNotFoundException($"Image asset '{key}' is not loaded.");

    internal void SetHandle(string key, IntPtr handle) => _handles[key] = handle;
}
