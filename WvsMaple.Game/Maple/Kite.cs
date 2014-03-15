using Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WvsGame.Maple.Fields;

namespace WvsGame.Maple
{
    public class Kite : FieldObject, ISpawnable
    {
        public int MapleID { get; private set; }
        
        public Kite(int mapleId)
            : base()
        {
            this.MapleID = mapleId;
        }

        public Packet GetCreatePacket()
        {
            return this.GetSpawnPacket();

        }

        public Packet GetSpawnPacket()
        {
            throw new NotImplementedException();
        }

        public Packet GetDestroyPacket()
        {
            Packet destroy = new Packet(ServerMessages.MessageBoxLeaveField);

            destroy.WriteInt(this.ObjectID);

            return destroy;
        }
    }
}
