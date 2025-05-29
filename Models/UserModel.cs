using Prism.Mvvm;
using System;

namespace car_storage_odometer.Models
{
    public class UserModel : BindableBase
    {
        private int _userId;
        public int UserId // odpowiada user_id
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        private string _firstName;
        public string FirstName // odpowiada imie
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private string _lastName;
        public string LastName // odpowiada nazwisko
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        private string _email;
        public string Email // odpowiada email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password;
        public string Password // odpowiada haslo
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private DateTime _registrationDate;
        public DateTime RegistrationDate // odpowiada data_rejestracji
        {
            get => _registrationDate;
            set => SetProperty(ref _registrationDate, value);
        }

        private bool _isActive;
        public bool IsActive // odpowiada czy_aktywny
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        private string _role;
        public string Role // odpowiada rola
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }

        // Pomocnicza właściwość do wyświetlania pełnej nazwy użytkownika
        public string UserName => $"{FirstName} {LastName}";
        public UserModel Clone()
        {
            return new UserModel
            {
                UserId = this.UserId,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email,
                Password = this.Password, // Kluczowe: NIE klonuj hasła w ten sposób w prod!
                RegistrationDate = this.RegistrationDate,
                IsActive = this.IsActive,
                Role = this.Role
            };
        }
    }
}
