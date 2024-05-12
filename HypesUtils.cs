namespace HypesUtils;

using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using global::HypesUtils.Caches;
using MySqlConnector;
using System.Reflection;

[MinimumApiVersion(28)]
public partial class HypesUtils : BasePlugin
{
    public override string ModuleName => "Hype's Utilities";
    public override string ModuleVersion => GetVersion();
    public override string ModuleAuthor => "Hype";
    public override string ModuleDescription => "A collection of utilities for managing Counter-Strike 2 Servers";

    // create a class to read config.json and parse it
    private Config? config;

    private List<Rank>? ranks;
    private Groups groups;
    private PlayerCache? playerCache;

    public override void Load(bool hotReload)
    {
        config = Config.Load(ModuleDirectory + "/config.json");
        if (config == null)
        {
            Log("Failed to load config, disabling plugin");
            return;
        }

        ranks = Ranks.Load(ModuleDirectory + "/ranks.json");
        if (ranks == null)
        {
            Log("Failed to load ranks, disabling plugin");
            return;
        }

        groups = new Groups();
        List<Group>? result = Groups.Load(ModuleDirectory + "/groups.json");
        if (result == null)
        {
            Log("Failed to load groups, disabling plugin");
            return;
        }

        config?.Validate();
        Log("Hype's Utilities loaded!");

        ConnectDb();

        using (var cmd = new MySqlCommand())
        {
            dbConnection.Open();
            cmd.Connection = dbConnection;
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS players (id INT AUTO_INCREMENT PRIMARY KEY, steamid VARCHAR(20), rank INT, exp INT, name VARCHAR(64), kills INT, deaths INT, assists INT, headshots INT, power INT DEFAULT 0 , lastseen DATETIME)";
            cmd.ExecuteNonQuery();
            dbConnection.Close();
        }

        using (var cmd = new MySqlCommand())
        {
            dbConnection.Open();
            cmd.Connection = dbConnection;
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS settings (id INT AUTO_INCREMENT PRIMARY KEY, setting VARCHAR(20), value VARCHAR(64))";
            cmd.ExecuteNonQuery();
            dbConnection.Close();
        }

        using (var cmd = new MySqlCommand())
        {
            dbConnection.Open();
            cmd.Connection = dbConnection;
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS bans (id INT AUTO_INCREMENT PRIMARY KEY, steamid VARCHAR(20), reason TEXT, admin VARCHAR(64), expires DATETIME)";
            cmd.ExecuteNonQuery();
            dbConnection.Close();
        }

        playerCache = new PlayerCache(dbConnection);

        SetupGameEvents(playerCache);
        AddCommandListener("say", OnPlayerChat);
        //AddCommandListener("say_team", OnPlayerChat);
    }

    public HookResult OnPlayerChat(CCSPlayerController playerController, CommandInfo info)
    {
        Log("kys");
        if (playerController == null || !playerController.IsValid || info.GetArg(1).Length == 0 || playerController.AuthorizedSteamID == null) return HookResult.Continue;
        Player player = playerCache.GetPlayer(playerController.SteamID.ToString());
        Log("hello!");

        //if(playerController.SteamID.ToString() != "" && Muted.Contains(player.SteamID.ToString())) return HookResult.Handled; // TODO: Add muting
        string deadIcon = !playerController.PawnIsAlive ? $"{ChatColors.White}☠ {ChatColors.Grey}" : "";
        if (info.GetArg(1).StartsWith("!") || info.GetArg(1).StartsWith("/") || info.GetArg(1) == "rtv") return HookResult.Continue;
        else if (player.power > 0)
        {
            Group playerGroup = groups.GetUserGroup(player);
            string groupColor = playerGroup.groupColor;
            string messageColor = playerGroup.messageColor;
            string nameColor = playerGroup.nameColor;
            Server.PrintToChatAll(new Helpers().ReplaceColorValue($"{deadIcon}{ChatColors.Grey}[{groupColor}{playerGroup.name}{ChatColors.Grey}] {nameColor}{player.name}{ChatColors.Default}: {messageColor}{info.GetArg(1)}"));

            return HookResult.Handled;
        }
        else
        {
            Rank? rank = ranks?.Where(r => r.exp <= player.exp).OrderByDescending(r => r.exp).FirstOrDefault();
            Server.PrintToChatAll(new Helpers().ReplaceColorValue($"{deadIcon}{ChatColors.Grey}[{rank.displayName}{ChatColors.Grey}] {ChatColors.White}{player.name}{ChatColors.Grey}: {ChatColors.White}{info.GetArg(1)}"));

            return HookResult.Handled;
        }
    }

    public bool IsPointsAllowed()
    {
        int minPlayers = Int32.Parse(config.Get("MinPlayers"));
        // cast config enableWarmupXP to boolean
        bool enableWarmupXP = Boolean.Parse(config.Get("EnableWarmupXP"));
        Helpers helper = new Helpers();
        return (!helper.GameRules().WarmupPeriod || enableWarmupXP && (minPlayers <= Utilities.GetPlayers().Count));
    }
}

// X+p9xA?B8AoPOd=ZV21g