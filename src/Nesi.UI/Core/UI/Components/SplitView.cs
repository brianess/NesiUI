using System.Numerics;
using ImGuiNET;

namespace Nesi.UI.Components;

/// <summary>Arranges up to two child elements in side-by-side regions.</summary>
public class SplitView : ContainerElement
{
    private float _splitterSize = 0f;
    private float _firstSize = 220f;
    private float _minFirstSize = 120f;
    private float _minSecondSize = 200f;

    private bool _border = false;

    /// <summary>Gets or sets the requested width of the first (left) region.</summary>
    public float FirstSize
    {
        get => _firstSize;
        set => _firstSize = Math.Max(_minFirstSize, value);
    }

    /// <summary>Gets or sets the minimum width of the first region.</summary>
    public float MinFirstSize
    {
        get => _minFirstSize;
        set => _minFirstSize = Math.Max(0f, value);
    }

    /// <summary>Gets or sets the minimum width of the second region.</summary>
    public float MinSecondSize
    {
        get => _minSecondSize;
        set => _minSecondSize = Math.Max(0f, value);
    }

    /// <summary>Gets or sets the fixed gap between the two regions.</summary>
    public float SplitterSize
    {
        get => _splitterSize;
        set => _splitterSize = Math.Max(0f, value);
    }

    /// <summary>Gets or sets the child-region border option.</summary>
    public bool Border
    {
        get => _border;
        set => _border = value;
    }

    /// <summary>Creates a two-region layout.</summary>
    /// <param name="id">Optional identifier.</param>
    public SplitView(string? id = null) : base(id)
    {
    }

    /// <summary>
    /// Renders the first child on the left and the second child on the right.
    /// The gap is fixed and is not a resize handle.
    /// </summary>
    /// <inheritdoc />
    public override void Render()
    {
        if (!IsVisible) return;

        ImGui.PushID(_id);

        ApplyMargin();

        try
        {
            if (_children.Count == 0)
                return;

            Vector2 available = ImGui.GetContentRegionAvail();

            float firstWidth = Math.Clamp(
                _firstSize,
                _minFirstSize,
                Math.Max(
                    _minFirstSize,
                    available.X - _minSecondSize - _splitterSize
                )
            );

            float secondWidth = Math.Max(
                _minSecondSize,
                available.X - firstWidth - _splitterSize
            );

            // LEFT
            ImGui.BeginChild(
                "##First",
                new Vector2(firstWidth, available.Y)
            //_border
            );

            _children[0].Render();

            ImGui.EndChild();

            // SPLITTER
            if (_splitterSize > 0f)
            {
                ImGui.SameLine(0f, 0f);

                ImGui.Dummy(
                    new Vector2(_splitterSize, available.Y)
                );

                ImGui.SameLine(0f, 0f);
            }
            else
            {
                ImGui.SameLine(0f, 0f);
            }

            // RIGHT
            ImGui.BeginChild(
                "##Second",
                new Vector2(secondWidth, available.Y)
            //_border
            );

            if (_children.Count > 1)
                _children[1].Render();

            ImGui.EndChild();
        }
        finally
        {
            RestoreMargin();
            ImGui.PopID();
        }
    }
}