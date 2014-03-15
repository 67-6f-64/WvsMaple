using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WvsGame.Maple.Life
{
    public class Loot
    {
        public int MapleID { get; set; }
        public int MinimumQuantity { get; set; }
        public int MaximumQuantity { get; set; }
        public int QuestID { get; set; }
        public int Chance { get; set; }

        public Loot(int minMeso, int maxMeso, int chance)
        {
            this.MapleID = 0;
            this.MinimumQuantity = minMeso;
            this.MaximumQuantity = maxMeso;
            this.Chance = chance;
        }
    }
}
