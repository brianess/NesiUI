using System.Numerics;
using ImGuiNET;

namespace Nesi.UI.Components;

/// <summary>A dropdown selection control containing string items.</summary>
public class UIComboBox : UIElement
{
    private string _label;
    private List<string> _items = new();
    private int _selectedIndex = -1;

    /// <summary>Raised when the selected item changes.</summary>
    public event Action<int, string>? OnSelectionChanged;

    /// <summary>Gets or sets the displayed label.</summary>
    public string Label
    {
        get => _label;
        set => _label = value;
    }

    /// <summary>Gets the current items.</summary>
    public IReadOnlyList<string> Items => _items.AsReadOnly();

    /// <summary>Gets or sets the selected item index, or -1 for no selection.</summary>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (value >= -1 && value < _items.Count && _selectedIndex != value)
            {
                _selectedIndex = value;
                if (_selectedIndex >= 0)
                {
                    OnSelectionChanged?.Invoke(_selectedIndex, _items[_selectedIndex]);
                }
            }
        }
    }

    /// <summary>Gets the selected item, or <see langword="null"/> when none is selected.</summary>
    public string? SelectedValue => _selectedIndex >= 0 ? _items[_selectedIndex] : null;

    /// <summary>Creates a combo box.</summary>
    /// <param name="label">Text displayed beside the combo box.</param>
    /// <param name="id">Optional stable ImGui identifier.</param>
    public UIComboBox(string label, string? id = null) : base(id)
    {
        _label = label;
    }

    /// <summary>Adds an item unless an equal item is already present.</summary>
    /// <param name="item">The item to add.</param>
    public void AddItem(string item)
    {
        if (!_items.Contains(item))
        {
            _items.Add(item);
        }
    }

    /// <summary>Adds multiple items.</summary>
    /// <param name="items">Items to add.</param>
    public void AddItems(params string[] items)
    {
        foreach (var item in items)
        {
            AddItem(item);
        }
    }

    /// <summary>Removes an item.</summary>
    /// <param name="item">The item to remove.</param>
    /// <returns><see langword="true"/> if the item was removed.</returns>
    public bool RemoveItem(string item)
    {
        return _items.Remove(item);
    }

    /// <summary>Removes all items and clears the selection.</summary>
    public void ClearItems()
    {
        _items.Clear();
        _selectedIndex = -1;
    }

    /// <inheritdoc />
    public override void Render()
    {
        if (!IsVisible) return;

        ImGui.PushID(_id);
        ApplyMargin();

        try
        {
            PushDisabledState();

            int oldIndex = _selectedIndex;
            string preview = _selectedIndex >= 0 ? _items[_selectedIndex] : "(none)";

            if (ImGui.BeginCombo(_label, preview))
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    bool isSelected = (_selectedIndex == i);
                    if (ImGui.Selectable(_items[i], isSelected))
                    {
                        _selectedIndex = i;
                        if (IsEnabled && oldIndex != _selectedIndex)
                        {
                            OnSelectionChanged?.Invoke(_selectedIndex, _items[_selectedIndex]);
                        }
                    }

                    if (isSelected)
                        ImGui.SetItemDefaultFocus();
                }

                ImGui.EndCombo();
            }

            PopDisabledState();
        }
        finally
        {
            RestoreMargin();
            ImGui.PopID();
        }
    }
}
