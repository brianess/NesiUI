// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Numerics;

namespace Nesi.UI.Style;

/// <summary>Shared mutable colors and color-conversion helpers used by framework styles.</summary>
public static class Colors
{
    // Main colors
    /// <summary>Default accent color.</summary>
    public static Vector4 accentColor = RGBA(10, 132, 255);
    /// <summary>Default background color.</summary>
    public static Vector4 backgroundColor = RGBA(28, 28, 30);
    /// <summary>Default header background color.</summary>
    public static Vector4 headerBg = RGBA(44, 44, 46);
    /// <summary>Default text color.</summary>
    public static Vector4 text = RGBA(245, 245, 247);

    // Basic colors
    /// <summary>Transparent black.</summary>
    public static Vector4 Zero = RGBA(0, 0, 0, 0);
    /// <summary>White.</summary>
    public static Vector4 White = RGBA(255, 255, 255);
    /// <summary>Black.</summary>
    public static Vector4 Black = RGBA(0, 0, 0);
    /// <summary>Dark gray.</summary>
    public static Vector4 DarkGray = RGBA(99, 99, 102);

    // System colors
    /// <summary>Red.</summary>
    public static Vector4 Red = RGBA(255, 69, 58);
    /// <summary>Yellow.</summary>
    public static Vector4 Yellow = RGBA(255, 214, 10);
    /// <summary>Green.</summary>
    public static Vector4 Green = RGBA(48, 209, 88);
    /// <summary>Blue.</summary>
    public static Vector4 Blue = RGBA(0, 122, 255);
    /// <summary>Orange.</summary>
    public static Vector4 Orange = RGBA(255, 159, 10);
    /// <summary>Pink.</summary>
    public static Vector4 Pink = RGBA(255, 55, 95);
    /// <summary>Purple.</summary>
    public static Vector4 Purple = RGBA(191, 90, 242);
    /// <summary>Gray.</summary>
    public static Vector4 Gray = RGBA(142, 142, 147);
    /// <summary>Disabled-state gray.</summary>
    public static Vector4 Disabled = RGBA(142, 142, 147, 128);

    /// <summary>Returns a color with its alpha channel replaced.</summary>
    /// <param name="color">Source color.</param>
    /// <param name="alpha">Replacement alpha value.</param>
    /// <returns>A copy of <paramref name="color"/> with the specified alpha.</returns>
    public static Vector4 GetAlpha(Vector4 color, float alpha)
    {
        return new Vector4(color.X, color.Y, color.Z, alpha);
    }

    /// <summary>Creates a normalized color from 0-255 RGBA channel values.</summary>
    /// <param name="r">Red channel.</param>
    /// <param name="g">Green channel.</param>
    /// <param name="b">Blue channel.</param>
    /// <param name="a">Alpha channel, defaulting to 255.</param>
    /// <returns>A normalized color vector.</returns>
    public static Vector4 RGBA(float r, float g, float b, float a = 255)
    {
        return new Vector4(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    /// <summary>Creates a color from a hue value in the range 0 to 1.</summary>
    /// <param name="h">Hue; values are wrapped modulo one.</param>
    /// <returns>The corresponding color.</returns>
    public static Vector4 ColorFromHue(float h)
    {
        h %= 1f;
        h *= 6f;

        int i = (int)h;
        float f = h - i;
        float q = 1f - f;

        return i switch
        {
            0 => new Vector4(1, f, 0, 1),
            1 => new Vector4(q, 1, 0, 1),
            2 => new Vector4(0, 1, f, 1),
            3 => new Vector4(0, q, 1, 1),
            4 => new Vector4(f, 0, 1, 1),
            _ => new Vector4(1, 0, q, 1)
        };
    }
}
