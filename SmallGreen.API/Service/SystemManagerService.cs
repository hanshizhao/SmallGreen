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
            throw new NotImplementedException();
        }

        public Task CheckOrderStatus()
        {
            throw new NotImplementedException();
        }

        public Task CheckStartWork()
        {
            throw new NotImplementedException("Task 8 将实现");
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

        private Task HandleStartWork(SubSystem subSystem, Equipment equip)
        {
            throw new NotImplementedException("Task 8 将实现");
        }

        private Task HandleOrderStatusChange(SubSystem subSystem, Equipment equip)
        {
            throw new NotImplementedException("Task 9 将实现");
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
