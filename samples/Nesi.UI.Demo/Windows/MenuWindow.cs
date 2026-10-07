// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Numerics;
using Nesi.UI.Utils;
using Nesi.UI.Style;
using ImGuiNET;
using Nesi.UI.Components;

namespace Nesi.UI.Demo;

public class MenuWindow : BaseWindow
{
    readonly WindowManager _windowManager;
    private readonly AssetManager _assets;

    // toggle visibility animated
    static float _globalAlpha;
    static float _oldAlpha;
    static float _contentOffsetY = 0f;
    static bool _menuOpen = true;
    static bool _isAnimating = false;
    // public api
    public static bool IsAnimating => _isAnimating;
    public static float GlobalAlpha => _globalAlpha;
    // temp vars
    Vector2 _windowSize;

    // components
    private VBox? _root;
    private UIText? _statusLabel;

    public MenuWindow(WindowManager windowManager, AssetManager assets) : base("MenuWindow")
    {
        _windowManager = windowManager;
        _assets = assets;

        _size = new Vector2(700, 500);
        _windowSize = _size;

        _menuOpen = true;
        _oldAlpha = _windowManager.CurrentWindow!.Style.BackgroundOpacity;
        _globalAlpha = _oldAlpha;

        InitializeUI();
    }

    // public prop to control menu visibility
    public static bool IsMenuOpen
    {
        get => _menuOpen;
        set
        {
            if (_menuOpen == value)
                return;

            _menuOpen = value;
            _isAnimating = true;
        }
    }

    public void ToggleMenu()
    {
        IsMenuOpen = !IsMenuOpen;
        if (IsMenuOpen)
            IsVisible = true;
    }

    public static void ResetAnimation()
    {
        _globalAlpha = 0f;
        _contentOffsetY = 60f;
        _isAnimating = true;
    }

    protected override void RenderContent()
    {
        if (!_menuOpen && _globalAlpha <= 0f && !_isAnimating)
        {
            IsVisible = false;
            return;
        }

        ThemeManager.Update();

        if (_isAnimating)
        {
            float dt = ImGui.GetIO().DeltaTime;
            float speed = 5f;

            if (_menuOpen)
            {
                _globalAlpha += dt * speed;
                _contentOffsetY -= dt * 40f;

                if (_globalAlpha >= _oldAlpha)
                {
                    _globalAlpha = _oldAlpha;
                    _contentOffsetY = 0f;
                    _isAnimating = false;
                }
            }
            else
            {
                _globalAlpha -= dt * speed;
                _contentOffsetY += dt * 40f;

                if (_globalAlpha <= 0f)
                {
                    _globalAlpha = 0f;
                    _contentOffsetY = 60f;
                    _isAnimating = false;
                }
            }

            _globalAlpha = Math.Clamp(_globalAlpha, 0f, _oldAlpha);
            _contentOffsetY = Math.Clamp(_contentOffsetY, 0f, 60f);
        }

        var combinedAlpha = _globalAlpha * TransitionAlpha;
        ImGui.PushStyleVar(
            ImGuiStyleVar.Alpha,
            ImGui.GetStyle().Alpha * _globalAlpha
        );
        _windowManager.CurrentWindow?.Style.BackgroundOpacity = _globalAlpha;

        var windowWidth = ImGui.GetWindowWidth();

        ImGui.SetCursorPosY(
            ImGui.GetCursorPosY() + _contentOffsetY
        );

        DrawHeader(windowWidth, combinedAlpha);

        _root?.Update();
        _root?.Render();

        ImGui.PopStyleVar();
    }

    void DrawHeader(float windowWidth, float alpha)
    {
        // pet on upper left corner 
        var dw = ImGui.GetForegroundDrawList();
        var windowPos = ImGui.GetWindowPos();

        // image size
        const float petWidth = 100f;
        const float petHeight = petWidth * (307f / 307f);

        // image position
        var petPos = windowPos + new Vector2(0, -87f);

        // applies alpha animation
        var petColor = ImGui.ColorConvertFloat4ToU32(
            new Vector4(1f, 1f, 1f, alpha)
        );

        dw.AddImage(
            _assets.TryGetImage("pet", out var miniJane) ? miniJane : IntPtr.Zero,
            petPos,
            petPos + new Vector2(petWidth, petHeight),
            new Vector2(0f, 0f),
            new Vector2(1f, 1f),
            petColor
        );

        // header
        var headerHeight = 40f;

        ImGui.SetCursorPos(
            new Vector2(12, (headerHeight - 28) / 2)
        );

        if (ImGui.Button(Icons.ArrowLeft))
        {
            _windowManager.GoBack();
        }

        var titleText = "Menu";
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

        ImGui.SetCursorPos(new Vector2(0, headerHeight));
        ImGui.Separator();
    }

    private void InitializeUI()
    {
        _root = new VBox("menu_root")
        {
            Spacing = 12f,
            Padding = new Vector2(10, 10)
        };

        _statusLabel = new UIText("Status: Ready", "menu_status")
        {
            TextColor = Colors.Green
        };

        _root.AddElement(new UIText("NesiUI Components", "menu_title")
        {
            TextColor = Colors.accentColor
        });

        _root.AddElement(new UIText("A showcase of NesiUI components and layouts.", "menu_subtitle")
        {
            TextColor = Colors.DarkGray
        });

        var layout = new SplitView("MainLayout")
        {
            SplitterSize = 20f,
            FirstSize = _windowSize.X * 0.2f,
            MinFirstSize = 120f,
            MinSecondSize = 200f
        };

        var sidebar = new VBox("Sidebar")
        {
            Spacing = 8f
        };

        var content = new VBox("Content")
        {
            Spacing = 12f
        };

        layout.AddElement(sidebar);
        layout.AddElement(content);

        // INPUTS
        var inputsView = new VBox("view_inputs")
        {
            Spacing = 12f
        };

        var buttonsPanel = new UIGroupBox("Buttons", "inputs_buttons")
        {
            GroupSize = new Vector2(0, 175f)
        };

        var clickButton = new UIButton("Click me", "input_click");
        clickButton.OnClick += () =>
        {
            _statusLabel.Text = "Button clicked!";
            _statusLabel.TextColor = Colors.Green;
        };

        buttonsPanel.AddElement(clickButton);

        var primaryButton = new UIButton("Primary Action", "input_primary");
        primaryButton.OnClick += () =>
        {
            _statusLabel.Text = "Primary action triggered.";
            _statusLabel.TextColor = Colors.Green;
        };

        buttonsPanel.AddElement(primaryButton);

        var secondaryButton = new UIButton("Secondary Action", "input_secondary");
        secondaryButton.OnClick += () =>
        {
            _statusLabel.Text = "Secondary action triggered.";
            _statusLabel.TextColor = Colors.accentColor;
        };

        buttonsPanel.AddElement(secondaryButton);

        inputsView.AddElement(buttonsPanel);

        var togglesPanel = new UIGroupBox("Checkboxes", "inputs_checkboxes")
        {
            GroupSize = new Vector2(0, 92f)
        };

        var checkboxEsp = new UICheckbox("Enable ESP", Backhud.bEsp, "input_esp");
        checkboxEsp.OnValueChanged += value =>
        {
            Backhud.bEsp = value;
            _statusLabel.Text = value ? "ESP enabled." : "ESP disabled.";
            _statusLabel.TextColor = value ? Colors.Green : Colors.DarkGray;
        };

        togglesPanel.AddElement(checkboxEsp);
        inputsView.AddElement(togglesPanel);

        var slidersPanel = new UIGroupBox("Sliders", "inputs_sliders");

        var opacitySlider = new UISlider("Opacity", 0f, 100f, 75f, "input_opacity");
        opacitySlider.OnValueChanged += value =>
        {
            _statusLabel.Text = $"Opacity: {(int)value}%";
            _statusLabel.TextColor = Colors.accentColor;
        };

        slidersPanel.AddElement(opacitySlider);
        inputsView.AddElement(slidersPanel);

        // LAYOUT
        var layoutView = new VBox("view_layout")
        {
            Spacing = 12f
        };

        // HBox 
        var hboxPanel = new UIGroupBox("HBox", "layout_hbox")
        {
            GroupSize = new Vector2(0, 95)
        };

        var hbox = new HBox("layout_hbox_row")
        {
            Spacing = 10f
        };

        hbox.AddElement(new UIText("Horizontal:", "layout_hbox_label")
        {
            TextColor = Colors.accentColor
        });

        hbox.AddElement(new UIButton("Left", "layout_left"));
        hbox.AddElement(new UIButton("Center", "layout_center"));
        hbox.AddElement(new UIButton("Right", "layout_right"));

        hboxPanel.AddElement(hbox);
        layoutView.AddElement(hboxPanel);

        // Two columns 
        var columnsPanel = new UIGroupBox("Two Columns", "layout_columns")
        {
            GroupSize = new Vector2(0, 205)
        };

        var columns = new HBox("layout_columns_row")
        {
            Spacing = 12f
        };

        var leftGroup = new UIGroupBox("Left Group", "layout_left_group")
        {
            GroupSize = new Vector2(250, 140)
        };

        leftGroup.AddElement(new UIText("Left content", "layout_left_text"));
        leftGroup.AddElement(new UIButton("Left Button", "layout_left_button"));

        var rightGroup = new UIGroupBox("Right Group", "layout_right_group")
        {
            GroupSize = new Vector2(250, 140)
        };

        rightGroup.AddElement(new UIText("Right content", "layout_right_text"));
        rightGroup.AddElement(new UIButton("Right Button", "layout_right_button"));

        columns.AddElement(leftGroup);
        columns.AddElement(rightGroup);

        columnsPanel.AddElement(columns);
        layoutView.AddElement(columnsPanel);

        // Nested layout
        var nestedPanel = new UIGroupBox("Nested Layout", "layout_nested")
        {
            GroupSize = new Vector2(0, 255)
        };

        var nestedRoot = new VBox("layout_nested_root")
        {
            Spacing = 10f
        };

        var nestedTop = new HBox("layout_nested_top")
        {
            Spacing = 10f
        };

        var nestedLeft = new UIGroupBox("Top Left", "layout_nested_left")
        {
            GroupSize = new Vector2(250, 90)
        };

        nestedLeft.AddElement(new UIText("Nested content", "nested_left_text"));

        var nestedRight = new UIGroupBox("Top Right", "layout_nested_right")
        {
            GroupSize = new Vector2(250, 90)
        };

        nestedRight.AddElement(new UIText("Nested content", "nested_right_text"));

        nestedTop.AddElement(nestedLeft);
        nestedTop.AddElement(nestedRight);

        var nestedBottom = new UIGroupBox("Bottom", "layout_nested_bottom")
        {
            GroupSize = new Vector2(0, 90)
        };

        nestedBottom.AddElement(new UIButton("Bottom Action", "nested_bottom_button"));

        nestedRoot.AddElement(nestedTop);
        nestedRoot.AddElement(nestedBottom);

        nestedPanel.AddElement(nestedRoot);
        layoutView.AddElement(nestedPanel);

        // SplitView
        var splitPanel = new UIGroupBox("SplitView", "layout_split")
        {
            GroupSize = new Vector2(0, 220)
        };

        var split = new SplitView("layout_split_view")
        {
            SplitterSize = 20f,
            FirstSize = 130f,
        };

        var splitSidebar = new VBox("layout_split_sidebar")
        {
            Spacing = 4f
        };

        splitSidebar.AddElement(new UIText("Sidebar", "split_sidebar_title")
        {
            TextColor = Colors.accentColor
        });

        var splitDashboardButton = new UIButton("Dashboard", "split_dashboard")
        {
            Style = UIButtonStyle.Sidebar
        };

        var splitSettingsButton = new UIButton("Dashboard", "split_settings")
        {
            Style = UIButtonStyle.Sidebar
        };

        var splitAboutButton = new UIButton("About", "split_about") { Style = UIButtonStyle.Sidebar };

        splitSidebar.AddElement(splitDashboardButton);
        splitSidebar.AddElement(splitSettingsButton);
        splitSidebar.AddElement(splitAboutButton);

        var splitContent = new VBox("layout_split_content") { Spacing = 6f };

        splitContent.AddElement(new UIText("Content Area", "split_content_title")
        {
            TextColor = Colors.accentColor
        });

        splitContent.AddElement(new UIText(
            "Content area on SplitView.",
            "split_content_text"
        ));

        split.AddElement(splitSidebar);
        split.AddElement(splitContent);

        splitPanel.AddElement(split);
        layoutView.AddElement(splitPanel);

        // ICONS
        var iconsView = new VBox("view_icons")
        {
            Spacing = 12f
        };

        var iconsPanel = new UIGroupBox("Icon Gallery", "icons_gallery")
        {
            GroupSize = new Vector2(0, 115)
        };

        var iconsRow1 = new HBox("icons_row_1")
        {
            Spacing = 12f
        };

        iconsRow1.AddElement(new UIText(Icons.Home, "icon_home"));
        iconsRow1.AddElement(new UIText(Icons.Gear, "icon_gear"));
        iconsRow1.AddElement(new UIText(Icons.User, "icon_user"));
        iconsRow1.AddElement(new UIText(Icons.Users, "icon_users"));
        iconsRow1.AddElement(new UIText(Icons.Folder, "icon_folder"));
        iconsRow1.AddElement(new UIText(Icons.Terminal, "icon_terminal"));
        iconsRow1.AddElement(new UIText(Icons.Plus, "icon_plus"));
        iconsRow1.AddElement(new UIText(Icons.Pencil, "icon_pencil"));
        iconsRow1.AddElement(new UIText(Icons.Trash, "icon_trash"));
        iconsRow1.AddElement(new UIText(Icons.Shield, "icon_shield"));
        iconsRow1.AddElement(new UIText(Icons.Eye, "icon_eye"));
        iconsRow1.AddElement(new UIText(Icons.Crosshair, "icon_crosshair"));

        var iconsRow2 = new HBox("icons_row_2")
        {
            Spacing = 12f
        };

        iconsRow2.AddElement(new UIText(Icons.Fire, "icon_fire"));
        iconsRow2.AddElement(new UIText(Icons.Thunderbolt, "icon_thunderbolt"));
        iconsRow2.AddElement(new UIText(Icons.Code, "icon_code"));
        iconsRow2.AddElement(new UIText(Icons.Download, "icon_download"));
        iconsRow2.AddElement(new UIText(Icons.Save, "icon_save"));
        iconsRow2.AddElement(new UIText(Icons.Wifi, "icon_wifi"));
        iconsRow2.AddElement(new UIText(Icons.Info, "icon_info"));
        iconsRow2.AddElement(new UIText(Icons.Bell, "icon_bell"));
        iconsRow2.AddElement(new UIText(Icons.Clock, "icon_clock"));
        iconsRow2.AddElement(new UIText(Icons.Play, "icon_play"));
        iconsRow2.AddElement(new UIText(Icons.Pause, "icon_pause"));
        iconsRow2.AddElement(new UIText(Icons.Stop, "icon_stop"));
        iconsRow2.AddElement(new UIText(Icons.X, "icon_x"));
        iconsRow2.AddElement(new UIText(Icons.Minus, "icon_minus"));
        iconsRow2.AddElement(new UIText(Icons.ArrowLeft, "icon_arrow_left"));

        iconsPanel.AddElement(iconsRow1);
        iconsPanel.AddElement(iconsRow2);
        iconsView.AddElement(iconsPanel);

        // NAVIGATION
        void ShowView(IUIElement view)
        {
            content.ClearElements();
            content.AddElement(view);
        }

        var inputsButton = new UIButton(Icons.Game + " Inputs", "nav_inputs")
        {
            Style = UIButtonStyle.Sidebar
        };
        inputsButton.OnClick += () => ShowView(inputsView);

        var layoutButton = new UIButton(Icons.Gear + " Layout", "nav_layout")
        {
            Style = UIButtonStyle.Sidebar
        };
        layoutButton.OnClick += () => ShowView(layoutView);

        var iconsButton = new UIButton(Icons.Thunderbolt + " Icons", "nav_icons")
        {
            Style = UIButtonStyle.Sidebar
        };
        iconsButton.OnClick += () => ShowView(iconsView);

        sidebar.AddElement(inputsButton);
        sidebar.AddElement(layoutButton);
        sidebar.AddElement(iconsButton);

        // INITIAL VIEW
        ShowView(inputsView);
        _root.AddElement(layout);
    }

}
