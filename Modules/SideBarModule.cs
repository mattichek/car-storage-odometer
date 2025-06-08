using car_storage_odometer.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

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

        }
    }
}
