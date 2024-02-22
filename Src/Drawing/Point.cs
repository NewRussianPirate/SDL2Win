using System.Runtime.InteropServices;
using SDL2;

namespace SDL2Win.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Point : IEquatable<Point>
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Point other) => X == other.X && Y == other.Y;

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public static bool operator ==(Point left, Point right) => left.X == right.X && left.Y == right.Y;
        public static bool operator !=(Point left, Point right) => !(left == right);

        public static Point operator +(Point left, Point right) => new Point(left.X + right.X, left.Y + right.Y);
        public static Point operator -(Point left, Point right) => new Point(left.X - right.X, left.Y - right.Y);
        public static Point operator *(Point left, Point right) => new Point(left.X * right.X, left.Y * right.Y);
        public static Point operator /(Point left, Point right) => new Point(left.X / right.X, left.Y / right.Y);

        public static Point operator *(int num, Point p) => new Point(p.X * num, p.Y * num);
        public static Point operator *(Point p, int num) => new Point(p.X * num, p.Y * num);
        public static Point operator /(Point p, int num) => new Point(p.X / num, p.Y / num);

        public static implicit operator Point(SDL.SDL_Point p) => new Point(p.x, p.y);
        public static implicit operator SDL.SDL_Point(Point p) => new SDL.SDL_Point() { x = p.X, y = p.Y };

        public static explicit operator Point(PointF p) => new((int)p.X, (int)p.Y);

        public static explicit operator Point(Size s) => new Point(s.Width, s.Height);

        public override bool Equals(object obj) => obj is Point && Equals((Point)obj);
    }
}
