using SqlSugar;

namespace SmallGreen.Entity.Basic
{
    /// <summary>
    /// 故障模块
    /// </summary>
    public class ErrorModule
    {
        /// <summary>
        /// ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public int ID { get; set; }

        /// <summary>
        /// 报警恢复
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<bool> DomErrorRecover { get; set; } = null!;

        /// <summary>
        /// 脱离反馈运行
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<bool> DomRunInNoFeedback { get; set; } = null!;

        /// <summary>
        /// 故障触发点位
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<short> DomErrorTrigger { get; set; } = null!;

        /// <summary>
        /// 是否存在故障
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public bool IsExsistError { get; set; }

        public List<IDom> GetAllDoms()
        {
            var list = new List<IDom>
            {
                DomErrorRecover,
                DomRunInNoFeedback,
                DomErrorTrigger
            };

            return list;
        }
    }
}
