using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace car_storage_odometer.Models
{
    public class DeviceModel : BindableBase, INotifyPropertyChanged
    {
        private int _deviceId;
        public int DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; OnPropertyChanged(); }
        }

        private string _serialNumber;
        public string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value; OnPropertyChanged(); }
        }

        private int _typeId;
        // TypeId jest używane wewnętrznie, ale do bindowania używamy TypeName
        public int TypeId
        {
            get { return _typeId; }
            set { _typeId = value; OnPropertyChanged(); }
        }

        private string _typeName;
        public string TypeName
        {
            get { return _typeName; }
            set { _typeName = value; OnPropertyChanged(); }
        }

        private int _statusId;
        // StatusId jest używane wewnętrznie, ale do bindowania używamy StatusName
        public int StatusId
        {
            get { return _statusId; }
            set { _statusId = value; OnPropertyChanged(); }
        }

        private string _statusName;
        public string StatusName
        {
            get { return _statusName; }
            set { _statusName = value; OnPropertyChanged(); }
        }

        private int _warehouseId;
        // WarehouseId jest używane wewnętrznie, ale do bindowania używamy WarehouseName
        public int WarehouseId
        {
            get { return _warehouseId; }
            set { _warehouseId = value; OnPropertyChanged(); }
        }

        private string _warehouseName;
        public string WarehouseName
        {
            get { return _warehouseName; }
            set { _warehouseName = value; OnPropertyChanged(); }
        }

        private int _userId;
        public int UserId
        {
            get { return _userId; }
            set { _userId = value; OnPropertyChanged(); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged(); }
        }

        private string _note;
        public string Note
        {
            get { return _note; }
            set { _note = value; OnPropertyChanged(); }
        }

        private DateTime _eventDate;
        public DateTime EventDate
        {
            get { return _eventDate; }
            set { _eventDate = value; OnPropertyChanged(); }
        }

        // Implementacja interfejsu INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Metoda do tworzenia głębokiej kopii obiektu (potrzebna przy edycji)
        public DeviceModel DeepCopy()
        {
            return new DeviceModel
            {
                DeviceId = this.DeviceId,
                SerialNumber = this.SerialNumber,
                TypeId = this.TypeId,
                TypeName = this.TypeName,
                StatusId = this.StatusId,
                StatusName = this.StatusName,
                WarehouseId = this.WarehouseId,
                WarehouseName = this.WarehouseName,
                UserId = this.UserId,
                UserName = this.UserName,
                Note = this.Note,
                EventDate = this.EventDate
            };
        }
    }
}