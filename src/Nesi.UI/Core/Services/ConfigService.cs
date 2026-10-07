// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Text.Json;
using Nesi.UI.Utils;

namespace Nesi.UI.Services;

public interface IConfigService
{
    T GetValue<T>(string key, T defaultValue = default!);
    void SetValue<T>(string key, T value);
    void Save();
    void Load();
}

public class ConfigService : IConfigService, IDisposable
{
    private readonly string _configPath;
    private Dictionary<string, object> _config = new();
    private readonly object _lock = new();
    private bool _disposed;

    public ConfigService()
    {
        var appFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Nesi");

        if (!Directory.Exists(appFolder))
            Directory.CreateDirectory(appFolder);

        _configPath = Path.Combine(appFolder, "config.json");
        Load();
    }

    public T GetValue<T>(string key, T defaultValue = default!)
    {
        lock (_lock)
        {
            if (_config.TryGetValue(key, out var value) && value is T typedValue)
                return typedValue;

            return defaultValue;
        }
    }

    public void SetValue<T>(string key, T value)
    {
        lock (_lock)
        {
            _config[key] = value!;
            Save();
        }
    }

    public void Load()
    {
        try
        {
            if (File.Exists(_configPath))
            {
                var json = File.ReadAllText(_configPath);
                _config = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                    ?? new Dictionary<string, object>();
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to load config: {ex.Message}");
            _config = new Dictionary<string, object>();
        }
    }

    public void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_config, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_configPath, json);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to save config: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        Save();
        _disposed = true;
    }
}

