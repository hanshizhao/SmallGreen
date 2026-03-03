using SmallGreen.Common;
using SmallGreen.Entity.Basic;
using SmallGreen.Entity.Interface;

namespace SmallGreen.API.IService
{
    public interface ISystemManagerService
    {
        /// <summary>
        /// 初始化系统
        /// </summary>
        /// <returns></returns>
        Task<OperateResult> Init();

        /// <summary>
        /// 检查运行时
        /// </summary>
        /// <returns></returns>
        Task CheckRuntime();

        /// <summary>
        /// 检查配液缸工作完成
        /// </summary>
        /// <returns></returns>
        Task CheckBulkCompleted();

        /// <summary>
        /// 机台翻页
        /// </summary>
        /// <returns></returns>
        Task EquipmentPageChanged();

        /// <summary>
        /// 检查订单状态
        /// </summary>
        /// <returns></returns>
        Task CheckOrderStatus();

        /// <summary>
        /// 根据子系统名称获取子系统
        /// </summary>
        /// <param name="subSystemName"></param>
        /// <returns></returns>
        ISubSystem? GetSubSystem(SubSystemName subSystemName);
    }
}
