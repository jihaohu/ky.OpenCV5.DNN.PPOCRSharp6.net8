using System;

namespace ky.OpenCV5.DNN.PPOCRSharp6.PaddleOCRSharp
{
	/// <summary>
	/// OCR 单条识别结果模型。
	/// </summary>
	public class OCRResult
	{
		/// <summary>
		/// 识别文本内容。
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// 识别置信度。
		/// </summary>
		public float Confidence { get; set; }

		/// <summary>
		/// 检测框左上角 x。
		/// </summary>
		public int X1 { get; set; }

		/// <summary>
		/// 检测框左上角 y。
		/// </summary>
		public int Y1 { get; set; }

		/// <summary>
		/// 检测框右上角 x。
		/// </summary>
		public int X2 { get; set; }

		/// <summary>
		/// 检测框右上角 y。
		/// </summary>
		public int Y2 { get; set; }

		/// <summary>
		/// 检测框右下角 x。
		/// </summary>
		public int X3 { get; set; }

		/// <summary>
		/// 检测框右下角 y。
		/// </summary>
		public int Y3 { get; set; }

		/// <summary>
		/// 检测框左下角 x。
		/// </summary>
		public int X4 { get; set; }

		/// <summary>
		/// 检测框左下角 y。
		/// </summary>
		public int Y4 { get; set; }
	}
}
