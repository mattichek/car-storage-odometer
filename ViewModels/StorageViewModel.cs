using car_storage_odometer.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace car_storage_odometer.ViewModels
{
    public class StorageViewModel : BindableBase
    {
        private ObservableCollection<StorageDeviceModel> _allWarehouseItems;
        private ObservableCollection<StorageDeviceModel> _latestWarehouseItems;
        public ObservableCollection<StorageDeviceModel> LatestWarehouseItems
        {
            get => _latestWarehouseItems;
            set => SetProperty(ref _latestWarehouseItems, value);
        }

        // --- Filtry ---
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

        private string _selectedDeviceTypeFilter;
        public string SelectedDeviceTypeFilter
        {
            get => _selectedDeviceTypeFilter;
            set => SetProperty(ref _selectedDeviceTypeFilter, value);
        }
        private ObservableCollection<string> _availableDeviceTypes;
        public ObservableCollection<string> AvailableDeviceTypes
        {
            get => _availableDeviceTypes;
            set => SetProperty(ref _availableDeviceTypes, value);
        }

        private string _selectedStatusFilter;
        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set => SetProperty(ref _selectedStatusFilter, value);
        }
        private ObservableCollection<string> _availableStatuses;
        public ObservableCollection<string> AvailableStatuses
        {
            get => _availableStatuses;
            set => SetProperty(ref _availableStatuses, value);
        }

        private string _selectedWarehouseFilter;
        public string SelectedWarehouseFilter
        {
            get => _selectedWarehouseFilter;
            set => SetProperty(ref _selectedWarehouseFilter, value);
        }
        private ObservableCollection<string> _availableWarehouses;
        public ObservableCollection<string> AvailableWarehouses
        {
            get => _availableWarehouses;
            set => SetProperty(ref _availableWarehouses, value);
        }

        private string _selectedAddedByUserFilter;
        public string SelectedAddedByUserFilter
        {
            get => _selectedAddedByUserFilter;
            set => SetProperty(ref _selectedAddedByUserFilter, value);
        }
        private ObservableCollection<string> _availableAddedByUsers;
        public ObservableCollection<string> AvailableAddedByUsers
        {
            get => _availableAddedByUsers;
            set => SetProperty(ref _availableAddedByUsers, value);
        }

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

        // --- Komendy ---
        public DelegateCommand FilterCommand { get; private set; }
        public DelegateCommand ResetFiltersCommand { get; private set; }

        public StorageViewModel()
        {
            LoadDummyData();
            InitializeFilterOptions();

            LatestWarehouseItems = new ObservableCollection<StorageDeviceModel>(_allWarehouseItems.OrderByDescending(item => item.DateAdded));

            FilterCommand = new DelegateCommand(ApplyFilters);
            ResetFiltersCommand = new DelegateCommand(ResetAllFilters);
        }

        private void LoadDummyData()
        {
            _allWarehouseItems = new ObservableCollection<StorageDeviceModel>
            {
                new StorageDeviceModel { DeviceId = 1, SerialNumber = "SN-001", TypeName = "Licznik", StatusName = "Dostępny", WarehouseName = "Magazyn A", AddedByUserName = "Jan Kowalski", DateAdded = new DateTime(2024, 1, 10)},
                new StorageDeviceModel { DeviceId = 2, SerialNumber = "SN-002", TypeName = "Tracker", StatusName = "W użyciu", WarehouseName = "Magazyn A", AddedByUserName = "Anna Nowak", DateAdded = new DateTime(2024, 1, 15)},
                new StorageDeviceModel { DeviceId = 3, SerialNumber = "SN-003", TypeName = "Licznik", StatusName = "Uszkodzony", WarehouseName = "Serwis", AddedByUserName = "Jan Kowalski", DateAdded = new DateTime(2024, 2, 1)},
                new StorageDeviceModel { DeviceId = 4, SerialNumber = "SN-004", TypeName = "Licznik", StatusName = "Dostępny", WarehouseName = "Magazyn B", AddedByUserName = "Piotr Zieliński", DateAdded = new DateTime(2024, 2, 20)},
                new StorageDeviceModel { DeviceId = 5, SerialNumber = "SN-005", TypeName = "Tracker", StatusName = "Dostępny", WarehouseName = "Magazyn A", AddedByUserName = "Anna Nowak", DateAdded = new DateTime(2024, 3, 5)},
                new StorageDeviceModel { DeviceId = 6, SerialNumber = "SN-001", TypeName = "Licznik", StatusName = "W naprawie", WarehouseName = "Serwis", AddedByUserName = "Jan Kowalski", DateAdded = new DateTime(2024, 3, 10)}, // SN-001 ponownie, ale inny status/data
            };
        }

        private void InitializeFilterOptions()
        {
            // Dla ComboBoxów dodajemy opcję "Wszystkie" / "Wszyscy"
            AvailableSerialNumbers = new ObservableCollection<string>(
                new[] { "Wszystkie" }.Concat(_allWarehouseItems.Select(item => item.SerialNumber).Distinct().OrderBy(s => s)));
            SelectedSerialNumberFilter = "Wszystkie";

            AvailableDeviceTypes = new ObservableCollection<string>(
                new[] { "Wszystkie" }.Concat(_allWarehouseItems.Select(item => item.TypeName).Distinct().OrderBy(t => t)));
            SelectedDeviceTypeFilter = "Wszystkie";

            AvailableStatuses = new ObservableCollection<string>(
                new[] { "Wszystkie" }.Concat(_allWarehouseItems.Select(item => item.StatusName).Distinct().OrderBy(s => s)));
            SelectedStatusFilter = "Wszystkie";

            AvailableWarehouses = new ObservableCollection<string>(
                new[] { "Wszystkie" }.Concat(_allWarehouseItems.Select(item => item.WarehouseName).Distinct().OrderBy(w => w)));
            SelectedWarehouseFilter = "Wszystkie";

            AvailableAddedByUsers = new ObservableCollection<string>(
                new[] { "Wszyscy" }.Concat(_allWarehouseItems.Select(item => item.AddedByUserName).Distinct().OrderBy(u => u)));
            SelectedAddedByUserFilter = "Wszyscy";

            FilterDateFrom = null;
            FilterDateTo = null;
        }

        private void ApplyFilters()
        {
            IEnumerable<StorageDeviceModel> filteredData = _allWarehouseItems;

            if (SelectedSerialNumberFilter != "Wszystkie" && !string.IsNullOrEmpty(SelectedSerialNumberFilter))
                filteredData = filteredData.Where(item => item.SerialNumber == SelectedSerialNumberFilter);

            if (SelectedDeviceTypeFilter != "Wszystkie" && !string.IsNullOrEmpty(SelectedDeviceTypeFilter))
                filteredData = filteredData.Where(item => item.TypeName == SelectedDeviceTypeFilter);

            if (SelectedStatusFilter != "Wszystkie" && !string.IsNullOrEmpty(SelectedStatusFilter))
                filteredData = filteredData.Where(item => item.StatusName == SelectedStatusFilter);

            if (SelectedWarehouseFilter != "Wszystkie" && !string.IsNullOrEmpty(SelectedWarehouseFilter))
                filteredData = filteredData.Where(item => item.WarehouseName == SelectedWarehouseFilter);

            if (SelectedAddedByUserFilter != "Wszyscy" && !string.IsNullOrEmpty(SelectedAddedByUserFilter))
                filteredData = filteredData.Where(item => item.AddedByUserName == SelectedAddedByUserFilter);

            if (FilterDateFrom.HasValue)
                filteredData = filteredData.Where(item => item.DateAdded.Date >= FilterDateFrom.Value.Date);

            if (FilterDateTo.HasValue)
                filteredData = filteredData.Where(item => item.DateAdded.Date <= FilterDateTo.Value.Date);

            LatestWarehouseItems = new ObservableCollection<StorageDeviceModel>(filteredData.OrderByDescending(item => item.DateAdded));
        }

        private void ResetAllFilters()
        {
            SelectedSerialNumberFilter = "Wszystkie";
            SelectedDeviceTypeFilter = "Wszystkie";
            SelectedStatusFilter = "Wszystkie";
            SelectedWarehouseFilter = "Wszystkie";
            SelectedAddedByUserFilter = "Wszyscy";
            FilterDateFrom = null;
            FilterDateTo = null;

            LatestWarehouseItems = new ObservableCollection<StorageDeviceModel>(_allWarehouseItems.OrderByDescending(item => item.DateAdded));
        }
    }
}