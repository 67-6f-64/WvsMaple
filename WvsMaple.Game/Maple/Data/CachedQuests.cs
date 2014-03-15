using reNX;
using reNX.NXProperties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace WvsGame.Maple.Data
{
    public class CachedQuests : KeyedCollection<ushort, Quest>
    {
        public CachedQuests(NXFile dataFile)
            : base()
        {
            foreach (NXNode questNode in dataFile.ResolvePath("/Quest/QuestInfo.img"))
            {
                MapleData.InfoNode = questNode;

                Quest quest = new Quest();

                quest.ID = ushort.Parse(questNode.Name);
                quest.Area = MapleData.GetSByte("area");

                this.Add(quest);
            }
        }

        protected override ushort GetKeyForItem(Quest item)
        {
            return item.ID;
        }
    }
}
