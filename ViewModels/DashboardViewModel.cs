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

            // W prawdziwej aplikacji: pobierz dane z serwisu/repozytorium
            // Na razie symulacja danych
            LoadDashboardData();
        }

        // --- Metody Prywatne ---
        private void LoadDashboardData()
        {
            // Symulacja danych stanów magazynowych
            // W prawdziwej aplikacji: zapytanie do bazy danych z agregacją
            WarehouseStatuses.Clear();
            WarehouseStatuses = SqliteDataAccess.LoadWarehouses();

          
            // W prawdziwej aplikacji: zapytanie do bazy danych z agregacją
            DeviceStatuses.Clear();
            DeviceStatuses = SqliteDataAccess.LoadStatusesQuantity();

            // Symulacja ostatnich logów użytkowników (5 ostatnich)
            // W prawdziwej aplikacji: zapytanie do bazy danych z ORDER BY i LIMIT
            LatestUserLogs.Clear();
            LatestUserLogs = new ObservableCollection<UserLogModel>(SqliteDataAccess.LoadUserLogs().OrderByDescending(l => l.EventDate).Take(5));


            // Symulacja ostatnich logów urządzeń (5 ostatnich)
            // Pamiętaj, DeviceLogModel powinien zawierać nazwy urządzeń itp.
            LatestDeviceLogs.Clear();
            LatestDeviceLogs.Add(new DeviceLogModel { LogId = 5, DeviceId = 101, DeviceName = "Skaner QR-01", Event = "Błąd odczytu", EventDate = new DateTime(2025, 5, 31, 9, 25, 0) });
            LatestDeviceLogs.Add(new DeviceLogModel { LogId = 4, DeviceId = 102, DeviceName = "Drukarka etykiet", Event = "Wydruk etykiet", EventDate = new DateTime(2025, 5, 31, 8, 25, 0) });
            LatestDeviceLogs.Add(new DeviceLogModel { LogId = 3, DeviceId = 103, DeviceName = "Czytnik RFID", Event = "Odczyt zakończony", EventDate = new DateTime(2025, 5, 31, 7, 25, 0) });
            LatestDeviceLogs.Add(new DeviceLogModel { LogId = 2, DeviceId = 101, DeviceName = "Skaner QR-01", Event = "Aktualizacja firmware", EventDate = new DateTime(2025, 5, 31, 6, 25, 0) });
            LatestDeviceLogs.Add(new DeviceLogModel { LogId = 1, DeviceId = 102, DeviceName = "Drukarka etykiet", Event = "Brak papieru", EventDate = new DateTime(2025, 5, 31, 5, 25, 0) });
            LatestDeviceLogs = new ObservableCollection<DeviceLogModel>(LatestDeviceLogs.OrderByDescending(l => l.EventDate).Take(5));


            // Symulacja ostatnich napraw (5 ostatnich)
            // Pamiętaj, RepairHistoryModel powinien zawierać nazwy elementów itp.
            LatestRepairs.Clear();
            LatestRepairs = new ObservableCollection<RepairHistoryModel>(SqliteDataAccess.LoadRepairHistory().OrderByDescending(r => r.StartDate).Take(5));
            //LatestRepairs.Add(new RepairHistoryModel { RepairId = 5, DeviceId = 201, DeviceName = "Licznik-005", Description = "Wymiana baterii", StartDate = new DateTime(2025, 5, 30), UserId = 1, UserName = "Jan Kowalski" });
            //LatestRepairs.Add(new RepairHistoryModel { RepairId = 4, DeviceId = 202, DeviceName = "Licznik-002", Description = "Czyszczenie styków", StartDate = new DateTime(2025, 5, 28), UserId = 2, UserName = "Anna Nowak" });
            //LatestRepairs.Add(new RepairHistoryModel { RepairId = 3, DeviceId = 203, DeviceName = "Licznik-010", Description = "Naprawa obudowy", StartDate = new DateTime(2025, 5, 24), UserId = 1, UserName = "Jan Kowalski" });
            //LatestRepairs.Add(new RepairHistoryModel { RepairId = 2, DeviceId = 204, DeviceName = "Moduł-ABC", Description = "Kalibracja", StartDate = new DateTime(2025, 5, 21), UserId = 3, UserName = "Piotr Zieliński" });
            //LatestRepairs.Add(new RepairHistoryModel { RepairId = 1, DeviceId = 205, DeviceName = "Licznik-002", Description = "Wymiana wyświetlacza", StartDate = new DateTime(2025, 5, 17), UserId = 2, UserName = "Anna Nowak" });
            //LatestRepairs = new ObservableCollection<RepairHistoryModel>(LatestRepairs.OrderByDescending(r => r.StartDate).Take(5));
        }
    }
}