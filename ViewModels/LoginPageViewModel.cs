using car_storage_odometer.Helpers;
using car_storage_odometer.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
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
        private readonly IDialogService _dialogService;
        private readonly ICurrentUserService _currentUserService;

        public DelegateCommand LoginCommand { get; private set; }

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

        public LoginPageViewModel(IRegionManager regionManager, IAuthenticationService authenticationService, IDialogService dialogService, ICurrentUserService currentUserService){
            _currentUserService = currentUserService;
            _regionManager = regionManager;
            _authenticationService = authenticationService; // Przypisanie serwisu
            _dialogService = dialogService;

            LoginCommand = new DelegateCommand(async () => await ExecuteLogin());
        }

        private async Task ExecuteLogin()
        {
            if (string.IsNullOrWhiteSpace(Mail) || string.IsNullOrWhiteSpace(Password))
            {
                _dialogService.ShowDialog(
                    "CustomMessageBoxView",
                    new DialogParameters
                    {
                        { "title", "Błąd logowania" },
                        { "message", "Proszę podać adres e-mail i hasło." },
                        { "buttons", CustomMessageBoxButtons.Ok }
                    },
                    r => {});

                return;
            }
            string hashedPassword = PasswordBoxHelper.HashPassword(Password);

            bool loginSuccessful = await _authenticationService.LoginAsync(Mail, hashedPassword);

            if (loginSuccessful)
            {
                _regionManager.RequestNavigate("FullScreenRegion", "DashboardWithSideBarView");
            }
            else
            {
                _dialogService.ShowDialog(
                    "CustomMessageBoxView",
                    new DialogParameters
                    {
                        { "title", "Błąd logowania" },
                        { "message", "Nieprawidłowy adres e-mail lub hasło." },
                        { "buttons", CustomMessageBoxButtons.Ok }
                    },
                    r => { });
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
