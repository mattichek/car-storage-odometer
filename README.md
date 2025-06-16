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
