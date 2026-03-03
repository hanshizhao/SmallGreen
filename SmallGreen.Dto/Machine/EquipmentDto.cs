namespace SmallGreen.Dto.Machine
{
    public class EquipmentDto
    {
        public long Id { get; set; }
        public long SubSystemId { get; set; }
        public string CodeNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public List<BulkDto> ListBulk { get; set; } = null!;
        public ushort WorkColorID { get; set; }
        public string? WorkOrderInfoArray { get; set; }
        public ushort FinishedType { get; set; }
        public ushort FomulaQueryStatus { get; set; }
        public ushort Line1Status { get; set; }
        public ushort Line2Status { get; set; }
        public ushort Line3Status { get; set; }
        public ushort Line4Status { get; set; }
        public ushort Line5Status { get; set; }
        public ushort Line1ColorID { get; set; }
        public ushort Line2ColorID { get; set; }
        public ushort Line3ColorID { get; set; }
        public ushort Line4ColorID { get; set; }
        public ushort Line5ColorID { get; set; }
        public string? Line1WrokInfoArray { get; set; }
        public string? Line2WrokInfoArray { get; set; }
        public string? Line3WrokInfoArray { get; set; }
        public string? Line4WrokInfoArray { get; set; }
        public string? Line5WrokInfoArray { get; set; }
        public bool TriggerFinished { get; set; }
    }
}
