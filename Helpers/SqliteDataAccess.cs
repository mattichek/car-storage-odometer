using car_storage_odometer.Models;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Windows.Documents;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Linq;
using System.Collections.ObjectModel;

namespace car_storage_odometer.Helpers
{
    public class SqliteDataAccess
    {
        /// <summary>
        /// Retrieves a collection of warehouse status records from the database.
        /// </summary>
        /// <remarks>This method queries the database for all records in the "Warehouses" table and
        /// returns them as an observable collection of <see cref="WarehouseStatusModel"/> objects. The returned
        /// collection can be used for data binding in UI frameworks that support observable collections.</remarks>
        /// <returns>An <see cref="ObservableCollection{T}"/> containing <see cref="WarehouseStatusModel"/> objects representing
        /// the status of each warehouse. The collection will be empty if no records are found.</returns>
        public static ObservableCollection<WarehouseStatusModel> LoadWarehouses()
        {
            using(IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<WarehouseStatusModel>("select * from Warehouses", new DynamicParameters());
                return new ObservableCollection<WarehouseStatusModel>(output);
            }
        }
        /// <summary>
        /// Retrieves a collection of device statuses along with the quantity of devices associated with each status.
        /// </summary>
        /// <remarks>This method queries the database to group devices by their status and calculates the
        /// count of devices for each status. The result is returned as an observable collection of <see
        /// cref="DeviceStatusModel"/> objects.</remarks>
        /// <returns>An <see cref="ObservableCollection{T}"/> of <see cref="DeviceStatusModel"/> objects, where each object
        /// contains the name of a status and the quantity of devices associated with that status.</returns>
        public static ObservableCollection<DeviceStatusModel> LoadStatusesQuantity()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<DeviceStatusModel>(
                    @"SELECT s.Name, COUNT(d.DeviceId) AS Quantity FROM Devices d JOIN Statuses s ON d.StatusId = s.StatusId GROUP BY s.Name;",
                    new DynamicParameters());
                return new ObservableCollection<DeviceStatusModel>(output);
            }
        }

        private static string LoadConnectionString(string id = "DataBase")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
