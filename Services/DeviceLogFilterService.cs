using car_storage_odometer.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace car_storage_odometer.Services
{
    public class DeviceLogFilterService
    {
        public ObservableCollection<DeviceLogModel> ApplyFilter(
            ObservableCollection<DeviceLogModel> allLogs,
            DateTime? filterDateFrom,
            DateTime? filterDateTo,
            string serialNumberFilter,
            string fromWarehouseFilter,
            string toWarehouseFilter,
            string fromUserFilter,
            string toUserFilter)
        {
            var filteredLogs = allLogs.AsQueryable();

            if (filterDateFrom.HasValue)
                filteredLogs = filteredLogs.Where(log => log.EventDate.Date >= filterDateFrom.Value.Date);

            if (filterDateTo.HasValue)
                filteredLogs = filteredLogs.Where(log => log.EventDate.Date <= filterDateTo.Value.Date);

            if (!string.IsNullOrEmpty(serialNumberFilter) && serialNumberFilter != "Wszystkie")
                filteredLogs = filteredLogs.Where(log => log.SerialNumber == serialNumberFilter);

            if (!string.IsNullOrEmpty(fromWarehouseFilter) && fromWarehouseFilter != "Wszystkie")
                filteredLogs = filteredLogs.Where(log => log.FromWarehouseName == fromWarehouseFilter);

            if (!string.IsNullOrEmpty(toWarehouseFilter) && toWarehouseFilter != "Wszystkie")
                filteredLogs = filteredLogs.Where(log => log.ToWarehouseName == toWarehouseFilter);

            if (!string.IsNullOrEmpty(fromUserFilter) && fromUserFilter != "Wszyscy")
                filteredLogs = filteredLogs.Where(log => log.FromUserName == fromUserFilter);

            if (!string.IsNullOrEmpty(toUserFilter) && toUserFilter != "Wszyscy")
                filteredLogs = filteredLogs.Where(log => log.ToUserName == toUserFilter);

            return new ObservableCollection<DeviceLogModel>(filteredLogs.OrderByDescending(log => log.EventDate));
        }

        public ObservableCollection<DeviceLogModel> ResetFilters(ObservableCollection<DeviceLogModel> allLogs)
            => new ObservableCollection<DeviceLogModel>(allLogs.OrderByDescending(log => log.EventDate));
    }
}