using SqlSugar;

namespace SmallGreen.Entity.Basic
{
    /// <summary>
    /// 温度计模块
    /// </summary>
    public class TemperatureModule
    {
        /// <summary>
        /// Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 当前温度
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float> DomCurrentValue { get; set; } = null!;

        /// <summary>
        /// 乘积系数
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float> DomCJXS { get; set; } = null!;

        /// <summary>
        /// 修正量
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float> DomXZL { get; set; } = null!;

        /// <summary>
        /// 设定温度
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float>? DomSetValue { get; set; } = null!;

        /// <summary>
        /// 加温按钮
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<bool>? DomBtnSwitch { get; set; } = null!;

        public float GetCurrentValue() => DomCurrentValue.GetCurrentValue();
        public float GetCJXS() => DomCJXS.GetCurrentValue();
        public float GetXZL() => DomXZL.GetCurrentValue();
        public float GetSetValue() => DomSetValue == null ? 0f : DomSetValue.GetCurrentValue();
        public bool GetBtnSwitch() => DomBtnSwitch != null && DomBtnSwitch.GetCurrentValue();


        public List<IDom> GetAllDoms()
        {
            var list = new List<IDom>
            {
                DomCurrentValue,
                DomCJXS,
                DomXZL,
            };
            if (DomSetValue != null) list.Add(DomSetValue);
            if (DomBtnSwitch != null) list.Add(DomBtnSwitch);
            return list;
        }
    }
}
