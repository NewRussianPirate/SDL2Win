using SDL2;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SDL2Win.Exceptions
{
    public class SDLException : Exception
    {
        public static class ThrowHelper
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void CheckException(int err)
            {
                if (err != 0)
                    throw new SDLException();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void CheckException(int err, string exc)
            {
                if (err != 0)
                    throw new SDLException(exc);
            }
        }

        public SDLException() : base(SDL.SDL_GetError()) { }
        public SDLException(IntPtr c8SdlError) : base(Marshal.PtrToStringAnsi(c8SdlError)) { }

        public unsafe SDLException(string message, IntPtr c8SdlError) : base(CombineMsg(message, c8SdlError)) { }

        public SDLException(string message) : base(message + " SDL error: " + SDL.SDL_GetError()) { }

        private unsafe static string CombineMsg(string message, IntPtr c8SdlError)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (c8SdlError == IntPtr.Zero)
                return message;
            int c8Length = 0;
            byte* sym = (byte*)c8SdlError.ToPointer();

            //Get length of c8 string
            while (*sym != '\0')
            {
                c8Length++;
                sym++;
            }

            StringBuilder sb = new(message.Length + c8Length + 10);
            sb.Append(message + "SdlError: ");

            sym = (byte*)c8SdlError.ToPointer();

            while (*sym != '\0')
            {
                sb.Append(*sym);
                sym++;
            }
            return sb.ToString();
        }
    }
}
