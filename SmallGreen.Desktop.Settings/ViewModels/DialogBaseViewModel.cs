using MaterialDesignThemes.Wpf;
using SmallGreen.Desktop.Settings.Common;
using SmallGreen.Desktop.Settings.Extensions;
using SmallGreen.Desktop.Settings.Views;

namespace SmallGreen.Desktop.Settings.ViewModels
{
    public class DialogBaseViewModel : BindableBase, IDialogHostAware
    {
        private readonly IDialogHostService dialogHostService;
        private readonly IEventAggregator eventAggregator;
        internal CancellationTokenSource? CancelToken;

        public string HostName { get; set; }
        public string Title { get; set; }

        public DelegateCommand CloseWindowCommand { get; }

        public DialogBaseViewModel(IContainer container)
        {
            dialogHostService = container.Resolve<IDialogHostService>();
            eventAggregator = container.Resolve<IEventAggregator>();

            HostName = "Root";
            Title = "未传入标题参数";
            CloseWindowCommand = new DelegateCommand(Close);
        }

        public IEventAggregator GetEventAggregator()
        {
            return eventAggregator;
        }

        public IDialogHostService GetDialogHostService()
        {
            return dialogHostService;
        }

        private void Close()
        {
            if (DialogHost.IsDialogOpen(HostName))
            {
                if (CancelToken != null)
                {
                    CancelToken.Cancel();
                    CancelToken.Dispose();
                }
                DialogHost.Close(HostName, new DialogResult(ButtonResult.Ignore));
            }

        }

        public async Task ShowErrorMessage(string message)
        {
            await dialogHostService.ShowDialog(nameof(ErrorMessageView), new DialogParameters { { "Message", message } });
        }

        public async Task ShowSuccessMessage(string message = "操作成功")
        {
            await dialogHostService.ShowDialog(nameof(OperateSuccessView), new DialogParameters { { "Message", message } });
        }


        public void Loading(bool isLoading)
        {
            eventAggregator.ShowLoading(isLoading);
        }

        public virtual void OnDialogOpend(IDialogParameters parameters)
        {
            if (!parameters.ContainsKey("Title")) throw new System.Exception("必须传入Title参数");
            Title = parameters.GetValue<string>("Title");
        }
    }
}
