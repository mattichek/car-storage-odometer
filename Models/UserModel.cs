using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace car_storage_odometer.Models
{
    public class UserModel : BindableBase, INotifyPropertyChanged
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
        public string Password // odpowiada haslo (UWAGA: powinno być haszowane!)
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

        // Metoda do tworzenia głębokiej kopii obiektu UserModel
        public UserModel Clone()
        {
            return (UserModel)this.MemberwiseClone();
        }

        // Implementacja INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}