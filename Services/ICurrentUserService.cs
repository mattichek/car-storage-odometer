namespace car_storage_odometer.Services
{
    public interface ICurrentUserService
    {
        int? LoggedInUserId { get; } 
        bool IsUserLoggedIn { get; }
        void SetLoggedInUserId(int userId);
        void ClearLoggedInUser();
    }
}