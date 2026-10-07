// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Numerics;
using ImGuiNET;
using Nesi.UI.Core.UI.Components;
using Nesi.UI.Style;

namespace Nesi.UI.LoaderDemo;

public class LoaderWindow : BaseWindow
{
    private readonly WindowManager _wm;
    private readonly DateTime _startTime;
    private readonly float _loadingDuration = 10f;

    public LoaderWindow(WindowManager windowManager) : base("LoaderWindow")
    {
        _wm = windowManager;
        _size = new Vector2(400, 200);
        _startTime = DateTime.Now;
    }

    protected override void RenderContent()
    {
        PreUI.DrawCloseButton(_size);
        PreUI.DrawCenterTitle("Nesi Loader", _size);

        float elapsed = (float)(DateTime.Now - _startTime).TotalSeconds;
        float progress = Math.Clamp(elapsed / _loadingDuration, 0f, 1f);

        DrawLoadingBar(progress);

        if (progress >= 1f)
            _wm.SwitchTo("Login");
    }

    private void DrawLoadingBar(float progress)
    {
        // Config
        var pos = new Vector2(20f, _size.Y - 15f);
        var size = new Vector2(ImGui.GetContentRegionAvail().X - 20f, 5f);

        // Render
        ImGui.SetCursorPos(pos);

        // Change color
        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, Colors.accentColor);
        ImGui.ProgressBar(progress, size);
        ImGui.PopStyleColor();
    }
}
