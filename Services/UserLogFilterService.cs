﻿using car_storage_odometer.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace car_storage_odometer.Services
{
    public class UserLogFilterService
    {
        public ObservableCollection<UserLogModel> ApplyFilter(
            ObservableCollection<UserLogModel> allLogs,
            DateTime? filterDateFrom,
            DateTime? filterDateTo,
            UserLogModel selectedUserFilter,
            string selectedActionFilter)
        {
            var filteredLogs = allLogs.AsQueryable();

            if (filterDateFrom.HasValue)
                filteredLogs = filteredLogs.Where(log => log.EventDate.Date >= filterDateFrom.Value.Date);

            if (filterDateTo.HasValue)
                filteredLogs = filteredLogs.Where(log => log.EventDate.Date <= filterDateTo.Value.Date);

            if (selectedUserFilter != null && !string.IsNullOrEmpty(selectedUserFilter.UserName))
                filteredLogs = filteredLogs.Where(log => log.UserName == selectedUserFilter.UserName);

            if (!string.IsNullOrEmpty(selectedActionFilter))
                filteredLogs = filteredLogs.Where(log => log.Event == selectedActionFilter);

            return new ObservableCollection<UserLogModel>(filteredLogs.OrderByDescending(log => log.EventDate));
        }

        public ObservableCollection<UserLogModel> ResetFilters(ObservableCollection<UserLogModel> allLogs) 
            => new ObservableCollection<UserLogModel>(allLogs.OrderByDescending(log => log.EventDate));
    }
}