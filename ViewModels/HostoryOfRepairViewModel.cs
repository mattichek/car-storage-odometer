using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

// Przykładowe modele danych. Dostosuj je do swoich rzeczywistych modeli.
public class RepairHistory
{
    // public int RepairId { get; set; } // Nie uwzględniamy w ViewModelu i widoku
    public int DeviceId { get; set; } // Zastąpione przez DeviceSerialNumber
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; } // Nullable, jeśli data zakończenia może być pusta
    public int UserId { get; set; } // Zastąpione przez UserName

    // Właściwości pomocnicze do wyświetlania w DataGrid
    public string DeviceSerialNumber { get; set; }
    public string UserName { get; set; }
}

// Device i User - te same klasy co w LogsDeviceViewModel (jeśli są współdzielone)
// Jeśli masz je już zdefiniowane w innym miejscu, możesz je pominąć lub odwołać się do nich.
/*
public class Device
{
    public int Id { get; set; }
    public string SerialNumber { get; set; }
    // Inne właściwości urządzenia
}

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    // Inne właściwości użytkownika
}
*/

namespace car_storage_odometer.ViewModels
{
    public class HostoryOfRepairViewModel : BindableBase
    {
        private DateTime? _filterStartDateFrom;
        public DateTime? FilterStartDateFrom
        {
            get => _filterStartDateFrom;
            set => SetProperty(ref _filterStartDateFrom, value);
        }

        private DateTime? _filterEndDateTo;
        public DateTime? FilterEndDateTo
        {
            get => _filterEndDateTo;
            set => SetProperty(ref _filterEndDateTo, value);
        }

        private string _selectedDeviceSerialFilter;
        public string SelectedDeviceSerialFilter
        {
            get => _selectedDeviceSerialFilter;
            set => SetProperty(ref _selectedDeviceSerialFilter, value);
        }

        private User _selectedUserFilter;
        public User SelectedUserFilter
        {
            get => _selectedUserFilter;
            set => SetProperty(ref _selectedUserFilter, value);
        }

        private ObservableCollection<RepairHistory> _filteredRepairs;
        public ObservableCollection<RepairHistory> FilteredRepairs
        {
            get => _filteredRepairs;
            set => SetProperty(ref _filteredRepairs, value);
        }

        private ObservableCollection<string> _availableDeviceSerials;
        public ObservableCollection<string> AvailableDeviceSerials
        {
            get => _availableDeviceSerials;
            set => SetProperty(ref _availableDeviceSerials, value);
        }

        private ObservableCollection<User> _availableUsers;
        public ObservableCollection<User> AvailableUsers
        {
            get => _availableUsers;
            set => SetProperty(ref _availableUsers, value);
        }

        public DelegateCommand FilterRepairsCommand { get; private set; }
        public DelegateCommand ResetFiltersCommand { get; private set; }

        // Symulowane dane z bazy danych
        private ObservableCollection<RepairHistory> _allRepairs;
        private ObservableCollection<Device> _allDevices;
        private ObservableCollection<User> _allUsers;

        public HostoryOfRepairViewModel()
        {
            // Inicjalizacja komend
            FilterRepairsCommand = new DelegateCommand(ExecuteFilterRepairs);
            ResetFiltersCommand = new DelegateCommand(ExecuteResetFilters);

            // Wczytaj dane początkowe (symulacja)
            LoadInitialData();

            // Zainicjuj ObservableCollections
            FilteredRepairs = new ObservableCollection<RepairHistory>();
            AvailableDeviceSerials = new ObservableCollection<string>();
            AvailableUsers = new ObservableCollection<User>();

            // Ustaw filtry i załaduj logi
            PopulateFilterOptions();
            ExecuteFilterRepairs(); // Wyświetl wszystkie naprawy na start
        }

        private void LoadInitialData()
        {
            // Symulacja danych: Urządzenia
            _allDevices = new ObservableCollection<Device>
            {
                new Device { Id = 1, SerialNumber = "DEV001" },
                new Device { Id = 2, SerialNumber = "DEV002" },
                new Device { Id = 3, SerialNumber = "DEV003" }
            };

            // Symulacja danych: Użytkownicy
            _allUsers = new ObservableCollection<User>
            {
                new User { Id = 1001, UserName = "Jan Kowalski" },
                new User { Id = 1002, UserName = "Anna Nowak" },
                new User { Id = 1003, UserName = "Piotr Zieliński" }
            };

            // Symulacja danych: Historia napraw
            _allRepairs = new ObservableCollection<RepairHistory>
            {
                new RepairHistory { DeviceId = 1, Description = "Wymiana ekranu", StartDate = DateTime.Now.AddDays(-30), EndDate = DateTime.Now.AddDays(-28), UserId = 1001 },
                new RepairHistory { DeviceId = 2, Description = "Naprawa zasilania", StartDate = DateTime.Now.AddDays(-15), EndDate = DateTime.Now.AddDays(-14), UserId = 1002 },
                new RepairHistory { DeviceId = 1, Description = "Aktualizacja oprogramowania", StartDate = DateTime.Now.AddDays(-7), EndDate = DateTime.Now.AddDays(-6), UserId = 1003 },
                new RepairHistory { DeviceId = 3, Description = "Wymiana baterii", StartDate = DateTime.Now.AddDays(-20), EndDate = null, UserId = 1001 }, // Brak daty zakończenia
                new RepairHistory { DeviceId = 2, Description = "Czyszczenie i konserwacja", StartDate = DateTime.Now.AddDays(-2), EndDate = DateTime.Now.AddDays(-1), UserId = 1002 }
            };

            // Wypełnij pomocnicze właściwości (np. nazwy zamiast ID)
            foreach (var repair in _allRepairs)
            {
                repair.DeviceSerialNumber = _allDevices.FirstOrDefault(d => d.Id == repair.DeviceId)?.SerialNumber;
                repair.UserName = _allUsers.FirstOrDefault(u => u.Id == repair.UserId)?.UserName;
            }
        }

        private void PopulateFilterOptions()
        {
            // Wyczyść stare opcje
            AvailableDeviceSerials.Clear();
            AvailableUsers.Clear();

            // Dodaj opcję "Wszystkie"
            AvailableDeviceSerials.Add("Wszystkie");
            AvailableUsers.Add(new User { Id = 0, UserName = "Wszyscy" });

            // Dodaj unikalne wartości z logów
            foreach (var serial in _allDevices.Select(d => d.SerialNumber).Distinct().OrderBy(s => s))
            {
                AvailableDeviceSerials.Add(serial);
            }
            foreach (var user in _allUsers.OrderBy(u => u.UserName))
            {
                AvailableUsers.Add(user);
            }

            // Ustaw domyślne wybrane opcje
            SelectedDeviceSerialFilter = "Wszystkie";
            SelectedUserFilter = AvailableUsers.FirstOrDefault();
        }

        private void ExecuteFilterRepairs()
        {
            var query = _allRepairs.AsEnumerable();

            if (FilterStartDateFrom.HasValue)
            {
                query = query.Where(repair => repair.StartDate.Date >= FilterStartDateFrom.Value.Date);
            }

            if (FilterEndDateTo.HasValue)
            {
                // Ważne: EndDate może być null, więc musimy to obsłużyć.
                // Jeśli EndDate jest null, to uznajemy, że naprawa jeszcze trwa i pasuje do filtru "do"
                query = query.Where(repair => !repair.EndDate.HasValue || repair.EndDate.Value.Date <= FilterEndDateTo.Value.Date);
            }

            if (SelectedDeviceSerialFilter != "Wszystkie" && !string.IsNullOrEmpty(SelectedDeviceSerialFilter))
            {
                query = query.Where(repair => repair.DeviceSerialNumber == SelectedDeviceSerialFilter);
            }

            if (SelectedUserFilter != null && SelectedUserFilter.Id != 0)
            {
                query = query.Where(repair => repair.UserId == SelectedUserFilter.Id);
            }

            // Posortuj naprawy od najnowszych dat rozpoczęcia do najstarszych
            var result = new ObservableCollection<RepairHistory>(query.OrderByDescending(repair => repair.StartDate));
            FilteredRepairs = result;
        }

        private void ExecuteResetFilters()
        {
            FilterStartDateFrom = null;
            FilterEndDateTo = null;
            SelectedDeviceSerialFilter = "Wszystkie";
            SelectedUserFilter = AvailableUsers.FirstOrDefault();
            ExecuteFilterRepairs(); // Odśwież naprawy po zresetowaniu filtrów
        }
    }
}