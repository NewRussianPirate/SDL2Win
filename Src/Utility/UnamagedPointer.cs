using System.Runtime.CompilerServices;

namespace SDL2Win.Utility
{
    internal struct UnmanagedPointer<T> : IUnmanagedPointer<T> where T : unmanaged
    {
        public IntPtr Ptr { get; set; }

        public unsafe ref T Ref
        {
            //There is a question to inlining this thing with Unsafe.AsRef call... (0x14 code size)
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Unsafe.AsRef<T>(Ptr.ToPointer());
        }

        public UnmanagedPointer(IntPtr ptr) => Ptr = ptr;

        public unsafe UnmanagedPointer(T* ptr) => Ptr = (IntPtr)ptr;
    }

    internal readonly struct ReadOnlyUnmanagedPointer<T> : IReadOnlyUnmanagedPointer<T> where T : unmanaged
    {
        private readonly IntPtr _ptr;
        public readonly IntPtr Ptr
        {
            get => _ptr;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            init
            {
                if(value == IntPtr.Zero)
                    throw new ArgumentNullException($"nameof(value) is null.");
                _ptr = value;
            }
        }

        public unsafe ref readonly T Ref => ref *(T*)Ptr.ToPointer();


        public ReadOnlyUnmanagedPointer(IntPtr ptr) => Ptr = ptr;
    }

    internal static class UnmanagedPointerExctensions
    {
        public static unsafe T* AsPtr<T>(this UnmanagedPointer<T> ptr) where T : unmanaged => (T*)ptr.Ptr.ToPointer();
        public static unsafe T* AsPtr<T>(this ReadOnlyUnmanagedPointer<T> ptr) where T : unmanaged => (T*)ptr.Ptr.ToPointer();
    }
}
