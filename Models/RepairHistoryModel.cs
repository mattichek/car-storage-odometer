using Prism.Mvvm;
using System;

namespace car_storage_odometer.Models
{
    public class RepairHistoryModel : BindableBase
    {
        private int _repairId;
        public int RepairId // odpowiada naprawa_id
        {
            get => _repairId;
            set => SetProperty(ref _repairId, value);
        }

        private int _deviceId;
        public int DeviceId // odpowiada urzadzenie_id (Foreign Key)
        {
            get => _deviceId;
            set => SetProperty(ref _deviceId, value);
        }

        private string _description;
        public string Description // odpowiada opis
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private DateTime _startDate;
        public DateTime StartDate // odpowiada data_rozpoczecia
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private DateTime? _endDate; // Może być nullable
        public DateTime? EndDate // odpowiada data_zakonczenia
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        private int _userId;
        public int UserId // odpowiada user_id (Foreign Key do User)
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }
    }
}
