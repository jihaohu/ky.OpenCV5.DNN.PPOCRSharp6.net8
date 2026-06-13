using System.Text;

namespace ky.OpenCV5.DNN.PPOCRSharp6
{
    public sealed class NativeOcrEngine : IDisposable
    {
        public IntPtr Handle { get; private set; }
        private readonly NativeOcrApi.DestroyDelegate _destroy;

        // 传统构造函数，兼容低版本 C#
        public NativeOcrEngine(IntPtr handle, NativeOcrApi.DestroyDelegate destroy)
        {
            Handle = handle;
            _destroy = destroy;
        }

        public void Dispose()
        {
            if (Handle == IntPtr.Zero)
            {
                return;
            }

            StringBuilder msg = new StringBuilder(128);
            _destroy(Handle, msg);
            Handle = IntPtr.Zero;
        }
    }
}