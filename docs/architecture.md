# Architecture

## Solution layout

```text
src/
  Nesi.UI/                 Reusable class library
samples/
  Nesi.UI.Demo/            Multi-window UI showcase
  Nesi.UI.SimpleDemo/      Basic components and layouts
  Nesi.UI.LoaderDemo/      Simulated loader and login-style screens
Nesi.slnx
```

Each sample is a separate executable project and references the library. The
library does not reference a sample.

## Rendering and window lifecycle

`NesiApplication` owns a `WindowManager`, an `AssetManager`, and an internal
renderer derived from ClickableTransparentOverlay's overlay host. Consumers start
the host with `RunAsync`; they do not need to instantiate the internal renderer.

The renderer initializes ImGui, loads an optional font and configured images,
updates the configured input source, raises `FrameUpdated`, and asks the window
manager to render the selected window. A window is registered with a string key
and implements `IWindow`, usually by deriving from `BaseWindow`. `WindowManager`
can select a registered window or return to the previous selection; its
`WindowStyle` fade durations control fade-out/fade-in around those switches.

`BaseWindow.RenderContent` is called during rendering. A component tree should be
created once, retained by the window, then updated and rendered each frame.

## Components and ImGui

The `Nesi.UI.Components` types wrap common ImGui controls and compose them into
containers. The public API also exposes ImGui.NET types such as
`ImGuiWindowFlags`; advanced custom drawing can use ImGui.NET draw lists directly.
Some helpers and controls therefore require understanding ImGui.NET behavior.

## Platform and implementation scope

The library targets `net10.0`. The overlay host uses ClickableTransparentOverlay,
and `WindowProtection` calls `user32.dll`; the current supported runtime scope is
Windows. The library project directly references ClickableTransparentOverlay
11.1.0; ImGui.NET and rendering packages are transitively resolved through it.

Some public helpers have application-specific defaults: configuration/logging
helpers use `%LocalAppData%\\Nesi`, and `FolderManager` exposes an `Assets` path
under the application base directory. These are helper defaults, not requirements
imposed on configured fonts and images.

## Current public API caveat

`FontManager` is public and accepts ClickableTransparentOverlay's `Overlay` in
its constructor. This currently exposes an implementation type in the public
surface; it is documented here as a current API characteristic, not hidden as an
internal abstraction.
