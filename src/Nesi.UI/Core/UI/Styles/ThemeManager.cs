// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Numerics;
using Nesi.UI.Utils;
using ImGuiNET;

namespace Nesi.UI.Style;

/// <summary>
/// Applies the shared dark/light theme to the current ImGui context.
/// Theme state is static and shared across application instances.
/// </summary>
public static class ThemeManager
{
    private static ETheme _currentTheme = ETheme.Dark;

    private static readonly Dictionary<ETheme, ThemeColors> _themes = new()
    {
        [ETheme.Dark] = ThemeColors.Dark,
        [ETheme.Light] = ThemeColors.Light
    };

    private static readonly ThemeStyle _style = ThemeStyle.Default;

    /// <summary>Gets the currently selected theme.</summary>
    public static ETheme GetCurrentTheme() => _currentTheme;

    /// <summary>Selects and immediately applies a theme.</summary>
    /// <param name="theme">Theme to apply.</param>
    public static void SetTheme(ETheme theme)
    {
        if (_currentTheme == theme)
            return;

        _currentTheme = theme;
        ApplyTheme(theme);
    }

    /// <summary>Applies the specified built-in theme to the current ImGui context.</summary>
    /// <param name="theme">Theme to apply.</param>
    public static void ApplyTheme(ETheme theme)
    {
        if (!_themes.TryGetValue(theme, out var colors))
            return;

        ApplyColors(colors);
        ApplyStyle(_style);
    }

    /// <summary>Applies the currently selected theme to the current ImGui context.</summary>
    public static void ApplyCurrentTheme()
    {
        ApplyTheme(_currentTheme);
    }

    // animation in theme transition
    private static ETheme _targetTheme;
    private static float _themeAnimationSpeed = 4f;
    private static bool _isThemeAnimating;

    /// <summary>Starts an animated transition to a built-in theme.</summary>
    /// <param name="theme">Target theme.</param>
    /// <param name="speed">Interpolation speed.</param>
    public static void ApplyThemeWithAnimation(
        ETheme theme,
        float speed = 4f)
    {
        if (!_themes.ContainsKey(theme))
            return;

        _targetTheme = theme;
        _themeAnimationSpeed = speed;
        _isThemeAnimating = true;
    }

    /// <summary>Advances and applies a pending animated theme transition.</summary>
    public static void Update()
    {
        if (!_isThemeAnimating)
            return;

        if (!_themes.TryGetValue(_targetTheme, out var targetColors))
            return;

        var animatedColors = new ThemeColors(
            Animation.AnimateColor("theme_accent", targetColors.Accent, _themeAnimationSpeed),
            Animation.AnimateColor("theme_bg", targetColors.Background, _themeAnimationSpeed),
            Animation.AnimateColor("theme_title", targetColors.TitleBar, _themeAnimationSpeed),
            Animation.AnimateColor("theme_title_active", targetColors.TitleBarActive, _themeAnimationSpeed),
            Animation.AnimateColor("theme_button", targetColors.Button, _themeAnimationSpeed),
            Animation.AnimateColor("theme_button_hover", targetColors.ButtonHover, _themeAnimationSpeed),
            Animation.AnimateColor("theme_button_active", targetColors.ButtonActive, _themeAnimationSpeed),
            Animation.AnimateColor("theme_text", targetColors.Text, _themeAnimationSpeed),
            Animation.AnimateColor("theme_text_disabled", targetColors.TextDisabled, _themeAnimationSpeed),
            Animation.AnimateColor("theme_frame_bg", targetColors.FrameBackground, _themeAnimationSpeed),
            Animation.AnimateColor("theme_frame_hover", targetColors.FrameBackgroundHover, _themeAnimationSpeed),
            Animation.AnimateColor("theme_header", targetColors.Header, _themeAnimationSpeed),
            Animation.AnimateColor("theme_header_hover", targetColors.HeaderHover, _themeAnimationSpeed),
            Animation.AnimateColor("theme_separator", targetColors.Separator, _themeAnimationSpeed)
        );

        ApplyColors(animatedColors);
        ApplyStyle(_style);

        _currentTheme = _targetTheme;
    }

    /// <summary>Applies the configured accent color to selected ImGui style entries.</summary>
    public static void ApplyAccentColor()
    {
        var style = ImGui.GetStyle();
        var accentAlpha = Colors.GetAlpha(Colors.accentColor, 0.5f);

        style.Colors[(int)ImGuiCol.CheckMark] = Colors.accentColor;

        style.Colors[(int)ImGuiCol.SliderGrab] = Colors.accentColor;
        style.Colors[(int)ImGuiCol.SliderGrabActive] = Colors.accentColor;

        style.Colors[(int)ImGuiCol.FrameBgActive] = accentAlpha;

        style.Colors[(int)ImGuiCol.Tab] = Colors.accentColor;
        style.Colors[(int)ImGuiCol.TabHovered] = Colors.accentColor;
        style.Colors[(int)ImGuiCol.TabDimmed] = Colors.accentColor;

        style.Colors[(int)ImGuiCol.HeaderActive] = accentAlpha;
    }

    private static void ApplyColors(ThemeColors colors)
    {
        var style = ImGui.GetStyle();

        Colors.backgroundColor = colors.Background;
        Colors.text = colors.Text;

        style.Colors[(int)ImGuiCol.ScrollbarGrab] =
            colors.Accent;

        style.Colors[(int)ImGuiCol.WindowBg] =
            colors.Background;

        style.Colors[(int)ImGuiCol.TitleBg] =
            colors.TitleBar;

        style.Colors[(int)ImGuiCol.TitleBgActive] =
            colors.TitleBarActive;

        style.Colors[(int)ImGuiCol.Button] =
            colors.Button;

        style.Colors[(int)ImGuiCol.ButtonHovered] =
            colors.ButtonHover;

        style.Colors[(int)ImGuiCol.ButtonActive] =
            colors.ButtonActive;

        style.Colors[(int)ImGuiCol.Text] = colors.Text;

        style.Colors[(int)ImGuiCol.TextDisabled] = colors.TextDisabled;

        style.Colors[(int)ImGuiCol.FrameBg] =
            colors.FrameBackground;

        style.Colors[(int)ImGuiCol.FrameBgHovered] =
            colors.FrameBackgroundHover;

        style.Colors[(int)ImGuiCol.Header] =
            colors.Header;

        style.Colors[(int)ImGuiCol.HeaderHovered] =
            colors.HeaderHover;

        style.Colors[(int)ImGuiCol.Separator] =
            colors.Separator;

        style.Colors[(int)ImGuiCol.ResizeGrip] =
            Colors.Zero;
        style.Colors[(int)ImGuiCol.ResizeGripActive] =
            Colors.Zero;
        style.Colors[(int)ImGuiCol.ResizeGripHovered] =
            Colors.Zero;
    }

    private static void ApplyStyle(ThemeStyle theme)
    {
        var style = ImGui.GetStyle();

        style.Alpha = theme.Alpha;
        style.DisabledAlpha = theme.DisabledAlpha;

        style.WindowPadding = theme.WindowPadding;
        style.WindowRounding = theme.WindowRounding;
        style.WindowBorderSize = theme.WindowBorderSize;
        style.WindowMinSize = theme.WindowMinSize;
        style.WindowTitleAlign = theme.WindowTitleAlign;

        style.ChildRounding = theme.ChildRounding;
        style.ChildBorderSize = theme.ChildBorderSize;

        style.PopupRounding = theme.PopupRounding;
        style.PopupBorderSize = theme.PopupBorderSize;

        style.FramePadding = theme.FramePadding;
        style.FrameRounding = theme.FrameRounding;
        style.FrameBorderSize = theme.FrameBorderSize;

        style.ItemSpacing = theme.ItemSpacing;
        style.ItemInnerSpacing = theme.ItemInnerSpacing;

        style.CellPadding = theme.CellPadding;

        style.TouchExtraPadding = theme.TouchExtraPadding;

        style.IndentSpacing = theme.IndentSpacing;

        style.ColumnsMinSpacing = theme.ColumnsMinSpacing;

        style.ScrollbarSize = theme.ScrollbarSize;
        style.ScrollbarRounding = theme.ScrollbarRounding;

        style.GrabMinSize = theme.GrabMinSize;
        style.GrabRounding = theme.GrabRounding;

        style.TabRounding = theme.TabRounding;
        style.TabBorderSize = theme.TabBorderSize;
        style.TabBarBorderSize = theme.TabBarBorderSize;
        style.TabBarOverlineSize = theme.TabBarOverlineSize;

        style.TableAngledHeadersAngle =
            theme.TableAngledHeadersAngle;

        style.TableAngledHeadersTextAlign =
            theme.TableAngledHeadersTextAlign;

        style.ColorButtonPosition =
            theme.ColorButtonPosition;

        style.ButtonTextAlign =
            theme.ButtonTextAlign;

        style.SelectableTextAlign =
            theme.SelectableTextAlign;

        style.SeparatorTextBorderSize =
            theme.SeparatorTextBorderSize;

        style.SeparatorTextAlign =
            theme.SeparatorTextAlign;

        style.SeparatorTextPadding =
            theme.SeparatorTextPadding;
    }
}

/// <summary>Built-in theme selection.</summary>
public enum ETheme
{
    /// <summary>Dark theme.</summary>
    Dark,
    /// <summary>Light theme.</summary>
    Light
}

/// <summary>Color values used by a Nesi.UI theme.</summary>
public record ThemeColors(
    Vector4 Accent,
    Vector4 Background,
    Vector4 TitleBar,
    Vector4 TitleBarActive,
    Vector4 Button,
    Vector4 ButtonHover,
    Vector4 ButtonActive,
    Vector4 Text,
    Vector4 TextDisabled,
    Vector4 FrameBackground,
    Vector4 FrameBackgroundHover,
    Vector4 Header,
    Vector4 HeaderHover,
    Vector4 Separator)
{
    public static ThemeColors Dark => new(
        Colors.RGBA(10, 132, 255),     // Accent
        Colors.RGBA(28, 28, 30),       // Background

        Colors.RGBA(44, 44, 46),       // TitleBar
        Colors.RGBA(58, 58, 60),       // TitleBarActive

        Colors.RGBA(50, 50, 54),       // Button
        Colors.RGBA(65, 65, 69),       // ButtonHover
        Colors.RGBA(74, 74, 78),       // ButtonActive

        Colors.RGBA(245, 245, 247),    // Text
        Colors.RGBA(142, 142, 147),    // TextDisabled

        Colors.RGBA(34, 34, 36),       // FrameBackground
        Colors.RGBA(58, 58, 60),       // FrameBackgroundHover

        Colors.RGBA(44, 44, 46),       // Header
        Colors.RGBA(58, 58, 60),       // HeaderHover

        Colors.RGBA(72, 72, 74)        // Separator
    );

    public static ThemeColors Light => new(
        Colors.RGBA(0, 122, 255),      // Accent
        Colors.RGBA(242, 242, 247),    // Background

        Colors.RGBA(255, 255, 255),    // TitleBar
        Colors.RGBA(242, 242, 247),    // TitleBarActive

        Colors.RGBA(255, 255, 255),    // Button
        Colors.RGBA(229, 229, 234),    // ButtonHover
        Colors.RGBA(209, 209, 214),    // ButtonActive

        Colors.RGBA(28, 28, 30),       // Text
        Colors.RGBA(142, 142, 147),    // TextDisabled

        Colors.RGBA(255, 255, 255),    // FrameBackground
        Colors.RGBA(229, 229, 234),    // FrameBackgroundHover

        Colors.RGBA(255, 255, 255),    // Header
        Colors.RGBA(229, 229, 234),    // HeaderHover

        Colors.RGBA(198, 198, 200)     // Separator
    );
}

/// <summary>ImGui style values used by the framework's default theme.</summary>
public record ThemeStyle(
    float Alpha,
    float DisabledAlpha,

    Vector2 WindowPadding,
    float WindowRounding,
    float WindowBorderSize,
    Vector2 WindowMinSize,
    Vector2 WindowTitleAlign,

    float ChildRounding,
    float ChildBorderSize,

    float PopupRounding,
    float PopupBorderSize,

    Vector2 FramePadding,
    float FrameRounding,
    float FrameBorderSize,

    Vector2 ItemSpacing,
    Vector2 ItemInnerSpacing,

    Vector2 CellPadding,

    Vector2 TouchExtraPadding,

    float IndentSpacing,

    float ColumnsMinSpacing,

    float ScrollbarSize,
    float ScrollbarRounding,

    float GrabMinSize,
    float GrabRounding,

    float TabRounding,
    float TabBorderSize,
    float TabBarBorderSize,
    float TabBarOverlineSize,

    float TableAngledHeadersAngle,
    Vector2 TableAngledHeadersTextAlign,

    ImGuiDir ColorButtonPosition,

    Vector2 ButtonTextAlign,
    Vector2 SelectableTextAlign,

    float SeparatorTextBorderSize,
    Vector2 SeparatorTextAlign,
    Vector2 SeparatorTextPadding)
{
    public static ThemeStyle Default => new(
        Alpha: 1.0f,
        DisabledAlpha: 0.55f,

        WindowPadding: new Vector2(10, 10),
        WindowRounding: 0.0f, // 6
        WindowBorderSize: 0.0f,
        WindowMinSize: new Vector2(32, 32),
        WindowTitleAlign: new Vector2(0.5f, 0.5f),

        ChildRounding: 4.0f,
        ChildBorderSize: 1.0f,

        PopupRounding: 5.0f,
        PopupBorderSize: 1.0f,

        FramePadding: new Vector2(8, 5),
        FrameRounding: 4.0f,
        FrameBorderSize: 0.0f,

        ItemSpacing: new Vector2(8, 6),
        ItemInnerSpacing: new Vector2(6, 4),

        CellPadding: new Vector2(6, 4),

        TouchExtraPadding: Vector2.Zero,

        IndentSpacing: 20.0f,

        ColumnsMinSpacing: 6.0f,

        ScrollbarSize: 12.0f,
        ScrollbarRounding: 6.0f,

        GrabMinSize: 10.0f,
        GrabRounding: 4.0f,

        TabRounding: 4.0f,
        TabBorderSize: 0.0f,
        TabBarBorderSize: 1.0f,
        TabBarOverlineSize: 2.0f,

        TableAngledHeadersAngle: 0.0f,
        TableAngledHeadersTextAlign: new Vector2(0.5f, 0.5f),

        ColorButtonPosition: ImGuiDir.Right,

        ButtonTextAlign: new Vector2(0.5f, 0.5f),
        SelectableTextAlign: new Vector2(0.0f, 0.0f),

        SeparatorTextBorderSize: 3.0f,
        SeparatorTextAlign: new Vector2(0.0f, 0.5f),
        SeparatorTextPadding: new Vector2(20, 3)
    );
}
