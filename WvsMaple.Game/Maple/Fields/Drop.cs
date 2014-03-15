using Common;
using Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Characters;

namespace WvsGame.Maple.Fields
{
    public abstract class Drop : FieldObject, ISpawnable
    {
        private FieldObject dropper;

        public Character Owner { get; set; }
        public Character Picker { get; set; }
        public Position Origin { get; set; }

        public FieldObject Dropper
        {
            get
            {
                return dropper;
            }
            set
            {
                this.Origin = value.Position;
                this.Position = value.Position; // TODO: this.Origin = value.Map.FootHolds.FindBelow(value.Position.Y - 25);
                dropper = value;
            }
        }

        public Drop() : base() { }

        public abstract Packet GetShowGainPacket();

        public Packet GetCreatePacket()
        {
            return this.GetInternalPacket(true, null);
        }

        public Packet GetSpawnPacket()
        {
            return this.GetInternalPacket(false, null);
        }

        public Packet GetCreatePacket(Character temporaryOwner)
        {
            return this.GetInternalPacket(true, temporaryOwner);
        }

        public Packet GetSpawnPacket(Character temporaryOwner)
        {
            return this.GetInternalPacket(false, temporaryOwner);
        }

        private Packet GetInternalPacket(bool dropped, Character temporaryOwner)
        {
            Packet p = new Packet(ServerMessages.DropEnterField);

            p.WriteByte((byte)(dropped ? 1 : 2));
            p.WriteInt(this.ObjectID);
            p.WriteBool(this is Meso);

            if (this is Meso)
            {
                p.WriteInt(((Meso)this).Amount);
            }
            else if (this is Item)
            {
                p.WriteInt(((Item)this).MapleID);
            }

            p.WriteInt(this.Owner == null ? temporaryOwner.ID : this.Owner.ID);
            p.WriteByte();
            p.WriteShort(this.Position.X);
            p.WriteShort(this.Position.Y);
            p.WriteInt(this.Dropper.ObjectID);

            if (dropped)
            {
                p.WriteShort(this.Origin.X);
                p.WriteShort(this.Origin.Y);
                p.WriteShort();
            }

            if (!(this is Meso))
            {
                p.WriteLong((long)ExpirationTime.DefaultTime);
            }

            p.WriteBool(this.Dropper == null);
            p.WriteLong();
            p.WriteLong();
            p.WriteLong();

            return p;
        }

        public Packet GetDestroyPacket()
        {
            Packet p = new Packet(ServerMessages.DropLeaveField);
            p.WriteByte();
            p.WriteInt(this.ObjectID);
            p.WriteLong();

            return p;
        }
    }
}
