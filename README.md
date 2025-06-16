## 📖 Opis projektu

Car Storage Odometer to profesjonalna aplikacja desktopowa stworzona w technologii WPF (.NET 6) do zarządzania magazynem pojazdów z zaawansowanym modułem śledzenia przebiegu (odometru). System umożliwia kompleksowe zarządzanie flotą pojazdów, śledzenie historii przebiegów oraz generowanie raportów.

## ✨ Kluczowe funkcje

- 🚗 **Zarządzanie pojazdami** - dodawanie, edycja i usuwanie urządzeń z magazynu
- 📊 **Śledzenie poczynań użytkowników z urządzeniami** - rejestracja aktualnego stanu urządzeń 
- 🔍 **Wyszukiwanie i filtrowanie** - szybki dostęp do urządzeń po parametrach
- 🛠 **Historia napraw** - rejestracja napraw urządzeń
- 💾 **Lokalna baza danych** - przechowywanie danych w SQLite

## 🛠 Stos technologiczny

- **Frontend**: WPF, XAML, Prism, Material Design
- **Backend**: C#, .NET 6
- **Baza danych**: SQLite
- **Wzorce projektowe**: MVVM, Repository, Dependency Injection
- **Narzędzia**: Visual Studio 2022, Git, GitHub

## 💻 Wymagania systemowe

- System operacyjny: Windows 10/11
- .NET Runtime 6.0 lub nowszy
- Zalecane 4 GB RAM
- 100 MB wolnego miejsca na dysku

## ⚙️ Instalacja i uruchomienie

1. Sklonuj repozytorium:
```bash
git clone https://github.com/mattichek/car-storage-odometer.git
```

2. Otwórz rozwiązanie w Visual Studio 2022:
```bash
car-storage-odometer.sln
```


Aplikacja nie wymaga dodatkowej konfiguracji do działania podstawowego.

## 📂 Struktura projektu

```
car-storage-odometer/
└── car-storage-odometer/             # Główny projekt WPF
    ├── Assets/                     # Zasoby graficzne (np. ikony)
    │   └── logo_icon.ico
    ├── Converters/                 # Konwertery wykorzystywane w XAML
    │   ├── BooleanToVisibilityConverter.cs
    │   ├── PercentageToWidthConverter.cs
    │   └── SortDirectionToArrowConverter.cs
    ├── DataBaseModules/            # Klasy odnoszące się do komunikacji z bazą danych
    │   ├── SqliteDataAccess.cs
    │   ├── SqliteDataAccessModifyingQuery.cs
    │   └── SqliteQuery.cs
    ├── Events/                     # Klasy zdarzeń
    │   └── LogoutEvent.cs
    ├── Helpers/                    # Klasy pomocnicze
    │   ├── CustomMessageBox.cs
    │   ├── CustomMessageBoxButtons.cs
    │   ├── PasswordBoxHelper.cs
    │   └── RelayCommand.cs
    ├── Models/                     # Encje domenowe
    │   ├── DeviceLogModel.cs
    │   ├── DeviceModel.cs
    │   ├── DeviceStatusModel.cs
    │   ├── RepairHistoryModel.cs
    │   ├── UserLogModel.cs
    │   ├── UserModel.cs
    │   └── WarehouseStatusModel.cs
    ├── Modules/                    # Klasy moduły
    │   └── SideBarModule.cs
    ├── Services/                   # Klasy serwisowe
    │   ├── AuthenticationService.cs
    │   ├── CurrentUserService.cs
    │   ├── DeviceLogFilterService.cs
    │   ├── IAuthenticationService.cs
    │   ├── ICurrentUserService.cs
    │   ├── IDeviceLogFilterService.cs
    │   └── IUserLogFilterService.cs
    ├── Styles/                     # Style wykorzystywane w XAML
    │   ├── DarkThemeStyles.xaml
    │   └── DashboardStyles.xaml
    ├── ViewModels/                 # ViewModels z logiką prezentacji
    │   ├── AccountViewModel.cs
    │   ├── CustomMessageBoxViewModel.cs
    │   ├── DashboardViewModel.cs
    │   ├── DashboardWithSideBarViewModel.cs
    │   ├── DevicesViewModel.cs
    │   ├── HistoryOfRepairViewModel.cs
    │   ├── LoginPageViewModel.cs
    │   ├── LogsDeviceViewModel.cs
    │   ├── LogsUsersViewModel.cs
    │   ├── MainPageViewModell.cs
    │   └── SideBarViewModel.cs
    ├── Views/                      # Widoki XAML
    │   ├── AccountView.xaml
    │   ├── CustomMessageBoxView.xaml
    │   ├── DashboardView.xaml
    │   ├── DashboardWithSideBarView.xaml
    │   ├── DevicesView.xaml
    │   ├── HistoryOfRepairView.xaml
    │   ├── LoginPageView.xaml
    │   ├── LogsDeviceView.xaml
    │   ├── LogsUsersView.xaml
    │   ├── MainPageView.xaml
    │   └── SideBarView.xaml
    ├── App.config                  # Plik konfiguracji aplikacji
    ├── App.xaml                    # Konfiguracja aplikacji
    ├── OdometerWarehouse.db        # Baza danych SQLite
    └── packages.config             # Konfiguracja pakietów NuGet
```

## 👥 Autorzy

- Mateusz Zaskórski - [GitHub](https://github.com/mattichek)
- Jakub Kwaśniewski - [GitHub](https://github.com/miszczur)
