// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using Nesi.UI;

namespace Nesi.UI.LoaderDemo;

internal static class Program
{
    static async Task Main()
    {
        using var app = new NesiApplication(
            new NesiApplicationOptions()
            {
                Title = "Loader",
                Width = 1920,
                Height = 1080,
                VSync = true,
                Font = new FontOptions() { Path = "C:/Windows/Fonts/seguisb.ttf", Size = 30 }
            }
        );

        var loaderWin = new LoaderWindow(app.Windows)
        {
            Style = new WindowStyle
            {
                Flags = ImGuiNET.ImGuiWindowFlags.NoResize,
                Background = Nesi.UI.Style.BackgroundType.Aurora,
                BackgroundOpacity = 1f,
                Border = true,
                BorderRadius = 10f,
                Shadow = 0f
            }
        };

        var loginWin = new LoginWindow(app.Windows)
        {
            Style = new WindowStyle
            {
                Flags = ImGuiNET.ImGuiWindowFlags.NoResize,
                Background = Nesi.UI.Style.BackgroundType.Particles,
                BackgroundOpacity = 1f,
                Border = true,
                BorderRadius = 10f,
                Shadow = 0f
            }
        };

        app.Windows.RegisterWindow("Loader", loaderWin, true);
        app.Windows.RegisterWindow("Login", loginWin, false);

        await app.RunAsync();
    }
}