using SqlSugar;

namespace SmallGreen.Entity.Basic
{
    /// <summary>
    /// 清洗模块
    /// </summary>
    public class ClearModule
    {
        /// <summary>
        /// ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 清洗用水量
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float> DomWaterPlanLitre { get; set; } = null!;

        /// <summary>
        /// 清洗搅拌时间（分钟）
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float> DomBlendPlanMinute { get; set; } = null!;

        /// <summary>
        /// 计划排污空缸延时（分钟）
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float> DomDelayPlanMinute { get; set; } = null!;

        /// <summary>
        /// 实际排污空缸延时（分钟）
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float> DomDelayRealMinute { get; set; } = null!;

        /// <summary>
        /// 清洗循环时间（分钟）
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float> DomLoopPlanMinute { get; set; } = null!;

        /// <summary>
        /// 空缸判断体积
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float> DomEmptyLitre { get; set; } = null!;

        /// <summary>
        /// 自动清洗
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<bool> DomAutoClear { get; set; } = null!;

        /// <summary>
        /// 取消清洗
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true)]
        public Dom<bool>? DomCancelClear { get; set; } = null!;

        /// <summary>
        /// 排污控制
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<bool> DomStartPollution { get; set; } = null!;

        public float GetWaterPlanLitre() => DomWaterPlanLitre.GetCurrentValue();
        public float GetBlendPlanMinute() => DomBlendPlanMinute.GetCurrentValue();
        public float GetDelayPlanMinute() => DomDelayPlanMinute.GetCurrentValue();
        public float GetDelayRealMinute() => DomDelayRealMinute.GetCurrentValue();
        public float GetLoopPlanMinute() => DomLoopPlanMinute.GetCurrentValue();
        public float GetEmptyLitre() => DomEmptyLitre.GetCurrentValue();
        public bool GetAutoClear() => DomAutoClear.GetCurrentValue();
        public bool GetCancelClear() => DomCancelClear != null && DomCancelClear.GetCurrentValue();
        public bool GetStartPollution() => DomStartPollution.GetCurrentValue();



        public List<IDom> GetAllDoms()
        {
            var list = new List<IDom>
            {
                DomWaterPlanLitre,
                DomBlendPlanMinute,
                DomDelayPlanMinute,
                DomDelayRealMinute,
                DomLoopPlanMinute,
                DomEmptyLitre,
                DomAutoClear,
                DomStartPollution,
            };

            if (DomCancelClear != null) list.Add(DomCancelClear);
            return list;
        }
    }
}
