using System;
using System.Numerics;
using ImGuiNET;
using Nesi.UI.Style;
using Nesi.UI.Utils;

namespace Nesi.UI.Core.UI.Components;

public static class PreUI
{
    public static void DrawCenterTitle(string text, Vector2 winSize)
    {
        // Configs
        var textSize = ImGui.CalcTextSize(text);

        // Render
        ImGui.SetCursorPos(new Vector2(winSize.X / 2 - textSize.X / 2, winSize.Y / 2 - textSize.Y / 2));
        ImGui.TextColored(Colors.DarkGray, text);
    }

    public static void DrawCloseButton(Vector2 winSize)
    {
        var closeText = "×";
        var closeTextSize = ImGui.CalcTextSize(closeText);
        var closePos = new Vector2(winSize.X - closeTextSize.X - 10f, 0f);

        ImGui.SetCursorPos(closePos);

        ImGui.InvisibleButton("##close_button", closeTextSize);

        var isHovered = ImGui.IsItemHovered();
        var closeColor = Animation.AnimateColor(
            "close_button_color",
            isHovered ? Colors.Red : Colors.DarkGray
        );

        var drawList = ImGui.GetWindowDrawList();
        var textPos = ImGui.GetItemRectMin();

        drawList.AddText(textPos, ImGui.ColorConvertFloat4ToU32(closeColor), closeText);

        if (ImGui.IsItemClicked()) Environment.Exit(0);
    }
}
