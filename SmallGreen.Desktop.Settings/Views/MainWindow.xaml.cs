using MaterialDesignThemes.Wpf;
using SmallGreen.Desktop.Settings.Common;
using SmallGreen.Desktop.Settings.Extensions;
using System.Windows;
using System.Windows.Input;

namespace SmallGreen.Desktop.Settings.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IEventAggregator aggregator, IDialogHostService dialogHostService)
        {
            InitializeComponent();

            //注册等待消息窗口
            aggregator.RegisterLoading(isLoading =>
            {
                DialogHost.IsOpen = isLoading;

                if (DialogHost.IsOpen)
                    DialogHost.DialogContent = new LoadingView();
            });

            // 注册底部弹窗事件
            aggregator.RegisterShowDrawerBottomEvent(async userControl =>
            {
                if (DrawerHost.IsBottomDrawerOpen)
                {
                    // 如果下方抽屉是打开的 那就关闭
                    DrawerHost.IsBottomDrawerOpen = false;
                    await Task.Delay(500);
                }

                DrawerHost.BottomDrawerContent = userControl;
                DrawerHost.IsBottomDrawerOpen = true;
            });

            btnClose.Click += async (s, e) =>
            {
                var dialogResult = await dialogHostService.Question("温馨提示", "确认退出系统?");
                if (dialogResult.Result != ButtonResult.Yes) return;
                this.Close();
            };

        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 最小化
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            double scWidth = SystemParameters.WorkArea.Width;
            double scHeight = SystemParameters.WorkArea.Height;
            // 窗口化/最大化
            if (Width == scWidth)
            {
                Width = 1366;
                Height = 768;
                Top = (scHeight - Height) / 2;
                Left = (scWidth - Width) / 2;
                IconMax.Kind = PackIconKind.Maximize;
                ResizeMode = ResizeMode.CanResizeWithGrip;
            }
            else
            {
                Width = scWidth;
                Height = scHeight;
                Top = 0;
                Left = 0;
                IconMax.Kind = PackIconKind.WindowRestore;
                ResizeMode = ResizeMode.NoResize;
            }
        }


        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception)
            {
            }
        }
    }
}