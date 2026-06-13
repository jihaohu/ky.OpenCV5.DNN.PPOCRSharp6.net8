using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Path = System.IO.Path;

namespace ky.OpenCV5.DNN.PPOCRSharp6.PaddleOCRSharp
{
    /// <summary>
    /// Base class for PaddleOCR engine wrappers.
    /// 负责初始化运行环境、定位本地 PaddleOCR 原生库、提供图像到字节的转换工具、以及获取原生错误信息等通用功能。
    /// 子类应在此基础上调用原生方法并实现资源释放逻辑。
    /// </summary>
    public abstract class EngineBase : IDisposable
    {
        private static readonly string LibFileName;
        #region Native Dll Import
        static EngineBase()
        {
            string libName;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                libName = "ky.OpenCVDNN.PPOCRSharp.dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                libName = "libky.OpenCVDNN.PPOCRSharp.so";
            }
            else
            {
                throw new PlatformNotSupportedException("当前平台不支持");
            }

            LibFileName = libName;

            // 初始化时把 NativeLibs 对应架构目录加入系统加载路径
            PreloadNativeLibrary();
        }


        /// <summary>
        /// 根据当前平台和架构，把 NativeLibs 目录加入库搜索路径
        /// </summary>
        private static void PreloadNativeLibrary()
        {
            string baseDir = GetDLLDirectory();
            string archDir;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                archDir = "windows-x64";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                archDir = RuntimeInformation.ProcessArchitecture == Architecture.Arm64
                    ? "linux-arm64"
                    : "linux-x64";
            }
            else
            {
                throw new PlatformNotSupportedException();
            }

            string libDir = Path.Combine(baseDir, archDir);

            if (!Directory.Exists(libDir))
            {
                throw new DirectoryNotFoundException($"找不到原生库目录：{libDir}");
            }

            // Windows：设置 DLL 搜索路径
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                SetDllDirectory(libDir);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux：通过 dlopen 预加载依赖库，或设置 LD_LIBRARY_PATH
                // 这里可以先预加载主库，让依赖自动解析
                string libPath = Path.Combine(libDir, LibFileName);
                if (!File.Exists(libPath))
                {
                    throw new FileNotFoundException($"找不到原生库文件：{libPath}");
                }

                var handle = dlopen(libPath, 2 /* RTLD_NOW */);
                if (handle == IntPtr.Zero)
                {
                    throw new InvalidOperationException($"无法加载原生库：{libPath}");
                }
            }
        }

        public static string GetDLLDirectory()
        {
            return Path.Combine(AppContext.BaseDirectory, "NativeLibs");
        }
        // 平台/架构对应的库名
        private const string DllName = "ky.OpenCVDNN.PPOCRSharp.dll";

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int init(ref IntPtr engine,
            [MarshalAs(UnmanagedType.I1)] bool use_gpu, int gpu_id,
            string det_model_dir, int limit_side_len,
            double det_db_thresh, double det_db_box_thresh, double det_db_unclip_ratio,
            [MarshalAs(UnmanagedType.I1)] bool use_dilation,
            [MarshalAs(UnmanagedType.I1)] bool cls,
            [MarshalAs(UnmanagedType.I1)] bool use_angle_cls,
            string cls_model_dir, double cls_thresh, double cls_batch_num,
            string rec_model_dir, string rec_char_dict_path,
            int rec_batch_num, int rec_img_h, int rec_img_w,
            int predictor_num, StringBuilder msg);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ocr(IntPtr engine, IntPtr image, StringBuilder msg, out IntPtr ocr_result, out int ocr_result_len);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ocr2(IntPtr engine, int rows, int cols, int channels, IntPtr data, StringBuilder msg, out IntPtr ocr_result, out int ocr_result_len);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void free_ocr_result(IntPtr ocr_result);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int destroy(IntPtr engine, StringBuilder msg);
        // Windows 用
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool SetDllDirectory(string lpPathName);

        // Linux 用
        [DllImport("libdl.so")]
        private static extern IntPtr dlopen(string filename, int flags);

        [DllImport("libdl.so")]
        private static extern int dlclose(IntPtr handle);
        #endregion

        #region Fields & Config
        protected IntPtr OCREngine = IntPtr.Zero;
        protected Image<Bgr24>? bmp;
        protected string imgPath = string.Empty;
        protected List<OCRResult> ltOCRResult = new List<OCRResult>();
        protected string fileFilter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tif;*.png";
        protected StringBuilder OCRResultInfo = new StringBuilder();
        protected StringBuilder OCRResultAllInfo = new StringBuilder();

        private readonly Color BoxDrawColor = Color.Red;
        private readonly float BoxDrawThickness = 2f;
        private bool _disposed = false;
        #endregion

        #region LoadModel 重载方法
        public bool LoadModel(double det_db_thresh, double det_db_box_thresh, double det_db_unclip_ratio,
            bool cls, int num, int rec_batch_num, int predictor_num,
            string modelName, ref StringBuilder stringBuilder)
        {
            stringBuilder ??= new StringBuilder(128);

            if (!Enum.TryParse(modelName, true, out InferenceModelName model))
            {
                stringBuilder.AppendLine($"无效的模型名称：{modelName}");
                return false;
            }

            return LoadModel(det_db_thresh, det_db_box_thresh, det_db_unclip_ratio,
                cls, num, rec_batch_num, predictor_num, model, ref stringBuilder);
        }

        public bool LoadModel(double det_db_thresh, double det_db_box_thresh, double det_db_unclip_ratio,
            bool cls, int num, int rec_batch_num, int predictor_num,
            InferenceModelName modelName, ref StringBuilder stringBuilder)
        {
            UnloadModel();
            stringBuilder = new StringBuilder(128);

            bool use_gpu = false;
            int gpu_id = 0;
            int limit_side_len = 960;
            bool use_dilation = false;
            bool use_angle_cls = true;
            string cls_model_dir = string.Empty;
            double cls_thresh = 0.9;
            int rec_img_h = 48;
            int rec_img_w = 320;

            string detModelPath = string.Empty;
            string recModelPath = string.Empty;
            string dictPath = string.Empty;
            string rootDir = GetRootDirectory();

            switch (modelName)
            {
                case InferenceModelName.v5_mobile:
                    detModelPath = Path.Combine(rootDir, "PP-OCRv5_mobile_det_onnx.onnx");
                    recModelPath = Path.Combine(rootDir, "PP-OCRv5_mobile_rec_onnx.onnx");
                    cls_model_dir = Path.Combine(rootDir, "PP-OCRv5_mobile_cls_onnx.onnx");
                    dictPath = Path.Combine(rootDir, "ppocrv5_dict.txt");
                    break;

                case InferenceModelName.v5_server:
                    detModelPath = Path.Combine(rootDir, "PP-OCRv5_server_det_infer.onnx");
                    recModelPath = Path.Combine(rootDir, "PP-OCRv5_server_rec_infer.onnx");
                    dictPath = Path.Combine(rootDir, "ppocrv5_dict.txt");
                    cls = false;
                    use_angle_cls = false;
                    break;

                case InferenceModelName.v6_small:
                    detModelPath = Path.Combine(rootDir, "PP-OCRv6_small_det.onnx");
                    recModelPath = Path.Combine(rootDir, "PP-OCRv6_small_rec.onnx");
                    dictPath = Path.Combine(rootDir, "PP-OCRv6_small_rec_dict.txt");
                    cls = false;
                    use_angle_cls = false;
                    break;

                case InferenceModelName.v6_tiny:
                    detModelPath = Path.Combine(rootDir, "PP-OCRv6_tiny_det.onnx");
                    recModelPath = Path.Combine(rootDir, "PP-OCRv6_tiny_rec.onnx");
                    dictPath = Path.Combine(rootDir, "PP-OCRv6_tiny_rec_dict.txt");
                    cls = false;
                    use_angle_cls = false;
                    break;

                case InferenceModelName.v6_medium:
                    detModelPath = Path.Combine(rootDir, "PP-OCRv6_medium_det.onnx");
                    recModelPath = Path.Combine(rootDir, "PP-OCRv6_medium_rec.onnx");
                    dictPath = Path.Combine(rootDir, "PP-OCRv6_medium_rec_dict.txt");
                    cls = false;
                    use_angle_cls = false;
                    break;

                default:
                    detModelPath = Path.Combine(rootDir, "PP-OCRv5_server_det_infer.onnx");
                    recModelPath = Path.Combine(rootDir, "PP-OCRv5_server_rec_infer.onnx");
                    dictPath = Path.Combine(rootDir, "ppocrv5_dict.txt");
                    cls = false;
                    use_angle_cls = false;
                    break;
            }

            int ret = init(ref OCREngine, use_gpu, gpu_id, detModelPath, limit_side_len,
                det_db_thresh, det_db_box_thresh, det_db_unclip_ratio, use_dilation,
                cls, use_angle_cls, cls_model_dir, cls_thresh, (double)num,
                recModelPath, dictPath, rec_batch_num, rec_img_h, rec_img_w,
                predictor_num, stringBuilder);

            if (ret == 0)
            {
                stringBuilder.Insert(0, "模型加载成功：");
                return true;
            }
            else
            {
                stringBuilder.Insert(0, "模型加载失败：");
                return false;
            }
        }
        #endregion

        #region 模型卸载 & 图像加载
        private void UnloadModel()
        {
            if (OCREngine != IntPtr.Zero)
            {
                StringBuilder sb = new StringBuilder(128);
                destroy(OCREngine, sb);
                OCREngine = IntPtr.Zero;
            }
        }

        protected Image<Bgr24> LoadBgrBitmap(string path, out byte[] bgrData)
        {
            using var image = Image.Load(path);
            var bgrImage = image.CloneAs<Bgr24>();

            int width = bgrImage.Width;
            int height = bgrImage.Height;
            int rowBytes = width * 3;
            bgrData = new byte[rowBytes * height];

            bgrImage.CopyPixelDataTo(bgrData);
            return bgrImage;
        }
        #endregion

        #region IDisposable 资源释放
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                bmp?.Dispose();
                bmp = null;
            }

            UnloadModel();
            _disposed = true;
        }

        ~EngineBase()
        {
            Dispose(false);
        }
        #endregion

        #region 绘制检测框
        protected void DrawBox(Image image, float x1, float y1, float x2, float y2)
        {
            image.Mutate(ctx =>
            {
                ctx.DrawPolygon(BoxDrawColor, BoxDrawThickness, new[]
                {
                    new PointF(x1, y1),
                    new PointF(x2, y1),
                    new PointF(x2, y2),
                    new PointF(x1, y2)
                });
            });
        }
        #endregion

        #region 路径工具
        public static string GetRootDirectory()
        {
            return Path.Combine(AppContext.BaseDirectory, "inference");
        }

        #endregion
    }
}