using car_storage_odometer.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace car_storage_odometer.ViewModels
{
    public class LoginPageViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private readonly IAuthenticationService _authenticationService;

        public DelegateCommand LoginCommand { get; private set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _mail;
        public string Mail
        {
            get => _mail;
            set
            {
                SetProperty(ref _mail, value);
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
            }
        }

        public LoginPageViewModel(IRegionManager regionManager, IAuthenticationService authenticationService) // Zmieniono konstruktor
        {
            _regionManager = regionManager;
            _authenticationService = authenticationService; // Przypisanie serwisu
            LoginCommand = new DelegateCommand(async () => await ExecuteLogin());
            Title = "Logowanie do systemu";
        }

        private async Task ExecuteLogin()
        {
            if (string.IsNullOrWhiteSpace(Mail) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Proszę podać adres e-mail i hasło.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Użyj serwisu do logowania
            bool loginSuccessful = await _authenticationService.LoginAsync(Mail, Password);

            if (loginSuccessful)
            {
                _regionManager.RequestNavigate("FullScreenRegion", "DashboardWithSideBarView");
            }
            else
            {
                MessageBox.Show("Nieprawidłowy adres e-mail lub hasło.", "Błąd logowania", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Mail = string.Empty;
            Password = string.Empty;
        }
        public void OnNavigatedTo(NavigationContext navigationContext) { }
        public bool IsNavigationTarget(NavigationContext navigationContext) { return true; }
    }
}
