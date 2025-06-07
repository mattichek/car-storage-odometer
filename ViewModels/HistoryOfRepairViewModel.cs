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
    // Zaktualizowano, aby implementował INavigationAware
    public class HistoryOfRepairViewModel : BindableBase, INavigationAware
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
            set
            {
                if (SetProperty(ref _filterDateFrom, value))
                {
                    ApplyFilters(); // Zastosuj filtry po zmianie daty
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
                    ApplyFilters(); // Zastosuj filtry po zmianie daty
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
                    ApplyFilters(); // Zastosuj filtry po zmianie użytkownika
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
                    ApplyFilters(); // Zastosuj filtry po zmianie urządzenia
                }
            }
        }
        private ObservableCollection<string> _availableDevices;
        public ObservableCollection<string> AvailableDevices
        {
            get => _availableDevices;
            set => SetProperty(ref _availableDevices, value);
        }


        private string _selectedActionFilter;
        public string SelectedActionFilter
        {
            get => _selectedActionFilter;
            set
            {
                if (SetProperty(ref _selectedActionFilter, value))
                {
                    ApplyFilters(); // Zastosuj filtry po zmianie akcji
                }
            }
        }
        private ObservableCollection<string> _availableActions;
        public ObservableCollection<string> AvailableActions
        {
            get => _availableActions;
            set => SetProperty(ref _availableActions, value);
        }


        public DelegateCommand LoadRepairHistoryCommand { get; private set; } // Zmieniono nazwę komendy
        public DelegateCommand ResetFiltersCommand { get; private set; }

        public HistoryOfRepairViewModel()
        {
            LoadRepairHistoryCommand = new DelegateCommand(async () => await LoadRepairHistoryAsync());
            ResetFiltersCommand = new DelegateCommand(ResetAllFilters); // Zmieniono nazwę metody

            _allRepairHistory = new ObservableCollection<RepairHistoryModel>();
            LatestRepairHistory = new ObservableCollection<RepairHistoryModel>();

            AvailableUsers = new ObservableCollection<string>();
            AvailableDevices = new ObservableCollection<string>();
            AvailableActions = new ObservableCollection<string>();

            ResetAllFilters(); // Ustaw domyślne wartości filtrów
        }

        private async Task LoadRepairHistoryAsync()
        {
            try
            {
                // Zakładam, że masz metodę LoadRepairHistoryAsync w SqliteDataAccess
                _allRepairHistory = await SqliteDataAccess<RepairHistoryModel>.LoadQuery(
                @"SELECT 
                    rh.RepairId,
                    sn.SerialNumber AS SerialNumber,
                    rh.Description,
                    rh.StartDate,
                    rh.EndDate,
                    u.FirstName || ' ' || u.LastName AS UserName
                FROM repairhistory rh
                LEFT JOIN devices d ON rh.DeviceId = d.DeviceId
                LEFT JOIN serialnumbers sn ON d.DeviceId = sn.DeviceId
                LEFT JOIN users u ON rh.UserId = u.UserId
                ;"
                );

                // Uzupełnij ComboBoxy do filtrowania
                AvailableUsers = new ObservableCollection<string>(
                    new[] { "Wszyscy" }.Concat(_allRepairHistory.Select(log => log.UserName).Where(u => u != null).Distinct().OrderBy(u => u))
                );
                AvailableDevices = new ObservableCollection<string>(
                    new[] { "Wszystkie" }.Concat(_allRepairHistory.Select(log => log.SerialNumber).Where(d => d != null).Distinct().OrderBy(d => d))
                );
                AvailableActions = new ObservableCollection<string>(
                    new[] { "Wszystkie" }.Concat(_allRepairHistory.Select(log => log.Description).Where(a => a != null).Distinct().OrderBy(a => a))
                );

                // Po załadowaniu danych, zastosuj filtry (uwzględniając domyślne wartości)
                ApplyFilters();
            }
            catch (Exception ex)
            {
                // Tutaj możesz dodać obsługę błędów, np. MessageBox.Show
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

            if (SelectedDeviceFilter != null && SelectedDeviceFilter != "Wszystkie")
                filteredData = filteredData.Where(log => log.SerialNumber == SelectedDeviceFilter);

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

            // Upewnij się, że ComboBoxy mają "Wszyscy" i "Wszystkie" jako domyślne wybrane po resecie
            if (AvailableUsers.Contains("Wszyscy")) SelectedUserFilter = "Wszyscy";
            if (AvailableDevices.Contains("Wszystkie")) SelectedDeviceFilter = "Wszystkie";
            if (AvailableActions.Contains("Wszystkie")) SelectedActionFilter = "Wszystkie";

            // Ponowne zastosowanie filtrów spowoduje odświeżenie LatestRepairHistory
            ApplyFilters();
        }

        // --- Implementacja INavigationAware ---

        // Wywoływana, gdy widok jest aktywowany
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            // Ładuj dane za każdym razem, gdy widok jest aktywowany
            LoadRepairHistoryCommand.Execute();
        }

        // Określa, czy widok powinien być ponownie użyty
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true; // Zwróć true, aby ponownie używać istniejącej instancji ViewModelu
        }

        // Wywoływana, gdy widok jest dezaktywowany
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _allRepairHistory.Clear(); // Jeśli chcesz zwolnić pamięć
            LatestRepairHistory.Clear();
            ResetAllFilters(); // Resetuj filtry po opuszczeniu widoku
        }
    }
}