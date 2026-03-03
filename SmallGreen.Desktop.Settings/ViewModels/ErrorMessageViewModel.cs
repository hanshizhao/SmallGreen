using MaterialDesignThemes.Wpf;
using SmallGreen.Desktop.Settings.Common;

namespace SmallGreen.Desktop.Settings.ViewModels
{
    public class ErrorMessageViewModel : BindableBase, IDialogHostAware
    {
        public string HostName { get; set; } = "Root";

        public DelegateCommand CloseWindowCommand { get; private set; }


        private string message;

        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }


        public ErrorMessageViewModel()
        {
            message = "错误消息未传入";
            CloseWindowCommand = new DelegateCommand(CloseWindow);
        }

        private void CloseWindow()
        {
            if (DialogHost.IsDialogOpen(HostName))
            {
                DialogHost.Close(HostName, new DialogResult(ButtonResult.OK));
            }
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
