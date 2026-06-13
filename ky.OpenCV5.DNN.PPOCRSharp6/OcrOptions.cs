using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ky.OpenCV5.DNN.PPOCRSharp6
{
    /// <summary>
    /// 封装了 OCR 相关的配置选项，包括模型路径、输入图像路径、推理参数等。
    /// </summary>
    public sealed class OcrOptions
    {
        /// <summary>
        /// 可选的原生库路径，如果未指定则默认从当前目录加载名为 "PPOCRSharp6.dll"（Windows）或 "libPPOCRSharp6.so"（Linux）的库。
        /// </summary>
        public string NativeLibraryPath { get; set; }

        /// <summary>
        /// 要处理的单张图像路径，支持 JPG、PNG、BMP、WEBP 等格式。如果未指定且 BatchFolder 也未指定，则默认使用 "OCRFrom.Test/inference/1.jpg" 作为输入图像。
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// 要处理的图像文件夹路径，程序会扫描该文件夹下的所有 JPG、PNG、BMP、WEBP 格式的图像进行批量处理。如果未指定且 ImagePath 也未指定，则默认使用 "OCRFrom.Test/inference/1.jpg" 作为输入图像。
        /// </summary>
        public string BatchFolder { get; set; }

        /// <summary>
        /// 文本检测模型的 ONNX 文件路径，默认为 "inference/PP-OCRv6_tiny_det.onnx"。该模型用于检测图像中的文本区域。
        /// </summary>
        public string DetModel { get; set; }

        /// <summary>
        /// 文本识别模型的 ONNX 文件路径，默认为 "inference/PP-OCRv6_tiny_rec.onnx"。该模型用于识别检测到的文本区域中的字符内容。
        /// </summary>
        public string RecModel { get; set; }

        /// <summary>
        /// 文本识别模型使用的字符字典文件路径，默认为 "inference/PP-OCRv6_tiny_rec_dict.txt"。该文件包含了文本识别模型能够识别的字符列表，每行一个字符，顺序对应模型输出的类别索引。
        /// </summary>
        public string DictPath { get; set; }

        /// <summary>
        /// 文本方向分类模型的 ONNX 文件路径，默认为 "inference/PP-OCRv5_mobile_cls_onnx.onnx"。该模型用于判断检测到的文本区域的方向（如水平、竖直、倒置等），以便在识别前进行正确的旋转调整。如果未指定该模型路径，则默认不使用文本方向分类功能。
        /// </summary>
        public string ClsModel { get; set; }

        /// <summary>
        /// 推理轮数，默认为 1。该参数指定了对每张输入图像进行多少轮的 OCR 推理，通常用于性能测试和稳定性评估。每轮推理都会对同一图像进行完整的检测和识别流程，并记录每轮的耗时和结果，以便分析模型的性能表现和输出稳定性。
        /// </summary>
        public int Rounds { get; set; }

        /// <summary>
        /// 限制输入图像的最大边长，默认为 960。该参数用于在推理前对输入图像进行缩放调整，以确保图像的最大边长不超过指定值，从而平衡推理速度和识别精度。过大的输入图像可能会导致推理时间过长，而过小的输入图像可能会降低识别准确率，因此合理设置该参数可以获得更好的性能表现。
        /// </summary>
        public int LimitSideLen { get; set; }

        /// <summary>
        /// 文本识别模型输入图像的尺寸，默认为 320x48。该参数指定了文本识别模型所需的输入图像尺寸，通常为宽度和高度的固定值。输入图像会在推理前被缩放调整到该尺寸，以满足模型的输入要求。合理设置该参数可以提高识别准确率和推理效率，但过大的输入尺寸可能会增加推理时间，而过小的输入尺寸可能会降低识别精度，因此需要根据实际应用场景进行调整。
        /// </summary>
        public int RecImgH { get; set; }

        /// <summary>
        /// 文本识别模型输入图像的宽度，默认为 320。该参数指定了文本识别模型所需的输入图像宽度，通常与 RecImgH 配合使用形成固定尺寸的输入图像。输入图像会在推理前被缩放调整到 RecImgW x RecImgH 的尺寸，以满足模型的输入要求。合理设置该参数可以提高识别准确率和推理效率，但过大的输入尺寸可能会增加推理时间，而过小的输入尺寸可能会降低识别精度，因此需要根据实际应用场景进行调整。
        /// </summary>
        public int RecImgW { get; set; }

        /// <summary>
        /// 文本识别模型的并行推理线程数，默认为 CPU 核心数。该参数指定了在进行文本识别推理时使用的线程数量，通常设置为 CPU 的核心数以获得最佳性能表现。增加线程数可以提高推理速度，但过多的线程可能会导致系统资源竞争和性能下降，因此建议根据实际硬件环境进行调整。
        /// </summary>
        public int PredictorNum { get; set; }

        /// <summary>
        /// 是否使用文本方向分类功能，默认为 false。该参数用于控制是否在 OCR 推理流程中启用文本方向分类步骤，如果设置为 true，则会在检测到的文本区域进行识别前先使用 ClsModel 进行方向分类，以判断文本的正确方向并进行相应的旋转调整，从而提高识别准确率。如果设置为 false，则直接进行文本识别而不考虑文本方向，可能会导致某些情况下的识别错误，特别是当输入图像中的文本存在不同方向时。
        /// </summary>
        public bool UseAngleCls { get; set; }

        public OcrOptions()
        {
            // 初始化默认值（替代 init 初始化器）
            NativeLibraryPath = null;
            ImagePath = null;
            BatchFolder = null;
            DetModel = Path.Combine("inference", "PP-OCRv6_tiny_det.onnx");
            RecModel = Path.Combine("inference", "PP-OCRv6_tiny_rec.onnx");
            DictPath = Path.Combine("inference", "PP-OCRv6_tiny_rec_dict.txt");
            ClsModel = Path.Combine("inference", "PP-OCRv5_mobile_cls_onnx.onnx");
            Rounds = 1;
            LimitSideLen = 960;
            RecImgH = 48;
            RecImgW = 320;
            PredictorNum = Math.Max(1, Environment.ProcessorCount);
            UseAngleCls = false;
        }

        /// <summary>
        /// 从命令行参数解析 OCR 配置选项。该方法接受一个字符串数组作为输入，解析其中的参数并返回一个 OcrOptions 实例。支持的命令行参数包括：
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static OcrOptions Parse(string[] args)
        {
            OcrOptions options = new OcrOptions();

            for (int i = 0; i < args.Length; i++)
            {
                string currentArg = args[i];
                string Next()
                {
                    if (i + 1 < args.Length)
                        return args[++i];
                    throw new ArgumentException($"参数 {currentArg} 缺少值。");
                }

                switch (currentArg)
                {
                    case "--native":
                        options.NativeLibraryPath = Next();
                        break;
                    case "--image":
                        options.ImagePath = Next();
                        break;
                    case "--batch":
                        options.BatchFolder = Next();
                        break;
                    case "--det":
                        options.DetModel = Next();
                        break;
                    case "--rec":
                        options.RecModel = Next();
                        break;
                    case "--dict":
                        options.DictPath = Next();
                        break;
                    case "--cls":
                        options.ClsModel = Next();
                        options.UseAngleCls = true;
                        break;
                    case "--rounds":
                        options.Rounds = Math.Max(1, int.Parse(Next()));
                        break;
                    case "--limit":
                        options.LimitSideLen = Math.Max(32, int.Parse(Next()));
                        break;
                    case "--rec-width":
                        options.RecImgW = Math.Max(32, int.Parse(Next()));
                        break;
                    case "-h":
                    case "--help":
                        throw new ArgumentException(HelpText);
                    default:
                        throw new ArgumentException($"未知参数: {currentArg}{Environment.NewLine}{HelpText}");
                }
            }

            // 补充默认图片路径（原 with 语法替换为直接赋值）
            if (string.IsNullOrEmpty(options.ImagePath) && string.IsNullOrEmpty(options.BatchFolder))
            {
                options.BatchFolder = AppContext.BaseDirectory ;
            }

            return options;
        }

        /// <summary>
        /// 解析输入图像路径，返回一个包含所有待处理图像文件路径的列表。该方法会根据 ImagePath 和 BatchFolder 的配置来确定要处理的图像文件：
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<string> ResolveImages()
        {
            List<string> images = new List<string>();

            if (!string.IsNullOrWhiteSpace(ImagePath) && File.Exists(ImagePath))
            {
                images.Add(ImagePath);
            }

            if (!string.IsNullOrWhiteSpace(BatchFolder) && Directory.Exists(BatchFolder))
            {
                // 替换 C# 内联数组简写
                string[] extensions = new string[] { ".jpg", ".jpeg", ".png", ".bmp", ".webp" };

                var files = Directory.EnumerateFiles(BatchFolder, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(p => extensions.Contains(Path.GetExtension(p), StringComparer.OrdinalIgnoreCase))
                    .OrderBy(p => p, StringComparer.OrdinalIgnoreCase);

                images.AddRange(files);
            }

            return images.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        }

        private const string HelpText1 = @"用法: dotnet run --project ky.OpenCVDNN.PPOCRSharp.ConsoleTest -- --image <file>|--batch <folder> [--rounds 10] [--native <dll/so/dylib>] [--det <onnx>] [--rec <onnx>] [--dict <txt>] [--cls <onnx>]  \r\n 说明：";

        public static readonly string HelpText = @"
OCR 推理程序使用说明
用法: dotnet run --project ky.OpenCVDNN.PPOCRSharp.ConsoleTest -- --image <file>|--batch <folder> [--rounds 10] [--native <dll/so/dylib>] [--det <onnx>] [--rec <onnx>] [--dict <txt>] [--cls <onnx>]  

必选参数:
  --image        单图路径            指定待处理的单张图像文件路径
或者
  --batch        批处理文件夹路径     指定文件夹，批量处理目录下所有图像

可选参数:
  --native       原生库路径          指定要加载的原生库文件路径
  --det          检测模型路径         指定文本检测 ONNX 模型文件路径
  --rec          识别模型路径         指定文本识别 ONNX 模型文件路径
  --dict         字符字典路径         指定识别模型使用的字符字典文件路径
  --cls          方向分类模型路径     指定文本方向分类 ONNX 模型路径，自动启用方向校正
  --rounds       推理轮数             单张图像 OCR 推理执行轮数，最小值 1，默认 1
  --limit        图像最大边长         推理前图像缩放最大边长，最小值 32，默认 960
  --rec-width    识别图宽度           识别模型输入图像宽度，最小值 32，默认 320

辅助参数:
  -h, --help     查看本帮助信息

提示:
  1. --image 与 --batch 二选一，分别对应单图处理、批量处理模式
  2. 所有数值型参数会自动限制下限，输入过小数值将按最小值生效
";
    }
}