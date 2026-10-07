using System.Numerics;
using ImGuiNET;

namespace Nesi.UI.Components;

/// <summary>Base class for elements that contain and update child elements.</summary>
public abstract class ContainerElement : UIElement
{
    /// <summary>Child elements rendered by this container.</summary>
    protected List<IUIElement> _children = new();

    /// <summary>Creates a container with the supplied optional identifier.</summary>
    /// <param name="id">Optional identifier.</param>
    public ContainerElement(string? id = null) : base(id)
    {
    }

    /// <summary>Adds a child element if it is not already present.</summary>
    /// <param name="element">The element to add.</param>
    public virtual void AddElement(IUIElement element)
    {
        if (element != null && !_children.Contains(element))
        {
            _children.Add(element);
        }
    }

    /// <summary>Removes a child element.</summary>
    /// <param name="element">The element to remove.</param>
    /// <returns><see langword="true"/> if the child was removed.</returns>
    public virtual bool RemoveElement(IUIElement element)
    {
        return _children.Remove(element);
    }

    /// <summary>Removes all child elements.</summary>
    public virtual void ClearElements()
    {
        _children.Clear();
    }

    /// <summary>Gets the current children as a read-only list.</summary>
    public IReadOnlyList<IUIElement> Children => _children.AsReadOnly();

    /// <summary>Gets the number of child elements.</summary>
    public int ChildCount => _children.Count;

    /// <summary>Updates all child elements.</summary>
    public override void Update()
    {
        base.Update();
        foreach (var child in _children)
        {
            child.Update();
        }
    }

    /// <summary>Renders all child elements.</summary>
    protected void RenderChildren()
    {
        foreach (var child in _children)
        {
            child.Render();
        }
    }
}
