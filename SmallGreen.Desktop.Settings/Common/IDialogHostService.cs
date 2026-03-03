using System.Windows.Controls;

namespace SmallGreen.Desktop.Settings.Common
{
    public interface IDialogHostService : IDialogService
    {
        Task<IDialogResult> ShowDialog(string name, IDialogParameters? parameters, string dialogHostName = "Root");

        void ShowDrawer(string name, IDialogParameters? parameters, Dock dock = Dock.Bottom);
        void ShowDrawer(string name, Dock dock = Dock.Bottom);
        void ShowNavigationDrawer(string name, NavigationParameters parameters);
    }
}
