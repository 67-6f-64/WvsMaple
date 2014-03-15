using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Life;

namespace WvsGame.Maple.Fields
{
    public class SpawnPoint : LifeObject
    {
        private bool IsMob { get; set; }

        public SpawnPoint(string type, int id, int mapleId, Position position, short foothold, short minimumClickX, short maximumClickX, int respawnTime, bool facesLeft)
            : base(id, mapleId, position, foothold, minimumClickX, maximumClickX, respawnTime, facesLeft)
        {
            this.IsMob = type.Equals("m");
        }

        public void Spawn(Field field)
        {
            if (this.IsMob)
            {
                field.Mobs.Add(new Mob(this));
            }
            else
            {
                // TODO: Append reactor to field.
            }
        }
    }
}
