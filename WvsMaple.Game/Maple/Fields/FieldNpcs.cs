using Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Data;
using WvsGame.Maple.Life;

namespace WvsGame.Maple.Fields
{
    public class FieldNpcs : FieldObjects<Npc>
    {
        public FieldNpcs(Field parent) : base(parent) { }

        protected override void InsertItem(int index, Npc item)
        {
            lock (this)
            {
                base.InsertItem(index, item);

                if (MapleData.IsInitialzied)
                {
                    using (Packet create = item.GetCreatePacket())
                    {
                        item.Field.Broadcast(create);
                    }

                    item.AssignController();
                }
            }
        }

        protected override int GetKeyForItem(Npc item)
        {
            return item.ObjectID;
        }

        protected override void RemoveItem(int index)
        {
            lock (this)
            {
                Npc item = this.GetAtIndex(index);

                if (MapleData.IsInitialzied)
                {
                    item.Controller.ControlledNpcs.Remove(item);

                    using (Packet destroy = item.GetDestroyPacket())
                    {
                        item.Field.Broadcast(destroy);
                    }
                }

                base.RemoveItem(index);
            }
        }
    }
}
