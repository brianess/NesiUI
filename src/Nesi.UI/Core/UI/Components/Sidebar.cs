using System.Numerics;
using ImGuiNET;
using Nesi.UI.Style;

namespace Nesi.UI.Components;

/// <summary>A fixed-width sidebar that draws a background behind its children.</summary>
public class Sidebar : ContainerElement
{
    private float _width = 200f;
    private float _itemHeight = 32f;
    private Vector4 _backgroundColor = Colors.RGBA(30, 30, 30, 255);
    private Vector4 _hoverColor = Colors.RGBA(50, 50, 60, 255);

    /// <summary>Gets or sets the sidebar background width.</summary>
    public float Width
    {
        get => _width;
        set => _width = Math.Max(50f, value);
    }

    /// <summary>Gets or sets the child frame-padding height used for sidebar items.</summary>
    public float ItemHeight
    {
        get => _itemHeight;
        set => _itemHeight = Math.Max(20f, value);
    }

    /// <summary>Gets or sets the sidebar background color.</summary>
    public Vector4 BackgroundColor
    {
        get => _backgroundColor;
        set => _backgroundColor = value;
    }

    /// <summary>Gets or sets the hover color exposed for sidebar styling.</summary>
    public Vector4 HoverColor
    {
        get => _hoverColor;
        set => _hoverColor = value;
    }

    /// <summary>Creates a sidebar.</summary>
    /// <param name="id">Optional identifier.</param>
    public Sidebar(string? id = null) : base(id)
    {
    }

    /// <inheritdoc />
    public override void Render()
    {
        if (!IsVisible) return;

        ImGui.PushID(_id);
        ApplyMargin();

        try
        {
            // Cria uma área com background customizado
            var drawList = ImGui.GetWindowDrawList();
            var cursorPos = ImGui.GetCursorScreenPos();
            var windowSize = ImGui.GetWindowSize();

            // Background da sidebar
            drawList.AddRectFilled(
                cursorPos,
                cursorPos + new Vector2(_width, windowSize.Y),
                ImGui.GetColorU32(_backgroundColor)
            );

            ImGui.BeginGroup();

            PushDisabledState();

            // Renderiza elementos dentro da sidebar
            var originalItemHeight = ImGui.GetTextLineHeightWithSpacing();
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(8, (_itemHeight - ImGui.GetTextLineHeight()) / 2));

            foreach (var child in _children)
            {
                child.Render();
            }

            ImGui.PopStyleVar();

            PopDisabledState();

            ImGui.EndGroup();
        }
        finally
        {
            RestoreMargin();
            ImGui.PopID();
        }
    }
}
