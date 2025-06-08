using System.Threading.Tasks;

namespace car_storage_odometer.Services
{
    public interface IAuthenticationService
    {
        bool IsLoggedIn { get; }
        event System.Action LoggedOut;

        Task<bool> LoginAsync(string email, string password);
        Task Logout();
    }
}