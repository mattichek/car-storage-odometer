using car_storage_odometer.DataBaseModules;
using car_storage_odometer.Helpers;
using car_storage_odometer.Models;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
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
    public class DevicesViewModel : INotifyPropertyChanged, INavigationAware
    {
        private readonly IDialogService _dialogService;
        private ObservableCollection<DeviceModel> _allDevices;

        private ObservableCollection<DeviceModel> _devices;
        public ObservableCollection<DeviceModel> Devices
        {
            get => _devices;
            set 
            { 
                _devices = value; 
                OnPropertyChanged(); 
            }
        }

        private DeviceModel _selectedDevice;
        public DeviceModel SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (SetProperty(ref _selectedDevice, value)) 
                {
                    CurrentEditDevice = value != null ? value.DeepCopy() : null;
                    RaiseAllCanExecuteChanged();
                }
            }
        }

        private DeviceModel _currentEditDevice; 
        public DeviceModel CurrentEditDevice
        {
            get => _currentEditDevice;
            set
            {
                if (SetProperty(ref _currentEditDevice, value))
                    RaiseAllCanExecuteChanged();
            }
        }

        private string _filterSerialNumber;
        public string FilterSerialNumber
        {
            get => _filterSerialNumber;
            set
            {
                if (SetProperty(ref _filterSerialNumber, value))
                {
                    ApplyFilters();
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
                    ApplyFilters();
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
                    ApplyFilters();
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
                    ApplyFilters();
                }
            }
        }


        private ObservableCollection<string> _availableDeviceTypes;
        public ObservableCollection<string> AvailableDeviceTypes
        {
            get => _availableDeviceTypes;
            set { _availableDeviceTypes = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _availableWarehouses;
        public ObservableCollection<string> AvailableWarehouses
        {
            get => _availableWarehouses;
            set { _availableWarehouses = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _availableStatuses;
        public ObservableCollection<string> AvailableStatuses
        {
            get => _availableStatuses;
            set { _availableStatuses = value; OnPropertyChanged(); }
        }
        public ObservableCollection<string> AllPossibleDeviceTypes => AvailableDeviceTypes;
        public ObservableCollection<string> AllPossibleWarehouses => AvailableWarehouses;
        public ObservableCollection<string> AllPossibleStatuses => AvailableStatuses;

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

        public ICommand NewDeviceCommand { get; private set; }
        public ICommand AddDeviceCommand { get; private set; } 
        public ICommand UpdateDeviceCommand { get; private set; } 
        public ICommand DeleteDeviceCommand { get; private set; }
        public ICommand ResetFiltersCommand { get; private set; }
        public ICommand MoveDeviceCommand { get; private set; }
        public ICommand ReportRepairCommand { get; private set; }
        public ICommand FinishRepairCommand { get; private set; }


        private const int CurrentUserId = 1;

        public DevicesViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            NewDeviceCommand = new RelayCommand(ExecuteNewDevice);
            AddDeviceCommand = new RelayCommand(async (obj) => await ExecuteAddDeviceAsync(), CanExecuteAddDevice);
            UpdateDeviceCommand = new RelayCommand(async (obj) => await ExecuteUpdateDeviceAsync(), CanExecuteUpdateDevice);
            DeleteDeviceCommand = new RelayCommand(async (obj) => await ExecuteDeleteDeviceAsync(), CanExecuteOnSelectedDevice);
            ResetFiltersCommand = new RelayCommand(ExecuteResetFilters);
            MoveDeviceCommand = new RelayCommand(async (obj) => await ExecuteMoveDeviceAsync(), CanExecuteMoveDevice);
            ReportRepairCommand = new RelayCommand(async (obj) => await ExecuteReportRepairAsync(), CanExecuteReportRepair);
            FinishRepairCommand = new RelayCommand(async (obj) => await ExecuteFinishRepairAsync(), CanExecuteFinishRepair);

            _devices = new ObservableCollection<DeviceModel>();
            _allDevices = new ObservableCollection<DeviceModel>();
        }

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

        private async Task LoadInitialDataAsync()
        {
            try
            {
                _allDevices = await SqliteDataAccessModifyingQuery.LoadDevicesAsync();
                Devices = new ObservableCollection<DeviceModel>(_allDevices);

                AvailableDeviceTypes = await SqliteDataAccessModifyingQuery.LoadDeviceTypesAsync();
                AvailableWarehouses = await SqliteDataAccessModifyingQuery.LoadWarehousesAsync();
                AvailableStatuses = await SqliteDataAccessModifyingQuery.LoadStatusesAsync();

                if (AvailableDeviceTypes != null && !AvailableDeviceTypes.Contains("Wszystkie"))
                    AvailableDeviceTypes.Insert(0, "Wszystkie");

                if (AvailableWarehouses != null && !AvailableWarehouses.Contains("Wszystkie"))
                    AvailableWarehouses.Insert(0, "Wszystkie");

                if (AvailableStatuses != null && !AvailableStatuses.Contains("Wszystkie"))
                    AvailableStatuses.Insert(0, "Wszystkie");

                FilterDeviceType = "Wszystkie";
                FilterWarehouse = "Wszystkie";
                FilterStatus = "Wszystkie";

            }
            catch (Exception ex)
            {
                ShowMessageBoxOk($"Błąd ładowania danych: {ex.Message}", "Błąd");

            }
        }

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
            SelectedDevice = null;
        }

        private void ExecuteResetFilters(object parameter)
        {
            FilterSerialNumber = string.Empty;
            FilterDeviceType = "Wszystkie";
            FilterWarehouse = "Wszystkie";
            FilterStatus = "Wszystkie";
        }

        private void ExecuteNewDevice(object parameter)
        {
            CurrentEditDevice = new DeviceModel
            {
                EventDate = DateTime.Now,
                TypeName = AvailableDeviceTypes?.FirstOrDefault(t => t != "Wszystkie") ?? string.Empty,
                StatusName = AvailableStatuses?.FirstOrDefault(s => s != "Wszystkie") ?? string.Empty,
                WarehouseName = AvailableWarehouses?.FirstOrDefault(w => w != "Wszystkie") ?? string.Empty,
                SerialNumber = string.Empty,
                Note = string.Empty
            };
            SelectedDevice = null;
            RaiseAllCanExecuteChanged();
        }

        private async Task ExecuteAddDeviceAsync()
        {
            if (CurrentEditDevice == null || string.IsNullOrWhiteSpace(CurrentEditDevice.SerialNumber))
            {
                ShowMessageBoxOk("Numer seryjny jest wymagany.", "Walidacja");
                return;
            }

            if (CurrentEditDevice.DeviceId != 0)
            {
                ShowMessageBoxOk("To urządzenie już istnieje. Użyj przycisku 'Aktualizuj urządzenie'.", "Błąd");
                return;
            }

            try
            {
                await SqliteDataAccessModifyingQuery.AddDeviceAsync(CurrentEditDevice, CurrentUserId);
                await SqliteDataAccessModifyingQuery.AddUserLogAsync(CurrentUserId, "Dodano urządznie");

                ShowMessageBoxOk("Urządzenie zostało dodane pomyślnie.", "Sukces");

                await LoadInitialDataAsync();
                CurrentEditDevice = null;
                SelectedDevice = null;
            }
            catch (InvalidOperationException ex)
            {
                ShowMessageBoxOk($"Błąd walidacji: {ex.Message}", "Błąd");
            }
            catch (Exception ex)
            {
                ShowMessageBoxOk($"Wystąpił błąd podczas dodawania urządzenia: {ex.Message}", "Błąd");
            }
        }

        private bool CanExecuteAddDevice(object parameter) 
            => CurrentEditDevice != null && !string.IsNullOrWhiteSpace(CurrentEditDevice.SerialNumber) && CurrentEditDevice.DeviceId == 0;

        private async Task ExecuteUpdateDeviceAsync()
        {
            if (CurrentEditDevice == null || CurrentEditDevice.DeviceId == 0)
            {
                ShowMessageBoxOk("Wybierz urządzenie do aktualizacji z listy lub użyj 'Dodaj urządzenie' dla nowego.", "Walidacja");
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrentEditDevice.SerialNumber))
            {
                ShowMessageBoxOk("Numer seryjny jest wymagany.", "Walidacja");
                return;
            }

            try
            {
                await SqliteDataAccessModifyingQuery.UpdateDeviceAsync(CurrentEditDevice);
                await SqliteDataAccessModifyingQuery.AddUserLogAsync(CurrentUserId, "Zaktualizowano status urządzenia");
                await SqliteDataAccessModifyingQuery.AddDeviceLogAsync(CurrentUserId, CurrentEditDevice.DeviceId, "Zaktualizowano status urządzenia", CurrentEditDevice.WarehouseId);

                ShowMessageBoxOk("Urządzenie zostało zaktualizowane pomyślnie.", "Sukces");

                await LoadInitialDataAsync();
                CurrentEditDevice = null;
                SelectedDevice = null;
            }
            catch (InvalidOperationException ex)
            {
                ShowMessageBoxOk($"Błąd walidacji: {ex.Message}", "Błąd");
            }
            catch (Exception ex)
            {
                ShowMessageBoxOk($"Wystąpił błąd podczas aktualizacji urządzenia: {ex.Message}", "Błąd");
            }

        }
        private bool CanExecuteUpdateDevice(object parameter)
            => CurrentEditDevice != null && !string.IsNullOrWhiteSpace(CurrentEditDevice.SerialNumber) && CurrentEditDevice.DeviceId > 0;

        private async Task ExecuteDeleteDeviceAsync()
        {
            if (SelectedDevice == null)
                return;

            bool confirm = await ShowMessageButtonYesNo(
                $"Czy na pewno chcesz usunąć urządzenie o numerze seryjnym: {SelectedDevice.SerialNumber}?",
                "Potwierdź usunięcie");

            if (!confirm)
                return;

            try
            {
                await SqliteDataAccessModifyingQuery.DeleteDeviceAsync(SelectedDevice.DeviceId);
                await SqliteDataAccessModifyingQuery.AddUserLogAsync(CurrentUserId, "Usunięto urządzenie");
                await SqliteDataAccessModifyingQuery.AddDeviceLogAsync(CurrentUserId, CurrentEditDevice.DeviceId, "Usunięto urządzenie", null);

                ShowMessageBoxOk("Urządzenie zostało usunięte pomyślnie.", "Sukces");

                await LoadInitialDataAsync();
                SelectedDevice = null;
                CurrentEditDevice = null;
            }
            catch (Exception ex)
            {
                ShowMessageBoxOk($"Wystąpił błąd podczas usuwania urządzenia: {ex.Message}", "Błąd");
            }
        }


        private bool CanExecuteOnSelectedDevice(object parameter)
            => SelectedDevice != null && SelectedDevice.DeviceId > 0;

        private async Task ExecuteMoveDeviceAsync()
        {
            if (SelectedDevice == null || SelectedTargetWarehouse == null || SelectedTargetWarehouse == "Wszystkie")
            {

                ShowMessageBoxOk("Wybierz urządzenie do przeniesienia i magazyn docelowy.", "Walidacja");
                return;
            }

            try
            {
                await SqliteDataAccessModifyingQuery.MoveDeviceToWarehouseAsync(SelectedDevice.DeviceId, SelectedTargetWarehouse);
                await SqliteDataAccessModifyingQuery.AddUserLogAsync(CurrentUserId, "Przeniesiono urządznie między magazynami");
                await SqliteDataAccessModifyingQuery.AddDeviceLogAsync(CurrentUserId, CurrentEditDevice.DeviceId, "Przeniesiono urządznie między magazynami", CurrentEditDevice.WarehouseId);


                ShowMessageBoxOk($"Urządzenie {SelectedDevice.SerialNumber} przeniesiono do magazynu {SelectedTargetWarehouse}.", "Sukces");
                await LoadInitialDataAsync();
                SelectedTargetWarehouse = null;
            }
            catch (Exception ex)
            {
                ShowMessageBoxOk($"Błąd podczas przenoszenia urządzenia: {ex.Message}", "Błąd");
            }
        }

        private bool CanExecuteMoveDevice(object parameter)
            => SelectedDevice != null && SelectedDevice.DeviceId > 0 &&
                   !string.IsNullOrWhiteSpace(SelectedTargetWarehouse) &&
                   SelectedTargetWarehouse != "Wszystkie" &&
                   SelectedTargetWarehouse != SelectedDevice.WarehouseName;


        private async Task ExecuteReportRepairAsync()
        {
            if (SelectedDevice == null) return;

            try
            {
                await SqliteDataAccessModifyingQuery.ReportDeviceForRepairAsync(SelectedDevice.DeviceId);
                await SqliteDataAccessModifyingQuery.AddUserLogAsync(CurrentUserId, "Zgłoszono naprawę");
                await SqliteDataAccessModifyingQuery.AddDeviceLogAsync(CurrentUserId, CurrentEditDevice.DeviceId, "Zgłoszono naprawę", CurrentEditDevice.WarehouseId);
                await SqliteDataAccessModifyingQuery.AddRepairHistoryAsync(SelectedDevice.DeviceId, "Naprawa rozpoczęta", CurrentUserId);
                ShowMessageBoxOk($"Urządzenie {SelectedDevice.SerialNumber} zgłoszono do naprawy.", "Sukces");
                await LoadInitialDataAsync(); 
            }
            catch (Exception ex)
            {
                ShowMessageBoxOk($"Błąd podczas zgłaszania naprawy: {ex.Message}", "Błąd");
            }
        }

        private async Task ExecuteFinishRepairAsync()
        {
            if (SelectedDevice == null) return;

            try
            {
                await SqliteDataAccessModifyingQuery.ResetDeviceAfterRepairAsync(SelectedDevice.DeviceId);
                await SqliteDataAccessModifyingQuery.AddUserLogAsync(CurrentUserId, "Zakończono naprawę");
                await SqliteDataAccessModifyingQuery.AddDeviceLogAsync(CurrentUserId, CurrentEditDevice.DeviceId, "Zakończono naprawę", CurrentEditDevice.WarehouseId);
                await SqliteDataAccessModifyingQuery.AddRepairHistoryAsync(SelectedDevice.DeviceId, "Naprawa zakończona", CurrentUserId, DateTime.Now);
                ShowMessageBoxOk($"Naprawa urządzenia {SelectedDevice.SerialNumber} została zakończona.", "Sukces");
                await LoadInitialDataAsync();
            }
            catch (Exception ex)
            {
                ShowMessageBoxOk($"Błąd podczas kończenia naprawy: {ex.Message}", "Błąd");
            }
        }

        private bool CanExecuteReportRepair(object parameter)
        {
            return SelectedDevice != null &&
                   SelectedDevice.StatusName != "W naprawie"; 
        }

        private bool CanExecuteFinishRepair(object parameter)
        {
            return SelectedDevice != null &&
                   SelectedDevice.StatusName == "W naprawie";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, newValue)) return false;
            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            // Load data when the view is navigated to
            await LoadInitialDataAsync();
            // Clear any previous selection or edit state when navigating to the view
            SelectedDevice = null;
            CurrentEditDevice = null;
            SelectedTargetWarehouse = null;
            RaiseAllCanExecuteChanged();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            // Always return true to reuse the existing instance of DevicesViewModel
            // This prevents unnecessary reloads of data and maintains the state (filters, etc.)
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Devices.Clear();
            _allDevices.Clear(); 
            SelectedDevice = null; // Clear selected device
            CurrentEditDevice = null; // Clear current edit device
            SelectedTargetWarehouse = null; // Clear selected target warehouse
            FilterSerialNumber = string.Empty;
            FilterDeviceType = "Wszystkie";
            FilterWarehouse = "Wszystkie";
            FilterStatus = "Wszystkie";
            RaiseAllCanExecuteChanged();
        }

        private void ShowMessageBoxOk(string message, string title)
        {
            _dialogService.ShowDialog("CustomMessageBoxView",
                new DialogParameters
                {
            { "message", message },
            { "title", title },
            { "buttons", CustomMessageBoxButtons.Ok }
                },
                r => { /* brak akcji po OK */ });
        }
        private Task<bool> ShowMessageButtonYesNo(string message, string title)
        {
            var tcs = new TaskCompletionSource<bool>();

            _dialogService.ShowDialog("CustomMessageBoxView",
                new DialogParameters
                {
            { "message", message },
            { "title", title },
            { "buttons", CustomMessageBoxButtons.YesNo}
                },
                r => tcs.SetResult(r.Result == ButtonResult.Yes));

            return tcs.Task;
        }
    }
}