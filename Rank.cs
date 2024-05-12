using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypesUtils
{
    internal class Rank
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? displayName { get; set; }
        public int? exp { get; set; }

        public Rank()
        {
        }

        public Rank setId(int ID)
        {
            this.id = ID;
            return this;
        }

        public Rank setName(string Name)
        {
            this.name = Name;
            return this;
        }

        public Rank setDisplayName(string DisplayName)
        {
            this.displayName = DisplayName;
            return this;
        }

        public Rank setExp(int EXP)
        {
            this.exp = EXP;
            return this;
        }
    }
}