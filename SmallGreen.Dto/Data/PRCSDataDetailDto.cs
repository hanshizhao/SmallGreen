namespace SmallGreen.Dto.Data
{
    public class PRCSDataDetailDto
    {
        public long Id { get; set; }
        public long ParentId { get; set; }
        public long AssId { get; set; }
        public int AssSequence { get; set; }
        public string? AssCodeNumber { get; set; }
        public string? AssName { get; set; }
        public double GramsPerLiter { get; set; }
        public double AssGl { get; set; }
        public double PlanVolumeWithWater { get; set; }
        public double RealVolumeWithWater { get; set; }
        public double PlanKg { get; set; }
        public double AssKg { get; set; }

        /// <summary>
        /// 实际有效浓度（g/L）
        /// </summary>
        public double EffectiveGramsPerLiter { get; set; }

        /// <summary>
        /// 财务消耗量（kg）
        /// </summary>
        public double EffectiveAssKg { get; set; }
    }
}
