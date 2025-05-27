using car_storage_odometer.ViewModels;
using car_storage_odometer.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_storage_odometer.Modules
{
    public class SideBarModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("SideBarRegion", typeof(SideBarView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<DashboardView>();
            containerRegistry.RegisterForNavigation<DevicesView>();
            containerRegistry.RegisterForNavigation<HostoryOfRepairView>();
            containerRegistry.RegisterForNavigation<LogsDeviceView>();
            containerRegistry.RegisterForNavigation<LogsUsersView>();
            containerRegistry.RegisterForNavigation<StorageView>();

            containerRegistry.RegisterForNavigation<AccountView>();
        }
    }
}
