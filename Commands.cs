using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypesUtils
{
    public partial class HypesUtils
    {
        [ConsoleCommand("rank", "Check your current rank and XP")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
        public void OnCommandRank(CCSPlayerController? playerController, CommandInfo command)
        {
            if (!playerController.IsValidPlayer()) return;

            if (playerController == null) return;
            Player player = playerCache.GetPlayer(playerController.SteamID.ToString());
            if (player == null) return;

            Rank rank = ranks.Where(r => r.exp <= player.exp).OrderByDescending(r => r.exp).FirstOrDefault();
            Rank nextRank = ranks.Where(r => r.exp > player.exp).OrderBy(r => r.exp).FirstOrDefault();

            playerController.PrintToChat($"{ChatColors.DarkBlue}---------------------------");
            playerController.PrintToChat($"{ChatColors.White}Your rank is {ChatColors.Blue}{new Helpers().ReplaceColorValue(rank.displayName)}");
            playerController.PrintToChat($"{ChatColors.White}You have {ChatColors.Blue}{player.exp} XP");
            playerController.PrintToChat($"{ChatColors.White}You need {ChatColors.Blue}{nextRank.exp - player.exp} XP {ChatColors.White}to rankup");
            playerController.PrintToChat($"{ChatColors.DarkBlue}---------------------------");
        }

        [ConsoleCommand("top", "Check the top 5 players")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
        public void OnCommandTop(CCSPlayerController? playerController, CommandInfo command)
        {
            if (!playerController.IsValidPlayer()) return;

            if (playerController == null) return;
            List<Player> topPlayers = playerCache.GetTop(5);

            playerController.PrintToChat($"{ChatColors.DarkBlue}---------------------------");
            playerController.PrintToChat($"{ChatColors.White}Top 5 players:");
            for (int i = 0; i < topPlayers.Count; i++)
            {
                playerController.PrintToChat($"{ChatColors.White}{i + 1}. {ChatColors.Blue}{topPlayers[i].name} {ChatColors.White}with {ChatColors.Blue}{topPlayers[i].exp} XP");
            }
            playerController.PrintToChat($"{ChatColors.DarkBlue}---------------------------");
        }

        [ConsoleCommand("iamgod", "Claims ownership of server")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
        public void OnCommandIAmGod(CCSPlayerController? playerController, CommandInfo command)
        {
            bool claimed = false;
            using (var cmd = new MySqlCommand())
            {
                dbConnection.Open();
                cmd.Connection = dbConnection;
                cmd.CommandText = "SELECT value FROM settings WHERE setting = 'ownershipClaimed'";
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        playerController.PrintToChat($"{ChatColors.DarkBlue}Ownership has already been claimed");
                        dbConnection.Close();
                        claimed = true;
                        return;
                    }
                }
                dbConnection.Close();
            }

            if (claimed) return;

            if (!playerController.IsValidPlayer()) return;
            if (playerController == null) return;

            Player player = playerCache.GetPlayer(playerController.SteamID.ToString());
            if (player == null) return;

            player.power = 4;
            playerCache.UpdateCache(player.steamid, player);

            using (var cmd = new MySqlCommand())
            {
                dbConnection.Open();
                cmd.Connection = dbConnection;
                cmd.CommandText = "UPDATE players SET power = 4 WHERE steamid = @steamid";
                cmd.Parameters.AddWithValue("@steamid", player.steamid);
                cmd.ExecuteNonQuery();
                dbConnection.Close();
            }

            using (var cmd = new MySqlCommand())
            {
                dbConnection.Open();
                cmd.Connection = dbConnection;
                cmd.CommandText = "INSERT INTO settings (setting, value) VALUES ('ownershipClaimed', @steamid) ON DUPLICATE KEY UPDATE value = @steamid";
                cmd.Parameters.AddWithValue("@steamid", player.steamid);
                cmd.ExecuteNonQuery();
                dbConnection.Close();
            }

            playerController.PrintToChat($"{ChatColors.DarkBlue}You are now the owner of the server");
        }

        [ConsoleCommand("rcon", "Execute an RCON command")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY, minArgs: 1, usage: "[command]")]
        public void OnCommandRcon(CCSPlayerController? playerController, CommandInfo command)
        {
            if (!playerController.IsValidPlayer()) return;
            if (playerController == null) return;

            Player player = playerCache.GetPlayer(playerController.SteamID.ToString());
            if (player == null) return;
            if (hasPermission(player, "owner"))
                Server.ExecuteCommand(command.ArgString);
            else
                playerController.PrintToChat($"{ChatColors.DarkRed}You do not have permission to use this command");
        }

        [ConsoleCommand("setgroup", "Set's a player's permission group")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY, minArgs: 2, usage: "[username] [group]")]
        public void OnCommandSetGroup(CCSPlayerController? playerController, CommandInfo command)
        {
            if (!playerController.IsValidPlayer()) return;
            if (playerController == null) return;

            Player player = playerCache.GetPlayer(playerController.SteamID.ToString());
            if (player == null) return;
            if (!hasPermission(player, "admin"))
            {
                playerController.PrintToChat($"{ChatColors.DarkRed}You do not have permission to use this command");
                return;
            }

            Player? target = null;
            List<CCSPlayerController>? onlinePlayers = Utilities.GetPlayers();
            foreach (CCSPlayerController onlinePlayer in onlinePlayers)
            {
                Log(command.GetArg(1).ToString().ToLower() + " " + onlinePlayer.PlayerName.ToLower() + " " + onlinePlayer.PlayerName.ToLower().Contains(command.GetArg(1).ToString().ToLower()));
                if (onlinePlayer.PlayerName.ToLower().Contains(command.GetArg(1).ToString().ToLower()))
                {
                    target = playerCache.GetPlayer(onlinePlayer.SteamID.ToString());
                    break;
                }
            }

            if (target == null)
            {
                playerController.PrintToChat($"{config.Get("ChatPrefix")} {ChatColors.DarkRed}Player not found");
                return;
            }

            Group group = groups.Get(command.GetArg(2));
            if (group == null)
            {
                playerController.PrintToChat($"{config.Get("ChatPrefix")} {ChatColors.DarkRed}Group not found");
                return;
            }

            //if (player.power <= target.power)
            //{
            //    playerController.PrintToChat($"{config.Get("ChatPrefix")} {ChatColors.DarkRed}You do not have permission to set this player's group");
            //    return;
            //}

            if (player.power <= group.power)
            {
                playerController.PrintToChat($"{config.Get("ChatPrefix")} {ChatColors.DarkRed}You are not permitted to assign this group.");
                return;
            }

            target.power = group.power ?? 0;
            playerCache.UpdateCache(target.steamid, target);

            using (var cmd = new MySqlCommand())
            {
                dbConnection.Open();
                cmd.Connection = dbConnection;
                cmd.CommandText = "UPDATE players SET power = @power WHERE steamid = @steamid";
                cmd.Parameters.AddWithValue("@power", group.power);
                cmd.Parameters.AddWithValue("@steamid", target.steamid);
                cmd.ExecuteNonQuery();
                dbConnection.Close();
            }

            playerController.PrintToChat($"{config.Get("ChatPrefix")} {ChatColors.DarkBlue}Set {target.name}'s group to {group.name}");
        }

        private bool hasPermission(Player player, string groupName)
        {
            Group group = groups.Get(groupName);

            Log(group + " " + player.power);

            return group.power <= player.power;
        }

        private bool hasPermission(Player player, int groupPower)
        {
            return groups.Get(groupPower).power <= player.power;
        }
    }
}