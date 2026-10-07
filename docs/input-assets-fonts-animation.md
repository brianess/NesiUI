# Input, assets, fonts, and animation

## Input

`NesiApplicationOptions.Input` accepts an `IInputSource`; by default it creates
an `InputManager`. The manager polls its built-in `DefaultKeys` and provides
`WasPressed` and `IsHeld` for supported key names. A custom implementation can
provide a different input source by implementing `IInputSource.Update`.

The current default input implementation uses Windows virtual-key polling. It
does not provide a configurable key-binding registry; the consumer can build
that behavior on top of its own `IInputSource`.

## Fonts

`FontOptions` accepts a font file `Path`, pixel `Size`, and ImGui `GlyphRanges`
encoded as inclusive start/end pairs followed by a `0` terminator. If no valid
path is configured, the renderer does not replace the overlay's default font.
The library does not assume a font filename or a fixed assets folder.

`FontManager` is also public, but its constructor accepts ClickableTransparentOverlay's
`Overlay`; most application consumers can configure a font through
`NesiApplicationOptions.Font` instead.

## Image assets

Configure named files through `AssetOptions.Images`. The renderer loads existing
files during initialization and stores the resulting handles in
`app.Assets`. Use `AssetManager.GetImage(key)` when a missing key should throw, or
`TryGetImage(key, out handle)` to handle an absent/unloaded image explicitly.
Asset paths are supplied by the application; there is no required `Assets`
directory.

## Animation

`Nesi.UI.Utils.Animation` exposes float, color, and vector interpolation helpers.
`AnimateFloat`, `AnimateColor`, and `AnimateVector2` keep static state keyed by
the caller-supplied ID and use ImGui's frame delta time. Use unique IDs for
independent animations. `ResetAnimation(id)` removes one animation's stored
state; `ResetAllAnimations()` clears all animation state. On the first call for
an ID and value type, the current value is initialized to the supplied target,
so seed the animation with its starting target before changing that target when
an initial transition is needed.

The easing and `Lerp` methods are pure value helpers. Explicit colors submitted
to an ImGui draw list are not automatically multiplied by ImGui's style alpha;
include the desired alpha in the color passed to the draw-list method.
