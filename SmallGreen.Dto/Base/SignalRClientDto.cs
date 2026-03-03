using SmallGreen.Common;

namespace SmallGreen.Dto.Base
{
    public class SignalRClientDto
    {
        public SubSystemName SubSystemName { get; set; }

        public string ConnectionID { get; set; } = null!;

        public string? ConnectionKey { get; set; }

        public DateTime PostDate { get; set; }
    }
}
