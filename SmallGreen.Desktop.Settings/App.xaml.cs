using SmallGreen.Desktop.Settings.Common;
using SmallGreen.Desktop.Settings.Dialogs;
using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Desktop.Settings.Services;
using SmallGreen.Desktop.Settings.ViewModels;
using SmallGreen.Desktop.Settings.Views;
using System.Windows;

namespace SmallGreen.Desktop.Settings
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 数据服务器
            containerRegistry.GetContainer()
                .Register<HttpRestClient>(made: Parameters.Of.Type<string>(serviceKey: "webUrl"));
            containerRegistry.GetContainer().RegisterInstance(@"http://192.168.1.180:8023/", serviceKey: "webUrl");

            // 注册登录窗口
            containerRegistry.RegisterDialog<LoginView, LoginViewModel>();
            
            
            // 弹窗
            containerRegistry.RegisterForNavigation<ConfirmOperateView, ConfirmOperateViewModel>();
            containerRegistry.RegisterForNavigation<UserPasswordChangeView, UserPasswordChangeViewModel>();
            containerRegistry.RegisterForNavigation<ErrorMessageView, ErrorMessageViewModel>();
            containerRegistry.RegisterForNavigation<AssBucketEditDialog, AssBucketEditDialogViewModel>();
            containerRegistry.RegisterForNavigation<MixedComponentEditDialog, MixedComponentEditDialogViewModel>();
            containerRegistry.RegisterForNavigation<MixedComponentsListDialog, MixedComponentsListDialogViewModel>();

            // 注册服务
            containerRegistry.Register<IDialogHostService, DialogHostService>();
            containerRegistry.Register<IUserService, UserService>();
            containerRegistry.Register<IPRCSDataService, PRCSDataService>();
            containerRegistry.Register<IChemicalBucketService, ChemicalBucketService>();
            containerRegistry.Register<IAssBucketService, AssBucketService>();
            


            // 注册导航窗体
            containerRegistry.RegisterForNavigation<ArthurView, ArthurViewModel>();
            containerRegistry.RegisterForNavigation<PRCSDataQueryView, PRCSDataQueryViewModel>();
            containerRegistry.RegisterForNavigation<PRCSDataDetailView, PRCSDataDetailViewModel>();
            containerRegistry.RegisterForNavigation<ChemicalBucketView, ChemicalBucketViewModel>();
            containerRegistry.RegisterForNavigation<ChemicalBucketDetailView, ChemicalBucketDetailViewModel>();
            containerRegistry.RegisterForNavigation<AssBucketManagementView, AssBucketManagementViewModel>();
            


        }

        protected override void OnInitialized()
        {
            var dialog = Container.Resolve<IDialogService>();

            dialog.ShowDialog(nameof(LoginView), callback =>
            {
                if (callback.Result != ButtonResult.OK)
                {
                    Environment.Exit(0);
                    return;
                }

                var mainWindow = Current.MainWindow;
                var regionManager = Container.Resolve<IRegionManager>();
                RegionManager.SetRegionManager(mainWindow, regionManager);
                RegionManager.UpdateRegions();

                if (mainWindow.DataContext is IConfigureService service) service.Configure();
                base.OnInitialized();
            });
        }
    }

}
