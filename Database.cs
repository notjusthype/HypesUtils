namespace HypesUtils;

using MySqlConnector;

public partial class HypesUtils
{
    public void ConnectDb()
    {
        try
        {
            if (dbConnection != null || dbConnection?.State == System.Data.ConnectionState.Open)
            {
                Log("Already connected to MySQL");
                return;
            }

            dbConnection = new MySqlConnection($"Server={config?.Get("SQLHost")}; Port={config?.Get("SQLPort")}; Database={config?.Get("SQLDatabase")}; Uid={config?.Get("SQLUser")}; Password={config?.Get("SQLPassword")};");
            dbConnection.Open();

            Log("Connected to MySQL");

            dbConnection.Close();
        }
        catch (Exception ex)
        {
            LogError($"Failed to connect to MySQL: {ex.Message}");
            return;
        }
    }
}