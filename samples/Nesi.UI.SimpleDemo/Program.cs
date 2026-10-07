// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using Nesi.UI;

namespace Nesi.UI.SimpleDemo;

internal static class Program
{
    private static async Task Main()
    {
        var root = AppContext.BaseDirectory;
        using var app = new NesiApplication(new NesiApplicationOptions
        {
            Title = "Nesi.UI Simple Demo",
            Width = 1920,
            Height = 1080,
            Font = new FontOptions { Path = "C:/Windows/Fonts/seguisb.ttf" },
            Assets = new AssetOptions()
        });

        var mainWindow = new SimpleWindow(app.Windows)
        {
            Style = new WindowStyle
            {
                BackgroundOpacity = 1f,
                Border = true,
                Flags = ImGuiNET.ImGuiWindowFlags.None,
                Background = Style.BackgroundType.Aurora
            }
        };

        // register windows
        app.Windows.RegisterWindow("Main", mainWindow);
        app.Windows.SwitchTo("Main");

        await app.RunAsync();
    }
}