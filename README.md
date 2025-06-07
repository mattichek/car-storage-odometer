## 📖 Opis projektu

Car Storage Odometer to profesjonalna aplikacja desktopowa stworzona w technologii WPF (.NET 6) do zarządzania magazynem pojazdów z zaawansowanym modułem śledzenia przebiegu (odometru). System umożliwia kompleksowe zarządzanie flotą pojazdów, śledzenie historii przebiegów oraz generowanie raportów.

## ✨ Kluczowe funkcje

- 🚗 **Zarządzanie pojazdami** - dodawanie, edycja i usuwanie pojazdów z magazynu
- 📊 **Śledzenie przebiegu** - rejestracja aktualnego przebiegu i historia odczytów
- 🔍 **Wyszukiwanie i filtrowanie** - szybki dostęp do pojazdów po parametrach
- 📈 **Statystyki i raporty** - analiza danych o przebiegu i stanie floty
- 🛠 **Historia przeglądów** - rejestracja przeglądów i napraw pojazdów
- 💾 **Lokalna baza danych** - przechowywanie danych w SQLite

## 🛠 Stos technologiczny

- **Frontend**: WPF, XAML, Prism, Material Design
- **Backend**: C#, .NET 6
- **Baza danych**: SQLite
- **Wzorce projektowe**: MVVM, Repository, Dependency Injection
- **Narzędzia**: Visual Studio 2022, Git, GitHub
- **Testy**: xUnit, Moq

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
CarStorageOdometer.sln
```

3. Przywróć pakiety NuGet:
```bash
dotnet restore
```

4. Uruchom aplikację:
```bash
dotnet run --project CarStorageOdometer
```

Lub użyj skrótu F5 w Visual Studio

## 🔧 Konfiguracja

Baza danych SQLite jest automatycznie tworzona w ścieżce:
```
%LocalAppData%/CarStorageOdometer/OdometerWarehouse.db
```

Aplikacja nie wymaga dodatkowej konfiguracji do działania podstawowego.

## 📂 Struktura projektu

```
CarStorageOdometer/
└── CarStorageOdometer/          # Główny projekt WPF
    ├── Views/                   # Widoki XAML
    ├── Converters/              # Konwertery wykoszystywane w XAML
    ├── DataBaseModules/         # Klasy odnoszące się do komunikacji z bazą danych
    ├── Helpers/                 # Klasy pomocnicze
    ├── Modules/                 # Klasy moduły 
    ├── Styles/                  # Style wykorzystywane w XAML
    ├── ViewModels/              # ViewModels z logiką prezentacji
    ├── Assets/                  # Zasoby graficzne
    ├── App.xaml                 # Konfiguracja aplikacji
    └── Models/                  # Encje domenowe
```

## 👥 Autorzy

- Mateusz Zaskórski - [GitHub](https://github.com/mattichek)
- Jakub Kwaśniewski - [GitHub](https://github.com/miszczur)
