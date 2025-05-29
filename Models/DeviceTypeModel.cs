using Prism.Mvvm;

namespace car_storage_odometer.Models
{
    public class DeviceTypeModel : BindableBase
    {
        private int _typeId;
        public int TypeId // odpowiada typ_id
        {
            get => _typeId;
            set => SetProperty(ref _typeId, value);
        }

        private string _typeName;
        public string TypeName // odpowiada nazwa
        {
            get => _typeName;
            set => SetProperty(ref _typeName, value);
        }

        private string _description;
        public string Description // odpowiada opis
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
    }
}
