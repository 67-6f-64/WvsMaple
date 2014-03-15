using Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Fields;

namespace WvsGame.Maple
{
    public class Meso : Drop
    {
        public int Amount { get; private set; }

        public Meso(int amount)
            : base()
        {
            this.Amount = amount;
        }

        public override Packet GetShowGainPacket()
        {
            using (Packet p = new Packet(ServerMessages.Message))
            {
                p.WriteByte();
                p.WriteBool(true);
                p.WriteInt(this.Amount);
                p.WriteShort();

                p.WriteLong();
                p.WriteLong();
                p.WriteLong();

                return p;
            }
        }
    }
}
