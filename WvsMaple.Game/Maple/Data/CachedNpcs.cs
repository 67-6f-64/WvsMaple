using reNX;
using reNX.NXProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WvsGame.Maple.Data
{
    public class CachedNpcs
    {
        public Dictionary<int, string> Labels { get; private set; }
        public Dictionary<int, int> StorageCosts { get; private set; }

        public CachedNpcs(NXFile dataFile)
        {
            this.Labels = new Dictionary<int, string>();
            this.StorageCosts = new Dictionary<int, int>();

            foreach (NXNode npcNode in dataFile.ResolvePath("/Npc"))
            {
                MapleData.InfoNode = npcNode["info"];

                int npcId = int.Parse(npcNode.Name.Replace(".img", ""));

                this.Labels.Add(npcId, MapleData.GetString("quest"));
            }
        }
    }
}
