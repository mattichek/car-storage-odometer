using car_storage_odometer.DataBaseModules;
using car_storage_odometer.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace car_storage_odometer.ViewModels
{
    public class DashboardViewModel : BindableBase
    {

        private ObservableCollection<WarehouseStatusModel> _warehouseStatuses;
        public ObservableCollection<WarehouseStatusModel> WarehouseStatuses
        {
            get => _warehouseStatuses;
            set => SetProperty(ref _warehouseStatuses, value);
        }

        private ObservableCollection<DeviceStatusModel> _deviceStatuses;
        public ObservableCollection<DeviceStatusModel> DeviceStatuses
        {
            get => _deviceStatuses;
            set => SetProperty(ref _deviceStatuses, value);
        }

        private ObservableCollection<UserLogModel> _latestUserLogs;
        public ObservableCollection<UserLogModel> LatestUserLogs
        {
            get => _latestUserLogs;
            set => SetProperty(ref _latestUserLogs, value);
        }

        private ObservableCollection<DeviceLogModel> _latestDeviceLogs;
        public ObservableCollection<DeviceLogModel> LatestDeviceLogs
        {
            get => _latestDeviceLogs;
            set => SetProperty(ref _latestDeviceLogs, value);
        }

        private ObservableCollection<RepairHistoryModel> _latestRepairs;
        public ObservableCollection<RepairHistoryModel> LatestRepairs
        {
            get => _latestRepairs;
            set => SetProperty(ref _latestRepairs, value);
        }


        public DashboardViewModel()
        {
            WarehouseStatuses = new ObservableCollection<WarehouseStatusModel>();
            DeviceStatuses = new ObservableCollection<DeviceStatusModel>();
            LatestUserLogs = new ObservableCollection<UserLogModel>();
            LatestDeviceLogs = new ObservableCollection<DeviceLogModel>();
            LatestRepairs = new ObservableCollection<RepairHistoryModel>();

            _ = LoadDashboardData();
        }

        private async Task LoadDashboardData()
        {
            WarehouseStatuses.Clear();
            WarehouseStatuses = await SqliteDataAccess<WarehouseStatusModel>.LoadQuery("select * from Warehouses");

          
            DeviceStatuses.Clear();
            DeviceStatuses = await SqliteDataAccess<DeviceStatusModel>.LoadQuery(
                @"SELECT 
                    s.Name,
                COUNT(d.DeviceId) AS Quantity
                FROM Devices d
                JOIN Statuses s ON d.StatusId = s.StatusId GROUP BY s.Name;");

            LatestUserLogs.Clear();
            LatestUserLogs = await SqliteDataAccess<UserLogModel>.LoadQuery(
                @"SELECT 
                    l.LogId,
                    u.FirstName || ' ' || u.LastName AS UserName,
                    d.DeviceId, 
                    l.Event, 
                    l.EventDate
                FROM UserLogs l
                LEFT JOIN Users u ON l.UserId = u.UserId
                LEFT JOIN Devices d ON l.DeviceId = d.DeviceId
                ORDER BY l.LogId DESC LIMIT 5;");


            LatestDeviceLogs.Clear();
            LatestDeviceLogs = await SqliteDataAccess<DeviceLogModel>.LoadQuery("" +
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
                ORDER BY dl.LogId DESC LIMIT 5;");

            LatestRepairs.Clear();
            LatestRepairs = await SqliteDataAccess<RepairHistoryModel>.LoadQuery(
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
                LIMIT 5;"
            );
        }
    }
}