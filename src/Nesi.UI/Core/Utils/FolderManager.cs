// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System;
using Nesi.UI.Utils;

namespace Nesi.UI;

public static class FolderManager
{
    public static string Name => "Nesi";

    public static string Cache =>
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData
            ),
            Name
        );

    public static string Root => AppContext.BaseDirectory;

    public static string Assets => Path.Combine(Root, "Assets");

    public static string Log =>
        Path.Combine(Cache, $"log_{DateTime.Now:yyyy-MM-dd}.txt");

    public static string Font(string name) =>
        Path.Combine(Assets, "Fonts", name);

    public static string Image(string name) =>
        Path.Combine(Assets, "Images", name);

    // check exist
    public static bool AssetExists(string path) =>
    File.Exists(path);

    public static bool FontExists(string name) =>
        File.Exists(Font(name));

    public static bool ImageExists(string name) =>
        File.Exists(Image(name));

    public static void Setup()
    {
        if (!Directory.Exists(Cache))
        {
            Directory.CreateDirectory(Cache);
            Logger.Info($"Cache folder created: {Cache}");
        }
        else
        {
            Logger.Info("Cache folder found");
        }
    }

    public static void CleanCache()
    {
        string[] cacheFiles = Directory.GetFiles(Cache, "*.txt");

        foreach (string file in cacheFiles)
        {
            try
            {
                File.Delete(file);
                Logger.Info("Cache cleaned");
            }
            catch { Logger.Error($"Failed to clean cache on '{Cache}'"); }
        }
    }
}

