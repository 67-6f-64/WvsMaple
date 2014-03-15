using Common;
using Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Characters;
using WvsGame.Maple.Data;
using WvsGame.Maple.Fields;

namespace WvsGame.Maple.Life
{
    public class Mob : FieldObject, ISpawnable
    {
        public int MapleID { get; set; }
        public Character Controller { get; set; }
        public Dictionary<Character, uint> Attackers { get; private set; }
        public SpawnPoint SpawnPoint { get; private set; }
        public bool IsProvoked { get; set; }
        public bool CanDrop { get; set; }
        public List<Loot> Loots { get; set; }
        public short Foothold { get; set; }
        public MobSkills Skills { get; private set; }
        public Dictionary<MobSkill, DateTime> Cooldowns { get; set; }
        public List<MobStatus> Buffs { get; set; }
        public List<int> DeathSummons { get; set; }

        public short Level { get; set; }
        public uint MaxHP { get; set; }
        public uint MaxMP { get; set; }
        public uint CurrentHP { get; set; }
        public uint CurrentMP { get; set; }
        public uint HpRecovery { get; set; }
        public uint MpRecovery { get; set; }
        public int ExplodeHP { get; set; }
        public uint Experience { get; set; }
        public int Link { get; set; }
        public short SummonType { get; set; }
        public int KnockBack { get; set; }
        public int FixedDamage { get; set; }
        public int DeathBuff { get; set; }
        public int DeathAfter { get; set; }
        public double Traction { get; set; }
        public int DamagedBySkillOnly { get;  set; }
        public int DamagedByMobOnly { get;  set; }
        public int DropItemPeriod { get;  set; }
        public sbyte HpBarForeColor { get;  set; }
        public sbyte HpBarBackColor { get;  set; }
        public short CarnivalPoints { get;  set; }
        public int WeaponAttack { get;  set; }
        public int WeaponDefense { get;  set; }
        public int MagicAttack { get;  set; }
        public int MagicDefense { get;  set; }
        public int Accuracy { get;  set; }
        public int Avoidability { get; set; }
        public short Speed { get; set; }
        public short ChaseSpeed { get; set; }

        public bool IsFacingLeft
        {
            get
            {
                return Math.Abs(this.Stance) % 2 == 1;
            }
        }

        public Mob CachedReference
        {
            get
            {
                return MapleData.CachedMobs[this.MapleID];
            }
        }

        public Mob() { }

        public Mob(int mapleId)
            : base()
        {
            this.MapleID = mapleId;

            this.Level = this.CachedReference.Level;
            //this.Flags = this.CachedReference.Flags;
            this.MaxHP = this.CachedReference.MaxHP;
            this.MaxMP = this.CachedReference.MaxMP;
            this.CurrentHP = this.CachedReference.CurrentHP;
            this.CurrentMP = this.CachedReference.CurrentMP;
            this.HpRecovery = this.CachedReference.HpRecovery;
            this.MpRecovery = this.CachedReference.MpRecovery;
            this.ExplodeHP = this.CachedReference.ExplodeHP;
            this.Experience = this.CachedReference.Experience;
            this.Link = this.CachedReference.Link;
            this.SummonType = this.CachedReference.SummonType;
            this.KnockBack = this.CachedReference.KnockBack;
            this.FixedDamage = this.CachedReference.FixedDamage;
            this.DeathBuff = this.CachedReference.DeathBuff;
            this.DeathAfter = this.CachedReference.DeathAfter;
            this.Traction = this.CachedReference.Traction;
            this.DamagedBySkillOnly = this.CachedReference.DamagedBySkillOnly;
            this.DamagedByMobOnly = this.CachedReference.DamagedByMobOnly;
            this.DropItemPeriod = this.CachedReference.DropItemPeriod;
            this.HpBarForeColor = this.CachedReference.HpBarForeColor;
            this.HpBarBackColor = this.CachedReference.HpBarBackColor;
            this.CarnivalPoints = this.CachedReference.CarnivalPoints;
            this.WeaponAttack = this.CachedReference.WeaponAttack;
            this.WeaponDefense = this.CachedReference.WeaponDefense;
            this.MagicAttack = this.CachedReference.MagicAttack;
            this.MagicDefense = this.CachedReference.MagicDefense;
            this.Accuracy = this.CachedReference.Accuracy;
            this.Avoidability = this.CachedReference.Avoidability;
            this.Speed = this.CachedReference.Speed;
            this.ChaseSpeed = this.CachedReference.ChaseSpeed;

            this.Loots = this.CachedReference.Loots;
            this.Skills = this.CachedReference.Skills;
            this.DeathSummons = this.CachedReference.DeathSummons;

            this.Attackers = new Dictionary<Character, uint>();
            this.Cooldowns = new Dictionary<MobSkill, DateTime>();
            this.Buffs = new List<MobStatus>();
            this.Stance = 5;
            this.CanDrop = true;
        }

        public Mob(SpawnPoint spawnPoint)
            : this(spawnPoint.MapleID)
        {
            this.SpawnPoint = spawnPoint;
            this.Foothold = spawnPoint.Foothold;
            this.Position = this.SpawnPoint.Position;
            this.Position.Y -= 1;
        }

        public Mob(int mapleId, Position position)
            : this(mapleId)
        {
            this.Foothold = 0; // TODO!
            this.Position = position;
            this.Position.Y -= 5;
        }

        public bool CanRespawn
        {
            get
            {
                return true; // TODO
            }
        }

        public int SpawnEffect { get; set; }

        public void AssignController()
        {
            if (this.Controller == null)
            {
                int leastControlled = int.MaxValue;
                Character newController = null;

                lock (this.Field.Characters)
                {
                    foreach (Character loopCharacter in this.Field.Characters)
                    {
                        if (loopCharacter.ControlledMobs.Count < leastControlled)
                        {
                            leastControlled = loopCharacter.ControlledMobs.Count;
                            newController = loopCharacter;
                        }
                    }
                }

                if (newController != null)
                {
                    this.IsProvoked = false;
                    newController.ControlledMobs.Add(this);
                }
            }
        }

        public void SwitchController(Character newController)
        {
            lock (this)
            {
                if (this.Controller != newController)
                {
                    this.Controller.ControlledMobs.Remove(this);
                    newController.ControlledMobs.Add(this);
                }
            }
        }

        public void Move(Packet inPacket)
        {

        }

        public Packet GetCreatePacket()
        {
            return this.GetInternalPacket(false, true);
        }

        public Packet GetSpawnPacket()
        {
            return this.GetInternalPacket(false, false);
        }

        public Packet GetDestroyPacket()
        {
            Packet destroy = new Packet(ServerMessages.MobLeaveField);

            destroy.WriteInt(this.ObjectID);
            destroy.WriteByte(1); // NOTE: 0 - Disappear, 1 - Fade Out, 2 - Special.
            destroy.WriteLong();
            destroy.WriteLong();

            return destroy;
        }

        public Packet GetInternalPacket(bool requestControl, bool fadeIn)
        {
            Packet outPacket = new Packet(requestControl ? ServerMessages.MobChangeController : ServerMessages.MobEnterField);

            if (requestControl)
            {
                outPacket.WriteByte((byte)(this.IsProvoked ? 2 : 1));
            }

            outPacket.WriteInt(this.ObjectID);
            outPacket.WriteByte((byte)(this.Controller == null ? 5 : 1));
            outPacket.WriteInt(this.MapleID);
            outPacket.WriteInt();
            outPacket.WriteShort(this.Position.X);
            outPacket.WriteShort(this.Position.Y);
            outPacket.WriteByte(this.Stance);
            outPacket.WriteShort(this.Foothold);
            outPacket.WriteShort(this.Foothold); // NOTE: Original foothold, doesn't actually matter.
            outPacket.WriteByte((byte)(fadeIn ? -2 : -1));

            return outPacket;
        }

        public Packet GetControlRequestPacket()
        {
            return this.GetInternalPacket(true, false);
        }

        public Packet GetControlCancelPacket()
        {
            Packet cancelControl = new Packet(ServerMessages.MobChangeController);

            cancelControl.WriteBool(false);
            cancelControl.WriteInt(this.ObjectID);

            return cancelControl;
        }
    }
}
