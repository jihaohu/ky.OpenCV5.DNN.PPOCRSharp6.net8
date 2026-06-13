namespace OCRFrom.Test
{
    // Form1 设计器代码（由 WinForms 设计器维护）。
    public partial class Form1 : global::System.Windows.Forms.Form
    {
        /// <summary>
        /// 释放设计器组件资源。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 初始化窗体上的所有控件及其属性。
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.rdomobile = new System.Windows.Forms.RadioButton();
            this.rdoserver = new System.Windows.Forms.RadioButton();
            this.rdov6small = new System.Windows.Forms.RadioButton();
            this.rdov6tiny = new System.Windows.Forms.RadioButton();
            this.btnInit = new System.Windows.Forms.Button();
            this.chkcls = new System.Windows.Forms.CheckBox();
            this.txtrec_batch_num = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtpredictor_num = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtdet_db_thresh = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtdet_db_box_thresh = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtdet_db_unclip_ratio = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblcls_batch_num = new System.Windows.Forms.Label();
            this.txtcls_batch_num = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(286, 144);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 34);
            this.button1.TabIndex = 0;
            this.button1.Text = "选择图片";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(420, 144);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(128, 34);
            this.button2.TabIndex = 1;
            this.button2.Text = "识别";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(12, 191);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(536, 623);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(569, 191);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBox1.Size = new System.Drawing.Size(522, 623);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(569, 162);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(96, 16);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "显示全部信息";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // rdomobile
            // 
            this.rdomobile.AutoSize = true;
            this.rdomobile.Location = new System.Drawing.Point(13, 20);
            this.rdomobile.Name = "rdomobile";
            this.rdomobile.Size = new System.Drawing.Size(77, 16);
            this.rdomobile.TabIndex = 5;
            this.rdomobile.TabStop = true;
            this.rdomobile.Text = "v5 mobile";
            this.rdomobile.UseVisualStyleBackColor = true;
            this.rdomobile.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // rdoserver
            // 
            this.rdoserver.AutoSize = true;
            this.rdoserver.Location = new System.Drawing.Point(103, 20);
            this.rdoserver.Name = "rdoserver";
            this.rdoserver.Size = new System.Drawing.Size(77, 16);
            this.rdoserver.TabIndex = 6;
            this.rdoserver.TabStop = true;
            this.rdoserver.Text = "v5 server";
            this.rdoserver.UseVisualStyleBackColor = true;
            this.rdoserver.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // rdov6small
            // 
            this.rdov6small.AutoSize = true;
            this.rdov6small.Location = new System.Drawing.Point(13, 45);
            this.rdov6small.Name = "rdov6small";
            this.rdov6small.Size = new System.Drawing.Size(71, 16);
            this.rdov6small.TabIndex = 25;
            this.rdov6small.TabStop = true;
            this.rdov6small.Text = "v6 small";
            this.rdov6small.UseVisualStyleBackColor = true;
            this.rdov6small.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // rdov6tiny
            // 
            this.rdov6tiny.AutoSize = true;
            this.rdov6tiny.Location = new System.Drawing.Point(103, 45);
            this.rdov6tiny.Name = "rdov6tiny";
            this.rdov6tiny.Size = new System.Drawing.Size(65, 16);
            this.rdov6tiny.TabIndex = 26;
            this.rdov6tiny.TabStop = true;
            this.rdov6tiny.Text = "v6 tiny";
            this.rdov6tiny.UseVisualStyleBackColor = true;
            this.rdov6tiny.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(893, 17);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(198, 112);
            this.btnInit.TabIndex = 7;
            this.btnInit.Text = "初始化";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // chkcls
            // 
            this.chkcls.AutoSize = true;
            this.chkcls.Location = new System.Drawing.Point(8, 20);
            this.chkcls.Name = "chkcls";
            this.chkcls.Size = new System.Drawing.Size(42, 16);
            this.chkcls.TabIndex = 9;
            this.chkcls.Text = "cls";
            this.chkcls.UseVisualStyleBackColor = true;
            // 
            // txtrec_batch_num
            // 
            this.txtrec_batch_num.Location = new System.Drawing.Point(130, 19);
            this.txtrec_batch_num.Name = "txtrec_batch_num";
            this.txtrec_batch_num.Size = new System.Drawing.Size(58, 21);
            this.txtrec_batch_num.TabIndex = 19;
            this.txtrec_batch_num.Text = "4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(35, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 18;
            this.label5.Text = "rec_batch_num:";
            // 
            // txtpredictor_num
            // 
            this.txtpredictor_num.Location = new System.Drawing.Point(130, 48);
            this.txtpredictor_num.Name = "txtpredictor_num";
            this.txtpredictor_num.Size = new System.Drawing.Size(58, 21);
            this.txtpredictor_num.TabIndex = 21;
            this.txtpredictor_num.Text = "4";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(113, 12);
            this.label6.TabIndex = 20;
            this.label6.Text = "rec_predictor_num:";
            // 
            // txtdet_db_thresh
            // 
            this.txtdet_db_thresh.Location = new System.Drawing.Point(138, 17);
            this.txtdet_db_thresh.Name = "txtdet_db_thresh";
            this.txtdet_db_thresh.Size = new System.Drawing.Size(58, 21);
            this.txtdet_db_thresh.TabIndex = 30;
            this.txtdet_db_thresh.Text = "0.3";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(43, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 12);
            this.label9.TabIndex = 29;
            this.label9.Text = "det_db_thresh:";
            // 
            // txtdet_db_box_thresh
            // 
            this.txtdet_db_box_thresh.Location = new System.Drawing.Point(138, 48);
            this.txtdet_db_box_thresh.Name = "txtdet_db_box_thresh";
            this.txtdet_db_box_thresh.Size = new System.Drawing.Size(58, 21);
            this.txtdet_db_box_thresh.TabIndex = 32;
            this.txtdet_db_box_thresh.Text = "0.6";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(19, 53);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(113, 12);
            this.label10.TabIndex = 31;
            this.label10.Text = "det_db_box_thresh:";
            // 
            // txtdet_db_unclip_ratio
            // 
            this.txtdet_db_unclip_ratio.Location = new System.Drawing.Point(138, 83);
            this.txtdet_db_unclip_ratio.Name = "txtdet_db_unclip_ratio";
            this.txtdet_db_unclip_ratio.Size = new System.Drawing.Size(58, 21);
            this.txtdet_db_unclip_ratio.TabIndex = 34;
            this.txtdet_db_unclip_ratio.Text = "1.6";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 86);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(125, 12);
            this.label11.TabIndex = 33;
            this.label11.Text = "det_db_unclip_ratio:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtrec_batch_num);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtpredictor_num);
            this.groupBox1.Location = new System.Drawing.Point(681, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(206, 117);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "rec";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtdet_db_unclip_ratio);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.txtdet_db_thresh);
            this.groupBox2.Controls.Add(this.txtdet_db_box_thresh);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Location = new System.Drawing.Point(247, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(214, 117);
            this.groupBox2.TabIndex = 36;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "det";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rdov6tiny);
            this.groupBox3.Controls.Add(this.rdov6small);
            this.groupBox3.Controls.Add(this.rdomobile);
            this.groupBox3.Controls.Add(this.rdoserver);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(228, 117);
            this.groupBox3.TabIndex = 37;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ocr";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.chkcls);
            this.groupBox4.Controls.Add(this.lblcls_batch_num);
            this.groupBox4.Controls.Add(this.txtcls_batch_num);
            this.groupBox4.Location = new System.Drawing.Point(468, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(206, 117);
            this.groupBox4.TabIndex = 36;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "cls";
            // 
            // lblcls_batch_num
            // 
            this.lblcls_batch_num.AutoSize = true;
            this.lblcls_batch_num.Location = new System.Drawing.Point(6, 50);
            this.lblcls_batch_num.Name = "lblcls_batch_num";
            this.lblcls_batch_num.Size = new System.Drawing.Size(89, 12);
            this.lblcls_batch_num.TabIndex = 18;
            this.lblcls_batch_num.Text = "cls_batch_num:";
            // 
            // txtcls_batch_num
            // 
            this.txtcls_batch_num.Location = new System.Drawing.Point(101, 47);
            this.txtcls_batch_num.Name = "txtcls_batch_num";
            this.txtcls_batch_num.Size = new System.Drawing.Size(58, 21);
            this.txtcls_batch_num.TabIndex = 19;
            this.txtcls_batch_num.Text = "8";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1107, 825);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnInit);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ky.OpenCVDNN.PPOCRSharp.Test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        // 设计器组件容器。
        private global::System.ComponentModel.IContainer components;

        // 选择图片按钮。
        private global::System.Windows.Forms.Button button1;

        // 识别按钮。
        private global::System.Windows.Forms.Button button2;

        // 图片预览区域。
        private global::System.Windows.Forms.PictureBox pictureBox1;

        // 识别结果文本区域。
        private global::System.Windows.Forms.RichTextBox richTextBox1;

        // 显示全部信息复选框。
        private global::System.Windows.Forms.CheckBox checkBox1;

        // v5 mobile 模型单选框。
        private global::System.Windows.Forms.RadioButton rdomobile;

        // v5 server 模型单选框。
        private global::System.Windows.Forms.RadioButton rdoserver;

        // v6 small 模型单选框。
        private global::System.Windows.Forms.RadioButton rdov6small;

        // v6 tiny 模型单选框。
        private global::System.Windows.Forms.RadioButton rdov6tiny;

        // 初始化按钮。
        private global::System.Windows.Forms.Button btnInit;

        // 分类模型开关复选框。
        private global::System.Windows.Forms.CheckBox chkcls;

        // rec_batch_num 输入框。
        private global::System.Windows.Forms.TextBox txtrec_batch_num;

        // rec_batch_num 标签。
        private global::System.Windows.Forms.Label label5;

        // predictor_num 输入框。
        private global::System.Windows.Forms.TextBox txtpredictor_num;

        // predictor_num 标签。
        private global::System.Windows.Forms.Label label6;

        // det_db_thresh 输入框。
        private global::System.Windows.Forms.TextBox txtdet_db_thresh;

        // det_db_thresh 标签。
        private global::System.Windows.Forms.Label label9;

        // det_db_box_thresh 输入框。
        private global::System.Windows.Forms.TextBox txtdet_db_box_thresh;

        // det_db_box_thresh 标签。
        private global::System.Windows.Forms.Label label10;

        // det_db_unclip_ratio 输入框。
        private global::System.Windows.Forms.TextBox txtdet_db_unclip_ratio;

        // det_db_unclip_ratio 标签。
        private global::System.Windows.Forms.Label label11;

        // rec 参数分组。
        private global::System.Windows.Forms.GroupBox groupBox1;

        // det 参数分组。
        private global::System.Windows.Forms.GroupBox groupBox2;

        // 模型选择分组。
        private global::System.Windows.Forms.GroupBox groupBox3;

        // cls 参数分组。
        private global::System.Windows.Forms.GroupBox groupBox4;

        // cls_batch_num 标签。
        private global::System.Windows.Forms.Label lblcls_batch_num;

        // cls_batch_num 输入框。
        private global::System.Windows.Forms.TextBox txtcls_batch_num;
    }
}
