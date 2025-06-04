using car_storage_odometer.Helpers;
using car_storage_odometer.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace car_storage_odometer.ViewModels
{
    public class LogsDeviceViewModel : BindableBase, INavigationAware
    {
        public LogsDeviceViewModel()
        {
            LoadDevicesLogsCommand = new DelegateCommand(async () => await LoadDevicesLogsAsync());
            ResetFiltersCommand = new DelegateCommand(ResetAllFiltersAndApply);

            AllDeviceLogs = new ObservableCollection<DeviceLogModel>();
            LatestDeviceLogs = new ObservableCollection<DeviceLogModel>();

            AvailableEvents = new ObservableCollection<string>();
            AvailableFromWarehouses = new ObservableCollection<string>();
            AvailableToWarehouses = new ObservableCollection<string>();
            AvailableUsers = new ObservableCollection<string>();

            ResetAllFiltersAndApply();
        }

        public DelegateCommand LoadDevicesLogsCommand { get; private set; }
        public DelegateCommand ResetFiltersCommand { get; private set; }

        private ObservableCollection<DeviceLogModel> _allDeviceLogs;
        public ObservableCollection<DeviceLogModel> AllDeviceLogs
        {
            get => _allDeviceLogs;
            set => SetProperty(ref _allDeviceLogs, value);
        }

        private ObservableCollection<DeviceLogModel> _latestDeviceLogs;
        public ObservableCollection<DeviceLogModel> LatestDeviceLogs
        {
            get => _latestDeviceLogs;
            set => SetProperty(ref _latestDeviceLogs, value);
        }

        // --- Filter properties ---
        private DateTime? _filterDateFrom;
        public DateTime? FilterDateFrom
        {
            get => _filterDateFrom;
            set => SetProperty(ref _filterDateFrom, value, ApplyFilters);
        }

        private DateTime? _filterDateTo;
        public DateTime? FilterDateTo
        {
            get => _filterDateTo;
            set => SetProperty(ref _filterDateTo, value, ApplyFilters);
        }

        private string _searchSerialNumber;
        public string SearchSerialNumber
        {
            get => _searchSerialNumber;
            set => SetProperty(ref _searchSerialNumber, value, ApplyFilters);
        }

        private string _selectedEventFilter;
        public string SelectedEventFilter
        {
            get => _selectedEventFilter;
            set => SetProperty(ref _selectedEventFilter, value, ApplyFilters);
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
            set => SetProperty(ref _selectedFromWarehouseFilter, value, ApplyFilters);
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
            set => SetProperty(ref _selectedToWarehouseFilter, value, ApplyFilters);
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
            set => SetProperty(ref _selectedUserFilter, value, ApplyFilters);
        }
        private ObservableCollection<string> _availableUsers;
        public ObservableCollection<string> AvailableUsers
        {
            get => _availableUsers;
            set => SetProperty(ref _availableUsers, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private async Task LoadDevicesLogsAsync()
        {
            IsLoading = true;
            try
            {
                AllDeviceLogs = await SqliteDataAccess.LoadDevicesLogsAsync();
                InitializeFilterOptions();
                ResetAllFiltersAndApply();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas ładowania logów urządzeń: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void InitializeFilterOptions()
        {
            AvailableEvents.Clear();
            AvailableEvents.Add("Wszystkie");
            foreach (var item in AllDeviceLogs.Select(log => log.Event).Where(e => e != null).Distinct().OrderBy(e => e))
                AvailableEvents.Add(item);

            AvailableFromWarehouses.Clear();
            AvailableFromWarehouses.Add("Wszystkie");
            foreach (var item in AllDeviceLogs.Select(log => log.FromWarehouseName).Where(w => w != null).Distinct().OrderBy(w => w))
                AvailableFromWarehouses.Add(item);

            AvailableToWarehouses.Clear();
            AvailableToWarehouses.Add("Wszystkie");
            foreach (var item in AllDeviceLogs.Select(log => log.ToWarehouseName).Where(w => w != null).Distinct().OrderBy(w => w))
                AvailableToWarehouses.Add(item);

            AvailableUsers.Clear();
            AvailableUsers.Add("Wszyscy");
            foreach (var item in AllDeviceLogs.Select(log => log.UserName).Where(u => u != null).Distinct().OrderBy(u => u))
                AvailableUsers.Add(item);
        }

        private void ApplyFilters()
        {
            IEnumerable<DeviceLogModel> filteredData = _allDeviceLogs;

            if (FilterDateFrom.HasValue)
                filteredData = filteredData.Where(log => log.EventDate.Date >= FilterDateFrom.Value.Date);

            if (FilterDateTo.HasValue)
                filteredData = filteredData.Where(log => log.EventDate < FilterDateTo.Value.Date.AddDays(1));

            if (!string.IsNullOrWhiteSpace(SearchSerialNumber))
                filteredData = filteredData.Where(log => log.SerialNumber != null &&
                                                          log.SerialNumber.IndexOf(SearchSerialNumber, StringComparison.OrdinalIgnoreCase) >= 0);

            if (SelectedEventFilter != "Wszystkie")
                filteredData = filteredData.Where(log => log.Event == SelectedEventFilter);

            if (SelectedFromWarehouseFilter != "Wszystkie")
                filteredData = filteredData.Where(log => log.FromWarehouseName == SelectedFromWarehouseFilter);

            if (SelectedToWarehouseFilter != "Wszystkie")
                filteredData = filteredData.Where(log => log.ToWarehouseName == SelectedToWarehouseFilter);

            if (SelectedUserFilter != "Wszyscy")
                filteredData = filteredData.Where(log => log.UserName == SelectedUserFilter);

            LatestDeviceLogs = new ObservableCollection<DeviceLogModel>(filteredData.OrderByDescending(log => log.EventDate));
        }

        private void ResetAllFiltersAndApply()
        {
            FilterDateFrom = null;
            FilterDateTo = null;
            SearchSerialNumber = string.Empty;
            SelectedEventFilter = "Wszystkie";
            SelectedFromWarehouseFilter = "Wszystkie";
            SelectedToWarehouseFilter = "Wszystkie";
            SelectedUserFilter = "Wszyscy";
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            LoadDevicesLogsCommand.Execute();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            AllDeviceLogs.Clear();
            LatestDeviceLogs.Clear();
            ResetAllFiltersAndApply();
        }
    }
}