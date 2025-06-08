// CurrentUserService.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace car_storage_odometer.Services
{
    public class CurrentUserService : ICurrentUserService, INotifyPropertyChanged
    {
        private int? _loggedInUserId;
        public int? LoggedInUserId 
        {
            get => _loggedInUserId;
            private set
            {
                if (_loggedInUserId != value)
                {
                    _loggedInUserId = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsUserLoggedIn));
                }
            }
        }

        public bool IsUserLoggedIn => LoggedInUserId.HasValue;

        public void SetLoggedInUserId(int userId)
        {
            LoggedInUserId = userId;
        }

        public void ClearLoggedInUser()
        {
            LoggedInUserId = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}