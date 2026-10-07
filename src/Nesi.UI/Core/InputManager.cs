// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using Nesi.UI.Utils;

namespace Nesi.UI;

/// <summary>Receives per-frame input updates for a Nesi.UI application.</summary>
public interface IInputSource
{
    /// <summary>Updates the input state.</summary>
    void Update();
}

/// <summary>
/// Polls the built-in Windows virtual keys and exposes pressed/held state.
/// </summary>
public sealed class InputManager : IInputSource
{
    private readonly Dictionary<int, bool> _previousState = new();
    private readonly Dictionary<int, bool> _currentState = new();

    /// <summary>Gets the key names recognized by this input manager.</summary>
    public static IReadOnlyDictionary<string, int> DefaultKeys { get; } =
        new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["Insert"] = 0x2D,
            ["Delete"] = 0x2E,
            ["End"] = 0x23,
            ["Home"] = 0x24,
            ["F1"] = 0x70,
            ["F5"] = 0x74,
            ["CapsLock"] = 0x14,
            ["Mouse4"] = 0x05,
            ["Mouse5"] = 0x06
        };

    /// <inheritdoc />
    public void Update()
    {
        foreach (var virtualKey in DefaultKeys.Values.Distinct())
        {
            _previousState[virtualKey] = _currentState.TryGetValue(virtualKey, out var wasDown) && wasDown;
            _currentState[virtualKey] = Helper.IsKeyDown(virtualKey);
        }
    }

    /// <summary>
    /// Returns whether the named key transitioned from up to down in the latest
    /// update.
    /// </summary>
    /// <param name="key">A key name from <see cref="DefaultKeys"/>.</param>
    /// <returns><see langword="true"/> when the key was newly pressed.</returns>
    public bool WasPressed(string key)
    {
        if (!DefaultKeys.TryGetValue(key, out var virtualKey))
            return false;

        return IsPressed(virtualKey);
    }

    /// <summary>Returns whether the named key is currently held down.</summary>
    /// <param name="key">A key name from <see cref="DefaultKeys"/>.</param>
    /// <returns><see langword="true"/> while the key is held.</returns>
    public bool IsHeld(string key)
    {
        if (!DefaultKeys.TryGetValue(key, out var virtualKey))
            return false;

        return _currentState.TryGetValue(virtualKey, out var isDown) && isDown;
    }

    private bool IsPressed(int virtualKey)
    {
        var isDown = _currentState.TryGetValue(virtualKey, out var current) && current;
        var wasDown = _previousState.TryGetValue(virtualKey, out var previous) && previous;
        return isDown && !wasDown;
    }
}
