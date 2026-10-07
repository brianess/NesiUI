using System.Numerics;
using ImGuiNET;

namespace Nesi.UI.Components;

/// <summary>Arranges child elements vertically.</summary>
public class VBox : ContainerElement
{
    private float _spacing = 8f;
    private bool _expandWidth = true;

    /// <summary>Gets or sets the vertical spacing between items.</summary>
    public float Spacing
    {
        get => _spacing;
        set => _spacing = value;
    }

    /// <summary>
    /// Gets or sets the width-expansion preference. The current renderer does not
    /// apply separate behavior for this property.
    /// </summary>
    public bool ExpandWidth
    {
        get => _expandWidth;
        set => _expandWidth = value;
    }

    /// <summary>Creates a vertical layout.</summary>
    /// <param name="id">Optional identifier.</param>
    public VBox(string? id = null) : base(id)
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

            float originalSpacing = ImGui.GetStyle().ItemSpacing.Y;
            ImGui.GetStyle().ItemSpacing.Y = _spacing;

            // Renderiza cada filho verticalmente
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Render();

                // Não adiciona espaço após o último elemento
                if (i < _children.Count - 1 && _spacing > 0)
                {
                    // ImGui adiciona automático o ItemSpacing
                }
            }

            ImGui.GetStyle().ItemSpacing.Y = originalSpacing;

            ImGui.EndGroup();
        }
        finally
        {
            RestoreMargin();
            ImGui.PopID();
        }
    }
}
