using System.ComponentModel.DataAnnotations;

namespace SmallGreen.Dto.Machine
{
    /// <summary>
    /// 更新助剂桶请求 DTO
    /// </summary>
    public class UpdateAssBucketDto
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public long Id { get; set; }

        /// <summary>
        /// 助剂编号
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? CodeNumber { get; set; }

        /// <summary>
        /// 助剂名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        /// <summary>
        /// 助剂的浓度，单位是 克每升
        /// </summary>
        [Required]
        [Range(0, double.MaxValue)]
        public double Concentration { get; set; }

        /// <summary>
        /// 实际有效浓度（g/L），用于财务消耗计算
        /// </summary>
        [Required]
        [Range(0, double.MaxValue)]
        public double EffectiveConcentration { get; set; }

        /// <summary>
        /// 是否混合助剂
        /// </summary>
        public bool IsMixed { get; set; }
    }
}
