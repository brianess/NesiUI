# Windows

The window API is in `Nesi.UI`.

## Define a window

Derive from `BaseWindow` and implement `RenderContent`. Use `OnShow`, `OnHide`,
and `Update` for lifecycle or per-frame behavior as needed. The standard
`BaseWindow.Render` calls `RenderContent` only while `IsVisible` is true.

```csharp
internal sealed class SettingsWindow : BaseWindow
{
    public SettingsWindow() : base("Settings")
    {
        Style = new WindowStyle
        {
            Flags = ImGuiNET.ImGuiWindowFlags.NoResize,
            BackgroundOpacity = 0.9f
        };
    }

    protected override void RenderContent()
    {
        // Update and render retained components here.
    }
}
```

## Register and navigate

```csharp
app.Windows.RegisterWindow("Main", new MainWindow(), isMain: true);
app.Windows.RegisterWindow("Settings", new SettingsWindow());
app.Windows.SwitchTo("Settings");
app.Windows.GoBack();
```

Registering a window with `isMain: true` selects it immediately. The key
`"Main"` is also treated as the initial window by the current implementation.
`SwitchTo` and `GoBack` use the current window's `FadeOutDuration` before
selecting the next window, then use the new window's `FadeInDuration`. Both
durations are measured in seconds; setting either to zero disables that part
of the transition. The defaults are 0.14 seconds for fade-out and 0.18 seconds
for fade-in.

`BaseWindow.TransitionAlpha` exposes the framework transition opacity to custom
window drawing. ImGui widgets inherit the window's style alpha automatically;
for explicit draw-list colors, multiply the color's alpha by
`TransitionAlpha`.

## WindowStyle

Each window has its own `Style`. `Flags` are combined with the host's default
`NoTitleBar` and `NoSavedSettings` flags. Other available settings are:

- `Background`: `BackgroundType.None`, `Glass`, `Aurora`, `Neon`, `Grid`,
  `Particles`, or `Waves`.
- `BackgroundOpacity`: background opacity from 0 to 1.
- `Border`: whether the framework draws its light border.
- `BorderRadius`: corner rounding used by the background/border drawing.
- `Shadow`: shadow strength; zero disables it.
- `FadeInDuration` and `FadeOutDuration`: transition durations in seconds.

The initial size comes from `BaseWindow.GetWindowSize()`. It is set on the
window's first frame after selection; `NoResize` can prevent resizing, but if
removed the user can resize the ImGui window. Saved ImGui settings are disabled
for the host window.

`IWindow` can be implemented directly, but `BaseWindow` supplies the common
visibility, style, and lifecycle behavior.
