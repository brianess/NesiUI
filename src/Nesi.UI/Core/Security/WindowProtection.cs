// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Runtime.InteropServices;

namespace Nesi.UI.Utils;

/// <summary>Provides Windows-native overlay visibility and capture-protection operations.</summary>
public static class WindowProtection
{
    private const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;
    private const uint WDA_NONE = 0x00000000;
    private const int GWL_EXSTYLE = -20;
    private const long WS_EX_TOOLWINDOW = 0x00000080L;
    private const long WS_EX_APPWINDOW = 0x00040000L;
    private const int SW_HIDE = 0;
    private const int SW_SHOW = 5;
    private const int SW_SHOWNOACTIVATE = 4;
    private static readonly IntPtr HWND_TOPMOST = new(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new(-2);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern long GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern long SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
        int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    /// <summary>Enables or disables supported Windows display-affinity capture exclusion.</summary>
    /// <param name="hWnd">Native window handle.</param>
    /// <param name="enabled">Whether capture exclusion should be enabled.</param>
    /// <returns><see langword="true"/> when Windows accepts the affinity change.</returns>
    public static bool SetStreamProof(IntPtr hWnd, bool enabled)
    {
        if (hWnd == IntPtr.Zero) return false;
        uint affinity = enabled ? WDA_EXCLUDEFROMCAPTURE : WDA_NONE;
        return SetWindowDisplayAffinity(hWnd, affinity);
    }

    /// <summary>Checks whether the native window has the tool-window taskbar style.</summary>
    /// <param name="hWnd">Native window handle.</param>
    /// <returns><see langword="true"/> when the tool-window style is set.</returns>
    public static bool IsHiddenFromTaskbar(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero) return false;
        var style = GetWindowLong(hWnd, GWL_EXSTYLE);
        return (style & WS_EX_TOOLWINDOW) != 0;
    }

    /// <summary>Removes the native window from the taskbar.</summary>
    /// <param name="hWnd">Native window handle.</param>
    /// <returns><see langword="true"/> when the native style refresh succeeds.</returns>
    public static bool HideFromTaskbar(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero) return false;

        var style = GetWindowLong(hWnd, GWL_EXSTYLE);
        style |= WS_EX_TOOLWINDOW;
        style &= ~WS_EX_APPWINDOW;
        SetWindowLong(hWnd, GWL_EXSTYLE, style);

        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOZORDER = 0x0004;
        const uint SWP_FRAMECHANGED = 0x0020;

        return SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0,
            SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED);
    }

    /// <summary>Adds the native window to the taskbar.</summary>
    /// <param name="hWnd">Native window handle.</param>
    /// <returns><see langword="true"/> when the native style refresh succeeds.</returns>
    public static bool ShowInTaskbar(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero) return false;

        var style = GetWindowLong(hWnd, GWL_EXSTYLE);
        style |= WS_EX_APPWINDOW;
        style &= ~WS_EX_TOOLWINDOW;
        SetWindowLong(hWnd, GWL_EXSTYLE, style);

        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOZORDER = 0x0004;
        const uint SWP_FRAMECHANGED = 0x0020;

        return SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0,
            SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED);
    }

    /// <summary>Enables or disables the native topmost window style.</summary>
    /// <param name="hWnd">Native window handle.</param>
    /// <param name="enabled">Whether the window should be topmost.</param>
    /// <returns><see langword="true"/> when the native position update succeeds.</returns>
    public static bool SetTopMost(IntPtr hWnd, bool enabled)
    {
        if (hWnd == IntPtr.Zero) return false;

        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOACTIVATE = 0x0010;

        return SetWindowPos(
            hWnd,
            enabled ? HWND_TOPMOST : HWND_NOTOPMOST,
            0,
            0,
            0,
            0,
            SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
    }

    /// <summary>Shows a native window without activating it.</summary>
    /// <param name="hWnd">Native window handle.</param>
    /// <returns>The result of the native show operation.</returns>
    public static bool ShowWindowNoActivate(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero) return false;

        bool result = ShowWindow(hWnd, SW_SHOWNOACTIVATE);

        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOACTIVATE = 0x0010;

        SetWindowPos(hWnd, new IntPtr(-1), 0, 0, 0, 0,
            SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);

        return result;
    }

    /// <summary>Hides a native window.</summary>
    /// <param name="hWnd">Native window handle.</param>
    /// <returns>The result of the native show operation.</returns>
    public static bool HideWindow(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero) return false;
        return ShowWindow(hWnd, SW_HIDE);
    }

    /// <summary>Checks whether a native window is visible.</summary>
    /// <param name="hWnd">Native window handle.</param>
    /// <returns><see langword="true"/> when the window is visible.</returns>
    public static bool IsVisible(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero) return false;
        return IsWindowVisible(hWnd);
    }

    /// <summary>Checks whether the current Windows build is at least 19041.</summary>
    /// <returns><see langword="true"/> when the version check passes.</returns>
    public static bool IsStreamProofSupported()
    {
        var version = Environment.OSVersion.Version;
        return version.Major >= 10 && version.Build >= 19041;
    }
}
