using System.Numerics;

using ImGuiNET;
using Nesi.UI.Style;

namespace Nesi.UI.Components;

/// <summary>Defines contextual layout behavior for a button.</summary>
public enum UIButtonStyle
{
    /// <summary>Uses the standard button layout.</summary>
    Default,
    /// <summary>Uses the sidebar-sized, left-aligned layout.</summary>
    Sidebar,
    /// <summary>Reserved group-box context; currently rendered like the default style.</summary>
    GroupBox
}

/// <summary>Defines a preset or custom button size.</summary>
public enum UIButtonSize
{
    /// <summary>Uses ImGui's automatic size.</summary>
    Auto,
    /// <summary>Uses the small preset size.</summary>
    Small,
    /// <summary>Uses the medium preset size.</summary>
    Medium,
    /// <summary>Uses the large preset size.</summary>
    Large,
    /// <summary>Uses the available content width.</summary>
    FullWidth,
    /// <summary>Uses <see cref="UIButton.ButtonSize"/>.</summary>
    Custom
}

/// <summary>Defines horizontal alignment for button text.</summary>
public enum UIButtonContentAlignment
{
    /// <summary>Aligns text to the left.</summary>
    Left,
    /// <summary>Centers text.</summary>
    Center,
    /// <summary>Aligns text to the right.</summary>
    Right
}

/// <summary>Names a button visual variant.</summary>
public enum UIButtonVariant
{
    /// <summary>Primary variant.</summary>
    Primary,
    /// <summary>Secondary variant.</summary>
    Secondary,
    /// <summary>Success variant.</summary>
    Success,
    /// <summary>Danger variant.</summary>
    Danger,
    /// <summary>Warning variant.</summary>
    Warning,
    /// <summary>Ghost variant.</summary>
    Ghost
}

/// <summary>A clickable button that raises an event when activated.</summary>
public class UIButton : UIElement
{
    private string _label;
    private Vector2 _buttonSize = Vector2.Zero;

    /// <summary>Raised when the button is activated while enabled.</summary>
    public event Action? OnClick;

    /// <summary>Gets or sets the button label.</summary>
    public string Label
    {
        get => _label;
        set => _label = value;
    }

    /// <summary>
    /// Custom button size.
    /// Used when Size is set to Custom.
    /// </summary>
    /// <summary>Gets or sets the custom size used by <see cref="UIButtonSize.Custom"/>.</summary>
    public Vector2 ButtonSize
    {
        get => _buttonSize;
        set => _buttonSize = value;
    }

    /// <summary>Gets or sets contextual button layout behavior.</summary>
    public UIButtonStyle Style { get; set; } = UIButtonStyle.Default;

    /// <summary>Gets or sets the button size mode.</summary>
    public new UIButtonSize Size { get; set; } = UIButtonSize.Auto;

    /// <summary>Gets or sets the horizontal alignment of the button label.</summary>
    public UIButtonContentAlignment ContentAlignment { get; set; } =
        UIButtonContentAlignment.Center;

    /// <summary>
    /// Gets or sets the named visual variant. The current renderer does not
    /// vary button rendering based on this property.
    /// </summary>
    public UIButtonVariant Variant { get; set; } = UIButtonVariant.Primary;

    /// <summary>Creates a button.</summary>
    /// <param name="label">Text displayed by the button.</param>
    /// <param name="id">Optional stable ImGui identifier.</param>
    public UIButton(string label, string? id = null) : base(id)
    {
        _label = label;
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
            PushDisabledState();

            ApplyButtonStyle();

            Vector2 buttonSize = GetButtonSize();
            Vector2 textAlignment = GetTextAlignment();

            ImGui.PushStyleVar(
                ImGuiStyleVar.ButtonTextAlign,
                textAlignment
            );

            if (ImGui.Button(_label, buttonSize))
            {
                if (IsEnabled)
                    OnClick?.Invoke();
            }

            ImGui.PopStyleVar();

            ImGui.PopStyleColor(4);
            PopDisabledState();
        }
        finally
        {
            RestoreMargin();
            ImGui.PopID();
        }
    }

    private Vector2 GetButtonSize()
    {
        return Style switch
        {
            UIButtonStyle.Sidebar =>
                new Vector2(ImGui.GetContentRegionAvail().X, 40f),

            _ => Size switch
            {
                UIButtonSize.Auto =>
                    Vector2.Zero,

                UIButtonSize.Small =>
                    new Vector2(100, 28),

                UIButtonSize.Medium =>
                    new Vector2(150, 36),

                UIButtonSize.Large =>
                    new Vector2(200, 44),

                UIButtonSize.FullWidth =>
                    new Vector2(ImGui.GetContentRegionAvail().X, 0),

                UIButtonSize.Custom =>
                    _buttonSize,

                _ =>
                    Vector2.Zero
            }
        };
    }

    private Vector2 GetTextAlignment()
    {
        return Style switch
        {
            UIButtonStyle.Sidebar =>
                new Vector2(0f, 0.5f),

            _ => ContentAlignment switch
            {
                UIButtonContentAlignment.Left =>
                    new Vector2(0f, 0.5f),

                UIButtonContentAlignment.Center =>
                    new Vector2(0.5f, 0.5f),

                UIButtonContentAlignment.Right =>
                    new Vector2(1f, 0.5f),

                _ =>
                    new Vector2(0.5f, 0.5f)
            }
        };
    }

    private void ApplyButtonStyle()
    {
        ImGui.PushStyleColor(
            ImGuiCol.Button,
            Colors.GetAlpha(Colors.accentColor, 0.7f)
        );

        ImGui.PushStyleColor(
            ImGuiCol.ButtonHovered,
            Colors.GetAlpha(Colors.accentColor, 0.5f)
        );

        ImGui.PushStyleColor(
            ImGuiCol.ButtonActive,
            Colors.GetAlpha(Colors.accentColor, 0.2f)
        );

        ImGui.PushStyleColor(
            ImGuiCol.Text,
            Colors.backgroundColor
        );
    }
}