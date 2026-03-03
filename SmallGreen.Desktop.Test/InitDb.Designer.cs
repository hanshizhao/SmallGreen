namespace SmallGreen.Desktop.Test
{
    partial class InitDb
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            label1 = new Label();
            textBox1 = new TextBox();
            label2 = new Label();
            button2 = new Button();
            button4 = new Button();
            button5 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(28, 77);
            button1.Name = "button1";
            button1.Size = new Size(108, 23);
            button1.TabIndex = 0;
            button1.Text = "生成字符串";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(28, 26);
            label1.Name = "label1";
            label1.Size = new Size(68, 17);
            label1.TabIndex = 1;
            label1.Text = "服务器地址";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(28, 48);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(205, 23);
            textBox1.TabIndex = 2;
            textBox1.Text = ".";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(142, 83);
            label2.Name = "label2";
            label2.Size = new Size(48, 17);
            label2.TabIndex = 3;
            label2.Text = "[result]";
            // 
            // button2
            // 
            button2.Location = new Point(28, 115);
            button2.Name = "button2";
            button2.Size = new Size(162, 23);
            button2.TabIndex = 4;
            button2.Text = "连接并创建数据库";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button4
            // 
            button4.Location = new Point(196, 115);
            button4.Name = "button4";
            button4.Size = new Size(162, 23);
            button4.TabIndex = 6;
            button4.Text = "创建表";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Location = new Point(28, 161);
            button5.Name = "button5";
            button5.Size = new Size(162, 23);
            button5.TabIndex = 7;
            button5.Text = "塞种子数据";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // InitDb
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(461, 215);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button2);
            Controls.Add(label2);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(button1);
            Name = "InitDb";
            Text = "InitDb";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private TextBox textBox1;
        private Label label2;
        private Button button2;
        private Button button4;
        private Button button5;
    }
}