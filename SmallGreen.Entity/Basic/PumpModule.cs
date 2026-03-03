using SqlSugar;

namespace SmallGreen.Entity.Basic
{
    public class PumpModule
    {
        /// <summary>
        /// ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }

        /// <summary>
        /// 抽料频率
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true)]
        public Dom<float>? DomSendPower { get; set; }

        /// <summary>
        /// 循环频率
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true)]
        public Dom<float>? DomLoopPower { get; set; }

        /// <summary>
        /// 循环控制
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<bool> DomStartLoop { get; set; } = null!;

        /// <summary>
        /// 循环设定时间（分钟）
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true)]
        public Dom<float>? DomLoopPlanMinute { get; set; }

        /// <summary>
        /// 循环实际时间（分钟）
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true)]
        public Dom<float>? DomLoopRealMinute { get; set; }

        /// <summary>
        /// 计划抽料延时时间(分钟)
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true)]
        public Dom<float>? DomDelayPlanMinute { get; set; }

        /// <summary>
        /// 实际抽料延时时间(分钟)
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true)]
        public Dom<float>? DomDelayRealMinute { get; set; }

        /// <summary>
        /// 抽料空缸判定体积
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true)]
        public Dom<float>? DomIfEmptyLitre { get; set; }

        /// <summary>
        /// 泵状态
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true)]
        public Dom<bool>? DomIsRunning { get; set; } = null!;

        public float GetSendPower() => DomSendPower == null ? 0f : DomSendPower.GetCurrentValue();
        public float GetLoopPower() => DomLoopPower == null ? 0f : DomLoopPower.GetCurrentValue();
        public bool GetStartLoop() => DomStartLoop.GetCurrentValue();
        public float GetLoopPlanMinute() => DomLoopPlanMinute == null ? 0f : DomLoopPlanMinute.GetCurrentValue();
        public float GetLoopRealMinute() => DomLoopRealMinute == null ? 0f : DomLoopRealMinute.GetCurrentValue();
        public float GetDelayPlanMinute() => DomDelayPlanMinute == null ? 0f : DomDelayPlanMinute.GetCurrentValue();
        public float GetDelayRealMinute() => DomDelayRealMinute == null ? 0f : DomDelayRealMinute.GetCurrentValue();
        public float GetIfEmptyLitre() => DomIfEmptyLitre == null ? 0f : DomIfEmptyLitre.GetCurrentValue();
        public bool GetIsRunning() => DomIsRunning != null && DomIsRunning.GetCurrentValue();

        public List<IDom> GetAllDoms()
        {
            List<IDom> list =
            [
                DomStartLoop,
            ];
            if (DomSendPower != null) list.Add(DomSendPower);
            if (DomLoopPower != null) list.Add(DomLoopPower);
            if (DomLoopPlanMinute != null) list.Add(DomLoopPlanMinute);
            if (DomLoopRealMinute != null) list.Add(DomLoopRealMinute);
            if (DomDelayPlanMinute != null) list.Add(DomDelayPlanMinute);
            if (DomDelayRealMinute != null) list.Add(DomDelayRealMinute);
            if (DomIfEmptyLitre != null) list.Add(DomIfEmptyLitre);
            if (DomIsRunning != null) list.Add(DomIsRunning);

            return list;
        }
    }
}
