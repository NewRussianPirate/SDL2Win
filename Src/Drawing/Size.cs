using System.Runtime.InteropServices;

namespace SDL2Win.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Size : IEquatable<Size>
    {
        public int Width;
        public int Height;

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public bool Equals(Size other) => this == other;

        public override int GetHashCode() => HashCode.Combine(Width, Height);

        public static bool operator ==(Size left, Size right) => left.Width == right.Width && left.Height == right.Height;
        public static bool operator !=(Size left, Size right) => !(left == right);

        public static Size operator +(Size left, Size right) => new Size(left.Width + right.Width, left.Height + right.Height);
        public static Size operator -(Size left, Size right) => new Size(left.Width - right.Width, left.Height - right.Height);
        public static Size operator *(Size left, Size right) => new Size(left.Width * right.Width, left.Height * right.Height);
        public static Size operator /(Size left, Size right) => new Size(left.Width / right.Width, left.Height / right.Height);

        public static Size operator *(int num, Size s) => new Size(num * s.Width, num * s.Height);
        public static Size operator *(Size s, int num) => new Size(num * s.Width, num * s.Height);
        public static Size operator /(Size s, int num) => new Size(s.Width / num, s.Height / num);

        public static explicit operator Size(Point p) => new Size(p.X, p.Y);

        public override bool Equals(object obj) => obj is Size && Equals((Size)obj);
    }
}
