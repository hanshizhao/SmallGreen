using MaterialDesignThemes.Wpf;
using SmallGreen.Desktop.Settings.Common;

namespace SmallGreen.Desktop.Settings.ViewModels
{
    public class OperateSuccessViewModel : IDialogHostAware
    {
        public string HostName { get; set; } = "Root";
        public string Message { get; set; }

        public DelegateCommand CloseWindowCommand { get; }

        public OperateSuccessViewModel()
        {
            Message = "操作成功";
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
