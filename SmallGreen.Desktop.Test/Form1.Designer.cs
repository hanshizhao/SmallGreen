namespace SmallGreen.Desktop.Test
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
            comboBox1 = new ComboBox();
            label1 = new Label();
            button1 = new Button();
            button2 = new Button();
            comboBox2 = new ComboBox();
            groupBox1 = new GroupBox();
            richTextBox1 = new RichTextBox();
            label9 = new Label();
            panel2 = new Panel();
            button3 = new Button();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            button4 = new Button();
            button6 = new Button();
            panel1 = new Panel();
            textBox8 = new TextBox();
            label10 = new Label();
            label3 = new Label();
            comboBox3 = new ComboBox();
            textBox5 = new TextBox();
            label4 = new Label();
            label7 = new Label();
            label5 = new Label();
            textBox4 = new TextBox();
            textBox3 = new TextBox();
            label6 = new Label();
            button5 = new Button();
            textBox6 = new TextBox();
            label2 = new Label();
            textBox7 = new TextBox();
            label8 = new Label();
            button7 = new Button();
            groupBox1.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "1#前处理PLC(S7 200 Smart) - 192.168.21.30", "2#前处理PLC(S7200 Smart) - 192.168.21.31", "1#固色PLC(S7 1200) - 192.168.21.200" });
            comboBox1.Location = new Point(22, 38);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(294, 25);
            comboBox1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(22, 18);
            label1.Name = "label1";
            label1.Size = new Size(53, 17);
            label1.TabIndex = 1;
            label1.Text = "选择PLC";
            // 
            // button1
            // 
            button1.Location = new Point(440, 40);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 2;
            button1.Text = "连接PLC";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Enabled = false;
            button2.Location = new Point(521, 40);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 4;
            button2.Text = "断开PLC";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // comboBox2
            // 
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(3, 26);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(185, 25);
            comboBox2.TabIndex = 5;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(richTextBox1);
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(panel2);
            groupBox1.Controls.Add(button6);
            groupBox1.Controls.Add(panel1);
            groupBox1.Controls.Add(button5);
            groupBox1.Enabled = false;
            groupBox1.Location = new Point(22, 98);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(570, 343);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "操作点位";
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(226, 234);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(338, 96);
            richTextBox1.TabIndex = 25;
            richTextBox1.Text = "";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(304, 214);
            label9.Name = "label9";
            label9.Size = new Size(15, 17);
            label9.TabIndex = 24;
            label9.Text = "0";
            // 
            // panel2
            // 
            panel2.Controls.Add(button3);
            panel2.Controls.Add(textBox1);
            panel2.Controls.Add(textBox2);
            panel2.Controls.Add(button4);
            panel2.Enabled = false;
            panel2.Location = new Point(295, 124);
            panel2.Name = "panel2";
            panel2.Size = new Size(220, 79);
            panel2.TabIndex = 23;
            // 
            // button3
            // 
            button3.Location = new Point(17, 16);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 6;
            button3.Text = "监控";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(98, 16);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(100, 23);
            textBox1.TabIndex = 7;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(17, 45);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(100, 23);
            textBox2.TabIndex = 8;
            // 
            // button4
            // 
            button4.Location = new Point(123, 45);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 9;
            button4.Text = "写入DOM";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button6
            // 
            button6.Location = new Point(226, 180);
            button6.Name = "button6";
            button6.Size = new Size(51, 51);
            button6.TabIndex = 22;
            button6.Text = "放";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(textBox8);
            panel1.Controls.Add(label10);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(comboBox2);
            panel1.Controls.Add(comboBox3);
            panel1.Controls.Add(textBox5);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label7);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(textBox4);
            panel1.Controls.Add(textBox3);
            panel1.Controls.Add(label6);
            panel1.Location = new Point(18, 22);
            panel1.Name = "panel1";
            panel1.Size = new Size(202, 315);
            panel1.TabIndex = 21;
            // 
            // textBox8
            // 
            textBox8.Location = new Point(8, 289);
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(180, 23);
            textBox8.TabIndex = 21;
            textBox8.Text = "1";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(8, 269);
            label10.Name = "label10";
            label10.Size = new Size(42, 17);
            label10.TabIndex = 20;
            label10.Text = "Count";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 6);
            label3.Name = "label3";
            label3.Size = new Size(63, 17);
            label3.TabIndex = 10;
            label3.Text = "DataType";
            // 
            // comboBox3
            // 
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.FormattingEnabled = true;
            comboBox3.Location = new Point(3, 79);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(185, 25);
            comboBox3.TabIndex = 11;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(8, 235);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(180, 23);
            textBox5.TabIndex = 19;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(3, 59);
            label4.Name = "label4";
            label4.Size = new Size(56, 17);
            label4.TabIndex = 12;
            label4.Text = "VarType";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(8, 215);
            label7.Name = "label7";
            label7.Size = new Size(44, 17);
            label7.TabIndex = 18;
            label7.Text = "BitAdr";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(8, 109);
            label5.Name = "label5";
            label5.Size = new Size(25, 17);
            label5.TabIndex = 14;
            label5.Text = "DB";
            // 
            // textBox4
            // 
            textBox4.Location = new Point(9, 184);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(180, 23);
            textBox4.TabIndex = 17;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(8, 129);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(180, 23);
            textBox3.TabIndex = 15;
            textBox3.Text = "100";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(9, 164);
            label6.Name = "label6";
            label6.Size = new Size(81, 17);
            label6.TabIndex = 16;
            label6.Text = "StartByteAdr";
            // 
            // button5
            // 
            button5.Location = new Point(226, 87);
            button5.Name = "button5";
            button5.Size = new Size(51, 51);
            button5.TabIndex = 20;
            button5.Text = "抓";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // textBox6
            // 
            textBox6.Location = new Point(326, 40);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(33, 23);
            textBox6.TabIndex = 19;
            textBox6.Text = "0";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(326, 20);
            label2.Name = "label2";
            label2.Size = new Size(33, 17);
            label2.TabIndex = 18;
            label2.Text = "rack";
            // 
            // textBox7
            // 
            textBox7.Location = new Point(376, 40);
            textBox7.Name = "textBox7";
            textBox7.Size = new Size(33, 23);
            textBox7.TabIndex = 21;
            textBox7.Text = "0";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(376, 20);
            label8.Name = "label8";
            label8.Size = new Size(29, 17);
            label8.TabIndex = 20;
            label8.Text = "slot";
            // 
            // button7
            // 
            button7.Location = new Point(362, 71);
            button7.Name = "button7";
            button7.Size = new Size(75, 23);
            button7.TabIndex = 22;
            button7.Text = "ADSAD";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(607, 469);
            Controls.Add(button7);
            Controls.Add(textBox7);
            Controls.Add(label8);
            Controls.Add(textBox6);
            Controls.Add(label2);
            Controls.Add(groupBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(comboBox1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox comboBox1;
        private Label label1;
        private Button button1;
        private Button button2;
        private ComboBox comboBox2;
        private GroupBox groupBox1;
        private Button button3;
        private TextBox textBox1;
        private TextBox textBox2;
        private Button button4;
        private Label label3;
        private Label label4;
        private ComboBox comboBox3;
        private Label label5;
        private TextBox textBox4;
        private Label label6;
        private TextBox textBox3;
        private TextBox textBox5;
        private Label label7;
        private Panel panel1;
        private Button button5;
        private Button button6;
        private Panel panel2;
        private TextBox textBox6;
        private Label label2;
        private TextBox textBox7;
        private Label label8;
        private Label label9;
        private TextBox textBox8;
        private Label label10;
        private RichTextBox richTextBox1;
        private Button button7;
    }
}
