using Prism.Mvvm;
using System.Collections.ObjectModel;
using car_storage_odometer.Models; // Upewnij się, że ta przestrzeń nazw jest poprawna dla Twoich modeli
using System;
using System.Linq; // Do użycia Linq do sortowania i pobierania ostatnich elementów
using Prism.Commands;
using car_storage_odometer.Helpers; 

namespace car_storage_odometer.ViewModels
{
    public class DashboardViewModel : BindableBase
    {
        // --- Właściwości do wiązania z XAML ---

        private ObservableCollection<WarehouseStatusModel> _warehouseStatuses;
        public ObservableCollection<WarehouseStatusModel> WarehouseStatuses
        {
            get => _warehouseStatuses;
            set => SetProperty(ref _warehouseStatuses, value);
        }

        private ObservableCollection<DeviceStatusModel> _deviceStatuses;
        public ObservableCollection<DeviceStatusModel> DeviceStatuses
        {
            get => _deviceStatuses;
            set => SetProperty(ref _deviceStatuses, value);
        }

        private ObservableCollection<UserLogModel> _latestUserLogs;
        public ObservableCollection<UserLogModel> LatestUserLogs
        {
            get => _latestUserLogs;
            set => SetProperty(ref _latestUserLogs, value);
        }

        private ObservableCollection<DeviceLogModel> _latestDeviceLogs;
        public ObservableCollection<DeviceLogModel> LatestDeviceLogs
        {
            get => _latestDeviceLogs;
            set => SetProperty(ref _latestDeviceLogs, value);
        }

        private ObservableCollection<RepairHistoryModel> _latestRepairs;
        public ObservableCollection<RepairHistoryModel> LatestRepairs
        {
            get => _latestRepairs;
            set => SetProperty(ref _latestRepairs, value);
        }

        // --- Komendy (opcjonalnie, jeśli będą potrzebne akcje na dashboardzie) ---
        // public DelegateCommand RefreshDataCommand { get; private set; }


        // --- Konstruktor ---
        public DashboardViewModel()
        {
            // Inicjalizacja kolekcji
            WarehouseStatuses = new ObservableCollection<WarehouseStatusModel>();
            DeviceStatuses = new ObservableCollection<DeviceStatusModel>();
            LatestUserLogs = new ObservableCollection<UserLogModel>();
            LatestDeviceLogs = new ObservableCollection<DeviceLogModel>();
            LatestRepairs = new ObservableCollection<RepairHistoryModel>();

            LoadDashboardData();
        }

        // --- Metody Prywatne ---
        private void LoadDashboardData()
        {
            // Stany magazynowe
            WarehouseStatuses.Clear();
            WarehouseStatuses = SqliteDataAccess<WarehouseStatusModel>.LoadQuery("select * from Warehouses");

          
            // Statusy i ich ilość
            DeviceStatuses.Clear();
            DeviceStatuses = SqliteDataAccess<DeviceStatusModel>.LoadQuery(
                @"SELECT 
                    s.Name,
                COUNT(d.DeviceId) AS Quantity
                FROM Devices d
                JOIN Statuses s ON d.StatusId = s.StatusId GROUP BY s.Name;");

            // Ostatnie logi użytkowników (5 ostatnich)
            LatestUserLogs.Clear();
            LatestUserLogs = new ObservableCollection<UserLogModel>(SqliteDataAccess<UserLogModel>.LoadQuery(
                @"SELECT 
                    l.LogId,
                    u.FirstName || ' ' || u.LastName AS UserName,
                    d.DeviceId, 
                    l.Event, 
                    l.EventDate
                FROM UserLogs l
                LEFT JOIN Users u ON l.UserId = u.UserId
                LEFT JOIN Devices d ON l.DeviceId = d.DeviceId;").OrderByDescending(l => l.EventDate).Take(5));


            // Symulacja ostatnich logów urządzeń (5 ostatnich)
            // Pamiętaj, DeviceLogModel powinien zawierać nazwy urządzeń itp.
            LatestDeviceLogs.Clear();
            LatestDeviceLogs = SqliteDataAccess.LoadDevicesLogsLastFiveLogs();

            // Ostatnie naprawy (5 ostatnich)
            LatestRepairs.Clear();
            LatestRepairs = new ObservableCollection<RepairHistoryModel>(SqliteDataAccess<RepairHistoryModel>.LoadQuery(
                @"SELECT 
                    rh.RepairId,
                    dt.Name AS DeviceName,
                    rh.Description,
                    rh.StartDate,
                    rh.EndDate,
                    u.FirstName || ' ' || u.LastName AS UserName
                FROM repairhistory rh
                LEFT JOIN devices d ON rh.DeviceId = d.DeviceId
                LEFT JOIN devicetypes dt ON d.TypeId = dt.TypeId
                LEFT JOIN users u ON rh.UserId = u.UserId;").OrderByDescending(r => r.StartDate).Take(5));

        }
    }
}