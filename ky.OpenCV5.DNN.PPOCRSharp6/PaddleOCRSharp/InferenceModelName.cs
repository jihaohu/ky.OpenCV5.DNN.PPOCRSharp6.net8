using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ky.OpenCV5.DNN.PPOCRSharp6.PaddleOCRSharp
{
    /// <summary>
    /// OCR 推理模型类型枚举
    /// </summary>
    public enum InferenceModelName
    {
        /// <summary>
        /// PP-OCRv5 移动端模型
        /// </summary>
        v5_mobile,

        /// <summary>
        /// PP-OCRv5 服务端模型
        /// </summary>
        v5_server,

        /// <summary>
        /// PP-OCRv6 small 模型
        /// </summary>
        v6_small,

        /// <summary>
        /// PP-OCRv6 tiny 模型
        /// </summary>
        v6_tiny,

        /// <summary>
        /// PP-OCRv6 medium 模型
        /// </summary>
        v6_medium
    }
}
