using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

// Przykładowe modele danych. Dostosuj je do swoich rzeczywistych modeli.
// Zazwyczaj te modele pochodziłyby z warstwy danych/modelu Twojej aplikacji.
public class DeviceLog
{
    public int LogId { get; set; }
    public DateTime EventDate { get; set; }
    public int DeviceId { get; set; } // Zastąpione przez DeviceSerialNumber
    public int FromWarehouseId { get; set; } // Zastąpione przez FromWarehouseName
    public int ToWarehouseId { get; set; }   // Zastąpione przez ToWarehouseName
    public int FromUserId { get; set; }     // Zastąpione przez FromUserName
    public int ToUserId { get; set; }       // Zastąpione przez ToUserName

    // Właściwości pomocnicze do wyświetlania w DataGrid
    public string DeviceSerialNumber { get; set; }
    public string FromWarehouseName { get; set; }
    public string ToWarehouseName { get; set; }
    public string FromUserName { get; set; }
    public string ToUserName { get; set; }
}

public class Device
{
    public int Id { get; set; }
    public string SerialNumber { get; set; }
    // Inne właściwości urządzenia
}

public class Warehouse
{
    public int Id { get; set; }
    public string WarehouseName { get; set; }
    // Inne właściwości magazynu
}

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public int UserId { get; internal set; }
    // Inne właściwości użytkownika
}


namespace car_storage_odometer.ViewModels
{
    public class LogsDeviceViewModel : BindableBase
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

        private string _selectedDeviceSerialFilter;
        public string SelectedDeviceSerialFilter
        {
            get => _selectedDeviceSerialFilter;
            set => SetProperty(ref _selectedDeviceSerialFilter, value);
        }

        private Warehouse _selectedFromWarehouseFilter;
        public Warehouse SelectedFromWarehouseFilter
        {
            get => _selectedFromWarehouseFilter;
            set => SetProperty(ref _selectedFromWarehouseFilter, value);
        }

        private Warehouse _selectedToWarehouseFilter;
        public Warehouse SelectedToWarehouseFilter
        {
            get => _selectedToWarehouseFilter;
            set => SetProperty(ref _selectedToWarehouseFilter, value);
        }

        private User _selectedFromUserFilter;
        public User SelectedFromUserFilter
        {
            get => _selectedFromUserFilter;
            set => SetProperty(ref _selectedFromUserFilter, value);
        }

        private User _selectedToUserFilter;
        public User SelectedToUserFilter
        {
            get => _selectedToUserFilter;
            set => SetProperty(ref _selectedToUserFilter, value);
        }

        private ObservableCollection<DeviceLog> _filteredDeviceLogs;
        public ObservableCollection<DeviceLog> FilteredDeviceLogs
        {
            get => _filteredDeviceLogs;
            set => SetProperty(ref _filteredDeviceLogs, value);
        }

        private ObservableCollection<string> _availableDeviceSerials;
        public ObservableCollection<string> AvailableDeviceSerials
        {
            get => _availableDeviceSerials;
            set => SetProperty(ref _availableDeviceSerials, value);
        }

        private ObservableCollection<Warehouse> _availableFromWarehouses;
        public ObservableCollection<Warehouse> AvailableFromWarehouses
        {
            get => _availableFromWarehouses;
            set => SetProperty(ref _availableFromWarehouses, value);
        }

        private ObservableCollection<Warehouse> _availableToWarehouses;
        public ObservableCollection<Warehouse> AvailableToWarehouses
        {
            get => _availableToWarehouses;
            set => SetProperty(ref _availableToWarehouses, value);
        }

        private ObservableCollection<User> _availableFromUsers;
        public ObservableCollection<User> AvailableFromUsers
        {
            get => _availableFromUsers;
            set => SetProperty(ref _availableFromUsers, value);
        }

        private ObservableCollection<User> _availableToUsers;
        public ObservableCollection<User> AvailableToUsers
        {
            get => _availableToUsers;
            set => SetProperty(ref _availableToUsers, value);
        }

        public DelegateCommand FilterDeviceLogsCommand { get; private set; }
        public DelegateCommand ResetFiltersCommand { get; private set; }

        // Symulowane dane z bazy danych
        private ObservableCollection<DeviceLog> _allDeviceLogs;
        private ObservableCollection<Device> _allDevices;
        private ObservableCollection<Warehouse> _allWarehouses;
        private ObservableCollection<User> _allUsers;

        public LogsDeviceViewModel()
        {
            // Inicjalizacja komend
            FilterDeviceLogsCommand = new DelegateCommand(ExecuteFilterDeviceLogs);
            ResetFiltersCommand = new DelegateCommand(ExecuteResetFilters);

            // Wczytaj dane początkowe (symulacja)
            LoadInitialData();

            // Zainicjuj ObservableCollections
            FilteredDeviceLogs = new ObservableCollection<DeviceLog>();
            AvailableDeviceSerials = new ObservableCollection<string>();
            AvailableFromWarehouses = new ObservableCollection<Warehouse>();
            AvailableToWarehouses = new ObservableCollection<Warehouse>();
            AvailableFromUsers = new ObservableCollection<User>();
            AvailableToUsers = new ObservableCollection<User>();

            // Ustaw filtry i załaduj logi
            PopulateFilterOptions();
            ExecuteFilterDeviceLogs(); // Wyświetl wszystkie logi na start
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

            // Symulacja danych: Magazyny
            _allWarehouses = new ObservableCollection<Warehouse>
            {
                new Warehouse { Id = 101, WarehouseName = "Magazyn A" },
                new Warehouse { Id = 102, WarehouseName = "Magazyn B" },
                new Warehouse { Id = 103, WarehouseName = "Magazyn C" }
            };

            // Symulacja danych: Użytkownicy
            _allUsers = new ObservableCollection<User>
            {
                new User { Id = 1001, UserName = "Jan Kowalski" },
                new User { Id = 1002, UserName = "Anna Nowak" },
                new User { Id = 1003, UserName = "Piotr Zieliński" }
            };

            // Symulacja danych: Logi urządzeń
            _allDeviceLogs = new ObservableCollection<DeviceLog>
            {
                new DeviceLog { LogId = 1, EventDate = DateTime.Now.AddDays(-5), DeviceId = 1, FromWarehouseId = 101, ToWarehouseId = 102, FromUserId = 1001, ToUserId = 1002 },
                new DeviceLog { LogId = 2, EventDate = DateTime.Now.AddDays(-3), DeviceId = 2, FromWarehouseId = 102, ToWarehouseId = 103, FromUserId = 1002, ToUserId = 1003 },
                new DeviceLog { LogId = 3, EventDate = DateTime.Now.AddDays(-1), DeviceId = 1, FromWarehouseId = 103, ToWarehouseId = 101, FromUserId = 1003, ToUserId = 1001 },
                new DeviceLog { LogId = 4, EventDate = DateTime.Now.AddDays(-0.5), DeviceId = 3, FromWarehouseId = 101, ToWarehouseId = 102, FromUserId = 1001, ToUserId = 1002 },
                new DeviceLog { LogId = 5, EventDate = DateTime.Now.AddHours(-1), DeviceId = 2, FromWarehouseId = 102, ToWarehouseId = 101, FromUserId = 1002, ToUserId = 1001 }
            };

            // Wypełnij pomocnicze właściwości (np. nazwy zamiast ID)
            foreach (var log in _allDeviceLogs)
            {
                log.DeviceSerialNumber = _allDevices.FirstOrDefault(d => d.Id == log.DeviceId)?.SerialNumber;
                log.FromWarehouseName = _allWarehouses.FirstOrDefault(w => w.Id == log.FromWarehouseId)?.WarehouseName;
                log.ToWarehouseName = _allWarehouses.FirstOrDefault(w => w.Id == log.ToWarehouseId)?.WarehouseName;
                log.FromUserName = _allUsers.FirstOrDefault(u => u.Id == log.FromUserId)?.UserName;
                log.ToUserName = _allUsers.FirstOrDefault(u => u.Id == log.ToUserId)?.UserName;
            }
        }

        private void PopulateFilterOptions()
        {
            // Wyczyść stare opcje
            AvailableDeviceSerials.Clear();
            AvailableFromWarehouses.Clear();
            AvailableToWarehouses.Clear();
            AvailableFromUsers.Clear();
            AvailableToUsers.Clear();

            // Dodaj opcję "Wszystkie"
            AvailableDeviceSerials.Add("Wszystkie");
            AvailableFromWarehouses.Add(new Warehouse { Id = 0, WarehouseName = "Wszystkie" });
            AvailableToWarehouses.Add(new Warehouse { Id = 0, WarehouseName = "Wszystkie" });
            AvailableFromUsers.Add(new User { Id = 0, UserName = "Wszyscy" });
            AvailableToUsers.Add(new User { Id = 0, UserName = "Wszyscy" });

            // Dodaj unikalne wartości z logów
            foreach (var serial in _allDevices.Select(d => d.SerialNumber).Distinct().OrderBy(s => s))
            {
                AvailableDeviceSerials.Add(serial);
            }
            foreach (var warehouse in _allWarehouses.OrderBy(w => w.WarehouseName))
            {
                AvailableFromWarehouses.Add(warehouse);
                AvailableToWarehouses.Add(warehouse);
            }
            foreach (var user in _allUsers.OrderBy(u => u.UserName))
            {
                AvailableFromUsers.Add(user);
                AvailableToUsers.Add(user);
            }

            // Ustaw domyślne wybrane opcje
            SelectedDeviceSerialFilter = "Wszystkie";
            SelectedFromWarehouseFilter = AvailableFromWarehouses.FirstOrDefault();
            SelectedToWarehouseFilter = AvailableToWarehouses.FirstOrDefault();
            SelectedFromUserFilter = AvailableFromUsers.FirstOrDefault();
            SelectedToUserFilter = AvailableToUsers.FirstOrDefault();
        }

        private void ExecuteFilterDeviceLogs()
        {
            var query = _allDeviceLogs.AsEnumerable(); // Użyj AsEnumerable, aby późniejsze filtrowanie odbywało się po stronie klienta (LINQ to Objects)

            if (FilterDateFrom.HasValue)
            {
                query = query.Where(log => log.EventDate.Date >= FilterDateFrom.Value.Date);
            }

            if (FilterDateTo.HasValue)
            {
                // Dodajemy jeden dzień i bierzemy datę, aby uwzględnić cały dzień 'do'
                query = query.Where(log => log.EventDate.Date <= FilterDateTo.Value.Date);
            }

            if (SelectedDeviceSerialFilter != "Wszystkie" && !string.IsNullOrEmpty(SelectedDeviceSerialFilter))
            {
                query = query.Where(log => log.DeviceSerialNumber == SelectedDeviceSerialFilter);
            }

            if (SelectedFromWarehouseFilter != null && SelectedFromWarehouseFilter.Id != 0)
            {
                query = query.Where(log => log.FromWarehouseId == SelectedFromWarehouseFilter.Id);
            }

            if (SelectedToWarehouseFilter != null && SelectedToWarehouseFilter.Id != 0)
            {
                query = query.Where(log => log.ToWarehouseId == SelectedToWarehouseFilter.Id);
            }

            if (SelectedFromUserFilter != null && SelectedFromUserFilter.Id != 0)
            {
                query = query.Where(log => log.FromUserId == SelectedFromUserFilter.Id);
            }

            if (SelectedToUserFilter != null && SelectedToUserFilter.Id != 0)
            {
                query = query.Where(log => log.ToUserId == SelectedToUserFilter.Id);
            }

            //Posortuj logi od najnowszych do najstarszych
           var result = new ObservableCollection<DeviceLog>(query.OrderByDescending(log => log.EventDate));
            FilteredDeviceLogs = result;
        }

        private void ExecuteResetFilters()
        {
            FilterDateFrom = null;
            FilterDateTo = null;
            SelectedDeviceSerialFilter = "Wszystkie";
            SelectedFromWarehouseFilter = AvailableFromWarehouses.FirstOrDefault();
            SelectedToWarehouseFilter = AvailableToWarehouses.FirstOrDefault();
            SelectedFromUserFilter = AvailableFromUsers.FirstOrDefault();
            SelectedToUserFilter = AvailableToUsers.FirstOrDefault();
            ExecuteFilterDeviceLogs(); // Odśwież logi po zresetowaniu filtrów
        }
    }
}