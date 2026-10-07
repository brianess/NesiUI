using System.Numerics;
using ImGuiNET;
using Nesi.UI.Style;

namespace Nesi.UI.Components;

/// <summary>A titled child region that groups related elements.</summary>
public class UIGroupBox : ContainerElement
{
    private string _title;

    private Vector2 _groupSize = Vector2.Zero;

    private float _rounding = 4f;
    private float _borderSize = 0.1f;

    private new float _padding = 10f;

    private float _headerHeight = 30f;
    private float _headerPadding = 12f;

    private float _headerShadowHeight = 8f;

    /// <summary>Gets or sets the title displayed in the header.</summary>
    public string Title
    {
        get => _title;
        set => _title = value;
    }

    /// <summary>Gets or sets the child-region size; zero dimensions use ImGui sizing.</summary>
    public Vector2 GroupSize
    {
        get => _groupSize;
        set => _groupSize = value;
    }

    /// <summary>Gets or sets the child-region corner rounding.</summary>
    public float Rounding
    {
        get => _rounding;
        set => _rounding = Math.Max(0f, value);
    }

    /// <summary>Gets or sets the border thickness used by the group visuals.</summary>
    public float BorderSize
    {
        get => _borderSize;
        set => _borderSize = Math.Max(0f, value);
    }

    /// <summary>Gets or sets the padding around the child content.</summary>
    public new float Padding
    {
        get => _padding;
        set => _padding = Math.Max(0f, value);
    }

    /// <summary>Gets or sets the header height.</summary>
    public float HeaderHeight
    {
        get => _headerHeight;
        set => _headerHeight = Math.Max(0f, value);
    }

    /// <summary>Gets or sets the horizontal inset for the header title.</summary>
    public float HeaderPadding
    {
        get => _headerPadding;
        set => _headerPadding = Math.Max(0f, value);
    }

    /// <summary>Gets or sets the header shadow height.</summary>
    public float HeaderShadowHeight
    {
        get => _headerShadowHeight;
        set => _headerShadowHeight = Math.Max(0f, value);
    }

    /// <summary>Creates a titled group box.</summary>
    /// <param name="title">The header title.</param>
    /// <param name="id">Optional identifier.</param>
    public UIGroupBox(string title, string? id = null)
        : base(id)
    {
        _title = title;
    }

    /// <inheritdoc />
    public override void Render()
    {
        if (!IsVisible)
            return;

        ImGui.PushID(_id);
        ApplyMargin();

        try
        {
            var style = ImGui.GetStyle();
            // header
            var titleColor = Colors.GetAlpha(style.Colors[(int)ImGuiCol.Text], style.Alpha);
            var headerColor = Colors.GetAlpha(style.Colors[(int)ImGuiCol.FrameBg], style.Alpha);
            // groupbox
            var bgColor = Colors.GetAlpha(headerColor, 0.5f);
            var borderColor = Colors.GetAlpha(Colors.text, 0.05f);

            ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, _rounding);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

            ImGui.BeginChild(_id, _groupSize);

            Background.Render(ImGui.GetWindowDrawList(), ImGui.GetWindowPos(), ImGui.GetWindowSize(), 6f, 0.5f, BackgroundType.Glass);
            DrawContent();
            DrawHeader(headerColor, titleColor);

            ImGui.EndChild();

            ImGui.PopStyleVar(2);
        }
        finally
        {
            RestoreMargin();
            ImGui.PopID();
        }
    }

    private void DrawHeader(Vector4 headerColor, Vector4 titleColor)
    {
        if (string.IsNullOrWhiteSpace(_title))
            return;

        var drawList = ImGui.GetWindowDrawList();
        var windowPos = ImGui.GetWindowPos();
        float width = ImGui.GetWindowSize().X;

        bool isDark = ThemeManager.GetCurrentTheme() == ETheme.Dark;

        var headerMin = new Vector2(
            windowPos.X,
            windowPos.Y - 1f
        );

        var headerMax = new Vector2(
            windowPos.X + width,
            windowPos.Y + _headerHeight
        );

        // Subtle vertical gradient
        float topMultiplier = isDark ? 1.12f : 0.94f;
        float bottomMultiplier = isDark ? 0.82f : 0.88f;

        var topColor = new Vector4(
            Math.Min(headerColor.X * topMultiplier, 1f),
            Math.Min(headerColor.Y * topMultiplier, 1f),
            Math.Min(headerColor.Z * topMultiplier, 1f),
            headerColor.W
        );

        var bottomColor = new Vector4(
            headerColor.X * bottomMultiplier,
            headerColor.Y * bottomMultiplier,
            headerColor.Z * bottomMultiplier,
            headerColor.W
        );

        drawList.AddRectFilledMultiColor(
            headerMin,
            headerMax,
            ImGui.ColorConvertFloat4ToU32(topColor),
            ImGui.ColorConvertFloat4ToU32(topColor),
            ImGui.ColorConvertFloat4ToU32(bottomColor),
            ImGui.ColorConvertFloat4ToU32(bottomColor)
        );

        // Top highlight
        var highlightColor = ImGui.ColorConvertFloat4ToU32(
            isDark
                ? new Vector4(1f, 1f, 1f, 0.06f)
                : new Vector4(1f, 1f, 1f, 0.35f)
        );

        drawList.AddLine(
            new Vector2(headerMin.X + _rounding, headerMin.Y),
            new Vector2(headerMax.X - _rounding, headerMin.Y),
            highlightColor,
            1f
        );

        // Bottom shadow
        var shadowColor = ImGui.ColorConvertFloat4ToU32(
            isDark
                ? new Vector4(0f, 0f, 0f, 0.30f)
                : new Vector4(0f, 0f, 0f, 0.18f)
        );

        drawList.AddLine(
            new Vector2(headerMin.X, headerMax.Y - 1f),
            new Vector2(headerMax.X, headerMax.Y - 1f),
            shadowColor,
            1f
        );

        // Title
        var textSize = ImGui.CalcTextSize(_title);

        var textPos = new Vector2(
            windowPos.X + _headerPadding,
            windowPos.Y + (_headerHeight - textSize.Y) * 0.5f
        );

        drawList.AddText(
            textPos,
            ImGui.ColorConvertFloat4ToU32(titleColor),
            _title
        );
    }

    private void DrawHeaderShadow(Vector2 windowPos, float width)
    {
        if (_headerShadowHeight <= 0f)
            return;

        var drawList = ImGui.GetWindowDrawList();

        float startY =
            windowPos.Y + _headerHeight;

        float endY =
            startY + _headerShadowHeight;

        var min = new Vector2(
            windowPos.X - _borderSize - 1f,
            startY
        );

        var max = new Vector2(
            windowPos.X +
            width +
            _borderSize +
            1f,
            endY
        );

        uint topColor =
            ImGui.ColorConvertFloat4ToU32(
                new Vector4(0f, 0f, 0f, 0.30f * ImGui.GetStyle().Alpha)
            );

        uint bottomColor =
            ImGui.ColorConvertFloat4ToU32(
                new Vector4(0f, 0f, 0f, 0f)
            );

        drawList.AddRectFilledMultiColor(
            min,
            max,
            topColor,
            topColor,
            bottomColor,
            bottomColor
        );
    }

    private void DrawContent()
    {
        float width = ImGui.GetWindowSize().X;
        float height = ImGui.GetWindowSize().Y;

        float contentX = _padding;
        float contentY = _headerHeight;

        float contentWidth = width - (_padding * 2f);
        float contentHeight = height - _headerHeight;

        if (contentWidth <= 0f || contentHeight <= 0f)
            return;

        ImGui.SetCursorPos(new Vector2(contentX, contentY));

        ImGui.BeginChild($"{_id}_content", new Vector2(contentWidth, contentHeight), ImGuiChildFlags.None);

        ImGui.Spacing();

        foreach (var child in _children)
            child.Render();

        ImGui.Spacing();

        ImGui.EndChild();

        DrawHeaderShadow(
            ImGui.GetWindowPos(),
            width
        );
    }
}