using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Data;

namespace WvsGame.Maple.Fields
{
    public class Portal : FieldObject
    {
        public byte ID { get; set; }
        public string Label { get; set; }
        public int DestinationFieldID { get; set; }
        public string DestinationLabel { get; set; }
        public string Script { get; set; }

        public bool IsSpawnPoint
        {
            get
            {
                return this.Label == "sp";
            }
        }

        public Field Destination
        {
            get
            {
                return MapleData.CachedFields[this.DestinationFieldID];
            }
        }

        public Portal Link
        {
            get
            {
                return MapleData.CachedFields[this.DestinationFieldID].Portals[this.DestinationLabel];
            }
        }
    }
}
