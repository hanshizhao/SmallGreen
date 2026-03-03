namespace SmallGreen.Dto.Base
{
    public class PumpModuleDto
    {
        public long Id { get; set; }
        public bool IsLoopping { get; set; }
        public float SendPower { get; set; }
        public float LoopPower { get; set; }
        public float LoopPlanMinute { get; set; }
        public float LoopRealMinute { get; set; }
        public float DelayPlanMinute { get; set; }
        public float DelayRealMinute { get; set; }
        public float IfEmptyLitre { get; set; }
    }
}
