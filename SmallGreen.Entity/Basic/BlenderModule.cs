using SqlSugar;

namespace SmallGreen.Entity.Basic
{
    public class BlenderModule
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 搅拌控制
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<bool> DomBtnSwitch { get; set; } = null!;

        /// <summary>
        /// 搅拌状态
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true)]
        public Dom<bool>? DomIsBlending { get; set; } = null!;

        /// <summary>
        /// 搅拌设定时间（分钟）
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true)]
        public Dom<float>? DomBlendPlanMinute { get; set; } = null!;

        /// <summary>
        /// 搅拌实际时间（分钟）
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true)]
        public Dom<float>? DomBlendRealMinute { get; set; } = null!;

        /// <summary>
        /// 需求转速
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true)]
        public Dom<float>? DomValuePlanRPM { get; set; } = null!;

        /// <summary>
        /// 最大转速
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true)]
        public Dom<float>? DomValueMaxRPM { get; set; } = null!;

        public bool GetBtnSwitch() => DomBtnSwitch.GetCurrentValue();
        public bool GetIsBlending() => DomIsBlending != null && DomIsBlending.GetCurrentValue();
        public float GetBlendPlanMinute() => DomBlendPlanMinute == null ? 0f : DomBlendPlanMinute.GetCurrentValue();
        public float GetBlendRealMinute() => DomBlendRealMinute == null ? 0f : DomBlendRealMinute.GetCurrentValue();
        public float GetValuePlanRPM() => DomValuePlanRPM == null ? 0f : DomValuePlanRPM.GetCurrentValue();
        public float GetValueMaxRPM() => DomValueMaxRPM == null ? 0f : DomValueMaxRPM.GetCurrentValue();

        public List<IDom> GetAllDoms()
        {
            var list = new List<IDom>
            {
                DomBtnSwitch,
            };

            if (DomIsBlending != null) list.Add(DomIsBlending);
            if (DomBlendPlanMinute != null) list.Add(DomBlendPlanMinute);
            if (DomBlendRealMinute != null) list.Add(DomBlendRealMinute);
            if (DomValuePlanRPM != null) list.Add(DomValuePlanRPM);
            if (DomValueMaxRPM != null) list.Add(DomValueMaxRPM);

            return list;
        }
    }
}
