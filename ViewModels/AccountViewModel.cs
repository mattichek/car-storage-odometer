using car_storage_odometer.DataBaseModules;
using car_storage_odometer.Helpers; // Zmieniono na Helpers, zakładając tam SqliteDataAccess
using car_storage_odometer.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions; // Dodano, aby używać INavigationAware
using System;
using System.ComponentModel;
using System.Threading.Tasks; // Dodano dla async/await
using System.Windows;

namespace car_storage_odometer.ViewModels
{
    // Zaktualizowano, aby implementował INavigationAware
    public class AccountViewModel : BindableBase, INavigationAware
    {
        private UserModel _loggedInUser; // Symulacja zalogowanego użytkownika (do autoryzacji zmiany hasła)
        private UserModel _editedUser;   // Kopia użytkownika do edycji w formularzu

        private string _oldPassword;
        private string _newPassword;
        private string _confirmNewPassword;

        private bool _isEditing; // Kontroluje, czy pola edycji są aktywne

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
        public DelegateCommand EditCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand CancelEditCommand { get; private set; }
        public DelegateCommand ChangePasswordCommand { get; private set; }

        // Założenie: Id zalogowanego użytkownika jest dostępne globalnie lub przekazywane
        // Zastąp to rzeczywistym sposobem pobierania ID zalogowanego użytkownika.
        private int CurrentUserId { get; set; } = 1; // PRZYKŁAD: Ustaw na ID zalogowanego użytkownika

        public AccountViewModel()
        {
            // Inicjalizacja ViewModelu
            EditedUser = new UserModel(); // Ustawienie początkowej pustej instancji

            EditCommand = new DelegateCommand(ExecuteEdit);
            SaveCommand = new DelegateCommand(async () => await ExecuteSave(), CanExecuteSave);
            CancelEditCommand = new DelegateCommand(ExecuteCancelEdit);
            ChangePasswordCommand = new DelegateCommand(async () => await ExecuteChangePassword(), CanChangePassword);

            IsEditing = false; // Domyślnie pola nie są edytowalne
        }

        // --- Metody do ładowania danych (wywoływane przez INavigationAware) ---
        private async Task LoadUserData(int userId)
        {
            try
            {
                // Załaduj dane użytkownika z bazy danych
                // Używamy SqliteDataAccess.LoadUserByIdAsync, którą za chwilę dodamy
                _loggedInUser = await SqliteDataAccessModifyingQuery.LoadUserByIdAsync(userId);
                if (_loggedInUser != null)
                {
                    EditedUser = _loggedInUser.Clone(); // Tworzymy kopię do edycji
                    // Resetujemy pola hasła
                    OldPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmNewPassword = string.Empty;
                }
                else
                {
                    MessageBox.Show("Nie można załadować danych użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania danych użytkownika: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                RaiseCanExecuteChangedForUserCommands();
            }
        }

        // --- Metody wykonawcze dla komend ---

        private void ExecuteEdit()
        {
            IsEditing = true;
            RaiseCanExecuteChangedForUserCommands();
        }

        private async Task ExecuteSave()
        {
            try
            {
                // Walidacja danych EditedUser (np. Email, FirstName, LastName)
                if (string.IsNullOrWhiteSpace(EditedUser.FirstName) ||
                    string.IsNullOrWhiteSpace(EditedUser.LastName) ||
                    string.IsNullOrWhiteSpace(EditedUser.Email))
                {
                    MessageBox.Show("Wszystkie pola (Imię, Nazwisko, Email) muszą być wypełnione.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Zapisz zmienione dane do bazy danych
                // Użyj SqliteDataAccess.UpdateUserProfileAsync, którą za chwilę dodamy
                await SqliteDataAccessModifyingQuery.UpdateUserProfileAsync(EditedUser);

                // Zaktualizuj _loggedInUser danymi z EditedUser
                _loggedInUser = EditedUser.Clone();

                IsEditing = false;
                MessageBox.Show("Dane użytkownika zostały zaktualizowane pomyślnie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisu danych użytkownika: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                RaiseCanExecuteChangedForUserCommands();
            }
        }

        private bool CanExecuteSave()
        {
            // Można zapisać tylko wtedy, gdy jest tryb edycji i wybrano użytkownika
            return IsEditing && EditedUser != null && EditedUser.UserId > 0;
        }

        private void ExecuteCancelEdit()
        {
            // Przywróć oryginalne dane z _loggedInUser
            EditedUser = _loggedInUser.Clone();
            IsEditing = false;
            RaiseCanExecuteChangedForUserCommands();
        }

        private async Task ExecuteChangePassword()
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

            try
            {
                // Sprawdź stare hasło (w prawdziwej aplikacji HASZUJ I PORÓWNUJ HASZE!)
                // Użyj SqliteDataAccess.VerifyUserPasswordAsync, którą za chwilę dodamy
                // W tej symulacji przyjmuję, że OldPassword to jawne hasło, które jest porównywane.
                // W rzeczywistości powinieneś przekazać OldPassword do metody weryfikującej hasło,
                // która następnie porówna zahaszowane wersje.
                bool isPasswordCorrect = await SqliteDataAccessModifyingQuery.VerifyUserPasswordAsync(CurrentUserId, OldPassword);

                if (!isPasswordCorrect)
                {
                    MessageBox.Show("Niepoprawne stare hasło.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Zmień hasło w bazie danych
                // Użyj SqliteDataAccess.UpdateUserPasswordAsync, którą za chwilę dodamy
                await SqliteDataAccessModifyingQuery.UpdateUserPasswordAsync(CurrentUserId, NewPassword); // Przekazuj jawne hasło, metoda powinna je haszować

                MessageBox.Show("Hasło zostało zmienione pomyślnie.", "Zmieniono hasło", MessageBoxButton.OK, MessageBoxImage.Information);

                // Wyczyść pola hasła po zmianie
                OldPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmNewPassword = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zmiany hasła: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ChangePasswordCommand.RaiseCanExecuteChanged(); // Zaktualizuj stan przycisku
            }
        }

        private bool CanChangePassword()
        {
            // Można zmieniać hasło, gdy wszystkie pola hasła są wypełnione
            return !string.IsNullOrWhiteSpace(OldPassword) &&
                   !string.IsNullOrWhiteSpace(NewPassword) &&
                   !string.IsNullOrWhiteSpace(ConfirmNewPassword);
        }

        // Metoda do aktualizowania stanu aktywności komend
        private void RaiseCanExecuteChangedForUserCommands()
        {
            SaveCommand.RaiseCanExecuteChanged();
            ChangePasswordCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            CancelEditCommand.RaiseCanExecuteChanged();
        }

        // --- Implementacja INavigationAware ---

        // Wywoływana, gdy widok jest aktywowany
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _ = LoadUserData(CurrentUserId);
            IsEditing = false; // Resetuj tryb edycji po nawigacji
        }

        // Określa, czy widok powinien być ponownie użyty
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true; // Zwróć true, aby ponownie używać istniejącej instancji ViewModelu
        }

        // Wywoływana, gdy widok jest dezaktywowany
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
             EditedUser = new UserModel();
            _loggedInUser = null;
            OldPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
            IsEditing = false;
        }
    }
}