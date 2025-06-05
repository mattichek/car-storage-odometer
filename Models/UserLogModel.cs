using Prism.Mvvm;
using System;

namespace car_storage_odometer.Models
{
    public class UserLogModel : BindableBase
    {
        private int _logId;
        public int LogId // odpowiada log_id z logi_uzytkownikow
        {
            get => _logId;
            set => SetProperty(ref _logId, value);
        }

        private int _userId;
        public int UserId // odpowiada user_id (Foreign Key do User)
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private string _event;
        public string Event // odpowiada akcja
        {
            get => _event;
            set => SetProperty(ref _event, value);
        }

        private DateTime _eventDate;
        public DateTime EventDate // odpowiada data_zdarzenia
        {
            get => _eventDate;
            set => SetProperty(ref _eventDate, value);
        }

        private string _serialNumber;
        public string SerialNumber // Numer seryjny urządzenia
        {
            get => _serialNumber;
            set => SetProperty(ref _serialNumber, value);
        }
    }
}
