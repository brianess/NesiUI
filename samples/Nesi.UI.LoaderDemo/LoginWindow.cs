// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Numerics;
using ImGuiNET;
using Nesi.UI.Components;
using Nesi.UI.Style;
using Nesi.UI.Utils;

namespace Nesi.UI.LoaderDemo;

public class LoginWindow : BaseWindow
{
    private readonly WindowManager _wm;
    private readonly VBox _root = new("vbox_main");

    public LoginWindow(WindowManager windowManager) : base("LoginWindow")
    {
        _wm = windowManager;
        _size = new Vector2(500, 450);

        InitializeUI();
    }

    protected void InitializeUI()
    {
        var group = new UIGroupBox("Login", "login_gb");

        var inputUser = new UITextInput("Username", "user_inp");
        var inputPass = new UITextInput("Password", "pass_inp");

        var submitButton = new UIButton("Sign in", "submit_btn");
        submitButton.Style = UIButtonStyle.GroupBox;

        var helpText = new UIText("Need help? click here");
        helpText.TextColor = Colors.Disabled;

        group.AddElement(new UIText("Connect"));
        group.AddElement(inputUser);
        group.AddElement(inputPass);
        group.AddElement(submitButton);
        group.AddElement(helpText);

        _root.AddElement(group);
    }

    protected override void RenderContent()
    {
        DrawCloseButton();

        _root.Update();
        _root.Render();
    }

    void DrawCloseButton()
    {
        var closeText = "×";
        var closeTextSize = ImGui.CalcTextSize(closeText);
        var closePos = new Vector2(_size.X - closeTextSize.X - 10f, 0f);

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
