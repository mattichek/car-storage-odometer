using Prism.Mvvm;
using System;

namespace car_storage_odometer.Models
{
    public class SerialNumberModel : BindableBase // do wyjebania
    {
        private int _serialNumberId;
        public int SerialNumberId // odpowiada numer_id
        {
            get => _serialNumberId;
            set => SetProperty(ref _serialNumberId, value);
        }

        private int _deviceId;
        public int DeviceId // odpowiada urzadzenie_id (Foreign Key)
        {
            get => _deviceId;
            set => SetProperty(ref _deviceId, value);
        }

        private string _serialNumberValue;
        public string SerialNumberValue // odpowiada numer_seryjny
        {
            get => _serialNumberValue;
            set => SetProperty(ref _serialNumberValue, value);
        }

        private DateTime _issueDate;
        public DateTime IssueDate // odpowiada data_wydania
        {
            get => _issueDate;
            set => SetProperty(ref _issueDate, value);
        }
    }
}
