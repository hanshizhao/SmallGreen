using MaterialDesignThemes.Wpf;

namespace SmallGreen.Desktop.Settings.Models
{
    public class ItemMenu
    {
        public string Title { get; private set; }
        public bool IsOpen { get; set; }
        public PackIconKind Icon { get; private set; }
        public List<SubItem>? SubItems { get; set; }

        public ItemMenu(string title, List<SubItem>? subItems, PackIconKind icon, bool isOpen = false)
        {
            Title = title;
            Icon = icon;
            SubItems = subItems;
            IsOpen = isOpen;
        }

    }
}
