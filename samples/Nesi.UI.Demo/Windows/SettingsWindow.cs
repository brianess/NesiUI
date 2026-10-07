// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Numerics;
using Nesi.UI.Utils;
using Nesi.UI.Style;
using ImGuiNET;
using Nesi.UI.Components;

namespace Nesi.UI.Demo;

public class SettingsWindow : BaseWindow
{
    private readonly WindowManager _windowManager;

    private int _selectedTab = 0;
    private readonly string[] _tabs =
    {
        "General",
        "Theme",
        "Protection"
    };

    // Settings
    private bool _autoStart = false;
    private bool _minimizeToTray = false;
    private int _fpsLimit = 144;

    private bool _streamProof = false;
    private bool _antiDebug = false;
    private bool _hwidCheck = false;

    // Root
    private VBox? _root;

    // Tab contents
    private VBox? _generalTab;
    private VBox? _themeTab;
    private VBox? _protectionTab;

    // Components
    private UICheckbox? _autoStartCheckbox;
    private UICheckbox? _minimizeToTrayCheckbox;
    private UISlider? _fpsSlider;

    private UIComboBox? _themeCombo;

    private UICheckbox? _streamProofCheckbox;
    private UICheckbox? _antiDebugCheckbox;
    private UICheckbox? _hwidCheckCheckbox;

    private UIText? _protectionStatus;

    public SettingsWindow(WindowManager windowManager)
        : base("SettingsWindow")
    {
        _windowManager = windowManager;

        _size = new Vector2(480, 400);
        IsVisible = false;


        InitializeUI();
    }

    protected override void RenderContent()
    {
        ThemeManager.Update();

        var windowWidth = ImGui.GetWindowWidth();

        DrawHeader(windowWidth);
        DrawTabs(windowWidth);

        _root?.Update();
        _root?.Render();
    }

    private void DrawHeader(float windowWidth)
    {
        var headerHeight = 40f;

        ImGui.SetCursorPos(
            new Vector2(
                12,
                (headerHeight - 28) / 2
            )
        );

        if (ImGui.Button(Icons.ArrowLeft))
        {
            _windowManager.GoBack();
        }

        var titleText = "Settings";
        var titleWidth = ImGui.CalcTextSize(titleText).X;

        ImGui.SetCursorPos(
            new Vector2(
                (windowWidth - titleWidth) / 2,
                (headerHeight - ImGui.GetFontSize()) / 2
            )
        );

        ImGui.TextColored(
            ThemeColors.Light.TextDisabled,
            titleText
        );

        ImGui.SetCursorPos(
            new Vector2(0, headerHeight)
        );

        ImGui.Separator();
    }

    private void DrawTabs(float windowWidth)
    {
        // A área do root começa depois das tabs.
        // As tabs continuam usando ImGui diretamente porque ainda não existe um UITab/UITabBar no meu framework

        ImGui.SetCursorPosY(48);

        var tabWidth = (windowWidth - 20f - 10f) / _tabs.Length;

        for (int i = 0; i < _tabs.Length; i++)
        {
            if (i > 0)
                ImGui.SameLine(0, 5);

            var isSelected = _selectedTab == i;

            var color = isSelected
                ? new Vector4(0.5f, 0.49f, 0.71f, 1f)
                : new Vector4(0.3f, 0.3f, 0.3f, 1f);

            ImGui.PushStyleColor(
                ImGuiCol.Button,
                isSelected
                    ? color * 0.3f
                    : new Vector4(0, 0, 0, 0)
            );

            ImGui.PushStyleColor(
                ImGuiCol.ButtonHovered,
                color * 0.2f
            );

            ImGui.PushStyleVar(
                ImGuiStyleVar.FrameRounding,
                6f
            );

            if (ImGui.Button(_tabs[i], new Vector2(tabWidth, 30)))
            {
                _selectedTab = i;
                UpdateSelectedTab();
            }

            ImGui.PopStyleVar();
            ImGui.PopStyleColor(2);
        }

        ImGui.SetCursorPosY(86);
        ImGui.Separator();
    }

    private void InitializeUI()
    {
        _root = new VBox("settings_root")
        {
            Spacing = 10f,
            Padding = new Vector2(10, 10)
        };

        CreateGeneralTab();
        CreateThemeTab();
        CreateProtectionTab();

        _root.AddElement(_generalTab!);
        _root.AddElement(_themeTab!);
        _root.AddElement(_protectionTab!);

        UpdateSelectedTab();
    }

    private void CreateGeneralTab()
    {
        _generalTab = new VBox("settings_general_tab")
        {
            Spacing = 8f
        };

        var group = new UIGroupBox(
            "General Settings",
            "settings_general_group"
        );

        _autoStartCheckbox = new UICheckbox(
            "Auto Start",
            _autoStart,
            "settings_auto_start"
        );

        _autoStartCheckbox.OnValueChanged += value =>
        {
            _autoStart = value;
        };

        group.AddElement(_autoStartCheckbox);

        _minimizeToTrayCheckbox = new UICheckbox(
            "Minimize to Tray",
            _minimizeToTray,
            "settings_minimize_tray"
        );

        _minimizeToTrayCheckbox.OnValueChanged += value =>
        {
            _minimizeToTray = value;
        };

        group.AddElement(_minimizeToTrayCheckbox);

        _fpsSlider = new UISlider(
            "FPS Limit",
            30f,
            300f,
            _fpsLimit,
            "settings_fps_limit"
        );

        _fpsSlider.OnValueChanged += value =>
        {
            _fpsLimit = (int)value;
        };

        group.AddElement(_fpsSlider);

        var saveButton = new UIButton(
            "Save Settings",
            "settings_save"
        );

        saveButton.OnClick += () =>
        {
            Logger.Info("Settings saved");
        };

        group.AddElement(saveButton);

        _generalTab.AddElement(group);
    }

    private void CreateThemeTab()
    {
        _themeTab = new VBox("settings_theme_tab")
        {
            Spacing = 8f
        };

        var group = new UIGroupBox(
            "Theme Settings",
            "settings_theme_group"
        );

        _themeCombo = new UIComboBox(
            "Theme",
            "settings_theme_combo"
        );

        _themeCombo.AddItem("Dark");
        _themeCombo.AddItem("Light");

        _themeCombo.SelectedIndex = 0;

        _themeCombo.OnSelectionChanged += (index, value) =>
        {
            ThemeManager.ApplyThemeWithAnimation(
                index == 0
                    ? ETheme.Dark
                    : ETheme.Light
            );
        };

        group.AddElement(_themeCombo);

        _themeTab.AddElement(group);
    }

    private void CreateProtectionTab()
    {
        _protectionTab = new VBox("settings_protection_tab")
        {
            Spacing = 8f
        };

        var group = new UIGroupBox(
            "Protection Settings",
            "settings_protection_group"
        );

        _streamProofCheckbox = new UICheckbox(
            "Stream Proof Mode",
            _streamProof,
            "settings_stream_proof"
        );

        _streamProofCheckbox.OnValueChanged += value =>
        {
            _streamProof = value;
            UpdateProtectionStatus();
        };

        group.AddElement(_streamProofCheckbox);

        _antiDebugCheckbox = new UICheckbox(
            "Anti-Debug",
            _antiDebug,
            "settings_anti_debug"
        );

        _antiDebugCheckbox.OnValueChanged += value =>
        {
            _antiDebug = value;
            UpdateProtectionStatus();
        };

        group.AddElement(_antiDebugCheckbox);

        _hwidCheckCheckbox = new UICheckbox(
            "HWID Check",
            _hwidCheck,
            "settings_hwid_check"
        );

        _hwidCheckCheckbox.OnValueChanged += value =>
        {
            _hwidCheck = value;
            UpdateProtectionStatus();
        };

        group.AddElement(_hwidCheckCheckbox);

        _protectionStatus = new UIText(
            "Protection Status: Inactive",
            "settings_protection_status"
        )
        {
            TextColor = Colors.Red
        };

        group.AddElement(_protectionStatus);

        _protectionTab.AddElement(group);
    }

    private void UpdateProtectionStatus()
    {
        if (_protectionStatus == null)
            return;

        var isProtected =
            _streamProof ||
            _antiDebug ||
            _hwidCheck;

        _protectionStatus.Text =
            $"Protection Status: {(isProtected ? "Active" : "Inactive")}";

        _protectionStatus.TextColor =
            isProtected
                ? Colors.Green
                : Colors.Red;
    }

    private void UpdateSelectedTab()
    {
        if (_generalTab != null)
            _generalTab.IsVisible = _selectedTab == 0;

        if (_themeTab != null)
            _themeTab.IsVisible = _selectedTab == 1;

        if (_protectionTab != null)
            _protectionTab.IsVisible = _selectedTab == 2;
    }

    public override void OnShow()
    {
        base.OnShow();
        IsVisible = true;

        UpdateSelectedTab();
    }

    public override void OnHide()
    {
        base.OnHide();
        IsVisible = false;
    }
}
