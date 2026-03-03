using SqlSugar;

namespace SmallGreen.Entity.Basic
{
    public class FlowGaugeModule
    {
        /// <summary>
        /// Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 进水开关
        /// </summary>
        [SugarColumn(IsJson = true)]
        public Dom<bool> DomStartInWater { get; set; } = null!;

        /// <summary>
        /// 进水计划量
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true)]
        public Dom<float>? DomPlanLitre { get; set; } = null!;

        /// <summary>
        /// 进水实际量
        /// </summary>
        [SugarColumn(IsJson = true, IsNullable = true)]
        public Dom<float>? DomRealLitre { get; set; } = null!;

        public bool GetStartInWater() => DomStartInWater.GetCurrentValue();
        public float GetPlanLitre() => DomPlanLitre == null ? 0f : DomPlanLitre.GetCurrentValue();
        public float GetRealLitre() => DomRealLitre == null ? 0f : DomRealLitre.GetCurrentValue();

        public List<IDom> GetAllDoms()
        {
            var list = new List<IDom>
            {
                DomStartInWater,
            };

            if (DomPlanLitre != null) list.Add(DomPlanLitre);
            if (DomRealLitre != null) list.Add(DomRealLitre);

            return list;
        }
    }
}
