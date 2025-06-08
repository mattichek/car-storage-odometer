using car_storage_odometer.DataBaseModules; 
using car_storage_odometer.Models;
using Prism.Commands; 
using Prism.Mvvm;
using Prism.Regions; 
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace car_storage_odometer.ViewModels
{
    public class DashboardViewModel : BindableBase, INavigationAware
    {
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

        public DelegateCommand LoadDashboardDataCommand { get; private set; }


        public DashboardViewModel()
        {
            WarehouseStatuses = new ObservableCollection<WarehouseStatusModel>();
            DeviceStatuses = new ObservableCollection<DeviceStatusModel>();
            LatestUserLogs = new ObservableCollection<UserLogModel>();
            LatestDeviceLogs = new ObservableCollection<DeviceLogModel>();
            LatestRepairs = new ObservableCollection<RepairHistoryModel>();

            LoadDashboardDataCommand = new DelegateCommand(async () => await LoadDashboardData());
        }

        private async Task LoadDashboardData()
        {
            try
            {
                WarehouseStatuses.Clear();
                DeviceStatuses.Clear();
                LatestUserLogs.Clear();
                LatestDeviceLogs.Clear();
                LatestRepairs.Clear();

                WarehouseStatuses = await SqliteDataAccess<WarehouseStatusModel>.LoadQuery(SqliteQuery.LoadWarehouseStatusesQuery);
                DeviceStatuses = await SqliteDataAccess<DeviceStatusModel>.LoadQuery(SqliteQuery.LoadDeviceStatusesQuery);
                LatestUserLogs = await SqliteDataAccess<UserLogModel>.LoadQuery(SqliteQuery.LoadLatestUserLogsQuery);
                LatestDeviceLogs = await SqliteDataAccess<DeviceLogModel>.LoadQuery(SqliteQuery.LoadLatestDeviceLogsQuery);
                LatestRepairs = await SqliteDataAccess<RepairHistoryModel>.LoadQuery(SqliteQuery.LoadLatestRepairsQuery);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Błąd ładowania danych Dashboardu: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            LoadDashboardDataCommand.Execute();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true; 
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            WarehouseStatuses.Clear();
            DeviceStatuses.Clear();
            LatestUserLogs.Clear();
            LatestDeviceLogs.Clear();
            LatestRepairs.Clear();
        }
    }
}