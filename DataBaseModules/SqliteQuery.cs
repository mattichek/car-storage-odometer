namespace car_storage_odometer.DataBaseModules
{
    public class SqliteQuery
    {
        public static readonly string LoadWarehouseStatusesQuery = @"SELECT * from Warehouses";
        
        public static readonly string LoadDeviceStatusesQuery = 
            @"SELECT
                s.Name,
                COUNT(d.DeviceId) AS Quantity
            FROM Devices d
            JOIN Statuses s ON d.StatusId = s.StatusId 
            GROUP BY s.Name;";

        public static readonly string LoadLatestUserLogsQuery =
            @"SELECT
                l.LogId,
                u.FirstName || ' ' || u.LastName AS UserName,
                d.DeviceId,
                l.Event,
                l.EventDate
            FROM UserLogs l
            LEFT JOIN Users u ON l.UserId = u.UserId
            LEFT JOIN Devices d ON l.DeviceId = d.DeviceId
            ORDER BY l.LogId DESC 
            LIMIT 5;";
        
        public static readonly string LoadLatestDeviceLogsQuery =
            @"SELECT
                dl.LogId,
                sn.SerialNumber,
                dt.Name AS DeviceName,
                dl.EventDate,
                dl.Event,
                w1.Name AS FromWarehouseName,
                w2.Name AS ToWarehouseName,
                u.FirstName || ' ' || u.LastName AS UserName
            FROM DeviceLogs dl
            JOIN Devices d ON dl.DeviceId = d.DeviceId
            JOIN SerialNumbers sn ON d.DeviceId = sn.DeviceId
            JOIN DeviceTypes dt ON d.TypeId = dt.TypeId
            LEFT JOIN Warehouses w1 ON dl.FromWarehouseId = w1.WarehouseId
            LEFT JOIN Warehouses w2 ON dl.ToWarehouseId = w2.WarehouseId
            JOIN Users u ON dl.UserId = u.UserId
            ORDER BY dl.LogId DESC 
            LIMIT 5;";

        public static readonly string LoadLatestRepairsQuery =
            @"SELECT
                rh.RepairId,
                sn.SerialNumber AS SerialNumber,
                rh.Description,
                rh.StartDate,
                rh.EndDate,
                u.FirstName || ' ' || u.LastName AS UserName
            FROM repairhistory rh
            LEFT JOIN devices d ON rh.DeviceId = d.DeviceId
            LEFT JOIN serialnumbers sn ON d.DeviceId = sn.DeviceId
            LEFT JOIN users u ON rh.UserId = u.UserId
            ORDER BY rh.RepairId DESC
            LIMIT 5;";


        public static readonly string LoadAllUserLogsQuery =
            @"SELECT 
                l.LogId,
                u.FirstName || ' ' || u.LastName AS UserName,
                d.DeviceId, 
                l.Event, 
                l.EventDate
            FROM UserLogs l
            LEFT JOIN Users u ON l.UserId = u.UserId
            LEFT JOIN Devices d ON l.DeviceId = d.DeviceId;";

        public static readonly string LoadAllDeviceLogsQuery = 
            @"SELECT 
                dl.LogId, 
                sn.SerialNumber, 
                dt.Name AS DeviceName, 
                dl.EventDate, 
                dl.Event, 
                w1.Name AS FromWarehouseName, 
                w2.Name AS ToWarehouseName, 
                u.FirstName || ' ' || u.LastName AS UserName 
            FROM DeviceLogs dl JOIN Devices d ON dl.DeviceId = d.DeviceId 
            JOIN SerialNumbers sn ON d.DeviceId = sn.DeviceId 
            JOIN DeviceTypes dt ON d.TypeId = dt.TypeId 
            LEFT JOIN Warehouses w1 ON dl.FromWarehouseId = w1.WarehouseId 
            LEFT JOIN Warehouses w2 ON dl.ToWarehouseId = w2.WarehouseId 
            JOIN Users u ON dl.UserId = u.UserId;";

        public static readonly string LoadHistroyOfRepairQuery =
            @"SELECT 
                rh.RepairId,
                sn.SerialNumber AS SerialNumber,
                rh.Description,
                rh.StartDate,
                rh.EndDate,
                u.FirstName || ' ' || u.LastName AS UserName
            FROM repairhistory rh
            LEFT JOIN devices d ON rh.DeviceId = d.DeviceId
            LEFT JOIN serialnumbers sn ON d.DeviceId = sn.DeviceId
            LEFT JOIN users u ON rh.UserId = u.UserId;";

        public static readonly string LoadAllDevicesQuery =
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
            LEFT JOIN users u ON d.UserId = u.UserId;";

        public static readonly string LoadDeviceTypesAsyncQuery = @"SELECT Name FROM devicetypes ORDER BY Name";
        public static readonly string LoadWarehousesAsyncQuery = @"SELECT Name FROM warehouses ORDER BY Name";
        public static readonly string LoadStatusesAsyncQuery = @"SELECT Name FROM statuses ORDER BY Name";

    }
}
