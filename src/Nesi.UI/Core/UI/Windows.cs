// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System.Numerics;
using ImGuiNET;
using Nesi.UI.Style;
using Nesi.UI.Utils;

namespace Nesi.UI;

/// <summary>Configures the appearance, transitions, and ImGui flags of a window.</summary>
public sealed class WindowStyle
{
    private float? _backgroundOpacity;
    private float? _borderRadius;

    /// <summary>Gets or sets additional ImGui window flags.</summary>
    public ImGuiWindowFlags Flags { get; set; }

    /// <summary>Duration of the fade-in after this window is shown. Set to zero to disable it.</summary>
    public float FadeInDuration { get; set; } = 0.18f;

    /// <summary>Duration of the fade-out before switching away from this window. Set to zero to disable it.</summary>
    public float FadeOutDuration { get; set; } = 0.14f;

    /// <summary>Gets or sets the window shadow strength; zero disables the shadow.</summary>
    public float Shadow { get; set; } = 0.4f;

    /// <summary>Gets or sets whether the framework draws the window outline.</summary>
    public bool Border { get; set; } = true;

    /// <summary>Gets or sets the rounding used by window background decorations.</summary>
    public float BorderRadius
    {
        get => _borderRadius ?? 2f;
        set => _borderRadius = value;
    }

    /// <summary>Gets or sets the opacity applied to the window background.</summary>
    public float BackgroundOpacity
    {
        get => _backgroundOpacity ?? 1f;
        set => _backgroundOpacity = value;
    }

    /// <summary>Gets or sets the background effect rendered behind the window content.</summary>
    public BackgroundType Background { get; set; } = BackgroundType.None;
}

/// <summary>Contract for a window managed and rendered by <see cref="WindowManager"/>.</summary>
public interface IWindow : IDisposable
{
    /// <summary>Gets the window's identifier.</summary>
    string Id { get; }

    /// <summary>Gets or sets whether this window participates in rendering.</summary>
    bool IsVisible { get; set; }

    /// <summary>Gets whether this window is modal.</summary>
    bool IsModal { get; }

    /// <summary>Gets or sets this window's visual style and ImGui flags.</summary>
    WindowStyle Style { get; set; }

    /// <summary>Renders the window content.</summary>
    void Render();

    /// <summary>Called when the window is selected.</summary>
    void OnShow();

    /// <summary>Called when the window is hidden or replaced.</summary>
    void OnHide();

    /// <summary>Updates per-frame window state before rendering.</summary>
    void Update();
}

/// <summary>Base implementation for windows managed by <see cref="WindowManager"/>.</summary>
public abstract class BaseWindow : IWindow
{
    protected bool _disposed;
    protected bool _isVisible = true;
    protected Vector2 _size = new(400, 350);
    /// <summary>Creates a window with the supplied identifier or its type name.</summary>
    /// <param name="id">Optional stable identifier.</param>
    protected BaseWindow(string? id = null) => Id = id ?? GetType().Name;

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public bool IsVisible { get => _isVisible; set => _isVisible = value; }

    /// <inheritdoc />
    public virtual bool IsModal => false;

    /// <inheritdoc />
    public WindowStyle Style { get; set; } = new();
    /// <summary>
    /// Current framework transition opacity. Multiply explicit draw-list color alpha by this value
    /// when custom drawing should participate in window transitions.
    /// </summary>
    public float TransitionAlpha { get; internal set; } = 1f;

    /// <summary>Renders the content when <see cref="IsVisible"/> is true.</summary>
    public virtual void Render() { if (IsVisible) RenderContent(); }

    /// <summary>Implements the visual contents of this window.</summary>
    protected abstract void RenderContent();

    /// <inheritdoc />
    public virtual void OnShow() => IsVisible = true;

    /// <inheritdoc />
    public virtual void OnHide() => IsVisible = false;

    /// <inheritdoc />
    public virtual void Update() { }

    /// <inheritdoc />
    public virtual void Dispose() => _disposed = true;

    /// <summary>Gets the initial ImGui window size in pixels.</summary>
    public virtual Vector2 GetWindowSize() => _size;
}

/// <summary>Registers, selects, transitions, and renders application windows.</summary>
public sealed class WindowManager : IDisposable
{
    private readonly Dictionary<string, IWindow> _windows = new();
    private IWindow? _current;
    private IWindow? _previous;
    private bool _positionInitialized;
    private bool _sizeInitialized;
    private IWindow? _pendingWindow;
    private bool _isFadingIn;
    private bool _isFadingOut;
    private float _transitionAlpha = 1f;
    private float _transitionStartAlpha;
    private float _transitionElapsed;
    private float _transitionDuration;
    /// <summary>Gets the currently selected window, if one is active.</summary>
    public IWindow? CurrentWindow => _current;
    private ImGuiWindowFlags _defaultFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoSavedSettings;

    public WindowManager() { }
    /// <summary>Registers a window and optionally selects it as the initial window.</summary>
    /// <param name="key">A unique key used for later navigation.</param>
    /// <param name="window">The window instance to register.</param>
    /// <param name="isMain">When <see langword="true"/>, selects this window immediately.</param>
    public void RegisterWindow(string key, IWindow window, bool isMain = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(window);
        _windows[key] = window;

        if (isMain || key.ToLower() == "main")
            SwitchTo(key);
    }
    /// <summary>Selects a registered window, fading between windows according to their styles.</summary>
    /// <param name="key">The key passed to <see cref="RegisterWindow"/>.</param>
    /// <returns><see langword="true"/> if the key identifies a registered window.</returns>
    public bool SwitchTo(string key)
    {
        if (!_windows.TryGetValue(key, out var next)) return false;
        return RequestSwitch(next);
    }
    /// <summary>Checks whether the current window is of the specified type.</summary>
    /// <typeparam name="T">The window type to check.</typeparam>
    /// <returns><see langword="true"/> if the current window is of type <typeparamref name="T"/>.</returns>
    public bool IsCurrentWindow<T>() where T : IWindow => _current is T;

    /// <summary>Returns to the previously selected window, when available.</summary>
    /// <returns><see langword="true"/> if a previous window was selected.</returns>
    public bool GoBack()
    {
        if (_previous is null)
            return false;

        return RequestSwitch(_previous);
    }

    private bool RequestSwitch(IWindow next)
    {
        if (ReferenceEquals(_current, next))
        {
            if (_pendingWindow is not null)
            {
                _pendingWindow = null;
                StartFadeInFromCurrent();
            }

            return true;
        }

        if (_current is null || !_current.IsVisible)
        {
            CommitWindow(next);
            return true;
        }

        if (ReferenceEquals(_pendingWindow, next))
            return true;

        _pendingWindow = next;
        var duration = Math.Max(0f, _current.Style.FadeOutDuration);
        if (duration == 0f)
        {
            CommitWindow(next);
            return true;
        }

        _isFadingIn = false;
        _isFadingOut = true;
        _transitionStartAlpha = GetCurrentTransitionAlpha();
        _transitionAlpha = _transitionStartAlpha;
        _transitionElapsed = 0f;
        _transitionDuration = duration;
        SetCurrentTransitionAlpha(_transitionAlpha);
        return true;
    }

    private void CommitWindow(IWindow next)
    {
        var previous = _current;
        previous?.OnHide();
        _previous = previous;
        _current = next;
        _current.OnShow();
        _positionInitialized = false;
        _sizeInitialized = false;
        _pendingWindow = null;

        var duration = Math.Max(0f, _current.Style.FadeInDuration);
        _isFadingOut = false;
        _isFadingIn = duration > 0f;
        _transitionAlpha = _isFadingIn ? 0f : 1f;
        _transitionStartAlpha = _transitionAlpha;
        _transitionElapsed = 0f;
        _transitionDuration = duration;
        SetCurrentTransitionAlpha(_transitionAlpha);
    }

    private void StartFadeInFromCurrent()
    {
        if (_current is null)
            return;

        var duration = Math.Max(0f, _current.Style.FadeInDuration);
        _isFadingOut = false;
        _isFadingIn = duration > 0f && GetCurrentTransitionAlpha() < 1f;
        _transitionStartAlpha = GetCurrentTransitionAlpha();
        _transitionAlpha = _transitionStartAlpha;
        _transitionElapsed = 0f;
        _transitionDuration = duration;
    }

    private void AdvanceTransition()
    {
        if (!_isFadingIn && !_isFadingOut)
            return;

        _transitionElapsed = Math.Min(
            _transitionElapsed + Math.Max(0f, ImGui.GetIO().DeltaTime),
            _transitionDuration
        );
        var progress = _transitionDuration <= 0f
            ? 1f
            : _transitionElapsed / _transitionDuration;

        if (_isFadingOut)
        {
            _transitionAlpha = Animation.Lerp(
                _transitionStartAlpha,
                0f,
                Animation.EaseInOutQuad(progress)
            );
            SetCurrentTransitionAlpha(_transitionAlpha);

            if (progress >= 1f && _pendingWindow is { } next)
                CommitWindow(next);

            return;
        }

        _transitionAlpha = Animation.Lerp(
            _transitionStartAlpha,
            1f,
            Animation.EaseOutQuad(progress)
        );
        SetCurrentTransitionAlpha(_transitionAlpha);

        if (progress >= 1f)
        {
            _transitionAlpha = 1f;
            SetCurrentTransitionAlpha(_transitionAlpha);
            _isFadingIn = false;
        }
    }

    private float GetCurrentTransitionAlpha() =>
        _current is BaseWindow window ? window.TransitionAlpha : _transitionAlpha;

    private void SetCurrentTransitionAlpha(float alpha)
    {
        _transitionAlpha = Math.Clamp(alpha, 0f, 1f);
        if (_current is BaseWindow window)
            window.TransitionAlpha = _transitionAlpha;
    }

    public void Render()
    {
        if (_current is null) return;
        AdvanceTransition();
        if (_current is null || !_current.IsVisible) return;

        _current.Update();
        var size = (_current as BaseWindow)?.GetWindowSize() ?? new(400, 350);
        if (!_positionInitialized)
        {
            var viewport = ImGui.GetMainViewport();
            var position = viewport.WorkPos + (viewport.WorkSize - size) * 0.5f;
            ImGui.SetNextWindowPos(position, ImGuiCond.Always);
            _positionInitialized = true;
        }
        if (!_sizeInitialized)
        {
            ImGui.SetNextWindowSize(size, ImGuiCond.Always);
            _sizeInitialized = true;
        }
        var style = _current.Style;
        var transitionAlpha = Math.Clamp(GetCurrentTransitionAlpha(), 0f, 1f);
        var backgroundOpacity = Math.Clamp(style.BackgroundOpacity, 0f, 1f);

        ImGui.SetNextWindowBgAlpha(
            (style.Background == BackgroundType.None ? backgroundOpacity : 0f) * transitionAlpha
        );

        ImGui.PushStyleVar(
            ImGuiStyleVar.Alpha,
            ImGui.GetStyle().Alpha * transitionAlpha
        );
        ImGui.PushStyleVar(
            ImGuiStyleVar.WindowBorderSize,
            Math.Max(0f, 0f)
        );

        ImGui.PushStyleVar(
            ImGuiStyleVar.WindowRounding,
            Math.Max(0f, style.BorderRadius)
        );

        var open = true;

        try
        {
            if (ImGui.Begin("##NesiWindow", ref open, _defaultFlags | style.Flags))
            {
                if (style.Background != BackgroundType.None)
                {
                    Background.Render(
                        ImGui.GetWindowDrawList(),
                        ImGui.GetWindowPos(),
                        ImGui.GetWindowSize(),
                        style.BorderRadius,
                        backgroundOpacity * transitionAlpha,
                        style.Background
                    );
                }

                _current.Render();
            }

            if (style.Shadow > 0f)
            {
                Shadow.Draw(
                    ImGui.GetWindowPos(),
                    ImGui.GetWindowSize(),
                    ImGui.GetBackgroundDrawList(),
                    style.Shadow * style.BackgroundOpacity * transitionAlpha
                );
            }

            if (style.Border)
            {
                ImGui.GetForegroundDrawList().AddRect(
                    ImGui.GetWindowPos(),
                    new Vector2(ImGui.GetWindowPos().X + ImGui.GetWindowSize().X, ImGui.GetWindowPos().Y + ImGui.GetWindowSize().Y),
                    ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, style.BackgroundOpacity * transitionAlpha * 0.05f)),
                    style.BorderRadius
                );
            }
        }
        finally
        {
            ImGui.End();
            ImGui.PopStyleVar(3);
        }

        if (!open) _current.IsVisible = false;
    }

    /// <summary>Releases all registered windows.</summary>
    public void Dispose()
    {
        foreach (var window in _windows.Values.Distinct())
            window.Dispose();

        _windows.Clear();
        _current = null;
        _previous = null;
        _pendingWindow = null;
    }
}
