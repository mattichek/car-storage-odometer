using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_storage_odometer.Models
{
    public class DeviceLogModel : BindableBase
    {
        public int LogId { get; set; } 

        public int DeviceId { get; set; } 
        public string SerialNumber { get; set; } 
        public string DeviceName { get; set; } 

        public DateTime EventDate { get; set; }

        public string Event { get; set; } 


        public int FromWarehouseId { get; set; }
        public string FromWarehouseName { get; set; }

        public int ToWarehouseId { get; set; }
        public string ToWarehouseName { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }

    }
}
