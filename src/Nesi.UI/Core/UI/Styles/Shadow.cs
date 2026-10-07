// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Numerics;
using ImGuiNET;

namespace Nesi.UI.Style;

/// <summary>Draws a soft shadow around a rectangle.</summary>
public static class Shadow
{
    private static float _opacity = 0f;

    private enum ShadowCorner
    {
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft
    }

    /// <summary>Draws a shadow into an ImGui draw list.</summary>
    /// <param name="pos">Top-left point of the rectangle.</param>
    /// <param name="size">Rectangle size.</param>
    /// <param name="dw">Target draw list; a null-equivalent handle uses the background draw list.</param>
    /// <param name="opacity">Shadow opacity multiplier.</param>
    /// <param name="radius">Shadow spread radius in pixels.</param>
    /// <param name="strength">Maximum shadow strength.</param>
    /// <param name="cornerInset">Corner adjustment in pixels.</param>
    public static void Draw(Vector2 pos, Vector2 size, ImDrawListPtr dw, float opacity = 1f, float radius = 16f, float strength = 0.6f, float cornerInset = 0f)
    {
        _opacity = opacity;

        if (dw.Equals(null))
            dw = ImGui.GetBackgroundDrawList();

        uint noColor = ImGui.ColorConvertFloat4ToU32(
            Colors.RGBA(0f, 0f, 0f, 0f)
        );

        uint blackColor = ImGui.ColorConvertFloat4ToU32(
            Colors.GetAlpha(Colors.Black, strength * _opacity)
        );

        // Top
        dw.AddRectFilledMultiColor(
            new Vector2(pos.X, pos.Y - radius),
            new Vector2(pos.X + size.X, pos.Y),
            noColor,
            noColor,
            blackColor,
            blackColor
        );

        // Right
        dw.AddRectFilledMultiColor(
            new Vector2(pos.X + size.X, pos.Y),
            new Vector2(pos.X + size.X + radius, pos.Y + size.Y),
            blackColor,
            noColor,
            noColor,
            blackColor
        );

        // Bottom
        dw.AddRectFilledMultiColor(
            new Vector2(pos.X, pos.Y + size.Y),
            new Vector2(pos.X + size.X, pos.Y + size.Y + radius),
            blackColor,
            blackColor,
            noColor,
            noColor
        );

        // Left
        dw.AddRectFilledMultiColor(
            new Vector2(pos.X - radius, pos.Y),
            new Vector2(pos.X, pos.Y + size.Y),
            noColor,
            blackColor,
            blackColor,
            noColor
        );

        // Corners
        DrawShadowCorner(
            dw,
            new Vector2(pos.X + cornerInset, pos.Y + cornerInset),
            ShadowCorner.TopLeft,
            radius,
            strength
        );

        DrawShadowCorner(
            dw,
            new Vector2(pos.X + size.X - cornerInset, pos.Y + cornerInset),
            ShadowCorner.TopRight,
            radius,
            strength
        );

        DrawShadowCorner(
            dw,
            new Vector2(pos.X + size.X - cornerInset, pos.Y + size.Y - cornerInset),
            ShadowCorner.BottomRight,
            radius,
            strength
        );

        DrawShadowCorner(
            dw,
            new Vector2(pos.X + cornerInset, pos.Y + size.Y - cornerInset),
            ShadowCorner.BottomLeft,
            radius,
            strength
        );
    }

    private static void DrawShadowCorner(ImDrawListPtr dw, Vector2 center, ShadowCorner corner, float radius, float strength)
    {
        const int layers = 32;

        Vector2 clipMin;
        Vector2 clipMax;

        switch (corner)
        {
            case ShadowCorner.TopLeft:
                clipMin = new Vector2(center.X - radius, center.Y - radius);
                clipMax = center;
                break;

            case ShadowCorner.TopRight:
                clipMin = new Vector2(center.X, center.Y - radius);
                clipMax = new Vector2(center.X + radius, center.Y);
                break;

            case ShadowCorner.BottomRight:
                clipMin = center;
                clipMax = new Vector2(center.X + radius, center.Y + radius);
                break;

            default: // BottomLeft
                clipMin = new Vector2(center.X - radius, center.Y);
                clipMax = new Vector2(center.X, center.Y + radius);
                break;
        }

        dw.PushClipRect(clipMin, clipMax, true);

        float previousTargetAlpha = 0f;

        for (int i = layers; i >= 1; i--)
        {
            float t = (float)i / layers;
            float currentRadius = radius * t;
            float targetAlpha = strength * (1f - t) * _opacity;

            float alpha =
                (targetAlpha - previousTargetAlpha) /
                (1f - previousTargetAlpha);

            if (alpha > 0f)
            {
                uint color = ImGui.ColorConvertFloat4ToU32(
                    Colors.GetAlpha(Colors.Black, alpha)
                );

                dw.AddCircleFilled(
                    center,
                    currentRadius,
                    color,
                    32
                );
            }

            previousTargetAlpha = targetAlpha;
        }

        dw.PopClipRect();
    }
}
