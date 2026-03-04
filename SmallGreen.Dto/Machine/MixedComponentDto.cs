namespace SmallGreen.Dto.Machine
{
    /// <summary>
    /// 混合组分 DTO
    /// </summary>
    public class MixedComponentDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 所属父类 Id
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 组分编号
        /// </summary>
        public string? CodeNumber { get; set; }

        /// <summary>
        /// 组分名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 组分浓度，单位是 克每升
        /// </summary>
        public double Concentration { get; set; }

        /// <summary>
        /// 实际有效浓度（g/L），用于财务消耗计算
        /// </summary>
        public double EffectiveConcentration { get; set; }

        /// <summary>
        /// 混合比例
        /// </summary>
        public double Ratio { get; set; }
    }
}
