using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

// Przykładowe modele danych. Dostosuj je do swoich rzeczywistych modeli.
// Te klasy powinny być spójne w całym projekcie (np. w osobnym folderze Models).
// Jeśli już masz te definicje, możesz je pominąć.
public class UserLog
{
    public int LogId { get; set; }
    public DateTime Timestamp { get; set; }
    public int UserId { get; set; } // Zastąpione przez UserName
    public string Action { get; set; }

    // Właściwość pomocnicza do wyświetlania w DataGrid
    public string UserName { get; set; }
}

namespace car_storage_odometer.ViewModels
{
    public class LogsUsersViewModel : BindableBase
    {
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

        private User _selectedUserFilter;
        public User SelectedUserFilter
        {
            get => _selectedUserFilter;
            set => SetProperty(ref _selectedUserFilter, value);
        }

        private string _selectedActionFilter;
        public string SelectedActionFilter
        {
            get => _selectedActionFilter;
            set => SetProperty(ref _selectedActionFilter, value);
        }

        private ObservableCollection<UserLog> _filteredUserLogs;
        public ObservableCollection<UserLog> FilteredUserLogs
        {
            get => _filteredUserLogs;
            set => SetProperty(ref _filteredUserLogs, value);
        }

        private ObservableCollection<User> _availableUsers;
        public ObservableCollection<User> AvailableUsers
        {
            get => _availableUsers;
            set => SetProperty(ref _availableUsers, value);
        }

        private ObservableCollection<string> _availableActions;
        public ObservableCollection<string> AvailableActions
        {
            get => _availableActions;
            set => SetProperty(ref _availableActions, value);
        }

        public DelegateCommand FilterLogsCommand { get; private set; }
        public DelegateCommand ResetFiltersCommand { get; private set; }

        // Symulowane dane z bazy danych
        private ObservableCollection<UserLog> _allUserLogs;
        private ObservableCollection<User> _allUsers;


        public LogsUsersViewModel()
        {
            // Inicjalizacja komend
            FilterLogsCommand = new DelegateCommand(ExecuteFilterLogs);
            ResetFiltersCommand = new DelegateCommand(ExecuteResetFilters);

            // Wczytaj dane początkowe (symulacja)
            LoadInitialData();
            
            // Zainicjuj ObservableCollections
            FilteredUserLogs = new ObservableCollection<UserLog>();
            AvailableUsers = new ObservableCollection<User>();
            AvailableActions = new ObservableCollection<string>();

            // Ustaw filtry i załaduj logi
            PopulateFilterOptions();
            ExecuteFilterLogs(); // Wyświetl wszystkie logi na start
        }

        private void LoadInitialData()
        {
            // Symulacja danych: Użytkownicy
            _allUsers = new ObservableCollection<User>
            {
                new User { Id = 1001, UserName = "Jan Kowalski" },
                new User { Id = 1002, UserName = "Anna Nowak" },
                new User { Id = 1003, UserName = "Piotr Zieliński" }
            };

            // Symulacja danych: Logi użytkowników
            _allUserLogs = new ObservableCollection<UserLog>
            {
                new UserLog { LogId = 1, Timestamp = DateTime.Now.AddDays(-5), UserId = 1001, Action = "Logowanie" },
                new UserLog { LogId = 2, Timestamp = DateTime.Now.AddDays(-3), UserId = 1002, Action = "Dodano urządzenie" },
                new UserLog { LogId = 3, Timestamp = DateTime.Now.AddDays(-1), UserId = 1001, Action = "Wylogowanie" },
                new UserLog { LogId = 4, Timestamp = DateTime.Now.AddHours(-2), UserId = 1003, Action = "Modyfikacja danych" },
                new UserLog { LogId = 5, Timestamp = DateTime.Now.AddHours(-1), UserId = 1002, Action = "Usunięto urządzenie" }
            };

            // Wypełnij pomocnicze właściwości (np. nazwy zamiast ID)
            foreach (var log in _allUserLogs)
            {
                log.UserName = _allUsers.FirstOrDefault(u => u.Id == log.UserId)?.UserName;
            }
        }

        private void PopulateFilterOptions()
        {
            // Wyczyść stare opcje
            AvailableUsers.Clear();
            AvailableActions.Clear();

            // Dodaj opcję "Wszyscy" dla użytkowników (ważne: Id = 0 dla "Wszyscy")
            AvailableUsers.Add(new User { Id = 0, UserName = "Wszyscy" });
            foreach (var user in _allUsers.OrderBy(u => u.UserName))
            {
                AvailableUsers.Add(user);
            }

            // Dodaj opcję "Wszystkie" dla akcji
            AvailableActions.Add("Wszystkie");
            foreach (var action in _allUserLogs.Select(log => log.Action).Distinct().OrderBy(a => a))
            {
                AvailableActions.Add(action);
            }

            // Ustaw domyślne wybrane opcje
            SelectedUserFilter = AvailableUsers.FirstOrDefault(); // Powinno ustawić na "Wszyscy"
            SelectedActionFilter = AvailableActions.FirstOrDefault(); // Powinno ustawić na "Wszystkie"
        }

        private void ExecuteFilterLogs()
        {
            var query = _allUserLogs.AsEnumerable(); // Użyj AsEnumerable, aby późniejsze filtrowanie odbywało się po stronie klienta (LINQ to Objects)

            if (FilterDateFrom.HasValue)
            {
                query = query.Where(log => log.Timestamp.Date >= FilterDateFrom.Value.Date);
            }

            if (FilterDateTo.HasValue)
            {
                // Dodajemy jeden dzień i bierzemy datę, aby uwzględnić cały dzień 'do'
                query = query.Where(log => log.Timestamp.Date <= FilterDateTo.Value.Date);
            }

            // Filtr użytkowników
            if (SelectedUserFilter != null && SelectedUserFilter.Id != 0) // Sprawdź, czy nie wybrano "Wszyscy" (Id = 0)
            {
                query = query.Where(log => log.UserId == SelectedUserFilter.Id);
            }

            // Filtr akcji
            if (SelectedActionFilter != "Wszystkie" && !string.IsNullOrEmpty(SelectedActionFilter))
            {
                query = query.Where(log => log.Action == SelectedActionFilter);
            }

            // Posortuj logi od najnowszych do najstarszych
            var result = new ObservableCollection<UserLog>(query.OrderByDescending(log => log.Timestamp));
            FilteredUserLogs = result;
        }

        private void ExecuteResetFilters()
        {
            FilterDateFrom = null;
            FilterDateTo = null;
            SelectedUserFilter = AvailableUsers.FirstOrDefault(); // Ustaw na "Wszyscy"
            SelectedActionFilter = AvailableActions.FirstOrDefault(); // Ustaw na "Wszystkie"
            ExecuteFilterLogs(); // Odśwież logi po zresetowaniu filtrów
        }
    }
}