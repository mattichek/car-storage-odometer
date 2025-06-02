using Prism.Mvvm;
using System; 

namespace car_storage_odometer.Models
{
    public class DeviceModel : BindableBase
    {
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _serialNumber;
        public string SerialNumber
        {
            get => _serialNumber;
            set => SetProperty(ref _serialNumber, value);
        }

        private string _deviceType;
        public string DeviceType
        {
            get => _deviceType;
            set => SetProperty(ref _deviceType, value);
        }

        private string _currentWarehouse;
        public string CurrentWarehouse
        {
            get => _currentWarehouse;
            set => SetProperty(ref _currentWarehouse, value);
        }

        private DateTime _entryDate;
        public DateTime EntryDate
        {
            get => _entryDate;
            set => SetProperty(ref _entryDate, value);
        }

        private string _status;
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private string _notes;
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }
    }
}