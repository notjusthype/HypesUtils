// create a class to read config.json and parse it
using CounterStrikeSharp.API.Modules.Utils;
using Newtonsoft.Json;

namespace HypesUtils
{
    internal class Ranks
    {
        public static List<Rank>? Load(string moduleDirectory)
        {
            try
            {
                // create config.json if it doesn't exist and set default values to dummy data

                if (!File.Exists(moduleDirectory))
                {
                    Rank unranked = new Rank().setId(0).setName("Unranked").setDisplayName("{white}Unranked").setExp(0);
                    Rank silver1 = new Rank().setId(1).setName("Silver 1").setDisplayName("{grey}Silver 1").setExp(300);
                    Rank silver2 = new Rank().setId(2).setName("Silver 2").setDisplayName("{grey}Silver 2").setExp(600);
                    Rank silver3 = new Rank().setId(3).setName("Silver 3").setDisplayName("{grey}Silver 3").setExp(900);
                    Rank silver4 = new Rank().setId(4).setName("Silver Elite").setDisplayName("{grey}Silver Elite").setExp(1200);
                    Rank silver5 = new Rank().setId(5).setName("Silver Elite Master").setDisplayName("{grey}Silver Elite Master").setExp(1500);
                    Rank nova1 = new Rank().setId(6).setName("Gold Nova 1").setDisplayName("{green}Gold Nova 1").setExp(1800);
                    Rank nova2 = new Rank().setId(7).setName("Gold Nova 2").setDisplayName("{green}Gold Nova 2").setExp(2100);
                    Rank nova3 = new Rank().setId(8).setName("Gold Nova 3").setDisplayName("{green}Gold Nova 3").setExp(2400);
                    Rank nova4 = new Rank().setId(9).setName("Gold Nova 4").setDisplayName("{green}Gold Nova Master").setExp(2700);
                    Rank mg1 = new Rank().setId(10).setName("MG 1").setDisplayName("{blue}Master Guardian 1").setExp(3000);
                    Rank mg2 = new Rank().setId(11).setName("MG 2").setDisplayName("{blue}Master Guardian 2").setExp(3300);
                    Rank mg3 = new Rank().setId(12).setName("MG 3").setDisplayName("{blue}Master Guardian 3").setExp(3600);
                    Rank mge = new Rank().setId(13).setName("MG Elite").setDisplayName("{blue}Master Guardian Elite").setExp(3900);
                    Rank dmg = new Rank().setId(14).setName("DMG").setDisplayName("{purple}Distinguished Master Guardian").setExp(4200);
                    Rank le = new Rank().setId(15).setName("LE").setDisplayName("{purple}Legendary Eagle").setExp(4500);
                    Rank lem = new Rank().setId(16).setName("LE Master").setDisplayName("{purple}Legendary Eagle Master").setExp(4800);
                    Rank supreme = new Rank().setId(17).setName("Supreme").setDisplayName("{red}Supreme Master First Class").setExp(5100);
                    Rank global = new Rank().setId(18).setName("Global Elite").setDisplayName("{magenta}Global Elite").setExp(5400);

                    List<Rank> ranks = new List<Rank>([unranked, silver1, silver2, silver3, silver4, silver5, nova1, nova2, nova3, nova4, mg1, mg2, mg3, mge, dmg, le, lem, supreme, global]);

                    File.WriteAllText(moduleDirectory, JsonConvert.SerializeObject(ranks, Newtonsoft.Json.Formatting.Indented));
                }
                return JsonConvert.DeserializeObject<List<Rank>>(File.ReadAllText(moduleDirectory));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load ranks: {ex.Message}");
                return null;
            }
        }

        public Rank? Get(string key)
        {
            return (Rank?)(GetType().GetProperty(key)?.GetValue(this));
        }
    }
}