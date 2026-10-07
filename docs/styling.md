# Styling and backgrounds

Styling APIs are in `Nesi.UI.Style`.

## Themes and colors

`ThemeManager` provides the current dark/light theme operations, including
`SetTheme`, `ApplyTheme`, `ApplyCurrentTheme`, and `ApplyThemeWithAnimation`.
`ThemeColors` and `ThemeStyle` describe the theme color/style values. The current
theme manager and `Colors` values are static shared state rather than
per-application instances.

`Colors` supplies predefined colors and helpers such as `RGBA` and `GetAlpha`.
Its predefined color fields are mutable.

## Window backgrounds

Set a window's `WindowStyle.Background` to one of the `BackgroundType` values.
`Background.Render` draws the selected background into the supplied ImGui draw
list. Available effects are `Glass`, `Aurora`, `Neon`, `Grid`, `Particles`, and
`Waves`; `None` leaves rendering to the ordinary ImGui window background.

The effects are draw-list compositions. `Glass` uses translucent fills,
highlights, and borders; it does not blur the pixels behind the window. The
`BackgroundOpacity` parameter controls effect alpha. This does not make the
whole window or native overlay transparent.

## Shadows and borders

`Shadow.Draw` renders a draw-list shadow with configurable opacity, radius,
strength, and corner inset. `WindowStyle.Shadow` controls the host window's
shadow. `WindowStyle.Border` toggles the framework's light outline; border
rounding uses `BorderRadius`.
