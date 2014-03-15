using reNX;
using reNX.NXProperties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using WvsGame.Maple.Fields;
using WvsGame.Maple.Life;

namespace WvsGame.Maple.Data
{
    public class CachedFields : KeyedCollection<int, Field>
    {
        public CachedFields(NXFile dataFile)
            : base()
        {
            foreach (NXNode categoryNode in dataFile.ResolvePath("/Map/Map"))
            {
                if (categoryNode.Name == "AreaCode.img")
                {
                    continue;
                }

                foreach (NXNode mapNode in categoryNode)
                {
                    MapleData.InfoNode = mapNode["info"];

                    Field map = new Field();

                    map.MapleID = int.Parse(mapNode.Name.Replace(".img", ""));
                    map.ForcedReturnMapID = MapleData.GetInt("forcedReturn");
                    map.ReturnMapID = MapleData.GetInt("returnMap");
                    map.SpawnRate = MapleData.GetFloat("mobRate");
                    map.IsTown = MapleData.GetBool("town");
                    map.HasClock = MapleData.GetBool("clock");

                    if (mapNode.ContainsChild("portal"))
                    {
                        foreach (NXNode portalNode in mapNode["portal"])
                        {
                            MapleData.InfoNode = portalNode;

                            Portal portal = new Portal();

                            portal.ID = byte.Parse(portalNode.Name);
                            portal.Label = MapleData.GetString("pn");

                            portal.DestinationFieldID = MapleData.GetInt("tm");
                            portal.DestinationLabel = MapleData.GetString("tn");

                            portal.Position = new Position(MapleData.GetInt("x"), MapleData.GetInt("y"));

                            map.Portals.Add(portal);
                        }
                    }

                    if (mapNode.ContainsChild("life"))
                    {
                        int count = -1;

                        foreach (NXNode lifeNode in mapNode["life"])
                        {
                            count++;

                            MapleData.InfoNode = lifeNode;

                            string type = MapleData.GetString("type");

                            int mapleId = int.Parse(MapleData.GetString("id"));
                            Position position = new Position(MapleData.GetInt("x"), MapleData.GetInt("y"));
                            short foothold = MapleData.GetShort("fh");
                            short minimumClickX = MapleData.GetShort("rx0");
                            short maximumClickX = MapleData.GetShort("rx1");
                            int respawnTime = MapleData.GetInt("respawnTime");
                            bool facesLeft = MapleData.GetBool("f");

                            switch (type)
                            {
                                case "n":

                                    map.Npcs.Add(new Npc(type, count, mapleId, position, foothold, minimumClickX, maximumClickX, respawnTime, facesLeft));

                                    break;

                                case "m":
                                case "r":

                                    map.SpawnPoints.Add(new SpawnPoint(type, count, mapleId, position, foothold, minimumClickX, maximumClickX, respawnTime, facesLeft));

                                    break;
                            }
                        }
                    }

                    this.Add(map);
                }
            }
        }

        protected override int GetKeyForItem(Field item)
        {
            return item.MapleID;
        }
    }
}
