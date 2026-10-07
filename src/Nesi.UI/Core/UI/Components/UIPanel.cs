using System.Numerics;
using ImGuiNET;
using Nesi.UI.Style;

namespace Nesi.UI.Components;

/// <summary>A container that draws a frame background and border around its children.</summary>
public class UIPanel : ContainerElement
{
    private Vector4 _backgroundColor = Colors.RGBA(40, 40, 45, 200);
    private Vector4 _borderColor = Colors.RGBA(60, 60, 65, 255);
    private float _borderSize = 1f;
    private float _rounding = 4f;
    private Vector2 _panelPadding = new Vector2(8, 8);

    /// <summary>Gets or sets the frame background color.</summary>
    public Vector4 BackgroundColor
    {
        get => _backgroundColor;
        set => _backgroundColor = value;
    }

    /// <summary>Gets or sets the border color.</summary>
    public Vector4 BorderColor
    {
        get => _borderColor;
        set => _borderColor = value;
    }

    /// <summary>Gets or sets the border thickness.</summary>
    public float BorderSize
    {
        get => _borderSize;
        set => _borderSize = Math.Max(0f, value);
    }

    /// <summary>Gets or sets the frame corner rounding.</summary>
    public float Rounding
    {
        get => _rounding;
        set => _rounding = Math.Max(0f, value);
    }

    /// <summary>Gets or sets the frame padding.</summary>
    public Vector2 PanelPadding
    {
        get => _panelPadding;
        set => _panelPadding = value;
    }

    /// <summary>Creates a panel.</summary>
    /// <param name="id">Optional identifier.</param>
    public UIPanel(string? id = null) : base(id)
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
            // Cria um container visual usando Spacing e styling
            ImGui.PushStyleColor(ImGuiCol.FrameBg, _backgroundColor);
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, _panelPadding);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, _rounding);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, _borderSize);
            ImGui.PushStyleColor(ImGuiCol.Border, _borderColor);

            // Renderiza usando Group para manter elementos juntos
            ImGui.BeginGroup();

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(6, 4));

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Render();
            }

            ImGui.PopStyleVar(); // ItemSpacing
            ImGui.EndGroup();

            ImGui.PopStyleColor(2); // FrameBg, Border
            ImGui.PopStyleVar(3); // FramePadding, FrameRounding, FrameBorderSize
        }
        finally
        {
            RestoreMargin();
            ImGui.PopID();
        }
    }
}
