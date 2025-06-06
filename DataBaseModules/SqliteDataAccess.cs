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

namespace car_storage_odometer.DataBaseModules
{
    public class SqliteDataAccess<T>
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


        /// <summary>
        /// Executes the specified SQL query and returns the results as an observable collection.
        /// </summary>
        /// <remarks>This method uses a SQLite connection to execute the query and map the results. Ensure
        /// that the connection string is properly configured before calling this method.</remarks>
        /// <param name="query">The SQL query to execute. Must be a valid SQL statement.</param>
        /// <returns>An <see cref="ObservableCollection{T}"/> containing the results of the query. If the query returns no
        /// results, the collection will be empty.</returns>
        public static async Task<ObservableCollection<T>> LoadQuery(string query)
        {
            return await Task.Run(() =>
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                    return new ObservableCollection<T>(cnn.Query<T>(query, new DynamicParameters()));
            });
        }
    }
}
