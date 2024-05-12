// create a class to read config.json and parse it
using CounterStrikeSharp.API.Modules.Utils;

namespace HypesUtils
{
    internal class Config
    {
        public string? SQLUser { get; set; }
        public string? SQLPassword { get; set; }
        public string? SQLHost { get; set; }
        public string? SQLDatabase { get; set; }
        public string? SQLPort { get; set; }
        public string? ChatPrefix { get; set; }
        public string? EXP_Headshot { get; set; }
        public string? EXP_Kill { get; set; }
        public string? EXP_PentrationKill { get; set; }
        public string? EXP_NoScopeKill { get; set; }
        public string? EXP_ThruSmokeKill { get; set; }
        public string? EXP_AttackerBlindKill { get; set; }
        public string? EXP_LongDistanceKill { get; set; }
        public string? LongDistanceKillDistance { get; set; }
        public string? EXP_Assist { get; set; }
        public string? EXP_Death { get; set; }
        public string? EXP_Suicide { get; set; }
        public string? EXP_RoundWin { get; set; }
        public string? EXP_RoundLoss { get; set; }
        public string? EXP_BombPlanted { get; set; }
        public string? EXP_BombDefused { get; set; }
        public string? EXP_RoundMVP { get; set; }
        public bool? EnableWarmupXP { get; set; }
        public string? MinPlayers { get; set; }

        public static Config? Load(string moduleDirectory)
        {
            try
            {
                // create config.json if it doesn't exist and set default values to dummy data
                if (!File.Exists(moduleDirectory))
                {
                    File.WriteAllText(moduleDirectory, Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        EXP_Assist = "5",
                        EXP_BombDefused = "50",
                        EXP_BombPlanted = "50",
                        EXP_Death = "-10",
                        EXP_Suicide = "-20",
                        EXP_Headshot = "10",
                        EXP_Kill = "10",
                        EXP_LongDistanceKill = "10",
                        EXP_NoScopeKill = "10",
                        EXP_PentrationKill = "10",
                        EXP_ThruSmokeKill = "10",
                        EXP_AttackerBlindKill = "10",
                        EXP_RoundMVP = "20",
                        LongDistanceKillDistance = "100",
                        EXP_RoundLoss = "-10",
                        EXP_RoundWin = "10",
                        SQLDatabase = "database",
                        SQLHost = "localhost",
                        SQLPassword = "password",
                        SQLPort = "3306",
                        SQLUser = "user",
                        ChatPrefix = $"{{Grey}}[{{Blue}}HypesUtils{{Grey}}]{{White}}",
                        minPlayers = 3,
                        EnableWarmupXP = true,
                    }, Newtonsoft.Json.Formatting.Indented));
                }
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(File.ReadAllText(moduleDirectory));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load config: {ex.Message}");
                return null;
            }
        }

        public void Save(string moduleDirectory)
        {
            try
            {
                File.WriteAllText(moduleDirectory, Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save config: {ex.Message}");
            }
        }

        public void Validate()
        {
            // create config.json if it doesn't exist and set default values to dummy data
            foreach (var prop in this.GetType().GetProperties())
            {
                if (prop.Name.StartsWith("SQL") && prop.GetValue(this) == null)
                {
                    prop.SetValue(this, "");
                }
            }

            EXP_Assist ??= "5";
            EXP_BombDefused ??= "50";
            EXP_BombPlanted ??= "50";
            EXP_Death ??= "-10";
            EXP_Headshot ??= "10";
            EXP_Kill ??= "10";
            EXP_LongDistanceKill ??= "10";
            EXP_NoScopeKill ??= "10";
            EXP_PentrationKill ??= "10";
            EXP_ThruSmokeKill ??= "10";
            EXP_AttackerBlindKill ??= "10";
            LongDistanceKillDistance ??= "35";
            EXP_RoundLoss ??= "-10";
            EXP_RoundWin ??= "10";
            ChatPrefix = new Helpers().ReplaceColorValue(ChatPrefix ?? "{Grey}[{Blue}HypesUtils{Grey}]{White}");
        }

        public string? Get(string key)
        {
            return this.GetType().GetProperty(key)?.GetValue(this)?.ToString();
        }
    }
}