using car_storage_odometer.DataBaseModules; // Zakładam, że tu masz dostęp do bazy
using car_storage_odometer.Helpers;
using Prism.Services.Dialogs;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
// using car_storage_odometer.Models; // Jeśli masz model użytkownika

namespace car_storage_odometer.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDialogService _dialogService;
        public bool IsLoggedIn => _currentUserService.IsUserLoggedIn;

        public AuthenticationService(ICurrentUserService currentUserService, IDialogService dialogService)
        {
            _currentUserService = currentUserService;
            _dialogService = dialogService;
        }

        public event System.Action LoggedOut;

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                int? userId = await SqliteDataAccessModifyingQuery.AuthenticateUserAndGetIdAsync(email, password);

                if (userId.HasValue)
                {
                    _currentUserService.SetLoggedInUserId(userId.Value);
                    await SqliteDataAccessModifyingQuery.AddUserLogAsync(userId.Value, "Zalogowano do systemu.");
                    return true;
                }
                else
                {
                    _currentUserService.ClearLoggedInUser(); // Upewnij się, że UserID jest wyczyszczone
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                _currentUserService.ClearLoggedInUser();
                _dialogService.ShowDialog("CustomMessageBoxView",
                    new Prism.Services.Dialogs.DialogParameters { { "message", $"Błąd podczas logowania: {ex.Message}" }, { "title", "Błąd" }, { "buttons", CustomMessageBoxButtons.Ok } },
                    r => { });
                return false;
            }
        }

        public async Task Logout()
        {
            if (_currentUserService.IsUserLoggedIn)
            {
                int? currentUserId = _currentUserService.LoggedInUserId;
                if (currentUserId.HasValue)
                {
                    await SqliteDataAccessModifyingQuery.AddUserLogAsync(currentUserId.Value, "Wylogowano z systemu.");
                }
            }

            _currentUserService.ClearLoggedInUser(); // Wyczyść ID zalogowanego użytkownika
            LoggedOut?.Invoke(); // Wywołaj event
        }
    }
}