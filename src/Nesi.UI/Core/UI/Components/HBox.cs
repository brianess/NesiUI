using System.Numerics;
using ImGuiNET;

namespace Nesi.UI.Components;

/// <summary>Arranges child elements horizontally on one line.</summary>
public class HBox : ContainerElement
{
    private float _spacing = 8f;
    private bool _expandHeight = true;

    /// <summary>Gets or sets the horizontal spacing between child items.</summary>
    public float Spacing
    {
        get => _spacing;
        set => _spacing = value;
    }

    /// <summary>
    /// Gets or sets the height-expansion preference. The current renderer does not
    /// apply separate behavior for this property.
    /// </summary>
    public bool ExpandHeight
    {
        get => _expandHeight;
        set => _expandHeight = value;
    }

    /// <summary>Creates a horizontal layout.</summary>
    /// <param name="id">Optional identifier.</param>
    public HBox(string? id = null) : base(id)
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
            ImGui.BeginGroup();

            for (int i = 0; i < _children.Count; i++)
            {
                if (i > 0)
                {
                    ImGui.SameLine(0f, _spacing);
                }

                _children[i].Render();
            }

            ImGui.EndGroup();
        }
        finally
        {
            RestoreMargin();
            ImGui.PopID();
        }
    }
}
