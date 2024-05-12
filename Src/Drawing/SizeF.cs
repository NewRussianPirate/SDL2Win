using System.Runtime.InteropServices;

namespace SDL2Win.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SizeF : IEquatable<SizeF>
    {
        public float Width;
        public float Height;

        public SizeF(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public bool Equals(SizeF other) => this == other;

        public override int GetHashCode() => HashCode.Combine(Width, Height);

        public static bool operator ==(SizeF left, SizeF right) => left.Width == right.Width && left.Height == right.Height;
        public static bool operator !=(SizeF left, SizeF right) => !(left == right);

        public static SizeF operator +(SizeF left, SizeF right) => new SizeF(left.Width + right.Width, left.Height + right.Height);
        public static SizeF operator -(SizeF left, SizeF right) => new SizeF(left.Width - right.Width, left.Height - right.Height);
        public static SizeF operator *(SizeF left, SizeF right) => new SizeF(left.Width * right.Width, left.Height * right.Height);
        public static SizeF operator /(SizeF left, SizeF right) => new SizeF(left.Width / right.Width, left.Height / right.Height);

        public static SizeF operator *(float num, SizeF s) => new SizeF(num * s.Width, num * s.Height);
        public static SizeF operator *(SizeF s, float num) => new SizeF(num * s.Width, num * s.Height);
        public static SizeF operator /(SizeF s, float num) => new SizeF(s.Width / num, s.Height / num);

        public static explicit operator SizeF(PointF p) => new SizeF(p.X, p.Y);

        public override bool Equals(object obj) => obj is SizeF && Equals((SizeF)obj);
    }
}
