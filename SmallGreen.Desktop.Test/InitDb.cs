using S7.Net;
using SmallGreen.Entity.Basic;
using SmallGreen.Entity.Machine;
using SqlSugar;

namespace SmallGreen.Desktop.Test
{
    public partial class InitDb : Form
    {
        public string? PreDbConnectString { get; set; }
        public string? DbConnectString { get; set; }

        private SqlSugarScope? db;

        public InitDb()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string dbServerAddress = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(dbServerAddress)) return;

            PreDbConnectString = $"server={dbServerAddress};uid=sa;pwd=123;Encrypt=True;TrustServerCertificate=True";
            DbConnectString = $"server={dbServerAddress};database=SmallGreenDB;uid=sa;pwd=123;Encrypt=True;TrustServerCertificate=True";

            label2.Text = DbConnectString;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var tempDb = new SqlSugarScope(new ConnectionConfig
                {
                    ConnectionString = PreDbConnectString,
                    DbType = SqlSugar.DbType.SqlServer,
                    IsAutoCloseConnection = true
                }, _db =>
                {
                    _db.Aop.OnLogExecuting = (sql, paras) =>
                    {
                        Console.WriteLine(UtilMethods.GetSqlString(SqlSugar.DbType.SqlServer, sql, paras));
                    };
                });

                await tempDb.Ado.ExecuteCommandAsync($"CREATE DATABASE SmallGreenDB");
                MessageBox.Show("数据库创建成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show("创建数据库时出现异常:" + ex.Message);
            }
        }

        private SqlSugarScope InitDbEntity()
        {
            return new SqlSugarScope(new ConnectionConfig
            {
                ConnectionString = DbConnectString,
                DbType = SqlSugar.DbType.SqlServer,
                IsAutoCloseConnection = true
            }, _db =>
            {
                _db.Aop.OnLogExecuting = (sql, paras) =>
                {
                    Console.WriteLine(UtilMethods.GetSqlString(SqlSugar.DbType.SqlServer, sql, paras));
                };
            });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 创建表
            db ??= InitDbEntity();

            db.CodeFirst.InitTables<SiemensPLC>();
            db.CodeFirst.InitTables<SubSystem>();
            db.CodeFirst.InitTables<Equipment>();
            db.CodeFirst.InitTables<Bulk>();

            try
            {
                MessageBox.Show("表创建成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show("创建表时出现异常:" + ex.Message);
            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            var listSubSystem = new List<SubSystem>();
            var qclPlcId1 = SnowFlakeSingle.instance.NextId();
            SubSystem qcl1 = new()
            {
                Id = SnowFlakeSingle.instance.NextId(),
                SubSystemName = SmallGreen.Common.SubSystemName.QCL1,
                PlcID = qclPlcId1,
                PLC = new SiemensPLC
                {
                    Id = qclPlcId1,
                    CpuType = CpuType.S7200Smart,
                    IPAddress = "192.168.21.30",
                    Name = "1#前处理PLC",
                    Rack = 0,
                    Slot = 0,
                },
                DataAssCoeArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 1901, 0, 88),
                ListEquipment = []
            };

            // 创建 3#退煮漂联合机-后段
            var qcl1_equip1_id = SnowFlakeSingle.instance.NextId();
            qcl1.ListEquipment.Add(new Equipment
            {
                Id = qcl1_equip1_id,
                CodeNumber = "123",
                Name = "3#退煮漂联合机-后段",
                BtnStart = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9816, 6, 1),
                BtnPageChange = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9816, 7, 1),
                DataCurrentPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9804, 0, 2),
                DataFinishedType = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9920, 0, 2),
                DataFomulaQueryStatus = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9938, 0, 2),
                DataTotalPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9802, 0, 2),
                DataWorkColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 710, 0, 2),
                DataWorkOrderInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 16801, 0, 69),
                Line1ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 700, 0, 2),
                Line1Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9806, 0, 2),
                Line1WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 16301, 0, 69),

                Line2ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 702, 0, 2),
                Line2Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9808, 0, 2),
                Line2WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 16401, 0, 69),

                Line3ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 704, 0, 2),
                Line3Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9810, 0, 2),
                Line3WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 16501, 0, 69),

                Line4ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 706, 0, 2),
                Line4Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9812, 0, 2),
                Line4WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 16601, 0, 69),

                Line5ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 708, 0, 2),
                Line5Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9814, 0, 2),
                Line5WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 16701, 0, 69),

                TriggerFinished = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9817, 5, 1),

                ListBulk =
                [
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "A",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 7001, 0, 88),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2256, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2400, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 12401, 0, 88),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 1700, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = qcl1_equip1_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "B",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 7201, 0, 88),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2260, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2404, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 12601, 0, 88),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 1702, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = qcl1_equip1_id,
                    },
                ]
            });

            // 1#退煮漂联合机
            var qcl1_equip2_id = SnowFlakeSingle.instance.NextId();
            qcl1.ListEquipment.Add(new Equipment
            {
                Id = qcl1_equip2_id,
                CodeNumber = "121",
                Name = "1#退煮漂联合机",
                BtnStart = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9836, 6, 1),
                BtnPageChange = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9836, 7, 1),
                DataCurrentPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9824, 0, 2),
                DataFinishedType = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9922, 0, 2),
                DataFomulaQueryStatus = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9940, 0, 2),
                DataTotalPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9822, 0, 2),
                DataWorkColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 722, 0, 2),
                DataWorkOrderInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17501, 0, 69),
                Line1ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 712, 0, 2),
                Line1Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9826, 0, 2),
                Line1WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17001, 0, 69),

                Line2ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 714, 0, 2),
                Line2Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9828, 0, 2),
                Line2WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17101, 0, 69),

                Line3ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 716, 0, 2),
                Line3Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9830, 0, 2),
                Line3WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17201, 0, 69),

                Line4ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 718, 0, 2),
                Line4Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9832, 0, 2),
                Line4WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17301, 0, 69),

                Line5ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 720, 0, 2),
                Line5Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9834, 0, 2),
                Line5WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17401, 0, 69),

                TriggerFinished = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9837, 5, 1),

                ListBulk =
                [
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "A",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 7401, 0, 88),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2264, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2408, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 12801, 0, 88),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 1704, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = qcl1_equip2_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "B",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 7601, 0, 88),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2268, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2412, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 13001, 0, 88),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 1706, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = qcl1_equip2_id,
                    },
                ]
            });

            // 冷堆机
            var qcl1_equip3_id = SnowFlakeSingle.instance.NextId();
            qcl1.ListEquipment.Add(new Equipment
            {
                Id = qcl1_equip3_id,
                CodeNumber = "141",
                Name = "冷堆机",
                BtnStart = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9856, 6, 1),
                BtnPageChange = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9856, 7, 1),
                DataCurrentPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9844, 0, 2),
                DataFinishedType = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9924, 0, 2),
                DataFomulaQueryStatus = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9942, 0, 2),
                DataTotalPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9842, 0, 2),
                DataWorkColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 734, 0, 2),
                DataWorkOrderInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 18201, 0, 69),
                Line1ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 724, 0, 2),
                Line1Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9846, 0, 2),
                Line1WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17701, 0, 69),

                Line2ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 726, 0, 2),
                Line2Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9848, 0, 2),
                Line2WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17801, 0, 69),

                Line3ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 728, 0, 2),
                Line3Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9850, 0, 2),
                Line3WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17901, 0, 69),

                Line4ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 730, 0, 2),
                Line4Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9852, 0, 2),
                Line4WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 18001, 0, 69),

                Line5ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 732, 0, 2),
                Line5Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9854, 0, 2),
                Line5WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 18101, 0, 69),

                TriggerFinished = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9857, 5, 1),

                ListBulk =
                [
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "A",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 7701, 0, 88),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2272, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2416, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 13201, 0, 88),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 1708, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = qcl1_equip3_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "B",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 7801, 0, 88),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2276, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2420, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 13401, 0, 88),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 1710, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = qcl1_equip3_id,
                    },
                ]
            });

            // 创建 3#退煮漂联合机-前段
            var qcl1_equip4_id = SnowFlakeSingle.instance.NextId();
            qcl1.ListEquipment.Add(new Equipment
            {
                Id = qcl1_equip4_id,
                CodeNumber = "123",
                Name = "3#退煮漂联合机-前段",
                BtnStart = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9876, 6, 1),
                BtnPageChange = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9876, 7, 1),
                DataCurrentPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9864, 0, 2),
                DataFinishedType = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9926, 0, 2),
                DataFomulaQueryStatus = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9944, 0, 2),
                DataTotalPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9862, 0, 2),
                DataWorkColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 746, 0, 2),
                DataWorkOrderInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 18901, 0, 69),
                Line1ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 736, 0, 2),
                Line1Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9866, 0, 2),
                Line1WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 18401, 0, 69),

                Line2ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 738, 0, 2),
                Line2Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9868, 0, 2),
                Line2WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 18501, 0, 69),

                Line3ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 740, 0, 2),
                Line3Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9870, 0, 2),
                Line3WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 18601, 0, 69),

                Line4ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 742, 0, 2),
                Line4Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9872, 0, 2),
                Line4WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 18701, 0, 69),

                Line5ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 744, 0, 2),
                Line5Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9874, 0, 2),
                Line5WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 18801, 0, 69),

                TriggerFinished = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9877, 5, 1),

                ListBulk =
                [
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "A",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 8001, 0, 88),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2280, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2424, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 13601, 0, 88),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 1712, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = qcl1_equip4_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "B",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 8201, 0, 88),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2284, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2428, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 13801, 0, 88),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 1714, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId =qcl1_equip4_id,
                    },
                ]
            });

            // 前处理2
            var qclPlcId2 = SnowFlakeSingle.instance.NextId();
            SubSystem qcl2 = new()
            {
                Id = SnowFlakeSingle.instance.NextId(),
                SubSystemName = Common.SubSystemName.QCL2,
                PlcID = qclPlcId2,
                PLC = new SiemensPLC
                {
                    Id = qclPlcId2,
                    CpuType = CpuType.S7200Smart,
                    IPAddress = "192.168.21.31",
                    Name = "2#前处理PLC",
                    Rack = 0,
                    Slot = 0,
                },
                DataAssCoeArray = null,
                ListEquipment = []
            };

            // 创建 4#退煮漂联合机
            var qcl2_equip1_id = SnowFlakeSingle.instance.NextId();
            qcl2.ListEquipment.Add(new Equipment
            {
                Id = qcl2_equip1_id,
                CodeNumber = "124",
                Name = "4#退煮漂联合机",
                BtnStart = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9816, 6, 1),
                BtnPageChange = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9816, 7, 1),
                DataCurrentPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9804, 0, 2),
                DataFinishedType = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9920, 0, 2),
                DataFomulaQueryStatus = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9938, 0, 2),
                DataTotalPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9802, 0, 2),
                DataWorkColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 710, 0, 2),
                DataWorkOrderInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 16801, 0, 69),
                Line1ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 700, 0, 2),
                Line1Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9806, 0, 2),
                Line1WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 16301, 0, 69),

                Line2ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 702, 0, 2),
                Line2Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9808, 0, 2),
                Line2WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 16401, 0, 69),

                Line3ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 704, 0, 2),
                Line3Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9810, 0, 2),
                Line3WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 16501, 0, 69),

                Line4ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 706, 0, 2),
                Line4Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9812, 0, 2),
                Line4WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 16601, 0, 69),

                Line5ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 708, 0, 2),
                Line5Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9814, 0, 2),
                Line5WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 16701, 0, 69),

                TriggerFinished = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9817, 5, 1),

                ListBulk =
                [
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "A",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 7001, 0, 88),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2256, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2400, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 12401, 0, 88),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 1700, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = qcl2_equip1_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "B",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 7201, 0, 88),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2260, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2404, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 12601, 0, 88),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 1702, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = qcl2_equip1_id,
                    },
                ]
            });

            // 2#退煮漂联合机-后段
            var qcl2_equip2_id = SnowFlakeSingle.instance.NextId();
            qcl2.ListEquipment.Add(new Equipment
            {
                Id = qcl2_equip2_id,
                CodeNumber = "122",
                Name = "2#退煮漂联合机-后段",
                BtnStart = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9836, 6, 1),
                BtnPageChange = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9836, 7, 1),
                DataCurrentPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9824, 0, 2),
                DataFinishedType = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9922, 0, 2),
                DataFomulaQueryStatus = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9940, 0, 2),
                DataTotalPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9822, 0, 2),
                DataWorkColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 722, 0, 2),
                DataWorkOrderInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17501, 0, 69),
                Line1ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 712, 0, 2),
                Line1Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9826, 0, 2),
                Line1WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17001, 0, 69),

                Line2ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 714, 0, 2),
                Line2Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9828, 0, 2),
                Line2WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17101, 0, 69),

                Line3ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 716, 0, 2),
                Line3Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9830, 0, 2),
                Line3WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17201, 0, 69),

                Line4ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 718, 0, 2),
                Line4Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9832, 0, 2),
                Line4WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17301, 0, 69),

                Line5ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 720, 0, 2),
                Line5Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9834, 0, 2),
                Line5WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17401, 0, 69),

                TriggerFinished = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9837, 5, 1),

                ListBulk =
                [
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "A",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 7401, 0, 88),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2264, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2408, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 12801, 0, 88),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 1704, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = qcl2_equip2_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "B",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 7601, 0, 88),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2268, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2412, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 13001, 0, 88),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 1706, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = qcl2_equip2_id,
                    },
                ]
            });

            // 2#退煮漂联合机-前段
            var qcl2_equip3_id = SnowFlakeSingle.instance.NextId();
            qcl2.ListEquipment.Add(new Equipment
            {
                Id = qcl2_equip3_id,
                CodeNumber = "122",
                Name = "2#退煮漂联合机-前段",
                BtnStart = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9856, 6, 1),
                BtnPageChange = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9856, 7, 1),
                DataCurrentPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9844, 0, 2),
                DataFinishedType = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9924, 0, 2),
                DataFomulaQueryStatus = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9942, 0, 2),
                DataTotalPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9842, 0, 2),
                DataWorkColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 734, 0, 2),
                DataWorkOrderInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 18201, 0, 69),
                Line1ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 724, 0, 2),
                Line1Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9846, 0, 2),
                Line1WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17701, 0, 69),

                Line2ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 726, 0, 2),
                Line2Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9848, 0, 2),
                Line2WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17801, 0, 69),

                Line3ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 728, 0, 2),
                Line3Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9850, 0, 2),
                Line3WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 17901, 0, 69),

                Line4ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 730, 0, 2),
                Line4Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9852, 0, 2),
                Line4WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 18001, 0, 69),

                Line5ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 732, 0, 2),
                Line5Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 9854, 0, 2),
                Line5WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 18101, 0, 69),

                TriggerFinished = new Dom<bool>(DataType.DataBlock, VarType.Bit, 1, 9857, 5, 1),

                ListBulk =
                [
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "A",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 7701, 0, 88),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2272, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2416, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 13201, 0, 88),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 1708, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = qcl2_equip3_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "B",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 7801, 0, 88),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2276, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 1, 2420, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 1, 13401, 0, 88),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 1, 1710, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = qcl2_equip3_id,
                    },
                ]
            });


            // 固色
            var gsPlcId = SnowFlakeSingle.instance.NextId();
            SubSystem gs = new()
            {
                Id = SnowFlakeSingle.instance.NextId(),
                SubSystemName = Common.SubSystemName.GS1,
                PlcID = gsPlcId,
                PLC = new SiemensPLC
                {
                    Id = gsPlcId,
                    CpuType = CpuType.S71200,
                    IPAddress = "192.168.21.200",
                    Name = "固色PLC",
                    Rack = 0,
                    Slot = 0,
                },
                DataAssCoeArray = new Dom<string>(DataType.DataBlock, VarType.String, 101, 330, 0, 24),
                ListEquipment = []
            };

            // 创建 修色机A
            var gs_equip1_id = SnowFlakeSingle.instance.NextId();
            gs.ListEquipment.Add(new Equipment
            {
                Id = gs_equip1_id,
                CodeNumber = "141",
                Name = "修色机A",
                BtnStart = new Dom<bool>(DataType.DataBlock, VarType.Bit, 102, 414, 6, 1),
                BtnPageChange = new Dom<bool>(DataType.DataBlock, VarType.Bit, 102, 414, 7, 1),
                DataCurrentPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 422, 0, 2),
                DataFinishedType = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 434, 0, 2),
                DataFomulaQueryStatus = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 436, 0, 2),
                DataTotalPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 420, 0, 2),
                DataWorkColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 466, 0, 2),
                DataWorkOrderInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 102, 828, 0, 69),
                Line1ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 456, 0, 2),
                Line1Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 424, 0, 2),
                Line1WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 102, 468, 0, 69),

                Line2ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 458, 0, 2),
                Line2Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 426, 0, 2),
                Line2WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 102, 540, 0, 69),

                Line3ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 460, 0, 2),
                Line3Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 428, 0, 2),
                Line3WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 102, 612, 0, 69),

                Line4ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 462, 0, 2),
                Line4Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 430, 0, 2),
                Line4WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 102, 684, 0, 69),

                Line5ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 464, 0, 2),
                Line5Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 432, 0, 2),
                Line5WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 102, 756, 0, 69),

                TriggerFinished = new Dom<bool>(DataType.DataBlock, VarType.Bit, 102, 415, 5, 1),

                ListBulk =
                [
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "A",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 102, 84, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 102, 36, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 102, 60, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 102, 110, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 78, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip1_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "B",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 102, 222, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 102, 174, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 102, 198, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 102, 248, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 216, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip1_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "C",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 102, 360, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 102, 312, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 102, 336, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 102, 386, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 102, 354, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip1_id,
                    },
                ]
            });

            // 2#染色机
            var gs_equip2_id = SnowFlakeSingle.instance.NextId();
            gs.ListEquipment.Add(new Equipment
            {
                Id = gs_equip2_id,
                CodeNumber = "203",
                Name = "2#染色机",
                BtnStart = new Dom<bool>(DataType.DataBlock, VarType.Bit, 103, 414, 6, 1),
                BtnPageChange = new Dom<bool>(DataType.DataBlock, VarType.Bit, 103, 414, 7, 1),
                DataCurrentPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 422, 0, 2),
                DataFinishedType = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 434, 0, 2),
                DataFomulaQueryStatus = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 436, 0, 2),
                DataTotalPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 420, 0, 2),
                DataWorkColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 466, 0, 2),
                DataWorkOrderInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 103, 828, 0, 69),
                Line1ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 456, 0, 2),
                Line1Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 424, 0, 2),
                Line1WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 103, 468, 0, 69),

                Line2ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 458, 0, 2),
                Line2Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 426, 0, 2),
                Line2WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 103, 540, 0, 69),

                Line3ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 460, 0, 2),
                Line3Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 428, 0, 2),
                Line3WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 103, 612, 0, 69),

                Line4ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 462, 0, 2),
                Line4Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 430, 0, 2),
                Line4WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 103, 684, 0, 69),

                Line5ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 464, 0, 2),
                Line5Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 432, 0, 2),
                Line5WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 103, 756, 0, 69),

                TriggerFinished = new Dom<bool>(DataType.DataBlock, VarType.Bit, 103, 415, 5, 1),

                ListBulk =
                [
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "A",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 103, 84, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 103, 36, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 103, 60, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 103, 110, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 78, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip2_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "B",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 103, 222, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 103, 174, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 103, 198, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 103, 248, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 216, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip2_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "C",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 103, 360, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 103, 312, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 103, 336, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 103, 386, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 103, 354, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip2_id,
                    },
                ]
            });

            // 1#染色机
            var gs_equip3_id = SnowFlakeSingle.instance.NextId();
            gs.ListEquipment.Add(new Equipment
            {
                Id = gs_equip3_id,
                CodeNumber = "202",
                Name = "1#染色机",
                BtnStart = new Dom<bool>(DataType.DataBlock, VarType.Bit, 104, 414, 6, 1),
                BtnPageChange = new Dom<bool>(DataType.DataBlock, VarType.Bit, 104, 414, 7, 1),
                DataCurrentPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 422, 0, 2),
                DataFinishedType = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 434, 0, 2),
                DataFomulaQueryStatus = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 436, 0, 2),
                DataTotalPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 420, 0, 2),
                DataWorkColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 466, 0, 2),
                DataWorkOrderInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 104, 828, 0, 69),
                Line1ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 456, 0, 2),
                Line1Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 424, 0, 2),
                Line1WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 104, 468, 0, 69),

                Line2ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 458, 0, 2),
                Line2Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 426, 0, 2),
                Line2WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 104, 540, 0, 69),

                Line3ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 460, 0, 2),
                Line3Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 428, 0, 2),
                Line3WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 104, 612, 0, 69),

                Line4ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 462, 0, 2),
                Line4Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 430, 0, 2),
                Line4WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 104, 684, 0, 69),

                Line5ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 464, 0, 2),
                Line5Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 432, 0, 2),
                Line5WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 104, 756, 0, 69),

                TriggerFinished = new Dom<bool>(DataType.DataBlock, VarType.Bit, 104, 415, 5, 1),

                ListBulk =
                [
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "A",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 104, 84, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 104, 36, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 104, 60, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 104, 110, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 78, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip3_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "B",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 104, 222, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 104, 174, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 104, 198, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 104, 248, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 216, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip3_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "C",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 104, 360, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 104, 312, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 104, 336, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 104, 386, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 104, 354, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip3_id,
                    },
                ]
            });

            // 创建 3#染色机
            var gs_equip4_id = SnowFlakeSingle.instance.NextId();
            gs.ListEquipment.Add(new Equipment
            {
                Id = gs_equip4_id,
                CodeNumber = "204",
                Name = "3#染色机",
                BtnStart = new Dom<bool>(DataType.DataBlock, VarType.Bit, 105, 414, 6, 1),
                BtnPageChange = new Dom<bool>(DataType.DataBlock, VarType.Bit, 105, 414, 7, 1),
                DataCurrentPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 422, 0, 2),
                DataFinishedType = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 434, 0, 2),
                DataFomulaQueryStatus = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 436, 0, 2),
                DataTotalPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 420, 0, 2),
                DataWorkColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 466, 0, 2),
                DataWorkOrderInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 105, 828, 0, 69),
                Line1ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 456, 0, 2),
                Line1Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 424, 0, 2),
                Line1WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 105, 468, 0, 69),

                Line2ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 458, 0, 2),
                Line2Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 426, 0, 2),
                Line2WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 105, 540, 0, 69),

                Line3ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 460, 0, 2),
                Line3Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 428, 0, 2),
                Line3WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 105, 612, 0, 69),

                Line4ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 462, 0, 2),
                Line4Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 430, 0, 2),
                Line4WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 105, 684, 0, 69),

                Line5ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 464, 0, 2),
                Line5Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 432, 0, 2),
                Line5WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 105, 756, 0, 69),

                TriggerFinished = new Dom<bool>(DataType.DataBlock, VarType.Bit, 105, 415, 5, 1),

                ListBulk =
                [
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "A",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 105, 84, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 105, 36, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 105, 60, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 105, 110, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 78, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip4_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "B",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 105, 222, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 105, 174, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 105, 198, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 105, 248, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 216, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip4_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "C",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 105, 360, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 105, 312, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 105, 336, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 105, 386, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 105, 354, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip4_id,
                    },
                ]
            });

            // 创建 4#染色机
            var gs_equip5_id = SnowFlakeSingle.instance.NextId();
            gs.ListEquipment.Add(new Equipment
            {
                Id = gs_equip5_id,
                CodeNumber = "205",
                Name = "4#染色机",
                BtnStart = new Dom<bool>(DataType.DataBlock, VarType.Bit, 106, 414, 6, 1),
                BtnPageChange = new Dom<bool>(DataType.DataBlock, VarType.Bit, 106, 414, 7, 1),
                DataCurrentPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 422, 0, 2),
                DataFinishedType = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 434, 0, 2),
                DataFomulaQueryStatus = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 436, 0, 2),
                DataTotalPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 420, 0, 2),
                DataWorkColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 466, 0, 2),
                DataWorkOrderInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 106, 828, 0, 69),
                Line1ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 456, 0, 2),
                Line1Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 424, 0, 2),
                Line1WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 106, 468, 0, 69),

                Line2ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 458, 0, 2),
                Line2Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 426, 0, 2),
                Line2WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 106, 540, 0, 69),

                Line3ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 460, 0, 2),
                Line3Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 428, 0, 2),
                Line3WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 106, 612, 0, 69),

                Line4ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 462, 0, 2),
                Line4Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 430, 0, 2),
                Line4WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 106, 684, 0, 69),

                Line5ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 464, 0, 2),
                Line5Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 432, 0, 2),
                Line5WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 106, 756, 0, 69),

                TriggerFinished = new Dom<bool>(DataType.DataBlock, VarType.Bit, 106, 415, 5, 1),

                ListBulk =
                [
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "A",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 106, 84, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 106, 36, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 106, 60, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 106, 110, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 78, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip5_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "B",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 106, 222, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 106, 174, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 106, 198, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 106, 248, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 216, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip5_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "C",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 106, 360, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 106, 312, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 106, 336, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 106, 386, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 106, 354, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip5_id,
                    },
                ]
            });

            // 创建 5#染色机
            var gs_equip6_id = SnowFlakeSingle.instance.NextId();
            gs.ListEquipment.Add(new Equipment
            {
                Id = gs_equip6_id,
                CodeNumber = "206",
                Name = " 5#染色机",
                BtnStart = new Dom<bool>(DataType.DataBlock, VarType.Bit, 107, 414, 6, 1),
                BtnPageChange = new Dom<bool>(DataType.DataBlock, VarType.Bit, 107, 414, 7, 1),
                DataCurrentPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 422, 0, 2),
                DataFinishedType = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 434, 0, 2),
                DataFomulaQueryStatus = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 436, 0, 2),
                DataTotalPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 420, 0, 2),
                DataWorkColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 466, 0, 2),
                DataWorkOrderInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 107, 828, 0, 69),
                Line1ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 456, 0, 2),
                Line1Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 424, 0, 2),
                Line1WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 107, 468, 0, 69),

                Line2ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 458, 0, 2),
                Line2Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 426, 0, 2),
                Line2WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 107, 540, 0, 69),

                Line3ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 460, 0, 2),
                Line3Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 428, 0, 2),
                Line3WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 107, 612, 0, 69),

                Line4ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 462, 0, 2),
                Line4Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 430, 0, 2),
                Line4WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 107, 684, 0, 69),

                Line5ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 464, 0, 2),
                Line5Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 432, 0, 2),
                Line5WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 107, 756, 0, 69),

                TriggerFinished = new Dom<bool>(DataType.DataBlock, VarType.Bit, 107, 415, 5, 1),

                ListBulk =
                [
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "A",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 107, 84, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 107, 36, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 107, 60, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 107, 110, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 78, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip6_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "B",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 107, 222, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 107, 174, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 107, 198, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 107, 248, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 216, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip6_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "C",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 107, 360, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 107, 312, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 107, 336, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 107, 386, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 107, 354, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip6_id,
                    },
                ]
            });

            // 创建 修色机B
            var gs_equip7_id = SnowFlakeSingle.instance.NextId();
            gs.ListEquipment.Add(new Equipment
            {
                Id = gs_equip7_id,
                CodeNumber = "208",
                Name = "修色机B",
                BtnStart = new Dom<bool>(DataType.DataBlock, VarType.Bit, 108, 414, 6, 1),
                BtnPageChange = new Dom<bool>(DataType.DataBlock, VarType.Bit, 108, 414, 7, 1),
                DataCurrentPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 422, 0, 2),
                DataFinishedType = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 434, 0, 2),
                DataFomulaQueryStatus = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 436, 0, 2),
                DataTotalPage = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 420, 0, 2),
                DataWorkColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 466, 0, 2),
                DataWorkOrderInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 108, 828, 0, 69),
                Line1ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 456, 0, 2),
                Line1Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 424, 0, 2),
                Line1WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 108, 468, 0, 69),

                Line2ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 458, 0, 2),
                Line2Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 426, 0, 2),
                Line2WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 108, 540, 0, 69),

                Line3ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 460, 0, 2),
                Line3Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 428, 0, 2),
                Line3WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 108, 612, 0, 69),

                Line4ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 462, 0, 2),
                Line4Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 430, 0, 2),
                Line4WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 108, 684, 0, 69),

                Line5ColorID = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 464, 0, 2),
                Line5Status = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 432, 0, 2),
                Line5WrokInfoArray = new Dom<string>(DataType.DataBlock, VarType.String, 108, 756, 0, 69),

                TriggerFinished = new Dom<bool>(DataType.DataBlock, VarType.Bit, 108, 415, 5, 1),

                ListBulk =
                [
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "A",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 108, 84, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 108, 36, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 108, 60, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 108, 110, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 78, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip7_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "B",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 108, 222, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 108, 174, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 108, 198, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 108, 248, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 216, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip7_id,
                    },
                    new()
                    {
                        Id = SnowFlakeSingle.instance.NextId(),
                        CodeNumber = "C",
                        DataFomulaArray = new Dom<string>(DataType.DataBlock, VarType.String, 108, 360, 0, 24),
                        DataLevel = new Dom<float>(DataType.DataBlock, VarType.Real, 108, 312, 0, 4),
                        DataPlanVolume  = new Dom<float>(DataType.DataBlock, VarType.Real, 108, 336, 0, 4),
                        DataRealLitreArray = new Dom<string>(DataType.DataBlock, VarType.String, 108, 386, 0, 24),
                        TriggerComplete = new Dom<ushort>(DataType.DataBlock, VarType.Word, 108, 354, 0, 2),
                        LastCompleteTime = DateTime.Now,
                        EquipmentId = gs_equip7_id,
                    },
                ]
            });



            listSubSystem.Add(qcl1);
            listSubSystem.Add(qcl2);
            listSubSystem.Add(gs);

            try
            {
                db ??= InitDbEntity();
                await db.InsertNav(listSubSystem)
                .Include(it => it.PLC)
                .Include(it => it.ListEquipment)
                .ThenInclude(it => it.ListBulk)
                .ExecuteCommandAsync();
                MessageBox.Show("种子创建成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
