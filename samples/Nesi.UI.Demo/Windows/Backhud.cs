// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Numerics;
using Nesi.UI.Style;
using Nesi.UI.Utils;
using ImGuiNET;

namespace Nesi.UI.Demo;

public static class Backhud
{
    private static string _brand = "Nesi";
    private static string _username = "dev";

    public static bool bEsp = false;

    public static void Render()
    {
        Watermark();

        if (!bEsp)
            return;

        DrawEspBox(
            new Vector2(500, 300),
            new Vector2(500, 600),
            Colors.accentColor
        );

        DrawEspBox(
            new Vector2(800, 250),
            new Vector2(800, 550),
            Colors.Purple
        );

        DrawEspBox(
            new Vector2(1100, 350),
            new Vector2(1100, 700),
            Colors.Blue
        );
    }

    private static void DrawEspBox(Vector2 head, Vector2 feet, Vector4 color)
    {
        var draw = ImGui.GetBackgroundDrawList();

        float height = feet.Y - head.Y;

        if (height <= 1f)
            return;

        // Player box width based on height.
        float width = height * 0.45f;

        Vector2 min = new Vector2(
            head.X - width * 0.5f,
            head.Y
        );

        Vector2 max = new Vector2(
            head.X + width * 0.5f,
            feet.Y
        );

        float rounding = MathF.Min(
            5f,
            width * 0.12f
        );

        uint border = ImGui.ColorConvertFloat4ToU32(color);

        uint fill = ImGui.ColorConvertFloat4ToU32(
            Colors.GetAlpha(color, 0.12f)
        );

        uint outline = ImGui.ColorConvertFloat4ToU32(
            Colors.Black
        );

        // Outer black outline.
        draw.AddRect(
            min - new Vector2(2f),
            max + new Vector2(2f),
            outline,
            rounding,
            ImDrawFlags.None,
            1.5f
        );

        // Transparent fill.
        draw.AddRectFilled(
            min,
            max,
            fill,
            rounding
        );

        // Main colored border.
        draw.AddRect(
            min,
            max,
            border,
            rounding,
            ImDrawFlags.None,
            1.5f
        );
    }

    static void Watermark(float alpha = 0.6f)
    {
        var drawList = ImGui.GetBackgroundDrawList();
        var io = ImGui.GetIO();

        var items = new[]
        {
            (Icons.Brand, true),
            (" " + _brand, false),
            ("    ", false),
            (Icons.Fps, true),
            (" " + (int)io.Framerate + " FPS", false),
            ("    ", false),
            (Icons.User, true),
            (" " + _username, false),
            ("    ", false),
            (Icons.Stopwatch, true),
            (" " + DateTime.Now.ToString("HH:mm:ss"), false)
        };

        var sizes = items.Select(x => ImGui.CalcTextSize(x.Item1)).ToArray();

        float totalWidth = sizes.Sum(x => x.X);
        float totalHeight = sizes.Max(x => x.Y);

        const float padX = 14f;
        const float padY = 8f;

        Vector2 min = new(
            (Helper.GetScreenWidth() / 2f) - (totalWidth / 2f) - padX,
            12f
        );

        Vector2 size = new(
            totalWidth + padX * 2f,
            totalHeight + padY * 2f
        );

        Background.Render(
            drawList,
            min,
            size,
            10f,
            alpha,
            BackgroundType.Glass
        );

        Vector2 textPos = min + new Vector2(padX, padY + 1f);

        foreach (var (text, isIcon) in items)
        {
            drawList.AddText(
                textPos,
                ImGui.ColorConvertFloat4ToU32(
                    isIcon ? Colors.accentColor : Colors.GetAlpha(Colors.text, 0.9f)
                ),
                text
            );

            textPos.X += ImGui.CalcTextSize(text).X;
        }
    }

}


