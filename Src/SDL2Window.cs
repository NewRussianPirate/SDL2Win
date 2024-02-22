using SDL2Win.Drawing;
using SDL2Win.Exceptions;
using System.Runtime.CompilerServices;

using static SDL2.SDL;

namespace SDL2Win
{
    /// <summary>
    /// Identical to <see cref="SDL_WindowFlags"/>
    /// </summary>
    [Flags]
    public enum WindowFlags
    {
        Fullscreen = 0x00000001,
        OpenGL = 0x00000002,
        Shown = 0x00000004,
        Hidden = 0x00000008,
        Borderless = 0x00000010,
        Resizable = 0x00000020,
        Minimized = 0x00000040,
        Maximized = 0x00000080,
        MouseGrabbed = 0x00000100,
        InputFocus = 0x00000200,
        MouseFocus = 0x00000400,
        FullscreenDesktop =
            Fullscreen | 0x00001000,
        Foreign = 0x00000800,
        AllowHighDpi = 0x00002000, /* Requires >= 2.0.1 */
        MouseCapture = 0x00004000, /* Requires >= 2.0.4 */
        AlwaysOnTop = 0x00008000, /* Requires >= 2.0.5 */
        SkipTaskBar = 0x00010000,  /* Requires >= 2.0.5 */
        Utility = 0x00020000,   /* Requires >= 2.0.5 */
        Tooltip = 0x00040000,   /* Requires >= 2.0.5 */
        Popup_Menu = 0x00080000,    /* Requires >= 2.0.5 */
        KeyboardGrabbed = 0x00100000,  /* Requires >= 2.0.16 */
        Vulkan = 0x10000000,    /* Requires >= 2.0.6 */
        Metal = 0x2000000,  /* Requires >= 2.0.14 */

        InputGrabbed =
            MouseGrabbed,
    }

    public enum WindowState
    {
        Show,
        Hidden,
        Exposed,
        Minimased,
        Maximased,
        Restored
    }

    public enum SizeEventType : byte
    {
        /// <summary>
        /// Window has been moved to a new position.
        /// </summary>
        Changed,
        /// <summary>
        /// Window size has changed, either as a result of an API call or through the system or user changing the window size; 
        /// this event is followed by a <see cref="Changed"/> state, if the size was changed by an external event, i.e. the user or the window manager.
        /// </summary>
        Resized
    }

    public enum MouseFocus : byte
    {
        Gained,
        Lost
    }

    public enum KeyboardFocus : byte
    {
        Gained,
        Lost
    }

    public readonly struct ResizeInfo
    {
        public readonly SizeEventType Type;
        public readonly Size NewSize;

        public ResizeInfo(SizeEventType type, Size newSize)
        {
            Type = type;
            NewSize = newSize;
        }
    }

    public abstract class SDL2Window : IDisposable
    {
        protected bool _disposed = false;
        private bool _alwaysOnTop = false;

        //Keep title name here instead of using SDL_GetWindowTitle(...) to avoid additional native UTF-8 convertion
        private string _title;

        private Cursor _cursor;

        private WinIcon _icon;

        public const int WINDOWPOS_CENTER = 0x2FFF0000;

        public IntPtr WindowHandle { get; init; }

        public WinIcon Icon
        {
            get => _icon;
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value is null");
                else
                    SDL_SetWindowIcon(WindowHandle, value.Surface.Ptr);
                _icon = value;
            }
        }

        public Cursor Cursor
        {
            get => _cursor;
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value is null");
                else
                    value.SetCursor();
                _cursor = value;
            }
        }

        public WindowState State { get; private set; }

        public delegate void OnCloseEvent(SDL2Window sender);
        public delegate void OnFocusOfferedEvent(SDL2Window sender);

        public event EventHandler<WindowState> StateChanged;
        public event EventHandler<Point> PositionChanged;
        public event EventHandler<ResizeInfo> SizeChanged;
        public event EventHandler<MouseFocus> MouseFocusChanged;
        public event EventHandler<KeyboardFocus> KeyboardFocusChanged;
        /// <summary>
        /// Event fired when the window manager requests that the window be closed.
        /// </summary>
        public event OnCloseEvent Close;
        public event OnFocusOfferedEvent FocusOffered;
        /// <summary>
        /// The window has been moved to another display. Arg is display index.
        /// </summary>
        public event EventHandler<int> DisplayChanged;

        public WindowFlags Flags => (WindowFlags)SDL_GetWindowFlags(WindowHandle);

        public string Title
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _title;
            }

            set
            {
                SDL_SetWindowTitle(WindowHandle, value);
                _title = value;
            }
        }

        public int DisplayIndex
        {
            get
            {
                var res = SDL_GetWindowDisplayIndex(WindowHandle);
                SDLException.ThrowHelper.CheckException(res);
                return res;
            }
        }

        public Size Size
        {
            get
            {
                SDL_GetWindowSize(WindowHandle, out int w, out int h);
                return new Size(w, h);
            }
            set => SDL_SetWindowSize(WindowHandle, value.Width, value.Height);
        }

        public Size MinSize
        {
            get
            {
                SDL_GetWindowMinimumSize(WindowHandle, out int w, out int h);
                return new Size(w, h);
            }
            set => SDL_SetWindowMinimumSize(WindowHandle, value.Width, value.Height);
        }

        public Size MaxSize
        {
            get
            {
                SDL_GetWindowMaximumSize(WindowHandle, out int w, out int h);
                return new Size(w, h);
            }
            set => SDL_SetWindowMaximumSize(WindowHandle, value.Width, value.Height);
        }

        public float Opacity
        {
            get
            {
                SDL_GetWindowOpacity(WindowHandle, out float res);
                return res;
            }
            set => SDL_SetWindowOpacity(WindowHandle, value);
        }

        public bool AlwaysOnTop
        {
            get => _alwaysOnTop;
            set
            {
                if (_alwaysOnTop == value)
                    return;
                SDL_SetWindowAlwaysOnTop(WindowHandle, BoolToSdlBool(value));
                _alwaysOnTop = value;
            }
        }

        public bool IsMouseFocused => SDL_GetMouseFocus() == WindowHandle;

        public Point Position
        {
            get
            {
                SDL_GetWindowPosition(WindowHandle, out int x, out int y);
                return new Point(x, y);
            }
            set => SDL_SetWindowPosition(WindowHandle, value.X, value.Y);
        }

        /// <summary>
        /// Numeric ID of the current window. 
        /// </summary>
        public uint ID
        {
            get
            {
                uint res = SDL_GetWindowID(WindowHandle);
                if (res == 0)
                    throw new SDLException($"Failed to get ID for next window: 0x{WindowHandle:X}.");
                return res;
            }
        }

        /// <summary>
        /// Handle the <see cref="SDL_Event"/> where <seealso cref="SDL_EventType"/> == <seealso cref="SDL_EventType.SDL_WINDOWEVENT"/>
        /// </summary>
        /// <param name="e"></param>
        protected void OnWinSDLEvent(in SDL_Event e)
        {
            if (e.type != SDL_EventType.SDL_WINDOWEVENT)
                return;
            switch (e.window.windowEvent)
            {
                case SDL_WindowEventID.SDL_WINDOWEVENT_SHOWN:
                {
                    State = WindowState.Show;
                    StateChanged?.Invoke(this, State);
                    break;
                }

                case SDL_WindowEventID.SDL_WINDOWEVENT_HIDDEN:
                {
                    State = WindowState.Hidden;
                    StateChanged?.Invoke(this, State);
                    break;
                }

                case SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED:
                {
                    State = WindowState.Exposed;
                    StateChanged?.Invoke(this, State);
                    break;
                }

                case SDL_WindowEventID.SDL_WINDOWEVENT_MOVED:
                {
                    PositionChanged?.Invoke(this, new Point(e.window.data1, e.window.data2));
                    break;
                }

                case SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                {
                    SizeChanged?.Invoke(this, new ResizeInfo(SizeEventType.Resized, new Size(e.window.data1, e.window.data2)));
                    break;
                }

                case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                {
                    SizeChanged?.Invoke(this, new ResizeInfo(SizeEventType.Changed, new Size(e.window.data1, e.window.data2)));
                    break;
                }

                case SDL_WindowEventID.SDL_WINDOWEVENT_MINIMIZED:
                {
                    State = WindowState.Minimased;
                    StateChanged?.Invoke(this, State);
                    break;
                }

                case SDL_WindowEventID.SDL_WINDOWEVENT_MAXIMIZED:
                {
                    State = WindowState.Maximased;
                    StateChanged?.Invoke(this, State);
                    break;
                }

                case SDL_WindowEventID.SDL_WINDOWEVENT_RESTORED:
                {
                    State = WindowState.Restored;
                    StateChanged?.Invoke(this, State);
                    break;
                }

                case SDL_WindowEventID.SDL_WINDOWEVENT_ENTER:
                {
                    MouseFocusChanged?.Invoke(this, MouseFocus.Gained);
                    break;
                }

                case SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE:
                {
                    MouseFocusChanged?.Invoke(this, MouseFocus.Lost);
                    break;
                }

                case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                {
                    KeyboardFocusChanged?.Invoke(this, KeyboardFocus.Gained);
                    break;
                }

                case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                {
                    KeyboardFocusChanged?.Invoke(this, KeyboardFocus.Lost);
                    break;
                }

                case SDL_WindowEventID.SDL_WINDOWEVENT_TAKE_FOCUS:
                {
                    FocusOffered?.Invoke(this);
                    break;
                }

                case SDL_WindowEventID.SDL_WINDOWEVENT_DISPLAY_CHANGED:
                {
                    DisplayChanged?.Invoke(this, e.window.data1);
                    break;
                }

                case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                {
                    Close?.Invoke(this);
                    break;
                }
            }
        }

        protected void Dispose(bool disposing)
        {
            DisposeCheck();
            SDL_DestroyWindow(WindowHandle);
            if (disposing)
            {
                //-----Events------//
                StateChanged = null;
                PositionChanged = null;
                SizeChanged = null;
                MouseFocusChanged = null;
                KeyboardFocusChanged = null;
                Close = null;
                FocusOffered = null;
                DisplayChanged = null;
                //-----------------//

                _title = null;
                //Window may not be an owner of icon, so no dispose call.
                _icon = null;
                //Same for the cursor.
                _cursor = null;
            }
            _disposed = true;
        }

        /// <summary>
        /// Close the window and destroy sdl context; Do not call this manually, use <see cref="Application.Dispose"/> instead.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SDL2Window() => Dispose(false);

        protected SDL2Window(string title, Point pos, Size size, WindowFlags winFlags, WinIcon icon = null, Cursor cursor = null)
        {
            WindowHandle = SDL_CreateWindow(title, pos.X, pos.Y, size.Width, size.Height, (SDL_WindowFlags)winFlags);
            if (WindowHandle == IntPtr.Zero)
                throw new SDLException("Failed to create window; SDL Error:" + SDL_GetError());

            if (icon != null)
                SDL_SetWindowIcon(WindowHandle, icon.Surface.Ptr);
            _icon = icon;

            if (cursor != null)
                cursor.SetCursor();
            _cursor = cursor;
        }

        /// <summary>
        /// Get <see cref="WindowHandle"/>.
        /// </summary>
        /// <param name="window"></param>
        public static implicit operator IntPtr(SDL2Window window) => window.WindowHandle;

        public static unsafe explicit operator void*(SDL2Window window) => window.WindowHandle.ToPointer();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SDL_bool BoolToSdlBool(bool value)
        {
            if (!value)
                return SDL_bool.SDL_FALSE;
            return SDL_bool.SDL_TRUE;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void DisposeCheck()
        {
            if (_disposed)
                throw new ObjectDisposedException("Window has been disposed.");
        }
    }
}