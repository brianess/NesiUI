using System.Numerics;
using ImGuiNET;

namespace Nesi.UI.Components;

/// <summary>A text input control with change and submit notifications.</summary>
public class UITextInput : UIElement
{
    private string _label;
    private string _value = "";
    private int _maxLength = 256;
    private Vector2 _inputSize = new Vector2(0, 0); // 0 = auto

    /// <summary>Raised when the input value changes.</summary>
    public event Action<string>? OnValueChanged;

    /// <summary>Raised when Enter is pressed while the input is active.</summary>
    public event Action? OnSubmit;

    /// <summary>Gets or sets the displayed label.</summary>
    public string Label
    {
        get => _label;
        set => _label = value;
    }

    /// <summary>Gets or sets the entered value.</summary>
    public string Value
    {
        get => _value;
        set => _value = value ?? "";
    }

    /// <summary>Gets or sets the maximum input length; values are at least one.</summary>
    public int MaxLength
    {
        get => _maxLength;
        set => _maxLength = Math.Max(1, value);
    }

    /// <summary>Gets or sets the input dimensions.</summary>
    public Vector2 InputSize
    {
        get => _inputSize;
        set => _inputSize = value;
    }

    /// <summary>Creates a text input.</summary>
    /// <param name="label">Text displayed beside the input.</param>
    /// <param name="id">Optional stable ImGui identifier.</param>
    public UITextInput(string label, string? id = null) : base(id)
    {
        _label = label;
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

            string oldValue = _value;

            // Renderiza o campo de input
            ImGui.InputText(_label, ref _value, (uint)_maxLength, ImGuiInputTextFlags.None);

            // Se mudou e está habilitado, dispara evento
            if (oldValue != _value && IsEnabled)
            {
                OnValueChanged?.Invoke(_value);
            }

            // Verifica se Enter foi pressionado
            if (IsEnabled && ImGui.IsItemActive() && ImGui.IsKeyPressed(ImGuiKey.Enter))
            {
                OnSubmit?.Invoke();
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
