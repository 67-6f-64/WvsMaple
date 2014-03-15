using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Fields;

namespace WvsGame.Maple.Life
{
    public abstract class LifeObject : FieldObject
    {
        public int ID { get; set; }
        public int MapleID { get; private set; }
        public short Foothold { get; private set; }
        public short MinimumClickX { get; private set; }
        public short MaximumClickX { get; private set; }
        public int RespawnTime { get; private set; }
        public bool FacesLeft { get; private set; }

        public LifeObject(int id, int mapleId, Position position, short foothold, short minimumClickX, short maximumClickX, int respawnTime, bool facesLeft)
            : base()
        {
            this.ID = id;
            this.MapleID = mapleId;
            this.Position = position;
            this.Foothold = foothold;
            this.MinimumClickX = minimumClickX;
            this.MaximumClickX = maximumClickX;
            this.RespawnTime = respawnTime;
            this.FacesLeft = facesLeft;
        }
    }
}
