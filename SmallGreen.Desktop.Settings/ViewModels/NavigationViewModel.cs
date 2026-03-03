using SmallGreen.Desktop.Settings.Common;
using SmallGreen.Desktop.Settings.Extensions;
using SmallGreen.Desktop.Settings.Views;

namespace SmallGreen.Desktop.Settings.ViewModels
{
    public class NavigationViewModel : BindableBase, INavigationAware
    {
        private readonly IDialogHostService dialogHostService;
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        //private readonly ISignalRClientService signalRClientService;
        //private HubConnection hubConnection;
        //private string hubConnectionKey = null!;
        //private SubSystemName hubSubSystemName;

        public NavigationViewModel(IContainerProvider container)
        {
            dialogHostService = container.Resolve<IDialogHostService>();
            eventAggregator = container.Resolve<IEventAggregator>();
            regionManager = container.Resolve<IRegionManager>();
            //signalRClientService = container.Resolve<ISignalRClientService>();

            //hubConnection = new HubConnectionBuilder().WithUrl("http://10.24.19.202:8823/CowboyHub")
            //    .WithAutomaticReconnect()
            //    .Build();
        }

        //public async Task HubConnect<T>(SubSystemName subSystemName, string hubKey, Action<T> action)
        //{
        //    hubSubSystemName = subSystemName;
        //    hubConnectionKey = hubKey;

        //    // 重连成功时
        //    hubConnection.Reconnected += arg =>
        //    {
        //        Application.Current.Dispatcher.Invoke(() =>
        //        {
        //            Loading(false);
        //            RegisterSignalRClient();
        //        });
        //        return Task.CompletedTask;
        //    };

        //    hubConnection.On(hubConnectionKey, action);
        //    if (hubConnection.State == HubConnectionState.Disconnected) await hubConnection.StartAsync();
        //    RegisterSignalRClient();
        //}

        //private async void RegisterSignalRClient()
        //{
        //    if (hubConnection == null)
        //    {
        //        await ShowErrorMessage($"注册SignalR信息失败:hub连接对象为空");
        //        return;
        //    }
        //    if (string.IsNullOrEmpty(hubConnection.ConnectionId))
        //    {
        //        await ShowErrorMessage($"注册SignalR信息失败：hub连接对象ID为空");
        //        return;
        //    }
        //    if (hubConnection.State != HubConnectionState.Connected)
        //    {
        //        await ShowErrorMessage($"注册SignalR信息失败：hub连接状态不正确(State: {hubConnection.State})");
        //        return;
        //    }
        //    var result = await signalRClientService.SetConnctionKey(new SignalRClientDto
        //    {
        //        ConnectionID = hubConnection.ConnectionId,
        //        ConnectionKey = hubConnectionKey,
        //        SubSystemName = hubSubSystemName
        //    });
        //    if (!result.IsSuccess)
        //    {
        //        await ShowErrorMessage($"注册SignalR信息失败：{result.Message}");
        //        Application.Current.Shutdown();
        //    }
        //}

        public void ShowDrawer(string url, DialogParameters dialogParam)
        {
            dialogHostService.ShowDrawer(url, dialogParam);
        }

        /// <summary>
        /// 显示带导航的抽屉
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        public void ShowNavigationDrawer(string url, NavigationParameters param)
        {
            dialogHostService.ShowNavigationDrawer(url, param);
        }

        public async Task ShowDialog(string url, DialogParameters dialogParam)
        {
            await dialogHostService.ShowDialog(url, dialogParam);
        }

        public IEventAggregator GetEventAggregator()
        {
            return eventAggregator;
        }


        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            // 每次都重新创建一个新实例
            return false;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //await hubConnection.StopAsync();
            //await hubConnection.DisposeAsync();
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public void Loading(bool isLoading)
        {
            eventAggregator.ShowLoading(isLoading);
        }

        public async Task ShowErrorMessage(string message)
        {
            await dialogHostService.ShowDialog(nameof(ErrorMessageView), new DialogParameters { { "Message", message } });
        }

        public async Task ShowSuccessMessage(string message = "操作成功")
        {
            await dialogHostService.ShowDialog(nameof(OperateSuccessView), new DialogParameters { { "Message", message } });
        }

        public async Task<IDialogResult> ShowConfirmOperateView(string message)
        {
            var param = new DialogParameters
            {
                {"Message", message}
            };
            return await dialogHostService.ShowDialog(nameof(ConfirmOperateView), param);
        }

        public void NavigationTo(string url, NavigationParameters param)
        {
            regionManager.Regions[PrismManager.DrawerRegion].RequestNavigate(url, param);
        }

        public IDialogHostService GetDialogHostService()
        {
            return dialogHostService;
        }
    }
}
