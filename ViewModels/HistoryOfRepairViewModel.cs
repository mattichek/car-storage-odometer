using car_storage_odometer.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using car_storage_odometer.DataBaseModules;

namespace car_storage_odometer.ViewModels
{
    public class HistoryOfRepairViewModel : BindableBase, INavigationAware
    {
        private ObservableCollection<RepairHistoryModel> _allRepairHistory;
        private ObservableCollection<RepairHistoryModel> _latestRepairHistory;
        public ObservableCollection<RepairHistoryModel> LatestRepairHistory
        {
            get => _latestRepairHistory;
            set => SetProperty(ref _latestRepairHistory, value);
        }

        private DateTime? _filterDateFrom;
        public DateTime? FilterDateFrom
        {
            get => _filterDateFrom;
            set
            {
                if (SetProperty(ref _filterDateFrom, value))
                {
                    ApplyFilters();
                }
            }
        }

        private DateTime? _filterDateTo;
        public DateTime? FilterDateTo
        {
            get => _filterDateTo;
            set
            {
                if (SetProperty(ref _filterDateTo, value))
                {
                    ApplyFilters();
                }
            }
        }

        private string _selectedUserFilter;
        public string SelectedUserFilter
        {
            get => _selectedUserFilter;
            set
            {
                if (SetProperty(ref _selectedUserFilter, value))
                {
                    ApplyFilters();
                }
            }
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
            set
            {
                if (SetProperty(ref _selectedDeviceFilter, value))
                {
                    ApplyFilters();
                }
            }
        }

        private string _searchSerialNumber;
        public string SearchSerialNumber
        {
            get => _searchSerialNumber;
            set => SetProperty(ref _searchSerialNumber, value, ApplyFilters);
        }

        private string _selectedActionFilter;
        public string SelectedActionFilter
        {
            get => _selectedActionFilter;
            set
            {
                if (SetProperty(ref _selectedActionFilter, value))
                {
                    ApplyFilters();
                }
            }
        }
        private ObservableCollection<string> _availableActions;
        public ObservableCollection<string> AvailableActions
        {
            get => _availableActions;
            set => SetProperty(ref _availableActions, value);
        }


        public DelegateCommand LoadRepairHistoryCommand { get; private set; } 
        public DelegateCommand ResetFiltersCommand { get; private set; }

        public HistoryOfRepairViewModel()
        {
            LoadRepairHistoryCommand = new DelegateCommand(async () => await LoadRepairHistoryAsync());
            ResetFiltersCommand = new DelegateCommand(ResetAllFilters);

            _allRepairHistory = new ObservableCollection<RepairHistoryModel>();
            LatestRepairHistory = new ObservableCollection<RepairHistoryModel>();
            AvailableUsers = new ObservableCollection<string>();
            AvailableActions = new ObservableCollection<string>();

            ResetAllFilters();
        }

        private async Task LoadRepairHistoryAsync()
        {
            try
            {
                _allRepairHistory = await SqliteDataAccess<RepairHistoryModel>.LoadQuery(SqliteQuery.LoadHistroyOfRepairQuery);

                AvailableUsers = new ObservableCollection<string>(
                    new[] { "Wszyscy" }.Concat(_allRepairHistory.Select(log => log.UserName).Where(u => u != null).Distinct().OrderBy(u => u))
                );
                AvailableActions = new ObservableCollection<string>(
                    new[] { "Wszystkie" }.Concat(_allRepairHistory.Select(log => log.Description).Where(a => a != null).Distinct().OrderBy(a => a))
                );

                ApplyFilters();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Błąd ładowania historii napraw: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
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

            if (!string.IsNullOrWhiteSpace(SearchSerialNumber))
                filteredData = filteredData.Where(log => log.SerialNumber != null &&
                                                          log.SerialNumber.IndexOf(SearchSerialNumber, StringComparison.OrdinalIgnoreCase) >= 0);

            if (SelectedActionFilter != null && SelectedActionFilter != "Wszystkie")
                filteredData = filteredData.Where(log => log.Description == SelectedActionFilter);

            LatestRepairHistory = new ObservableCollection<RepairHistoryModel>(filteredData.OrderByDescending(log => log.StartDate));
        }

        private void ResetAllFilters()
        {
            FilterDateFrom = null;
            FilterDateTo = null;
            SearchSerialNumber = string.Empty;
            SelectedUserFilter = "Wszyscy";
            SelectedActionFilter = "Wszystkie";

            if (AvailableUsers.Contains("Wszyscy")) SelectedUserFilter = "Wszyscy";
            if (AvailableActions.Contains("Wszystkie")) SelectedActionFilter = "Wszystkie";

            ApplyFilters();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            LoadRepairHistoryCommand.Execute();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _allRepairHistory.Clear();
            LatestRepairHistory.Clear();
            ResetAllFilters();
        }
    }
}