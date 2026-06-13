using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ky.OpenCV5.DNN.PPOCRSharp6.PaddleOCRSharp
{

    /// <summary>
    /// OCR 模型路径配置：检测(det)、分类(cls)、识别(rec)模型目录与字典文件路径。
    /// 生产环境中通常使用 OCRModelConfig.Default 来基于可执行目录下的 inference 子目录定位默认模型。
    /// </summary>
    public class OCRModelConfig
    {
        /// <summary>
        /// 初始化空的 OCR 模型配置对象。
        /// </summary>
        public OCRModelConfig()
        {
        }

        /// <summary>
        /// 使用指定模型目录与字典初始化 OCR 模型配置。
        /// </summary>
        /// <param name="det_infer_path">检测模型目录。</param>
        /// <param name="cls_infer_path">分类模型目录。</param>
        /// <param name="rec_infer_path">识别模型目录。</param>
        /// <param name="dict">字典文件路径。</param>
        public OCRModelConfig(string det_infer_path, string cls_infer_path, string rec_infer_path, string dict)
        {
            this.det_infer = det_infer_path;
            this.cls_infer = cls_infer_path;
            this.rec_infer = rec_infer_path;
            this.keys = dict;
        }

        /// <summary>
        /// 检测模型目录（det_infer）。
        /// </summary>
        public string det_infer { get; set; }

        /// <summary>
        /// 分类模型目录（cls_infer）。
        /// </summary>
        public string cls_infer { get; set; }

        /// <summary>
        /// 识别模型目录（rec_infer）。
        /// </summary>
        public string rec_infer { get; set; }

        /// <summary>
        /// 字典文件路径，默认 ppocr_keys.txt（通常位于 inference 目录下）。
        /// </summary>
        public string keys { get; set; } = "ppocr_keys.txt";

        /// <summary>
        /// 获取默认的 OCR 模型配置，基于可执行目录下的 inference 子目录定位默认模型路径。默认模型包括：
        /// - 检测模型
        /// - 分类模型
        /// - 识别模型
        /// </summary>
        public static OCRModelConfig Default
        {
            get
            {
                string rootDirectory = EngineBase.GetRootDirectory();
                OCRModelConfig ocrmodelConfig = new OCRModelConfig();
                string path = Path.Combine(rootDirectory, "inference");
                ocrmodelConfig.det_infer = Path.Combine(path, "PP-OCRv6_medium_det");
                //ocrmodelConfig.cls_infer = Path.Combine(path, "PP-OCRv6_medium_rec");
                ocrmodelConfig.rec_infer = Path.Combine(path, "PP-OCRv6_medium_rec");
                ocrmodelConfig.keys = Path.Combine(path, "PP-OCRv6_medium_rec_dict.txt");
                return ocrmodelConfig;
            }
        }
    }
}