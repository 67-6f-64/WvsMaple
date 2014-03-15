using Common.Net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WvsGame.Maple.Characters;
using WvsGame.Maple.Data;
using WvsGame.Maple.Fields;
using WvsGame.Net;

namespace WvsGame.Maple
{
    public class Skill
    {
        public CharacterSkills Parent { get; set; }

        private byte currentLevel;
        private byte maxLevel;
        private DateTime cooldownEnd = DateTime.MinValue;

        public int ID { get; private set; }
        public int MapleID { get; set; }

        public sbyte MobCount { get; set; }
        public sbyte HitCount { get; set; }
        public short Range { get; set; }
        public int BuffTime { get; set; }
        public short CostMP { get; set; }
        public short CostHP { get; set; }
        public short Damage { get; set; }
        public int FixedDamage { get; set; }
        public byte CriticalDamage { get; set; }
        public sbyte Mastery { get; set; }
        public int OptionalItemCost { get; set; }
        public int CostItem { get; set; }
        public short ItemCount { get; set; }
        public short CostBullet { get; set; }
        public short CostMeso { get; set; }
        public short ParameterA { get; set; }
        public short ParameterB { get; set; }
        public short Speed { get; set; }
        public short Jump { get; set; }
        public short Strength { get; set; }
        public short WeaponAttack { get; set; }
        public short WeaponDefense { get; set; }
        public short MagicAttack { get; set; }
        public short MagicDefense { get; set; }
        public short Accuracy { get; set; }
        public short Avoid { get; set; }
        public short HP { get; set; }
        public short MP { get; set; }
        public short Probability { get; set; }
        public short Morph { get; set; }
        public Position LT { get; set; }
        public Position RB { get; set; }
        public int Cooldown { get; set; }

        public Character Character
        {
            get
            {
                return this.Parent.Parent;
            }
        }

        public bool HasBuff
        {
            get
            {
                return this.BuffTime > 0;
            }
        }

        public byte CurrentLevel
        {
            get
            {
                return currentLevel;
            }
            set
            {
                currentLevel = value;

                if (this.Parent != null)
                {
                    this.Recalculate();

                    if (this.Character.IsInitialized)
                    {
                        this.Update();
                    }
                }
            }
        }

        public byte MaxLevel
        {
            get
            {
                return maxLevel;
            }
            set
            {
                maxLevel = value;

                if (this.Parent != null && this.Character.IsInitialized)
                {
                    this.Update();
                }
            }
        }

        public bool IsFromBeginner
        {
            get
            {
                return this.MapleID % 10000000 > 999 && this.MapleID % 10000000 < 1003;
            }
        }

        public bool IsCoolingDown
        {
            get
            {
                return DateTime.Now < this.CooldownEnd;
            }
        }

        public int RemainingCooldownSeconds
        {
            get
            {
                return Math.Min(0, (int)(this.CooldownEnd - DateTime.Now).TotalSeconds);
            }
        }

        public Skill CachedReference
        {
            get
            {
                return MapleData.CachedSkills[this.MapleID][this.CurrentLevel];
            }
        }

        public DateTime CooldownEnd
        {
            get
            {
                return cooldownEnd;
            }
            set
            {
                cooldownEnd = value;

                //if (this.IsCoolingDown)
                //{
                //    using (Packet outPacket = new Packet(MapleServerOperationCode.Cooldown))
                //    {
                //        outPacket.WriteInt(this.MapleID);
                //        outPacket.WriteInt((short)this.RemainingCooldownSeconds);

                //        this.Character.Client.Send(outPacket);
                //    }

                //    Delay.Execute(this.RemainingCooldownSeconds * 1000, () =>
                //    {
                //        using (Packet outPacket = new Packet(MapleServerOperationCode.Cooldown))
                //        {
                //            outPacket.WriteInt(this.MapleID);
                //            outPacket.WriteInt(0);

                //            this.Character.Client.Send(outPacket);
                //        }
                //    });
                //}
            }
        }

        public Skill() { }

        public Skill(MySqlDataReader reader)
        {
            this.Assigned = true;

            this.ID = reader.GetInt32("ID");
            this.MapleID = reader.GetInt32("MapleID");
            this.CurrentLevel = reader.GetByte("CurrentLevel");
            this.MaxLevel = reader.GetByte("MaxLevel");
            this.CooldownEnd = reader.GetDateTime("CooldownEnd");
        }

        public Skill(int mapleId) // Used for used for non-fourth-job skills
        {
            this.MapleID = mapleId;
            this.CurrentLevel = 0;
            this.MaxLevel = (byte)MapleData.CachedSkills[this.MapleID].Count;
        }

        public void Update()
        {
            using (Packet outPacket = new Packet(ServerMessages.ChangeSkillRecordResult))
            {
                outPacket.WriteByte(1);
                outPacket.WriteShort(1);
                outPacket.WriteInt(this.MapleID);
                outPacket.WriteInt(this.CurrentLevel);
                outPacket.WriteByte(1);

                this.Character.Client.Send(outPacket);
            }
        }

        public void Recalculate()
        {
            this.MobCount = this.CachedReference.MobCount;
            this.HitCount = this.CachedReference.HitCount;
            this.Range = this.CachedReference.Range;
            this.BuffTime = this.CachedReference.BuffTime;
            this.CostMP = this.CachedReference.CostMP;
            this.CostHP = this.CachedReference.CostHP;
            this.Damage = this.CachedReference.Damage;
            this.FixedDamage = this.CachedReference.FixedDamage;
            this.CriticalDamage = this.CachedReference.CriticalDamage;
            this.Mastery = this.CachedReference.Mastery;
            this.OptionalItemCost = this.CachedReference.OptionalItemCost;
            this.CostItem = this.CachedReference.CostItem;
            this.ItemCount = this.CachedReference.ItemCount;
            this.CostBullet = this.CachedReference.CostBullet;
            this.CostMeso = this.CachedReference.CostMeso;
            this.ParameterA = this.CachedReference.ParameterA;
            this.ParameterB = this.CachedReference.ParameterB;
            this.Speed = this.CachedReference.Speed;
            this.Jump = this.CachedReference.Jump;
            this.Strength = this.CachedReference.Strength;
            this.WeaponAttack = this.CachedReference.WeaponAttack;
            this.WeaponDefense = this.CachedReference.WeaponDefense;
            this.MagicAttack = this.CachedReference.MagicAttack;
            this.MagicDefense = this.CachedReference.MagicDefense;
            this.Accuracy = this.CachedReference.Accuracy;
            this.Avoid = this.CachedReference.Avoid;
            this.HP = this.CachedReference.HP;
            this.MP = this.CachedReference.MP;
            this.Probability = this.CachedReference.Probability;
            this.Morph = this.CachedReference.Morph;
            this.LT = this.CachedReference.LT;
            this.RB = this.CachedReference.RB;
            this.Cooldown = this.CachedReference.Cooldown;
        }

        public void Cast()
        {

        }

        public void Save()
        {
            StringBuilder query = new StringBuilder();

            if (this.Assigned)
            {
                query.Append("UPDATE skills SET ");
                query.AppendFormat("{0} = '{1}', ", "CharacterID", this.Character.ID);
                query.AppendFormat("{0} = '{1}', ", "MapleID", this.MapleID);
                query.AppendFormat("{0} = '{1}', ", "CurrentLevel", this.CurrentLevel);
                query.AppendFormat("{0} = '{1}', ", "MaxLevel", this.MaxLevel);
                query.AppendFormat("{0} = '{1}' ", "CooldownEnd", this.CooldownEnd);
                query.AppendFormat("WHERE ID = '{0}'", this.ID);
            }
            else
            {
                query.Append("INSERT INTO skills (CharacterID, MapleID, CurrentLevel, MaxLevel, CooldownEnd) VALUES (");
                query.AppendFormat("'{0}', ", this.Character.ID);
                query.AppendFormat("'{0}', ", this.MapleID);
                query.AppendFormat("'{0}', ", this.CurrentLevel);
                query.AppendFormat("'{0}', ", this.MaxLevel);
                query.AppendFormat("'{0}')", this.CooldownEnd);

                this.ID = GameServer.Database.Fetch("skills", "ID", "CharacterID = '{0}' && MapleID = '{1}'", this.Character.ID, this.MapleID);

                this.Assigned = true;
            }

            GameServer.Database.RunQuery(query.ToString());
        }

        private bool Assigned { get; set; } // TODO: Move the assigneds somewhere good.

    }
}
