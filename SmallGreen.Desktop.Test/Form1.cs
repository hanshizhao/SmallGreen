using S7.Net;
using S7.Net.Types;
using SmallGreen.Entity.Basic;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace SmallGreen.Desktop.Test
{
    public partial class Form1 : Form
    {
        public Plc? Plc { get; set; }
        public DataItem? Item { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox2.Items.AddRange(
                [
                    DataType.DataBlock.ToString(),
                    DataType.Memory.ToString(),
                    DataType.Counter.ToString(),
                    DataType.Timer.ToString(),
                    DataType.Input.ToString(),
                    DataType.Output.ToString(),

                ]);

            comboBox3.Items.AddRange(
                [
                    VarType.String.ToString(),
                    VarType.Bit.ToString(),
                    VarType.Byte.ToString(),
                    VarType.DInt.ToString(),
                    VarType.DWord.ToString(),
                    VarType.Int.ToString(),
                    VarType.Word.ToString(),
                    VarType.Real.ToString(),
                ]);

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            // 连接PLC
            try
            {
                var rack = short.Parse(textBox6.Text);
                var slot = short.Parse(textBox7.Text);

                Plc = comboBox1.SelectedIndex switch
                {
                    0 => new Plc(CpuType.S7200Smart, "192.168.21.30", rack, slot),
                    1 => new Plc(CpuType.S7200Smart, "192.168.21.31", rack, slot),
                    2 => new Plc(CpuType.S71200, "192.168.21.200", rack, slot),
                    _ => throw new Exception("啥？" + comboBox1.SelectedIndex),
                };

                button1.Enabled = false;
                await Plc.OpenAsync();

                if (Plc.IsConnected)
                {
                    button2.Enabled = true;
                    groupBox1.Enabled = true;
                    MessageBox.Show("连接成功");
                }
                else
                {
                    button1.Enabled = true;
                    MessageBox.Show("没连上");
                }


            }
            catch (Exception ex)
            {
                button1.Enabled = true;
                MessageBox.Show(ex.Message);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 断开PLC
            if (Plc != null)
            {
                Plc.Close();
                Plc = null;
            }

            button1.Enabled = true;
            button2.Enabled = false;
            groupBox1.Enabled = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // 抓
            var dataType = comboBox2.SelectedIndex switch
            {
                0 => DataType.DataBlock,
                1 => DataType.Memory,
                2 => DataType.Counter,
                3 => DataType.Timer,
                4 => DataType.Input,
                5 => DataType.Output,
                _ => throw new Exception("dataType？" + comboBox2.SelectedIndex),
            };

            var varType = comboBox3.SelectedIndex switch
            {
                0 => VarType.String,
                1 => VarType.Bit,
                2 => VarType.Byte,
                3 => VarType.DInt,
                4 => VarType.DWord,
                5 => VarType.Int,
                6 => VarType.Word,
                7 => VarType.Real,
                _ => throw new Exception("varType？" + comboBox3.SelectedIndex),
            };

            var db = int.Parse(textBox3.Text);
            var startByteAdr = int.Parse(textBox4.Text);
            var bitAdr = byte.Parse(textBox5.Text);
            var count = int.Parse(textBox8.Text);
            Item = new DataItem
            {
                DataType = dataType,
                VarType = varType,
                DB = db,
                StartByteAdr = startByteAdr,
                BitAdr = bitAdr,
                Count = count,
            };

            panel2.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // 放
            Item = null;
            panel2.Enabled = false;
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            // 监控
            if (Plc == null)
            {
                MessageBox.Show("PLC没初始化");
                return;
            }

            if (!Plc.IsConnected)
            {
                MessageBox.Show("PLC没连接");
                return;
            }

            if (Item == null)
            {
                MessageBox.Show("点位没初始化");
                return;
            }

            var begin = System.DateTime.Now;

            var valueBytes = await Plc.ReadBytesAsync(Item.DataType, Item.DB, Item.StartByteAdr, Item.Count);

            var end = System.DateTime.Now;

            label9.Text = (end - begin).TotalMilliseconds.ToString() + " ms";

            string value = "no data";
            switch (Item.VarType)
            {
                case VarType.Real: value = BitConverter.ToSingle(valueBytes.Reverse().ToArray()).ToString(); break;
                case VarType.Bit:
                    var fb = valueBytes[0];
                    var pb = (byte)(1 << Item.BitAdr);
                    value = (fb & pb) != 0 ? bool.TrueString : bool.FalseString;
                    break;
                case VarType.Int: value = BitConverter.ToInt16(valueBytes.Reverse().ToArray()).ToString(); break;
                case VarType.Word: value = BitConverter.ToUInt16(valueBytes.Reverse().ToArray()).ToString(); break;
                case VarType.Byte: value = valueBytes[Item.StartByteAdr].ToString(); break;
                case VarType.String: value = Encoding.ASCII.GetString(valueBytes); break;
                default: throw new Exception("啥？" + Item.VarType);
            }

            string xx = "";
            for (int i = 0; i < valueBytes.Length; i++)
            {
                xx += valueBytes[i].ToString() + " ";
            }

            richTextBox1.Text = xx;

            //xx += "__| " + Encoding.ASCII.GetString(valueBytes) + " |__";
            //xx += "__| " + Encoding.UTF8.GetString(valueBytes) + " |__";
            //MessageBox.Show(xx);
            textBox1.Text = value;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 写入DOM
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }
    }
}
