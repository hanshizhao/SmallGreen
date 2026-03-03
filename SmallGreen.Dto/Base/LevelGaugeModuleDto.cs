namespace SmallGreen.Dto.Base
{
    public class LevelGaugeModuleDto
    {
        public int ID { get; set; }

        /// <summary>
        /// 当前体积
        /// </summary>
        public float CurrentLitre { get; set; }

        /// <summary>
        /// 当前高度
        /// </summary>
        public float CurrentHeight { get; set; }

        /// <summary>
        /// 乘积系数
        /// </summary>
        public float Cjxs { get; set; }

        /// <summary>
        /// 零点修正
        /// </summary>
        public float Ldxz { get; set; }

        /// <summary>
        /// 底部量
        /// </summary>
        public float Dbl { get; set; }

        /// <summary>
        /// 密度值
        /// </summary>
        public float Mdz { get; set; }

        /// <summary>
        /// 盘管体积
        /// </summary>
        public float Pgtj { get; set; }
    }
}
