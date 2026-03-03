namespace SmallGreen.Dto.Data
{
    public class AssInfoDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 在配液系统中管道的顺序
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 在液位显示功能中助剂桶的顺序
        /// </summary>
        public int BulkSequence { get; set; }

        /// <summary>
        /// 助剂编号
        /// </summary>
        public string? CodeNumber { get; set; }

        /// <summary>
        /// 助剂名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 用于液位显示，表示助剂桶的最大体积
        /// </summary>
        public float MaxV { get; set; } = 1000f;

        /// <summary>
        /// 助剂的浓度，单位是 克每升，用于计算助剂消耗的公斤数
        /// </summary>
        public double Concentration { get; set; }

        /// <summary>
        /// 是否是混合助剂
        /// </summary>
        public bool IsMixed { get; set; }

        /// <summary>
        /// 混合助剂详情
        /// </summary>
        public List<MixedDetailDto>? ListMixedDetail { get; set; }

    }
}
