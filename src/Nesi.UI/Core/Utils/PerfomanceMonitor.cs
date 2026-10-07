// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Diagnostics;
using ImGuiNET;

namespace Nesi.UI.Utils;

public class PerformanceMonitor : IDisposable
{
    private readonly Stopwatch _frameTimer = new();
    private readonly Queue<float> _frameTimes = new();
    private readonly int _maxSamples = 60;
    private bool _disposed;
    public bool ShouldShowDebug { get; set; }

    public PerformanceMonitor()
    {
        _frameTimer.Start();
    }

    public void StartFrame()
    {
        _frameTimer.Restart();
    }

    public void EndFrame()
    {
        _frameTimer.Stop();
        var frameTime = (float)_frameTimer.Elapsed.TotalMilliseconds;

        lock (_frameTimes)
        {
            _frameTimes.Enqueue(frameTime);
            if (_frameTimes.Count > _maxSamples)
                _frameTimes.Dequeue();
        }
    }

    public float GetAverageFPS()
    {
        lock (_frameTimes)
        {
            if (_frameTimes.Count == 0) return 0;

            var avgFrameTime = _frameTimes.Average();
            return avgFrameTime > 0 ? 1000f / avgFrameTime : 0;
        }
    }

    public float GetCurrentFPS()
    {
        return _frameTimer.Elapsed.TotalMilliseconds > 0
            ? (float)(1000.0 / _frameTimer.Elapsed.TotalMilliseconds)
            : 0;
    }

    public void RenderDebug()
    {
        ImGui.SetNextWindowBgAlpha(0.7f);
        if (ImGui.Begin("Performance Debug", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.Text($"FPS: {GetCurrentFPS():F1}");
            ImGui.Text($"Avg FPS: {GetAverageFPS():F1}");
            ImGui.Text($"Frame Time: {_frameTimer.Elapsed.TotalMilliseconds:F2}ms");
            ImGui.Text($"Memory: {GC.GetTotalMemory(false) / 1024 / 1024} MB");
        }
        ImGui.End();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _frameTimer.Stop();
    }
}

