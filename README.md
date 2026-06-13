基于 OpenCV 5 DNN 实现的跨平台 PPOCR .NET 封装库，支持 Windows x64/ARM64、Linux x64/ARM64（含国产麒麟系统），专为高性能图文识别场景设计。
作者：JiHao.Hu
公司：湖北中软科怡档案信息技术有限公司
许可证：Apache License 2.0
✨ 功能特性
跨平台支持：Windows x64/ARM64、Linux x64/ARM64（含国产麒麟系统）
高性能后端：基于 OpenCV 5 DNN 原生推理引擎，无额外 Python 依赖
开箱即用：内置预编译原生库，无需复杂配置
架构兼容：统一接口适配 x64 与 ARM64 架构
轻量集成：纯 .NET 封装，可直接嵌入 WinForms/WPF/ 控制台项目
📦 快速开始
1. 环境要求
.NET 8.0 及以上
Windows / Linux（麒麟系统兼容）
架构：x64 / ARM64
2. 项目配置
在 .csproj 中添加依赖：
xml
<ItemGroup>
  <PackageReference Include="SixLabors.ImageSharp" Version="3.1.12" />
</ItemGroup>
3. 基础使用示例
csharp
运行
using System;
using ky.OpenCV5.DNN.PPOCRSharp6;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine($"组件版本：{AppVersion.FullVersion}");

        // 初始化引擎（示例）
        using var engine = new PPOCREngine();
        var result = engine.Ocr("test.png");

        Console.WriteLine($"识别结果：{result.Text}");
        Console.WriteLine($"置信度：{result.Score:F3}");
    }
}
🚀 性能压测示例
csharp
运行
using System.Diagnostics;
using System.Text.Json;

// ... 初始化引擎与图像 ...

int rounds = 10;
double bestTotal = double.MaxValue;
OcrResponse best = null;

Console.WriteLine("=============================================");
Console.WriteLine($"PPOCR 压测 | 版本：{AppVersion.FullVersion}");
Console.WriteLine("=============================================");

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
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};
Console.WriteLine(JsonSerializer.Serialize(best, jsonOpts));


📁 目录结构
plaintext
ky.OpenCV5.DNN.PPOCRSharp6/
├── src/
│   └── ky.OpenCV5.DNN.PPOCRSharp6.csproj  # 主项目文件
├── NativeLibs/                             # 预编译原生库
│   ├── x64/
│   │   ├── ky.OpenCVDNN.PPOCRSharp.dll
│   │   └── opencv_world5xx.dll
│   └── arm64/
│       └── libky.OpenCVDNN.PPOCRSharp.so
└── README.md                               # 项目说明


🛠️ 部署说明
编译时原生库会自动复制到对应架构的输出目录
部署时确保 x64/arm64 目录下的原生库与主程序在同一层级
Linux / 麒麟系统部署时，需确保系统已安装 libopencv 依赖
📄 许可证
本项目基于 Apache License 2.0 开源，详情请见 LICENSE 文件。
📧 联系与支持
作者：JiHao.Hu
公司：湖北中软科怡档案信息技术有限公司
项目地址：https://github.com/jihaohu/ky.OpenCV5.DNN.PPOCRSharp6
