using Common;
using Common.Net;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Fields;
using WvsGame.Net;

namespace WvsGame.Maple.Characters
{
    public class CharacterInventory : IEnumerable<Item>
    {
        public Character Parent { get; private set; }

        public List<Item> Items { get; private set; }
        public Dictionary<ItemType, byte> MaxSlots { get; private set; }

        public CharacterInventory(Character parent)
        {
            this.Parent = parent;
            this.MaxSlots = new Dictionary<ItemType, byte>(Enum.GetValues(typeof(ItemType)).Length);

            this.MaxSlots.Add(ItemType.Equipment, 24);
            this.MaxSlots.Add(ItemType.Usable, 24);
            this.MaxSlots.Add(ItemType.Setup, 24);
            this.MaxSlots.Add(ItemType.Etcetera, 24);
            this.MaxSlots.Add(ItemType.Cash, 90);

            this.Items = new List<Item>();
        }

        public void Operate(Packet pPacket)
        {
            ItemType type = (ItemType)pPacket.ReadByte();
            sbyte source = (sbyte)pPacket.ReadShort();
            sbyte destination = (sbyte)pPacket.ReadShort();
            short quantity = pPacket.ReadShort();

            try
            {
                Item item = this[type, source];

                if (destination < 0)
                {
                    item.Equip();
                }
                else if (source < 0 && destination > 0)
                {
                    item.Unequip(destination);
                }
                else if (destination == 0)
                {
                    item.Drop(quantity);
                }
                else
                {
                    item.Move(destination);
                }
            }
            catch (InventoryFullException)
            {
                this.NotifyFull();
            }
        }

        public void Append(Packet outPacket)
        {
            outPacket.WriteInt(this.Parent.Meso);

            outPacket.WriteByte(this.MaxSlots[ItemType.Equipment]);
            outPacket.WriteByte(this.MaxSlots[ItemType.Usable]);
            outPacket.WriteByte(this.MaxSlots[ItemType.Setup]);
            outPacket.WriteByte(this.MaxSlots[ItemType.Etcetera]);
            outPacket.WriteByte(this.MaxSlots[ItemType.Cash]);

            foreach (Item item in this.GetEquipped(EquippedQueryMode.Normal))
            {
                item.Append(outPacket);
            }
            outPacket.WriteByte();

            foreach (Item item in this.GetEquipped(EquippedQueryMode.Cash))
            {
                item.Append(outPacket);
            }
            outPacket.WriteByte();

            foreach (Item item in this[ItemType.Equipment])
            {
                item.Append(outPacket);
            }
            outPacket.WriteByte();

            foreach (Item item in this[ItemType.Usable])
            {
                item.Append(outPacket);
            }
            outPacket.WriteByte();

            foreach (Item item in this[ItemType.Setup])
            {
                item.Append(outPacket);
            }
            outPacket.WriteByte();

            foreach (Item item in this[ItemType.Etcetera])
            {
                item.Append(outPacket);
            }
            outPacket.WriteByte();

            foreach (Item item in this[ItemType.Cash])
            {
                item.Append(outPacket);
            }
            outPacket.WriteByte();
        }

        public void Load()
        {
            using (MySqlDataReader reader = GameServer.Database.RunQuery("SELECT Meso, EquipmentSlots, UsableSlots, SetupSlots, EtceteraSlots, CashSlots FROM characters WHERE ID = '" + this.Parent.ID + "'"))
            {
                if (reader.Read())
                {
                    this.MaxSlots[ItemType.Equipment] = reader.GetByte("EquipmentSlots");
                    this.MaxSlots[ItemType.Usable] = reader.GetByte("UsableSlots");
                    this.MaxSlots[ItemType.Setup] = reader.GetByte("SetupSlots");
                    this.MaxSlots[ItemType.Etcetera] = reader.GetByte("EtceteraSlots");
                    this.MaxSlots[ItemType.Cash] = reader.GetByte("CashSlots");
                }
            }

            using (MySqlDataReader reader = GameServer.Database.RunQuery("SELECT * FROM items WHERE CharacterID = '" + this.Parent.ID + "' AND IsStored = 0"))
            {
                while (reader.Read())
                {
                    Item item = new Item(reader.GetInt32("MapleID"));

                    item.ID = reader.GetInt32("ID");
                    item.Parent = this;
                    item.Load(reader);

                    this.Items.Add(item);
                }
            }

            //foreach (Item pet in this[ItemType.Cash])
            //{
            //    //this.Parent.Pets.Load(pet);
            //}
        }

        public void Save()
        {
            string query = "UPDATE characters SET ";

            query += "EquipmentSlots = '" + this.MaxSlots[ItemType.Equipment] + "', ";
            query += "UsableSlots = '" + this.MaxSlots[ItemType.Usable] + "', ";
            query += "SetupSlots = '" + this.MaxSlots[ItemType.Setup] + "', ";
            query += "EtceteraSlots = '" + this.MaxSlots[ItemType.Etcetera] + "', ";
            query += "CashSlots = '" + this.MaxSlots[ItemType.Cash] + "' ";
            query += "WHERE ID = '" + this.Parent.ID + "'";

            GameServer.Database.RunQuery(query);

            foreach (Item loopItem in this.Items)
            {
                loopItem.Save();
            }
        }

        public void Delete()
        {

        }

        public void Add(Item item, bool fromDrop = false, bool autoMerge = true, bool inChat = false)
        {
            if (inChat && this.Parent.IsInitialized)
            {
                //using (Packet outPacket = new Packet(MapleServerOperationCode.ShowItemGainInChat))
                //{
                //    outPacket.WriteBytes(5, 1);
                //    outPacket.WriteInt(item.MapleID);
                //    outPacket.WriteInt(item.Quantity);

                //    this.Parent.Client.Send(outPacket);
                //}
            }

            if (this.Available(item.MapleID) % item.MaxPerStack != 0 && autoMerge)
            {
                foreach (Item loopItem in this.Items)
                {
                    if (loopItem.MapleID == item.MapleID && loopItem.Quantity < loopItem.MaxPerStack)
                    {
                        if (loopItem.Quantity + item.Quantity <= loopItem.MaxPerStack)
                        {
                            loopItem.Quantity += item.Quantity;
                            loopItem.Update();

                            item.Quantity = 0;

                            break;
                        }
                        else
                        {
                            item.Quantity -= (short)(loopItem.MaxPerStack - loopItem.Quantity);
                            item.Slot = this.GetNextFreeSlot(item.Type);

                            loopItem.Quantity = loopItem.MaxPerStack;
                            if (this.Parent.IsInitialized)
                            {
                                loopItem.Update();
                            }

                            break;
                        }
                    }
                }
            }

            item.Parent = this;

            //if (item.Type == ItemType.Pet)
            //{
            //    GameServer.Database.RunQuery("UPDATE inc_table SET last_cash_serial = last_cash_serial + 1;");
            //    using (MySqlDataReader reader = GameServer.Database.RunQuery("SELECT last_cash_serial FROM inc_table;"))
            //    {
            //        if (reader.Read())
            //        {
            //            item.UniqueID = reader.GetInt64(0);
            //        }
            //    }

            //    Pet pet = new Pet()
            //    {
            //        Name = Strings.Pets[item.MapleID],
            //        Level = 1,
            //        Closeness = 0,
            //        Fullness = 100,
            //        Expiration = (long)ExpirationTime.Permanent,
            //        IsSpawned = false,
            //        Item = item
            //    };

            //    item.Pet = pet;

            //    pet.Save();

            //    this.Parent.Pets.Add(item);
            //}

            if (item.Quantity > 0)
            {
                if (this.Parent.IsInitialized && item.Slot == 0)
                {
                    item.Slot = this.GetNextFreeSlot(item.Type);
                }

                this.Items.Add(item);

                if (this.Parent.IsInitialized)
                {
                    using (Packet p = new Packet(ServerMessages.InventoryOperation))
                    {
                        p.WriteByte(1);
                        p.WriteByte(1);
                        p.WriteByte();
                        p.WriteByte(item.Inventory);
                        item.Append(p);
                        p.WriteLong();
                        p.WriteLong();
                        p.WriteLong();

                        this.Parent.Client.Send(p);
                    }
                }
            }
        }

        public void Remove(int mapleId, short quantity)
        {
            short leftToRemove = quantity;

            List<Item> toRemove = new List<Item>();

            foreach (Item loopItem in this.Items)
            {
                if (loopItem.MapleID == mapleId)
                {
                    if (loopItem.Quantity > leftToRemove)
                    {
                        loopItem.Quantity -= leftToRemove;
                        loopItem.Update();
                        break;
                    }
                    else
                    {
                        leftToRemove -= loopItem.Quantity;
                        toRemove.Add(loopItem);
                    }
                }
            }

            foreach (Item loopItem in toRemove)
            {
                this.Remove(loopItem, true);
            }
        }

        public void Remove(Item item, bool removeFromSlot, bool fromDrop = false)
        {
            if (removeFromSlot && item.IsEquipped)
            {
                throw new InvalidOperationException("Cannot remove equipped items from slot.");
            }

            if (removeFromSlot)
            {
                using (Packet outPacket = new Packet(ServerMessages.InventoryOperation))
                {
                    outPacket.WriteBool(fromDrop);
                    outPacket.WriteByte(1);
                    outPacket.WriteByte(2);
                    outPacket.WriteByte((byte)item.Inventory);
                    outPacket.WriteShort((short)item.Slot);
                    outPacket.WriteShort();

                    this.Parent.Client.Send(outPacket);
                }
            }

            if (item.Assigned)
            {
                this.Delete(item);
            }

            item.Parent = null;

            bool wasEquipped = item.IsEquipped;

            this.Items.Remove(item);

            if (wasEquipped)
            {
                this.Parent.UpdateLooks();
            }
        }

        public void Pickup(Packet pPacket)
        {
            pPacket.Skip(4);

            int objectID = pPacket.ReadInt();

            lock (this.Parent.Field.Drops)
            {
                if (this.Parent.Field.Drops.Contains(objectID))
                {
                    this.Pickup(this.Parent.Field.Drops[objectID]);
                }
            }
        }

        public void Pickup(Drop drop)
        {
            if (drop.Picker == null)
            {
                try
                {
                    drop.Picker = this.Parent;

                    if (drop is Meso)
                    {
                        this.Parent.Meso += ((Meso)drop).Amount; // TODO: Check for max meso.
                    }
                    else if (drop is Item)
                    {
                        ((Item)drop).Slot = this.GetNextFreeSlot(((Item)drop).Type); // TODO: Check for inv. full. 
                        this.Add((Item)drop, true);
                    }

                    using (Packet p = new Packet(ServerMessages.DropLeaveField))
                    {
                        p.WriteByte(2);
                        p.WriteInt(drop.ObjectID);
                        p.WriteInt(drop.Picker.ID);
                        p.WriteLong();

                        drop.Picker.Client.Send(p);
                    }

                    this.Parent.Field.Drops.Remove(drop);

                    using (Packet gain = drop.GetShowGainPacket())
                    {
                        this.Parent.Client.Send(gain);
                    }

                    this.Parent.Release();
                }
                catch (InventoryFullException)
                {
                    this.NotifyFull();
                    this.Parent.Release();
                }
            }
        }

        public void Delete(Item item)
        {
            GameServer.Database.Delete("items", "ID = '{0}'", item.MapleID);
            item.Assigned = false;
        }

        public void NotifyFull()
        {
            using (Packet p = new Packet(ServerMessages.Message))
            {
                p.WriteByte();
                p.WriteSByte(-1);

                this.Parent.Client.Send(p);
            }
        }

        public int Available(int mapleID)
        {
            int count = 0;

            foreach (Item loopItem in this.Items)
            {
                if (loopItem.MapleID == mapleID)
                {
                    count += loopItem.Quantity;
                }
            }

            return count;
        }

        public sbyte GetNextFreeSlot(ItemType type)
        {
            for (sbyte i = 1; i <= this.MaxSlots[type]; i++)
            {
                if (this[type, i] == null)
                {
                    return i;
                }
            }

            throw new InventoryFullException();
        }

        public int SpaceTakenBy(Item item, bool autoMerge = true)
        {
            if (this.Available(item.MapleID) % item.MaxPerStack != 0 && autoMerge)
            {
                foreach (Item loopItem in this.Items)
                {
                    if (loopItem.MapleID == item.MapleID && loopItem.Quantity < loopItem.MaxPerStack)
                    {
                        if (loopItem.Quantity + item.Quantity <= loopItem.MaxPerStack)
                        {
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }

                return 1;
            }
            else
            {
                return 1;
            }
        }

        public bool CouldReceive(IEnumerable<Item> items, bool autoMerge = true)
        {
            Dictionary<ItemType, int> spaceCount = new Dictionary<ItemType, int>(5);
            {
                spaceCount.Add(ItemType.Equipment, 0);
                spaceCount.Add(ItemType.Usable, 0);
                spaceCount.Add(ItemType.Setup, 0);
                spaceCount.Add(ItemType.Etcetera, 0);
                spaceCount.Add(ItemType.Cash, 0);
            }

            foreach (Item loopItem in items)
            {
                spaceCount[loopItem.Type] += this.SpaceTakenBy(loopItem, autoMerge);
            }

            foreach (KeyValuePair<ItemType, int> loopSpaceCount in spaceCount)
            {
                if (this.RemainingSlots(loopSpaceCount.Key) < loopSpaceCount.Value)
                {
                    return false;
                }
            }

            return true;
        }

        public int RemainingSlots(ItemType type)
        {
            short remaining = this.MaxSlots[type];

            foreach (Item item in this.Items)
            {
                if (item.Type == type)
                {
                    remaining--;
                }
            }

            return remaining;
        }

        public void AddRange(IEnumerable<Item> items, bool fromDrop = false, bool autoMerge = true)
        {
            foreach (Item loopItem in items)
            {
                this.Add(loopItem, fromDrop, autoMerge);
            }
        }

        public Item this[EquipmentSlot slot]
        {
            get
            {
                foreach (Item item in this.Items)
                {
                    if (item.Slot == (sbyte)slot)
                    {
                        return item;
                    }
                }

                return null; // TODO: Should be keynotfoundexception, but I'm lazy.
            }
        }

        public IEnumerable<Item> this[ItemType type]
        {
            get
            {
                foreach (Item loopItem in this.Items)
                {
                    if (((ItemType)loopItem.Type) == type && !loopItem.IsEquipped)
                    {
                        yield return loopItem;
                    }
                }
            }
        }

        public Item this[ItemType type, sbyte slot]
        {
            get
            {
                foreach (Item item in this.Items)
                {
                    if ((ItemType)item.Type == type && item.Slot == slot)
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public Item this[int mapleId, sbyte slot]
        {
            get
            {
                foreach (Item item in this.Items)
                {
                    if (item.Slot == (sbyte)slot && item.Type == Item.GetType(mapleId))
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public IEnumerable<Item> GetEquipped(EquippedQueryMode mode = EquippedQueryMode.Any)
        {
            foreach (Item loopItem in this.Items)
            {
                if (loopItem.IsEquipped)
                {
                    switch (mode)
                    {
                        case EquippedQueryMode.Any:
                            yield return loopItem;
                            break;

                        case EquippedQueryMode.Normal:
                            if (loopItem.Slot > -100)
                            {
                                yield return loopItem;
                            }
                            break;

                        case EquippedQueryMode.Cash:
                            if (loopItem.Slot < -100)
                            {
                                yield return loopItem;
                            }
                            break;
                    }
                }
            }
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.Items).GetEnumerator();
        }
    }
}
