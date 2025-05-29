using car_storage_odometer.Models;
using ImTools;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows;

namespace car_storage_odometer.ViewModels
{
    public class AccountViewModel : BindableBase
    {
        private UserModel _loggedInUser; // Symulacja zalogowanego użytkownika
        private UserModel _editedUser;   // Kopia użytkownika do edycji

        private string _oldPassword;
        private string _newPassword;
        private string _confirmNewPassword;

        private bool _isEditing;

        // --- Właściwości Publiczne (do wiązania z XAML) ---
        public UserModel EditedUser
        {
            get => _editedUser;
            set => SetProperty(ref _editedUser, value);
        }

        public string OldPassword
        {
            get => _oldPassword;
            set
            {
                SetProperty(ref _oldPassword, value);
                ChangePasswordCommand.RaiseCanExecuteChanged();
            }
        }

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                SetProperty(ref _newPassword, value);
                ChangePasswordCommand.RaiseCanExecuteChanged();
            }
        }

        public string ConfirmNewPassword
        {
            get => _confirmNewPassword;
            set
            {
                SetProperty(ref _confirmNewPassword, value);
                ChangePasswordCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        // --- Komendy ---
        public DelegateCommand EditAccountCommand { get; private set; }
        public DelegateCommand SaveAccountCommand { get; private set; }
        public DelegateCommand CancelEditCommand { get; private set; }
        public DelegateCommand ChangePasswordCommand { get; private set; }

        // --- Konstruktor ---
        public AccountViewModel()
        {
            // Inicjalizacja komend
            EditAccountCommand = new DelegateCommand(ExecuteEditAccount, CanExecuteEditAccount);
            SaveAccountCommand = new DelegateCommand(ExecuteSaveAccount, CanSaveAccount);
            CancelEditCommand = new DelegateCommand(ExecuteCancelEdit);
            ChangePasswordCommand = new DelegateCommand(ExecuteChangePassword, () => CanChangePassword());

            // Symulacja ładowania danych zalogowanego użytkownika
            LoadLoggedInUser();

            // Ustaw początkowy stan
            ExecuteCancelEdit(); // Inicjalizuje EditedUser jako kopię
        }

        // --- Metody Prywatne (implementacje komend i pomocnicze) ---
        private void LoadLoggedInUser()
        {
            _loggedInUser = new UserModel
            {
                UserId = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jan.kowalski@example.com",
                Password = "hashed_password_123", // Nigdy nie przechowuj hasła jawnie!
                RegistrationDate = new DateTime(2023, 1, 15),
                IsActive = true,
                Role = "Administrator"
            };
        }

        private void ExecuteEditAccount()
        {
            IsEditing = true;
            SaveAccountCommand.RaiseCanExecuteChanged();
            CancelEditCommand.RaiseCanExecuteChanged();
            EditAccountCommand.RaiseCanExecuteChanged(); // Zablokuj przycisk Edytuj
        }

        private bool CanExecuteEditAccount()
        {
            return !IsEditing;
        }

        private void ExecuteSaveAccount()
        {
            if (EditedUser == null) return;

            // Walidacja podstawowych pól
            if (string.IsNullOrWhiteSpace(EditedUser.FirstName) ||
                string.IsNullOrWhiteSpace(EditedUser.LastName) ||
                string.IsNullOrWhiteSpace(EditedUser.Email))
            {
                MessageBox.Show("Wszystkie pola (Imię, Nazwisko, Email) muszą być wypełnione.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Walidacja formatu email (uproszczona)
            if (!EditedUser.Email.Contains("@") || !EditedUser.Email.Contains("."))
            {
                MessageBox.Show("Wprowadź poprawny format adresu email.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // W prawdziwej aplikacji: wyślij zaktualizowane dane do serwisu/repozytorium
            // Pamiętaj, aby nie aktualizować hasła tą samą metodą co inne dane profilu.
            // Sprawdź, czy Email nie jest już używany przez innego użytkownika (jeśli zmieniono)
            // ... logika zapisu do bazy danych ...

            _loggedInUser.FirstName = EditedUser.FirstName;
            _loggedInUser.LastName = EditedUser.LastName;
            _loggedInUser.Email = EditedUser.Email;
            // Pozostałe pola są tylko do odczytu lub zmieniane inną metodą (np. hasło)

            MessageBox.Show("Dane konta zostały zaktualizowane.", "Zapisano", MessageBoxButton.OK, MessageBoxImage.Information);
            IsEditing = false;
            SaveAccountCommand.RaiseCanExecuteChanged();
            CancelEditCommand.RaiseCanExecuteChanged();
            EditAccountCommand.RaiseCanExecuteChanged(); // Odblokuj przycisk Edytuj
        }

        private bool CanSaveAccount()
        {
            // Można zapisać tylko w trybie edycji
            return IsEditing;
        }

        private void ExecuteCancelEdit()
        {
            // Przywróć oryginalne dane użytkownika
            EditedUser = _loggedInUser.Clone(); 

            IsEditing = false;
            OldPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;

            SaveAccountCommand.RaiseCanExecuteChanged();
            ChangePasswordCommand.RaiseCanExecuteChanged();
            CancelEditCommand.RaiseCanExecuteChanged();
            EditAccountCommand.RaiseCanExecuteChanged();
        }

        private void ExecuteChangePassword()
        {
            
            if (string.IsNullOrWhiteSpace(OldPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmNewPassword))
            {
                MessageBox.Show("Wszystkie pola hasła muszą być wypełnione.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewPassword != ConfirmNewPassword)
            {
                MessageBox.Show("Nowe hasło i potwierdzenie hasła nie są zgodne.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewPassword.Length < 6) 
            {
                MessageBox.Show("Nowe hasło musi mieć co najmniej 6 znaków.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (OldPassword != "admin123") 
            {
                MessageBox.Show("Niepoprawne stare hasło.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _loggedInUser.Password = "hashed_" + NewPassword; 
            MessageBox.Show("Hasło zostało zmienione pomyślnie.", "Zmieniono hasło", MessageBoxButton.OK, MessageBoxImage.Information);

            
            OldPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
        }

        private bool CanChangePassword()
        {
            return !string.IsNullOrWhiteSpace(OldPassword) &&
                   !string.IsNullOrWhiteSpace(NewPassword) &&
                   !string.IsNullOrWhiteSpace(ConfirmNewPassword);
        }
    }
}
