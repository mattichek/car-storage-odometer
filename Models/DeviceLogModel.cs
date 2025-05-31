using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_storage_odometer.Models
{
    public class DeviceLogModel : BindableBase
    {
        private int _logId;
        public int LogId // odpowiada log_id
        {
            get => _logId;
            set => SetProperty(ref _logId, value);
        }

        private int _deviceId;
        public int DeviceId // odpowiada urzadzenie_id (Foreign Key)
        {
            get => _deviceId;
            set => SetProperty(ref _deviceId, value);
        }

        private DateTime _eventDate;
        public DateTime EventDate // odpowiada data_zdarzenia
        {
            get => _eventDate;
            set => SetProperty(ref _eventDate, value);
        }

        private int _fromWarehouseId;
        public int FromWarehouseId // odpowiada from_magazyn_id (Foreign Key)
        {
            get => _fromWarehouseId;
            set => SetProperty(ref _fromWarehouseId, value);
        }

        private int _toWarehouseId;
        public int ToWarehouseId // odpowiada to_magazyn_id (Foreign Key)
        {
            get => _toWarehouseId;
            set => SetProperty(ref _toWarehouseId, value);
        }

        private int _fromUserId;
        public int FromUserId // odpowiada from_user_id (Foreign Key)
        {
            get => _fromUserId;
            set => SetProperty(ref _fromUserId, value);
        }

        private int _toUserId;
        public int ToUserId // odpowiada to_user_id (Foreign Key)
        {
            get => _toUserId;
            set => SetProperty(ref _toUserId, value);
        }

        // --- Właściwości pomocnicze do wyświetlania w DataGrid ---
        private string _serialNumber;
        public string SerialNumber // Numer seryjny urządzenia
        {
            get => _serialNumber;
            set => SetProperty(ref _serialNumber, value);
        }

        private string _fromWarehouseName;
        public string FromWarehouseName // Nazwa magazynu źródłowego
        {
            get => _fromWarehouseName;
            set => SetProperty(ref _fromWarehouseName, value);
        }

        private string _toWarehouseName;
        public string ToWarehouseName // Nazwa magazynu docelowego
        {
            get => _toWarehouseName;
            set => SetProperty(ref _toWarehouseName, value);
        }

        private string _fromUserName;
        public string FromUserName // Nazwa użytkownika źródłowego
        {
            get => _fromUserName;
            set => SetProperty(ref _fromUserName, value);
        }

        private string _toUserName;

        public string ToUserName // Nazwa użytkownika docelowego
        {
            get => _toUserName;
            set => SetProperty(ref _toUserName, value);
        }

        private string _deviceName;
        public string DeviceName // Nazwa urządznia
        {
            get => _deviceName;
            set => SetProperty(ref _deviceName, value);
        }

        private string _event;
        public string Event // Nazwa co działo się z urządzeniem
        {
            get => _event;
            set => SetProperty(ref _event, value);
        }


    }
}
