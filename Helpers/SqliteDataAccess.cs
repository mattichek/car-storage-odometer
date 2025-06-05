using car_storage_odometer.Models;
using Dapper;
using System;
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
                        bool serialNumberExists = await DoesSerialNumberExistAsync(device.SerialNumber, null);
                        if (serialNumberExists)
                        {
                            throw new InvalidOperationException("Numer seryjny już istnieje.");
                        }

                        // Kwerenda 1: Wstaw do tabeli 'devices'
                        // Używamy EventDate z modelu, które mapuje na kolumnę EventDate w bazie
                        string sqlInsertDevice = @"
                            INSERT INTO devices (TypeId, StatusId, WarehouseId, UserId, EventDate, Note) -- Usunięto EntryDate, dodano EventDate
                            VALUES ((SELECT TypeId FROM devicetypes WHERE Name = @TypeName),
                                    (SELECT StatusId FROM statuses WHERE Name = @StatusName),
                                    (SELECT WarehouseId FROM warehouses WHERE Name = @WarehouseName),
                                    @UserId,
                                    @EventDate, @Note); -- Używamy @EventDate
                            SELECT last_insert_rowid();";

                        long newDeviceId = await cnn.ExecuteScalarAsync<long>(sqlInsertDevice,
                            new
                            {
                                TypeName = device.TypeName,
                                StatusName = device.StatusName,
                                WarehouseName = device.WarehouseName,
                                UserId = userId,
                                EventDate = device.EventDate, // Używamy EventDate
                                Note = device.Note
                            }, transaction);

                        // Kwerenda 2: Wstaw do tabeli 'serialnumbers'
                        // ReleaseDate z serialnumbers będzie mapowane na EventDate z DeviceModel
                        string sqlInsertSerialNumber = @"
                            INSERT INTO serialnumbers (DeviceId, SerialNumber, ReleaseDate)
                            VALUES (@DeviceId, @SerialNumber, @ReleaseDate);";
                        await cnn.ExecuteAsync(sqlInsertSerialNumber,
                            new
                            {
                                DeviceId = newDeviceId,
                                SerialNumber = device.SerialNumber,
                                ReleaseDate = device.EventDate // Mapujemy EventDate na ReleaseDate
                            }, transaction);

                        transaction.Commit();
                        device.DeviceId = (int)newDeviceId;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
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
                        bool serialNumberExists = await DoesSerialNumberExistAsync(device.SerialNumber, device.DeviceId);
                        if (serialNumberExists)
                        {
                            throw new InvalidOperationException("Numer seryjny jest już używany przez inne urządzenie.");
                        }

                        // Kwerenda 1: UPDATE devices
                        // Zakładam, że kolumny Note ORAZ EventDate są w tabeli devices
                        string sqlUpdateDevice = @"
                            UPDATE devices
                            SET TypeId = (SELECT TypeId FROM devicetypes WHERE Name = @TypeName),
                                StatusId = (SELECT StatusId FROM statuses WHERE Name = @StatusName),
                                WarehouseId = (SELECT WarehouseId FROM warehouses WHERE Name = @WarehouseName),
                                Note = @Note,
                                EventDate = @EventDate -- Zmienione z EntryDate na EventDate
                            WHERE DeviceId = @DeviceId;";

                        await cnn.ExecuteAsync(sqlUpdateDevice,
                            new
                            {
                                TypeName = device.TypeName,
                                StatusName = device.StatusName,
                                WarehouseName = device.WarehouseName,
                                Note = device.Note,
                                EventDate = device.EventDate, // Używamy EventDate
                                DeviceId = device.DeviceId
                            }, transaction);

                        // Kwerenda 2: UPDATE serialnumbers
                        // ReleaseDate z serialnumbers będzie mapowane na EventDate z DeviceModel
                        string sqlUpdateSerialNumber = @"
                            UPDATE serialnumbers
                            SET SerialNumber = @SerialNumber,
                                ReleaseDate = @ReleaseDate -- Zmienione z EntryDate na ReleaseDate
                            WHERE DeviceId = @DeviceId;";
                        await cnn.ExecuteAsync(sqlUpdateSerialNumber,
                            new
                            {
                                SerialNumber = device.SerialNumber,
                                ReleaseDate = device.EventDate, // Mapujemy EventDate na ReleaseDate
                                DeviceId = device.DeviceId
                            }, transaction);

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
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
                        await cnn.ExecuteAsync("DELETE FROM repairhistory WHERE DeviceId = @DeviceId", new { DeviceId = deviceId }, transaction);
                        await cnn.ExecuteAsync("DELETE FROM devicelogs WHERE DeviceId = @DeviceId", new { DeviceId = deviceId }, transaction);
                        await cnn.ExecuteAsync("DELETE FROM serialnumbers WHERE DeviceId = @DeviceId", new { DeviceId = deviceId }, transaction);
                        await cnn.ExecuteAsync("DELETE FROM devices WHERE DeviceId = @DeviceId", new { DeviceId = deviceId }, transaction);

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static async Task<bool> DoesSerialNumberExistAsync(string serialNumber, int? excludeDeviceId = null)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string sql = "SELECT COUNT(1) FROM serialnumbers WHERE SerialNumber = @SerialNumber COLLATE NOCASE";
                if (excludeDeviceId.HasValue)
                {
                    sql += " AND DeviceId != @ExcludeDeviceId";
                }

                var parameters = new DynamicParameters();
                parameters.Add("SerialNumber", serialNumber);
                if (excludeDeviceId.HasValue)
                {
                    parameters.Add("ExcludeDeviceId", excludeDeviceId.Value);
                }

                int count = await cnn.QuerySingleOrDefaultAsync<int>(sql, parameters);
                return count > 0;
            }
        }

        // Metoda AddDeviceLogAsync (nie zmieniona w tej modyfikacji, bo już używała EventDate)


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

        // --- NOWE METODY DO IMPLEMENTACJI ---

        // Metoda do przenoszenia urządzenia do innego magazynu
        public static async Task MoveDeviceToWarehouseAsync(int deviceId, string newWarehouseName, int userId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                // Sprawdź, czy magazyn docelowy istnieje i pobierz jego ID
                int newWarehouseId = await cnn.ExecuteScalarAsync<int?>(
                    "SELECT WarehouseId FROM Warehouses WHERE WarehouseName = @NewWarehouseName",
                    new { NewWarehouseName = newWarehouseName }
                ) ?? throw new InvalidOperationException($"Magazyn '{newWarehouseName}' nie istnieje.");

                // Aktualizuj WarehouseId dla danego urządzenia
                string sql = @"
                    UPDATE Devices
                    SET WarehouseId = @NewWarehouseId,
                        EventDate = @CurrentDate, -- Opcjonalnie aktualizuj datę wydarzenia
                        Note = 'Przeniesiono do nowego magazynu', -- Opcjonalnie dodaj notatkę
                        LastModifiedByUserId = @UserId,
                        LastModifiedDate = @CurrentDate
                    WHERE DeviceId = @DeviceId";

                await cnn.ExecuteAsync(sql, new
                {
                    NewWarehouseId = newWarehouseId,
                    DeviceId = deviceId,
                    CurrentDate = DateTime.Now,
                    UserId = userId
                });

                // Opcjonalnie: możesz dodać wpis do tabeli historii, jeśli śledzisz ruchy urządzeń
                // string historySql = @"INSERT INTO DeviceHistory (DeviceId, EventType, OldValue, NewValue, EventDate, UserId)
                //                      VALUES (@DeviceId, 'Przeniesienie', @OldWarehouseId, @NewWarehouseId, @CurrentDate, @UserId)";
                // await cnn.ExecuteAsync(historySql, new { DeviceId = deviceId, ... });
            }
        }

        // Metoda do aktualizacji statusu urządzenia
        public static async Task UpdateDeviceStatusAsync(int deviceId, string newStatusName, int userId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                // Sprawdź, czy status docelowy istnieje i pobierz jego ID
                int newStatusId = await cnn.ExecuteScalarAsync<int?>(
                    "SELECT StatusId FROM Statuses WHERE StatusName = @NewStatusName",
                    new { NewStatusName = newStatusName }
                ) ?? throw new InvalidOperationException($"Status '{newStatusName}' nie istnieje.");

                // Aktualizuj StatusId dla danego urządzenia
                string sql = @"
                    UPDATE Devices
                    SET StatusId = @NewStatusId,
                        EventDate = @CurrentDate, -- Opcjonalnie aktualizuj datę wydarzenia
                        LastModifiedByUserId = @UserId,
                        LastModifiedDate = @CurrentDate
                    WHERE DeviceId = @DeviceId";

                await cnn.ExecuteAsync(sql, new
                {
                    NewStatusId = newStatusId,
                    DeviceId = deviceId,
                    CurrentDate = DateTime.Now,
                    UserId = userId
                });

                // Opcjonalnie: możesz dodać wpis do tabeli historii
                // string historySql = @"INSERT INTO DeviceHistory (DeviceId, EventType, OldValue, NewValue, EventDate, UserId)
                //                      VALUES (@DeviceId, 'Zmiana statusu', @OldStatusId, @NewStatusId, @CurrentDate, @UserId)";
                // await cnn.ExecuteAsync(historySql, new { DeviceId = deviceId, ... });
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
