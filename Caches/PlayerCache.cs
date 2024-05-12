using MySqlConnector;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypesUtils.Caches
{
    internal class PlayerCache
    {
        private Dictionary<string, Player> players = new Dictionary<string, Player>();
        private MySqlConnection dbConnection;

        public PlayerCache(MySqlConnection dbConnection)
        {
            this.dbConnection = dbConnection;
            using (var cmd = new MySqlCommand())
            {
                dbConnection.Open();
                cmd.Connection = dbConnection;
                cmd.CommandText = "SELECT * FROM players";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var player = new Player
                        {
                            id = reader.GetInt32("id"),
                            steamid = reader.GetString("steamid"),
                            exp = reader.GetInt32("exp"),
                            name = reader.GetString("name"),
                            kills = reader.GetInt32("kills"),
                            deaths = reader.GetInt32("deaths"),
                            assists = reader.GetInt32("assists"),
                            headshots = reader.GetInt32("headshots"),
                            lastseen = reader.GetDateTime("lastseen"),
                            power = reader.GetInt32("power")
                        };
                        players.Add(player.steamid, player);
                    }
                }
                dbConnection.Close();
            }
            return;
        }

        public Player? GetPlayer(string steamid)
        {
            if (players.ContainsKey(steamid))
            {
                return players[steamid];
            }
            else
            {
                using (var cmd = new MySqlCommand())
                {
                    dbConnection.Open();
                    cmd.Connection = dbConnection;
                    cmd.CommandText = "SELECT * FROM players WHERE steamid = @steamid";
                    cmd.Parameters.AddWithValue("@steamid", steamid);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var player = new Player
                            {
                                id = reader.GetInt32("id"),
                                steamid = reader.GetString("steamid"),
                                exp = reader.GetInt32("exp"),
                                name = reader.GetString("name"),
                                kills = reader.GetInt32("kills"),
                                deaths = reader.GetInt32("deaths"),
                                assists = reader.GetInt32("assists"),
                                headshots = reader.GetInt32("headshots"),
                                lastseen = reader.GetDateTime("lastseen")
                            };
                            players.Add(player.steamid, player);
                            return player;
                        }
                    }
                    dbConnection.Close();
                }

                return null;
            }
        }

        public void Add(Player player)
        {
            using (var cmd = new MySqlCommand())
            {
                dbConnection.Open();
                cmd.Connection = dbConnection;
                cmd.CommandText = "INSERT INTO players (steamid, exp, name, kills, deaths, assists, headshots, lastseen) VALUES (@steamid, @exp, @name, @kills, @deaths, @assists, @headshots, @lastseen)";
                cmd.Parameters.AddWithValue("@steamid", player.steamid);
                cmd.Parameters.AddWithValue("@exp", player.exp);
                cmd.Parameters.AddWithValue("@name", player.name);
                cmd.Parameters.AddWithValue("@kills", player.kills);
                cmd.Parameters.AddWithValue("@deaths", player.deaths);
                cmd.Parameters.AddWithValue("@assists", player.assists);
                cmd.Parameters.AddWithValue("@headshots", player.headshots);
                cmd.Parameters.AddWithValue("@lastseen", player.lastseen);
                cmd.ExecuteNonQuery();
                dbConnection.Close();

                players.Add(player.steamid, player);

                return;
            }
        }

        public Player UpdateDB(String steamid, Player player, int? exp, int? rank, int? kills, int? deaths, int? assists, int? headshots, DateTime? lastseen)
        {
            using (var cmd = new MySqlCommand())
            {
                dbConnection.Open();
                cmd.Connection = dbConnection;
                cmd.CommandText = "UPDATE players SET exp = @exp, rank = @rank, kills = @kills, deaths = @deaths, assists = @assists, headshots = @headshots, lastseen = @lastseen WHERE steamid = @steamid";

                cmd.Parameters.AddWithValue("@steamid", steamid);
                cmd.Parameters.AddWithValue("@exp", exp ?? player.exp);
                cmd.Parameters.AddWithValue("@rank", rank ?? player.rank);
                cmd.Parameters.AddWithValue("@kills", kills ?? player.kills);
                cmd.Parameters.AddWithValue("@deaths", deaths ?? player.deaths);
                cmd.Parameters.AddWithValue("@assists", assists ?? player.assists);
                cmd.Parameters.AddWithValue("@headshots", headshots ?? player.headshots);
                cmd.Parameters.AddWithValue("@lastseen", lastseen ?? player.lastseen);

                cmd.ExecuteNonQuery();
                dbConnection.Close();

                return player;
            }
        }

        public void UpdateCache(string steamid, Player player)
        {
            if (players.ContainsKey(steamid))
            {
                players[steamid].exp = player.exp;
                players[steamid].rank = player.rank;
                players[steamid].kills = player.kills;
                players[steamid].deaths = player.deaths;
            }
            else
            {
                players.Add(steamid, player);
            }
        }

        public void UpdateCache(string steamid, Player player, int? exp, int? rank, int? kills, int? deaths, int? assists, int? headshots, DateTime? lastseen)
        {
            if (players.ContainsKey(steamid))
            {
                players[steamid].exp = exp ?? player.exp;
                players[steamid].rank = rank ?? player.rank;
                players[steamid].kills = kills ?? player.kills;
                players[steamid].deaths = deaths ?? player.deaths;
            }
            else
            {
                players.Add(steamid, player);
            }
        }

        public List<Player> GetTop(int amount)
        {
            return players.Values.OrderByDescending(p => p.exp).Take(amount).ToList();
        }
    }
}