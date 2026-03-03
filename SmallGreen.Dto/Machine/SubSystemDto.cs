using SmallGreen.Common;

namespace SmallGreen.Dto.Machine
{
    public class SubSystemDto
    {
        public long Id { get; set; }
        public SubSystemName SubSystemName { get; set; }
        public long PlcID { get; set; }
        public string? DataAssCoeArray { get; set; }
        public List<EquipmentDto> ListEquipment { get; set; } = [];
    }
}
