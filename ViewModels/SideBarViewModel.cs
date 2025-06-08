using car_storage_odometer.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace car_storage_odometer.ViewModels
{
    public class SideBarViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand<string> NavigateCommand { get; private set; }
        public DelegateCommand LogoutCommand { get; }

        public SideBarViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            NavigateCommand = new DelegateCommand<string>(Navigate, _ => CanNavigate);
            LogoutCommand = new DelegateCommand(ExecuteLogout);
        }

        private bool _canNavigate = true;
        public bool CanNavigate
        {
            get => _canNavigate; 
            set 
            { 
                _canNavigate = value; 
                SetProperty(ref _canNavigate, value);
            }
        }

        private void Navigate(string viewName)
        {
            _regionManager.RequestNavigate("MainContentRegion", viewName);
        }

        private void ExecuteLogout()
        {
            _eventAggregator.GetEvent<LogoutEvent>().Publish(); 
        }

        public void Logout()
        {
            _regionManager.RequestNavigate("FullScreenRegion", "LoginPageView");
        }
    }
}
