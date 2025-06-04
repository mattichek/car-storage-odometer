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

        public static ObservableCollection<WarehouseStatusModel> LoadX()
        {
            using(IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<WarehouseStatusModel>("select * from Warehouses", new DynamicParameters());
                return new ObservableCollection<WarehouseStatusModel>(output);
            }
        }
        
        private static string LoadConnectionString(string id = "DataBase")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
