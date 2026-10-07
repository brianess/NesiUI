// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using Nesi.UI;

namespace Nesi.UI.Demo;

internal static class Program
{
    private static async Task Main()
    {
        var root = AppContext.BaseDirectory;
        var input = new InputManager();
        using var app = new NesiApplication(new NesiApplicationOptions
        {
            Input = input,
            TopMost = true,
            Font = new FontOptions
            {
                Path = Path.Combine(root, "Assets", "Fonts", "FontWithIcons.ttf"),
                GlyphRanges = new ushort[]
                {
                    0x0020, 0x00FF, 0xF000, 0xF8FF, 0
                }
            },
            Assets = new AssetOptions
            {
                Images = new Dictionary<string, string>
                {
                    ["pet"] = Path.Combine(root, "Assets", "Images", "pet.png")
                }
            }
        });

        var style = new WindowStyle()
        {
            Flags = ImGuiNET.ImGuiWindowFlags.NoResize,
            BackgroundOpacity = 0.9f,
            Background = Style.BackgroundType.Particles,
            Border = false
        };

        var mainWindow = new MainWindow(app.Windows) { Style = style };
        app.Windows.RegisterWindow("Main", mainWindow);

        var menu = new MenuWindow(app.Windows, app.Assets) { Style = style };
        app.Windows.RegisterWindow("Menu", menu);

        var settingsWindow = new SettingsWindow(app.Windows) { Style = style };
        app.Windows.RegisterWindow("Settings", settingsWindow);

        app.FrameUpdated += () =>
        {
            if (app.Windows.IsCurrentWindow<MenuWindow>())
                Backhud.Render();

            if (!input.WasPressed("Insert"))
                return;

            if (app.Windows.IsCurrentWindow<MenuWindow>())
                menu.ToggleMenu();
        };
        await app.RunAsync();
    }
}
