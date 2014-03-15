using Common;
using Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Characters;
using WvsGame.Maple.Data;
using WvsGame.Maple.Life;

namespace WvsGame.Maple.Fields
{
    public class FieldCharacters : FieldObjects<Character>
    {
        public FieldCharacters(Field parent) : base(parent) { }

        protected override void InsertItem(int index, Character item)
        {
            lock (this)
            {
                foreach (Character character in this)
                {
                    using (Packet spawn = character.GetSpawnPacket())
                    {
                        item.Client.Send(spawn);
                    }
                }

                item.Position = this.Field.Portals[item.SpawnPoint].Position;

                base.InsertItem(index, item);
            }

            lock (this.Field.Npcs)
            {
                foreach (Npc npc in this.Field.Npcs)
                {
                    using (Packet spawn = npc.GetSpawnPacket())
                    {
                        item.Client.Send(spawn);
                    }
                }
            }

            lock (this.Field.Mobs)
            {
                foreach (Mob mob in this.Field.Mobs)
                {
                    using (Packet spawn = mob.GetSpawnPacket())
                    {
                        item.Client.Send(spawn);
                    }
                }
            }

            lock (this.Field.Npcs)
            {
                foreach (Npc npc in this.Field.Npcs)
                {
                    npc.AssignController();
                }
            }

            lock (this.Field.Mobs)
            {
                foreach (Mob mob in this.Field.Mobs)
                {
                    mob.AssignController();
                }
            }

            using (Packet create = item.GetCreatePacket())
            {
                item.Field.Broadcast(create, item);
            }

            if (this.Field.HasShip)
            {
                this.Field.ShowEffect(FieldEffect.ShipArrive, item);
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (this)
            {
                Character item = this.GetAtIndex(index);

                if (MapleData.IsInitialzied)
                {
                    item.ControlledMobs.Clear();
                    item.ControlledNpcs.Clear();
                }

                base.RemoveItem(index);

                if (MapleData.IsInitialzied)
                {
                    lock (this.Field.Npcs)
                    {
                        foreach (Npc npc in this.Field.Npcs)
                        {
                            npc.AssignController();
                        }
                    }

                    lock (this.Field.Mobs)
                    {
                        foreach (Mob mob in this.Field.Mobs)
                        {
                            mob.AssignController();
                        }
                    }

                    using (Packet destroy = item.GetDestroyPacket())
                    {
                        this.Field.Broadcast(destroy);
                    }
                }
            }
        }
    }
}
