using car_storage_odometer.Models; // Zakładam, że UserLogModel będzie w Models
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions; // Dodano, aby używać INavigationAware
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using car_storage_odometer.DataBaseModules;

namespace car_storage_odometer.ViewModels
{
    // Zaktualizowano, aby implementował INavigationAware
    public class LogsUsersViewModel : BindableBase, INavigationAware
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

        public DelegateCommand LoadUserLogsCommand { get; private set; } // Zmieniono nazwę na LoadUserLogsCommand
        public DelegateCommand ResetFiltersCommand { get; private set; }

        public LogsUsersViewModel()
        {
            LoadUserLogsCommand = new DelegateCommand(async () => await LoadUserLogsAsync()); // Przypisanie asynchronicznej metody
            ResetFiltersCommand = new DelegateCommand(ResetAllFilters); // Zmieniono nazwę metody

            _allUserLogs = new ObservableCollection<UserLogModel>();
            LatestUserLogs = new ObservableCollection<UserLogModel>();

            AvailableUsers = new ObservableCollection<string>();
            AvailableActions = new ObservableCollection<string>();

            ResetAllFilters(); // Resetuj filtry przy inicjalizacji
        }

        private async Task LoadUserLogsAsync()
        {
            try
            {
                _allUserLogs = await SqliteDataAccess<UserLogModel>.LoadQuery(SqliteQuery.LoadAllUserLogsQuery);

                AvailableUsers = new ObservableCollection<string>(
                    new[] { "Wszyscy" }.Concat(_allUserLogs.Select(log => log.UserName).Where(u => u != null).Distinct().OrderBy(u => u))
                );
                AvailableActions = new ObservableCollection<string>(
                    new[] { "Wszystkie" }.Concat(_allUserLogs.Select(log => log.Event).Where(a => a != null).Distinct().OrderBy(a => a))
                );

                ApplyFilters();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Błąd ładowania logów użytkowników: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
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

            // Upewnij się, że ComboBoxy mają "Wszyscy" i "Wszystkie" jako domyślne wybrane po resecie
            if (AvailableUsers.Contains("Wszyscy")) SelectedUserFilter = "Wszyscy";
            if (AvailableActions.Contains("Wszystkie")) SelectedActionFilter = "Wszystkie";

            // Ponowne zastosowanie filtrów spowoduje odświeżenie LatestUserLogs
            ApplyFilters();
        }

        // --- Implementacja INavigationAware ---

        // Wywoływana, gdy widok jest aktywowany
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            // Ładuj dane za każdym razem, gdy widok jest aktywowany
            LoadUserLogsCommand.Execute();
        }

        // Określa, czy widok powinien być ponownie użyty
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true; // Zwróć true, aby ponownie używać istniejącej instancji ViewModelu
        }

        // Wywoływana, gdy widok jest dezaktywowany
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _allUserLogs.Clear(); // Jeśli chcesz zwolnić pamięć, ale wtedy będziesz musiał ponownie ładować AvailableUsers/Actions
            LatestUserLogs.Clear();
            ResetAllFilters(); // Resetuj filtry po opuszczeniu widoku
        }
    }
}