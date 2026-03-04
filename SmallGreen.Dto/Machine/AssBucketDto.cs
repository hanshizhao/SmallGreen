namespace SmallGreen.Dto.Machine
{
    /// <summary>
    /// 助剂桶查询响应 DTO
    /// </summary>
    public class AssBucketDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 所属系统名称（前处理/固色）
        /// </summary>
        public string? SubSystemName { get; set; }

        /// <summary>
        /// 在配液系统中管道的顺序（只读）
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 助剂编号
        /// </summary>
        public string? CodeNumber { get; set; }

        /// <summary>
        /// 助剂名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 助剂的浓度，单位是 克每升
        /// </summary>
        public double Concentration { get; set; }

        /// <summary>
        /// 实际有效浓度（g/L），用于财务消耗计算
        /// </summary>
        public double EffectiveConcentration { get; set; }

        /// <summary>
        /// 最大容量（只读，固定1000L）
        /// </summary>
        public float MaxV { get; set; } = 1000f;

        /// <summary>
        /// 是否混合助剂
        /// </summary>
        public bool IsMixed { get; set; }

        /// <summary>
        /// 混合组分列表
        /// </summary>
        public List<MixedComponentDto>? MixedComponents { get; set; }
    }
}
