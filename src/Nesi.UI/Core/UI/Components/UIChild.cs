using System.Numerics;

using ImGuiNET;

namespace Nesi.UI.Components;

/// <summary>Container based on ImGui.BeginChild with independent size and scrolling.</summary>
public class UIChild : ContainerElement
{
    private new Vector2 _size;
    private ImGuiWindowFlags _flags;

    /// <summary>Gets or sets the child region size; zero dimensions use available space.</summary>
    public new Vector2 Size
    {
        get => _size;
        set => _size = value;
    }

    /// <summary>Gets or sets the flags passed to ImGui.BeginChild.</summary>
    public ImGuiWindowFlags Flags
    {
        get => _flags;
        set => _flags = value;
    }

    /// <summary>Creates a child region.</summary>
    /// <param name="id">Optional identifier.</param>
    /// <param name="size">Optional child region size.</param>
    /// <param name="flags">ImGui child window flags.</param>
    public UIChild(
        string? id = null,
        Vector2? size = null,
        ImGuiWindowFlags flags = ImGuiWindowFlags.None
    ) : base(id)
    {
        _size = size ?? Vector2.Zero;
        _flags = flags;
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
            ImGui.BeginChild(
                _id,
                _size,
                ImGuiChildFlags.None,
                _flags
            );

            RenderChildren();

            ImGui.EndChild();
        }
        finally
        {
            RestoreMargin();
            ImGui.PopID();
        }
    }
}