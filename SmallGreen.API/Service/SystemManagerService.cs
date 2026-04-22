using System.Data;
using Microsoft.Data.SqlClient;
using SmallGreen.API.IService;
using SmallGreen.Common;
using SmallGreen.Entity;
using SmallGreen.Entity.Basic;
using SmallGreen.Entity.Interface;
using SmallGreen.Entity.Machine;

namespace SmallGreen.API.Service
{
    public class SystemManagerService : ISystemManagerService
    {
        private readonly ILogger<SystemManagerService> logger;
        private readonly ErpDbHelper erpDbHelper;
        private List<SubSystem> ListSubSystem;

        /// <summary>
        /// ERP 处方数据模型（对应存储过程返回的行）
        /// </summary>
        private class RealFomula
        {
            public int BulkID { get; set; }
            public string No { get; set; } = "";
            public float GL { get; set; }
            public string uGUID { get; set; } = "";
        }

        public SystemManagerService(ILogger<SystemManagerService> logger, ErpDbHelper erpDbHelper)
        {
            ListSubSystem = new List<SubSystem>();
            this.logger = logger;
            this.erpDbHelper = erpDbHelper;
        }

        public async Task<OperateResult> Init()
        {
            try
            {
                List<SubSystem> listSubSystem = await new Repository<SubSystem>().AsQueryable()
                .Includes(it => it.PLC)
                //.Includes(it => it.ListEquipment)
                //.Includes(it => it.ListEquipment, e => e.ListBulk)
                .ToListAsync();

                //logger.LogInformation("subSystem：{subSystemName}", listSubSystem.Count);
                var listEquipment = await new Repository<Equipment>().Context.Queryable<Equipment>()
                    .Includes(it => it.ListBulk)
                    .ToListAsync();

                foreach (var subSystem in listSubSystem)
                {
                    subSystem.ListEquipment = listEquipment.FindAll(it => it.SubSystemId == subSystem.Id);
                    //foreach (var eq in subSystem.ListEquipment)
                    //{
                    //    logger.LogInformation("subSystem：{subSystemName}-{eCount}-{bCount}", subSystem.SubSystemName, subSystem.ListEquipment.Count, eq.ListBulk.Count);
                    //}
                }

                listSubSystem[0].PLC.ConnectionS7Plc([new(1, 0, 18970)]);
                listSubSystem[1].PLC.ConnectionS7Plc([new(1, 0, 18270)]);
                listSubSystem[2].PLC.ConnectionS7Plc([
                    new(101, 0, 354),
                new(102, 0, 897),
                new(103, 0, 897),
                new(104, 0, 897),
                new(105, 0, 897),
                new(106, 0, 897),
                new(107, 0, 897),
                new(108, 0, 897),
                ]);


                ListSubSystem = listSubSystem;

                return new OperateResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new OperateResult { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task CheckRuntime()
        {
            string subSystemName = string.Empty;
            try
            {
                foreach (var subSystem in ListSubSystem)
                {
                    subSystemName = subSystem.SubSystemName.ToString();
                    var result = await subSystem.CheckRuntimeOnlyTrigger();

                    if (!result.IsSuccess)
                    {
                        logger.LogError("{subSystemName}检查运行时出现故障：{Message}", subSystem.SubSystemName, result.Message);
                        continue;
                    }

                    // 检查各机台的触发器
                    foreach (var equip in subSystem.ListEquipment)
                    {
                        // 翻页触发
                        if (equip.BtnPageChange?.GetCurrentValue() == true)
                        {
                            await HandleEquipmentPageChanged(subSystem, equip);
                        }

                        // 开始生产触发
                        if (equip.BtnStart?.GetCurrentValue() == true)
                        {
                            await HandleStartWork(subSystem, equip);
                        }

                        // 订单完成触发
                        if (equip.TriggerFinished?.GetCurrentValue() == true)
                        {
                            await HandleOrderStatusChange(subSystem, equip);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "检查运行时出现故障,子系统队列：{sub_system_name}， {message}", subSystemName, ex.Message);
            }
        }

        public async Task CheckBulkCompleted()
        {
            foreach (var subSystem in ListSubSystem)
            {
                foreach (var equip in subSystem.ListEquipment)
                {
                    foreach (var bulk in equip.ListBulk)
                    {
                        var trigger = bulk.TriggerComplete;
                        if (trigger == null) continue;
                        if (trigger.GetCurrentValue() != 1) continue;


                        var result = await subSystem.SavePRCSData(equip, bulk);
                        if (!result.IsSuccess) logger.LogError(result.Message);
                    }
                }
            }
        }

        public Task EquipmentPageChanged()
        {
            // 已在 CheckRuntime 中通过 HandleEquipmentPageChanged 实现
            return Task.CompletedTask;
        }

        public Task CheckOrderStatus()
        {
            // 已在 CheckRuntime 中通过 HandleOrderStatusChange 实现
            return Task.CompletedTask;
        }

        public Task CheckStartWork()
        {
            // 已在 CheckRuntime 中通过 HandleStartWork 实现
            return Task.CompletedTask;
        }

        /// <summary>
        /// 处理机台翻页请求
        /// </summary>
        private async Task HandleEquipmentPageChanged(SubSystem subSystem, Equipment equip)
        {
            try
            {
                var currentPage = equip.DataCurrentPage?.GetCurrentValue() ?? 1;
                var equipType = subSystem.GetEquipType();
                var equipName = equip.Name;

                // 老项目中会截取 '-' 之前的部分作为机台名
                var dashIndex = equipName.IndexOf('-');
                if (dashIndex > 0) equipName = equipName[..dashIndex];

                var paras = new SqlParameter[]
                {
                    new("@currentPageNum", currentPage),
                    new("@equipName", equipName),
                    new("@MaxPerPage", 5),
                    new("@equipType", equipType)
                };

                var dt = erpDbHelper.RunProcedure("PROC_OrderInfoPageQueryByEquipName", paras);

                var lineStatuses = new[] { equip.Line1Status, equip.Line2Status, equip.Line3Status, equip.Line4Status, equip.Line5Status };
                var lineColorIDs = new[] { equip.Line1ColorID, equip.Line2ColorID, equip.Line3ColorID, equip.Line4ColorID, equip.Line5ColorID };
                var lineWorkInfos = new[] { equip.Line1WrokInfoArray, equip.Line2WrokInfoArray, equip.Line3WrokInfoArray, equip.Line4WrokInfoArray, equip.Line5WrokInfoArray };

                // 先清空5行数据
                var domsToWrite = new List<IDom>();
                for (int i = 0; i < 5; i++)
                {
                    if (lineStatuses[i] != null) { lineStatuses[i].NewValue = (ushort)0; domsToWrite.Add(lineStatuses[i]); }
                    if (lineColorIDs[i] != null) { lineColorIDs[i].NewValue = (ushort)0; domsToWrite.Add(lineColorIDs[i]); }
                    if (lineWorkInfos[i] != null) { lineWorkInfos[i].NewValue = string.Empty; domsToWrite.Add(lineWorkInfos[i]); }
                }

                // 写入查询到的订单数据
                for (int i = 0; i < dt.Rows.Count && i < 5; i++)
                {
                    var row = dt.Rows[i];
                    if (lineStatuses[i] != null && row["iStatus"] != DBNull.Value)
                        lineStatuses[i].NewValue = Convert.ToUInt16(row["iStatus"]);
                    if (lineColorIDs[i] != null && row["iColorID"] != DBNull.Value)
                        lineColorIDs[i].NewValue = Convert.ToUInt16(row["iColorID"]);
                    if (lineWorkInfos[i] != null && row["sWorkInfoArray"] != DBNull.Value)
                        lineWorkInfos[i].NewValue = row["sWorkInfoArray"].ToString();
                }

                // 写入总页数
                if (dt.Rows.Count > 0 && equip.DataTotalPage != null)
                {
                    var totalPage = dt.Rows[0]["iTotalPageCount"];
                    if (totalPage != DBNull.Value)
                    {
                        equip.DataTotalPage.NewValue = Convert.ToUInt16(totalPage);
                        domsToWrite.Add(equip.DataTotalPage);
                    }
                }

                // 批量写入 PLC
                if (domsToWrite.Count > 0)
                {
                    await subSystem.PLC.Write(domsToWrite);
                }

                // 重置翻页按钮
                if (equip.BtnPageChange != null)
                {
                    equip.BtnPageChange.NewValue = false;
                    await subSystem.PLC.Write([equip.BtnPageChange]);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{equipName} 翻页查询出现异常：{message}", equip.Name, ex.Message);
            }
        }

        /// <summary>
        /// 处理开始生产 — 获取处方并下发到PLC
        /// </summary>
        private async Task HandleStartWork(SubSystem subSystem, Equipment equip)
        {
            try
            {
                var workInfoArray = equip.DataWorkOrderInfoArray?.GetCurrentValue() ?? "";

                // 验证字符串长度
                if (string.IsNullOrEmpty(workInfoArray) || workInfoArray.Length < 44)
                {
                    await SetFomulaQueryStatus(subSystem, equip, 2, "生产信息字符串长度不足");
                    return;
                }

                // 解析69位字符串（44位有效 + 25位保留）
                var jobGroupOrderNo = workInfoArray.Substring(0, 5).TrimEnd();
                var orderNo = workInfoArray.Substring(5, 11).TrimEnd();
                var prescriptionNo = workInfoArray.Substring(16, 6).TrimEnd();
                var component = workInfoArray.Substring(22, 11).TrimEnd();
                var unionQty = workInfoArray.Substring(33, 11).TrimEnd();

                var equipType = subSystem.GetEquipType();
                var equipName = equip.Name;
                var dashIndex = equipName.IndexOf('-');
                if (dashIndex > 0) equipName = equipName[..dashIndex];

                // 调用存储过程查询处方
                var paras = new SqlParameter[]
                {
                    new("@jobGroupOrderNo", jobGroupOrderNo),
                    new("@orderNo", orderNo),
                    new("@component", component),
                    new("@unionQty", unionQty),
                    new("@prescriptionNo", prescriptionNo),
                    new("@equipName", equipName),
                    new("@stepNo", equip.StepNo),
                    new("@equipID", equip.Id),
                    new("@paraEquipType", equipType)
                };

                var dt = erpDbHelper.RunProcedure("PROC_QueryFomulaByEquipWorkInfo", paras);

                if (dt.Rows.Count == 0)
                {
                    await SetFomulaQueryStatus(subSystem, equip, 2, "存储过程未返回处方数据");
                    return;
                }

                // 解析处方数据
                var listFomula = new List<RealFomula>();
                foreach (DataRow row in dt.Rows)
                {
                    listFomula.Add(new RealFomula
                    {
                        BulkID = Convert.ToInt32(row["BulkID"]),
                        No = row["No"].ToString() ?? "",
                        GL = Convert.ToSingle(row["GL"]),
                        uGUID = row["uGUID"].ToString() ?? ""
                    });
                }

                // 按设备类型解析配方
                float[] fomulaValues;
                if (equipType == 0)
                {
                    fomulaValues = GetQCLFomular(listFomula);
                }
                else
                {
                    fomulaValues = GetGSFomular(listFomula);
                }

                // 编码为 A 分隔字符串
                var fomulaString = SubSystem.EncodeFomulaArray(fomulaValues);

                // 写入所有配液缸的 DataFomulaArray
                var domsToWrite = new List<IDom>();
                foreach (var bulk in equip.ListBulk)
                {
                    if (bulk.DataFomulaArray != null)
                    {
                        bulk.DataFomulaArray.NewValue = fomulaString;
                        domsToWrite.Add(bulk.DataFomulaArray);
                    }
                }

                if (domsToWrite.Count > 0)
                {
                    var writeResult = await subSystem.PLC.Write(domsToWrite);
                    if (!writeResult.IsSuccess)
                    {
                        await SetFomulaQueryStatus(subSystem, equip, 2, $"写入PLC失败:{writeResult.Message}");
                        return;
                    }
                }

                // 保存 uGuid（用于后续订单状态回调和用量回写）
                equip.uGuid = listFomula[0].uGUID;

                // 设置处方下发成功
                await SetFomulaQueryStatus(subSystem, equip, 1, "");

                // 调用 Cache 存储过程更新订单状态为"生产中"
                try
                {
                    erpDbHelper.UpdateByProcedure("Cache", new SqlParameter[]
                    {
                        new("@uGUID", equip.uGuid),
                        new("@workStatus", "生产中"),
                        new("@sysID", ""),
                        new("@equipName", equip.Name),
                        new("@equipID", equip.Id)
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "{equipName} 调用Cache存储过程失败", equip.Name);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{equipName} 处方下发出现异常：{message}", equip.Name, ex.Message);
                await SetFomulaQueryStatus(subSystem, equip, 2, ex.Message);
            }
        }

        /// <summary>
        /// 设置处方下发状态并重置 BtnStart
        /// </summary>
        private async Task SetFomulaQueryStatus(SubSystem subSystem, Equipment equip, ushort status, string errorMessage)
        {
            try
            {
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    logger.LogError("{equipName} 处方下发失败：{error}", equip.Name, errorMessage);
                }

                if (equip.DataFomulaQueryStatus != null)
                {
                    equip.DataFomulaQueryStatus.NewValue = status;
                    await subSystem.PLC.Write([equip.DataFomulaQueryStatus]);
                }

                // 重置 BtnStart
                if (equip.BtnStart != null)
                {
                    equip.BtnStart.NewValue = false;
                    await subSystem.PLC.Write([equip.BtnStart]);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{equipName} 设置处方状态失败", equip.Name);
            }
        }

        /// <summary>
        /// 前处理配方解析 — 11种助剂映射到桶号1-11
        /// </summary>
        private float[] GetQCLFomular(List<RealFomula> list)
        {
            var result = new float[11];

            // 先处理组合助剂
            var item8110 = list.Find(x => x.No == "8110");
            var item8103 = list.Find(x => x.No == "8103");
            var item8109 = list.Find(x => x.No == "8109");

            // 8110+8103 组合为10#桶，比例15:1
            if (item8110 != null && item8103 != null)
            {
                var ratio = item8110.GL / (item8103.GL > 0 ? item8103.GL : 1);
                if (Math.Abs(ratio - 15) > 0.1f)
                {
                    logger.LogWarning("前处理组合助剂8110+8103比例异常：{ratio}", ratio);
                }
                result[9] = item8110.GL + item8103.GL;
            }

            // 8109+8103 组合为7#桶，比例10:1
            if (item8109 != null && item8103 != null)
            {
                var ratio = item8109.GL / (item8103.GL > 0 ? item8103.GL : 1);
                if (Math.Abs(ratio - 10) > 0.1f)
                {
                    logger.LogWarning("前处理组合助剂8109+8103比例异常：{ratio}", ratio);
                }
                result[6] = item8109.GL + item8103.GL;
            }

            var combinedNos = new HashSet<string> { "8110", "8103", "8109" };

            foreach (var item in list)
            {
                if (combinedNos.Contains(item.No)) continue;
                if (item.BulkID >= 1 && item.BulkID <= 11)
                {
                    result[item.BulkID - 1] += item.GL;
                }
            }

            return result;
        }

        /// <summary>
        /// 固色配方解析 — 3种助剂映射到桶号12-14
        /// </summary>
        private float[] GetGSFomular(List<RealFomula> list)
        {
            var result = new float[3];

            foreach (var item in list)
            {
                if (item.BulkID >= 12 && item.BulkID <= 14)
                {
                    result[item.BulkID - 12] += item.GL;
                }
                else if (item.BulkID == 0)
                {
                    result[1] += item.GL;
                }
            }

            if (result[0] > 0 && result[1] > 0 && result[0] / result[1] > 10)
            {
                logger.LogWarning("固色配方异常：元明粉与纯碱用量比值超过10:1");
            }

            return result;
        }

        /// <summary>
        /// 处理订单完成/暂停/取消
        /// </summary>
        private async Task HandleOrderStatusChange(SubSystem subSystem, Equipment equip)
        {
            try
            {
                var finishedType = equip.DataFinishedType?.GetCurrentValue() ?? 0;

                // 更新 T_EquipStartFinishStatus
                try
                {
                    erpDbHelper.UpdateByProcedure("UpdateEquipFinishStatus", new SqlParameter[]
                    {
                        new("@equipID", equip.Id),
                        new("@equipName", equip.Name),
                        new("@finishType", finishedType),
                        new("@finishDateTime", DateTime.Now)
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "{equipName} 更新完成状态表失败", equip.Name);
                }

                // 如果是手动模式（无 uGuid），跳过 ERP 通知
                if (string.IsNullOrEmpty(equip.uGuid) || equip.uGuid == "手动模式")
                {
                    equip.uGuid = string.Empty;
                    await ResetTriggerFinished(subSystem, equip);
                    return;
                }

                // 调用 ERP 存储过程
                try
                {
                    if (finishedType == 3)
                    {
                        // 取消
                        erpDbHelper.UpdateByProcedure("Cancel", new SqlParameter[]
                        {
                            new("@uGUID", equip.uGuid),
                            new("@index", 0),
                            new("@sysID", ""),
                            new("@equipName", equip.Name),
                            new("@equipID", equip.Id)
                        });
                    }
                    else
                    {
                        // 完成(1) 或 暂停(2)
                        var status = finishedType == 1 ? "完成" : "暂停";
                        erpDbHelper.UpdateByProcedure("Finish", new SqlParameter[]
                        {
                            new("@uGUID", equip.uGuid),
                            new("@equipName", equip.Name),
                            new("@status", status),
                            new("@equipID", equip.Id),
                            new("@sysID", "")
                        });
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "{equipName} 调用ERP订单状态存储过程失败", equip.Name);
                }

                equip.uGuid = string.Empty;
                await ResetTriggerFinished(subSystem, equip);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{equipName} 订单状态回调异常：{message}", equip.Name, ex.Message);
            }
        }

        /// <summary>
        /// 重置订单完成触发器
        /// </summary>
        private async Task ResetTriggerFinished(SubSystem subSystem, Equipment equip)
        {
            if (equip.TriggerFinished != null)
            {
                equip.TriggerFinished.NewValue = false;
                await subSystem.PLC.Write([equip.TriggerFinished]);
            }
        }

        public ISubSystem? GetSubSystem(SubSystemName subSystemName)
        {
            foreach (var subSystem in ListSubSystem)
            {
                if (subSystem.SubSystemName == subSystemName) return subSystem;
            }

            return null;
        }

        
    }
}
