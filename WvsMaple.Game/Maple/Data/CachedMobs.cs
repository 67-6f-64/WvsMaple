using reNX;
using reNX.NXProperties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using WvsGame.Maple.Life;

namespace WvsGame.Maple.Data
{
    public class CachedMobs : KeyedCollection<int, Mob>
    {
        public CachedMobs(NXFile dataFile)
        {
            foreach (NXNode mobNode in dataFile.ResolvePath("/Mob"))
            {
                MapleData.InfoNode = mobNode["info"];

                Mob mob = new Mob();

                mob.MapleID = int.Parse(mobNode.Name.Replace(".img", ""));
                mob.Level = MapleData.GetShort("level");
                mob.MaxHP = MapleData.GetUInt("maxHP");
                mob.MaxMP = MapleData.GetUInt("maxMP");
                mob.CurrentHP = mob.MaxHP;
                mob.CurrentMP = mob.MaxMP;
                mob.HpRecovery = MapleData.GetUInt("hpRecovery");
                mob.MpRecovery = MapleData.GetUInt("mpRecovery");
                mob.Experience = MapleData.GetUInt("exp");
                mob.SummonType = MapleData.GetShort("summonType");
                mob.Speed = MapleData.GetShort("speed");
                mob.Accuracy = MapleData.GetShort("acc");
                mob.Avoidability = MapleData.GetShort("eva");

                if (mobNode["info"].ContainsChild("revive"))
                {
                    mob.DeathSummons = new List<int>();

                    foreach (NXNode reviveNode in mobNode["info"]["revive"])
                    {
                        mob.DeathSummons.Add(int.Parse(reviveNode.Name));
                    }
                }

                // TODO: Skills, Attacks and Loots.

                this.Add(mob);
            }
        }

        protected override int GetKeyForItem(Mob item)
        {
            return item.MapleID;
        }
    }
}
