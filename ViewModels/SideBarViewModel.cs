using Prism.Commands; 
using Prism.Mvvm;
using Prism.Regions;
using System.Security.AccessControl;

namespace car_storage_odometer.ViewModels
{
    public class SideBarViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public DelegateCommand<string> NavigateCommand { get; private set; }

        public SideBarViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateCommand = new DelegateCommand<string>(Navigate, _ => CanNavigate);
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
    }
}
