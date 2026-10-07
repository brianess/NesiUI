using System.Numerics;
using ImGuiNET;
using Nesi.UI.Utils;

namespace Nesi.UI.Components;

/// <summary>Base implementation of common UI element state and layout properties.</summary>
public abstract class UIElement : IUIElement
{
    // Identity and state
    /// <summary>Identifier used to scope the element's ImGui IDs.</summary>
    protected string _id;
    /// <summary>Backing field for <see cref="IsVisible"/>.</summary>
    protected bool _isVisible = true;
    /// <summary>Backing field for <see cref="IsEnabled"/>.</summary>
    protected bool _isEnabled = true;

    // Layout and dimensions
    /// <summary>Backing field for <see cref="Size"/>.</summary>
    protected Vector2 _size = Vector2.Zero;
    /// <summary>Backing field for <see cref="Position"/>.</summary>
    protected Vector2 _position = Vector2.Zero;
    /// <summary>Backing field for <see cref="Padding"/>.</summary>
    protected Vector2 _padding = new Vector2(4, 4);
    /// <summary>Backing field for <see cref="Margin"/>.</summary>
    protected Vector2 _margin = Vector2.Zero;

    // ImGui identifier
    /// <inheritdoc />
    public string Id => _id;

    /// <inheritdoc />
    public bool IsVisible
    {
        get => _isVisible;
        set => _isVisible = value;
    }
    /// <inheritdoc />
    public bool IsEnabled
    {
        get => _isEnabled;
        set => _isEnabled = value;
    }

    /// <summary>Gets or sets the element's size.</summary>
    public Vector2 Size
    {
        get => _size;
        set => _size = value;
    }

    /// <summary>Gets or sets the element's position.</summary>
    public Vector2 Position
    {
        get => _position;
        set => _position = value;
    }

    /// <summary>Gets or sets the element's inner padding.</summary>
    public Vector2 Padding
    {
        get => _padding;
        set => _padding = value;
    }

    /// <summary>Gets or sets the element's outer margin.</summary>
    public Vector2 Margin
    {
        get => _margin;
        set => _margin = value;
    }

    /// <summary>Creates an element with the supplied ID or a generated unique ID.</summary>
    /// <param name="id">Optional identifier used to scope ImGui item IDs.</param>
    protected UIElement(string? id = null)
    {
        _id = id ?? $"{GetType().Name}_{Guid.NewGuid():N}";
    }

    /// <summary>Renders the element. Derived elements provide their rendering implementation.</summary>
    /// <inheritdoc />
    public virtual void Render()
    {
        if (!IsVisible) return;
    }

    /// <summary>Updates element state before rendering.</summary>
    /// <inheritdoc />
    public virtual void Update()
    {
    }

    /// <summary>
    /// Aplica margin posicionando o cursor.
    /// </summary>
    protected void ApplyMargin()
    {
        if (_margin.X > 0 || _margin.Y > 0)
        {
            var currentPos = ImGui.GetCursorPos();
            ImGui.SetCursorPos(currentPos + _margin);
        }
    }

    /// <summary>
    /// Restaura a posição do cursor após aplicar margin.
    /// </summary>
    protected void RestoreMargin()
    {
        if (_margin.X > 0 || _margin.Y > 0)
        {
            var currentPos = ImGui.GetCursorPos();
            ImGui.SetCursorPos(currentPos - _margin);
        }
    }

    /// <summary>
    /// Aplica padding interno ao conteúdo.
    /// </summary>
    protected void PushPadding()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, _padding);
    }

    /// <summary>
    /// Remove o padding anterior.
    /// </summary>
    protected void PopPadding()
    {
        ImGui.PopStyleVar();
    }

    /// <summary>
    /// Desabilita o elemento visualmente (reduz opacidade).
    /// </summary>
    protected void PushDisabledState()
    {
        if (!IsEnabled)
        {
            ImGui.BeginDisabled(true);
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().DisabledAlpha);
        }
    }

    /// <summary>
    /// Remove o estado desabilitado do elemento.
    /// </summary>
    protected void PopDisabledState()
    {
        if (!IsEnabled)
        {
            ImGui.PopStyleVar();
            ImGui.EndDisabled();
        }
    }
}
