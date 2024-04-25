using Oracle.ManagedDataAccess.Client;

namespace CamerasInfo.Helpers
{
    public static class DbChangeTracker
    {
        private static string connectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)" +
                "(HOST=RCODAX8-SCAN)(PORT=1521))(CONNECT_DATA=(SERVER = DEDICATED)" +
                "(SERVICE_NAME=RENOVIAS.renoviasconcessionaria.local)));;" +
                "User Id=CAMERASINFO;Password=#1oJH7#3+Ld4_gqZgP1;";

        private readonly static string _tableName = "Configs";

        public static void InitializeWatcher()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    // Open the connection
                    conn.Open();
                    // Create your SQL query to retrieve data from the table
                    string sql = $"SELECT * FROM {_tableName}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
