using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WvsGame.Maple.Fields
{
    public class Seat : FieldObject
    {
        public short ID { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public bool IsTaken { get; set; }

        public override int ObjectID
        {
            get
            {
                return this.ID;
            }
            set
            {
                base.ObjectID = value;
            }
        }
    }
}
