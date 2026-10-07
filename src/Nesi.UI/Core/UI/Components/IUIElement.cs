using System.Numerics;

namespace Nesi.UI.Components;

/// <summary>Defines the common contract for UI elements and containers.</summary>
public interface IUIElement
{
    /// <summary>Gets the element identifier used to scope ImGui item IDs.</summary>
    string Id { get; }

    /// <summary>Gets or sets whether the element is rendered.</summary>
    bool IsVisible { get; set; }

    /// <summary>Gets or sets whether the element is enabled for interaction.</summary>
    bool IsEnabled { get; set; }

    /// <summary>Renders the element and, where applicable, its children.</summary>
    void Render();

    /// <summary>Updates per-frame element state.</summary>
    void Update();
}

