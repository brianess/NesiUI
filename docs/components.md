# Components

UI components are in the `Nesi.UI.Components` namespace. They implement
`IUIElement` and expose `Render` and `Update`. Most controls should be constructed
once and retained as fields of their owning window.

## Controls

- `UIButton` invokes `OnClick` when activated. `UIButtonSize` supports automatic,
  named, full-width, and custom sizes. `UIButtonStyle.Sidebar` changes the
  control's width and text alignment. `UIButtonVariant` is exposed, but the
  current renderer does not select different rendering based on its value.
- `UICheckbox` exposes `IsChecked` and `OnValueChanged`.
- `UISlider` exposes a floating-point `Value`, minimum/maximum, and
  `OnValueChanged`.
- `UIComboBox` accepts items using `AddItem`/`AddItems`, and exposes
  `SelectedIndex`, `SelectedValue`, and `OnSelectionChanged`. It starts with no
  selected item.
- `UIText` displays text and supports a custom color and wrapping.
- `UITextInput` exposes its current `Value`, `MaxLength`, `OnValueChanged`, and
  `OnSubmit` (Enter).

Controls inherit common visibility, enabled state, ID, size, position, padding,
and margin properties from `UIElement`; actual layout behavior is controlled by
the ImGui calls made by each component.

## Containers

`ContainerElement` stores child elements and provides `AddElement`,
`RemoveElement`, `ClearElements`, `Children`, and `ChildCount`. Specialized
containers are described in [Layouts](layouts.md).

## Panels and group boxes

- `UIPanel` groups children and supports frame background, border color, border
  size, rounding, and inner padding.
- `UIGroupBox` provides a titled child area with a glass-style background and
  header; its children are rendered in an inner content region.
- `UIChild` wraps an ImGui child region and exposes its size and
  `ImGuiWindowFlags`; the underlying child region provides ImGui scrolling.
- `Sidebar` draws a background and renders child elements; it is not a separate
  window.

`PreUI` provides small immediate-mode helpers (centered title and close button)
for windows that use ImGui directly.
