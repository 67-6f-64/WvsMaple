using Common;
using Common.Net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Characters;
using WvsGame.Maple.Data;
using WvsGame.Maple.Fields;
using WvsGame.Net;

namespace WvsGame.Maple
{
    public class MobSummon
    {
        public int MapleID { get; set; }
        public byte Probability { get; set; }
    }

    public class Item : Drop
    {
        public int ID { get; set; }
        public CharacterInventory Parent { get; set; }
        public int MapleID { get; set; }
        private short maxPerStack;
        private short quantity;
        public sbyte Slot { get; set; }
        public string Creator { get; private set; }
        public bool IsStored { get; private set; }

        public int MesoReward { get; set; }

        public bool IsCash { get; set; }
        public bool OnlyOne { get; set; }
        public int SalePrice { get; set; }

        public byte UpgradesAvailable { get; set; }
        public byte UpgradesApplied { get; set; }
        public byte MaxUpgradesAvailable { get; set; }
        public short Strength { get; set; }
        public short Dexterity { get; set; }
        public short Intelligence { get; set; }
        public short Luck { get; set; }
        public short HP { get; set; }
        public short MP { get; set; }
        public short WeaponAttack { get; set; }
        public short MagicAttack { get; set; }
        public short WeaponDefense { get; set; }
        public short MagicDefense { get; set; }
        public short Accuracy { get; set; }
        public short Avoidability { get; set; }
        public short Agility { get; set; }
        public short Speed { get; set; }
        public short Jump { get; set; }

        public sbyte AttackSpeed { get; set; }
        public short RecoveryRate { get; set; }
        public short KnockBackChance { get; set; }

        public short RequiredLevel { get; set; }
        public short RequiredStrength { get; set; }
        public short RequiredDexterity { get; set; }
        public short RequiredIntelligence { get; set; }
        public short RequiredLuck { get; set; }
        public short RequiredFame { get; set; }
        public short RequiredJob { get; set; }

        public int Success { get; set; }
        public int BreakItem { get; set; }
        public string Flag { get; set; }
        public short IStrength { get; set; }
        public short IDexterity { get; set; }
        public short IIntelligence { get; set; }
        public short ILuck { get; set; }
        public short IHP { get; set; }
        public short IMP { get; set; }
        public short IWeaponAttack { get; set; }
        public short IMagicAttack { get; set; }
        public short IWeaponDefense { get; set; }
        public short IMagicDefense { get; set; }
        public short IAccuracy { get; set; }
        public short IAvoidability { get; set; }
        public short IJump { get; set; }
        public short ISpeed { get; set; }

        public int CItemId { get; set; }
        public byte CFlags { get; set; }
        public string CCureAilments { get; set; }
        public short CEffect { get; set; }
        public short CHP { get; set; }
        public short CMP { get; set; }
        public short CHPPercentage { get; set; }
        public short CMPPercentage { get; set; }
        public int CMoveTo { get; set; }
        public short CProb { get; set; }
        public int CBuffTime { get; set; }
        public short CWeaponAttack { get; set; }
        public short CMagicAttack { get; set; }
        public short CWeaponDefense { get; set; }
        public short CMagicDefense { get; set; }
        public short CAccuracy { get; set; }
        public short CAvoid { get; set; }
        public short CSpeed { get; set; }
        public short CJump { get; set; }
        public short CMorph { get; set; }

        public byte Flags
        {
            get
            {
                byte flags = 0;

                return flags;
            }
        }

        public Pet Pet { get; set; }

        public List<MobSummon> CSummons { get; set; }

        public long UniqueID { get; set; }

        public Item() { }

        public Item(int mapleID, short quantity = 1, bool equipped = false)
        {
            this.MapleID = mapleID;
            this.MaxPerStack = this.CachedReference.MaxPerStack;
            this.Quantity = (this.Type == ItemType.Equipment) ? (short)1 : quantity;
            if (equipped) this.Slot = (sbyte)this.GetEquippedSlot();
            this.IsStored = false;
            this.Creator = string.Empty;

            this.MesoReward = this.CachedReference.MesoReward;
            this.IsCash = this.CachedReference.IsCash;
            this.OnlyOne = this.CachedReference.OnlyOne;
            this.SalePrice = this.CachedReference.SalePrice;
            this.RequiredLevel = this.CachedReference.RequiredLevel;

            this.UniqueID = this.CachedReference.UniqueID;

            if (this.Type == ItemType.Equipment)
            {
                this.AttackSpeed = this.CachedReference.AttackSpeed;
                this.RecoveryRate = this.CachedReference.RecoveryRate;
                this.KnockBackChance = this.CachedReference.KnockBackChance;

                this.RequiredStrength = this.CachedReference.RequiredStrength;
                this.RequiredDexterity = this.CachedReference.RequiredDexterity;
                this.RequiredIntelligence = this.CachedReference.RequiredIntelligence;
                this.RequiredLuck = this.CachedReference.RequiredLuck;
                this.RequiredFame = this.CachedReference.RequiredFame;
                this.RequiredJob = this.CachedReference.RequiredJob;

                this.UpgradesAvailable = this.CachedReference.MaxUpgradesAvailable;
                this.UpgradesApplied = this.CachedReference.UpgradesApplied;

                this.Strength = this.CachedReference.Strength;
                this.Dexterity = this.CachedReference.Dexterity;
                this.Intelligence = this.CachedReference.Intelligence;
                this.Luck = this.CachedReference.Luck;
                this.HP = this.CachedReference.HP;
                this.MP = this.CachedReference.MP;
                this.WeaponAttack = this.CachedReference.WeaponAttack;
                this.MagicAttack = this.CachedReference.MagicAttack;
                this.WeaponDefense = this.CachedReference.WeaponDefense;
                this.MagicDefense = this.CachedReference.MagicDefense;
                this.Accuracy = this.CachedReference.Accuracy;
                this.Avoidability = this.CachedReference.Avoidability;
                this.Agility = this.CachedReference.Agility;
                this.Speed = this.CachedReference.Speed;
                this.Jump = this.CachedReference.Jump;
            }
            else if (this.IsScroll)
            {
                this.Success = this.CachedReference.Success;
                this.BreakItem = this.CachedReference.BreakItem;
                this.Flag = this.CachedReference.Flag;
                this.IStrength = this.CachedReference.IStrength;
                this.IDexterity = this.CachedReference.IDexterity;
                this.IIntelligence = this.CachedReference.IIntelligence;
                this.ILuck = this.CachedReference.ILuck;
                this.IHP = this.CachedReference.IHP;
                this.IMP = this.CachedReference.IMP;
                this.IWeaponAttack = this.CachedReference.IWeaponAttack;
                this.IMagicAttack = this.CachedReference.IMagicAttack;
                this.IWeaponDefense = this.CachedReference.IWeaponDefense;
                this.IMagicDefense = this.CachedReference.IMagicDefense;
                this.IAccuracy = this.CachedReference.IAccuracy;
                this.IAvoidability = this.CachedReference.IAvoidability;
                this.IJump = this.CachedReference.IJump;
                this.ISpeed = this.CachedReference.ISpeed;
            }
            else if (this.IsItemConsume)
            {
                this.CHP = this.CachedReference.CHP;
                this.CMP = this.CachedReference.CMP;
                this.CHPPercentage = this.CachedReference.CHPPercentage;
                this.CMPPercentage = this.CachedReference.CMPPercentage;
                this.CMoveTo = this.CachedReference.CMoveTo;
                this.CProb = this.CachedReference.CProb;
                this.CBuffTime = this.CachedReference.CBuffTime;
                this.CWeaponAttack = this.CachedReference.CWeaponAttack;
                this.CMagicAttack = this.CachedReference.CMagicAttack;
                this.CWeaponDefense = this.CachedReference.CWeaponDefense;
                this.CMagicDefense = this.CachedReference.CMagicDefense;
                this.CAccuracy = this.CachedReference.CAccuracy;
                this.CAvoid = this.CachedReference.CAvoid;
                this.CSpeed = this.CachedReference.CSpeed;
                this.CJump = this.CachedReference.CJump;
                this.CMorph = this.CachedReference.CMorph;
                this.CSummons = this.CachedReference.CSummons;
            }
        }

        public void Load(MySqlDataReader reader)
        {
            this.Assigned = true;

            this.ID = reader.GetInt32("ID");
            this.MapleID = reader.GetInt32("MapleID");
            this.MaxPerStack = this.CachedReference.MaxPerStack;
            this.Quantity = reader.GetInt16("Quantity");
            this.Slot = reader.GetSByte("Slot");
            this.IsStored = reader.GetBoolean("IsStored");
            this.Creator = reader.GetString("Creator");

            this.IsCash = this.CachedReference.IsCash;
            this.OnlyOne = this.CachedReference.OnlyOne;
            this.SalePrice = this.CachedReference.SalePrice;

            this.UniqueID = reader.GetInt64("UniqueID");

            if (this.Type == ItemType.Equipment)
            {
                this.RequiredLevel = this.CachedReference.RequiredLevel;

                this.AttackSpeed = this.CachedReference.AttackSpeed;
                this.RecoveryRate = this.CachedReference.RecoveryRate;
                this.KnockBackChance = this.CachedReference.KnockBackChance;

                this.RequiredStrength = this.CachedReference.RequiredStrength;
                this.RequiredDexterity = this.CachedReference.RequiredDexterity;
                this.RequiredIntelligence = this.CachedReference.RequiredIntelligence;
                this.RequiredLuck = this.CachedReference.RequiredLuck;
                this.RequiredFame = this.CachedReference.RequiredFame;
                this.RequiredJob = this.CachedReference.RequiredJob;

                this.UpgradesAvailable = reader.GetByte("UpgradesAvailable");
                this.UpgradesApplied = reader.GetByte("UpgradesApplied");
                this.HP = reader.GetInt16("HP");
                this.MP = reader.GetInt16("MP");
                this.Strength = reader.GetInt16("Strength");
                this.Dexterity = reader.GetInt16("Dexterity");
                this.Intelligence = reader.GetInt16("Intelligence");
                this.Luck = reader.GetInt16("Luck");
                this.Agility = reader.GetInt16("Agility");
                this.WeaponAttack = reader.GetInt16("WeaponAttack");
                this.WeaponDefense = reader.GetInt16("WeaponDefense");
                this.MagicAttack = reader.GetInt16("MagicAttack");
                this.MagicDefense = reader.GetInt16("MagicDefense");
                this.Accuracy = reader.GetInt16("Accuracy");
                this.Avoidability = reader.GetInt16("Avoidability");
                this.Jump = reader.GetInt16("Jump");
                this.Speed = reader.GetInt16("Speed");
            }
            else if (this.IsScroll)
            {
                this.Success = this.CachedReference.Success;
                this.BreakItem = this.CachedReference.BreakItem;
                this.Flag = this.CachedReference.Flag;
                this.IStrength = this.CachedReference.IStrength;
                this.IDexterity = this.CachedReference.IDexterity;
                this.IIntelligence = this.CachedReference.IIntelligence;
                this.ILuck = this.CachedReference.ILuck;
                this.IHP = this.CachedReference.IHP;
                this.IMP = this.CachedReference.IMP;
                this.IWeaponAttack = this.CachedReference.IWeaponAttack;
                this.IMagicAttack = this.CachedReference.IMagicAttack;
                this.IWeaponDefense = this.CachedReference.IWeaponDefense;
                this.IMagicDefense = this.CachedReference.IMagicDefense;
                this.IAccuracy = this.CachedReference.IAccuracy;
                this.IAvoidability = this.CachedReference.IAvoidability;
                this.IJump = this.CachedReference.IJump;
                this.ISpeed = this.CachedReference.ISpeed;
            }
            else if (this.IsItemConsume)
            {
                this.CFlags = this.CachedReference.CFlags;
                this.CCureAilments = this.CachedReference.CCureAilments;
                this.CEffect = this.CachedReference.CEffect;
                this.CHP = this.CachedReference.CHP;
                this.CMP = this.CachedReference.CMP;
                this.CHPPercentage = this.CachedReference.CHPPercentage;
                this.CMPPercentage = this.CachedReference.CMPPercentage;
                this.CMoveTo = this.CachedReference.CMoveTo;
                this.CProb = this.CachedReference.CProb;
                this.CBuffTime = this.CachedReference.CBuffTime;
                this.CWeaponAttack = this.CachedReference.CWeaponAttack;
                this.CMagicAttack = this.CachedReference.CMagicAttack;
                this.CWeaponDefense = this.CachedReference.CWeaponDefense;
                this.CMagicDefense = this.CachedReference.CMagicDefense;
                this.CAccuracy = this.CachedReference.CAccuracy;
                this.CAvoid = this.CachedReference.CAvoid;
                this.CSpeed = this.CachedReference.CSpeed;
                this.CJump = this.CachedReference.CJump;
                this.CMorph = this.CachedReference.CMorph;
                this.CSummons = this.CachedReference.CSummons;
            }
        }

        public void Save()
        {
            if (this.Assigned)
            {
                StringBuilder query = new StringBuilder();

                query.AppendFormat("UPDATE items SET ");
                query.AppendFormat("CharacterID = '{0}', ", this.Character.ID);
                query.AppendFormat("MapleID = '{0}', ", this.MapleID);
                query.AppendFormat("Quantity = '{0}', ", this.Quantity);
                query.AppendFormat("Slot = '{0}', ", this.Slot);
                query.AppendFormat("IsStored = '{0}', ", this.IsStored);
                query.AppendFormat("Creator = '{0}', ", this.Creator);
                query.AppendFormat("UpgradesAvailable = '{0}', ", this.UpgradesAvailable);
                query.AppendFormat("UpgradesApplied = '{0}', ", this.UpgradesApplied);
                query.AppendFormat("HP = '{0}', ", this.HP);
                query.AppendFormat("MP = '{0}', ", this.MP);
                query.AppendFormat("Strength = '{0}', ", this.Strength);
                query.AppendFormat("Dexterity = '{0}', ", this.Dexterity);
                query.AppendFormat("Intelligence = '{0}', ", this.Intelligence);
                query.AppendFormat("Luck = '{0}', ", this.Luck);
                query.AppendFormat("WeaponAttack = '{0}', ", this.WeaponAttack);
                query.AppendFormat("WeaponDefense = '{0}', ", this.WeaponDefense);
                query.AppendFormat("MagicAttack = '{0}', ", this.MagicAttack);
                query.AppendFormat("MagicDefense = '{0}', ", this.MagicDefense);
                query.AppendFormat("Accuracy = '{0}', ", this.Accuracy);
                query.AppendFormat("Avoidability = '{0}', ", this.Avoidability);
                query.AppendFormat("Jump = '{0}', ", this.Jump);
                query.AppendFormat("Speed = '{0}', ", this.Speed);
                query.AppendFormat("UniqueID = '{0}' ", this.UniqueID);
                query.AppendFormat("WHERE ID = '{0}'", this.ID);

                GameServer.Database.RunQuery(query.ToString());
            }
            else
            {
                StringBuilder query = new StringBuilder();

                query.AppendFormat("INSERT INTO items (CharacterID, MapleID, Quantity, Slot, IsStored, Creator, UpgradesAvailable, UpgradesApplied, HP, MP, Strength, Dexterity, Intelligence, Luck, WeaponAttack, WeaponDefense, MagicAttack, MagicDefense, Accuracy, Avoidability, Jump, Speed, UniqueID) VALUES (");
                query.AppendFormat("'{0}', ", this.Character.ID);
                query.AppendFormat("'{0}', ", this.MapleID);
                query.AppendFormat("'{0}', ", this.Quantity);
                query.AppendFormat("'{0}', ", this.Slot);
                query.AppendFormat("'{0}', ", this.IsStored);
                query.AppendFormat("'{0}', ", this.Creator);
                query.AppendFormat("'{0}', ", this.UpgradesAvailable);
                query.AppendFormat("'{0}', ", this.UpgradesApplied);
                query.AppendFormat("'{0}', ", this.HP);
                query.AppendFormat("'{0}', ", this.MP);
                query.AppendFormat("'{0}', ", this.Strength);
                query.AppendFormat("'{0}', ", this.Dexterity);
                query.AppendFormat("'{0}', ", this.Intelligence);
                query.AppendFormat("'{0}', ", this.Luck);
                query.AppendFormat("'{0}', ", this.WeaponAttack);
                query.AppendFormat("'{0}', ", this.WeaponDefense);
                query.AppendFormat("'{0}', ", this.MagicAttack);
                query.AppendFormat("'{0}', ", this.MagicDefense);
                query.AppendFormat("'{0}', ", this.Accuracy);
                query.AppendFormat("'{0}', ", this.Avoidability);
                query.AppendFormat("'{0}', ", this.Jump);
                query.AppendFormat("'{0}', ", this.Speed);
                query.AppendFormat("'{0}')", this.UniqueID);

                GameServer.Database.RunQuery(query.ToString());

                this.ID = GameServer.Database.GetLastInsertId();
                this.Assigned = true;
            }
        }

        public void Delete()
        {
            GameServer.Database.Delete("items", "ID = '{0}'", this.ID);
        }

        public void Append(Packet p, bool zeroPosition = false, bool leaveOut = false, bool shortSlot = true)
        {
            if (!zeroPosition && !leaveOut)
            {
                p.WriteByte(this.ComputedSlot);
            }

            p.WriteByte((byte)(this.Type == ItemType.Equipment ? 1 : 2));
            p.WriteInt(this.MapleID);
            p.WriteBool(this.IsCash);

            if (this.IsCash)
            {
                p.WriteLong(this.UniqueID);
            }

            p.WriteLong((long)ExpirationTime.DefaultTime);

            if (this.Type == ItemType.Equipment)
            {
                p.WriteByte(this.UpgradesAvailable);
                p.WriteByte(this.UpgradesApplied);
                p.WriteShort(this.Strength);
                p.WriteShort(this.Dexterity);
                p.WriteShort(this.Intelligence);
                p.WriteShort(this.Luck);
                p.WriteShort(this.HP);
                p.WriteShort(this.MP);
                p.WriteShort(this.WeaponAttack);
                p.WriteShort(this.MagicAttack);
                p.WriteShort(this.WeaponDefense);
                p.WriteShort(this.MagicDefense);
                p.WriteShort(this.Accuracy);
                p.WriteShort(this.Avoidability);
                p.WriteShort(this.Agility);
                p.WriteShort(this.Speed);
                p.WriteShort(this.Jump);
                p.WriteString(this.Creator); 
                p.WriteShort(this.Flags);
            }
            else if (this.Type == ItemType.Cash)
            {
                p.WriteString(this.Pet.Name, 13);
                p.WriteByte(this.Pet.Level);
                p.WriteShort(this.Pet.Closeness);
                p.WriteByte(this.Pet.Fullness);
                p.WriteLong(this.Pet.Expiration);
            }
            else
            {
                p.WriteShort(this.Quantity);
                p.WriteString(this.Creator);
                p.WriteShort(this.Flags);
            }
        }

        public void Equip()
        {
            if ((this.Character.Statistics.Strength < this.RequiredStrength ||
                this.Character.Statistics.Dexterity < this.RequiredDexterity ||
                this.Character.Statistics.Intelligence < this.RequiredIntelligence ||
                this.Character.Statistics.Luck < this.RequiredLuck))
            {
                return; // TODO: Hacking.
            }
            else
            {
                sbyte sourceSlot = this.Slot;
                EquipmentSlot destinationSlot = this.GetEquippedSlot();

                Item destination = this.Parent[destinationSlot];


                if (destination != null)
                {
                    destination.Slot = sourceSlot;
                }

                this.Slot = (sbyte)destinationSlot;

                using (Packet p = new Packet(ServerMessages.InventoryOperation))
                {
                    p.WriteByte(1);
                    p.WriteByte(1);
                    p.WriteByte(2);
                    p.WriteByte(this.Inventory);
                    p.WriteShort((short)sourceSlot);
                    p.WriteShort((short)destinationSlot);
                    p.WriteByte();

                    this.Character.Client.Send(p);
                }

                this.Character.UpdateLooks();
            }
        }

        public void Unequip(sbyte destinationSlot = 0)
        {
            sbyte sourceSlot = this.Slot;

            if (destinationSlot == 0)
            {
                destinationSlot = this.Parent.GetNextFreeSlot(this.Type);
            }

            this.Slot = destinationSlot;

            using (Packet p = new Packet(ServerMessages.InventoryOperation))
            {
                p.WriteByte(1);
                p.WriteByte(1);
                p.WriteByte(2);
                p.WriteByte(this.Inventory);
                p.WriteShort((short)sourceSlot);
                p.WriteShort((short)destinationSlot);
                p.WriteByte();

                this.Character.Client.Send(p);
            }

            this.Character.UpdateLooks();
        }

        public void Move(sbyte destinationSlot)
        {
            sbyte sourceSlot = this.Slot;

            Item destination = this.Parent[(ItemType)this.Type, destinationSlot];

            if (destination != null &&
                (ItemType)this.Type != ItemType.Equipment &&
                this.MapleID == destination.MapleID &&
                destination.Quantity < destination.MaxPerStack)
            {
                if (this.Quantity + destination.Quantity > destination.MaxPerStack)
                {
                    this.Quantity -= (short)(destination.MaxPerStack - destination.Quantity);
                    destination.Quantity = destination.MaxPerStack;

                    // Update quantity and shit packet.
                }
                else
                {
                    destination.Quantity += this.Quantity;

                    this.Parent.Remove(this, false);
                }
            }
            else
            {
                if (destination != null)
                {
                    destination.Slot = sourceSlot;
                }

                this.Slot = destinationSlot;

                using (Packet p = new Packet(ServerMessages.InventoryOperation))
                {
                    p.WriteByte(1);
                    p.WriteByte(1);
                    p.WriteByte(2);
                    p.WriteByte(this.Inventory);
                    p.WriteShort(sourceSlot);
                    p.WriteShort(destinationSlot);
                    p.WriteByte();

                    this.Character.Client.Send(p);
                }
            }
        }

        public void Drop(short quantity)
        {
            if (quantity == this.Quantity)
            {
                using (Packet p = new Packet(ServerMessages.InventoryOperation))
                {
                    p.WriteByte(1);
                    p.WriteByte(1);
                    p.WriteByte(2);
                    p.WriteByte(this.Inventory);
                    p.WriteShort(this.Slot);
                    p.WriteShort();
                    p.WriteByte();

                    this.Character.Client.Send(p);
                }

                this.Dropper = this.Character;
                this.Owner = null;

                this.Character.Field.Drops.Add(this);
                this.Parent.Remove(this, false);
            }
            else if (quantity < this.Quantity)
            {
                this.Quantity -= quantity;

                using (Packet p = new Packet(ServerMessages.InventoryOperation))
                {
                    p.WriteByte(1);
                    p.WriteByte(1);
                    p.WriteBool(true);
                    p.WriteByte(this.Inventory);
                    p.WriteShort(this.Slot);
                    p.WriteShort(this.Quantity);
                    p.WriteLong();
                    p.WriteLong();
                    p.WriteLong();

                    this.Character.Client.Send(p);
                }

                Item drop = new Item(this.MapleID, quantity)
                {
                    Dropper = this.Character,
                    Owner = null
                };

                this.Character.Field.Drops.Add(drop);
            }
            else if (quantity > this.Quantity)
            {
                return; // TODO: Hacking.
            }
        }

        public void Update(bool isNew = false)
        {
            using (Packet p = new Packet(ServerMessages.InventoryOperation))
            {
                p.WriteByte(1);
                p.WriteByte(1);
                p.WriteBool(!isNew);
                p.WriteByte((byte)(this.Inventory));

                if (isNew)
                {
                    this.Append(p, shortSlot: true);
                }
                else
                {
                    p.WriteShort(this.Slot);
                    p.WriteShort(this.Quantity);
                }

                p.WriteLong();
                p.WriteLong();
                p.WriteLong();

                this.Character.Client.Send(p);
            }
        }

        private EquipmentSlot GetEquippedSlot()
        {
            sbyte position = 0;

            if (this.MapleID >= 1000000 && this.MapleID < 1010000)
            {
                position -= 1;
            }
            else if (this.MapleID >= 1010000 && this.MapleID < 1020000)
            {
                position -= 2;
            }
            else if (this.MapleID >= 1020000 && this.MapleID < 1030000)
            {
                position -= 3;
            }
            else if (this.MapleID >= 1030000 && this.MapleID < 1040000)
            {
                position -= 4;
            }
            else if (this.MapleID >= 1040000 && this.MapleID < 1060000)
            {
                position -= 5;
            }
            else if (this.MapleID >= 1060000 && this.MapleID < 1070000)
            {
                position -= 6;
            }
            else if (this.MapleID >= 1070000 && this.MapleID < 1080000)
            {
                position -= 7;
            }
            else if (this.MapleID >= 1080000 && this.MapleID < 1090000)
            {
                position -= 8;
            }
            else if (this.MapleID >= 1102000 && this.MapleID < 1103000)
            {
                position -= 9;
            }
            else if (this.MapleID >= 1092000 && this.MapleID < 1100000)
            {
                position -= 10;
            }
            else if (this.MapleID >= 1300000 && this.MapleID < 1800000)
            {
                position -= 11;
            }
            else if (this.MapleID >= 1112000 && this.MapleID < 1120000)
            {
                position -= 12;
            }
            else if (this.MapleID >= 1122000 && this.MapleID < 1123000)
            {
                position -= 17;
            }
            else if (this.MapleID >= 1900000 && this.MapleID < 2000000)
            {
                position -= 18;
            }

            if (this.IsCash)
            {
                position -= 100;
            }

            return (EquipmentSlot)position;
        }

        public override Packet GetShowGainPacket()
        {
            using (Packet p = new Packet(ServerMessages.Message))
            {
                p.WriteByte();
                p.WriteByte();
                p.WriteInt(this.MapleID);

                if ((ItemType)this.Type != ItemType.Equipment)
                    p.WriteShort(this.Quantity);

                p.WriteLong();
                p.WriteLong();
                p.WriteLong();

                return p;
            }
        }

        public bool Assigned { get; set; }

        public byte Inventory
        {
            get
            {
                return (byte)(this.MapleID / 1000000);
            }
        }

        public bool IsScroll
        {
            get
            {
                return this.MapleID / 10000 == 204;
            }
        }

        public bool IsSkillBook
        {
            get
            {
                return this.MapleID / 10000 == 228 || this.MapleID / 10000 == 229;
            }
        }

        public bool IsItemConsume
        {
            get
            {
                return this.MapleID / 10000 >= 200 && this.MapleID / 10000 <= 218;
            }
        }

        public byte ComputedSlot
        {
            get
            {
                if (this.IsEquippedCash)
                {
                    return ((byte)(this.AbsoluteSlot - 100));
                }
                else if (this.IsEquipped)
                {
                    return this.AbsoluteSlot;
                }
                else
                {
                    return (byte)this.Slot;
                }
            }
        }

        public byte AbsoluteSlot
        {
            get
            {
                if (this.IsEquipped)
                {
                    return (byte)(this.Slot * -1);
                }
                else
                {
                    throw new InvalidOperationException("Attempting to retrieve absolute slot for non-equipped item.");
                }
            }
        }

        public bool IsEquipped
        {
            get
            {
                return (this.Slot < 0);
            }
        }

        public bool IsEquippedCash
        {
            get
            {
                return (this.Slot < -100);
            }
        }

        public short MaxPerStack
        {
            get
            {
                if (this.Type == ItemType.Equipment)
                    return 1;
                else if (this.maxPerStack == 0)
                    return 100;
                else
                    return maxPerStack;
            }
            set
            {
                maxPerStack = value;
            }
        }

        public short Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                if (value > this.MaxPerStack)
                {
                    throw new ArgumentException("Quantity too high.");
                }
                else
                {
                    quantity = value;
                }
            }
        }

        public ItemType Type
        {
            get
            {
                return Item.GetType(this.MapleID);
            }
        }

        public bool IsRechargeable
        {
            get
            {
                return this.IsThrowingStar || this.IsBullet;
            }
        }

        public bool IsThrowingStar
        {
            get
            {
                return this.MapleID / 10000 == 207;
            }
        }

        public bool IsBullet
        {
            get
            {
                return this.MapleID / 10000 == 233;
            }
        }

        public Character Character
        {
            get
            {
                try
                {
                    return this.Parent.Parent;
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }
        }

        public Item CachedReference
        {
            get
            {
                return MapleData.CachedItems[this.MapleID];
            }
        }

        public static ItemType GetType(int mapleId)
        {
            int firstDigit = mapleId;

            while (firstDigit >= 10)
            {
                firstDigit /= 10;
            }

            return (ItemType)firstDigit;
        }
    }
}
