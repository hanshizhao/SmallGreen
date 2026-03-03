using S7.Net;
using SmallGreen.Entity.Basic;
using SmallGreen.Entity.Machine;
using System.Data;
using System.Text;

namespace SmallGreen.Desktop.Test
{
    public partial class Form2 : Form
    {
        public List<SubSystem>? ListSubSystem { get; set; }

        public Form2()
        {
            InitializeComponent();

            dgvQcl1.CellDoubleClick += DgvQcl_CellDoubleClick;
            dgvQcl2.CellDoubleClick += DgvQcl_CellDoubleClick;
            dgvGs.CellDoubleClick += DgvQcl_CellDoubleClick;
        }

        private async Task<List<SubSystem>> InitListSubSystem()
        {
            return null;
            //List<SubSystem> listSubSystem = await new Repository<SubSystem>().Context.Queryable<SubSystem>()
            //    .Includes(it => it.PLC)
            //    .Includes(it => it.ListEquipment)
            //    .Includes(it => it.ListEquipment, e => e.ListBulk)
            //    .ToListAsync();

            //var listEquipment = await new Repository<Equipment>().Context.Queryable<Equipment>()
            //    .Includes<>(it => it.ListBulk)
            //    .ToListAsync();

            //foreach (var subSystem in listSubSystem)
            //{
            //    subSystem.ListEquipment = listEquipment.FindAll(it => it.SubSystemId == subSystem.Id);
            //}

            //listSubSystem[0].PLC.ConnectionS7Plc([new(1, 0, 18970)]);
            //listSubSystem[1].PLC.ConnectionS7Plc([new(1, 0, 18270)]);

            //listSubSystem[2].PLC.ConnectionS7Plc([
            //    new(101, 0, 354),
            //    new(102, 0, 897),
            //    new(103, 0, 897),
            //    new(104, 0, 897),
            //    new(105, 0, 897),
            //    new(106, 0, 897),
            //    new(107, 0, 897),
            //    new(108, 0, 897),
            //    ]);

            //return listSubSystem;
        }


        private async void button2_Click(object sender, EventArgs e)
        {
            ListSubSystem ??= await InitListSubSystem();

            MessageBox.Show("小绿初始化完毕");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            // 启动
            ListSubSystem ??= await InitListSubSystem();

            await Task.Run(async () =>
            {
                this.label1.Invoke(new Action(() =>
                {
                    label1.Text = $"reading....";
                }));
                //while (RefreshRunning)
                //{
                var strTimeSpan = string.Empty;
                var timeSpanTotal = TimeSpan.Zero;
                foreach (var subSystem in ListSubSystem)
                {
                    var t1 = DateTime.Now;
                    var result = await subSystem.CheckRuntime();
                    var t1_1 = DateTime.Now;
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show(result.Message);
                    }

                    var strByteArray = string.Empty;
                    if (subSystem.PLC.ListDataPool != null)
                    {
                        foreach (var dataPool in subSystem.PLC.ListDataPool)
                        {
                            var dd = dataPool.Data;
                            if (dd != null) strByteArray += ByteArrayToString(dd);
                        }
                    }

                    if (subSystem.SubSystemName == Common.SubSystemName.QCL1)
                    {
                        dgvQcl1.Invoke(new Action(() =>
                        {
                            dgvQcl1.DataSource = ListToDataTable(subSystem.ListDom);
                        }));

                        richTextBox1.Invoke(new Action(() =>
                        {
                            richTextBox1.Text = strByteArray;
                        }));
                    }
                    else if (subSystem.SubSystemName == Common.SubSystemName.QCL2)
                    {
                        dgvQcl2.Invoke(new Action(() =>
                        {
                            dgvQcl2.DataSource = ListToDataTable(subSystem.ListDom);
                        }));

                        richTextBox2.Invoke(new Action(() =>
                        {
                            richTextBox2.Text = strByteArray;
                        }));
                    }
                    else
                    {
                        dgvGs.Invoke(new Action(() =>
                        {
                            dgvGs.DataSource = ListToDataTable(subSystem.ListDom);
                        }));

                        richTextBox3.Invoke(new Action(() =>
                        {
                            richTextBox3.Text = strByteArray;
                        }));
                    }
                    var ts = t1_1 - t1;
                    timeSpanTotal += ts;

                    strTimeSpan += ts.TotalSeconds + "s ";

                    this.label1.Invoke(new Action(() =>
                    {
                        label1.Text = $"{subSystem.PLC.Name} ok, {ts.TotalSeconds} s  reading....";
                    }));
                }


                this.label1.Invoke(new Action(() =>
                {
                    label1.Text = $"PLC数据池刷新：{strTimeSpan} --> {timeSpanTotal.TotalSeconds}";
                }));

                //}  // while
            });
        }
        public DataTable ListToDataTable(List<IDom>? list)
        {
            DataTable dataTable = new DataTable();

            if (list == null) return dataTable;

            dataTable.Columns.Add("DataType");
            dataTable.Columns.Add("VarType");
            dataTable.Columns.Add("DB");
            dataTable.Columns.Add("StartByteAdr");
            dataTable.Columns.Add("BitAdr");
            dataTable.Columns.Add("Count");
            dataTable.Columns.Add("Value");

            foreach (IDom it in list)
            {
                DataRow dr = dataTable.NewRow();

                dr["DataType"] = it.DataItem.DataType.ToString();
                dr["VarType"] = it.DataItem.VarType.ToString();
                dr["DB"] = it.DataItem.DB.ToString();
                dr["StartByteAdr"] = it.DataItem.StartByteAdr.ToString();
                dr["BitAdr"] = it.DataItem.BitAdr.ToString();
                dr["Count"] = it.DataItem.Count.ToString();
                dr["Value"] = it.DataItem.Value?.ToString();

                dataTable.Rows.Add(dr);
            }

            return dataTable;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new Form1().Show();
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            ListSubSystem ??= await InitListSubSystem();
            var dd = ListSubSystem[0].PLC.ListDataPool[0].Data;
            richTextBox1.Text = "no dd";


        }

        private async void DgvQcl_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            ListSubSystem ??= await InitListSubSystem();
            var subSystem = ListSubSystem[tabControl1.SelectedIndex];
            var plc = subSystem.PLC;
            var listDom = subSystem.ListDom;
            if (listDom == null) return;
            if (plc == null) return;

            var dom = listDom[e.RowIndex];
            txtValue.Text = dom.DataItem.Value?.ToString();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            var value = txtValue.Text.Trim();
            if (string.IsNullOrEmpty(value)) return;
            ListSubSystem ??= await InitListSubSystem();
            var subSystem = ListSubSystem[tabControl1.SelectedIndex];
            var plc = subSystem.PLC;
            var listDom = subSystem.ListDom;
            if (listDom == null) return;
            if (plc == null) return;

            var rowIndex = tabControl1.SelectedIndex switch
            {
                0 => dgvQcl1.SelectedRows[0].Index,
                1 => dgvQcl2.SelectedRows[0].Index,
                2 => dgvGs.SelectedRows[0].Index,
                _ => throw new Exception("选项卡没有这个选项哦"),
            };
            var idom = listDom[rowIndex];
            var result = new OperateResult();

            if (idom.DataItem.VarType == VarType.Real)
            {
                var dom = idom as Dom<float>;
                if (float.TryParse(value, out float v))
                {
                    dom.NewValue = v;
                    result = await plc.Write(dom);
                }
            }
            else if (idom.DataItem.VarType == VarType.Bit)
            {
                var dom = idom as Dom<bool>;
                if (bool.TryParse(value, out bool v))
                {
                    dom.NewValue = v;
                    result = await plc.Write(dom);
                }
            }
            else if (idom.DataItem.VarType == VarType.Word)
            {
                var dom = idom as Dom<ushort>;
                if (ushort.TryParse(value, out ushort v))
                {
                    dom.NewValue = v;
                    result = await plc.Write(dom);
                }
            }
            else if (idom.DataItem.VarType == VarType.String)
            {
                var bytes = Encoding.ASCII.GetBytes(value);
                label1.Text = ByteArrayToString(bytes);
                result = await plc.Write(idom, bytes);
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "没有这个Dom类型" + idom.DataItem.VarType;
            }

            if (result.IsSuccess)
            {
                MessageBox.Show("写入成功");
            }
            else
            {
                MessageBox.Show(result.Message);
            }
        }

        private string ByteArrayToString(byte[] dd)
        {
            string xx = "";
            for (int i = 0; i < dd.Length; i++)
            {
                xx += dd[i].ToString() + " ";
            }
            return xx;
        }


    }
}
