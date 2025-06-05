using car_storage_odometer.Helpers;
using car_storage_odometer.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace car_storage_odometer.ViewModels
{
    public class DevicesViewModel : INotifyPropertyChanged
    {
        // --- Właściwości dla DataGrid i formularza edycji ---
        private ObservableCollection<DeviceModel> _devices;
        public ObservableCollection<DeviceModel> Devices
        {
            get { return _devices; }
            set { _devices = value; OnPropertyChanged(); }
        }

        // Właściwość do przechowywania aktualnie wybranego urządzenia z DataGrid
        // KLUCZOWA ZMIANA: Aktualizuje CurrentEditDevice i stan komend
        private DeviceModel _selectedDevice;
        public DeviceModel SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (SetProperty(ref _selectedDevice, value)) // Używamy SetProperty dla lepszego zarządzania zmianami
                {
                    // KLUCZOWA LOGIKA: Kopiowanie wybranego urządzenia do CurrentEditDevice
                    // Aby panel edycji odzwierciedlał wybór z DataGrid.
                    // Używamy DeepCopy, aby zmiany w formularzu nie wpływały od razu na DataGrid,
                    // dopóki użytkownik nie kliknie "Aktualizuj".
                    CurrentEditDevice = value != null ? value.DeepCopy() : null;

                    // Powiadom komendy o zmianie stanu, aby zaktualizować CanExecute
                    // Wszystkie komendy, których stan zależy od SelectedDevice/CurrentEditDevice
                    RaiseAllCanExecuteChanged();
                }
            }
        }

        // Właściwość do bindowania na formularzu dodawania/edycji (w XAML to było CurrentEditDevice)
        private DeviceModel _currentEditDevice; // Zmieniono nazwę na CurrentEditDevice dla czytelności z XAML
        public DeviceModel CurrentEditDevice
        {
            get { return _currentEditDevice; }
            set
            {
                if (SetProperty(ref _currentEditDevice, value))
                {
                    // Po zmianie CurrentEditDevice, stan komend może się zmienić.
                    RaiseAllCanExecuteChanged();
                }
            }
        }

        // --- Właściwości dla filtrów ---
        private string _filterSerialNumber;
        public string FilterSerialNumber
        {
            get => _filterSerialNumber;
            set
            {
                if (SetProperty(ref _filterSerialNumber, value))
                {
                    ApplyFilters(); // Zastosuj filtry po zmianie numeru seryjnego
                }
            }
        }

        private string _filterDeviceType;
        public string FilterDeviceType
        {
            get => _filterDeviceType;
            set
            {
                if (SetProperty(ref _filterDeviceType, value))
                {
                    ApplyFilters(); // Zastosuj filtry
                }
            }
        }

        private string _filterWarehouse;
        public string FilterWarehouse
        {
            get => _filterWarehouse;
            set
            {
                if (SetProperty(ref _filterWarehouse, value))
                {
                    ApplyFilters(); // Zastosuj filtry
                }
            }
        }

        private string _filterStatus;
        public string FilterStatus
        {
            get => _filterStatus;
            set
            {
                if (SetProperty(ref _filterStatus, value))
                {
                    ApplyFilters(); // Zastosuj filtry
                }
            }
        }


        // --- Kolekcje do wypełniania ComboBoxów (dla filtrów i edycji) ---
        private ObservableCollection<string> _availableDeviceTypes;
        public ObservableCollection<string> AvailableDeviceTypes
        {
            get { return _availableDeviceTypes; }
            set { _availableDeviceTypes = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _availableWarehouses;
        public ObservableCollection<string> AvailableWarehouses
        {
            get { return _availableWarehouses; }
            set { _availableWarehouses = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _availableStatuses;
        public ObservableCollection<string> AvailableStatuses
        {
            get { return _availableStatuses; }
            set { _availableStatuses = value; OnPropertyChanged(); }
        }

        // Dodatkowe kolekcje dla ComboBoxów w panelu edycji (AllPossible...)
        // Zakładam, że są to te same dane co Available..., ale XAML używa innych nazw.
        // Jeśli dane są różne, musisz je ładować osobno.
        public ObservableCollection<string> AllPossibleDeviceTypes => AvailableDeviceTypes;
        public ObservableCollection<string> AllPossibleWarehouses => AvailableWarehouses;
        public ObservableCollection<string> AllPossibleStatuses => AvailableStatuses;

        // Właściwość dla 'Przenieś do magazynu'
        private string _selectedTargetWarehouse;
        public string SelectedTargetWarehouse
        {
            get => _selectedTargetWarehouse;
            set
            {
                if (SetProperty(ref _selectedTargetWarehouse, value))
                {
                    ((RelayCommand)MoveDeviceCommand)?.RaiseCanExecuteChanged();
                }
            }
        }



        // --- Komendy UI ---
        public ICommand NewDeviceCommand { get; private set; }
        public ICommand AddDeviceCommand { get; private set; } // Dodawanie nowego urządzenia do bazy (po kliknięciu "Dodaj")
        public ICommand UpdateDeviceCommand { get; private set; } // Aktualizacja istniejącego urządzenia (po kliknięciu "Aktualizuj")
        public ICommand DeleteDeviceCommand { get; private set; }
        public ICommand ResetFiltersCommand { get; private set; }
        public ICommand MoveDeviceCommand { get; private set; }
        public ICommand ReportRepairCommand { get; private set; }
        public ICommand FinishRepairCommand { get; private set; }


        // Przykładowy ID użytkownika - dostosuj to do swojego systemu logowania
        private const int CurrentUserId = 1;

        // Pełna lista urządzeń załadowana z bazy (do filtrowania)
        private ObservableCollection<DeviceModel> _allDevices;

        public DevicesViewModel()
        {
            _devices = new ObservableCollection<DeviceModel>(); // Inicjalizacja domyślna
            _allDevices = new ObservableCollection<DeviceModel>(); // Inicjalizacja do przechowywania wszystkich danych

            InitializeCommands();
            _ = LoadInitialDataAsync(); // Zmieniono nazwę na LoadInitialDataAsync, aby rozróżnić od ApplyFilters
        }

        private void InitializeCommands()
        {
            NewDeviceCommand = new RelayCommand(ExecuteNewDevice);
            AddDeviceCommand = new RelayCommand(async (obj) => await ExecuteAddDeviceAsync(), CanExecuteAddDevice);
            UpdateDeviceCommand = new RelayCommand(async (obj) => await ExecuteUpdateDeviceAsync(), CanExecuteUpdateDevice);
            DeleteDeviceCommand = new RelayCommand(async (obj) => await ExecuteDeleteDeviceAsync(), CanExecuteOnSelectedDevice);
            ResetFiltersCommand = new RelayCommand(ExecuteResetFilters);
            MoveDeviceCommand = new RelayCommand(async (obj) => await ExecuteMoveDeviceAsync(), CanExecuteMoveDevice);
            ReportRepairCommand = new RelayCommand(async (obj) => await ExecuteReportRepairAsync(), CanExecuteOnSelectedDevice);
            FinishRepairCommand = new RelayCommand(async (obj) => await ExecuteFinishRepairAsync(), CanExecuteOnSelectedDevice);
        }

        // Metoda pomocnicza do wywołania RaiseCanExecuteChanged dla wszystkich komend
        private void RaiseAllCanExecuteChanged()
        {
            ((RelayCommand)NewDeviceCommand)?.RaiseCanExecuteChanged();
            ((RelayCommand)AddDeviceCommand)?.RaiseCanExecuteChanged();
            ((RelayCommand)UpdateDeviceCommand)?.RaiseCanExecuteChanged();
            ((RelayCommand)DeleteDeviceCommand)?.RaiseCanExecuteChanged();
            ((RelayCommand)ResetFiltersCommand)?.RaiseCanExecuteChanged();
            ((RelayCommand)MoveDeviceCommand)?.RaiseCanExecuteChanged();
            ((RelayCommand)ReportRepairCommand)?.RaiseCanExecuteChanged();
            ((RelayCommand)FinishRepairCommand)?.RaiseCanExecuteChanged();
        }

        // Asynchroniczna metoda do ładowania wszystkich danych początkowych
        private async Task LoadInitialDataAsync()
        {
            try
            {
                _allDevices = await SqliteDataAccess.LoadDevicesAsync(); // Ładujemy wszystkie do _allDevices
                Devices = new ObservableCollection<DeviceModel>(_allDevices); // Kopiujemy na start do wyświetlanej listy

                AvailableDeviceTypes = await SqliteDataAccess.LoadDeviceTypesAsync();
                AvailableWarehouses = await SqliteDataAccess.LoadWarehousesAsync();
                AvailableStatuses = await SqliteDataAccess.LoadStatusesAsync();

                // Dodaj opcję "Wszystkie" do filtrów, jeśli to pożądane
                if (AvailableDeviceTypes != null && !AvailableDeviceTypes.Contains("Wszystkie"))
                    AvailableDeviceTypes.Insert(0, "Wszystkie");

                if (AvailableWarehouses != null && !AvailableWarehouses.Contains("Wszystkie"))
                    AvailableWarehouses.Insert(0, "Wszystkie");

                if (AvailableStatuses != null && !AvailableStatuses.Contains("Wszystkie"))
                    AvailableStatuses.Insert(0, "Wszystkie");

                // Domyślnie ustaw filtry na "Wszystkie"
                FilterDeviceType = "Wszystkie";
                FilterWarehouse = "Wszystkie";
                FilterStatus = "Wszystkie";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- Logika filtrowania ---
        private void ApplyFilters()
        {
            if (_allDevices == null) return;

            var filteredDevices = _allDevices.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(FilterSerialNumber))
                filteredDevices = filteredDevices.Where(d => d.SerialNumber.Contains(FilterSerialNumber));

            if (FilterDeviceType != "Wszystkie" && !string.IsNullOrWhiteSpace(FilterDeviceType))
                filteredDevices = filteredDevices.Where(d => d.TypeName == FilterDeviceType);

            if (FilterWarehouse != "Wszystkie" && !string.IsNullOrWhiteSpace(FilterWarehouse))
                filteredDevices = filteredDevices.Where(d => d.WarehouseName == FilterWarehouse);

            if (FilterStatus != "Wszystkie" && !string.IsNullOrWhiteSpace(FilterStatus))
                filteredDevices = filteredDevices.Where(d => d.StatusName == FilterStatus);

            Devices = new ObservableCollection<DeviceModel>(filteredDevices);
            SelectedDevice = null; // Wyczyść zaznaczenie po przefiltrowaniu
        }

        private void ExecuteResetFilters(object parameter)
        {
            FilterSerialNumber = string.Empty;
            FilterDeviceType = "Wszystkie";
            FilterWarehouse = "Wszystkie";
            FilterStatus = "Wszystkie";
            // ApplyFilters() zostanie wywołane automatycznie przez settery
        }

        // --- Logika dla 'Nowe urządzenie' ---
        private void ExecuteNewDevice(object parameter)
        {
            // Przygotuj CurrentEditDevice do wprowadzenia nowego urządzenia
            CurrentEditDevice = new DeviceModel
            {
                EventDate = DateTime.Now,
                TypeName = AvailableDeviceTypes?.FirstOrDefault(t => t != "Wszystkie") ?? string.Empty, // Ustaw pierwszą nie-pustą/nie-wszystkie
                StatusName = AvailableStatuses?.FirstOrDefault(s => s != "Wszystkie") ?? string.Empty,
                WarehouseName = AvailableWarehouses?.FirstOrDefault(w => w != "Wszystkie") ?? string.Empty,
                SerialNumber = string.Empty,
                Note = string.Empty
            };
            SelectedDevice = null; // Upewnij się, że nic nie jest zaznaczone w DataGrid
            RaiseAllCanExecuteChanged(); // Odśwież stan przycisków
        }

        // --- Logika dla 'Dodaj urządzenie' (teraz dedykowana tylko do dodawania) ---
        private async Task ExecuteAddDeviceAsync()
        {
            if (CurrentEditDevice == null || string.IsNullOrWhiteSpace(CurrentEditDevice.SerialNumber))
            {
                MessageBox.Show("Numer seryjny jest wymagany.", "Walidacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CurrentEditDevice.DeviceId != 0) // To nie jest nowe urządzenie, błąd logiki
            {
                MessageBox.Show("To urządzenie już istnieje. Użyj przycisku 'Aktualizuj urządzenie'.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                await SqliteDataAccess.AddDeviceAsync(CurrentEditDevice, CurrentUserId);
                MessageBox.Show("Urządzenie zostało dodane pomyślnie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadInitialDataAsync(); // Odśwież pełną listę i zastosuj filtry
                CurrentEditDevice = null; // Wyczyść formularz
                SelectedDevice = null; // Wyczyść zaznaczenie
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Błąd walidacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas dodawania urządzenia: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Czy można dodać urządzenie (gdy CurrentEditDevice jest ustawione na nowe)
        private bool CanExecuteAddDevice(object parameter)
        {
            // Można dodać, jeśli jest obiekt do edycji, ma numer seryjny i jest to nowe urządzenie (DeviceId == 0)
            return CurrentEditDevice != null && !string.IsNullOrWhiteSpace(CurrentEditDevice.SerialNumber) && CurrentEditDevice.DeviceId == 0;
        }

        // --- Logika dla 'Aktualizuj urządzenie' (teraz dedykowana tylko do aktualizacji) ---
        private async Task ExecuteUpdateDeviceAsync()
        {
            if (CurrentEditDevice == null || CurrentEditDevice.DeviceId == 0) // Nie wybrano urządzenia do aktualizacji lub to nowe
            {
                MessageBox.Show("Wybierz urządzenie do aktualizacji z listy lub użyj 'Dodaj urządzenie' dla nowego.", "Walidacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(CurrentEditDevice.SerialNumber))
            {
                MessageBox.Show("Numer seryjny jest wymagany.", "Walidacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await SqliteDataAccess.UpdateDeviceAsync(CurrentEditDevice);
                MessageBox.Show("Urządzenie zostało zaktualizowane pomyślnie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadInitialDataAsync(); // Odśwież pełną listę i zastosuj filtry
                CurrentEditDevice = null; // Wyczyść formularz
                SelectedDevice = null; // Wyczyść zaznaczenie
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Błąd walidacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas aktualizacji urządzenia: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Czy można zaktualizować urządzenie (gdy CurrentEditDevice to istniejące urządzenie)
        private bool CanExecuteUpdateDevice(object parameter)
        {
            // Można aktualizować, jeśli jest obiekt do edycji, ma numer seryjny i jest to istniejące urządzenie (DeviceId > 0)
            return CurrentEditDevice != null && !string.IsNullOrWhiteSpace(CurrentEditDevice.SerialNumber) && CurrentEditDevice.DeviceId > 0;
        }

        // --- Logika dla 'Usuń urządzenie' ---
        private async Task ExecuteDeleteDeviceAsync()
        {
            if (SelectedDevice == null) return; // Powinno być zablokowane przez CanExecute

            MessageBoxResult result = MessageBox.Show(
                $"Czy na pewno chcesz usunąć urządzenie o numerze seryjnym: {SelectedDevice.SerialNumber}?",
                "Potwierdź usunięcie",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await SqliteDataAccess.DeleteDeviceAsync(SelectedDevice.DeviceId);
                    MessageBox.Show("Urządzenie zostało usunięte pomyślnie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadInitialDataAsync(); // Odśwież listę urządzeń
                    SelectedDevice = null; // Wyczyść zaznaczenie
                    CurrentEditDevice = null; // Wyczyść formularz edycji
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd podczas usuwania urządzenia: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Czy można edytować lub usunąć (czy wybrano urządzenie z listy)
        private bool CanExecuteOnSelectedDevice(object parameter)
        {
            return SelectedDevice != null && SelectedDevice.DeviceId > 0;
        }

        // --- Logika dla 'Przenieś do magazynu' ---
        private async Task ExecuteMoveDeviceAsync()
        {
            if (SelectedDevice == null || SelectedTargetWarehouse == null || SelectedTargetWarehouse == "Wszystkie")
            {
                MessageBox.Show("Wybierz urządzenie do przeniesienia i magazyn docelowy.", "Walidacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Załóżmy, że metoda w SqliteDataAccess potrafi przenieść urządzenie
                // Będziesz potrzebował dostosować tę metodę w swoim SqliteDataAccess
                await SqliteDataAccess.MoveDeviceToWarehouseAsync(SelectedDevice.DeviceId, SelectedTargetWarehouse, CurrentUserId);
                MessageBox.Show($"Urządzenie {SelectedDevice.SerialNumber} przeniesiono do magazynu {SelectedTargetWarehouse}.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadInitialDataAsync(); // Odśwież listę
                SelectedTargetWarehouse = null; // Wyczyść wybór magazynu
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas przenoszenia urządzenia: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanExecuteMoveDevice(object parameter)
        {
           return SelectedDevice != null && SelectedDevice.DeviceId > 0 &&
                   !string.IsNullOrWhiteSpace(SelectedTargetWarehouse) &&
                   SelectedTargetWarehouse != "Wszystkie" &&
                   SelectedTargetWarehouse != SelectedDevice.WarehouseName;
        }


        // --- Logika dla 'Zgłoś naprawę' ---
        private async Task ExecuteReportRepairAsync()
        {
            if (SelectedDevice == null) return;

            try
            {
                // Zakładam, że metoda w SqliteDataAccess aktualizuje status urządzenia na "W naprawie"
                await SqliteDataAccess.UpdateDeviceStatusAsync(SelectedDevice.DeviceId, "W naprawie", CurrentUserId);
                MessageBox.Show($"Urządzenie {SelectedDevice.SerialNumber} zgłoszono do naprawy.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadInitialDataAsync(); // Odśwież listę
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zgłaszania naprawy: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- Logika dla 'Zakończ naprawę' ---
        private async Task ExecuteFinishRepairAsync()
        {
            if (SelectedDevice == null) return;

            try
            {
                // Zakładam, że metoda w SqliteDataAccess aktualizuje status urządzenia na "Dostępny" lub podobny
                await SqliteDataAccess.UpdateDeviceStatusAsync(SelectedDevice.DeviceId, "Dostępny", CurrentUserId);
                MessageBox.Show($"Naprawa urządzenia {SelectedDevice.SerialNumber} została zakończona.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadInitialDataAsync(); // Odśwież listę
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas kończenia naprawy: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- Implementacja INotifyPropertyChanged ---
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Pomocnicza metoda do uproszczenia setterów właściwości
        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, newValue)) return false;
            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}