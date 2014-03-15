using reNX;
using reNX.NXProperties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace WvsGame.Maple.Data
{
    public class CachedItems : KeyedCollection<int, Item>
    {
        public CachedItems(NXFile dataFile)
            : base()
        {
            foreach (NXNode categoryNode in dataFile.ResolvePath("/Character"))
            {
                if (categoryNode.Name == "Afterimage" || categoryNode.Name == "Face" || categoryNode.Name == "Hair" || categoryNode.Name.Contains(".img"))
                {
                    continue;
                }

                foreach (NXNode equipNode in categoryNode)
                {
                    MapleData.InfoNode = equipNode["info"];

                    Item item = new Item();

                    item.MapleID = int.Parse(equipNode.Name.Replace(".img", ""));
                    item.IsCash = MapleData.GetBool("cash");
                    item.SalePrice = MapleData.GetInt("price");

                    item.MaxUpgradesAvailable = MapleData.GetByte("tuc");

                    item.RequiredLevel = MapleData.GetShort("reqLevel");
                    item.RequiredJob = MapleData.GetShort("reqJob");
                    item.RequiredStrength = MapleData.GetShort("reqSTR");
                    item.RequiredDexterity = MapleData.GetShort("reqDEX");
                    item.RequiredIntelligence = MapleData.GetShort("reqINT");
                    item.RequiredLuck = MapleData.GetShort("reqLUK");

                    item.Strength = MapleData.GetShort("incSTR");
                    item.Dexterity = MapleData.GetShort("incDEX");
                    item.Intelligence = MapleData.GetShort("incINT");
                    item.Luck = MapleData.GetShort("incLUK");
                    item.MP = MapleData.GetShort("incHP");
                    item.MP = MapleData.GetShort("incMP");
                    item.WeaponAttack = MapleData.GetShort("incPAD");
                    item.WeaponDefense = MapleData.GetShort("incPDD");
                    item.MagicAttack = MapleData.GetShort("incMAD");
                    item.MagicDefense = MapleData.GetShort("incMDD");
                    item.Speed = MapleData.GetShort("incSpeed");
                    item.Jump = MapleData.GetShort("incJump");
                    item.Accuracy = MapleData.GetShort("incACC");
                    item.Avoidability = MapleData.GetShort("incEVA");

                    this.Add(item);
                }
            }

            foreach (NXNode categoryNode in dataFile.ResolvePath("/Item"))
            {
                if (categoryNode.Name == "Pet" || categoryNode.Name == "Special")
                {
                    continue;
                }

                foreach (NXNode containNode in categoryNode)
                {
                    foreach (NXNode itemNode in containNode)
                    {
                        Item item = new Item();

                        item.MapleID = int.Parse(itemNode.Name
                            .Replace(".img", ""));

                        if (!itemNode.ContainsChild("info"))
                        {
                            item.MaxPerStack = 1;
                            item.IsCash = false;

                            this.Add(item);
                        }
                        else
                        {
                            MapleData.InfoNode = itemNode["info"];

                            item.MesoReward = MapleData.GetInt("meso");
                            item.IsCash = MapleData.GetBool("cash");
                            item.MaxPerStack = MapleData.GetShort("maxSlot");
                            item.SalePrice = MapleData.GetInt("price");

                            item.Success = MapleData.GetInt("success");
                            item.BreakItem = MapleData.GetInt("cursed");
                            item.IStrength = MapleData.GetShort("incSTR");
                            item.IDexterity = MapleData.GetShort("incDEX");
                            item.IIntelligence = MapleData.GetShort("incINT");
                            item.ILuck = MapleData.GetShort("incLUK");
                            item.IHP = MapleData.GetShort("incMHP");
                            item.IMP = MapleData.GetShort("incMMP");
                            item.IWeaponAttack = MapleData.GetShort("incPAD");
                            item.IMagicAttack = MapleData.GetShort("incMAD");
                            item.IWeaponDefense = MapleData.GetShort("incPDD");
                            item.IMagicDefense = MapleData.GetShort("incMDD");
                            item.IAccuracy = MapleData.GetShort("incACC");
                            item.IAvoidability = MapleData.GetShort("incEVA");
                            item.IJump = MapleData.GetShort("incJump");
                            item.ISpeed = MapleData.GetShort("incSpeed");

                            if (itemNode.ContainsChild("spec"))
                            {
                                MapleData.InfoNode = itemNode["spec"];

                                item.CMoveTo = MapleData.GetInt("moveTo");

                                byte cureFlags = 0x00;

                                if (itemNode["spec"].ContainsChild("curse"))
                                    cureFlags += 0x01;
                                if (itemNode["spec"].ContainsChild("seal"))
                                    cureFlags += 0x02;
                                if (itemNode["spec"].ContainsChild("weakness"))
                                    cureFlags += 0x04;
                                if (itemNode["spec"].ContainsChild("darkness"))
                                    cureFlags += 0x08;
                                if (itemNode["spec"].ContainsChild("poison"))
                                    cureFlags += 0x10;

                                item.CFlags = cureFlags;

                                item.CHP = MapleData.GetShort("hp");
                                item.CMP = MapleData.GetShort("mp");
                                item.CHPPercentage = MapleData.GetShort("hpR");
                                item.CMPPercentage = MapleData.GetShort("mpR");
                                item.CSpeed = MapleData.GetShort("speed");
                                item.CAvoid = MapleData.GetShort("eva");
                                item.CAccuracy = MapleData.GetShort("acc");
                                item.CWeaponAttack = MapleData.GetShort("pad");
                                item.CMagicAttack = MapleData.GetShort("mad");
                                item.CBuffTime = MapleData.GetInt("time");
                            }

                            if (itemNode.ContainsChild("mob"))
                            {
                                item.CSummons = new List<MobSummon>();

                                foreach (NXNode summonNode in itemNode["mob"])
                                {
                                    MapleData.InfoNode = summonNode;

                                    MobSummon summon = new MobSummon();

                                    summon.MapleID = MapleData.GetInt("id");
                                    summon.Probability = MapleData.GetByte("prob");

                                    item.CSummons.Add(summon);
                                }
                            }

                            this.Add(item);
                        }
                    }
                }
            }
        }

        protected override int GetKeyForItem(Item item)
        {
            return item.MapleID;
        }
    }
}
