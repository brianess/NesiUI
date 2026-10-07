using System.Numerics;
using ImGuiNET;

namespace Nesi.UI.Components;

/// <summary>A floating-point slider bounded by a minimum and maximum value.</summary>
public class UISlider : UIElement
{
    private string _label;
    private float _value = 0f;
    private float _minValue = 0f;
    private float _maxValue = 100f;
    private Vector2 _sliderSize = new Vector2(0, 0); // 0 = auto

    /// <summary>Raised when the slider value changes during interaction or assignment.</summary>
    public event Action<float>? OnValueChanged;

    /// <summary>Gets or sets the displayed label.</summary>
    public string Label
    {
        get => _label;
        set => _label = value;
    }

    /// <summary>Gets or sets the value, clamped to the configured range.</summary>
    public float Value
    {
        get => _value;
        set
        {
            float newValue = Math.Clamp(value, _minValue, _maxValue);
            if (Math.Abs(_value - newValue) > 0.0001f)
            {
                _value = newValue;
                OnValueChanged?.Invoke(_value);
            }
        }
    }

    /// <summary>Gets or sets the lower bound.</summary>
    public float MinValue
    {
        get => _minValue;
        set => _minValue = value;
    }

    /// <summary>Gets or sets the upper bound.</summary>
    public float MaxValue
    {
        get => _maxValue;
        set => _maxValue = value;
    }

    /// <summary>Gets or sets the slider dimensions; a zero width uses ImGui's default width.</summary>
    public Vector2 SliderSize
    {
        get => _sliderSize;
        set => _sliderSize = value;
    }

    /// <summary>Creates a slider with the specified range and initial value.</summary>
    /// <param name="label">Text displayed beside the slider.</param>
    /// <param name="minValue">Inclusive lower bound.</param>
    /// <param name="maxValue">Inclusive upper bound.</param>
    /// <param name="initialValue">Initial value, clamped to the range.</param>
    /// <param name="id">Optional stable ImGui identifier.</param>
    public UISlider(string label, float minValue = 0f, float maxValue = 100f, float initialValue = 0f, string? id = null)
        : base(id)
    {
        _label = label;
        _minValue = minValue;
        _maxValue = maxValue;
        _value = Math.Clamp(initialValue, _minValue, _maxValue);
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

            float oldValue = _value;

            // Renderiza o slider
            if (_sliderSize.X > 0)
            {
                ImGui.SliderFloat(_label, ref _value, _minValue, _maxValue, "%.2f");
            }
            else
            {
                ImGui.SliderFloat(_label, ref _value, _minValue, _maxValue);
            }

            // Se mudou e está habilitado, dispara evento
            if (Math.Abs(oldValue - _value) > 0.0001f && IsEnabled)
            {
                OnValueChanged?.Invoke(_value);
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
