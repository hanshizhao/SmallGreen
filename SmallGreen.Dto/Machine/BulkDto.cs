namespace SmallGreen.Dto.Machine
{
    public class BulkDto
    {
        public long Id { get; set; }
        public string CodeNumber { get; set; } = null!;
        public long EquipmentId { get; set; }
        public float Level { get; set; }
        public float PlanVolume { get; set; }
        public string? FomulaArray { get; set; }
        public ushort Complete { get; set; }
        public string? RealLitreArray { get; set; }
        public DateTime LastCompleteTime { get; set; }
    }
}
