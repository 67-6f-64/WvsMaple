using Common;
using Common.Net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Net;

namespace WvsGame.Maple.Characters
{
    public class CharacterStatistics
    {
        public Character Parent { get; private set; }

        private byte level;
        private short job;
        private short strength;
        private short dexterity;
        private short intelligence;
        private short luck;
        private short currentHP;
        private short maxHP;
        private short currentMP;
        private short maxMP;
        private short availableAP;
        private short availableSP;
        private int experience;
        private short fame;

        public byte Level
        {
            get
            {
                return level;
            }

            set
            {
                level = value;

                if (this.Parent.IsInitialized)
                {
                    this.Parent.UpdateStatistics(StatisticType.Level);
                }
            }
        }

        public short Job
        {
            get
            {
                return job;
            }

            set
            {
                job = value;

                if (this.Parent.IsInitialized)
                {
                    this.Parent.UpdateStatistics(StatisticType.Job);
                }
            }
        }

        public short Strength 
        {
            get
            {
                return strength;
            }

            set
            {
                strength = value;

                if (this.Parent.IsInitialized)
                {
                    this.Parent.UpdateStatistics(StatisticType.Strength);
                }
            }
        }

        public short Dexterity
        {
            get
            {
                return dexterity;
            }

            set
            {
                dexterity = value;

                if (this.Parent.IsInitialized)
                {
                    this.Parent.UpdateStatistics(StatisticType.Dexterity);
                }
            }
        }

        public short Intelligence
        {
            get
            {
                return intelligence;
            }

            set
            {
                intelligence = value;

                if (this.Parent.IsInitialized)
                {
                    this.Parent.UpdateStatistics(StatisticType.Intelligence);
                }
            }
        }

        public short Luck
        {
            get
            {
                return luck;
            }

            set
            {
                luck = value;

                if (this.Parent.IsInitialized)
                {
                    this.Parent.UpdateStatistics(StatisticType.Luck);
                }
            }
        }

        public short CurrentHP
        {
            get
            {
                return currentHP;
            }

            set
            {
                currentHP = value;

                if (currentHP < 0)
                {
                    currentHP = 0;
                }

                if (this.Parent.IsInitialized)
                {
                    this.Parent.UpdateStatistics(StatisticType.CurrentHP);
                }
            }
        }

        public short MaxHP
        {
            get
            {
                return maxHP;
            }

            set
            {
                maxHP = value;

                if (this.Parent.IsInitialized)
                {
                    this.Parent.UpdateStatistics(StatisticType.MaxHP);
                }
            }
        }

        public short CurrentMP
        {
            get
            {
                return currentMP;
            }

            set
            {
                currentMP = value;

                if (currentMP < 0)
                {
                    currentMP = 0;
                }

                if (this.Parent.IsInitialized)
                {
                    this.Parent.UpdateStatistics(StatisticType.CurrentMP);
                }
            }
        }

        public short MaxMP
        {
            get
            {
                return maxMP;
            }

            set
            {
                maxMP = value;

                if (this.Parent.IsInitialized)
                {
                    this.Parent.UpdateStatistics(StatisticType.MaxMP);
                }
            }
        }

        public short AvailableAP
        {
            get
            {
                return availableAP;
            }
            set
            {
                availableAP = value;

                if (this.Parent.IsInitialized)
                {
                    this.Parent.UpdateStatistics(StatisticType.AvailableAP);
                }
            }
        }

        public short AvailableSP
        {
            get
            {
                return availableSP;
            }
            set
            {
                availableSP = value;

                if (this.Parent.IsInitialized)
                {
                    this.Parent.UpdateStatistics(StatisticType.AvailableSP);
                }
            }
        }

        public int Experience
        {
            get
            {
                return experience;
            }

            set
            {
                experience = value;

                if (this.Parent.IsInitialized)
                {
                    this.Parent.UpdateStatistics(StatisticType.Experience);
                }
            }
        }

        public short Fame
        {
            get
            {
                return fame;
            }

            set
            {
                fame = value;

                if (this.Parent.IsInitialized)
                {
                    this.Parent.UpdateStatistics(StatisticType.Fame);
                }
            }
        }

        public CharacterStatistics(Character parent)
        {
            this.Parent = parent;
        }

        public void Append(Packet outPacket)
        {
            outPacket.WriteInt(this.Parent.ID);
            outPacket.WriteString(this.Parent.Name, 13);
            outPacket.WriteByte(this.Parent.Gender);
            outPacket.WriteByte(this.Parent.Skin);
            outPacket.WriteInt(this.Parent.Face);
            outPacket.WriteInt(this.Parent.Hair);

            outPacket.WriteLong(); // TODO: Pets.

            outPacket.WriteByte(this.Level);
            outPacket.WriteShort(this.Job);
            outPacket.WriteShort(this.Strength);
            outPacket.WriteShort(this.Dexterity);
            outPacket.WriteShort(this.Intelligence);
            outPacket.WriteShort(this.Luck);
            outPacket.WriteShort(this.CurrentHP);
            outPacket.WriteShort(this.MaxHP);
            outPacket.WriteShort(this.CurrentMP);
            outPacket.WriteShort(this.MaxMP);
            outPacket.WriteShort(this.AvailableAP);
            outPacket.WriteShort(this.AvailableSP);
            outPacket.WriteInt(this.Experience);
            outPacket.WriteShort(this.Fame);
            outPacket.WriteInt(this.Parent.Field.MapleID);
            outPacket.WriteByte(this.Parent.SpawnPoint);

            outPacket.WriteByte(this.Parent.MaxBuddies);
        }

        public void Load()
        {
            using (MySqlDataReader reader = GameServer.Database.RunQuery("SELECT * FROM characters WHERE ID = '{0}'", this.Parent.ID))
            {
                if (reader.Read())
                {
                    this.Level = (byte)reader.GetInt16("Level");
                    this.Job = reader.GetInt16("Job");
                    this.Strength = reader.GetInt16("Strength");
                    this.Dexterity = reader.GetInt16("Dexterity");
                    this.Intelligence = reader.GetInt16("Intelligence");
                    this.Luck = reader.GetInt16("Luck");
                    this.CurrentHP = reader.GetInt16("CurrentHP");
                    this.MaxHP = reader.GetInt16("MaxHP");
                    this.CurrentMP = reader.GetInt16("CurrentMP");
                    this.MaxMP = reader.GetInt16("MaxMP");
                    this.AvailableAP = reader.GetInt16("AvailableAP");
                    this.AvailableSP = reader.GetInt16("AvailableSP");
                    this.Experience = reader.GetInt32("Experience");
                    this.Fame = reader.GetInt16("Fame");
                }
            }
        }

        public void Save() // TODO: Better saving.
        {
            string query = "UPDATE characters SET ";

            query += "Level = '" + this.Level + "', ";
            query += "Job = '" + this.Job + "', ";
            query += "Strength = '" + this.Strength + "', ";
            query += "Dexterity = '" + this.Dexterity + "', ";
            query += "Intelligence = '" + this.Intelligence + "', ";
            query += "Luck = '" + this.Luck + "', ";
            query += "CurrentHP = '" + this.CurrentHP + "', ";
            query += "MaxHP = '" + this.MaxHP + "', ";
            query += "CurrentMP = '" + this.CurrentMP + "', ";
            query += "MaxMP = '" + this.MaxMP + "', ";
            query += "AvailableAP = '" + this.AvailableAP + "', ";
            query += "AvailableSP = '" + this.AvailableSP + "', ";
            query += "Experience = '" + this.Experience + "', ";
            query += "Fame = '" + this.Fame + "' ";

            query += "WHERE ID = '" + this.Parent.ID + "'";

            GameServer.Database.RunQuery(query);
        }
    }
}
