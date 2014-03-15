using Common;
using Common.Collections;
using Common.Net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Net;

namespace WvsGame.Maple.Characters
{
    public class CharacterSkills : NumericalKeyedCollection<Skill>
    {
        public Character Parent { get; private set; }

        public CharacterSkills(Character parent)
            : base()
        {
            this.Parent = parent;
        }

        protected override void InsertItem(int index, Skill item)
        {
            item.Parent = this;

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            Skill item = this.GetAtIndex(index);

            item.Parent = null;

            base.RemoveItem(index);
        }

        public void Append(Packet outPacket)
        {
            outPacket.WriteShort((short)this.Count);

            List<Skill> cooldownSkills = new List<Skill>();

            foreach (Skill skill in this)
            {
                outPacket.WriteInt(skill.MapleID);
                outPacket.WriteInt(skill.CurrentLevel);
                outPacket.WriteLong((long)ExpirationTime.DefaultTime);

                if (skill.IsCoolingDown)
                {
                    cooldownSkills.Add(skill);
                }
            }

            outPacket.WriteShort((short)cooldownSkills.Count);

            foreach (Skill cooldownSkill in cooldownSkills)
            {
                outPacket.WriteInt(cooldownSkill.MapleID);
                outPacket.WriteShort((short)cooldownSkill.RemainingCooldownSeconds);
            }
        }

        public void Load()
        {
            using (MySqlDataReader reader = GameServer.Database.RunQuery("SELECT * FROM skills WHERE CharacterID = '{0}'", this.Parent.ID))
            {
                while (reader.Read())
                {
                    this.Add(new Skill(reader));
                }
            }
        }

        public void Save()
        {
            foreach (Skill skill in this)
            {
                skill.Save();
            }
        }

        protected override int GetKeyForItem(Skill item)
        {
            return item.MapleID;
        }
    }
}
