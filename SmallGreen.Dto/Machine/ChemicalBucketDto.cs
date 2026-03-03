namespace SmallGreen.Dto.Machine
{
    public class ChemicalBucketDto
    {
        public long Id { get; set; }
        public string CodeNumber { get; set; } = null!;
        public float Level { get; set; }
        public float MaxCapacity { get; set; }
        /// <summary>
        /// 助剂浓度，单位：克/升，用于计算助剂消耗量
        /// 计算公式：消耗量 = 体积(L) × 浓度 ÷ 1000
        /// </summary>
        public float Concentration { get; set; }
        public long SubSystemId { get; set; }
        public string SubSystemName { get; set; } = null!;
        public DateTime LastCompleteTime { get; set; }
        public List<FormulaItemDto>? FormulaItems { get; set; }
    }
}
