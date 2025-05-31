using car_storage_odometer.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Prism.Mvvm; // Potrzebne dla BindableBase
using Prism.Commands; // Potrzebne dla DelegateCommand

namespace car_storage_odometer.ViewModels
{
    public class LogsUsersViewModel : BindableBase
    {
        private ObservableCollection<UserLogModel> _userLogs;
        public ObservableCollection<UserLogModel> UserLogs
        {
            get { return _userLogs; }
            set
            {
                // Używamy SetProperty z BindableBase
                SetProperty(ref _userLogs, value);

                // Kiedy bazowa kolekcja (UserLogs) się zmienia, musimy zaktualizować CollectionView
                // ICollectionView nie ma właściwości 'Source'. Zamiast tego, uzyskujemy nową widok.
                if (_userLogs != null) // Sprawdzamy, czy nowa kolekcja nie jest null
                {
                    UserLogsView = CollectionViewSource.GetDefaultView(_userLogs);
                    // Ponownie zastosuj sortowanie po zmianie kolekcji
                    ApplySort();
                }
                else
                {
                    UserLogsView = null; // Jeśli kolekcja jest null, wyczyść widok
                }
            }
        }

        public ICollectionView UserLogsView { get; private set; }

        private string _selectedSortProperty;
        public string SelectedSortProperty
        {
            get { return _selectedSortProperty; }
            set
            {
                SetProperty(ref _selectedSortProperty, value); // Używamy SetProperty
                ApplySort();
            }
        }

        private ListSortDirection _sortDirection = ListSortDirection.Descending; // Domyślnie malejąco dla daty
        public ListSortDirection SortDirection
        {
            get { return _sortDirection; }
            set
            {
                SetProperty(ref _sortDirection, value); // Używamy SetProperty
                ApplySort();
            }
        }

        public DelegateCommand ToggleSortDirectionCommand { get; }

        public LogsUsersViewModel()
        {
            // Inicjalizujemy UserLogs pustą kolekcją, co automatycznie ustawi UserLogsView
            UserLogs = new ObservableCollection<UserLogModel>();

            LoadDummyData(); // Ładowanie przykładowych danych (spowoduje ponowne wywołanie settera UserLogs)
            ToggleSortDirectionCommand = new DelegateCommand(ToggleSortDirection);

            // Domyślne sortowanie zostanie zastosowane przez setter UserLogs po LoadDummyData()
            // Możemy jednak ustawić tutaj domyślne wartości, które będą użyte w ApplySort()
            SelectedSortProperty = nameof(UserLogModel.EventDate);
            SortDirection = ListSortDirection.Descending;
            ApplySort(); // Ponowne wywołanie, aby upewnić się, że sortowanie jest aktywne po załadowaniu danych.
        }

        private void LoadDummyData()
        {
            // Tworzymy nową kolekcję i przypisujemy ją do UserLogs.
            // Setter UserLogs zajmie się zaktualizowaniem UserLogsView.
            ObservableCollection<UserLogModel> dummyData = new ObservableCollection<UserLogModel>();
            dummyData.Add(new UserLogModel { UserLogId = 1, UserId = 1, UserName = "admin", Action = "Login", EventDate = DateTime.Now.AddHours(-2) });
            dummyData.Add(new UserLogModel { UserLogId = 2, UserId = 2, UserName = "user1", Action = "ViewCars", EventDate = DateTime.Now.AddHours(-1) });
            dummyData.Add(new UserLogModel { UserLogId = 3, UserId = 1, UserName = "admin", Action = "AddCar", EventDate = DateTime.Now.AddMinutes(-30) });
            dummyData.Add(new UserLogModel { UserLogId = 4, UserId = 3, UserName = "user2", Action = "Logout", EventDate = DateTime.Now.AddMinutes(-10) });
            dummyData.Add(new UserLogModel { UserLogId = 5, UserId = 1, UserName = "admin", Action = "EditCar", EventDate = DateTime.Now.AddMinutes(-5) });
            dummyData.Add(new UserLogModel { UserLogId = 6, UserId = 4, UserName = "guest", Action = "Search", EventDate = DateTime.Now.AddHours(-3) });
            dummyData.Add(new UserLogModel { UserLogId = 7, UserId = 2, UserName = "user1", Action = "UpdateProfile", EventDate = DateTime.Now.AddMinutes(-45) });
            dummyData.Add(new UserLogModel { UserLogId = 8, UserId = 1, UserName = "admin", Action = "DeleteCar", EventDate = DateTime.Now.AddMinutes(-20) });

            UserLogs = dummyData; // To wywoła setter UserLogs i zaktualizuje UserLogsView
        }

        private void ApplySort()
        {
            if (UserLogsView != null && !string.IsNullOrEmpty(SelectedSortProperty))
            {
                UserLogsView.SortDescriptions.Clear();
                UserLogsView.SortDescriptions.Add(new SortDescription(SelectedSortProperty, SortDirection));
                UserLogsView.Refresh();
            }
        }

        private void ToggleSortDirection()
        {
            SortDirection = (SortDirection == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending;
        }

        public string[] SortProperties { get; } = new string[]
        {
            nameof(UserLogModel.UserLogId),
            nameof(UserLogModel.UserId),
            nameof(UserLogModel.UserName),
            nameof(UserLogModel.Action),
            nameof(UserLogModel.EventDate)
        };
    }
}