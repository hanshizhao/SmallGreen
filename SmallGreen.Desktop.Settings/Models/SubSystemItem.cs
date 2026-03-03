namespace SmallGreen.Desktop.Settings.Models
{
    public class SubSystemItem
    {
        public string DisplayName { get; set; } = null!;
        public string[] SubSystemNames { get; set; } = Array.Empty<string>();
        public bool IsShared { get; set; }
    }
}
