using reNX;
using reNX.NXProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Commands;
using WvsGame.Maple.Events;
using WvsGame.Maple.Fields;
using WvsGame.Maple.Scripting;

namespace WvsGame.Maple.Data
{
    public static class MapleData
    {
        public static bool IsInitialzied { get; private set; }

        public static NXNode InfoNode { get; set; }

        public static CachedStyles CachedStyles { get; private set; }
        public static CachedNpcs CachedNpcs { get; private set; }
        public static CachedMobs CachedMobs { get; private set; }
        public static CachedItems CachedItems { get; private set; }
        public static CachedSkills CachedSkills {get; private set;}
        public static CachedFields CachedFields { get; private set; }
        public static CachedQuests CachedQuests { get; private set; }

        public static void Initialize()
        {
            MapleData.IsInitialzied = false;

            using (NXFile dataFile = new NXFile(@"..\DataSvr\WvsMaple.nx"))
            {
                CachedStyles = new CachedStyles(dataFile);
                CachedNpcs = new CachedNpcs(dataFile);
                CachedMobs = new CachedMobs(dataFile);
                CachedItems = new CachedItems(dataFile);
                CachedSkills = new CachedSkills(dataFile);
                CachedFields = new CachedFields(dataFile);
                CachedQuests = new CachedQuests(dataFile);
            }

            foreach (Field field in MapleData.CachedFields)
            {
                foreach (SpawnPoint spawnPoint in field.SpawnPoints)
                {
                    spawnPoint.Spawn(field);
                }
            }

            EventFactory.Initialize();
            CommandFactory.Initialize();
            ScriptingFactory.Initialize();

            MapleData.IsInitialzied = true;
        }

        public static byte GetByte(string childNode)
        {
            if (MapleData.InfoNode.ContainsChild(childNode))
            {
                return (byte)(MapleData.InfoNode[childNode].ValueOrDefault<Int64>(0));
            }
            else
            {
                return 0;
            }
        }

        public static sbyte GetSByte(string childNode)
        {
            if (MapleData.InfoNode.ContainsChild(childNode))
            {
                return (sbyte)(MapleData.InfoNode[childNode].ValueOrDefault<Int64>(0));
            }
            else
            {
                return 0;
            }
        }

        public static short GetShort(string childNode)
        {
            if (MapleData.InfoNode.ContainsChild(childNode))
            {
                return (short)(MapleData.InfoNode[childNode].ValueOrDefault<Int64>(0));
            }
            else
            {
                return 0;
            }
        }

        public static int GetInt(string childNode)
        {
            if (MapleData.InfoNode.ContainsChild(childNode))
            {
                return (int)(MapleData.InfoNode[childNode].ValueOrDefault<Int64>(0));
            }
            else
            {
                return 0;
            }
        }

        public static uint GetUInt(string childNode)
        {
            if (MapleData.InfoNode.ContainsChild(childNode))
            {
                return (uint)(MapleData.InfoNode[childNode].ValueOrDefault<Int64>(0));
            }
            else
            {
                return 0;
            }
        }

        public static double GetDouble(string childNode)
        {
            if (MapleData.InfoNode.ContainsChild(childNode))
            {
                return MapleData.InfoNode[childNode].ValueOrDefault<double>(0.0);
            }
            else
            {
                return 0.0;
            }
        }

        public static float GetFloat(string childNode)
        {
            if (MapleData.InfoNode.ContainsChild(childNode))
            {
                return (float)(MapleData.InfoNode[childNode].ValueOrDefault<Int64>(0));
            }
            else
            {
                return 0;
            }
        }

        public static bool GetBool(string childNode)
        {
            if (MapleData.InfoNode.ContainsChild(childNode))
            {
                return (MapleData.InfoNode[childNode].ValueOrDefault<Int64>(0) == 0 ? false : true);
            }
            else
            {
                return false;
            }
        }

        public static string GetString(string childNode)
        {
            if (MapleData.InfoNode.ContainsChild(childNode))
            {
                return MapleData.InfoNode[childNode].ValueOrDefault<string>(string.Empty);
            }
            else
            {
                return string.Empty;
            }
        }

        public static Position GetPosition(string childNode)
        {
            if (MapleData.InfoNode.ContainsChild(childNode))
            {
                Point lt = MapleData.InfoNode[childNode].ValueOrDefault<Point>(new Point(0, 0));

                return new Position(lt.X, lt.Y);
            }
            else
            {
                return new Position(0, 0);
            }
        }
    }
}
