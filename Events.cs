using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using HypesUtils.Caches;

namespace HypesUtils
{
    public partial class HypesUtils
    {
        private void UpdateClantag(PlayerCache playerCache)
        {
            List<CCSPlayerController> players = Utilities.GetPlayers();
            foreach (CCSPlayerController playerController in players)
            {
                Player player = playerCache.GetPlayer(playerController.SteamID.ToString());
                if (player == null) continue;

                Group group = groups.GetUserGroup(player);

                if (group == null || group.name == "Player")
                {
                    Rank? rank = ranks?.Where(r => r.exp <= player.exp).OrderByDescending(r => r.exp).FirstOrDefault();
                    playerController.Clan = $"[{rank?.name}]";
                }
                else
                {
                    playerController.Clan = $"[{new Helpers().ReplaceColorValue(group.name)}]";
                }
            }
            return;
        }

        private void SetupGameEvents(PlayerCache playerCache)
        {
            RegisterEventHandler<EventPlayerConnectFull>((@event, info) =>
            {
                CCSPlayerController playerController = @event.Userid;

                playerController.PrintToChat($"{config.Get("ChatPrefix")} {ChatColors.White}This server is running {ChatColors.Blue}HypesUtils {ChatColors.DarkBlue}V{GetVersion()}");

                if (!playerController.IsValidPlayer() || playerController == null) return HookResult.Continue;

                Log($"{playerController.PlayerName} connected to the server");

                Player? player = playerCache.GetPlayer(playerController.SteamID.ToString());

                if (player == null)
                {
                    playerCache.Add(new Player
                    {
                        steamid = playerController.SteamID.ToString(),
                        exp = 0,
                        name = playerController.PlayerName,
                        kills = 0,
                        deaths = 0,
                        assists = 0,
                        headshots = 0,
                        lastseen = DateTime.Now
                    });

                    player = playerCache.GetPlayer(playerController.SteamID.ToString());
                }

                if (player == null)
                {
                    LogError("Failed to create player");
                    return HookResult.Continue;
                }

                player.lastseen = DateTime.Now;

                Log(player.steamid);

                playerController.PrintToChat($"{config?.Get("ChatPrefix")} {ChatColors.White}Welcome back, {ChatColors.Blue}{player.name}{ChatColors.White}!");

                // get players rank based on exp
                Group group = groups.GetUserGroup(player);

                if (group == null || group.name == "Player")
                {
                    Rank? rank = ranks?.Where(r => r.exp <= player.exp).OrderByDescending(r => r.exp).FirstOrDefault();
                    playerController.Clan = $"[{rank?.name}]";
                }
                else
                {
                    playerController.Clan = $"[{new Helpers().ReplaceColorValue(group.name)}]";
                }

                return HookResult.Continue;
            });

            RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
            {
                CCSPlayerController playerController = @event.Userid;

                if (!playerController.IsValidPlayer() || playerController == null) return HookResult.Continue;

                Log($"{playerController.PlayerName} disconnected from the server");

                Player? player = playerCache.GetPlayer(playerController.SteamID.ToString());

                if (player == null)
                {
                    LogError("Failed to find player");
                    return HookResult.Continue;
                }

                player.lastseen = DateTime.Now;

                playerCache.UpdateDB(player.steamid, player, null, null, null, null, null, null, null);

                return HookResult.Continue;
            });

            RegisterEventHandler<EventPlayerDeath>((@event, info) =>
            {
                if (!IsPointsAllowed()) return HookResult.Continue;
                CCSPlayerController victimController = @event.Userid;
                CCSPlayerController attackerController = @event.Attacker;
                CCSPlayerController assisterController = @event.Assister;

                if (!victimController.IsValidPlayer() || victimController == null) return HookResult.Continue;

                Player? victim = playerCache.GetPlayer(victimController.SteamID.ToString());
                Player? attacker = playerCache.GetPlayer(attackerController.SteamID.ToString());
                Player? assister = null;
                if (assisterController != null) assister = playerCache.GetPlayer(assisterController?.SteamID.ToString());

                if (assister != null)
                {
                    int assistExp = Int32.Parse(config.Get("EXP_Assist"));
                    assister.assists++;
                    assister.exp += assistExp;

                    assisterController.PrintToChat($"{config?.Get("ChatPrefix")} {ChatColors.Green}+{assistExp} XP {ChatColors.White}(Assist)");
                }

                if (victim != null)
                {
                    victim.deaths++;

                    int deathExp = Int32.Parse(config?.Get("EXP_Death"));
                    int suicideExp = Int32.Parse(config?.Get("EXP_Suicide"));
                    int expChange = deathExp;
                    List<string> punishments = new List<string>(["Death"]);

                    if (victimController.UserId == attackerController?.UserId)
                    {
                        expChange += suicideExp;
                        punishments.Add("Suicide");
                    }

                    victim.exp += expChange;
                    string punishmentsString = string.Join($"{ChatColors.Red},{ChatColors.White} ", punishments);
                    victimController.PrintToChat($"{config?.Get("ChatPrefix")} {ChatColors.Red}-{Math.Abs(expChange).ToString()} XP {ChatColors.White}({punishmentsString})");
                }
                if (attacker != null && victimController.UserId != attackerController?.UserId)
                {
                    attacker.kills++;
                    int killExp = Int32.Parse(config.Get("EXP_Kill"));
                    int headshotExp = Int32.Parse(config.Get("EXP_Headshot"));
                    int penetrationKillExp = Int32.Parse(config.Get("EXP_PentrationKill"));
                    int noscopeKillExp = Int32.Parse(config.Get("EXP_NoScopeKill"));
                    int thruSmokeKillExp = Int32.Parse(config.Get("EXP_ThruSmokeKill"));
                    int attackerBlindKillExp = Int32.Parse(config.Get("EXP_AttackerBlindKill"));
                    int longDistanceKillExp = Int32.Parse(config.Get("EXP_LongDistanceKill"));
                    int longDistanceKillDistance = Int32.Parse(config.Get("LongDistanceKillDistance"));

                    List<string> eventsAchieved = new List<string>();

                    int expChange = 0;

                    if (killExp > 0)
                    {
                        expChange += killExp;
                        eventsAchieved.Add("Kill");
                    };
                    if (headshotExp > 0 && @event.Headshot)
                    {
                        expChange += headshotExp;
                        eventsAchieved.Add("Headshot");
                    }
                    if (penetrationKillExp > 0 && @event.Penetrated > 0)
                    {
                        expChange += penetrationKillExp * @event.Penetrated;
                        eventsAchieved.Add("Penetration");
                    }
                    if (noscopeKillExp > 0 && @event.Noscope)
                    {
                        expChange += noscopeKillExp;
                        eventsAchieved.Add("NoScope");
                    };
                    if (thruSmokeKillExp > 0 && @event.Thrusmoke)
                    {
                        expChange += thruSmokeKillExp;
                        eventsAchieved.Add("Smoke");
                    };
                    if (attackerBlindKillExp > 0 && @event.Attackerblind)
                    {
                        expChange += attackerBlindKillExp;
                        eventsAchieved.Add("Blind");
                    };
                    if (longDistanceKillExp > 0 && @event.Distance >= longDistanceKillDistance)
                    {
                        expChange += longDistanceKillExp;
                        eventsAchieved.Add("Long Distance");
                    };

                    string eventsAchievedString = string.Join($"{ChatColors.Blue},{ChatColors.White} ", eventsAchieved);
                    attackerController.PrintToChat($"{config?.Get("ChatPrefix")} {ChatColors.Blue}+{expChange} XP {ChatColors.White}({eventsAchievedString})");

                    attacker.exp += expChange;
                }

                return HookResult.Continue;
            });

            RegisterEventHandler<EventBombPlanted>((@event, info) =>
            {
                if (!IsPointsAllowed()) return HookResult.Continue;
                int bombPlantedExp = Int32.Parse(config.Get("EXP_BombPlanted"));
                CCSPlayerController playerController = @event.Userid;

                if (!playerController.IsValid || playerController == null) return HookResult.Continue;

                Player? player = playerCache.GetPlayer(playerController.SteamID.ToString());

                if (player == null)
                {
                    return HookResult.Continue;
                }

                player.exp += bombPlantedExp;
                return HookResult.Continue;
            });

            RegisterEventHandler<EventBombDefused>((@event, info) =>
            {
                if (!IsPointsAllowed()) return HookResult.Continue;
                int bombPlantedExp = Int32.Parse(config.Get("EXP_BombDefused"));
                CCSPlayerController playerController = @event.Userid;

                if (!playerController.IsValid || playerController == null) return HookResult.Continue;

                Player? player = playerCache.GetPlayer(playerController.SteamID.ToString());

                if (player == null)
                {
                    return HookResult.Continue;
                }

                player.exp += bombPlantedExp;
                return HookResult.Continue;
            });

            RegisterEventHandler<EventRoundEnd>((@event, info) =>
            {
                if (!IsPointsAllowed()) return HookResult.Continue;
                CsTeam winnerTeam = (CsTeam)@event.Winner;

                List<CCSPlayerController> players = new List<CCSPlayerController>();
                players = Utilities.GetPlayers();
                foreach (CCSPlayerController playerController in players)
                {
                    Player player = playerCache.GetPlayer(playerController.SteamID.ToString());
                    if (player == null) continue;

                    playerCache.UpdateDB(player.steamid, player, null, null, null, null, null, null, null);

                    if (!playerController.IsValidPlayer() || !player.SpawnedThisRound)
                    {
                        CsTeam playerTeam = (CsTeam)playerController.TeamNum;

                        if (playerTeam != CsTeam.None && playerTeam != CsTeam.Spectator)
                        {
                            if (playerTeam == winnerTeam)
                            {
                                int roundWinExp = Int32.Parse(config.Get("EXP_RoundWin"));
                                player.exp += roundWinExp;
                                playerController.PrintToChat($"{config.Get("ChatPrefix")} {ChatColors.Green}+{roundWinExp} XP {ChatColors.White}(Round Win)");
                            }
                            else
                            {
                                int roundLossExp = Int32.Parse(config.Get("EXP_RoundLoss"));
                                player.exp += roundLossExp;
                                playerController.PrintToChat($"{config.Get("ChatPrefix")} {ChatColors.Red}-{Math.Abs(roundLossExp)} XP {ChatColors.White}(Round Loss)");
                            }
                        }
                    }
                    player.SpawnedThisRound = false;
                }

                UpdateClantag(playerCache);

                return HookResult.Continue;
            });

            RegisterEventHandler<EventRoundStart>((@event, info) =>
            {
                if (!IsPointsAllowed()) return HookResult.Continue;
                List<CCSPlayerController> players = Utilities.GetPlayers();

                foreach (CCSPlayerController playerController in players)
                {
                    Player player = playerCache.GetPlayer(playerController.SteamID.ToString());
                    if (player == null) continue;
                    if (!playerController.IsValidPlayer()) continue;
                    if (player.SpawnedThisRound) continue;

                    player.SpawnedThisRound = false;
                }

                return HookResult.Continue;
            });

            RegisterEventHandler<EventRoundMvp>((@event, info) =>
            {
                if (!IsPointsAllowed()) return HookResult.Continue;
                int mvpXp = Int32.Parse(config.Get("EXP_RoundMVP"));
                CCSPlayerController playerController = @event.Userid;

                if (!playerController.IsValid || playerController == null) return HookResult.Continue;
                Player? player = playerCache.GetPlayer(playerController.SteamID.ToString());

                if (player == null)
                {
                    return HookResult.Continue;
                }

                player.exp += mvpXp;

                playerController.PrintToChat($"{config.Get("ChatPrefix")} {ChatColors.Gold}+{mvpXp} XP {ChatColors.White}(Round MVP)");
                return HookResult.Continue;
            });
        }
    }
}