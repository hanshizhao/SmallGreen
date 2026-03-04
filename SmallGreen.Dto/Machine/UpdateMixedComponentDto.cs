using System.ComponentModel.DataAnnotations;

namespace SmallGreen.Dto.Machine
{
    /// <summary>
    /// 更新混合组分请求 DTO
    /// </summary>
    public class UpdateMixedComponentDto
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public long Id { get; set; }

        /// <summary>
        /// 组分编号
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? CodeNumber { get; set; }

        /// <summary>
        /// 组分名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        /// <summary>
        /// 组分浓度，单位是 克每升
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
        /// 混合比例（不要求和等于1，系统自动归一化）
        /// </summary>
        [Required]
        [Range(double.Epsilon, double.MaxValue)]
        public double Ratio { get; set; }
    }
}
