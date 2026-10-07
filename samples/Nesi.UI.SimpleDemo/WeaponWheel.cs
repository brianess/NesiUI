// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Nesi.UI.Style;
using Nesi.UI.Utils;

namespace Nesi.UI.SimpleDemo;

public static class WeaponWheel
{
    public static void Render()
    {
        var dw = ImGui.GetForegroundDrawList();
        var arcCol = ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.3f, 0.3f, 0.5f));
        var center = new Vector2(1920f / 2f, 1080f / 2f);
        var radius = 250f;
        var innerRadius = 100f;
        var gap = 8f;

        // background
        var bgCol = ImGui.ColorConvertFloat4ToU32(Colors.GetAlpha(Colors.Black, 0.3f));
        dw.AddRectFilled(Vector2.Zero, new Vector2(1920, 1080), bgCol);

        // arc blocks
        if (DrawArc(dw, center, innerRadius, radius, arcCol, -135f, -45f, gap))
            Logger.Info("Top clicked");

        if (DrawArc(dw, center, innerRadius, radius, arcCol, -45f, 45f, gap))
            Logger.Info("Right clicked");

        if (DrawArc(dw, center, innerRadius, radius, arcCol, 45f, 135f, gap))
            Logger.Info("Bottom clicked");

        if (DrawArc(dw, center, innerRadius, radius, arcCol, 135f, 225f, gap))
            Logger.Info("Left clicked");

    }

    private static bool DrawArc(ImDrawListPtr drawList, Vector2 center, float innerRadius, float outerRadius, uint color, float startDeg, float endDeg, float gap)
    {
        var gapAngle = 2f * MathF.Asin(gap / (2f * outerRadius)) * 180f / MathF.PI;
        var drawStartDeg = startDeg + gapAngle / 2f;
        var drawEndDeg = endDeg - gapAngle / 2f;

        var mousePos = ImGui.GetMousePos();
        var mouseDelta = mousePos - center;
        var mouseDistance = mouseDelta.Length();

        var hovered = mouseDistance >= innerRadius && mouseDistance <= outerRadius;

        var mouseAngle = MathF.Atan2(mouseDelta.Y, mouseDelta.X) * 180f / MathF.PI;

        if (mouseAngle < 0f)
            mouseAngle += 360f;

        var hitStartDeg = drawStartDeg % 360f;

        if (hitStartDeg < 0f)
            hitStartDeg += 360f;

        var hitEndDeg = drawEndDeg % 360f;

        if (hitEndDeg < 0f)
            hitEndDeg += 360f;

        var angleInHitArea = hitStartDeg <= hitEndDeg
            ? mouseAngle >= hitStartDeg && mouseAngle <= hitEndDeg
            : mouseAngle >= hitStartDeg || mouseAngle <= hitEndDeg;

        hovered &= angleInHitArea;

        var finalColor = hovered
            ? ImGui.ColorConvertFloat4ToU32(Colors.GetAlpha(Colors.White, 0.35f))
            : color;

        const int segments = 32;

        for (int i = 0; i < segments; i++)
        {
            var t1 = i / (float)segments;
            var t2 = (i + 1) / (float)segments;

            var outerAngle1 = drawStartDeg + (drawEndDeg - drawStartDeg) * t1;
            var outerAngle2 = drawStartDeg + (drawEndDeg - drawStartDeg) * t2;

            var innerAngle1 = drawStartDeg + (drawEndDeg - drawStartDeg) * t1;
            var innerAngle2 = drawStartDeg + (drawEndDeg - drawStartDeg) * t2;

            var outer1 = center + new Vector2(MathF.Cos(AngleToRad(outerAngle1)), MathF.Sin(AngleToRad(outerAngle1))) * outerRadius;
            var outer2 = center + new Vector2(MathF.Cos(AngleToRad(outerAngle2)), MathF.Sin(AngleToRad(outerAngle2))) * outerRadius;

            var inner1 = center + new Vector2(MathF.Cos(AngleToRad(innerAngle1)), MathF.Sin(AngleToRad(innerAngle1))) * innerRadius;
            var inner2 = center + new Vector2(MathF.Cos(AngleToRad(innerAngle2)), MathF.Sin(AngleToRad(innerAngle2))) * innerRadius;

            drawList.AddQuadFilled(outer1, outer2, inner2, inner1, finalColor);
        }

        return hovered && ImGui.IsMouseClicked(ImGuiMouseButton.Left);
    }

    private static float AngleToRad(float angle) => angle * MathF.PI / 180f;
}
