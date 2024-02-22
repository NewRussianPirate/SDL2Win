using SDL2Win.Exceptions;
using SDL2Win.Drawing;
using SDL2;

namespace SDL2Win
{
    /// <summary>
    /// Icon for the main window. SDL_Image is required.
    /// </summary>
    public sealed class WinIcon : IDisposable
    {
        private bool _disposed = false;

        public string Path { get; private set; }

        internal SDLSurfaceHolder Surface { get; init; }

        public bool IsDisposed => _disposed;

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
                Path = null;
            Surface.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~WinIcon() => Dispose(false);

        public WinIcon(string path)
        {
            Path = path;
            Surface = new(SDL_image.IMG_Load(path));
            if (Surface.Ptr == IntPtr.Zero)
                throw new SDLException($"Failed to load an image, path: {path}. ");
        }
    }
}
