using System.Numerics;
using ImGuiNET;
using Nesi.UI.Style;

namespace Nesi.UI.Components;

/// <summary>Displays text with optional custom color and wrapping.</summary>
public class UIText : UIElement
{
    private string _text;
    private Vector4 _textColor = Colors.text;
    private bool _hasCustomTextColor;
    private bool _isWrapped = false;

    /// <summary>Gets or sets the displayed text.</summary>
    public string Text
    {
        get => _text;
        set => _text = value;
    }

    /// <summary>Gets or sets a custom text color.</summary>
    public Vector4 TextColor
    {
        get => _textColor;
        set
        {
            _textColor = value;
            _hasCustomTextColor = true;
        }
    }

    /// <summary>Gets or sets whether text is wrapped.</summary>
    public bool IsWrapped
    {
        get => _isWrapped;
        set => _isWrapped = value;
    }

    /// <summary>Creates a text element.</summary>
    /// <param name="text">Text to display.</param>
    /// <param name="id">Optional stable ImGui identifier.</param>
    public UIText(string text, string? id = null) : base(id)
    {
        _text = text;
    }

    /// <inheritdoc />
    public override void Render()
    {
        if (!IsVisible) return;

        var textColor = _hasCustomTextColor
            ? _textColor
            : Colors.text;

        ImGui.PushID(_id);
        ApplyMargin();

        try
        {
            PushDisabledState();

            ImGui.PushStyleColor(ImGuiCol.Text, textColor);

            if (_isWrapped)
                ImGui.TextWrapped(_text);
            else
                ImGui.Text(_text);

            ImGui.PopStyleColor();

            PopDisabledState();
        }
        finally
        {
            RestoreMargin();
            ImGui.PopID();
        }
    }
}