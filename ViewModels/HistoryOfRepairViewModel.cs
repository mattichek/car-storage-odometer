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
    public class HistoryOfRepairViewModel : BindableBase
    {
        private ObservableCollection<RepairHistoryModel> _allRepairHistory;
        private ObservableCollection<RepairHistoryModel> _latestRepairHistory;
        public ObservableCollection<RepairHistoryModel> LatestRepairHistory
        {
            get => _latestRepairHistory;
            set => SetProperty(ref _latestRepairHistory, value);
        }

        // Filter properties
        private DateTime? _filterDateFrom;
        public DateTime? FilterDateFrom
        {
            get => _filterDateFrom;
            set { SetProperty(ref _filterDateFrom, value); /* Usunięto ApplyFilters() */ }
        }

        private DateTime? _filterDateTo;
        public DateTime? FilterDateTo
        {
            get => _filterDateTo;
            set { SetProperty(ref _filterDateTo, value); /* Usunięto ApplyFilters() */ }
        }

        private string _selectedUserFilter;
        public string SelectedUserFilter
        {
            get => _selectedUserFilter;
            set { SetProperty(ref _selectedUserFilter, value); /* Usunięto ApplyFilters() */ }
        }
        private ObservableCollection<string> _availableUsers;
        public ObservableCollection<string> AvailableUsers
        {
            get => _availableUsers;
            set => SetProperty(ref _availableUsers, value);
        }

        private string _selectedDeviceFilter;
        public string SelectedDeviceFilter
        {
            get => _selectedDeviceFilter;
            set { SetProperty(ref _selectedDeviceFilter, value); /* Usunięto ApplyFilters() */ }
        }
        private ObservableCollection<string> _availableDeviceTypes; // Zmieniona nazwa
        public ObservableCollection<string> AvailableDeviceTypes // Zmieniona nazwa
        {
            get => _availableDeviceTypes;
            set => SetProperty(ref _availableDeviceTypes, value);
        }

        private string _selectedActionFilter;
        public string SelectedActionFilter
        {
            get => _selectedActionFilter;
            set { SetProperty(ref _selectedActionFilter, value); /* Usunięto ApplyFilters() */ }
        }
        private ObservableCollection<string> _availableActions;
        public ObservableCollection<string> AvailableActions
        {
            get => _availableActions;
            set => SetProperty(ref _availableActions, value);
        }


        // Commands
        public DelegateCommand FilterCommand { get; private set; }
        public DelegateCommand ResetFiltersCommand { get; private set; }

        public HistoryOfRepairViewModel()
        {
            LoadDummyData(); // Load initial data
            InitializeFilterOptions(); // Populate filter dropdowns

            // Initially, display all repair history, posortowane domyślnie
            LatestRepairHistory = new ObservableCollection<RepairHistoryModel>(_allRepairHistory.OrderByDescending(log => log.StartDate));

            FilterCommand = new DelegateCommand(ApplyFilters);
            ResetFiltersCommand = new DelegateCommand(ResetAllFilters);
        }

        private void LoadDummyData()
        {
            // Sample data for repair history
            _allRepairHistory = new ObservableCollection<RepairHistoryModel>
            {
                new RepairHistoryModel { RepairId = 1, DeviceId = 101, DeviceName = "Odometer A1", Description = "Wymiana baterii", StartDate = new DateTime(2024, 1, 15, 10, 0, 0), EndDate = new DateTime(2024, 1, 15, 12, 0, 0), UserId = 1, UserName = "Jan Kowalski" },
                new RepairHistoryModel { RepairId = 2, DeviceId = 102, DeviceName = "Odometer B2", Description = "Naprawa ekranu", StartDate = new DateTime(2024, 2, 10, 14, 30, 0), EndDate = new DateTime(2024, 2, 11, 10, 0, 0), UserId = 2, UserName = "Anna Nowak" },
                new RepairHistoryModel { RepairId = 3, DeviceId = 101, DeviceName = "Odometer A1", Description = "Aktualizacja oprogramowania", StartDate = new DateTime(2024, 3, 5, 9, 0, 0), UserId = 1, UserName = "Jan Kowalski" },
                new RepairHistoryModel { RepairId = 4, DeviceId = 103, DeviceName = "Tracker C3", Description = "Wymiana modułu GPS", StartDate = new DateTime(2024, 4, 20, 11,0,0), EndDate = new DateTime(2024, 4, 21, 16,30,0), UserId = 3, UserName = "Piotr Zieliński" },
                new RepairHistoryModel { RepairId = 5, DeviceId = 102, DeviceName = "Odometer B2", Description = "Wymiana baterii", StartDate = new DateTime(2024, 5, 1, 13,0,0), UserId = 2, UserName = "Anna Nowak" }
            };
        }

        private void InitializeFilterOptions()
        {
            // Add "All" option and populate from distinct values in the data
            AvailableUsers = new ObservableCollection<string>(
                new[] { "Wszyscy" }.Concat(_allRepairHistory.Select(log => log.UserName).Where(u => u != null).Distinct().OrderBy(u => u)));
            SelectedUserFilter = "Wszyscy";

            AvailableDeviceTypes = new ObservableCollection<string>( // Użycie nowej nazwy
                 new[] { "Wszystkie" }.Concat(_allRepairHistory.Select(log => log.DeviceName).Where(d => d != null).Distinct().OrderBy(d => d)));
            SelectedDeviceFilter = "Wszystkie";

            AvailableActions = new ObservableCollection<string>(
                 new[] { "Wszystkie" }.Concat(_allRepairHistory.Select(log => log.Description).Where(a => a != null).Distinct().OrderBy(a => a)));
            SelectedActionFilter = "Wszystkie";
        }

        private void ApplyFilters()
        {
            IEnumerable<RepairHistoryModel> filteredData = _allRepairHistory;

            if (FilterDateFrom.HasValue)
                filteredData = filteredData.Where(log => log.StartDate.Date >= FilterDateFrom.Value.Date);

            if (FilterDateTo.HasValue)
                filteredData = filteredData.Where(log => log.StartDate.Date <= FilterDateTo.Value.Date);

            if (SelectedUserFilter != null && SelectedUserFilter != "Wszyscy")
                filteredData = filteredData.Where(log => log.UserName == SelectedUserFilter);

            if (SelectedDeviceFilter != null && SelectedDeviceFilter != "Wszystkie")
                filteredData = filteredData.Where(log => log.DeviceName == SelectedDeviceFilter);

            if (SelectedActionFilter != null && SelectedActionFilter != "Wszystkie")
                filteredData = filteredData.Where(log => log.Description == SelectedActionFilter);

            LatestRepairHistory = new ObservableCollection<RepairHistoryModel>(filteredData.OrderByDescending(log => log.StartDate));
        }

        private void ResetAllFilters()
        {
            FilterDateFrom = null;
            FilterDateTo = null;
            SelectedUserFilter = "Wszyscy";
            SelectedDeviceFilter = "Wszystkie";
            SelectedActionFilter = "Wszystkie";

            // Po zresetowaniu filtrów, również posortuj domyślnie
            LatestRepairHistory = new ObservableCollection<RepairHistoryModel>(_allRepairHistory.OrderByDescending(log => log.StartDate));
        }
    }
}