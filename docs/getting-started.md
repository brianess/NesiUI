# Getting Started

## Requirements

- .NET 10 SDK.
- Windows for running the current overlay host and its native window-protection APIs.

## Create a window

The library exposes `NesiApplication`, `NesiApplicationOptions`, `WindowManager`,
and `BaseWindow`. This minimal example uses the UI component API without directly
calling ImGui.NET:

```csharp
using Nesi.UI;
using Nesi.UI.Components;

internal static class Program
{
    private static async Task Main()
    {
        using var app = new NesiApplication(new NesiApplicationOptions
        {
            Title = "NesiUI sample",
            Width = 1280,
            Height = 720
        });

        app.Windows.RegisterWindow("Main", new MainWindow(), isMain: true);
        await app.RunAsync();
    }
}

internal sealed class MainWindow : BaseWindow
{
    private readonly VBox _content = new();

    public MainWindow()
    {
        _content.AddElement(new UIText("Hello from NesiUI"));
        _content.AddElement(new UIButton("Click me"));
    }

    protected override void RenderContent()
    {
        _content.Update();
        _content.Render();
    }
}
```

`RegisterWindow(..., isMain: true)` selects the initial window. Alternatively,
register windows and select one with `app.Windows.SwitchTo("Main")`. Keep each
window's component tree as fields and update/render it from `RenderContent`;
recreating controls every frame would discard their state and event subscriptions.

## Run the repository samples

From the repository root, build the solution and run a sample project:

```powershell
dotnet build Nesi.slnx
dotnet run --project samples/Nesi.UI.SimpleDemo/Nesi.UI.SimpleDemo.csproj
```

The samples reference `src/Nesi.UI/Nesi.UI.csproj` directly. For an application
in this repository, use the same project-reference approach until a NuGet package
is intentionally published. To create a local consumer project from the repository
root:

```powershell
dotnet new console -n MyNesiApp -f net10.0
dotnet add MyNesiApp/MyNesiApp.csproj reference src/Nesi.UI/Nesi.UI.csproj
```

Then copy the `MainWindow` and startup example above into that project.

## Configure fonts and images

Fonts and named images are configured through `NesiApplicationOptions`. Use paths
that are valid for the consuming application; the library does not require an
`Assets` directory. For example:

```csharp
using System.Collections.Generic;
using System.IO;

var baseDirectory = AppContext.BaseDirectory;

var options = new NesiApplicationOptions
{
    Font = new FontOptions
    {
        Path = Path.Combine(baseDirectory, "Assets", "Font.ttf"),
        Size = 18,
        GlyphRanges = new ushort[] { 0x0020, 0x00FF, 0 }
    },
    Assets = new AssetOptions
    {
        Images = new Dictionary<string, string>
        {
            ["logo"] = Path.Combine(baseDirectory, "Assets", "logo.png")
        }
    }
};
```

After the renderer initializes, retrieve a loaded image handle with
`app.Assets.GetImage("logo")` or `TryGetImage`. Glyph ranges are encoded as
start/end pairs and terminated by `0`, as in the sample above.

## Next

- [Windows](windows.md)
- [Components](components.md)
- [Layouts](layouts.md)
- [Styling and backgrounds](styling.md)
- [All samples](samples.md)
