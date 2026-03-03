using SmallGreen.Desktop.Settings.Common;
using SmallGreen.Desktop.Settings.Extensions;

namespace SmallGreen.Desktop.Settings.ViewModels
{
    public class DrawerNavigationViewModel : BindableBase, IDrawerHostAware
    {
        private readonly IRegionManager regionManager;

        public DrawerNavigationViewModel(IContainer container)
        {
            regionManager = container.Resolve<IRegionManager>();
        }

        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (!parameters.ContainsKey("DefaultDrawer")) throw new Exception("必须传入参数[DefaultDrawer]");
            if (!parameters.ContainsKey("DefaultDrawerParameters")) throw new Exception("必须传入参数[DefaultDrawerParameters]");

            var defaultDrawer = parameters.GetValue<string>("DefaultDrawer");
            var defaultDrawerParameters = parameters.GetValue<NavigationParameters>("DefaultDrawerParameters");

            regionManager.Regions[PrismManager.DrawerRegion].RequestNavigate(defaultDrawer, defaultDrawerParameters);
        }

    }
}
