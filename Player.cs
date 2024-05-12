using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypesUtils
{
    internal class Player
    {
        public int id { get; set; }

        public string steamid { get; set; }
        public int exp { get; set; }
        public string name { get; set; }
        public int kills { get; set; }
        public int deaths { get; set; }
        public int assists { get; set; }
        public int headshots { get; set; }
        public DateTime lastseen { get; set; }
        public int rank { get; set; }
        public bool SpawnedThisRound { get; set; }
        public int power { get; set; }

        public Player()
        {
            id = 0;
            steamid = "";
            exp = 0;
            name = "";
            kills = 0;
            deaths = 0;
            assists = 0;
            headshots = 0;
            power = 0;
            lastseen = DateTime.Now;
        }
    }
}