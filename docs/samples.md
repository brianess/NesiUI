# Samples

All projects under `samples/` are independent executables that reference
`src/Nesi.UI/Nesi.UI.csproj`. Build the solution with `dotnet build Nesi.slnx`,
then run any project using `dotnet run --project <sample-project.csproj>`.

## Nesi.UI.SimpleDemo

**Purpose:** the smallest current example of an application consuming Nesi.UI.

**Demonstrates:** `NesiApplication`, options, a `BaseWindow`, retained component
instances, `VBox`, `SplitView`, group boxes, button/checkbox/slider/text
components, and custom ImGui draw-list rendering through the optional
`WeaponWheel`.

**Dependencies/assets:** the Nesi.UI project reference and a font path hard-coded
to `C:/Windows/Fonts/seguisb.ttf`. That path is machine-specific and should be
changed when adapting the sample.

## Nesi.UI.Demo

**Purpose:** a broader UI showcase.

**Demonstrates:** Main/Menu/Settings windows, layouts and controls, menu input
toggle, theme-related UI, custom Backhud draw-list rendering, configured image
assets, and a font with icon glyph ranges.

**Dependencies/assets:** the Nesi.UI project reference; bundled
`FontWithIcons.ttf`, `mini-jane.png`, and `shadow.png`; input polling via
`InputManager`. Asset/font redistribution rights and any required attribution
must be verified independently of the repository's MIT code license.

## Nesi.UI.LoaderDemo

**Purpose:** show a staged visual flow between two windows.

**Demonstrates:** `BaseWindow`, `WindowManager.SwitchTo`, a simulated ten-second
progress/loading screen, login-style text inputs, buttons, and
`BackgroundType.Aurora`/`Particles`.

**Dependencies/assets:** the Nesi.UI project reference and a font path
hard-coded to `C:/Windows/Fonts/seguisb.ttf`. The loader is a timed demonstration,
and the login screen does not perform authentication.

## Selecting samples

- Start from **SimpleDemo** to learn the application/window/component structure.
- Use **LoaderDemo** to see window switching and background effects.
- Use **Demo** to explore a more complete UI and custom draw-list content.
