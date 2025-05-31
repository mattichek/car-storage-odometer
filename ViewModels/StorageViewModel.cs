using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

// Przykładowe modele danych. Dostosuj je do swoich rzeczywistych modeli.
// Te klasy powinny być spójne w całym projekcie (np. w osobnym folderze Models).
// Jeśli już masz te definicje, możesz je pominąć.

// Model reprezentujący urządzenie w magazynie
public class StoredDevice
{
    public int DeviceId { get; set; } // To jest urzadzenie_id z bazy danych
    public int TypeId { get; set; }
    public int StatusId { get; set; }
    public int WarehouseId { get; set; }
    public int UserId { get; set; } // user_id - kto dodał

    // Właściwości pomocnicze do wyświetlania w DataGrid
    public string SerialNumber { get; set; } // Numer seryjny urządzenia
    public string TypeName { get; set; }
    public string StatusName { get; set; }
    public string WarehouseName { get; set; }
    public string AddedByUserName { get; set; }
}

public class DeviceType
{
    public int Id { get; set; }
    public string TypeName { get; set; }
}

public class DeviceStatus
{
    public int Id { get; set; }
    public string StatusName { get; set; }
}

namespace car_storage_odometer.ViewModels
{
    public class StorageViewModel : BindableBase
    {
        private string _selectedDeviceSerialFilter;
        public string SelectedDeviceSerialFilter
        {
            get => _selectedDeviceSerialFilter;
            set => SetProperty(ref _selectedDeviceSerialFilter, value);
        }

        private DeviceType _selectedDeviceTypeFilter;
        public DeviceType SelectedDeviceTypeFilter
        {
            get => _selectedDeviceTypeFilter;
            set => SetProperty(ref _selectedDeviceTypeFilter, value);
        }

        private DeviceStatus _selectedDeviceStatusFilter;
        public DeviceStatus SelectedDeviceStatusFilter
        {
            get => _selectedDeviceStatusFilter;
            set => SetProperty(ref _selectedDeviceStatusFilter, value);
        }

        private Warehouse _selectedWarehouseFilter;
        public Warehouse SelectedWarehouseFilter
        {
            get => _selectedWarehouseFilter;
            set => SetProperty(ref _selectedWarehouseFilter, value);
        }

        private User _selectedUserFilter;
        public User SelectedUserFilter
        {
            get => _selectedUserFilter;
            set => SetProperty(ref _selectedUserFilter, value);
        }

        private ObservableCollection<StoredDevice> _filteredDevices;
        public ObservableCollection<StoredDevice> FilteredDevices
        {
            get => _filteredDevices;
            set => SetProperty(ref _filteredDevices, value);
        }

        private ObservableCollection<string> _availableDeviceSerials;
        public ObservableCollection<string> AvailableDeviceSerials
        {
            get => _availableDeviceSerials;
            set => SetProperty(ref _availableDeviceSerials, value);
        }

        private ObservableCollection<DeviceType> _availableDeviceTypes;
        public ObservableCollection<DeviceType> AvailableDeviceTypes
        {
            get => _availableDeviceTypes;
            set => SetProperty(ref _availableDeviceTypes, value);
        }

        private ObservableCollection<DeviceStatus> _availableDeviceStatuses;
        public ObservableCollection<DeviceStatus> AvailableDeviceStatuses
        {
            get => _availableDeviceStatuses;
            set => SetProperty(ref _availableDeviceStatuses, value);
        }

        private ObservableCollection<Warehouse> _availableWarehouses;
        public ObservableCollection<Warehouse> AvailableWarehouses
        {
            get => _availableWarehouses;
            set => SetProperty(ref _availableWarehouses, value);
        }

        private ObservableCollection<User> _availableUsers;
        public ObservableCollection<User> AvailableUsers
        {
            get => _availableUsers;
            set => SetProperty(ref _availableUsers, value);
        }

        public DelegateCommand FilterDevicesCommand { get; private set; }
        public DelegateCommand ResetFiltersCommand { get; private set; }

        // Symulowane dane z bazy danych
        private ObservableCollection<StoredDevice> _allStoredDevices;
        // Zakładam, że te dane są pobierane z globalnych źródeł, np. singletonów, repozytoriów
        private ObservableCollection<DeviceType> _allDeviceTypes;
        private ObservableCollection<DeviceStatus> _allDeviceStatuses;
        private ObservableCollection<Warehouse> _allWarehouses;
        private ObservableCollection<User> _allUsers;

        public StorageViewModel()
        {
            // Inicjalizacja komend
            FilterDevicesCommand = new DelegateCommand(ExecuteFilterDevices);
            ResetFiltersCommand = new DelegateCommand(ExecuteResetFilters);

            // Wczytaj dane początkowe (symulacja)
            LoadInitialData();

            // Zainicjuj ObservableCollections
            FilteredDevices = new ObservableCollection<StoredDevice>();
            AvailableDeviceSerials = new ObservableCollection<string>();
            AvailableDeviceTypes = new ObservableCollection<DeviceType>();
            AvailableDeviceStatuses = new ObservableCollection<DeviceStatus>();
            AvailableWarehouses = new ObservableCollection<Warehouse>();
            AvailableUsers = new ObservableCollection<User>();

            // Ustaw filtry i załaduj urządzenia
            PopulateFilterOptions();
            ExecuteFilterDevices(); // Wyświetl wszystkie urządzenia na start
        }

        private void LoadInitialData()
        {
            // Symulacja danych: Typy urządzeń
            _allDeviceTypes = new ObservableCollection<DeviceType>
            {
                new DeviceType { Id = 1, TypeName = "Telefon" },
                new DeviceType { Id = 2, TypeName = "Laptop" },
                new DeviceType { Id = 3, TypeName = "Tablet" }
            };

            // Symulacja danych: Statusy urządzeń
            _allDeviceStatuses = new ObservableCollection<DeviceStatus>
            {
                new DeviceStatus { Id = 1, StatusName = "Dostępny" },
                new DeviceStatus { Id = 2, StatusName = "W naprawie" },
                new DeviceStatus { Id = 3, StatusName = "Wypożyczony" },
                new DeviceStatus { Id = 4, StatusName = "Zezłomowany" }
            };

            // Symulacja danych: Magazyny
            _allWarehouses = new ObservableCollection<Warehouse>
            {
                new Warehouse { Id = 101, WarehouseName = "Magazyn Główny" },
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

            // Symulacja danych: Urządzenia w magazynie (na podstawie urzadzenie_id, typ_id, status_id, magazyn_id, user_id)
            _allStoredDevices = new ObservableCollection<StoredDevice>
            {
                new StoredDevice { DeviceId = 1, SerialNumber = "SN-ABC-001", TypeId = 1, StatusId = 1, WarehouseId = 101, UserId = 1001 },
                new StoredDevice { DeviceId = 2, SerialNumber = "SN-DEF-002", TypeId = 2, StatusId = 1, WarehouseId = 102, UserId = 1002 },
                new StoredDevice { DeviceId = 3, SerialNumber = "SN-GHI-003", TypeId = 1, StatusId = 2, WarehouseId = 101, UserId = 1001 },
                new StoredDevice { DeviceId = 4, SerialNumber = "SN-JKL-004", TypeId = 3, StatusId = 3, WarehouseId = 103, UserId = 1003 },
                new StoredDevice { DeviceId = 5, SerialNumber = "SN-MNO-005", TypeId = 2, StatusId = 1, WarehouseId = 101, UserId = 1002 }
            };

            // Wypełnij pomocnicze właściwości (nazwy zamiast ID)
            foreach (var device in _allStoredDevices)
            {
                device.TypeName = _allDeviceTypes.FirstOrDefault(t => t.Id == device.TypeId)?.TypeName;
                device.StatusName = _allDeviceStatuses.FirstOrDefault(s => s.Id == device.StatusId)?.StatusName;
                device.WarehouseName = _allWarehouses.FirstOrDefault(w => w.Id == device.WarehouseId)?.WarehouseName;
                device.AddedByUserName = _allUsers.FirstOrDefault(u => u.Id == device.UserId)?.UserName;
            }
        }

        private void PopulateFilterOptions()
        {
            // Wyczyść stare opcje
            AvailableDeviceSerials.Clear();
            AvailableDeviceTypes.Clear();
            AvailableDeviceStatuses.Clear();
            AvailableWarehouses.Clear();
            AvailableUsers.Clear();

            // Dodaj opcję "Wszystkie"
            AvailableDeviceSerials.Add("Wszystkie");
            AvailableDeviceTypes.Add(new DeviceType { Id = 0, TypeName = "Wszystkie" });
            AvailableDeviceStatuses.Add(new DeviceStatus { Id = 0, StatusName = "Wszystkie" });
            AvailableWarehouses.Add(new Warehouse { Id = 0, WarehouseName = "Wszystkie" });
            AvailableUsers.Add(new User { Id = 0, UserName = "Wszyscy" });

            // Dodaj unikalne wartości z danych
            foreach (var serial in _allStoredDevices.Select(d => d.SerialNumber).Distinct().OrderBy(s => s))
            {
                AvailableDeviceSerials.Add(serial);
            }
            foreach (var type in _allDeviceTypes.OrderBy(t => t.TypeName))
            {
                AvailableDeviceTypes.Add(type);
            }
            foreach (var status in _allDeviceStatuses.OrderBy(s => s.StatusName))
            {
                AvailableDeviceStatuses.Add(status);
            }
            foreach (var warehouse in _allWarehouses.OrderBy(w => w.WarehouseName))
            {
                AvailableWarehouses.Add(warehouse);
            }
            foreach (var user in _allUsers.OrderBy(u => u.UserName))
            {
                AvailableUsers.Add(user);
            }

            // Ustaw domyślne wybrane opcje
            SelectedDeviceSerialFilter = "Wszystkie";
            SelectedDeviceTypeFilter = AvailableDeviceTypes.FirstOrDefault();
            SelectedDeviceStatusFilter = AvailableDeviceStatuses.FirstOrDefault();
            SelectedWarehouseFilter = AvailableWarehouses.FirstOrDefault();
            SelectedUserFilter = AvailableUsers.FirstOrDefault();
        }

        private void ExecuteFilterDevices()
        {
            var query = _allStoredDevices.AsEnumerable();

            if (SelectedDeviceSerialFilter != "Wszystkie" && !string.IsNullOrEmpty(SelectedDeviceSerialFilter))
            {
                query = query.Where(device => device.SerialNumber == SelectedDeviceSerialFilter);
            }

            if (SelectedDeviceTypeFilter != null && SelectedDeviceTypeFilter.Id != 0)
            {
                query = query.Where(device => device.TypeId == SelectedDeviceTypeFilter.Id);
            }

            if (SelectedDeviceStatusFilter != null && SelectedDeviceStatusFilter.Id != 0)
            {
                query = query.Where(device => device.StatusId == SelectedDeviceStatusFilter.Id);
            }

            if (SelectedWarehouseFilter != null && SelectedWarehouseFilter.Id != 0)
            {
                query = query.Where(device => device.WarehouseId == SelectedWarehouseFilter.Id);
            }

            if (SelectedUserFilter != null && SelectedUserFilter.Id != 0)
            {
                query = query.Where(device => device.UserId == SelectedUserFilter.Id);
            }

            // Domyślne sortowanie: po numerze seryjnym
            var result = new ObservableCollection<StoredDevice>(query.OrderBy(device => device.SerialNumber));
            FilteredDevices = result;
        }

        private void ExecuteResetFilters()
        {
            SelectedDeviceSerialFilter = "Wszystkie";
            SelectedDeviceTypeFilter = AvailableDeviceTypes.FirstOrDefault();
            SelectedDeviceStatusFilter = AvailableDeviceStatuses.FirstOrDefault();
            SelectedWarehouseFilter = AvailableWarehouses.FirstOrDefault();
            SelectedUserFilter = AvailableUsers.FirstOrDefault();
            ExecuteFilterDevices(); // Odśwież listę po zresetowaniu filtrów
        }
    }
}