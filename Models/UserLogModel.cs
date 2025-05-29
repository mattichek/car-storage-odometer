using Prism.Mvvm;
using System;

namespace car_storage_odometer.Models
{
    public class UserLogModel : BindableBase
    {
        private int _userLogId;
        public int UserLogId // odpowiada log_id z logi_uzytkownikow
        {
            get => _userLogId;
            set => SetProperty(ref _userLogId, value);
        }

        private int _userId;
        public int UserId // odpowiada user_id (Foreign Key do User)
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        private string _action;
        public string Action // odpowiada akcja
        {
            get => _action;
            set => SetProperty(ref _action, value);
        }

        private DateTime _eventDate;
        public DateTime EventDate // odpowiada data_zdarzenia
        {
            get => _eventDate;
            set => SetProperty(ref _eventDate, value);
        }

        // Opcjonalnie: właściwość pomocnicza, jeśli potrzebujesz nazwy użytkownika w logu
        private string _userName;
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }
    }
}
