using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using car_storage_odometer.DataBaseModules; // Zakładam, że tu masz dostęp do bazy
// using car_storage_odometer.Models; // Jeśli masz model użytkownika

namespace car_storage_odometer.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public bool IsLoggedIn { get; private set; } = false;
        public event System.Action LoggedOut;

        public async Task<bool> LoginAsync(string email, string password)
        {
            // TODO: Zaimplementuj prawdziwą logikę weryfikacji z bazą danych
            // Przykład:
            // var user = await SqliteDataAccess.GetUserByEmail(email);
            // if (user != null && VerifyPassword(password, user.HashedPassword, user.Salt)) { ... }
            await Task.Delay(500); // Symulacja opóźnienia

            if ((email == "test@test.com" && password == "123456") || (email == "111" && password == "111"))
            {
                IsLoggedIn = true;
                // Możesz tu przechowywać dane zalogowanego użytkownika (np. w globalnym obiekcie sesji)
                return true;
            }
            IsLoggedIn = false;
            return false;
        }

        public void Logout()
        {
            IsLoggedIn = false;
            // Tutaj możesz również wyczyścić wszelkie globalne dane sesji użytkownika
            LoggedOut?.Invoke(); // Wywołaj event, aby inni mogli zareagować
        }

        // Helper do haszowania (możesz go mieć w innym miejscu)
        private string ComputeSha256(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}