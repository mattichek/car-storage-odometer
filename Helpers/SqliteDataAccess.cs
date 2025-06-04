using car_storage_odometer.Models;
using Dapper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace car_storage_odometer.Helpers
{
    public class SqliteDataAccess
    {

        public static ObservableCollection<WarehouseStatusModel> LoadX()
        {
            using(IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<WarehouseStatusModel>("select * from Warehouses", new DynamicParameters());
                return new ObservableCollection<WarehouseStatusModel>(output);
            }
        }
        
        public static ObservableCollection<DeviceLogModel> LoadDevicesLogs()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<DeviceLogModel>("SELECT dl.LogId, sn.SerialNumber, dt.Name AS DeviceName, dl.EventDate, dl.Event, w1.Name AS FromWarehouseName, w2.Name AS ToWarehouseName, u.FirstName || ' ' || u.LastName AS UserName FROM DeviceLogs dl JOIN Devices d ON dl.DeviceId = d.DeviceId JOIN SerialNumbers sn ON d.DeviceId = sn.DeviceId JOIN DeviceTypes dt ON d.TypeId = dt.TypeId LEFT JOIN Warehouses w1 ON dl.FromWarehouseId = w1.WarehouseId LEFT JOIN Warehouses w2 ON dl.ToWarehouseId = w2.WarehouseId JOIN Users u ON dl.UserId = u.UserId;"
                    , new DynamicParameters());
                return new ObservableCollection<DeviceLogModel>(output);
            }
        }

        public static async Task<ObservableCollection<DeviceLogModel>> LoadDevicesLogsAsync()
        {
            return await Task.Run(() =>
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    var output = cnn.Query<DeviceLogModel>("SELECT dl.LogId, sn.SerialNumber, dt.Name AS DeviceName, dl.EventDate, dl.Event, w1.Name AS FromWarehouseName, w2.Name AS ToWarehouseName, u.FirstName || ' ' || u.LastName AS UserName FROM DeviceLogs dl JOIN Devices d ON dl.DeviceId = d.DeviceId JOIN SerialNumbers sn ON d.DeviceId = sn.DeviceId JOIN DeviceTypes dt ON d.TypeId = dt.TypeId LEFT JOIN Warehouses w1 ON dl.FromWarehouseId = w1.WarehouseId LEFT JOIN Warehouses w2 ON dl.ToWarehouseId = w2.WarehouseId JOIN Users u ON dl.UserId = u.UserId;"
                        , new DynamicParameters());
                    return new ObservableCollection<DeviceLogModel>(output);
                }
            });
        }

        public static async Task<ObservableCollection<DeviceLogModel>> LoadDevicesLastFiveLogsAsync()
        {
            return await Task.Run(() =>
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    var output = cnn.Query<DeviceLogModel>("SELECT dl.LogId, sn.SerialNumber, dt.Name AS DeviceName, dl.EventDate, dl.Event, w1.Name AS FromWarehouseName, w2.Name AS ToWarehouseName, u.FirstName || ' ' || u.LastName AS UserName FROM DeviceLogs dl JOIN Devices d ON dl.DeviceId = d.DeviceId JOIN SerialNumbers sn ON d.DeviceId = sn.DeviceId JOIN DeviceTypes dt ON d.TypeId = dt.TypeId LEFT JOIN Warehouses w1 ON dl.FromWarehouseId = w1.WarehouseId LEFT JOIN Warehouses w2 ON dl.ToWarehouseId = w2.WarehouseId JOIN Users u ON dl.UserId = u.UserId ORDER BY dl.LogId DESC LIMIT 5;"
                        , new DynamicParameters());
                    return new ObservableCollection<DeviceLogModel>(output);
                }
            });
        }


        public static ObservableCollection<DeviceLogModel> LoadDevicesLogsLastFiveLogs()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<DeviceLogModel>("SELECT dl.LogId, sn.SerialNumber, dt.Name AS DeviceName, dl.EventDate, dl.Event, w1.Name AS FromWarehouseName, w2.Name AS ToWarehouseName, u.FirstName || ' ' || u.LastName AS UserName FROM DeviceLogs dl JOIN Devices d ON dl.DeviceId = d.DeviceId JOIN SerialNumbers sn ON d.DeviceId = sn.DeviceId JOIN DeviceTypes dt ON d.TypeId = dt.TypeId LEFT JOIN Warehouses w1 ON dl.FromWarehouseId = w1.WarehouseId LEFT JOIN Warehouses w2 ON dl.ToWarehouseId = w2.WarehouseId JOIN Users u ON dl.UserId = u.UserId ORDER BY dl.LogId DESC LIMIT 5;"
                    , new DynamicParameters());
                return new ObservableCollection<DeviceLogModel>(output);
            }
        }


        private static string LoadConnectionString(string id = "DataBase")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
