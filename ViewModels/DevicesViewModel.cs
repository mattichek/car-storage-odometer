// ViewModels/DevicesViewModel.cs
using car_storage_odometer.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows; // Do MessageBox.Show
using System.Collections.Generic; // Dodaj to dla List<string>

namespace car_storage_odometer.ViewModels
{
    public class DevicesViewModel : BindableBase
    {
        private ObservableCollection<DeviceModel> _allDevices;
        private ObservableCollection<DeviceModel> _devicesList;
        public ObservableCollection<DeviceModel> DevicesList
        {
            get => _devicesList;
            set => SetProperty(ref _devicesList, value);
        }

        private DeviceModel _selectedDevice;
        public DeviceModel SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                SetProperty(ref _selectedDevice, value);
                // When a device is selected, populate the editing fields
                if (value != null)
                {
                    // Upewnij się, że zawsze tworzysz NOWY obiekt, a nie referencję do istniejącego
                    // aby edycja nie wpływała od razu na listę, zanim nie klikniesz "Aktualizuj"
                    CurrentEditDevice = new DeviceModel
                    {
                        Id = value.Id,
                        SerialNumber = value.SerialNumber,
                        DeviceType = value.DeviceType,
                        CurrentWarehouse = value.CurrentWarehouse,
                        EntryDate = value.EntryDate,
                        Status = value.Status,
                        Notes = value.Notes
                    };
                    IsEditing = true; // Enable editing mode
                }
                else
                {
                    // Jeśli SelectedDevice jest null (np. po usunięciu lub kliknięciu "Nowe urządzenie")
                    // Przygotuj formularz do dodawania nowego urządzenia
                    CurrentEditDevice = new DeviceModel { EntryDate = DateTime.Now, Status = "Dostępny" }; // Clear editing fields, default values
                    IsEditing = false;
                }
                // Update command CanExecute after SelectedDevice changes
                AddDeviceCommand.RaiseCanExecuteChanged();
                UpdateDeviceCommand.RaiseCanExecuteChanged();
                DeleteDeviceCommand.RaiseCanExecuteChanged();
                MoveDeviceCommand.RaiseCanExecuteChanged();
                ReportRepairCommand.RaiseCanExecuteChanged();
                FinishRepairCommand.RaiseCanExecuteChanged();
            }
        }

        private DeviceModel _currentEditDevice;
        public DeviceModel CurrentEditDevice
        {
            get => _currentEditDevice;
            set
            {
                // Ważne: Jeśli zmieniamy cały obiekt CurrentEditDevice,
                // musimy usunąć stare nasłuchiwanie i dodać nowe
                if (_currentEditDevice != null)
                {
                    _currentEditDevice.PropertyChanged -= CurrentEditDevice_PropertyChanged;
                }
                SetProperty(ref _currentEditDevice, value);
                if (_currentEditDevice != null)
                {
                    _currentEditDevice.PropertyChanged += CurrentEditDevice_PropertyChanged;
                }
                // Po zmianie obiektu CurrentEditDevice, stan komend może się zmienić
                AddDeviceCommand.RaiseCanExecuteChanged();
                UpdateDeviceCommand.RaiseCanExecuteChanged();
                // Inne komendy mogą również zależeć od statusu CurrentEditDevice, np. ReportRepairCommand
                ReportRepairCommand.RaiseCanExecuteChanged();
                FinishRepairCommand.RaiseCanExecuteChanged();
            }
        }

        // Metoda do obsługi PropertyChanged z CurrentEditDevice
        private void CurrentEditDevice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Gdy zmieni się właściwość CurrentEditDevice, odśwież stan komend
            // które zależą od tych właściwości (np. AddDeviceCommand)
            AddDeviceCommand.RaiseCanExecuteChanged();
            UpdateDeviceCommand.RaiseCanExecuteChanged();
            ReportRepairCommand.RaiseCanExecuteChanged();
            FinishRepairCommand.RaiseCanExecuteChanged();
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                SetProperty(ref _isEditing, value);
                // Gdy zmienia się IsEditing, stan AddDeviceCommand zmienia się
                AddDeviceCommand.RaiseCanExecuteChanged();
            }
        }

        // Filter properties
        private string _filterSerialNumber;
        public string FilterSerialNumber
        {
            get => _filterSerialNumber;
            set { SetProperty(ref _filterSerialNumber, value); ApplyFilters(); }
        }

        private string _filterDeviceType;
        public string FilterDeviceType
        {
            get => _filterDeviceType;
            set { SetProperty(ref _filterDeviceType, value); ApplyFilters(); }
        }
        private ObservableCollection<string> _availableDeviceTypes;
        public ObservableCollection<string> AvailableDeviceTypes // Lista dla filtrów
        {
            get => _availableDeviceTypes;
            set => SetProperty(ref _availableDeviceTypes, value);
        }

        private string _filterWarehouse;
        public string FilterWarehouse
        {
            get => _filterWarehouse;
            set { SetProperty(ref _filterWarehouse, value); ApplyFilters(); }
        }
        private ObservableCollection<string> _availableWarehouses;
        public ObservableCollection<string> AvailableWarehouses // Lista dla filtrów
        {
            get => _availableWarehouses;
            set => SetProperty(ref _availableWarehouses, value);
        }

        private string _filterStatus;
        public string FilterStatus
        {
            get => _filterStatus;
            set { SetProperty(ref _filterStatus, value); ApplyFilters(); }
        }
        private ObservableCollection<string> _availableStatuses;
        public ObservableCollection<string> AvailableStatuses
        {
            get => _availableStatuses;
            set => SetProperty(ref _availableStatuses, value);
        }

        // Properties for Move Device
        private string _selectedTargetWarehouse;
        public string SelectedTargetWarehouse
        {
            get => _selectedTargetWarehouse;
            set { SetProperty(ref _selectedTargetWarehouse, value); MoveDeviceCommand.RaiseCanExecuteChanged(); }
        }

        // NOWE: Stałe listy dla ComboBoxów w formularzu dodawania/edycji
        public ObservableCollection<string> AllPossibleDeviceTypes { get; }
        public ObservableCollection<string> AllPossibleWarehouses { get; }
        public ObservableCollection<string> AllPossibleStatuses { get; }

        // Commands
        public DelegateCommand AddDeviceCommand { get; private set; }
        public DelegateCommand UpdateDeviceCommand { get; private set; }
        public DelegateCommand DeleteDeviceCommand { get; private set; }
        public DelegateCommand NewDeviceCommand { get; private set; }
        public DelegateCommand MoveDeviceCommand { get; private set; }
        public DelegateCommand ReportRepairCommand { get; private set; }
        public DelegateCommand FinishRepairCommand { get; private set; }
        public DelegateCommand ResetFiltersCommand { get; private set; }

        public DevicesViewModel()
        {
            // 1. ZAWSZE inicjalizuj stałe listy i komendy NA POCZĄTKU konstruktora.
            // To zapobiega NullReferenceException, gdy UI próbuje się bindować.
            AllPossibleDeviceTypes = new ObservableCollection<string> { "Odometer", "Tracker", "Gateway", "Sensor", "Kamera" };
            AllPossibleWarehouses = new ObservableCollection<string> { "Magazyn Główny", "Magazyn A", "Magazyn B", "Serwis" };
            AllPossibleStatuses = new ObservableCollection<string> { "Dostępny", "W naprawie", "Wydany", "Złomowany" };

            // Inicjalizacja komend musi nastąpić ZANIM cokolwiek innego spróbuje ich użyć
            AddDeviceCommand = new DelegateCommand(AddDevice, CanAddDevice);
            UpdateDeviceCommand = new DelegateCommand(UpdateDevice, CanUpdateOrDeleteDevice);
            DeleteDeviceCommand = new DelegateCommand(DeleteDevice, CanUpdateOrDeleteDevice);
            NewDeviceCommand = new DelegateCommand(NewDevice);
            MoveDeviceCommand = new DelegateCommand(MoveDevice, CanUpdateOrDeleteDevice);
            ReportRepairCommand = new DelegateCommand(ReportRepair, CanReportRepair);
            FinishRepairCommand = new DelegateCommand(FinishRepair, CanFinishRepair);
            ResetFiltersCommand = new DelegateCommand(ResetAllFilters);

            // 2. Ładowanie danych i inicjalizacja stanów
            LoadDummyData();
            InitializeFilterOptions();

            // 3. Ustawienie początkowego stanu formularza edycji/dodawania
            // Bezpośrednie przypisanie do pola, aby uniknąć wywołania settera CurrentEditDevice
            // przed pełną inicjalizacją, a następnie ręczne podłączenie eventu.
            _currentEditDevice = new DeviceModel { EntryDate = DateTime.Now, Status = "Dostępny" };
            _currentEditDevice.PropertyChanged += CurrentEditDevice_PropertyChanged;

            DevicesList = new ObservableCollection<DeviceModel>(_allDevices); // Initial list

            // 4. Obsługa zmian właściwości samego ViewModelu
            PropertyChanged += (s, e) =>
            {
                // SelectedDevice i IsEditing mają już w swoich setterach RaiseCanExecuteChanged
                // SelectedTargetWarehouse również ma w swoim setterze
                // Ten blok może być uproszczony, jeśli wszystko jest obsługiwane w setterach
                // Upewnij się, że wszystkie warunki dla CanExecute są pokryte
            };

            // Upewnij się, że formularz jest gotowy do dodawania nowego urządzenia przy starcie.
            // Wywołanie NewDevice() inicjuje CurrentEditDevice i IsEditing oraz odświeża komendy.
            NewDevice();
        }

        private void LoadDummyData()
        {
            _allDevices = new ObservableCollection<DeviceModel>
            {
                new DeviceModel { Id = 1, SerialNumber = "DEV001", DeviceType = "Odometer", CurrentWarehouse = "Magazyn Główny", EntryDate = new DateTime(2023, 1, 1), Status = "Dostępny", Notes = "Nowy model" },
                new DeviceModel { Id = 2, SerialNumber = "DEV002", DeviceType = "Tracker", CurrentWarehouse = "Serwis", EntryDate = new DateTime(2023, 2, 15), Status = "W naprawie", Notes = "Uszkodzony GPS" },
                new DeviceModel { Id = 3, SerialNumber = "DEV003", DeviceType = "Odometer", CurrentWarehouse = "Magazyn A", EntryDate = new DateTime(2023, 3, 10), Status = "Dostępny", Notes = "" },
                new DeviceModel { Id = 4, SerialNumber = "DEV004", DeviceType = "Gateway", CurrentWarehouse = "Magazyn B", EntryDate = new DateTime(2023, 4, 5), Status = "Dostępny", Notes = "Do testów" },
                new DeviceModel { Id = 5, SerialNumber = "DEV005", DeviceType = "Odometer", CurrentWarehouse = "Magazyn Główny", EntryDate = new DateTime(2023, 5, 20), Status = "Dostępny", Notes = "" }
            };
        }

        private void InitializeFilterOptions()
        {
            AvailableDeviceTypes = new ObservableCollection<string>(
                new[] { "Wszystkie" }.Concat(_allDevices.Select(d => d.DeviceType).Where(dt => dt != null).Distinct().OrderBy(dt => dt)));
            FilterDeviceType = "Wszystkie";

            AvailableWarehouses = new ObservableCollection<string>(
                new[] { "Wszystkie" }.Concat(_allDevices.Select(d => d.CurrentWarehouse).Where(w => w != null).Distinct().OrderBy(w => w)));
            FilterWarehouse = "Wszystkie";
            SelectedTargetWarehouse = AllPossibleWarehouses.FirstOrDefault(); // Ustaw domyślny magazyn docelowy

            AvailableStatuses = new ObservableCollection<string>(
                new[] { "Wszystkie" }.Concat(_allDevices.Select(d => d.Status).Where(s => s != null).Distinct().OrderBy(s => s)));
            FilterStatus = "Wszystkie";
        }

        private void ApplyFilters()
        {
            var filteredData = _allDevices.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(FilterSerialNumber))
                filteredData = filteredData.Where(d => d.SerialNumber.Contains(FilterSerialNumber));

            if (FilterDeviceType != null && FilterDeviceType != "Wszystkie")
                filteredData = filteredData.Where(d => d.DeviceType == FilterDeviceType);

            if (FilterWarehouse != null && FilterWarehouse != "Wszystkie")
                filteredData = filteredData.Where(d => d.CurrentWarehouse == FilterWarehouse);

            if (FilterStatus != null && FilterStatus != "Wszystkie")
                filteredData = filteredData.Where(d => d.Status == FilterStatus);

            DevicesList = new ObservableCollection<DeviceModel>(filteredData.OrderBy(d => d.SerialNumber));
        }

        private void ResetAllFilters()
        {
            FilterSerialNumber = null;
            FilterDeviceType = "Wszystkie";
            FilterWarehouse = "Wszystkie";
            FilterStatus = "Wszystkie";
            ApplyFilters(); // Apply filters after reset
        }

        private void NewDevice()
        {
            SelectedDevice = null; // Deselect any existing device, which triggers SelectedDevice's setter
                                   // SelectedDevice's setter already handles setting CurrentEditDevice and IsEditing
                                   // and calling RaiseCanExecuteChanged for relevant commands.
        }

        private bool CanAddDevice()
        {
            // Teraz sprawdzamy, czy wszystkie wymagane pola są wypełnione
            return !IsEditing && CurrentEditDevice != null &&
                   !string.IsNullOrWhiteSpace(CurrentEditDevice.SerialNumber) &&
                   !string.IsNullOrWhiteSpace(CurrentEditDevice.DeviceType) &&
                   !string.IsNullOrWhiteSpace(CurrentEditDevice.CurrentWarehouse) &&
                   !string.IsNullOrWhiteSpace(CurrentEditDevice.Status);
        }

        private void AddDevice()
        {
            if (CanAddDevice())
            {
                // Sprawdź, czy numer seryjny już istnieje
                if (_allDevices.Any(d => d.SerialNumber.Equals(CurrentEditDevice.SerialNumber, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Urządzenie o podanym numerze seryjnym już istnieje.", "Błąd dodawania", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Generate a simple ID (in a real app, this would be from a database)
                CurrentEditDevice.Id = _allDevices.Any() ? _allDevices.Max(d => d.Id) + 1 : 1;

                _allDevices.Add(CurrentEditDevice);
                ApplyFilters(); // Refresh the list
                InitializeFilterOptions(); // Update filter options (e.g., new device type)
                MessageBox.Show("Urządzenie zostało dodane.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                NewDevice(); // Prepare for next new device entry
            }
            else
            {
                MessageBox.Show("Wypełnij wszystkie wymagane pola (Numer seryjny, Typ, Magazyn, Status).", "Błąd dodawania", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool CanUpdateOrDeleteDevice()
        {
            return SelectedDevice != null;
        }

        private void UpdateDevice()
        {
            if (CanUpdateOrDeleteDevice())
            {
                // Sprawdź, czy zmieniony numer seryjny nie koliduje z innym urządzeniem
                if (_allDevices.Any(d => d.Id != CurrentEditDevice.Id && d.SerialNumber.Equals(CurrentEditDevice.SerialNumber, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Numer seryjny jest już używany przez inne urządzenie.", "Błąd aktualizacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var existingDevice = _allDevices.FirstOrDefault(d => d.Id == CurrentEditDevice.Id);
                if (existingDevice != null)
                {
                    // Używamy SetProperty dla właściwości, aby wywołać INotifyPropertyChanged
                    existingDevice.SerialNumber = CurrentEditDevice.SerialNumber;
                    existingDevice.DeviceType = CurrentEditDevice.DeviceType;
                    existingDevice.CurrentWarehouse = CurrentEditDevice.CurrentWarehouse;
                    existingDevice.EntryDate = CurrentEditDevice.EntryDate;
                    existingDevice.Status = CurrentEditDevice.Status;
                    existingDevice.Notes = CurrentEditDevice.Notes;

                    ApplyFilters(); // Refresh the list
                    InitializeFilterOptions(); // Update filter options if necessary
                    MessageBox.Show("Dane urządzenia zostały zaktualizowane.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Wybierz urządzenie do aktualizacji.", "Błąd aktualizacji", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteDevice()
        {
            if (CanUpdateOrDeleteDevice() && MessageBox.Show($"Czy na pewno chcesz usunąć urządzenie o numerze seryjnym: {SelectedDevice.SerialNumber}?", "Potwierdź usunięcie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _allDevices.Remove(SelectedDevice);
                ApplyFilters(); // Refresh the list
                InitializeFilterOptions(); // Update filter options
                SelectedDevice = null; // Deselect
                MessageBox.Show("Urządzenie zostało usunięte.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MoveDevice()
        {
            if (CanUpdateOrDeleteDevice())
            {
                if (string.IsNullOrWhiteSpace(SelectedTargetWarehouse))
                {
                    MessageBox.Show("Wybierz docelowy magazyn.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (SelectedTargetWarehouse == SelectedDevice.CurrentWarehouse)
                {
                    MessageBox.Show("Urządzenie już znajduje się w wybranym magazynie docelowym.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var existingDevice = _allDevices.FirstOrDefault(d => d.Id == SelectedDevice.Id);
                if (existingDevice != null)
                {
                    existingDevice.CurrentWarehouse = SelectedTargetWarehouse;
                    existingDevice.Notes = $"{existingDevice.Notes} [Przeniesiono do {SelectedTargetWarehouse} {DateTime.Now.ToString("dd.MM.yyyy")}]";
                    ApplyFilters();
                    InitializeFilterOptions(); // Update available warehouses if new one is added
                    MessageBox.Show($"Urządzenie {existingDevice.SerialNumber} przeniesiono do: {SelectedTargetWarehouse}.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Wybierz urządzenie do przeniesienia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool CanReportRepair()
        {
            return SelectedDevice != null && SelectedDevice.Status != "W naprawie";
        }

        private void ReportRepair()
        {
            if (CanReportRepair())
            {
                var existingDevice = _allDevices.FirstOrDefault(d => d.Id == SelectedDevice.Id);
                if (existingDevice != null)
                {
                    existingDevice.Status = "W naprawie";
                    existingDevice.Notes = $"{existingDevice.Notes} [Zgłoszono do naprawy {DateTime.Now.ToString("dd.MM.yyyy")}]";
                    ApplyFilters();
                    MessageBox.Show($"Urządzenie {existingDevice.SerialNumber} zostało zgłoszone do naprawy.", "Zgłoszono naprawę", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else if (SelectedDevice?.Status == "W naprawie")
            {
                MessageBox.Show("Urządzenie jest już w statusie 'W naprawie'.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Wybierz urządzenie do zgłoszenia naprawy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool CanFinishRepair()
        {
            return SelectedDevice != null && SelectedDevice.Status == "W naprawie";
        }

        private void FinishRepair()
        {
            if (CanFinishRepair())
            {
                var existingDevice = _allDevices.FirstOrDefault(d => d.Id == SelectedDevice.Id);
                if (existingDevice != null)
                {
                    existingDevice.Status = "Dostępny"; // Zmieniamy status na Dostępny
                    existingDevice.Notes = $"{existingDevice.Notes} [Zakończono naprawę {DateTime.Now.ToString("dd.MM.yyyy")}]";
                    ApplyFilters();
                    MessageBox.Show($"Naprawa urządzenia {existingDevice.SerialNumber} została zakończona. Status zmieniono na 'Dostępny'.", "Naprawa zakończona", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else if (SelectedDevice?.Status != "W naprawie")
            {
                MessageBox.Show("Wybrane urządzenie nie jest w statusie 'W naprawie'.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Wybierz urządzenie, aby zakończyć naprawę.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}