using SmallGreen.Common;
using SmallGreen.Entity.Basic;
using SmallGreen.Entity.Machine;

namespace SmallGreen.Entity.Interface
{
    public interface ISubSystem
    {
        /// <summary>
        /// 子系统名称
        /// </summary>
        SubSystemName SubSystemName { get; set; }
        SiemensPLC PLC { get; set; }

        List<Equipment> ListEquipment { get; set; }

        List<IDom>? ListDom { get; }

        /// <summary>
        /// 检查运行时
        /// </summary>
        /// <returns></returns>
        Task<OperateResult> CheckRuntime();

        /// <summary>
        /// 检查运行时
        /// </summary>
        /// <returns></returns>
        Task<OperateResult> CheckRuntimeOnlyTrigger();


    }
}
