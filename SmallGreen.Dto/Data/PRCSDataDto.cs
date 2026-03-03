using SmallGreen.Dto.Machine;

namespace SmallGreen.Dto.Data
{
    public class PRCSDataDto
    {
        public long Id { get; set; }
        public string? CardNo { get; set; }
        public long EquipmentId { get; set; }
        public string? EquipmentCodeNumber { get; set; }
        public string? EquipmentName { get; set; }
        public EquipmentDto? Equipment { get; set; }
        public long BulkId { get; set; }
        public string? BulkCodeNumber { get; set; }
        public BulkDto? Bulk { get; set; }
        public float PlanVolume { get; set; }
        public float ActualVolume { get; set; }
        public List<PRCSDataDetailDto>? ListDetail { get; set; }
        public bool IsChange { get; set; } = false;
        public DateTime CompletedDateTime { get; set; } = DateTime.Now;
    }
}
