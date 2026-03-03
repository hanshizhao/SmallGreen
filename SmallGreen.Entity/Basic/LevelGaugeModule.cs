using SqlSugar;

namespace SmallGreen.Entity.Basic
{
    /// <summary>
    /// 液位计类
    /// </summary>
    public class LevelGaugeModule
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 当前缸内体积(升)
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float> DomCurrentLitre { get; set; } = null!;

        /// <summary>
        /// 当前缸内液位高度(米)
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float> DomCurrentHeight { get; set; } = null!;

        /// <summary>
        /// 乘积系数
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float> DomCJXS { get; set; } = null!;

        /// <summary>
        /// 零点修正
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float> DomLDXZ { get; set; } = null!;

        /// <summary>
        /// 底部量
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float> DomDBL { get; set; } = null!;

        /// <summary>
        /// 密度值
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float> DomMDZ { get; set; } = null!;

        /// <summary>
        /// 盘管体积
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<float> DomPGTJ { get; set; } = null!;

        public float GetCurrentLitre() => DomCurrentLitre.GetCurrentValue();
        public float GetCurrentHeight() => DomCurrentHeight.GetCurrentValue();
        public float GetCJXS() => DomCJXS.GetCurrentValue();
        public float GetLDXZ() => DomLDXZ.GetCurrentValue();
        public float GetDBL() => DomDBL.GetCurrentValue();
        public float GetMDZ() => DomMDZ.GetCurrentValue();
        public float GetPGTJ() => DomPGTJ.GetCurrentValue();



        public List<IDom> GetAllDoms()
        {
            List<IDom> list = new();

            if (DomCJXS is not null) list.Add(DomCJXS);
            if (DomCurrentHeight is not null) list.Add(DomCurrentHeight);
            if (DomCurrentLitre is not null) list.Add(DomCurrentLitre);
            if (DomDBL is not null) list.Add(DomDBL);
            if (DomLDXZ is not null) list.Add(DomLDXZ);
            if (DomMDZ is not null) list.Add(DomMDZ);
            if (DomPGTJ is not null) list.Add(DomPGTJ);

            return list;
        }
    }
}
