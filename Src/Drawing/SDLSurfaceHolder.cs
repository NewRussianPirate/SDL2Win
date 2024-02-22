using SDL2Win.Utility;
using System.Runtime.CompilerServices;
using static SDL2.SDL;

namespace SDL2Win.Drawing
{
    /// <summary>
    /// Contains a collection of pixels used in software blitting. Represents SDL_Surface.
    /// </summary>
    internal struct SDLSurfaceHolder : IUnmanagedPointer<SDL_Surface>, IDisposable
    {
        private bool _isDisposed = false;
        private UnmanagedPointer<SDL_Surface> _pSurface;

        public bool IsDisposed => _isDisposed;

        public IntPtr Ptr => _pSurface.Ptr;

        public ref SDL_Surface Ref => ref _pSurface.Ref;

        public unsafe ref readonly SDL_PixelFormat PixelFormat
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Unsafe.AsRef<SDL_PixelFormat>(_pSurface.Ref.format.ToPointer());
        }

        public SDLSurfaceHolder(IntPtr sdlSurface) => _pSurface = new(sdlSurface);

        public void Dispose()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Object already been disposed.");
            SDL_FreeSurface(Ptr);
            _isDisposed = true;
        }
    }
}
