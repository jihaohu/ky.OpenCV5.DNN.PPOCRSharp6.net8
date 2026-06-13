using System;

namespace OCRFrom.Test
{
	/// <summary>
	/// OCR 单条识别结果模型。
	/// </summary>
	public class OCRResult
	{
		/// <summary>
		/// 识别文本内容。
		/// </summary>
		public string text { get; set; }

		/// <summary>
		/// 识别置信度。
		/// </summary>
		public float score { get; set; }

		/// <summary>
		/// 检测框左上角 x。
		/// </summary>
		public int x1 { get; set; }

		/// <summary>
		/// 检测框左上角 y。
		/// </summary>
		public int y1 { get; set; }

		/// <summary>
		/// 检测框右上角 x。
		/// </summary>
		public int x2 { get; set; }

		/// <summary>
		/// 检测框右上角 y。
		/// </summary>
		public int y2 { get; set; }

		/// <summary>
		/// 检测框右下角 x。
		/// </summary>
		public int x3 { get; set; }

		/// <summary>
		/// 检测框右下角 y。
		/// </summary>
		public int y3 { get; set; }

		/// <summary>
		/// 检测框左下角 x。
		/// </summary>
		public int x4 { get; set; }

		/// <summary>
		/// 检测框左下角 y。
		/// </summary>
		public int y4 { get; set; }
	}
}
