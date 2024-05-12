using MySqlConnector;

namespace HypesUtils
{
    public partial class HypesUtils
    {
        public static string GetVersion()
        {
            return "1.0.0";
        }

        //private  dbclient;
        //private MongoDB.Driver.IMongoDatabase? database;
        //private MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>? dbcollection;
        private MySqlConnection dbConnection;

        public void Log(params string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{DateTime.Now.ToString("HH:mm:ss")} ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("HypesUtils");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{string.Join(" ", args)}\n");
        }

        public void LogError(params string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{DateTime.Now.ToString("HH:mm:ss")} ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("HypesUtils");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("ERROR");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{string.Join(" ", args)}\n");
        }
    }
}