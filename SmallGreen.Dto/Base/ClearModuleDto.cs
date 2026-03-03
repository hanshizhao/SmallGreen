namespace SmallGreen.Dto.Base
{
    public class ClearModuleDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 计划用水量
        /// </summary>
        public float WaterPlanLitre { get; set; }

        /// <summary>
        /// 计划搅拌时间
        /// </summary>
        public float BlendPlanMinute { get; set; }


        /// <summary>
        /// 计划延时时间
        /// </summary>
        public float DelayPlanMinute { get; set; }

        /// <summary>
        /// 实际延时时间
        /// </summary>
        public float DelayRealMinute { get; set; }

        /// <summary>
        /// 计划循环时间
        /// </summary>
        public float LoopPlanMinute { get; set; }


        /// <summary>
        /// 判定空缸体积
        /// </summary>
        public float EmptyLitre { get; set; }


    }
}
