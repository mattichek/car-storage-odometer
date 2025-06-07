using car_storage_odometer.Events;
using car_storage_odometer.Services;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Linq;
using System.Windows; 

namespace car_storage_odometer.ViewModels
{
    public class DashboardWithSideBarViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly IEventAggregator _eventAggregator; 


        public DashboardWithSideBarViewModel(IRegionManager regionManager, IAuthenticationService authenticationService, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _authenticationService = authenticationService;
            _eventAggregator = eventAggregator; 

            _eventAggregator.GetEvent<LogoutEvent>().Subscribe(ExecuteLogout);

            _authenticationService.LoggedOut += OnLoggedOut;
        }

        private void ExecuteLogout() 
        {
            if (MessageBox.Show("Czy na pewno chcesz się wylogować?", "Wylogowanie", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _authenticationService.Logout();
            }
        }

        private void OnLoggedOut()
        {
            _regionManager.RequestNavigate("FullScreenRegion", "LoginPageView");
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _regionManager.RequestNavigate("SideBarRegion", "SideBarView");
            _regionManager.RequestNavigate("MainContentRegion", "DashboardView");
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        private void ResetRegion(string regionName, string defaultViewName)
        {
            var region = _regionManager.Regions.FirstOrDefault(r => r.Name == regionName);
            if (region != null)
            {
                foreach (var activeView in region.ActiveViews.ToList())
                {
                    region.Deactivate(activeView);
                }

                _regionManager.RequestNavigate(regionName, defaultViewName);
            }
        }
    }
    
}