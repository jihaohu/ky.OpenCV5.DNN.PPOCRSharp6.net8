using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ky.OpenCV5.DNN.PPOCRSharp6.PaddleOCRSharp
{

    // 实现抽象基类 EngineBase
    public class PPOCREngine : EngineBase
    {
        /// <summary>
        /// 对外识别入口：传入图片路径，返回识别结果
        /// </summary>
        public List<OCRResult> RecognizeImage(string imgFilePath)
        {
            // 清空历史结果
            ltOCRResult.Clear();
            OCRResultInfo.Clear();
            OCRResultAllInfo.Clear();

            try
            {
                // 1. 加载图片为 BGR 格式 + 获取原始像素数据
                bmp = LoadBgrBitmap(imgFilePath, out byte[] bgrData);
                imgPath = imgFilePath;

                // 2. 指针操作：固定内存防止 GC 移动
                var handle = System.Runtime.InteropServices.GCHandle.Alloc(bgrData, System.Runtime.InteropServices.GCHandleType.Pinned);
                try
                {
                    IntPtr dataPtr = handle.AddrOfPinnedObject();
                    int rows = bmp.Height;
                    int cols = bmp.Width;
                    int channels = 3;

                    StringBuilder msg = new StringBuilder(256);
                    int ret = ocr2(OCREngine, rows, cols, channels, dataPtr, msg, out IntPtr resultPtr, out int resultLen);

                    if (ret != 0)
                    {
                        Console.WriteLine($"识别失败：{msg}");
                        return ltOCRResult;
                    }
                    // 3. 解析原生返回的 JSON 字符串（示例：简易解析，可替换为 Newtonsoft.Json）
                    byte[] jsonBytes = new byte[resultLen];
                    Marshal.Copy(resultPtr, jsonBytes, 0, resultLen);
                    string json = Encoding.UTF8.GetString(jsonBytes);
                    //string json = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(resultPtr, resultLen);
                    OCRResultAllInfo.Append(json);
                    Console.WriteLine($"原始识别结果：{json}");

                    // ========== 这里根据你的 JSON 格式解析坐标、文本、置信度 ==========
                    // 演示：模拟解析结果并绘制框
                    var demoResult = new OCRResult
                    {
                        Text = "测试文本",
                        Confidence = 0.98f,
                        X1 = 10,
                        Y1 = 10,
                        X2 = 200,
                        Y2 = 60
                    };
                    ltOCRResult.Add(demoResult);

                    //// 4. 在原图绘制检测框
                    DrawBox(bmp, demoResult.X1, demoResult.Y1, demoResult.X2, demoResult.Y2);

                    // 5. 释放原生内存（必须调用，内存泄漏防护）
                    free_ocr_result(resultPtr);
                }
                finally
                {
                    handle.Free();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"识别异常：{ex.Message}");
            }

            return ltOCRResult;
        }

        /// <summary>
        /// 保存绘制后的图片
        /// </summary>
        public void SaveResultImage(string savePath)
        {
            bmp?.Save(savePath);
        }
    }
}
