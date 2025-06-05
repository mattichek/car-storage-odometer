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
                var warehouses = cnn.Query<WarehouseStatusModel>("select * from Warehouses", new DynamicParameters());
                return new ObservableCollection<WarehouseStatusModel>(warehouses);
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
                var statusesQuantity = cnn.Query<DeviceStatusModel>(
                    @"SELECT 
                        s.Name,
                    COUNT(d.DeviceId) AS Quantity
                    FROM Devices d
                    JOIN Statuses s ON d.StatusId = s.StatusId GROUP BY s.Name;",
                    new DynamicParameters());
                return new ObservableCollection<DeviceStatusModel>(statusesQuantity);
            }
        }
        /// <summary>
        /// Retrieves a collection of user logs from the database, including user details, device information, and event
        /// data.
        /// </summary>
        /// <remarks>This method queries the database to load user log entries, combining data from
        /// multiple tables. The returned collection includes details such as the log ID, user name, device ID, event
        /// description, and event date.</remarks>
        /// <returns>An <see cref="ObservableCollection{T}"/> of <see cref="UserLogModel"/> objects representing the user logs.
        /// The collection will be empty if no logs are found.</returns>
        public static ObservableCollection<UserLogModel> LoadUserLogs()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<UserLogModel>(
                        @"SELECT 
                          l.LogId,
                          u.FirstName || ' ' || u.LastName AS UserName,
                          d.DeviceId, 
                          l.Event, 
                          l.EventDate
                      FROM UserLogs l
                      LEFT JOIN Users u ON l.UserId = u.UserId
                      LEFT JOIN Devices d ON l.DeviceId = d.DeviceId;",
                    new DynamicParameters());
                return new ObservableCollection<UserLogModel>(output);
            }
        }
        /// <summary>
        /// Retrieves the repair history records from the database.
        /// </summary>
        /// <remarks>This method queries the database to load repair history information, including repair
        /// details, associated device names, user information, and repair dates. The data is returned as an observable
        /// collection of <see cref="RepairHistoryModel"/> objects.</remarks>
        /// <returns>An <see cref="ObservableCollection{T}"/> containing <see cref="RepairHistoryModel"/> objects that represent
        /// the repair history records. The collection will be empty if no records are found.</returns>
        public static ObservableCollection<RepairHistoryModel> LoadRepairHistory()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var repairHistory = cnn.Query<RepairHistoryModel>(
                    @"SELECT 
                        rh.RepairId,
                        dt.Name AS DeviceName,
                        rh.Description,
                        rh.StartDate,
                        rh.EndDate,
                        u.FirstName || ' ' || u.LastName AS UserName
                    FROM repairhistory rh
                    LEFT JOIN devices d ON rh.DeviceId = d.DeviceId
                    LEFT JOIN devicetypes dt ON d.TypeId = dt.TypeId
                    LEFT JOIN users u ON rh.UserId = u.UserId;",
                    new DynamicParameters());
                return new ObservableCollection<RepairHistoryModel>(repairHistory);
            }
        }
        private static string LoadConnectionString(string id = "DataBase")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
