namespace SmallGreen.Desktop.Test
{
    partial class Form2
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
            button2 = new Button();
            label1 = new Label();
            button1 = new Button();
            dgvQcl1 = new DataGridView();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            richTextBox1 = new RichTextBox();
            tabPage2 = new TabPage();
            richTextBox2 = new RichTextBox();
            dgvQcl2 = new DataGridView();
            tabPage3 = new TabPage();
            richTextBox3 = new RichTextBox();
            dgvGs = new DataGridView();
            button4 = new Button();
            txtValue = new TextBox();
            button3 = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvQcl1).BeginInit();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvQcl2).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGs).BeginInit();
            SuspendLayout();
            // 
            // button2
            // 
            button2.Location = new Point(12, 12);
            button2.Name = "button2";
            button2.Size = new Size(92, 23);
            button2.TabIndex = 1;
            button2.Text = "创建小绿模型";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(208, 15);
            label1.Name = "label1";
            label1.Size = new Size(65, 17);
            label1.TabIndex = 2;
            label1.Text = "未连接PLC";
            // 
            // button1
            // 
            button1.Location = new Point(110, 12);
            button1.Name = "button1";
            button1.Size = new Size(92, 23);
            button1.TabIndex = 3;
            button1.Text = "启动数据池";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // dgvQcl1
            // 
            dgvQcl1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvQcl1.Location = new Point(6, 6);
            dgvQcl1.Name = "dgvQcl1";
            dgvQcl1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvQcl1.Size = new Size(926, 507);
            dgvQcl1.TabIndex = 6;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(12, 41);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1359, 551);
            tabControl1.TabIndex = 7;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(richTextBox1);
            tabPage1.Controls.Add(dgvQcl1);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1351, 521);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "1#前处理";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(938, 6);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(407, 507);
            richTextBox1.TabIndex = 10;
            richTextBox1.Text = "";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(richTextBox2);
            tabPage2.Controls.Add(dgvQcl2);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1351, 521);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "2#前处理";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // richTextBox2
            // 
            richTextBox2.Location = new Point(938, 6);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.Size = new Size(407, 507);
            richTextBox2.TabIndex = 11;
            richTextBox2.Text = "";
            // 
            // dgvQcl2
            // 
            dgvQcl2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvQcl2.Location = new Point(6, 6);
            dgvQcl2.Name = "dgvQcl2";
            dgvQcl2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvQcl2.Size = new Size(926, 507);
            dgvQcl2.TabIndex = 7;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(richTextBox3);
            tabPage3.Controls.Add(dgvGs);
            tabPage3.Location = new Point(4, 26);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(1351, 521);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "固色";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // richTextBox3
            // 
            richTextBox3.Location = new Point(938, 6);
            richTextBox3.Name = "richTextBox3";
            richTextBox3.Size = new Size(407, 507);
            richTextBox3.TabIndex = 11;
            richTextBox3.Text = "";
            // 
            // dgvGs
            // 
            dgvGs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvGs.Location = new Point(6, 6);
            dgvGs.Name = "dgvGs";
            dgvGs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGs.Size = new Size(926, 507);
            dgvGs.TabIndex = 7;
            // 
            // button4
            // 
            button4.Location = new Point(1289, 15);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 8;
            button4.Text = "读写测试";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // txtValue
            // 
            txtValue.Location = new Point(22, 603);
            txtValue.Name = "txtValue";
            txtValue.Size = new Size(1263, 23);
            txtValue.TabIndex = 9;
            // 
            // button3
            // 
            button3.Location = new Point(1296, 603);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 10;
            button3.Text = " 写入";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1383, 776);
            Controls.Add(button3);
            Controls.Add(txtValue);
            Controls.Add(button4);
            Controls.Add(tabControl1);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(button2);
            Name = "Form2";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Form2";
            ((System.ComponentModel.ISupportInitialize)dgvQcl1).EndInit();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvQcl2).EndInit();
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvGs).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button2;
        private Label label1;
        private Button button1;
        private DataGridView dgvQcl1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private DataGridView dgvQcl2;
        private TabPage tabPage3;
        private DataGridView dgvGs;
        private Button button4;
        private RichTextBox richTextBox1;
        private RichTextBox richTextBox2;
        private RichTextBox richTextBox3;
        private TextBox txtValue;
        private Button button3;
    }
}