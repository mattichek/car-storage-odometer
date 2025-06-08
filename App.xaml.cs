using car_storage_odometer.Modules;
using car_storage_odometer.Services;
using car_storage_odometer.ViewModels;
using car_storage_odometer.Views;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
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
            
            containerRegistry.RegisterForNavigation<LoginPageView, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<DashboardWithSideBarView, DashboardWithSideBarViewModel>(); // Nowy widok po zalogowaniu
            
            containerRegistry.RegisterForNavigation<DashboardView, DashboardViewModel>();
            containerRegistry.RegisterForNavigation<LogsUsersView, LogsUsersViewModel>();
            containerRegistry.RegisterForNavigation<LogsDeviceView, LogsDeviceViewModel>();
            containerRegistry.RegisterForNavigation<HistoryOfRepairView, HistoryOfRepairViewModel>();
            containerRegistry.RegisterForNavigation<AccountView, AccountViewModel>();
            containerRegistry.RegisterForNavigation<DevicesView, DevicesViewModel>();
            containerRegistry.RegisterDialog<CustomMessageBoxView, CustomMessageBoxViewModel>();
            containerRegistry.RegisterSingleton<IAuthenticationService, AuthenticationService>();
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var regionManager = ContainerLocator.Container.Resolve<IRegionManager>();
            regionManager.RequestNavigate("FullScreenRegion", "LoginPageView");
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<Modules.SideBarModule>();
        }
    }
}
