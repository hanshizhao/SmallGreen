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
        private List<SubSystem> ListSubSystem;

        public SystemManagerService(ILogger<SystemManagerService> logger)
        {
            ListSubSystem = new List<SubSystem>();
            this.logger = logger;
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
                    // 检查各个点位触发器
                    var result = await subSystem.CheckRuntimeOnlyTrigger();

                    if (!result.IsSuccess) logger.LogError("{subSystemName}检查运行时出现故障：{Message}", subSystem.SubSystemName, result.Message);
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
