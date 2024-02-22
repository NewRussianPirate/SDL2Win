using SDL2Win.Drawing;
using SDL2Win.Exceptions;
using static SDL2.SDL;
using static SDL2.SDL_image;

namespace SDL2Win
{
    /// <summary>
    /// Colored app's cursor. SDL_Image is required.
    /// </summary>
    public sealed class Cursor : IDisposable
    {
        private bool _disposed = false;
        private IntPtr _sdlCursorHandle;
        private SDLSurfaceHolder _surface;

        public string Path { get; private set; }

        public Point HotSpot { get; init; }

        public IntPtr CursorHandle => _sdlCursorHandle;

        public bool IsDisposed => _disposed;

        private void Dispose(bool disposing)
        {
            if (_disposed)
                throw new ObjectDisposedException("Object already been disposed.");
            if (disposing)
                Path = null;
            if(_sdlCursorHandle != IntPtr.Zero)
                SDL_FreeCursor(_sdlCursorHandle);
            if(_surface.Ptr != IntPtr.Zero)
                _surface.Dispose();
            _disposed = true;
        }

        ~Cursor() => Dispose(false);

        public Cursor(string path, int hotSpotX, int hotSpotY)
        {
            if (path == null)
                throw new ArgumentNullException($"{path} is null.");
            if (path.Length == 0)
                throw new ArgumentException($"{Path} is empty.");
            var ptr = IMG_Load(path);
            if (ptr == IntPtr.Zero)
                throw new SDLException("Failed to load cursor image.");
            _surface = new(ptr);
            Path = path;
            HotSpot = new(hotSpotX, hotSpotY);
        }

        public Cursor(string path, Point cursorHotSpot) : this(path, cursorHotSpot.X, cursorHotSpot.Y) { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void SetCursor()
        {
            //This should be done AFTER window creation!
            _sdlCursorHandle = SDL_CreateColorCursor(_surface.Ptr, HotSpot.X, HotSpot.Y);
            if (_sdlCursorHandle == IntPtr.Zero)
            {
                Dispose();
                throw new SDLException("Failed to init cursor.");
            }
            SDL_SetCursor(_sdlCursorHandle);
        }
    }
}
