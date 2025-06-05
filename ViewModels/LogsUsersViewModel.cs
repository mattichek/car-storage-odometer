using car_storage_odometer.Models; // Zakładam, że UserLogModel będzie w Models
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace car_storage_odometer.ViewModels
{
    public class LogsUsersViewModel : BindableBase
    {
        private ObservableCollection<UserLogModel> _allUserLogs;
        private ObservableCollection<UserLogModel> _latestUserLogs;
        public ObservableCollection<UserLogModel> LatestUserLogs
        {
            get => _latestUserLogs;
            set => SetProperty(ref _latestUserLogs, value);
        }

        // Filter properties for LogsUsersView
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

        private string _selectedActionFilter;
        public string SelectedActionFilter
        {
            get => _selectedActionFilter;
            set { SetProperty(ref _selectedActionFilter, value); }
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

        public LogsUsersViewModel()
        {
            LoadDummyData(); // Load initial data
            InitializeFilterOptions(); // Populate filter dropdowns

            // Initially, display all user logs, sorted by date
            LatestUserLogs = new ObservableCollection<UserLogModel>(_allUserLogs.OrderByDescending(log => log.EventDate));

            FilterCommand = new DelegateCommand(ApplyFilters);
            ResetFiltersCommand = new DelegateCommand(ResetAllFilters);
        }

        private void LoadDummyData()
        {
            // Sample data for user logs
            _allUserLogs = new ObservableCollection<UserLogModel>
            {
                new UserLogModel { LogId = 1, EventDate = new DateTime(2024, 5, 20, 9, 0, 0), UserName = "Admin", Event = "Logowanie" },
                new UserLogModel { LogId = 2, EventDate = new DateTime(2024, 5, 20, 9, 5, 0), UserName = "Jan Kowalski", Event = "Dodanie urządzenia" },
                new UserLogModel { LogId = 3, EventDate = new DateTime(2024, 5, 21, 10, 30, 0), UserName = "Admin", Event = "Zmiana uprawnień" },
                new UserLogModel { LogId = 4, EventDate = new DateTime(2024, 5, 21, 11, 0, 0), UserName = "Anna Nowak", Event = "Wydanie urządzenia" },
                new UserLogModel { LogId = 5, EventDate = new DateTime(2024, 5, 22, 14, 15, 0), UserName = "Jan Kowalski", Event = "Wylogowanie" },
                new UserLogModel { LogId = 6, EventDate = new DateTime(2024, 5, 22, 14, 20, 0), UserName = "Admin", Event = "Logowanie" }
            };
        }

        private void InitializeFilterOptions()
        {
            AvailableUsers = new ObservableCollection<string>(
                new[] { "Wszyscy" }.Concat(_allUserLogs.Select(log => log.UserName).Where(u => u != null).Distinct().OrderBy(u => u)));
            SelectedUserFilter = "Wszyscy";

            AvailableActions = new ObservableCollection<string>(
                new[] { "Wszystkie" }.Concat(_allUserLogs.Select(log => log.Event).Where(a => a != null).Distinct().OrderBy(a => a)));
            SelectedActionFilter = "Wszystkie";
        }

        private void ApplyFilters()
        {
            IEnumerable<UserLogModel> filteredData = _allUserLogs;

            if (FilterDateFrom.HasValue)
                filteredData = filteredData.Where(log => log.EventDate.Date >= FilterDateFrom.Value.Date);

            if (FilterDateTo.HasValue)
                filteredData = filteredData.Where(log => log.EventDate.Date <= FilterDateTo.Value.Date);

            if (SelectedUserFilter != null && SelectedUserFilter != "Wszyscy")
                filteredData = filteredData.Where(log => log.UserName == SelectedUserFilter);

            if (SelectedActionFilter != null && SelectedActionFilter != "Wszystkie")
                filteredData = filteredData.Where(log => log.Event == SelectedActionFilter);

            LatestUserLogs = new ObservableCollection<UserLogModel>(filteredData.OrderByDescending(log => log.EventDate));
        }

        private void ResetAllFilters()
        {
            FilterDateFrom = null;
            FilterDateTo = null;
            SelectedUserFilter = "Wszyscy";
            SelectedActionFilter = "Wszystkie";

            LatestUserLogs = new ObservableCollection<UserLogModel>(_allUserLogs.OrderByDescending(log => log.EventDate));
        }
    }
}