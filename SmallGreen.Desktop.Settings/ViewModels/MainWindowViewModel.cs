using MaterialDesignThemes.Wpf;
using SmallGreen.Desktop.Settings.Common;
using SmallGreen.Desktop.Settings.Extensions;
using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Desktop.Settings.Models;
using SmallGreen.Desktop.Settings.Views;
using SmallGreen.Dto.Data;

namespace SmallGreen.Desktop.Settings.ViewModels
{
    public class MainWindowViewModel : BindableBase, IConfigureService
    {
        private readonly IRegionManager regionManager;
        private readonly IUserService userService;
        private readonly IDialogHostService dialogHostService;
        private DateTime menuItemClickDateTime;
        private UserDto user = null!;

        public UserDto User
        {
            get { return user; }
            set { SetProperty(ref user, value); }
        }

        public DelegateCommand ChangeUserPasswordCommand { get; }

        private List<ItemMenu> listMenu = null!;

        public List<ItemMenu> ListMenu
        {
            get { return listMenu; }
            set { SetProperty(ref listMenu, value); }
        }

        public DelegateCommand<SubItem> MenuClickCommand { get; }

        public MainWindowViewModel(IContainerProvider container)
        {
            dialogHostService = container.Resolve<IDialogHostService>();
            regionManager = container.Resolve<IRegionManager>();
            userService = container.Resolve<IUserService>();

            MenuClickCommand = new DelegateCommand<SubItem>(MenuClick);
            ChangeUserPasswordCommand = new DelegateCommand(ChangeUserPassword);
            menuItemClickDateTime = DateTime.Now;
        }

        private void MenuClick(SubItem subItem)
        {
            if ((DateTime.Now - menuItemClickDateTime).TotalMilliseconds < 1000d) return;
            if (subItem == null) return;
            if (string.IsNullOrEmpty(subItem.Url)) return;
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(subItem.Url);
            menuItemClickDateTime = DateTime.Now;
        }

        private async void ChangeUserPassword()
        {
            // 显示修改密码弹窗
            var dialogParam = new DialogParameters
            {
                { "Title", "修改密码"}
            };
            await dialogHostService.ShowDialog(nameof(UserPasswordChangeView), dialogParam);

        }

        public void Configure()
        {
            User = GlobalParam.GetInstance().User;

            ListMenu = new List<ItemMenu>
            {
                new ItemMenu("小绿系统实时状态", new List<SubItem>
                {
                    new SubItem("助剂桶液位信息", nameof(ChemicalBucketView), false),
                    new SubItem("前处理配液信息", nameof(ArthurView), false),
                    new SubItem("固色配液信息", nameof(ArthurView), false),
                }, PackIconKind.TrayArrowUp),
                new ItemMenu("历史数据查询", new List<SubItem>
                {
                    new SubItem("配料记录", nameof(PRCSDataQueryView), false),
                }, PackIconKind.ClipboardTextClockOutline),
                new ItemMenu("助剂信息管理", new List<SubItem>
                {
                    new SubItem("助剂管理", nameof(ArthurView), false),
                    new SubItem("助剂桶管理", nameof(ArthurView), false),
                }, PackIconKind.ChemicalWeapon, false)


            };
        }
    }
}
