// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Numerics;
using ImGuiNET;

namespace Nesi.UI.Utils;

/// <summary>
/// Provides frame-delta-based interpolation helpers with static state keyed by
/// caller-provided IDs.
/// </summary>
public static class Animation
{
    private static readonly Dictionary<string, float> _floatAnimations = new();
    private static readonly Dictionary<string, Vector4> _colorAnimations = new();
    private static readonly Dictionary<string, Vector2> _vectorAnimations = new();

    /// <summary>Animates a float value toward a target using the current ImGui frame delta.</summary>
    /// <param name="id">Unique key for this animation's stored state.</param>
    /// <param name="target">Target value.</param>
    /// <param name="speed">Interpolation speed.</param>
    /// <param name="threshold">Distance from the target at which the value snaps to it.</param>
    /// <returns>The current interpolated value.</returns>
    public static float AnimateFloat(string id, float target, float speed = 8f, float threshold = 0.001f)
    {
        if (!_floatAnimations.TryGetValue(id, out float current))
            current = target;

        current = Lerp(current, target, ImGui.GetIO().DeltaTime * speed);

        if (Math.Abs(current - target) < threshold)
            current = target;

        _floatAnimations[id] = current;
        return current;
    }

    /// <summary>Animates a color toward a target using the current ImGui frame delta.</summary>
    /// <param name="id">Unique key for this animation's stored state.</param>
    /// <param name="target">Target color.</param>
    /// <param name="speed">Interpolation speed.</param>
    /// <param name="threshold">Distance from the target at which the color snaps to it.</param>
    /// <returns>The current interpolated color.</returns>
    public static Vector4 AnimateColor(string id, Vector4 target, float speed = 8f, float threshold = 0.001f)
    {
        if (!_colorAnimations.TryGetValue(id, out Vector4 current))
            current = target;

        current = Lerp(
            current,
            target,
            ImGui.GetIO().DeltaTime * speed
        );

        if (Vector4.Distance(current, target) < threshold)
            current = target;

        _colorAnimations[id] = current;
        return current;
    }

    /// <summary>Animates a vector toward a target using the current ImGui frame delta.</summary>
    /// <param name="id">Unique key for this animation's stored state.</param>
    /// <param name="target">Target vector.</param>
    /// <param name="speed">Interpolation speed.</param>
    /// <returns>The current interpolated vector.</returns>
    public static Vector2 AnimateVector2(string id, Vector2 target, float speed = 8f)
    {
        if (!_vectorAnimations.TryGetValue(id, out Vector2 current))
            current = target;

        current = Lerp(current, target, ImGui.GetIO().DeltaTime * speed);
        _vectorAnimations[id] = current;
        return current;
    }

    /// <summary>Linearly interpolates between two float values.</summary>
    public static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * Math.Clamp(t, 0f, 1f);
    }

    /// <summary>Linearly interpolates between two colors.</summary>
    public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return new Vector4(
            a.X + (b.X - a.X) * t,
            a.Y + (b.Y - a.Y) * t,
            a.Z + (b.Z - a.Z) * t,
            a.W + (b.W - a.W) * t
        );
    }

    /// <summary>Linearly interpolates between two vectors.</summary>
    public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return new Vector2(
            a.X + (b.X - a.X) * t,
            a.Y + (b.Y - a.Y) * t
        );
    }

    /// <summary>Removes stored animation state for an ID across all supported value types.</summary>
    /// <param name="id">Animation ID to reset.</param>
    public static void ResetAnimation(string id)
    {
        _floatAnimations.Remove(id);
        _colorAnimations.Remove(id);
        _vectorAnimations.Remove(id);
    }

    /// <summary>Removes all stored animation state.</summary>
    public static void ResetAllAnimations()
    {
        _floatAnimations.Clear();
        _colorAnimations.Clear();
        _vectorAnimations.Clear();
    }

    /// <summary>Maps a normalized value using quadratic ease-out.</summary>
    public static float EaseOutQuad(float t)
    {
        return t * (2 - t);
    }

    /// <summary>Maps a normalized value using quadratic ease-in.</summary>
    public static float EaseInQuad(float t)
    {
        return t * t;
    }

    /// <summary>Maps a normalized value using quadratic ease-in/ease-out.</summary>
    public static float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2 * t * t : 1 - (float)Math.Pow(-2 * t + 2, 2) / 2;
    }

    /// <summary>Maps a normalized value using an overshooting back ease-out curve.</summary>
    public static float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1;
        return 1 + c3 * (float)Math.Pow(t - 1, 3) + c1 * (float)Math.Pow(t - 1, 2);
    }
}
