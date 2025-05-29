using Prism.Mvvm;

namespace car_storage_odometer.Models
{
    public class StatusModel : BindableBase
    {
        private int _statusId;
        public int StatusId // odpowiada status_id
        {
            get => _statusId;
            set => SetProperty(ref _statusId, value);
        }

        private string _name;
        public string Name // odpowiada nazwa
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _description;
        public string Description // odpowiada opis
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
    }
}
