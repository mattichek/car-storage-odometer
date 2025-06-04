using Prism.Mvvm;
using System;

namespace car_storage_odometer.Models
{
    public class StorageDeviceModel : BindableBase // do wyjebania
    {
        private int _deviceId;
        public int DeviceId // odpowiada urzadzenie_id
        {
            get => _deviceId;
            set => SetProperty(ref _deviceId, value);
        }

        private int _typeId;
        public int TypeId // odpowiada typ_id
        {
            get => _typeId;
            set => SetProperty(ref _typeId, value);
        }

        private int _statusId;
        public int StatusId // odpowiada status_id
        {
            get => _statusId;
            set => SetProperty(ref _statusId, value);
        }

        private int? _warehouseId; // Może być nullable, jeśli urządzenie może być poza magazynem
        public int? WarehouseId // odpowiada magazyn_id
        {
            get => _warehouseId;
            set => SetProperty(ref _warehouseId, value);
        }

        private int? _userId; // Może być nullable, kto dodał/jest przypisany
        public int? UserId // odpowiada user_id
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        // --- Właściwości pomocnicze do wyświetlania w DataGrid ---
        private string _serialNumber;
        public string SerialNumber // Numer seryjny urządzenia (z tabeli numery_seryjne)
        {
            get => _serialNumber;
            set => SetProperty(ref _serialNumber, value);
        }

        private string _typeName;
        public string TypeName // Nazwa typu (z tabeli typy_urzadzen)
        {
            get => _typeName;
            set => SetProperty(ref _typeName, value);
        }

        private string _statusName;
        public string StatusName // Nazwa statusu (z tabeli statusy)
        {
            get => _statusName;
            set => SetProperty(ref _statusName, value);
        }

        private string _warehouseName;
        public string WarehouseName // Nazwa magazynu (z tabeli magazyn)
        {
            get => _warehouseName;
            set => SetProperty(ref _warehouseName, value);
        }

        private string _addedByUserName;

        public string AddedByUserName // Nazwa użytkownika (z tabeli uzytkownicy, kto przypisany/dodał)
        {
            get => _addedByUserName;
            set => SetProperty(ref _addedByUserName, value);
        }

        private DateTime _dateAdded;
        public DateTime DateAdded
        {
            get => _dateAdded;
            set => SetProperty(ref _dateAdded, value);
        }

    }
}
