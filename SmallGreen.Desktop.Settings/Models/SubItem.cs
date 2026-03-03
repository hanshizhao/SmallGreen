namespace SmallGreen.Desktop.Settings.Models
{
    public class SubItem
    {
        public string Title { get; private set; }
        public string Url { get; private set; }
        public bool IsSelected { get; set; }
        public SubItem(string title, string url, bool isSelected = false)
        {
            Title = title;
            Url = url;
            IsSelected = isSelected;
        }
    }
}
