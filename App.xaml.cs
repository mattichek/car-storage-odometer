using car_storage_odometer.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Regions;
using System.Windows;

namespace car_storage_odometer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return ContainerLocator.Container.Resolve<MainPageView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //throw new System.NotImplementedException();
            containerRegistry.RegisterForNavigation<MainPageView>();

        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var regionManager = ContainerLocator.Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("TopBarRegion", typeof(TopBarView));
            regionManager.RegisterViewWithRegion("DashboardRegion", typeof(DashboardView));
            regionManager.RegisterViewWithRegion("LogsUsersRegion", typeof(LogsUsersView));
            regionManager.RegisterViewWithRegion("LogsDeviceRegion", typeof(LogsDeviceView));
            regionManager.RegisterViewWithRegion("HostoryOfRepairRegion", typeof(HostoryOfRepairView));
            regionManager.RegisterViewWithRegion("StorageRegion", typeof(StorageView));
            regionManager.RegisterViewWithRegion("DevicesRegion", typeof(DevicesView));
            regionManager.RegisterViewWithRegion("AccountRegion", typeof(AccountView));
        }
    }
}
