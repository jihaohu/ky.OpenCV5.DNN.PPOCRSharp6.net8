using ky.OpenCV5.DNN.PPOCRSharp6;
using ky.OpenCV5.DNN.PPOCRSharp6.PaddleOCRSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ky.OpenCVDNN.PPOCRSharp.ConsoleTest;

internal static class Program
{

    // 临时输出根目录
    private static readonly string TempOutputDir = Path.Combine(AppContext.BaseDirectory, "OCR_Temp_Output");

    static int Main(string[] args)
    {
        try
        {
            Console.WriteLine("===== PaddleOCR 跨平台批量识别工具 =====");

            // 解析命令行参数
            OcrOptions options = OcrOptions.Parse(args);
            IReadOnlyList<string> images = options.ResolveImages();

            if (images.Count == 0)
            {
                Console.Error.WriteLine("未找到待识别图片，请使用 --image <file> 或 --batch <folder>。");
                Console.ReadKey();
                return 2;
            }

            // 创建临时输出目录
            if (!Directory.Exists(TempOutputDir))
            {
                Directory.CreateDirectory(TempOutputDir);
            }
            Console.WriteLine($"识别结果将保存至：{TempOutputDir}\n");

            // 初始化OCR引擎
            using var ocrEngine = new PaddleOcrEngine();
            StringBuilder loadMsg = new StringBuilder();

            bool loadOk = ocrEngine.LoadModel(
                det_db_thresh: 0.3,
                det_db_box_thresh: 0.5,
                det_db_unclip_ratio: 1.6,
                cls: true,
                num: 10,
                rec_batch_num: 6,
                predictor_num: 4,
                modelName: InferenceModelName.v5_mobile,
                stringBuilder: ref loadMsg);

            Console.WriteLine($"模型加载结果：{loadMsg}");
            if (!loadOk)
            {
                Console.WriteLine("模型加载失败，程序退出");
                Console.ReadKey();
                return 3;
            }

            // 批量遍历图片
            int total = images.Count;
            int successCount = 0;

            for (int i = 0; i < total; i++)
            {
                string imgPath = images[i];
                Console.WriteLine($"[{i + 1}/{total}] 正在处理：{Path.GetFileName(imgPath)}");

                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    // 单张识别
                    var ocrResults = ocrEngine.RecognizeImage(imgPath);

                    // 生成输出文件名（保留原文件名）
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(imgPath);
                    string ext = Path.GetExtension(imgPath);
                    stopwatch.Stop();
                    // 1. 保存画框后的图片
                    string saveImgPath = Path.Combine(TempOutputDir, $"{fileNameWithoutExt}_result{ext}");
                    ocrEngine.SaveResultImage(saveImgPath);

                    // 2. 保存识别文本到txt
                    string saveTxtPath = Path.Combine(TempOutputDir, $"{fileNameWithoutExt}_result.txt");
                    SaveOcrText(saveTxtPath, imgPath, ocrResults);

                    successCount++;
                    double totalMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
                    Console.WriteLine(string.Format("耗时: {0:F2}ms", totalMilliseconds));
                    Console.WriteLine("---------------------------");
                    Console.WriteLine($"  处理成功 | 标注图+文本已保存\n");
                }
                catch (Exception imgEx)
                {
                    Console.WriteLine($"  处理失败：{imgEx.Message}\n");
                }
            }

            // 批量处理汇总
            Console.WriteLine("=====================================");
            Console.WriteLine($"批量处理完成！总数：{total} | 成功：{successCount} | 失败：{total - successCount}");
            Console.WriteLine($"输出目录：{TempOutputDir}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"程序全局异常: {ex}");
        }

        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
        return 0;
    }

    /// <summary>
    /// 保存单张图片的识别结果到文本文件
    /// </summary>
    private static void SaveOcrText(string txtPath, string sourceImg, List<OCRResult> results)
    {
        using var sw = new StreamWriter(txtPath, false, Encoding.UTF8);
        sw.WriteLine($"源图片：{sourceImg}");
        sw.WriteLine($"识别时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sw.WriteLine("----------------------------------------");

        if (results.Any())
        {
            foreach (var item in results)
            {
                sw.WriteLine($"文本：{item.Text}");
                sw.WriteLine($"置信度：{item.Confidence:F4}");
                sw.WriteLine($"坐标：({item.X1},{item.Y1}) -> ({item.X2},{item.Y2})");
                sw.WriteLine("----------------------------------------");
            }
        }
        else
        {
            sw.WriteLine("未识别到有效内容");
        }
    }
    private static int Main1(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        try
        {

            OcrOptions options = OcrOptions.Parse(args);


            IReadOnlyList<string> images = options.ResolveImages();
            if (images.Count == 0)
            {
                //images = new List<string>
                //{
                //    "1.jpg"
                //}.Where(File.Exists).ToList();
                Console.Error.WriteLine("未找到待识别图片，请使用 --image <file> 或 --batch <folder>。");
                Console.Read();
                return 2;
            }

            using NativeOcrApi api = NativeOcrApi.Load(options.NativeLibraryPath);
            using NativeOcrEngine engine = api.CreateEngine(options);

            Console.WriteLine("ky.OpenCV5.DNN.PPOCRSharp6 .NET 8 跨平台 OCR 测试");
            Console.WriteLine($"Native库: {api.LibraryPath}");
            Console.WriteLine($"图片数量: {images.Count}, 每张测试轮数: {options.Rounds}");
            Console.WriteLine(new string('-', 96));

            foreach (string image in images)
            {
                RunImageBenchmark(api, engine, image, options.Rounds);
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static void RunImageBenchmark(NativeOcrApi api, NativeOcrEngine engine, string imagePath, int rounds)
    {
        using Image<Rgb24> image = Image.Load<Rgb24>(imagePath);
        byte[] bgr = ImageSharpBgrConverter.ToBgrBytes(image);

        double angle = api.DetectOrientation(image.Height, image.Width, 3, bgr);
        Console.WriteLine($"图片: {imagePath}");
        Console.WriteLine($"尺寸: {image.Width}x{image.Height}, 方向估计: {angle:F2}°");
        Console.WriteLine("轮次 | 外层耗时(ms) | Native总(ms) | det(ms) | cls(ms) | rec(ms) | 文本块数 | 平均分");

        double bestTotal = double.MaxValue;
        OcrResponse? best = null;
        for (int i = 1; i <= rounds; i++)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            OcrResponse response = api.OcrWithTiming(engine.Handle, image.Height, image.Width, 3, bgr);
            stopwatch.Stop();

            double averageScore = response.Results.Count == 0 ? 0 : response.Results.Average(r => r.Score);
            if (response.Timing.TotalMs < bestTotal)
            {
                bestTotal = response.Timing.TotalMs;
                best = response;
            }

            Console.WriteLine(
                $"{i,4} | {stopwatch.Elapsed.TotalMilliseconds,12:F2} | {response.Timing.TotalMs,10:F2} | " +
                $"{response.Timing.DetMs,7:F2} | {response.Timing.ClsMs,7:F2} | {response.Timing.RecMs,7:F2} | " +
                $"{response.Results.Count,6} | {averageScore,6:F3}");
        }

        Console.WriteLine("最佳轮次结果(JSON):");
        var jsonOpts = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        Console.WriteLine(JsonSerializer.Serialize(best ?? throw new InvalidOperationException("未产生 OCR 基准结果。"), jsonOpts));
        Console.WriteLine(new string('-', 96));
    }
}
