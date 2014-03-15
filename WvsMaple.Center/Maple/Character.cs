using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WvsCenter.Maple
{
    public class Character
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public byte WorldID { get; set; }
        public byte ChannelID { get; set; }

        public bool IsMigrating { get; set; }
        public byte LastChannel { get; set; }

        public short Job { get; set; }
        public byte Level { get; set; }
    }
}
