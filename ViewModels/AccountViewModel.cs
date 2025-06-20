﻿using car_storage_odometer.DataBaseModules;
using car_storage_odometer.Helpers;
using car_storage_odometer.Models;
using car_storage_odometer.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;

namespace car_storage_odometer.ViewModels
{
    public class AccountViewModel : BindableBase, INavigationAware
    {
        private readonly IDialogService _dialogService; 
        private readonly ICurrentUserService _currentUserService;
        private UserModel _loggedInUser; 
        private UserModel _editedUser;
        private string _oldPassword;
        private string _newPassword;
        private string _confirmNewPassword;

        private bool _isEditing; 

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

        public DelegateCommand EditCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand CancelEditCommand { get; private set; }
        public DelegateCommand ChangePasswordCommand { get; private set; }


        public AccountViewModel(IDialogService dialogService, ICurrentUserService currentUserService)
        {
            _dialogService = dialogService;
            _currentUserService = currentUserService;

            EditedUser = new UserModel(); 

            EditCommand = new DelegateCommand(ExecuteEdit);
            SaveCommand = new DelegateCommand(async () => await ExecuteSave(), CanExecuteSave);
            CancelEditCommand = new DelegateCommand(ExecuteCancelEdit);
            ChangePasswordCommand = new DelegateCommand(async () => await ExecuteChangePassword(), CanChangePassword);

            IsEditing = false; 
        }

        private async Task LoadUserData() 
        {
            if (!_currentUserService.IsUserLoggedIn) 
            {
                ShowMessageBoxOk("Błąd: Brak zalogowanego użytkownika. Zaloguj się ponownie.", "Błąd autoryzacji");
                return;
            }


            try
            {
                _loggedInUser = await SqliteDataAccessModifyingQuery.LoadUserByIdAsync(_currentUserService.LoggedInUserId.Value);
                if (_loggedInUser != null)
                {
                    EditedUser = _loggedInUser.Clone();
                    OldPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmNewPassword = string.Empty;
                }
                else
                {
                    ShowMessageBoxOk("Nie znaleziono użytkownika o podanym ID. Proszę skontaktować się z administratorem.", "Błąd ładowania danych");
                    _currentUserService.ClearLoggedInUser();
                }
            }
            catch (Exception ex)
            {
                ShowMessageBoxOk($"Błąd podczas ładowania danych użytkownika: {ex.Message}", "Błąd ładowania danych");
            }
            finally
            {
                RaiseCanExecuteChangedForUserCommands();
            }
        }

        private void ExecuteEdit()
        {
            IsEditing = true;
            RaiseCanExecuteChangedForUserCommands();
        }

        private async Task ExecuteSave()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EditedUser.FirstName) ||
                    string.IsNullOrWhiteSpace(EditedUser.LastName) ||
                    string.IsNullOrWhiteSpace(EditedUser.Email))
                {
                    ShowMessageBoxOk("Wszystkie pola (Imię, Nazwisko, Email) muszą być wypełnione.", "Błąd walidacji");
                    return;
                }

                await SqliteDataAccessModifyingQuery.UpdateUserProfileAsync(EditedUser);

                _loggedInUser = EditedUser.Clone();

                IsEditing = false;
                ShowMessageBoxOk("Dane użytkownika zostały zaktualizowane pomyślnie.", "Sukces");
            }
            catch (Exception ex)
            {
                ShowMessageBoxOk($"Błąd podczas zapisu danych użytkownika: {ex.Message}", "Błąd zapisu danych");
            }
            finally
            {
                RaiseCanExecuteChangedForUserCommands();
            }
        }

        private bool CanExecuteSave()
        {
            return IsEditing && EditedUser != null && EditedUser.UserId > 0;
        }

        private void ExecuteCancelEdit()
        {
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
                ShowMessageBoxOk("Wszystkie pola hasła muszą być wypełnione.", "Błąd walidacji");
                return;
            }

            if (NewPassword != ConfirmNewPassword)
            {
                ShowMessageBoxOk("Nowe hasło i potwierdzenie hasła nie są zgodne.", "Błąd walidacji");
                return;
            }

            if (NewPassword.Length < 6)
            {
                ShowMessageBoxOk("Nowe hasło musi mieć co najmniej 6 znaków.", "Błąd walidacji");
                return;
            }

            try
            {
                bool isPasswordCorrect = await SqliteDataAccessModifyingQuery.VerifyUserPasswordAsync(_currentUserService.LoggedInUserId.Value, OldPassword);

                if (!isPasswordCorrect)
                {
                    ShowMessageBoxOk("Niepoprawne stare hasło.", "Błąd autoryzacji");
                    return;
                }

                await SqliteDataAccessModifyingQuery.UpdateUserPasswordAsync(_currentUserService.LoggedInUserId.Value, NewPassword);
                ShowMessageBoxOk("Hasło zostało zmienione pomyślnie.", "Sukces");

                OldPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmNewPassword = string.Empty;
            }
            catch (Exception ex)
            {
                ShowMessageBoxOk($"Błąd podczas zmiany hasła: {ex.Message}", "Błąd zmiany hasła");
            }
            finally
            {
                ChangePasswordCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanChangePassword()
        {
            return !string.IsNullOrWhiteSpace(OldPassword) &&
                   !string.IsNullOrWhiteSpace(NewPassword) &&
                   !string.IsNullOrWhiteSpace(ConfirmNewPassword);
        }

        private void RaiseCanExecuteChangedForUserCommands()
        {
            SaveCommand.RaiseCanExecuteChanged();
            ChangePasswordCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            CancelEditCommand.RaiseCanExecuteChanged();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _ = LoadUserData();
            IsEditing = false;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true; 
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
             EditedUser = new UserModel();
            _loggedInUser = null;
            OldPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
            IsEditing = false;
        }

        private void ShowMessageBoxOk(string message, string title)
        {
            _dialogService.ShowDialog("CustomMessageBoxView",
                new DialogParameters
                {
            { "message", message },
            { "title", title },
            { "buttons", CustomMessageBoxButtons.Ok }
                },
                r => { });
        }
    }
}