using System.Numerics;
using ImGuiNET;

namespace Nesi.UI.Components;

/// <summary>A checkbox with a Boolean value and change notification.</summary>
public class UICheckbox : UIElement
{
    private string _label;
    private bool _isChecked = false;

    /// <summary>Raised when the checked state changes.</summary>
    public event Action<bool>? OnValueChanged;

    /// <summary>Gets or sets the displayed label.</summary>
    public string Label
    {
        get => _label;
        set => _label = value;
    }

    /// <summary>Gets or sets whether the checkbox is checked.</summary>
    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked != value)
            {
                _isChecked = value;
                OnValueChanged?.Invoke(_isChecked);
            }
        }
    }

    /// <summary>Creates a checkbox.</summary>
    /// <param name="label">Text displayed beside the checkbox.</param>
    /// <param name="initialValue">Initial checked state.</param>
    /// <param name="id">Optional stable ImGui identifier.</param>
    public UICheckbox(string label, bool initialValue = false, string? id = null) : base(id)
    {
        _label = label;
        _isChecked = initialValue;
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

            bool wasChecked = _isChecked;

            // Renderiza o checkbox
            ImGui.Checkbox(_label, ref _isChecked);

            // Se mudou e está habilitado, dispara evento
            if (wasChecked != _isChecked && IsEnabled)
            {
                OnValueChanged?.Invoke(_isChecked);
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
