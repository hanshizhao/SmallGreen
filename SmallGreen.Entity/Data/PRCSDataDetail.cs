using SmallGreen.Dto.Data;
using SqlSugar;

namespace SmallGreen.Entity.Data
{
    /// <summary>
    /// 配液完成时，记录的配液数据(详情表)
    /// </summary>
    public class PRCSDataDetail
    {
        /// <summary>
        /// ID 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 所属对象
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 助剂Id
        /// </summary>
        public long AssId { get; set; }

        /// <summary>
        /// 管道顺序
        /// </summary>
        public int AssSequence { get; set; }

        /// <summary>
        /// 助剂编码
        /// </summary>
        public string? AssCodeNumber { get; set; }

        /// <summary>
        /// 助剂名称
        /// </summary>
        public string? AssName { get; set; }

        /// <summary>
        /// 一升溶液有多少克助剂
        /// </summary>
        //grams per liter
        public double GramsPerLiter { get; set; }

        /// <summary>
        /// 配方量 克每升
        /// </summary>
        public double AssGl { get; set; }

        /// <summary>
        /// 计划体积（含水）
        /// </summary>
        public double PlanVolumeWithWater { get; set; }

        /// <summary>
        /// 实际体积（含水，来自流量计）
        /// </summary>
        public double RealVolumeWithWater { get; set; }

        /// <summary>
        /// 计划公斤数：配液量 * 配方量
        /// </summary>
        public double PlanKg { get; set; }

        /// <summary>
        /// 实际公斤数
        /// </summary>
        public double AssKg { get; set; }


        public PRCSDataDetailDto ToDto()
        {
            return new PRCSDataDetailDto
            {
                Id = Id,
                AssSequence = AssSequence,
                AssCodeNumber = AssCodeNumber,
                AssGl = AssGl,
                AssId = AssId,
                AssKg = AssKg,
                AssName = AssName,
                ParentId = ParentId,
                PlanKg = PlanKg,
                GramsPerLiter = GramsPerLiter,
                PlanVolumeWithWater = PlanVolumeWithWater,
                RealVolumeWithWater = RealVolumeWithWater
            };
        }
        

    }
}
