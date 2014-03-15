using Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Characters;
using WvsGame.Maple.Fields;

namespace WvsGame.Maple
{
    public class Pet : FieldObject, ISpawnable
    {
        public Item Item { get; set; }
        public string Name { get; set; }
        public byte Level { get; set; }
        public short Closeness { get; set; }
        public byte Fullness { get; set; }
        public long Expiration { get; set; }
        public bool IsSpawned { get; set; }

        public Character Character
        {
            get
            {
                return this.Item.Character;
            }
        }

        public Packet GetCreatePacket()
        {
            throw new NotImplementedException();
        }

        public Packet GetSpawnPacket()
        {
            throw new NotImplementedException();
        }

        public Packet GetDestroyPacket()
        {
            throw new NotImplementedException();
        }
    }
}
