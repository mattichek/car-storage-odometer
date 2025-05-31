using car_storage_odometer.Models;
using car_storage_odometer.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
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

        // Filtry daty
        private DateTime? _filterDateFrom;
        public DateTime? FilterDateFrom
        {
            get => _filterDateFrom;
            set => SetProperty(ref _filterDateFrom, value);
        }

        private DateTime? _filterDateTo;
        public DateTime? FilterDateTo
        {
            get => _filterDateTo;
            set => SetProperty(ref _filterDateTo, value);
        }

        // Filtry tekstowe (ComboBox)
        private string _selectedSerialNumberFilter;
        public string SelectedSerialNumberFilter
        {
            get => _selectedSerialNumberFilter;
            set => SetProperty(ref _selectedSerialNumberFilter, value);
        }
        private ObservableCollection<string> _availableSerialNumbers;
        public ObservableCollection<string> AvailableSerialNumbers
        {
            get => _availableSerialNumbers;
            set => SetProperty(ref _availableSerialNumbers, value);
        }

        private string _selectedFromWarehouseFilter;
        public string SelectedFromWarehouseFilter
        {
            get => _selectedFromWarehouseFilter;
            set => SetProperty(ref _selectedFromWarehouseFilter, value);
        }
        private ObservableCollection<string> _availableWarehouses;
        public ObservableCollection<string> AvailableWarehouses
        {
            get => _availableWarehouses;
            set => SetProperty(ref _availableWarehouses, value);
        }

        private string _selectedToWarehouseFilter;
        public string SelectedToWarehouseFilter
        {
            get => _selectedToWarehouseFilter;
            set => SetProperty(ref _selectedToWarehouseFilter, value);
        }

        private string _selectedFromUserFilter;
        public string SelectedFromUserFilter
        {
            get => _selectedFromUserFilter;
            set => SetProperty(ref _selectedFromUserFilter, value);
        }
        private ObservableCollection<string> _availableUsers;
        public ObservableCollection<string> AvailableUsers
        {
            get => _availableUsers;
            set => SetProperty(ref _availableUsers, value);
        }

        private string _selectedToUserFilter;
        public string SelectedToUserFilter
        {
            get => _selectedToUserFilter;
            set => SetProperty(ref _selectedToUserFilter, value);
        }

        // Komendy
        public DelegateCommand FilterLogsCommand { get; private set; }
        public DelegateCommand ResetFiltersCommand { get; private set; }

        private readonly DeviceLogFilterService _filterService;

        public LogsDeviceViewModel()
        {
            _filterService = new DeviceLogFilterService();

            LoadDummyData();
            InitializeFilterOptions();

            LatestDeviceLogs = _filterService.ResetFilters(_allDeviceLogs);

            FilterLogsCommand = new DelegateCommand(ApplyFilter);
            ResetFiltersCommand = new DelegateCommand(ResetFilters);
        }

        private void LoadDummyData()
        {
            _allDeviceLogs = new ObservableCollection<DeviceLogModel>
            {
                new DeviceLogModel { LogId = 1, SerialNumber = "SN-001", Event = "Przyjęcie na magazyn", FromWarehouseName = "Dostawca", ToWarehouseName = "Magazyn A", FromUserName = "Jan Kowalski", EventDate = new DateTime(2025, 05, 31, 10, 0, 0) },
                new DeviceLogModel { LogId = 2, SerialNumber = "SN-002", Event = "Wydanie z magazynu", FromWarehouseName = "Magazyn A", ToWarehouseName = "Klient X", FromUserName = "Anna Nowak", EventDate = new DateTime(2025, 05, 31, 11, 30, 0) },
                new DeviceLogModel { LogId = 3, SerialNumber = "SN-001", Event = "Przeniesienie między magazynami", FromWarehouseName = "Magazyn A", ToWarehouseName = "Magazyn B", FromUserName = "Piotr Zieliński", EventDate = new DateTime(2025, 05, 30, 09, 0, 0) },
                new DeviceLogModel { LogId = 4, SerialNumber = "SN-003", Event = "Przyjęcie na magazyn", FromWarehouseName = "Dostawca", ToWarehouseName = "Magazyn B", FromUserName = "Jan Kowalski", EventDate = new DateTime(2025, 05, 29, 14, 0, 0) },
                new DeviceLogModel { LogId = 5, SerialNumber = "SN-002", Event = "Zwrot", FromWarehouseName = "Klient X", ToWarehouseName = "Magazyn A", FromUserName = "Anna Nowak", EventDate = new DateTime(2025, 05, 28, 16, 0, 0) }
            };
        }

        private void InitializeFilterOptions()
        {
            // Dodaj "Wszystkie" / "Wszyscy" jako pierwszą opcję
            AvailableSerialNumbers = new ObservableCollection<string>(
                new[] { "Wszystkie" }.Concat(_allDeviceLogs.Select(log => log.SerialNumber).Distinct().OrderBy(s => s)));

            AvailableWarehouses = new ObservableCollection<string>(
                new[] { "Wszystkie" }.Concat(_allDeviceLogs.Select(log => log.FromWarehouseName).Distinct().OrderBy(w => w))
                                     .Concat(_allDeviceLogs.Select(log => log.ToWarehouseName).Distinct().OrderBy(w => w))
                                     .Distinct()); // Upewnij się, że są unikalne

            AvailableUsers = new ObservableCollection<string>(
                new[] { "Wszyscy" }.Concat(_allDeviceLogs.Select(log => log.FromUserName).Distinct().OrderBy(u => u)));

            // Domyślne wartości dla filtrów
            SelectedSerialNumberFilter = "Wszystkie";
            SelectedFromWarehouseFilter = "Wszystkie";
            SelectedToWarehouseFilter = "Wszystkie";
            SelectedFromUserFilter = "Wszyscy";
            SelectedToUserFilter = "Wszyscy";
        }

        private void ApplyFilter()
        {
            LatestDeviceLogs = _filterService.ApplyFilter(
                _allDeviceLogs,
                FilterDateFrom,
                FilterDateTo,
                SelectedSerialNumberFilter,
                SelectedFromWarehouseFilter,
                SelectedToWarehouseFilter,
                SelectedFromUserFilter,
                SelectedToUserFilter
            );
        }

        private void ResetFilters()
        {
            FilterDateFrom = null;
            FilterDateTo = null;
            SelectedSerialNumberFilter = "Wszystkie";
            SelectedFromWarehouseFilter = "Wszystkie";
            SelectedToWarehouseFilter = "Wszystkie";
            SelectedFromUserFilter = "Wszyscy";
            SelectedToUserFilter = "Wszyscy";

            LatestDeviceLogs = _filterService.ResetFilters(_allDeviceLogs);
        }
    }
}