using car_storage_odometer.DataBaseModules; 
using car_storage_odometer.Helpers;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using car_storage_odometer.Models;

namespace car_storage_odometer.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDialogService _dialogService;
        public bool IsLoggedIn => _currentUserService.IsUserLoggedIn;
        private Dictionary<string, string> _userHashesPassword = new Dictionary<string, string>();

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
                    _currentUserService.ClearLoggedInUser();
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

            _currentUserService.ClearLoggedInUser();
            LoggedOut?.Invoke();
        }
    }
}