using SDL2Win;
using SDL2Win.Drawing;
using System.Runtime.CompilerServices;

using static SDL2.SDL;

namespace SDLWindowConsole
{
    sealed class SDLWin : SDL2Window
    {
        public SDLWin(string title, Point pos, Size size, WindowFlags winFlags, WinIcon icon = null, Cursor cursor = null) :
            base(title, pos, size, winFlags, icon, cursor)
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void HandleWinEvent(in SDL_Event e) => OnWinSDLEvent(in e);
    }

    internal class Program
    {
        static bool _running = true;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            SDLWin window = new("Window", new(SDL2Window.WINDOWPOS_CENTER, SDL2Window.WINDOWPOS_CENTER), new Size(640, 480), WindowFlags.Shown | WindowFlags.Resizable);
            window.SizeChanged += (s, e) => Console.WriteLine($"New size: Width = {e.NewSize.Width}; Height = {e.NewSize.Height}");
            window.Close += (s) =>
            {
                Console.WriteLine("Closing the window...");
                _running = false;
            };
            while(_running)
            {
                while(SDL_PollEvent(out SDL_Event e) != 0)
                {
                    window.HandleWinEvent(in e);
                }
                Thread.Sleep(50);
            }
				window.Dispose();
            SDL_Quit();
        }
    }
}