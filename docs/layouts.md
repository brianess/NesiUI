# Layouts

Layouts are component containers in `Nesi.UI.Components`. Add child elements
through `AddElement`; retain the layout and its child instances between frames.

- `VBox` renders children vertically. Its `Spacing` controls vertical item
  spacing; the default is 8 pixels.
- `HBox` renders children on one row using ImGui `SameLine`; its `Spacing`
  controls the horizontal gap; the default is 8 pixels.
- `SplitView` renders its first child on the left and its second child on the
  right, with `FirstSize`, minimum region widths, and an optional fixed
  `SplitterSize` gap. The current splitter is a spacer, not a draggable
  resize handle. Additional children beyond the first two are not rendered by
  this layout.
- `Sidebar` renders a fixed-width background and its children, and can be
  composed alongside another region.
- `UIPanel`, `UIGroupBox`, and `UIChild` provide visual or child-region
  containers; see [Components](components.md).

These types use ImGui's current content region and layout rules. Their declared
`ExpandWidth`/`ExpandHeight` properties are present in the API, but current
rendering does not use them to apply separate expansion behavior.
