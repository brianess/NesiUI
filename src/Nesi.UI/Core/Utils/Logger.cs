// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

namespace Nesi.UI.Utils;

public static class Logger
{
    private static readonly object _lock = new();
    private static string _logPath;

    static Logger()
    {
        _logPath = Path.Combine(FolderManager.Cache, $"log_{DateTime.Now:yyyy-MM-dd}.txt");
    }

    public static void Info(string message) =>
        Log("INFO", message);

    public static void Warning(string message) =>
        Log("WARN", message);

    public static void Error(string message) =>
        Log("ERROR", message);

    public static void Debug(string message) =>
        Log("DEBUG", message);

    private static void Log(string level, string message)
    {
        var logLine = $"[{DateTime.Now:HH:mm:ss}] [{level}] {message}";

        lock (_lock)
        {
            Console.WriteLine(logLine);

            try
            {
                File.AppendAllText(_logPath, logLine + Environment.NewLine);
            }
            catch { /* Ignora erros de log pra n quebrar o bgl */ }
        }
    }
}

