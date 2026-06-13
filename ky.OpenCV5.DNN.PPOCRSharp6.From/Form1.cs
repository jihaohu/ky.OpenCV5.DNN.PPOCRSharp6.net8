using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace OCRFrom.Test
{
    // Form1: 主窗口类，负责界面交互、调用本地 OCR DLL 并显示识别结果。
    public partial class Form1 : Form
    {
        /// <summary>
        /// 构造函数：初始化窗体组件。
        /// </summary>
        public Form1()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// P/Invoke: 初始化 OCR 引擎（本地 DLL）。
        /// 参数说明参见 C++ 动态库接口文档。
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int init(ref IntPtr engine, [MarshalAs(UnmanagedType.I1)] bool use_gpu, int gpu_id, string det_model_dir, int limit_side_len, double det_db_thresh, double det_db_box_thresh, double det_db_unclip_ratio, [MarshalAs(UnmanagedType.I1)] bool use_dilation, [MarshalAs(UnmanagedType.I1)] bool cls, [MarshalAs(UnmanagedType.I1)] bool use_angle_cls, string cls_model_dir, double cls_thresh, double cls_batch_num, string rec_model_dir, string rec_char_dict_path, int rec_batch_num, int rec_img_h, int rec_img_w, int predictor_num, StringBuilder msg);

        /// <summary>
        /// P/Invoke: 使用已创建的引擎对内存中的 cv::Mat* 图像进行识别。
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ocr(IntPtr engine, IntPtr image, StringBuilder msg, out IntPtr ocr_result, out int ocr_result_len);

        /// <summary>
        /// P/Invoke: 使用已创建的引擎对原始 BGR/GRAY/ BGRA 数据进行识别。
        /// 该接口由 C# 侧调用，直接传入 LockBits 获得的连续像素数组地址。
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ocr2(IntPtr engine, int rows, int cols, int channels, IntPtr data, StringBuilder msg, out IntPtr ocr_result, out int ocr_result_len);

        /// <summary>
        /// P/Invoke: 释放 Native 侧通过 malloc 返回的 OCR JSON 缓冲区。
        /// 这是跨平台释放方式，不能再使用 Windows COM 专用的 Marshal.FreeCoTaskMem。
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void free_ocr_result(IntPtr ocr_result);

        /// <summary>
        /// P/Invoke: 销毁并释放 OCR 引擎资源。
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int destroy(IntPtr engine, StringBuilder msg);

        /// <summary>
        /// 选择图片按钮点击事件：弹出文件对话框，加载所选图片并触发识别。
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = this.fileFilter;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.imgPath = openFileDialog.FileName;
                Bitmap bitmap = this.bmp;
                if (bitmap != null)
                {
                    bitmap.Dispose();
                }
                this.bmp = new Bitmap(this.imgPath);
                this.pictureBox1.Image = this.bmp;
                this.richTextBox1.Clear();
                this.button2_Click(null, null);
            }
        }

        /// <summary>
        /// 识别按钮点击事件：将当前图片转换为 BGR byte 数组，调用本地 ocr2 接口并在界面上绘制结果。
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            if (Form1.OCREngine == IntPtr.Zero)
            {
                MessageBox.Show("请先初始化！！！");
                return;
            }
            if (this.imgPath == null)
            {
                MessageBox.Show("请先选择图片！！！");
                return;
            }
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.richTextBox1.Clear();
            this.OCRResultInfo.Clear();
            this.OCRResultAllInfo.Clear();
            StringBuilder stringBuilder = new StringBuilder(128);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            IntPtr zero = IntPtr.Zero;
            int num = 0;
            string value = string.Empty;
            byte[] value2;
            int num2;
            using (Bitmap bitmap = this.LoadBgrBitmap(this.imgPath, out value2))
            {
                GCHandle gchandle = GCHandle.Alloc(value2, GCHandleType.Pinned);
                try
                {
                    num2 = Form1.ocr2(Form1.OCREngine, bitmap.Height, bitmap.Width, 3, gchandle.AddrOfPinnedObject(), stringBuilder, out zero, out num);
                }
                finally
                {
                    gchandle.Free();
                }
            }
            if (zero != IntPtr.Zero && num > 0)
            {
                byte[] array = new byte[num];
                Marshal.Copy(zero, array, 0, num);
                value = Encoding.UTF8.GetString(array);
                Form1.free_ocr_result(zero);
                zero = IntPtr.Zero;
            }
            stopwatch.Stop();
            double totalMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
            this.OCRResultAllInfo.AppendLine(string.Format("耗时: {0:F2}ms", totalMilliseconds));
            this.OCRResultAllInfo.AppendLine("---------------------------");
            this.OCRResultInfo.AppendLine(string.Format("耗时: {0:F2}ms", totalMilliseconds));
            this.OCRResultInfo.AppendLine("---------------------------");
            if (num2 == 0)
            {
                this.ltOCRResult = JsonConvert.DeserializeObject<List<OCRResult>>(value);
                this.OCRResultAllInfo.Append(JsonConvert.SerializeObject(this.ltOCRResult, Formatting.Indented));
                Graphics graphics = Graphics.FromImage(this.bmp);
                foreach (OCRResult ocrresult in this.ltOCRResult)
                {
                    this.OCRResultInfo.AppendLine(ocrresult.text);
                    Point[] points = new Point[]
                    {
                        new Point(ocrresult.x1, ocrresult.y1),
                        new Point(ocrresult.x2, ocrresult.y2),
                        new Point(ocrresult.x3, ocrresult.y3),
                        new Point(ocrresult.x4, ocrresult.y4)
                    };
                    graphics.DrawPolygon(this.pen, points);
                }
                graphics.Dispose();
                if (this.checkBox1.Checked)
                {
                    this.richTextBox1.Text = this.OCRResultAllInfo.ToString();
                }
                else
                {
                    this.richTextBox1.Text = this.OCRResultInfo.ToString();
                }
                this.pictureBox1.Image = null;
                this.pictureBox1.Image = this.bmp;
            }
            else
            {
                if (zero != IntPtr.Zero)
                {
                    Form1.free_ocr_result(zero);
                }
                MessageBox.Show("识别失败，" + stringBuilder.ToString());
            }
            this.button1.Enabled = true;
            this.button2.Enabled = true;
        }

        /// <summary>
        /// 窗体加载事件：设置默认模型选项并加载默认图片。
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            this.rdov6tiny.Checked = true;
            this.chkcls.Checked = false;
            this.LoadDefaultImage();
        }

        /// <summary>
        /// 切换是否显示全部识别信息（切换富文本框内容）。
        /// </summary>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.richTextBox1.Clear();
            if (this.checkBox1.Checked)
            {
                this.richTextBox1.Text = this.OCRResultAllInfo.ToString();
                return;
            }
            this.richTextBox1.Text = this.OCRResultInfo.ToString();
        }

        /// <summary>
        /// OCR 模型单选项改变事件（保留空实现以兼容 Designer 绑定）。
        /// </summary>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 窗体关闭事件：在关闭前释放模型资源。
        /// </summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.UnloadModel();
        }

        /// <summary>
        /// 手动释放模型按钮点击事件（调用 UnloadModel）。
        /// </summary>
        private void btnDestroy_Click(object sender, EventArgs e)
        {
            this.UnloadModel();
        }

        /// <summary>
        /// 卸载并销毁已初始化的 OCR 引擎实例。
        /// </summary>
        private void UnloadModel()
        {
            if (Form1.OCREngine != IntPtr.Zero)
            {
                StringBuilder stringBuilder = new StringBuilder(128);
                Form1.destroy(Form1.OCREngine, stringBuilder);
                this.AppendStatus("释放成功: " + stringBuilder.ToString());
                Form1.OCREngine = IntPtr.Zero;
            }
        }

        /// <summary>
        /// 初始化按钮点击事件：如果已有引擎则先释放，再加载模型；否则直接加载模型。
        /// </summary>
        private void btnInit_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Text = "";
            if (Form1.OCREngine != IntPtr.Zero)
            {
                StringBuilder stringBuilder = new StringBuilder(128);
                Form1.destroy(Form1.OCREngine, stringBuilder);
                this.AppendStatus("释放成功: " + stringBuilder.ToString());
                Form1.OCREngine = IntPtr.Zero;
                this.LoadModel();
                return;
            }
            this.LoadModel();
        }

        /// <summary>
        /// 根据界面选择的模型配置初始化并加载检测/识别/（可选）分类模型。
        /// 调用本地 init 接口并在界面输出加载结果或错误信息。
        /// </summary>
        private void LoadModel()
        {
            StringBuilder stringBuilder = new StringBuilder(128);
            bool use_gpu = false;
            int gpu_id = 0;
            int limit_side_len = 960;
            bool use_dilation = false;
            bool use_angle_cls = true;
            string cls_model_dir = "";
            double cls_thresh = 0.9;
            int rec_img_h = 48;
            int rec_img_w = 320;
            double det_db_thresh = Convert.ToDouble(this.txtdet_db_thresh.Text.ToString());
            double det_db_box_thresh = Convert.ToDouble(this.txtdet_db_box_thresh.Text.ToString());
            double det_db_unclip_ratio = Convert.ToDouble(this.txtdet_db_unclip_ratio.Text.ToString());
            bool cls = this.chkcls.Checked;
            int num = Convert.ToInt32(this.txtcls_batch_num.Text.ToString());
            int rec_batch_num = Convert.ToInt32(this.txtrec_batch_num.Text.ToString());
            int predictor_num = Convert.ToInt32(this.txtpredictor_num.Text.ToString());
            string text;
            string text2;
            string text3;
            if (this.rdomobile.Checked)
            {
                text = "inference/PP-OCRv5_mobile_det_onnx.onnx";
                text2 = "inference/PP-OCRv5_mobile_rec_onnx.onnx";
                cls_model_dir = "inference/PP-OCRv5_mobile_cls_onnx.onnx";
                text3 = "inference/ppocrv5_dict.txt";
            }
            else if (this.rdov6small.Checked)
            {
                text = "inference/PP-OCRv6_small_det.onnx";
                text2 = "inference/PP-OCRv6_small_rec.onnx";
                text3 = "inference/PP-OCRv6_small_rec_dict.txt";
                cls = false;
                use_angle_cls = false;
            }
            else if (this.rdov6tiny.Checked)
            {
                text = "inference/PP-OCRv6_tiny_det.onnx";
                text2 = "inference/PP-OCRv6_tiny_rec.onnx";
                text3 = "inference/PP-OCRv6_tiny_rec_dict.txt";
                cls = false;
                use_angle_cls = false;
            }
            else
            {
                text = "inference/PP-OCRv5_server_det_infer.onnx";
                text2 = "inference/PP-OCRv5_server_rec_infer.onnx";
                text3 = "inference/ppocrv5_dict.txt";
                cls = false;
                use_angle_cls = false;
            }
            this.AppendStatus("正在初始化模型...");
            this.AppendStatus("det: " + text);
            this.AppendStatus("rec: " + text2);
            this.AppendStatus("dict: " + text3);
            this.AppendStatus("device: CPU(OpenCV DNN)");
            if (Form1.init(ref Form1.OCREngine, use_gpu, gpu_id, text, limit_side_len, det_db_thresh, det_db_box_thresh, det_db_unclip_ratio, use_dilation, cls, use_angle_cls, cls_model_dir, cls_thresh, (double)num, text2, text3, rec_batch_num, rec_img_h, rec_img_w, predictor_num, stringBuilder) == 0)
            {
                this.AppendStatus("模型加载成功: " + stringBuilder.ToString());
                return;
            }
            string str = stringBuilder.ToString();
            this.AppendStatus("模型加载失败: " + str);
            MessageBox.Show("模型加载失败，" + str);
        }

        /// <summary>
        /// 在富文本状态框中追加带时间戳的日志信息。
        /// </summary>
        private void AppendStatus(string text)
        {
            this.richTextBox1.AppendText(string.Format("[{0:HH:mm:ss}] {1}{2}", DateTime.Now, text, Environment.NewLine));
        }

        /// <summary>
        /// 加载图片并返回 24bpp BGR 格式的 Bitmap，同时输出连续的 BGR 字节数组（行首连续，不含 stride 填充）。
        /// </summary>
        private Bitmap LoadBgrBitmap(string path, out byte[] bgrData)
        {
            Bitmap result;
            using (Bitmap bitmap = new Bitmap(path))
            {
                Bitmap bitmap2 = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
                using (Graphics graphics = Graphics.FromImage(bitmap2))
                {
                    graphics.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
                }
                Rectangle rect = new Rectangle(0, 0, bitmap2.Width, bitmap2.Height);
                BitmapData bitmapData = bitmap2.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                try
                {
                    int num = bitmap2.Width * 3;
                    bgrData = new byte[num * bitmap2.Height];
                    for (int i = 0; i < bitmap2.Height; i++)
                    {
                        Marshal.Copy(IntPtr.Add(bitmapData.Scan0, i * bitmapData.Stride), bgrData, i * num, num);
                    }
                }
                finally
                {
                    bitmap2.UnlockBits(bitmapData);
                }
                result = bitmap2;
            }
            return result;
        }

        /// <summary>
        /// 加载项目输出目录下的默认示例图片（inference\1.jpg），若不存在则忽略。
        /// </summary>
        private void LoadDefaultImage()
        {
            string path = Path.Combine(Application.StartupPath, "inference", "1.jpg");
            if (!File.Exists(path))
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inference", "1.jpg");
            }
            if (!File.Exists(path))
            {
                return;
            }
            this.imgPath = path;
            Bitmap bitmap = this.bmp;
            if (bitmap != null)
            {
                bitmap.Dispose();
            }
            this.bmp = new Bitmap(this.imgPath);
            this.pictureBox1.Image = this.bmp;
        }

        // 本地 OCR 动态库名称。
        private const string DllName = "ky.OpenCVDNN.PPOCRSharp.dll";

        // OCR 引擎句柄（由本地 DLL 初始化和释放）。
        private static IntPtr OCREngine;

        // 当前显示的图片位图。
        private Bitmap bmp;

        // 当前图片路径。
        private string imgPath;

        // OCR 识别结果集合。
        private List<OCRResult> ltOCRResult;

        // 打开文件对话框支持的图片筛选器。
        private string fileFilter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tif;*.png";

        // 简要识别输出缓存。
        private StringBuilder OCRResultInfo = new StringBuilder();

        // 完整识别输出缓存（包含 JSON）。
        private StringBuilder OCRResultAllInfo = new StringBuilder();

        // 绘制检测框的画笔。
        private Pen pen = new Pen(Brushes.Red, 2f);
    }
}
