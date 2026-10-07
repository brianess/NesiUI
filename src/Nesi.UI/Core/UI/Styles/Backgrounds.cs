// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Numerics;
using ImGuiNET;

namespace Nesi.UI.Style;

/// <summary>Built-in draw-list background effects for windows and panels.</summary>
public enum BackgroundType
{
    /// <summary>Does not draw a framework background effect.</summary>
    None,
    /// <summary>Draws a translucent layered glass-like effect; it is not a pixel blur.</summary>
    Glass,
    /// <summary>Draws animated aurora-like curves.</summary>
    Aurora,
    /// <summary>Draws a neon-style background.</summary>
    Neon,
    /// <summary>Draws a grid.</summary>
    Grid,
    /// <summary>Draws animated particles.</summary>
    Particles,
    /// <summary>Draws animated waves.</summary>
    Waves
}

/// <summary>Draws built-in background effects into an ImGui draw list.</summary>
public static class Background
{
    /// <summary>Renders a background effect within the specified rectangle.</summary>
    /// <param name="drawList">The target ImGui draw list.</param>
    /// <param name="min">Top-left point in screen coordinates.</param>
    /// <param name="size">Rectangle size in pixels.</param>
    /// <param name="cornerRadius">Corner radius in pixels.</param>
    /// <param name="opacity">Effect opacity from zero to one.</param>
    /// <param name="type">Background effect to draw.</param>
    /// <param name="color">Optional accent color for effects that use one.</param>
    public static void Render(ImDrawListPtr drawList, Vector2 min, Vector2 size, float cornerRadius, float opacity, BackgroundType type, Vector4? color = null)
    {
        var accentColor = color ?? Colors.accentColor;

        switch (type)
        {
            case BackgroundType.Glass:
                Glass(drawList, min, size, cornerRadius, opacity);
                break;

            case BackgroundType.Aurora:
                Aurora(drawList, min, size, cornerRadius, opacity, accentColor);
                break;

            case BackgroundType.Neon:
                Neon(drawList, min, size, cornerRadius, opacity, accentColor);
                break;

            case BackgroundType.Grid:
                Grid(drawList, min, size, cornerRadius, opacity, accentColor);
                break;

            case BackgroundType.Particles:
                Particles(drawList, min, size, cornerRadius, opacity, accentColor);
                break;

            case BackgroundType.Waves:
                Waves(drawList, min, size, cornerRadius, opacity, accentColor);
                break;
        }
    }

    private static void Glass(ImDrawListPtr drawList, Vector2 min, Vector2 size, float cornerRadius, float opacity)
    {
        var max = min + size;
        var radius = Math.Clamp(cornerRadius, 0f, Math.Min(size.X, size.Y) * 0.5f);

        drawList.AddRectFilled(
            min,
            max,
            ImGui.ColorConvertFloat4ToU32(Colors.GetAlpha(Colors.backgroundColor, opacity)),
            radius
        );

        var highlightMin = min + new Vector2(4f, 2f);
        var highlightMax = new Vector2(max.X - 4f, min.Y + 6f);

        if (highlightMax.X > highlightMin.X)
        {
            drawList.AddRectFilled(
                highlightMin,
                highlightMax,
                ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, opacity * 0.02f)),
                Math.Max(0f, radius - 2f)
            );
        }

        drawList.AddRect(
            min,
            max,
            ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, opacity * 0.08f)),
            radius
        );

        var glowMin = min + new Vector2(2f);
        var glowMax = max - new Vector2(2f);

        if (glowMax.X > glowMin.X && glowMax.Y > glowMin.Y)
        {
            drawList.AddRect(
                glowMin,
                glowMax,
                ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, opacity * 0.03f)),
                Math.Max(0f, radius - 1f)
            );
        }
    }

    private static void Aurora(ImDrawListPtr drawList, Vector2 min, Vector2 size, float cornerRadius, float opacity, Vector4 color)
    {
        var max = min + size;
        var radius = Math.Clamp(cornerRadius, 0f, Math.Min(size.X, size.Y) * 0.5f);
        var time = (float)ImGui.GetTime();

        drawList.AddRectFilled(
            min,
            max,
            ImGui.ColorConvertFloat4ToU32(Colors.GetAlpha(Colors.backgroundColor, opacity)),
            radius
        );

        const int waves = 14;

        for (int i = 0; i < waves; i++)
        {
            float t = i / (float)(waves - 1);
            float offset = i * 0.55f;

            float wave1 = MathF.Sin(time * 0.7f + offset) * 45f;
            float wave2 = MathF.Sin(time * 1.15f - offset) * 20f;

            var p1 = new Vector2(min.X - 80f, min.Y + size.Y * (0.2f + t * 0.65f));
            var p2 = new Vector2(min.X + size.X * 0.3f, min.Y + size.Y * 0.15f + wave1 + wave2);
            var p3 = new Vector2(min.X + size.X * 0.7f, min.Y + size.Y * 0.85f - wave1);
            var p4 = new Vector2(max.X + 80f, min.Y + size.Y * (0.25f + t * 0.55f));

            float alpha = (0.025f + (1f - MathF.Abs(t - 0.5f) * 1.5f) * 0.035f) * opacity;
            var baseColor = new Vector4(color.X, color.Y, color.Z, alpha);

            for (int glow = 6; glow >= 1; glow--)
            {
                float thickness = 2f + glow * 3f;
                float glowAlpha = alpha * (0.10f + (6 - glow) * 0.025f);
                var glowColor = ImGui.ColorConvertFloat4ToU32(new Vector4(color.X, color.Y, color.Z, glowAlpha));

                drawList.AddBezierCubic(p1, p2, p3, p4, glowColor, thickness);
            }

            drawList.AddBezierCubic(p1, p2, p3, p4, ImGui.ColorConvertFloat4ToU32(baseColor), 1.5f);
        }

        for (int i = 0; i < 35; i++)
        {
            float seed = i * 17.31f;
            float x = MathF.Abs(MathF.Sin(seed)) * size.X;
            float y = MathF.Abs(MathF.Sin(seed * 2.37f)) * size.Y;
            float movement = MathF.Sin(time * 0.5f + seed) * 10f;

            var particle = new Vector2(min.X + x, min.Y + y + movement);
            float pulse = 0.5f + MathF.Sin(time * 2f + seed) * 0.5f;

            uint particleColor = ImGui.ColorConvertFloat4ToU32(
                new Vector4(color.X, color.Y, color.Z, (0.15f + pulse * 0.25f) * opacity)
            );

            drawList.AddCircleFilled(particle, 1.5f, particleColor, 8);
        }
    }

    private static void Neon(ImDrawListPtr drawList, Vector2 min, Vector2 size, float cornerRadius, float opacity, Vector4 color)
    {
        var max = min + size;

        drawList.AddRectFilled(
            min,
            max,
            ImGui.ColorConvertFloat4ToU32(Colors.GetAlpha(Colors.backgroundColor, opacity)),
            cornerRadius
        );

        var time = (float)ImGui.GetTime();

        for (int i = -2; i < 6; i++)
        {
            float x = min.X + i * 140f;

            Vector2 start = new(x + MathF.Sin(time + i) * 30f, max.Y);
            Vector2 end = new(x + size.X * 0.35f, min.Y);

            for (int glow = 5; glow >= 1; glow--)
            {
                uint glowColor = ImGui.ColorConvertFloat4ToU32(
                    new Vector4(color.X, color.Y, color.Z, opacity * 0.025f)
                );

                drawList.AddLine(start, end, glowColor, glow * 3f);
            }

            drawList.AddLine(
                start,
                end,
                ImGui.ColorConvertFloat4ToU32(new Vector4(color.X, color.Y, color.Z, opacity * 0.25f)),
                1.5f
            );
        }
    }

    private static void Grid(ImDrawListPtr drawList, Vector2 min, Vector2 size, float cornerRadius, float opacity, Vector4 color)
    {
        var max = min + size;
        var time = (float)ImGui.GetTime();

        drawList.AddRectFilled(
            min,
            max,
            ImGui.ColorConvertFloat4ToU32(Colors.GetAlpha(Colors.backgroundColor, opacity)),
            cornerRadius
        );

        const float spacing = 35f;
        float offset = (time * 15f) % spacing;

        uint gridColor = ImGui.ColorConvertFloat4ToU32(
            new Vector4(color.X, color.Y, color.Z, opacity * 0.07f)
        );

        for (float x = -spacing + offset; x < size.X + spacing; x += spacing)
        {
            drawList.AddLine(new Vector2(min.X + x, min.Y), new Vector2(min.X + x, max.Y), gridColor, 1f);
        }

        for (float y = -spacing + offset; y < size.Y + spacing; y += spacing)
        {
            drawList.AddLine(new Vector2(min.X, min.Y + y), new Vector2(max.X, min.Y + y), gridColor, 1f);
        }
    }

    private static void Particles(ImDrawListPtr drawList, Vector2 min, Vector2 size, float cornerRadius, float opacity, Vector4 color)
    {
        var max = min + size;
        var time = (float)ImGui.GetTime();

        drawList.AddRectFilled(
            min,
            max,
            ImGui.ColorConvertFloat4ToU32(Colors.GetAlpha(Colors.backgroundColor, opacity)),
            cornerRadius
        );

        for (int i = 0; i < 100; i++)
        {
            float seed = i * 13.731f;
            float x = MathF.Abs(MathF.Sin(seed)) * size.X;
            float baseY = MathF.Abs(MathF.Cos(seed * 1.73f)) * size.Y;

            float y = baseY + MathF.Sin(time * (0.3f + i % 3 * 0.15f) + seed) * 15f;

            float pulse = 0.5f + MathF.Sin(time * 2f + seed) * 0.5f;
            float particleSize = 1f + pulse * 1.5f;

            uint particleColor = ImGui.ColorConvertFloat4ToU32(
                new Vector4(color.X, color.Y, color.Z, opacity * (0.1f + pulse * 0.3f))
            );

            drawList.AddCircleFilled(new Vector2(min.X + x, min.Y + y), particleSize, particleColor, 8);
        }
    }

    private static void Waves(ImDrawListPtr drawList, Vector2 min, Vector2 size, float cornerRadius, float opacity, Vector4 color)
    {
        var max = min + size;
        var time = (float)ImGui.GetTime();

        drawList.AddRectFilled(
            min,
            max,
            ImGui.ColorConvertFloat4ToU32(Colors.GetAlpha(Colors.backgroundColor, opacity)),
            cornerRadius
        );

        const int lines = 30;

        for (int i = 0; i < lines; i++)
        {
            float t = i / (float)(lines - 1);
            float y = t * size.Y;

            float wave1 = MathF.Sin(t * 9f + time * 0.8f) * 25f;
            float wave2 = MathF.Sin(t * 17f - time * 1.2f) * 12f;
            float wave = wave1 + wave2;

            Vector2 p1 = new(min.X, min.Y + y);
            Vector2 p2 = new(min.X + size.X * 0.3f, min.Y + y + wave);
            Vector2 p3 = new(min.X + size.X * 0.7f, min.Y + y - wave);
            Vector2 p4 = new(max.X, min.Y + y);

            uint waveColor = ImGui.ColorConvertFloat4ToU32(
                new Vector4(color.X, color.Y, color.Z, opacity * 0.35f)
            );

            drawList.AddBezierCubic(p1, p2, p3, p4, waveColor, 1.5f);
        }
    }
}