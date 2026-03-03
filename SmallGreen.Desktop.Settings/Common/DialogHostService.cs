using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using SmallGreen.Desktop.Settings.Extensions;
using SmallGreen.Desktop.Settings.Views;

namespace SmallGreen.Desktop.Settings.Common
{
    public class DialogHostService : DialogService, IDialogHostService
    {
        private readonly IContainerExtension containerExtension;

        public DialogHostService(IContainerExtension containerExtension) : base(containerExtension)
        {
            this.containerExtension = containerExtension;
        }

        public async Task<IDialogResult> ShowDialog(string name, IDialogParameters? parameters, string dialogHostName = "Root")
        {
            //parameters ??= new DialogParameters();

            // 从容器中去除弹出窗口的实例
            var content = containerExtension.Resolve<object>(name);

            // 验证实例的有效性
            if (content is not FrameworkElement dialogContent)
            {
                throw new NullReferenceException("dialog的content属性必须是 FrameworkElement 类型");
            }

            if (dialogContent is FrameworkElement view && view.DataContext is null && ViewModelLocator.GetAutoWireViewModel(view) is null)
            {
                ViewModelLocator.SetAutoWireViewModel(view, true);
            }

            if (dialogContent.DataContext is not IDialogHostAware viewModel)
            {
                throw new NullReferenceException("dialog 的ViewModel属性必须要实现 IDialogAware 接口");
            }

            viewModel.HostName = dialogHostName;
            DialogOpenedEventHandler eventHandler = (sender, eventArgs) =>
            {
                if (viewModel is IDialogHostAware aware)
                {
                    if (parameters != null) aware.OnDialogOpend(parameters);
                }
                eventArgs.Session.UpdateContent(content);
            };

            if (DialogHost.IsDialogOpen(viewModel.HostName))
            {
                DialogHost.Close(viewModel.HostName);
                await Task.Delay(100);
            }

            return (IDialogResult)await DialogHost.Show(dialogContent, viewModel.HostName, eventHandler);
        }

        public void ShowDrawer(string name, IDialogParameters? parameters, Dock dock = Dock.Bottom)
        {
            var content = containerExtension.Resolve<object>(name);
            // 验证实例的有效性
            if (content is not UserControl dialogContent)
            {
                throw new NullReferenceException("dialog的content属性必须是 FrameworkElement 类型");
            }

            if (dialogContent is FrameworkElement view && view.DataContext is null && ViewModelLocator.GetAutoWireViewModel(view) is null)
            {
                ViewModelLocator.SetAutoWireViewModel(view, true);
            }

            if (dialogContent.DataContext is not IDrawerHostAware viewModel)
            {
                throw new NullReferenceException("dialog 的ViewModel属性必须要实现 IDrawerHostAware 接口");
            }

            // 执行抽屉中的初始化方法
            if (viewModel is IDrawerHostAware aware)
            {
                if (parameters != null) aware.OnDialogOpend(parameters);
            }
            var aggregator = containerExtension.Resolve<IEventAggregator>();
            aggregator.ShowBottomDrawer(dialogContent);

        }

        public void ShowDrawer(string name, Dock dock = Dock.Bottom)
        {
            var content = containerExtension.Resolve<object>(name);

            // 验证实例的有效性
            if (content is not UserControl dialogContent)
            {
                throw new NullReferenceException("dialog的content属性必须是 FrameworkElement 类型");
            }

            if (dialogContent is FrameworkElement view && view.DataContext is null && ViewModelLocator.GetAutoWireViewModel(view) is null)
            {
                ViewModelLocator.SetAutoWireViewModel(view, true);
            }

            if (dialogContent.DataContext is not IDrawerHostAware viewModel)
            {
                throw new NullReferenceException("dialog 的ViewModel属性必须要实现 IDrawerHostAware 接口");
            }

            var aggregator = containerExtension.Resolve<IEventAggregator>();
            aggregator.ShowBottomDrawer(dialogContent);
        }


        public void ShowNavigationDrawer(string name, NavigationParameters parameters)
        {
            var content = containerExtension.Resolve<object>(name);

            // 验证实例的有效性
            if (content is not UserControl dialogContent)
            {
                throw new NullReferenceException("dialog的content属性必须是 FrameworkElement 类型");
            }

            if (dialogContent is FrameworkElement view && view.DataContext is null && ViewModelLocator.GetAutoWireViewModel(view) is null)
            {
                ViewModelLocator.SetAutoWireViewModel(view, true);
            }

            if (dialogContent.DataContext is not INavigationAware viewModel)
            {
                throw new NullReferenceException("dialog 的ViewModel属性必须要实现 INavigationAware 接口");
            }

            var drawer = containerExtension.Resolve<DrawerNavigationView>();
            ViewModelLocator.SetAutoWireViewModel(drawer, true);
            var aware = (IDrawerHostAware)drawer.DataContext;

            var regionManager = containerExtension.Resolve<IRegionManager>();
            RegionManager.SetRegionManager(drawer, regionManager);
            RegionManager.UpdateRegions();

            var dialogParam = new DialogParameters
            {
                {"DefaultDrawer", name },
                {"DefaultDrawerParameters", parameters }
            };
            aware.OnDialogOpend(dialogParam);

            // 执行抽屉中的初始化方法
            var aggregator = containerExtension.Resolve<IEventAggregator>();
            aggregator.ShowBottomDrawer(drawer);

        }
    }

}
