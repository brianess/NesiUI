// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using Nesi.UI.Components;
using System.Numerics;
using Nesi.UI.Style;
using ImGuiNET;

namespace Nesi.UI.SimpleDemo;

// Implements abstract class "BaseWindow" to make easily window
public class SimpleWindow : BaseWindow
{
    // Configs
    private readonly WindowManager _windowManager;
    private readonly VBox _root = new("vbox_main");

    // Elements bind
    private readonly UIText _textFps = new("0 FPS", "text_fps_main");
    private readonly UIText _textClickCounts = new("Click counts: ");
    private readonly UIText _textCheckboxValue = new("Checkbox value: ");
    private readonly UIText _textSliderValue = new("Slider value: ");

    // Bindings
    private float _fps = 180f;
    private DateTime _lastUpdate = DateTime.Now;
    static int iClickCounts = 0;
    static bool bCheckbox = false;
    static float fSlider = 1f;

    // Constructor
    public SimpleWindow(WindowManager windowManager) : base("SimpleWindow")
    {
        // Setup configs
        _windowManager = windowManager;
        _size = new Vector2(600, 450);

        // Call UI once time 
        InitializeUI();
    }

    // Build your graphical user interface here (run once frame)
    private void InitializeUI()
    {
        // Titlebar example
        var textTitle = new UIText("Simple Demo");
        textTitle.TextColor = Colors.Disabled;

        // Split layout
        var splitView = new SplitView("split_main")
        {
            SplitterSize = 10f,
            FirstSize = _size.X / 2 - 30f
        };

        // Groupboxes
        var groupLeft = new UIGroupBox("Left panel");
        var groupRight = new UIGroupBox("Right panel");

        // Elements
        var button = new UIButton("Click me", "btn_main");
        button.OnClick += () => iClickCounts += 1; // Events

        var checkbox = new UICheckbox("Change me", bCheckbox, "cbox_main");
        checkbox.OnValueChanged += value => bCheckbox = value;

        var slider = new UISlider("Slide me", 0f, 10f, fSlider, "sld_main");
        slider.OnValueChanged += value => fSlider = value;

        var buttonClose = new UIButton("Close", "btn_close_main");
        buttonClose.OnClick += () => Environment.Exit(0);

        // Left group
        groupLeft.AddElement(new UIText("Simple Use Example", "text_title_main"));
        groupLeft.AddElement(button);
        groupLeft.AddElement(checkbox);
        groupLeft.AddElement(slider);
        groupLeft.AddElement(_textFps);

        // Right group
        groupRight.AddElement(_textClickCounts);
        groupRight.AddElement(_textCheckboxValue);
        groupRight.AddElement(_textSliderValue);
        groupRight.AddElement(buttonClose);

        // Add groupboxes to splitview layout
        splitView.AddElement(groupLeft);
        splitView.AddElement(groupRight);

        // Add layouts to root grid
        _root.AddElement(textTitle);
        _root.AddElement(splitView);
    }

    // Method run every frame
    protected override void RenderContent()
    {
        var now = DateTime.Now;
        _fps = (float)(1.0 / (now - _lastUpdate).TotalSeconds);
        _lastUpdate = now;

        _textFps.Text = $"{_fps:F0} FPS";
        _textClickCounts.Text = "Click counts: " + iClickCounts;
        _textCheckboxValue.Text = "Checkbox value: " + bCheckbox;
        _textSliderValue.Text = "Slider value: " + fSlider;

        _root.Update();
        _root.Render();

        UpdateDrawDebug();
    }

    private void UpdateDrawDebug()
    {
        // Configs
        var dw = ImGui.GetForegroundDrawList();
        var col = ImGui.ColorConvertFloat4ToU32(ImGui.GetStyle().Colors[(int)ImGuiCol.CheckMark]);
        var size = new Vector2(200, 200);
        var pos = new Vector2((1920f - size.X) / 2f, (1080f - size.Y) / 2f);

        if (!bCheckbox)
            return;

        // Box - glass + shadow
        // Background.Render(dw, pos, size, 0, 0.5f, BackgroundType.Glass);
        // Shadow.Draw(pos, size, dw, 0.2f);

        // GTA-like weapon wheel
        WeaponWheel.Render();
    }
}