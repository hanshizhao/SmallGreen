namespace SmallGreen.Dto.Data
{
    public class MixedDetailDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 所属父类Id
        /// </summary>
        /// 
        public long ParentId { get; set; }

        /// <summary>
        /// 助剂编号
        /// </summary>
        public string? CodeNumber { get; set; }

        /// <summary>
        /// 助剂名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 助剂的浓度，单位是 克每升，用于计算助剂消耗的公斤数
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
