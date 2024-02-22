using SDL2;
using System.Runtime.InteropServices;

namespace SDL2Win.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PointF : IEquatable<PointF>
    {
        public float X;
        public float Y;

        public PointF(float x, float y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(PointF other) => X == other.X && Y == other.Y;

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public static bool operator ==(PointF left, PointF right) => left.X == right.X && left.Y == right.Y;
        public static bool operator !=(PointF left, PointF right) => !(left == right);

        public static PointF operator +(PointF left, PointF right) => new PointF(left.X + right.X, left.Y + right.Y);
        public static PointF operator -(PointF left, PointF right) => new PointF(left.X - right.X, left.Y - right.Y);
        public static PointF operator *(PointF left, PointF right) => new PointF(left.X * right.X, left.Y * right.Y);
        public static PointF operator /(PointF left, PointF right) => new PointF(left.X / right.X, left.Y / right.Y);

        public static PointF operator *(float num, PointF p) => new PointF(p.X * num, p.Y * num);
        public static PointF operator *(PointF p, float num) => new PointF(p.X * num, p.Y * num);
        public static PointF operator /(PointF p, float num) => new PointF(p.X / num, p.Y / num);

        public static implicit operator PointF(SDL.SDL_FPoint p) => new PointF(p.x, p.y);
        public static implicit operator SDL.SDL_FPoint(PointF p) => new SDL.SDL_FPoint() { x = p.X, y = p.Y };

        public static explicit operator PointF(Point p) => new PointF(p.X, p.Y);

        public static explicit operator PointF(Size s) => new PointF(s.Width, s.Height);

        public override bool Equals(object obj) => obj is PointF && Equals((PointF)obj);
    }
}
