using car_storage_odometer.Models; // Zakładam, że LogsDeviceModel będzie w Models
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace car_storage_odometer.ViewModels
{
    public class LogsDeviceViewModel : BindableBase
    {
        private ObservableCollection<DeviceLogModel> _allDeviceLogs;
        private ObservableCollection<DeviceLogModel> _latestDeviceLogs;
        public ObservableCollection<DeviceLogModel> LatestDeviceLogs
        {
            get => _latestDeviceLogs;
            set => SetProperty(ref _latestDeviceLogs, value);
        }

        // Filter properties for LogsDeviceView
        private DateTime? _filterDateFrom;
        public DateTime? FilterDateFrom
        {
            get => _filterDateFrom;
            set { SetProperty(ref _filterDateFrom, value); }
        }

        private DateTime? _filterDateTo;
        public DateTime? FilterDateTo
        {
            get => _filterDateTo;
            set { SetProperty(ref _filterDateTo, value); }
        }

        private string _selectedSerialNumberFilter;
        public string SelectedSerialNumberFilter
        {
            get => _selectedSerialNumberFilter;
            set { SetProperty(ref _selectedSerialNumberFilter, value); }
        }
        private ObservableCollection<string> _availableSerialNumbers;
        public ObservableCollection<string> AvailableSerialNumbers
        {
            get => _availableSerialNumbers;
            set => SetProperty(ref _availableSerialNumbers, value);
        }

        private string _selectedEventFilter;
        public string SelectedEventFilter
        {
            get => _selectedEventFilter;
            set { SetProperty(ref _selectedEventFilter, value); }
        }
        private ObservableCollection<string> _availableEvents;
        public ObservableCollection<string> AvailableEvents
        {
            get => _availableEvents;
            set => SetProperty(ref _availableEvents, value);
        }

        private string _selectedFromWarehouseFilter;
        public string SelectedFromWarehouseFilter
        {
            get => _selectedFromWarehouseFilter;
            set { SetProperty(ref _selectedFromWarehouseFilter, value); }
        }
        private ObservableCollection<string> _availableFromWarehouses;
        public ObservableCollection<string> AvailableFromWarehouses
        {
            get => _availableFromWarehouses;
            set => SetProperty(ref _availableFromWarehouses, value);
        }

        private string _selectedToWarehouseFilter;
        public string SelectedToWarehouseFilter
        {
            get => _selectedToWarehouseFilter;
            set { SetProperty(ref _selectedToWarehouseFilter, value); }
        }
        private ObservableCollection<string> _availableToWarehouses;
        public ObservableCollection<string> AvailableToWarehouses
        {
            get => _availableToWarehouses;
            set => SetProperty(ref _availableToWarehouses, value);
        }

        private string _selectedUserFilter;
        public string SelectedUserFilter
        {
            get => _selectedUserFilter;
            set { SetProperty(ref _selectedUserFilter, value); }
        }
        private ObservableCollection<string> _availableUsers;
        public ObservableCollection<string> AvailableUsers
        {
            get => _availableUsers;
            set => SetProperty(ref _availableUsers, value);
        }

        // Commands
        public DelegateCommand FilterCommand { get; private set; }
        public DelegateCommand ResetFiltersCommand { get; private set; }

        public LogsDeviceViewModel()
        {
            LoadDummyData(); // Load initial data
            InitializeFilterOptions(); // Populate filter dropdowns

            // Initially, display all device logs, sorted by date
            LatestDeviceLogs = new ObservableCollection<DeviceLogModel>(_allDeviceLogs.OrderByDescending(log => log.EventDate));

            FilterCommand = new DelegateCommand(ApplyFilters);
            ResetFiltersCommand = new DelegateCommand(ResetAllFilters);
        }

        private void LoadDummyData()
        {
            // Sample data for device logs
            _allDeviceLogs = new ObservableCollection<DeviceLogModel>
            {
                new DeviceLogModel { LogId = 1, EventDate = new DateTime(2024, 5, 20, 10, 0, 0), SerialNumber = "SN001", Event = "Przyjęcie", FromWarehouseName = "Dostawca", ToWarehouseName = "Magazyn Główny", FromUserName = "Admin" },
                new DeviceLogModel { LogId = 2, EventDate = new DateTime(2024, 5, 21, 11, 30, 0), SerialNumber = "SN002", Event = "Wydanie", FromWarehouseName = "Magazyn Główny", ToWarehouseName = "Serwis", FromUserName = "Jan Kowalski" },
                new DeviceLogModel { LogId = 3, EventDate = new DateTime(2024, 5, 22, 14, 0, 0), SerialNumber = "SN001", Event = "Przesunięcie", FromWarehouseName = "Magazyn Główny", ToWarehouseName = "Magazyn Awaryjny", FromUserName = "Admin" },
                new DeviceLogModel { LogId = 4, EventDate = new DateTime(2024, 5, 23, 9, 15, 0), SerialNumber = "SN003", Event = "Przyjęcie", FromWarehouseName = "Dostawca", ToWarehouseName = "Magazyn Główny", FromUserName = "Anna Nowak" },
                new DeviceLogModel { LogId = 5, EventDate = new DateTime(2024, 5, 24, 16, 45, 0), SerialNumber = "SN002", Event = "Zwrot", FromWarehouseName = "Serwis", ToWarehouseName = "Magazyn Główny", FromUserName = "Piotr Zieliński" }
            };
        }

        private void InitializeFilterOptions()
        {
            AvailableSerialNumbers = new ObservableCollection<string>(
                new[] { "Wszystkie" }.Concat(_allDeviceLogs.Select(log => log.SerialNumber).Where(s => s != null).Distinct().OrderBy(s => s)));
            SelectedSerialNumberFilter = "Wszystkie";

            AvailableEvents = new ObservableCollection<string>(
                new[] { "Wszystkie" }.Concat(_allDeviceLogs.Select(log => log.Event).Where(e => e != null).Distinct().OrderBy(e => e)));
            SelectedEventFilter = "Wszystkie";

            AvailableFromWarehouses = new ObservableCollection<string>(
                new[] { "Wszystkie" }.Concat(_allDeviceLogs.Select(log => log.FromWarehouseName).Where(w => w != null).Distinct().OrderBy(w => w)));
            SelectedFromWarehouseFilter = "Wszystkie";

            AvailableToWarehouses = new ObservableCollection<string>(
                new[] { "Wszystkie" }.Concat(_allDeviceLogs.Select(log => log.ToWarehouseName).Where(w => w != null).Distinct().OrderBy(w => w)));
            SelectedToWarehouseFilter = "Wszystkie";

            AvailableUsers = new ObservableCollection<string>(
                new[] { "Wszyscy" }.Concat(_allDeviceLogs.Select(log => log.FromUserName).Where(u => u != null).Distinct().OrderBy(u => u)));
            SelectedUserFilter = "Wszyscy";
        }

        private void ApplyFilters()
        {
            IEnumerable<DeviceLogModel> filteredData = _allDeviceLogs;

            if (FilterDateFrom.HasValue)
                filteredData = filteredData.Where(log => log.EventDate.Date >= FilterDateFrom.Value.Date);

            if (FilterDateTo.HasValue)
                filteredData = filteredData.Where(log => log.EventDate.Date <= FilterDateTo.Value.Date);

            if (SelectedSerialNumberFilter != null && SelectedSerialNumberFilter != "Wszystkie")
                filteredData = filteredData.Where(log => log.SerialNumber == SelectedSerialNumberFilter);

            if (SelectedEventFilter != null && SelectedEventFilter != "Wszystkie")
                filteredData = filteredData.Where(log => log.Event == SelectedEventFilter);

            if (SelectedFromWarehouseFilter != null && SelectedFromWarehouseFilter != "Wszystkie")
                filteredData = filteredData.Where(log => log.FromWarehouseName == SelectedFromWarehouseFilter);

            if (SelectedToWarehouseFilter != null && SelectedToWarehouseFilter != "Wszystkie")
                filteredData = filteredData.Where(log => log.ToWarehouseName == SelectedToWarehouseFilter);

            if (SelectedUserFilter != null && SelectedUserFilter != "Wszyscy")
                filteredData = filteredData.Where(log => log.FromUserName == SelectedUserFilter);

            LatestDeviceLogs = new ObservableCollection<DeviceLogModel>(filteredData.OrderByDescending(log => log.EventDate));
        }

        private void ResetAllFilters()
        {
            FilterDateFrom = null;
            FilterDateTo = null;
            SelectedSerialNumberFilter = "Wszystkie";
            SelectedEventFilter = "Wszystkie";
            SelectedFromWarehouseFilter = "Wszystkie";
            SelectedToWarehouseFilter = "Wszystkie";
            SelectedUserFilter = "Wszyscy";

            LatestDeviceLogs = new ObservableCollection<DeviceLogModel>(_allDeviceLogs.OrderByDescending(log => log.EventDate));
        }
    }
}