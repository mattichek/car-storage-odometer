using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using car_storage_odometer.Models;
using car_storage_odometer.Services; // Dodaj przestrzeń nazw dla UserLogFilterService

namespace car_storage_odometer.ViewModels
{
    public class LogsUsersViewModel : BindableBase
    {
        private ObservableCollection<UserLogModel> _allUserLogs;
        private ObservableCollection<UserLogModel> _latestUserLogs;
        public ObservableCollection<UserLogModel> LatestUserLogs
        {
            get => _latestUserLogs;
            set => SetProperty(ref _latestUserLogs, value);
        }

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

        private UserLogModel _selectedUserFilter;
        public UserLogModel SelectedUserFilter
        {
            get => _selectedUserFilter;
            set => SetProperty(ref _selectedUserFilter, value);
        }

        private ObservableCollection<UserLogModel> _availableUsers;
        public ObservableCollection<UserLogModel> AvailableUsers
        {
            get => _availableUsers;
            set => SetProperty(ref _availableUsers, value);
        }

        private string _selectedActionFilter;
        public string SelectedActionFilter
        {
            get => _selectedActionFilter;
            set => SetProperty(ref _selectedActionFilter, value);
        }

        private ObservableCollection<string> _availableActions;
        public ObservableCollection<string> AvailableActions
        {
            get => _availableActions;
            set => SetProperty(ref _availableActions, value);
        }

        public DelegateCommand FilterLogsCommand { get; private set; }
        public DelegateCommand ResetFiltersCommand { get; private set; }

        // Instancja klasy pomocniczej
        private readonly UserLogFilterService _filterService;

        public LogsUsersViewModel(UserLogFilterService filterService)
        {
            // Inicjalizacja serwisu filtrującego
            _filterService = filterService;

            LoadDummyData();
            InitializeFilterOptions();

            // Użyj serwisu do początkowego załadowania logów
            LatestUserLogs = _filterService.ResetFilters(_allUserLogs);

            FilterLogsCommand = new DelegateCommand(ApplyFilter);
            ResetFiltersCommand = new DelegateCommand(ResetFilters);
        }

        private void LoadDummyData()
        {
            _allUserLogs = new ObservableCollection<UserLogModel>
            {
                new UserLogModel { UserLogId = 1, UserId = 1, UserName = "Jan Kowalski", Action = "Zalogował się", EventDate = new DateTime(2025, 05, 31, 08, 55, 0) },
                new UserLogModel { UserLogId = 2, UserId = 2, UserName = "Anna Nowak", Action = "Dodała licznik 'Licznik-001'", EventDate = new DateTime(2025, 05, 31, 07, 55, 0) },
                new UserLogModel { UserLogId = 3, UserId = 3, UserName = "Piotr Zieliński", Action = "Wylogował się", EventDate = new DateTime(2025, 05, 31, 06, 55, 0) },
                new UserLogModel { UserLogId = 4, UserId = 1, UserName = "Jan Kowalski", Action = "Usunął moduł 'Moduł-ABC'", EventDate = new DateTime(2025, 05, 31, 05, 55, 0) },
                new UserLogModel { UserLogId = 5, UserId = 2, UserName = "Anna Nowak", Action = "Zalogowała się", EventDate = new DateTime(2025, 05, 31, 04, 55, 0) },
                new UserLogModel { UserLogId = 6, UserId = 4, UserName = "Alicja Nowak", Action = "Zalogowała się", EventDate = new DateTime(2025, 05, 30, 09, 00, 0) },
                new UserLogModel { UserLogId = 7, UserId = 1, UserName = "Jan Kowalski", Action = "Zmienił licznik 'Licznik-002'", EventDate = new DateTime(2025, 05, 30, 10, 15, 0) },
                new UserLogModel { UserLogId = 8, UserId = 3, UserName = "Piotr Zieliński", Action = "Dodał moduł 'Moduł-XYZ'", EventDate = new DateTime(2025, 05, 29, 11, 30, 0) }
            };
        }

        private void InitializeFilterOptions()
        {
            AvailableUsers = new ObservableCollection<UserLogModel>(_allUserLogs
                                                                     .GroupBy(log => log.UserName)
                                                                     .Select(g => new UserLogModel { UserName = g.Key })
                                                                     .OrderBy(u => u.UserName));
            AvailableActions = new ObservableCollection<string>(_allUserLogs.Select(log => log.Action).Distinct().OrderBy(a => a));
        }

        // Teraz metoda ApplyFilter tylko wywołuje serwis
        private void ApplyFilter()
        {
            LatestUserLogs = _filterService.ApplyFilter(
                _allUserLogs,
                FilterDateFrom,
                FilterDateTo,
                SelectedUserFilter,
                SelectedActionFilter
            );
        }

        // Teraz metoda ResetFilters tylko wywołuje serwis i resetuje właściwości ViewModelu
        private void ResetFilters()
        {
            FilterDateFrom = null;
            FilterDateTo = null;
            SelectedUserFilter = null;
            SelectedActionFilter = null;

            LatestUserLogs = _filterService.ResetFilters(_allUserLogs);
        }
    }
}