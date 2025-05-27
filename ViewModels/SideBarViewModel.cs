using Prism.Commands; 
using Prism.Mvvm;
using Prism.Regions;

namespace car_storage_odometer.ViewModels
{
    public class SideBarViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public DelegateCommand<string> NavigateCommand { get; private set; }

        public SideBarViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateCommand = new DelegateCommand<string>(Navigate, (s) => true);
        }

        private void Navigate(string viewName)
        {
            _regionManager.RequestNavigate("MainContentRegion", viewName);
        }
    }
}
