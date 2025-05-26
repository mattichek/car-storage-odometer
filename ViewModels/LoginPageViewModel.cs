using Prism.Mvvm;

namespace car_storage_odometer.ViewModels
{
    public class LoginPageViewModel : BindableBase
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public LoginPageViewModel()
        {
            Title = "Login Page";
        }
    }
}
