// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Numerics;
using Nesi.UI.Utils;
using Nesi.UI.Style;
using ImGuiNET;

namespace Nesi.UI.Demo;

public class MainWindow : BaseWindow
{
    private readonly WindowManager _windowManager;
    private float _fps = 180f;
    private DateTime _lastUpdate = DateTime.Now;

    public MainWindow(WindowManager windowManager) : base("MainWindow")
    {
        _windowManager = windowManager;
        _size = new Vector2(400, 350);
    }

    protected override void RenderContent()
    {
        var windowWidth = ImGui.GetWindowWidth();
        float windowHeight = ImGui.GetWindowHeight();

        Controlbar("Nesi", Icons.Code, false, true, false);
        ImGui.Dummy(new Vector2(0, 5));
        DrawContent(windowWidth, windowHeight);
    }

    // widget provisório, só pra usar por enquanto
    public static void Controlbar(string title = "", string icon = "", bool background = false, bool canClose = true, bool canMinimize = false)
    {
        if (background)
        {
            var drawList = ImGui.GetWindowDrawList();

            Vector2 windowPos = ImGui.GetWindowPos();
            float windowWidth = ImGui.GetWindowSize().X;

            drawList.AddRectFilled(
                windowPos,
                new Vector2(windowPos.X + windowWidth, windowPos.Y + 30),
                ImGui.ColorConvertFloat4ToU32(Colors.headerBg)
            );
            ImGui.SetCursorPosY(30f + ImGui.GetStyle().WindowPadding.Y * 2);
        }

        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(icon))
        {
            ImGui.SetCursorPos(new Vector2(10, 10));
            ImGui.TextColored(Colors.accentColor, icon);
            Vector2 iconSize = ImGui.CalcTextSize(icon);
            ImGui.SetCursorPos(new Vector2(iconSize.X + 20, 10));
            ImGui.TextColored(Colors.DarkGray, title);
        }

        if (canClose)
        {
            Vector2 windowSize = ImGui.GetWindowSize();
            ImGui.SetCursorPos(new Vector2(windowSize.X - 24, 10));

            Vector2 pos = ImGui.GetCursorScreenPos();
            Vector2 size = new Vector2(14, 14);
            ImGui.InvisibleButton("##mac_close_btn", size);

            bool hovered = ImGui.IsItemHovered();
            bool clicked = ImGui.IsItemClicked();

            float t = Animation.Lerp(hovered ? 1f : 0f, 10f, ImGui.GetIO().DeltaTime * 8f);

            Vector4 hoverColor = Colors.RGBA(150f, 20f, 20f, 255f);
            Vector4 finalColor = Animation.Lerp(Colors.Red, hoverColor, t);

            float radius = 7f;
            Vector2 center = pos + new Vector2(radius, radius);
            Vector2 iconCenter = center + new Vector2(-0.5f, 0f);

            ImDrawListPtr drawList = ImGui.GetWindowDrawList();
            drawList.AddCircleFilled(
                center,
                radius,
                ImGui.ColorConvertFloat4ToU32(finalColor),
                16
            );

            Vector4 iconX = new(
                MathF.Max(finalColor.X - 0.45f, 0f),
                MathF.Max(finalColor.Y - 0.45f, 0f),
                MathF.Max(finalColor.Z - 0.45f, 0f),
                t
            );

            uint iconColor = ImGui.ColorConvertFloat4ToU32(iconX);

            const float arm = 2.9f;
            const float thickness = 1.7f;

            drawList.AddLine(
                new Vector2(iconCenter.X - arm, iconCenter.Y - arm),
                new Vector2(iconCenter.X + arm, iconCenter.Y + arm),
                iconColor,
                thickness);

            drawList.AddLine(
                new Vector2(iconCenter.X + arm, iconCenter.Y - arm),
                new Vector2(iconCenter.X - arm, iconCenter.Y + arm),
                iconColor,
                thickness);

            if (clicked) Environment.Exit(0);
        }
    }

    private void DrawContent(float windowWidth, float windowHeight)
    {
        var now = DateTime.Now;
        _fps = (float)(1.0 / (now - _lastUpdate).TotalSeconds);
        _lastUpdate = now;

        ImGui.Text($"FPS: {_fps:F1}");
        ImGui.Separator();
        ImGui.Dummy(new Vector2(0, 10));

        if (DrawButton("Menu", windowWidth))
            _windowManager.SwitchTo("Menu");
        ImGui.Dummy(new Vector2(0, 8));

        if (DrawButton("Settings", windowWidth))
            _windowManager.SwitchTo("Settings");
        ImGui.Dummy(new Vector2(0, 8));

        if (DrawButton("Clean cache", windowWidth))
            FolderManager.CleanCache();

        ImGui.Dummy(new Vector2(0, 8));

        ImGui.SetCursorPosY(windowHeight - 35f);
        ImGui.Separator();

        ImGui.TextColored(Colors.DarkGray, "System Status");
        ImGui.SameLine(windowWidth - 60f);
        ImGui.TextColored(new Vector4(0, 1, 0, 1), "Online");
    }

    private bool DrawButton(string label, float windowWidth)
    {
        float width = 180f;
        ImGui.SetCursorPosX(Nesi.UI.Utils.AppMath.GetCenterX(width, windowWidth));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 8f);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(10, 6));

        var result = ImGui.Button(label, new Vector2(width, 35));

        ImGui.PopStyleVar(2);
        return result;
    }
}

