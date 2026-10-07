// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Nesi.UI.Utils;

public static class Helper
{
    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;

    private const short KEY_DOWN_MASK = unchecked((short)0x8000);

    private static readonly IReadOnlyDictionary<string, int> KeyMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        ["LButton"] = 0x01,
        ["RButton"] = 0x02,
        ["Cancel"] = 0x03,
        ["MButton"] = 0x04,

        ["Backspace"] = 0x08,
        ["Tab"] = 0x09,
        ["Clear"] = 0x0C,
        ["Enter"] = 0x0D,

        ["Shift"] = 0x10,
        ["Ctrl"] = 0x11,
        ["Alt"] = 0x12,
        ["Pause"] = 0x13,
        ["CapsLock"] = 0x14,

        ["Esc"] = 0x1B,
        ["Space"] = 0x20,

        ["PageUp"] = 0x21,
        ["PageDown"] = 0x22,
        ["End"] = 0x23,
        ["Home"] = 0x24,

        ["LeftArrow"] = 0x25,
        ["UpArrow"] = 0x26,
        ["RightArrow"] = 0x27,
        ["DownArrow"] = 0x28,

        ["Select"] = 0x29,
        ["Print"] = 0x2A,
        ["Execute"] = 0x2B,
        ["PrintScreen"] = 0x2C,
        ["Insert"] = 0x2D,
        ["Delete"] = 0x2E,
        ["Help"] = 0x2F,

        ["0"] = 0x30,
        ["1"] = 0x31,
        ["2"] = 0x32,
        ["3"] = 0x33,
        ["4"] = 0x34,
        ["5"] = 0x35,
        ["6"] = 0x36,
        ["7"] = 0x37,
        ["8"] = 0x38,
        ["9"] = 0x39,

        ["A"] = 0x41,
        ["B"] = 0x42,
        ["C"] = 0x43,
        ["D"] = 0x44,
        ["E"] = 0x45,
        ["F"] = 0x46,
        ["G"] = 0x47,
        ["H"] = 0x48,
        ["I"] = 0x49,
        ["J"] = 0x4A,
        ["K"] = 0x4B,
        ["L"] = 0x4C,
        ["M"] = 0x4D,
        ["N"] = 0x4E,
        ["O"] = 0x4F,
        ["P"] = 0x50,
        ["Q"] = 0x51,
        ["R"] = 0x52,
        ["S"] = 0x53,
        ["T"] = 0x54,
        ["U"] = 0x55,
        ["V"] = 0x56,
        ["W"] = 0x57,
        ["X"] = 0x58,
        ["Y"] = 0x59,
        ["Z"] = 0x5A,

        ["LWin"] = 0x5B,
        ["RWin"] = 0x5C,
        ["Apps"] = 0x5D,
        ["Sleep"] = 0x5F,

        ["Numpad0"] = 0x60,
        ["Numpad1"] = 0x61,
        ["Numpad2"] = 0x62,
        ["Numpad3"] = 0x63,
        ["Numpad4"] = 0x64,
        ["Numpad5"] = 0x65,
        ["Numpad6"] = 0x66,
        ["Numpad7"] = 0x67,
        ["Numpad8"] = 0x68,
        ["Numpad9"] = 0x69,

        ["Multiply"] = 0x6A,
        ["Add"] = 0x6B,
        ["Separator"] = 0x6C,
        ["Subtract"] = 0x6D,
        ["Decimal"] = 0x6E,
        ["Divide"] = 0x6F,

        ["F1"] = 0x70,
        ["F2"] = 0x71,
        ["F3"] = 0x72,
        ["F4"] = 0x73,
        ["F5"] = 0x74,
        ["F6"] = 0x75,
        ["F7"] = 0x76,
        ["F8"] = 0x77,
        ["F9"] = 0x78,
        ["F10"] = 0x79,
        ["F11"] = 0x7A,
        ["F12"] = 0x7B,

        ["NumLock"] = 0x90,
        ["ScrollLock"] = 0x91,

        ["LShift"] = 0xA0,
        ["RShift"] = 0xA1,
        ["LControl"] = 0xA2,
        ["RControl"] = 0xA3,
        ["LAlt"] = 0xA4,
        ["RAlt"] = 0xA5,

        ["BrowserBack"] = 0xA6,
        ["BrowserForward"] = 0xA7,

        ["VolumeMute"] = 0xAD,
        ["VolumeDown"] = 0xAE,
        ["VolumeUp"] = 0xAF,

        ["MediaNext"] = 0xB0,
        ["MediaPrev"] = 0xB1,
        ["MediaStop"] = 0xB2,
        ["MediaPlayPause"] = 0xB3,

        ["LaunchMail"] = 0xB4
    };

    private static readonly IReadOnlyDictionary<int, string> KeyNames =
        KeyMap.ToDictionary(x => x.Value, x => x.Key);

    private static readonly Dictionary<int, long> KeyDownCache = new();

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out Point lpPoint);

    [DllImport("user32.dll")]
    private static extern IntPtr WindowFromPoint(Point point);

    [DllImport("user32.dll")]
    private static extern bool ScreenToClient(IntPtr hWnd, ref Point point);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(
        IntPtr hWnd,
        out uint processId);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr FindWindow(
        string? className,
        string? windowName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool ClientToScreen(
        IntPtr hWnd,
        ref Point point);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetClientRect(
        IntPtr hWnd,
        out Rect rect);

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int key);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int index);

    public static bool IsKeyDown(int key)
    {
        return (GetAsyncKeyState(key) & KEY_DOWN_MASK) != 0;
    }

    public static bool IsKeyDown(string key)
    {
        return KeyMap.TryGetValue(key, out var virtualKey) &&
               IsKeyDown(virtualKey);
    }

    public static string GetKeyName(int virtualKey)
    {
        return KeyNames.TryGetValue(virtualKey, out var name)
            ? name
            : $"0x{virtualKey:X2}";
    }

    public static bool WasKeyPressed(string key)
    {
        if (!KeyMap.TryGetValue(key, out var virtualKey))
            return false;

        if (!KeyDownCache.TryGetValue(virtualKey, out var time))
            return false;

        if (Environment.TickCount64 - time > 100)
        {
            KeyDownCache.Remove(virtualKey);
            return false;
        }

        KeyDownCache.Remove(virtualKey);
        return true;
    }

    public static void UpdateKeyDowns()
    {
        var now = Environment.TickCount64;

        foreach (var virtualKey in KeyMap.Values)
        {
            if (IsKeyDown(virtualKey))
            {
                KeyDownCache.TryAdd(virtualKey, now);
            }
            else
            {
                KeyDownCache.Remove(virtualKey);
            }
        }
    }

    public static Point GetCursorPosition(IntPtr windowHandle)
    {
        if (!GetCursorPos(out var position))
            return Point.Empty;

        var cursorWindow = WindowFromPoint(position);

        if (cursorWindow != windowHandle)
            return Point.Empty;

        return ScreenToClient(windowHandle, ref position)
            ? position
            : Point.Empty;
    }

    public static bool IsProcessActiveWindow(int processId)
    {
        var foregroundWindow = GetForegroundWindow();

        if (foregroundWindow == IntPtr.Zero)
            return false;

        GetWindowThreadProcessId(
            foregroundWindow,
            out var activeProcessId);

        return activeProcessId == processId;
    }

    public static Rectangle GetClientRectangle(IntPtr windowHandle)
    {
        if (!GetClientRect(windowHandle, out var rect))
            return Rectangle.Empty;

        var topLeft = new Point(rect.Left, rect.Top);

        if (!ClientToScreen(windowHandle, ref topLeft))
            return Rectangle.Empty;

        return new Rectangle(
            topLeft.X,
            topLeft.Y,
            rect.Right - rect.Left,
            rect.Bottom - rect.Top);
    }

    public static Rectangle GetClientRectangle(string windowTitle)
    {
        var handle = FindWindow(null, windowTitle);

        return handle == IntPtr.Zero
            ? Rectangle.Empty
            : GetClientRectangle(handle);
    }

    public static (int Width, int Height) GetScreenSize(string windowTitle)
    {
        var rectangle = GetClientRectangle(windowTitle);

        if (rectangle == Rectangle.Empty)
            throw new InvalidOperationException(
                $"Window '{windowTitle}' was not found or has an invalid client area.");

        return (rectangle.Width, rectangle.Height);
    }

    public static int GetScreenWidth()
    {
        return GetSystemMetrics(SM_CXSCREEN);
    }

    public static int GetScreenHeight()
    {
        return GetSystemMetrics(SM_CYSCREEN);
    }

    public static Vector2 GetViewport()
    {
        return new(GetScreenWidth(), GetScreenHeight());
    }

    public static Vector2 GetCenter(Vector2 size)
    {
        return new(
            (GetScreenWidth() - size.X) / 2f,
            (GetScreenHeight() - size.Y) / 2f);
    }

    public static void SetCursorPosition(int x, int y)
    {
        if (!SetCursorPos(x, y))
            throw new InvalidOperationException(
                "Failed to set cursor position.");
    }
}

[StructLayout(LayoutKind.Sequential)]
internal struct Rect
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}

