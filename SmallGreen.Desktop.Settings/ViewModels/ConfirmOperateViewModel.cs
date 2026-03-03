using MaterialDesignThemes.Wpf;
using SmallGreen.Desktop.Settings.Common;

namespace SmallGreen.Desktop.Settings.ViewModels
{
    public class ConfirmOperateViewModel : BindableBase, IDialogHostAware
    {
        public string Message { get; set; } = null!;
        public string HostName { get; set; } = "Root";
        public DelegateCommand YesCommand { get; private set; }
        public DelegateCommand CloseWindowCommand { get; private set; }

        public ConfirmOperateViewModel()
        {
            YesCommand = new DelegateCommand(() =>
            {
                if (DialogHost.IsDialogOpen(HostName))
                {
                    DialogHost.Close(HostName, new DialogResult(ButtonResult.Yes));
                }
            });

            CloseWindowCommand = new DelegateCommand(() =>
            {
                if (DialogHost.IsDialogOpen(HostName))
                {
                    DialogHost.Close(HostName, new DialogResult(ButtonResult.Ignore));
                }
            });
        }

        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Message"))
            {
                Message = parameters.GetValue<string>("Message");
            }
        }

    }
}
