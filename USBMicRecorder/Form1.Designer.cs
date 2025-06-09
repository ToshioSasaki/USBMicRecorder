namespace USBMicRecorder
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            button1 = new Button();
            textBox1 = new TextBox();
            btnStart = new Button();
            groupBox2 = new ThickGroupBox();
            lblPlay = new Label();
            label7 = new Label();
            cbPlayDevice = new ComboBox();
            label6 = new Label();
            button2 = new Button();
            btnPlay = new Button();
            label5 = new Label();
            btnPlayDir = new Button();
            txtPlayPath = new TextBox();
            trkVolume = new TrackBar();
            GroupBox1 = new ThickGroupBox();
            label4 = new Label();
            label3 = new Label();
            lblCapture = new Label();
            label2 = new Label();
            label1 = new Label();
            pbLevel = new ProgressBar();
            btnBrowse = new Button();
            txtSavePath = new TextBox();
            btnRecord = new Button();
            tbVolume = new TrackBar();
            btnTest = new Button();
            cbDevices = new ComboBox();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trkVolume).BeginInit();
            GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tbVolume).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Font = new Font("游ゴシック", 8.25F);
            button1.Location = new Point(370, 24);
            button1.Name = "button1";
            button1.Size = new Size(75, 25);
            button1.TabIndex = 18;
            button1.Text = "参照";
            button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(67, 24);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(297, 23);
            textBox1.TabIndex = 17;
            // 
            // btnStart
            // 
            btnStart.Font = new Font("游ゴシック", 8.25F);
            btnStart.Location = new Point(370, 65);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(75, 25);
            btnStart.TabIndex = 16;
            btnStart.Text = "再生";
            btnStart.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.BorderThickness = 2F;
            groupBox2.Controls.Add(lblPlay);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(cbPlayDevice);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(button2);
            groupBox2.Controls.Add(btnPlay);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(btnPlayDir);
            groupBox2.Controls.Add(txtPlayPath);
            groupBox2.Controls.Add(trkVolume);
            groupBox2.Location = new Point(27, 205);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(485, 149);
            groupBox2.TabIndex = 13;
            groupBox2.TabStop = false;
            groupBox2.Text = "再生";
            // 
            // lblPlay
            // 
            lblPlay.AutoSize = true;
            lblPlay.Font = new Font("游ゴシック", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 128);
            lblPlay.ForeColor = Color.LimeGreen;
            lblPlay.Location = new Point(412, 32);
            lblPlay.Name = "lblPlay";
            lblPlay.Size = new Size(47, 17);
            lblPlay.TabIndex = 23;
            lblPlay.Text = "再生中";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(7, 32);
            label7.Name = "label7";
            label7.Size = new Size(73, 14);
            label7.TabIndex = 22;
            label7.Text = "出力デバイス";
            // 
            // cbPlayDevice
            // 
            cbPlayDevice.FormattingEnabled = true;
            cbPlayDevice.Location = new Point(96, 29);
            cbPlayDevice.Name = "cbPlayDevice";
            cbPlayDevice.Size = new Size(289, 22);
            cbPlayDevice.TabIndex = 21;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(29, 93);
            label6.Name = "label6";
            label6.Size = new Size(51, 14);
            label6.TabIndex = 16;
            label6.Text = "再生音量";
            // 
            // button2
            // 
            button2.Font = new Font("游ゴシック", 8.25F);
            button2.Location = new Point(397, 93);
            button2.Name = "button2";
            button2.Size = new Size(35, 25);
            button2.TabIndex = 14;
            button2.Text = "■";
            button2.UseVisualStyleBackColor = true;
            // 
            // btnPlay
            // 
            btnPlay.Font = new Font("游ゴシック", 8.25F);
            btnPlay.Location = new Point(439, 93);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(35, 25);
            btnPlay.TabIndex = 13;
            btnPlay.Text = "▶";
            btnPlay.UseVisualStyleBackColor = true;
            btnPlay.Click += btnPlay_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(27, 62);
            label5.Name = "label5";
            label5.Size = new Size(51, 14);
            label5.TabIndex = 12;
            label5.Text = "出力パス";
            // 
            // btnPlayDir
            // 
            btnPlayDir.Font = new Font("游ゴシック", 8.25F);
            btnPlayDir.Location = new Point(397, 57);
            btnPlayDir.Name = "btnPlayDir";
            btnPlayDir.Size = new Size(75, 25);
            btnPlayDir.TabIndex = 11;
            btnPlayDir.Text = "参照";
            btnPlayDir.UseVisualStyleBackColor = true;
            btnPlayDir.Click += btnPlayDir_Click;
            // 
            // txtPlayPath
            // 
            txtPlayPath.Location = new Point(96, 57);
            txtPlayPath.Name = "txtPlayPath";
            txtPlayPath.Size = new Size(287, 25);
            txtPlayPath.TabIndex = 10;
            // 
            // trkVolume
            // 
            trkVolume.Location = new Point(96, 93);
            trkVolume.Name = "trkVolume";
            trkVolume.Size = new Size(289, 45);
            trkVolume.TabIndex = 15;
            // 
            // GroupBox1
            // 
            GroupBox1.BorderThickness = 2F;
            GroupBox1.Controls.Add(label4);
            GroupBox1.Controls.Add(label3);
            GroupBox1.Controls.Add(lblCapture);
            GroupBox1.Controls.Add(label2);
            GroupBox1.Controls.Add(label1);
            GroupBox1.Controls.Add(pbLevel);
            GroupBox1.Controls.Add(btnBrowse);
            GroupBox1.Controls.Add(txtSavePath);
            GroupBox1.Controls.Add(btnRecord);
            GroupBox1.Controls.Add(tbVolume);
            GroupBox1.Controls.Add(btnTest);
            GroupBox1.Controls.Add(cbDevices);
            GroupBox1.Location = new Point(25, 18);
            GroupBox1.Name = "GroupBox1";
            GroupBox1.Size = new Size(485, 175);
            GroupBox1.TabIndex = 14;
            GroupBox1.TabStop = false;
            GroupBox1.Text = "録音";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(42, 116);
            label4.Name = "label4";
            label4.Size = new Size(40, 14);
            label4.TabIndex = 24;
            label4.Text = "レベル";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(29, 85);
            label3.Name = "label3";
            label3.Size = new Size(51, 14);
            label3.TabIndex = 23;
            label3.Text = "入力音量";
            // 
            // lblCapture
            // 
            lblCapture.AutoSize = true;
            lblCapture.Font = new Font("游ゴシック", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 128);
            lblCapture.ForeColor = Color.IndianRed;
            lblCapture.Location = new Point(414, 24);
            lblCapture.Name = "lblCapture";
            lblCapture.Size = new Size(47, 17);
            lblCapture.TabIndex = 22;
            lblCapture.Text = "録音中";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(31, 54);
            label2.Name = "label2";
            label2.Size = new Size(51, 14);
            label2.TabIndex = 21;
            label2.Text = "入力パス";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(9, 24);
            label1.Name = "label1";
            label1.Size = new Size(73, 14);
            label1.TabIndex = 20;
            label1.Text = "入力デバイス";
            // 
            // pbLevel
            // 
            pbLevel.Location = new Point(98, 113);
            pbLevel.Name = "pbLevel";
            pbLevel.Size = new Size(378, 21);
            pbLevel.TabIndex = 19;
            // 
            // btnBrowse
            // 
            btnBrowse.Font = new Font("游ゴシック", 8.25F);
            btnBrowse.Location = new Point(401, 49);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(75, 25);
            btnBrowse.TabIndex = 18;
            btnBrowse.Text = "参照";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click_1;
            // 
            // txtSavePath
            // 
            txtSavePath.Location = new Point(98, 49);
            txtSavePath.Name = "txtSavePath";
            txtSavePath.Size = new Size(289, 25);
            txtSavePath.TabIndex = 17;
            // 
            // btnRecord
            // 
            btnRecord.Font = new Font("游ゴシック", 8.25F);
            btnRecord.Location = new Point(401, 140);
            btnRecord.Name = "btnRecord";
            btnRecord.Size = new Size(75, 25);
            btnRecord.TabIndex = 16;
            btnRecord.Text = "録音";
            btnRecord.UseVisualStyleBackColor = true;
            btnRecord.Click += btnRecord_Click_1;
            // 
            // tbVolume
            // 
            tbVolume.Location = new Point(98, 80);
            tbVolume.Name = "tbVolume";
            tbVolume.Size = new Size(289, 45);
            tbVolume.TabIndex = 15;
            // 
            // btnTest
            // 
            btnTest.Font = new Font("游ゴシック", 8.25F);
            btnTest.Location = new Point(312, 140);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(75, 25);
            btnTest.TabIndex = 14;
            btnTest.Text = "入力テスト";
            btnTest.UseVisualStyleBackColor = true;
            btnTest.Click += btnTest_Click_1;
            // 
            // cbDevices
            // 
            cbDevices.FormattingEnabled = true;
            cbDevices.Location = new Point(98, 21);
            cbDevices.Name = "cbDevices";
            cbDevices.Size = new Size(289, 22);
            cbDevices.TabIndex = 13;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(6F, 14F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightGray;
            ClientSize = new Size(538, 385);
            Controls.Add(GroupBox1);
            Controls.Add(groupBox2);
            Font = new Font("游ゴシック", 8.25F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "USBデバイス録音";
            Load += Form1_Load;
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trkVolume).EndInit();
            GroupBox1.ResumeLayout(false);
            GroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tbVolume).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button button1;
        private TextBox textBox1;
        private Button btnStart;
        private ThickGroupBox groupBox2;
        private Label label5;
        private Button btnPlayDir;
        private TextBox txtPlayPath;
        private Button btnPlay;
        private TrackBar trkVolume;
        private Button button2;
        private Label label6;
        private ThickGroupBox GroupBox1;
        private Label label4;
        private Label label3;
        private Label lblCapture;
        private Label label2;
        private Label label1;
        private ProgressBar pbLevel;
        private Button btnBrowse;
        private TextBox txtSavePath;
        private Button btnRecord;
        private TrackBar tbVolume;
        private Button btnTest;
        private ComboBox cbDevices;
        private Label label7;
        private ComboBox cbPlayDevice;
        private Label lblPlay;
    }
}
