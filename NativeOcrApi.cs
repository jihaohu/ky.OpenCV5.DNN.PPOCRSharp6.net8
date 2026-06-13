using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ky.OpenCV5.DNN.PPOCRSharp6
{


    /// <summary>
    /// Native 返回的完整 OCR 响应：results 是文本框数组，timing 是分段耗时。
    /// </summary>
    public sealed record OcrResponse(
        [property: JsonPropertyName("results")] IReadOnlyList<OcrResult> Results,
        [property: JsonPropertyName("timing")] OcrTiming Timing);

    /// <summary>
    /// 单条 OCR 文本结果，坐标顺序为左上、右上、右下、左下。
    /// </summary>
    public sealed record OcrResult(
        [property: JsonPropertyName("text")] string Text,
        [property: JsonPropertyName("score")] float Score,
        [property: JsonPropertyName("x1")] int X1,
        [property: JsonPropertyName("y1")] int Y1,
        [property: JsonPropertyName("x2")] int X2,
        [property: JsonPropertyName("y2")] int Y2,
        [property: JsonPropertyName("x3")] int X3,
        [property: JsonPropertyName("y3")] int Y3,
        [property: JsonPropertyName("x4")] int X4,
        [property: JsonPropertyName("y4")] int Y4);

    /// <summary>
    /// Native 内部分段耗时，单位毫秒。det=检测，cls=方向分类，rec=识别，total=总耗时。
    /// </summary>
    public sealed record OcrTiming(
        [property: JsonPropertyName("detMs")] double DetMs,
        [property: JsonPropertyName("clsMs")] double ClsMs,
        [property: JsonPropertyName("recMs")] double RecMs,
        [property: JsonPropertyName("totalMs")] double TotalMs);

    /// <summary>
    /// System.Text.Json 源生成上下文，减少运行时反射开销
    /// </summary>
    [JsonSerializable(typeof(OcrResponse))]
    public sealed partial class JsonContext : JsonSerializerContext
    {
        // 源生成器会自动补全分部类代码，此处无需手写内容
    }


    public sealed class NativeOcrApi : IDisposable
    {
        private readonly IntPtr libraryHandle;
        private readonly InitDelegate init;
        private readonly OcrWithTimingDelegate ocrWithTiming;
        private readonly FreeResultDelegate freeResult;
        private readonly DestroyDelegate destroy;
        private readonly DetectOrientationDelegate detectOrientation;

        private NativeOcrApi(IntPtr libraryHandle, string libraryPath)
        {
            this.libraryHandle = libraryHandle;
            LibraryPath = libraryPath;
            init = GetDelegate<InitDelegate>("init");
            ocrWithTiming = GetDelegate<OcrWithTimingDelegate>("ocr2_with_timing");
            freeResult = GetDelegate<FreeResultDelegate>("free_ocr_result");
            destroy = GetDelegate<DestroyDelegate>("destroy");
            detectOrientation = GetDelegate<DetectOrientationDelegate>("detect_orientation");
        }

        public string LibraryPath { get; }

        public static NativeOcrApi Load(string? explicitPath)
        {
            string path = explicitPath ?? ResolveDefaultLibraryPath();
            return new NativeOcrApi(NativeLibrary.Load(path), path);
        }

        public NativeOcrEngine CreateEngine(OcrOptions options)
        {
            StringBuilder msg = new(128);
            IntPtr engine = IntPtr.Zero;
            int ret = init(ref engine, false, 0, options.DetModel, options.LimitSideLen, 0.3, 0.6, 1.6, false,
                options.UseAngleCls, options.UseAngleCls, options.ClsModel, 0.9, 8.0, options.RecModel, options.DictPath,
                4, options.RecImgH, options.RecImgW, options.PredictorNum, msg);
            if (ret != 0 || engine == IntPtr.Zero)
            {
                throw new InvalidOperationException($"init failed: ret={ret}, msg={msg}");
            }

            return new NativeOcrEngine(engine, destroy);
        }

        public OcrResponse OcrWithTiming(IntPtr engine, int rows, int cols, int channels, byte[] bgr)
        {
            GCHandle pinned = GCHandle.Alloc(bgr, GCHandleType.Pinned);
            IntPtr result = IntPtr.Zero;
            int resultLen = 0;
            StringBuilder msg = new(128);
            try
            {
                int ret = ocrWithTiming(engine, rows, cols, channels, pinned.AddrOfPinnedObject(), msg, out result, out resultLen);
                if (ret != 0)
                {
                    throw new InvalidOperationException($"ocr2_with_timing failed: ret={ret}, msg={msg}");
                }

                string json = CopyUtf8(result, resultLen);
                return JsonSerializer.Deserialize(json, JsonContext.Default.OcrResponse)
                    ?? throw new InvalidOperationException("Native 返回空 OCR JSON。");
            }
            finally
            {
                if (result != IntPtr.Zero)
                {
                    freeResult(result);
                }
                pinned.Free();
            }
        }

        public double DetectOrientation(int rows, int cols, int channels, byte[] bgr)
        {
            GCHandle pinned = GCHandle.Alloc(bgr, GCHandleType.Pinned);
            StringBuilder msg = new(128);
            try
            {
                int ret = detectOrientation(rows, cols, channels, pinned.AddrOfPinnedObject(), out double angle, msg);
                if (ret != 0)
                {
                    throw new InvalidOperationException($"detect_orientation failed: ret={ret}, msg={msg}");
                }
                return angle;
            }
            finally
            {
                pinned.Free();
            }
        }

        public void Dispose() => NativeLibrary.Free(libraryHandle);

        private T GetDelegate<T>(string name) where T : Delegate => Marshal.GetDelegateForFunctionPointer<T>(NativeLibrary.GetExport(libraryHandle, name));

        private static string CopyUtf8(IntPtr result, int resultLen)
        {
            if (result == IntPtr.Zero || resultLen <= 0)
            {
                return "{}";
            }

            byte[] bytes = new byte[resultLen];
            Marshal.Copy(result, bytes, 0, resultLen);
            return Encoding.UTF8.GetString(bytes);
        }

        private static string ResolveDefaultLibraryPath()
        {
            //string fileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            //    ? "ky.OpenCVDNN.PPOCRSharp.dll"
            //    : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            //        ? "libky.OpenCVDNN.PPOCRSharp.dylib"
            //        : "libky.OpenCVDNN.PPOCRSharp.so";

            //string baseDir = AppContext.BaseDirectory;
            //string[] candidates =
            //[
            //    Path.Combine(baseDir, fileName),
            //    Path.Combine(baseDir, "..", "..", "..", "..", "bin", "x64", "Release", fileName),
            //    Path.Combine(baseDir, "..", "..", "..", "..", "bin", "x64", "Debug", fileName),
            //    fileName
            //];

            // 1. 判断系统 + 架构，得到原生库文件名
            string fileName;
            string arch = RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.LoongArch64 => "loongarch64",
                Architecture.Arm64 => "arm64",
                Architecture.X64 => "x64",
                _ => "x64" // 其他架构默认走x64
            };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fileName = "ky.OpenCVDNN.PPOCRSharp.dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                fileName = "libky.OpenCVDNN.PPOCRSharp.dylib";
            }
            else
            {
                // Linux / 麒麟等类Unix系统
                fileName = "libky.OpenCVDNN.PPOCRSharp.so";
            }

            // 2. 程序运行根目录
            string baseDir = AppContext.BaseDirectory;

            // 3. 候选查找路径：同时兼容 x64 / arm64 + Debug/Release + 部署目录
            //string[] candidates =
            //[
            //    // 优先级1：程序当前目录（正式部署）
            //    Path.Combine(baseDir, fileName),
            //// 优先级2：当前目录下按架构分目录存放（推荐部署方式）
            //Path.Combine(baseDir, arch, fileName),
            //// 优先级3：VS 调试 - 上层目录 bin/架构/Release
            //Path.Combine(baseDir, "..", "..", "..", "..", "bin", arch, "Release", fileName),
            //// 优先级4：VS 调试 - 上层目录 bin/架构/Debug
            //Path.Combine(baseDir, "..", "..", "..", "..", "bin", arch, "Debug", fileName),
            //// 优先级5：仅文件名，依赖系统PATH
            //fileName
            //];
            string[] candidates =
            {
                Path.Combine(baseDir, fileName),
                Path.Combine(baseDir, arch, fileName),
                Path.Combine(baseDir, "..", "..", "..", "..", "bin", arch, "Release", fileName),
                Path.Combine(baseDir, "..", "..", "..", "..", "bin", arch, "Debug", fileName),
                fileName
            };
            return candidates.FirstOrDefault(File.Exists) ?? fileName;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int InitDelegate(ref IntPtr engine, [MarshalAs(UnmanagedType.I1)] bool useGpu, int gpuId,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string detModelDir, int limitSideLen, double detDbThresh, double detDbBoxThresh,
            double detDbUnclipRatio, [MarshalAs(UnmanagedType.I1)] bool useDilation, [MarshalAs(UnmanagedType.I1)] bool cls,
            [MarshalAs(UnmanagedType.I1)] bool useAngleCls, [MarshalAs(UnmanagedType.LPUTF8Str)] string clsModelDir, double clsThresh,
            double clsBatchNum, [MarshalAs(UnmanagedType.LPUTF8Str)] string recModelDir, [MarshalAs(UnmanagedType.LPUTF8Str)] string recCharDictPath,
            int recBatchNum, int recImgH, int recImgW, int predictorNum, StringBuilder msg);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int OcrWithTimingDelegate(IntPtr engine, int rows, int cols, int channels, IntPtr data, StringBuilder msg, out IntPtr result, out int resultLen);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int DetectOrientationDelegate(int rows, int cols, int channels, IntPtr data, out double angle, StringBuilder msg);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void FreeResultDelegate(IntPtr result);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int DestroyDelegate(IntPtr engine, StringBuilder msg);
    }
}
