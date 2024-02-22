namespace SDL2Win.Utility
{
    internal interface IUnmanagedPointer
    {
        public IntPtr Ptr { get; }
    }

    internal interface IUnmanagedPointer<T> : IUnmanagedPointer where T : unmanaged
    {

        public ref T Ref { get; }
    }

    internal interface IReadOnlyUnmanagedPointer<T> : IUnmanagedPointer where T : unmanaged
    {
        public ref readonly T Ref { get; }
    }

    internal static class IUnmanagedPointerExtensions
    {
        public static unsafe T* AsPtr<T>(this IUnmanagedPointer<T> ptr) where T : unmanaged => (T*)ptr.Ptr.ToPointer();
        public static unsafe T* AsPtr<T>(this IReadOnlyUnmanagedPointer<T> ptr) where T : unmanaged => (T*)ptr.Ptr.ToPointer();
    }
}
