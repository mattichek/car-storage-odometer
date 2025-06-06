using car_storage_odometer.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_storage_odometer.DataBaseModules
{
    public class SqliteDataAccessModifyingQuery
    {
        /// <summary>
        /// Retrieves the connection string associated with the specified identifier from the application's
        /// configuration file.
        /// </summary>
        /// <param name="id">The identifier of the connection string to retrieve. Defaults to "DataBase" if no value is provided.</param>
        /// <returns>The connection string corresponding to the specified identifier.</returns>
        private static string LoadConnectionString(string id = "DataBase")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public static async Task AddUserLogAsync(int userId, string action)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string insertSql = @"
            INSERT INTO UserLogs (UserId, Event, EventDate)
            VALUES (@UserId, @Event, @EventDate);";

                await cnn.ExecuteAsync(insertSql, new
                {
                    UserId = userId,
                    Event = action,
                    EventDate = DateTime.Now
                });
            }
        }

        public static async Task AddDeviceLogAsync(int userId, int deviceId, string eventDescription, int? toWarehouseId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();

                int? fromWarehouseId = await cnn.ExecuteScalarAsync<int?>(
                    "SELECT WarehouseId FROM Devices WHERE DeviceId = @DeviceId",
                    new { DeviceId = deviceId });

                string insertSql = @"
                    INSERT INTO DeviceLogs (UserId, DeviceId, Event, EventDate, FromWarehouseId, ToWarehouseId)
                    VALUES (@UserId, @DeviceId, @Event, @EventDate, @FromWarehouseId, @ToWarehouseId);";

                await cnn.ExecuteAsync(insertSql, new
                {
                    UserId = userId,
                    DeviceId = deviceId,
                    Event = eventDescription,
                    EventDate = DateTime.Now,
                    FromWarehouseId = fromWarehouseId,
                    ToWarehouseId = toWarehouseId
                });
            }
        }

        public static async Task AddRepairHistoryAsync(int deviceId, string description, int userId, DateTime? endDate = null)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();

                string insertSql = @"
            INSERT INTO RepairHistory (DeviceId, Description, StartDate, EndDate, UserId)
            VALUES (@DeviceId, @Description, @StartDate, @EndDate, @UserId);";

                await cnn.ExecuteAsync(insertSql, new
                {
                    DeviceId = deviceId,
                    Description = description,
                    StartDate = DateTime.Now,
                    EndDate = endDate,
                    UserId = userId
                });
            }
        }

        public static async Task UpdateDeviceAsync(DeviceModel device)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        // 1. Sprawdź, czy ten numer seryjny nie należy do innego urządzenia
                        var existingDeviceId = await cnn.ExecuteScalarAsync<int?>(
                            "SELECT DeviceId FROM SerialNumbers WHERE SerialNumber = @SerialNumber",
                            new { device.SerialNumber }, transaction);

                        if (existingDeviceId != null && existingDeviceId != device.DeviceId)
                        {
                            throw new InvalidOperationException($"Numer seryjny '{device.SerialNumber}' jest już przypisany do innego urządzenia.");
                        }

                        // 2. Pobierz ID typu, statusu, magazynu
                        int typeId = await cnn.ExecuteScalarAsync<int>(
                            "SELECT TypeId FROM DeviceTypes WHERE Name = @Name",
                            new { Name = device.TypeName }, transaction);

                        int statusId = await cnn.ExecuteScalarAsync<int>(
                            "SELECT StatusId FROM Statuses WHERE Name = @Name",
                            new { Name = device.StatusName }, transaction);

                        int warehouseId = await cnn.ExecuteScalarAsync<int>(
                            "SELECT WarehouseId FROM Warehouses WHERE Name = @Name",
                            new { Name = device.WarehouseName }, transaction);

                        // 3. Aktualizacja rekordu w Devices
                        string updateDeviceSql = @"
                    UPDATE Devices
                    SET TypeId = @TypeId,
                        StatusId = @StatusId,
                        WarehouseId = @WarehouseId,
                        Note = @Note,
                        EventDate = @EventDate
                    WHERE DeviceId = @DeviceId;";

                        await cnn.ExecuteAsync(updateDeviceSql, new
                        {
                            TypeId = typeId,
                            StatusId = statusId,
                            WarehouseId = warehouseId,
                            device.Note,
                            device.EventDate,
                            DeviceId = device.DeviceId
                        }, transaction);

                        // 4. Aktualizacja numeru seryjnego
                        string updateSerialSql = @"
                    UPDATE SerialNumbers
                    SET SerialNumber = @SerialNumber
                    WHERE DeviceId = @DeviceId;";

                        await cnn.ExecuteAsync(updateSerialSql, new
                        {
                            SerialNumber = device.SerialNumber,
                            DeviceId = device.DeviceId
                        }, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Błąd podczas aktualizacji urządzenia: " + ex.Message, ex);
                    }
                }
            }
        }

        public static async Task DeleteDeviceAsync(int deviceId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        // 1. Usuń powiązany numer seryjny
                        string deleteSerialSql = "DELETE FROM SerialNumbers WHERE DeviceId = @DeviceId;";
                        await cnn.ExecuteAsync(deleteSerialSql, new { DeviceId = deviceId }, transaction);

                        // 2. Usuń urządzenie
                        string deleteDeviceSql = "DELETE FROM Devices WHERE DeviceId = @DeviceId;";
                        await cnn.ExecuteAsync(deleteDeviceSql, new { DeviceId = deviceId }, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Błąd podczas usuwania urządzenia: " + ex.Message, ex);
                    }
                }
            }
        }


        public static async Task MoveDeviceToWarehouseAsync(int deviceId, string newWarehouseName)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        // 1. Pobierz ID nowego magazynu
                        int newWarehouseId = await cnn.ExecuteScalarAsync<int>(
                            "SELECT WarehouseId FROM Warehouses WHERE Name = @Name",
                            new { Name = newWarehouseName }, transaction);

                        // 2. Zaktualizuj magazyn urządzenia
                        string updateSql = @"
                    UPDATE Devices
                    SET WarehouseId = @WarehouseId
                    WHERE DeviceId = @DeviceId;";

                        await cnn.ExecuteAsync(updateSql, new
                        {
                            WarehouseId = newWarehouseId,
                            DeviceId = deviceId
                        }, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Błąd podczas przenoszenia urządzenia do nowego magazynu: " + ex.Message, ex);
                    }
                }
            }
        }

        public static async Task ResetDeviceAfterRepairAsync(int deviceId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        // Pobierz najniższy StatusId
                        int? defaultStatusId = await cnn.ExecuteScalarAsync<int?>(
                            "SELECT MIN(StatusId) FROM Statuses", transaction);

                        // Pobierz najniższy WarehouseId
                        int? defaultWarehouseId = await cnn.ExecuteScalarAsync<int?>(
                            "SELECT MIN(WarehouseId) FROM Warehouses", transaction);

                        if (defaultStatusId == null || defaultWarehouseId == null)
                        {
                            throw new InvalidOperationException("Nie znaleziono domyślnego statusu lub magazynu.");
                        }

                        // Aktualizacja statusu i magazynu w jednej operacji
                        string updateSql = @"
                    UPDATE Devices
                    SET StatusId = @StatusId,
                        WarehouseId = @WarehouseId
                    WHERE DeviceId = @DeviceId;";

                        await cnn.ExecuteAsync(updateSql, new
                        {
                            StatusId = defaultStatusId.Value,
                            WarehouseId = defaultWarehouseId.Value,
                            DeviceId = deviceId
                        }, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Błąd podczas resetowania urządzenia po naprawie: " + ex.Message, ex);
                    }
                }
            }
        }

        public static async Task ReportDeviceForRepairAsync(int deviceId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        // 1. Pobierz StatusId dla statusu "W naprawie"
                        int? repairStatusId = await cnn.ExecuteScalarAsync<int?>(
                            "SELECT StatusId FROM Statuses WHERE Name = 'W naprawie';", transaction);

                        if (repairStatusId == null)
                        {
                            throw new InvalidOperationException("Nie znaleziono statusu 'W naprawie'.");
                        }

                        // 2. Pobierz ostatni (największy) WarehouseId
                        int? lastWarehouseId = await cnn.ExecuteScalarAsync<int?>(
                            "SELECT MAX(WarehouseId) FROM Warehouses;", transaction);

                        if (lastWarehouseId == null)
                        {
                            throw new InvalidOperationException("Nie znaleziono żadnego magazynu.");
                        }

                        // 3. Aktualizuj status i magazyn w tabeli Devices
                        string updateSql = @"
                    UPDATE Devices
                    SET StatusId = @StatusId,
                        WarehouseId = @WarehouseId
                    WHERE DeviceId = @DeviceId;";

                        await cnn.ExecuteAsync(updateSql, new
                        {
                            StatusId = repairStatusId.Value,
                            WarehouseId = lastWarehouseId.Value,
                            DeviceId = deviceId
                        }, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Błąd podczas zgłaszania naprawy: " + ex.Message, ex);
                    }
                }
            }
        }

        public static async Task<ObservableCollection<DeviceModel>> LoadDevicesAsync()
        {
            return await Task.Run(() =>
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    var output = cnn.Query<DeviceModel>(
                        @"SELECT 
                            d.DeviceId,
                            sn.SerialNumber,
                            d.TypeId,
                            dt.Name AS TypeName,
                            d.StatusId,
                            s.Name AS StatusName,
                            d.WarehouseId,
                            w.Name AS WarehouseName,
                            d.UserId,
                            u.FirstName || ' ' || u.LastName AS UserName,
                            d.Note,
                            d.EventDate
                        FROM devices d
                        LEFT JOIN serialnumbers sn ON d.DeviceId = sn.DeviceId
                        LEFT JOIN devicetypes dt ON d.TypeId = dt.TypeId
                        LEFT JOIN statuses s ON d.StatusId = s.StatusId
                        LEFT JOIN warehouses w ON d.WarehouseId = w.WarehouseId
                        LEFT JOIN users u ON d.UserId = u.UserId;"
                        , new DynamicParameters());
                    return new ObservableCollection<DeviceModel>(output);
                }
            });
        }

        // --- Metody dla DeviceModel ---
        public static async Task AddDeviceAsync(DeviceModel device, int userId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        // 1. Sprawdzenie czy numer seryjny istnieje
                        bool serialExists = await cnn.ExecuteScalarAsync<bool>(
                            "SELECT COUNT(1) FROM SerialNumbers WHERE SerialNumber = @SerialNumber",
                            new { device.SerialNumber }, transaction);

                        if (serialExists)
                        {
                            throw new InvalidOperationException($"Numer seryjny '{device.SerialNumber}' już istnieje.");
                        }

                        // 2. Pobierz ID typu, statusu i magazynu
                        int typeId = await cnn.ExecuteScalarAsync<int>(
                            "SELECT TypeId FROM DeviceTypes WHERE Name = @Name",
                            new { Name = device.TypeName }, transaction);

                        int statusId = await cnn.ExecuteScalarAsync<int>(
                            "SELECT StatusId FROM Statuses WHERE Name = @Name",
                            new { Name = device.StatusName }, transaction);

                        int warehouseId = await cnn.ExecuteScalarAsync<int>(
                            "SELECT WarehouseId FROM Warehouses WHERE Name = @Name",
                            new { Name = device.WarehouseName }, transaction);

                        // 3. Wstawienie urządzenia
                        string insertDeviceSql = @"
                                INSERT INTO Devices (TypeId, StatusId, WarehouseId, UserId, Note, EventDate)
                                VALUES (@TypeId, @StatusId, @WarehouseId, @UserId, @Note, @EventDate);
                                SELECT last_insert_rowid();";

                        int newDeviceId = await cnn.ExecuteScalarAsync<int>(insertDeviceSql, new
                        {
                            TypeId = typeId,
                            StatusId = statusId,
                            WarehouseId = warehouseId,
                            UserId = userId,
                            device.Note,
                            device.EventDate
                        }, transaction);

                        // 4. Dodanie numeru seryjnego
                        string insertSerialSql = @"
                                INSERT INTO SerialNumbers (DeviceId, SerialNumber)
                                VALUES (@DeviceId, @SerialNumber);";

                        await cnn.ExecuteAsync(insertSerialSql, new
                        {
                            DeviceId = newDeviceId,
                            device.SerialNumber
                        }, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Błąd przy dodawaniu urządzenia: " + ex.Message, ex);
                    }
                }
            }
        }

        public static async Task<ObservableCollection<string>> LoadDeviceTypesAsync()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var types = await cnn.QueryAsync<string>("SELECT Name FROM devicetypes ORDER BY Name");
                return new ObservableCollection<string>(types);
            }
        }

        public static async Task<ObservableCollection<string>> LoadWarehousesAsync()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var warehouses = await cnn.QueryAsync<string>("SELECT Name FROM warehouses ORDER BY Name");
                return new ObservableCollection<string>(warehouses);
            }
        }

        public static async Task<ObservableCollection<string>> LoadStatusesAsync()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var statuses = await cnn.QueryAsync<string>("SELECT Name FROM statuses ORDER BY Name");
                return new ObservableCollection<string>(statuses);
            }
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
                var output = cnn.Query<DeviceLogModel>(""
                    , new DynamicParameters());
                return new ObservableCollection<DeviceLogModel>(output);
            }
        }
    }
}
