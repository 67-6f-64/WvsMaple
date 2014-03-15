using Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Data;
using WvsGame.Maple.Life;

namespace WvsGame.Maple.Fields
{
    public class FieldMobs : FieldObjects<Mob>
    {
        public FieldMobs(Field parent) : base(parent) { }

        protected override void InsertItem(int index, Mob item)
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

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
        }
    }
}
