using reNX;
using reNX.NXProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WvsGame.Maple.Data
{
    public class CachedSkills : Dictionary<int, Dictionary<byte, Skill>>
    {
        public CachedSkills(NXFile dataFile)
            : base()
        {
            foreach (NXNode categoryNode in dataFile.ResolvePath("/Skill"))
            {
                if (categoryNode.Name == "MobSkill.img")
                {
                    continue; // TODO: Mob Skills.
                }

                foreach (NXNode skillNode in categoryNode["skill"])
                {
                    int mapleId = int.Parse(skillNode.Name);

                    this.Add(mapleId, new Dictionary<byte, Skill>());

                    foreach (NXNode levelNode in skillNode["level"])
                    {
                        MapleData.InfoNode = levelNode;

                        Skill skill = new Skill();

                        skill.MapleID = mapleId;
                        skill.ParameterA = MapleData.GetShort("x");
                        skill.ParameterB = MapleData.GetShort("y");
                        skill.LT = MapleData.GetPosition("lt");
                        skill.RB = MapleData.GetPosition("rb");
                        skill.Mastery = MapleData.GetSByte("mastery");
                        skill.BuffTime = MapleData.GetInt("time");
                        skill.MobCount = MapleData.GetSByte("mobCount");
                        skill.HP = MapleData.GetShort("hp");
                        skill.MP = MapleData.GetShort("mp");
                        skill.WeaponAttack = MapleData.GetShort("pad");
                        skill.WeaponDefense = MapleData.GetShort("pdd");
                        skill.MagicAttack = MapleData.GetShort("mad");
                        skill.MagicDefense = MapleData.GetShort("mdd");
                        skill.Avoid = MapleData.GetShort("eva");
                        skill.Accuracy = MapleData.GetShort("acc");
                        skill.Jump = MapleData.GetShort("jump");
                        skill.Speed = MapleData.GetShort("speed");
                        skill.CostHP = MapleData.GetShort("hpCon");
                        skill.CostMP = MapleData.GetShort("mpCon");
                        skill.CostItem = MapleData.GetInt("itemCon");
                        skill.ItemCount = MapleData.GetShort("itemConNo");
                        skill.CostBullet = MapleData.GetShort("bulletCount");
                        skill.CostMeso = MapleData.GetShort("moneyCon");
                        skill.Probability = MapleData.GetShort("prop");

                        this[mapleId].Add(byte.Parse(levelNode.Name), skill);
                    }
                }
            }
        }
    }
}
