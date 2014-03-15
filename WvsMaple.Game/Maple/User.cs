using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Characters;
using WvsGame.Net;

namespace WvsGame.Maple
{
    public class User
    {
        public bool IsAssigned { get; set; }
        public string Hash { get; private set; }
        public bool IsMigrating { get; set; }

        public MapleClient Client { get; private set; }
        public Character Character { get; set; }

        public User(MapleClient client)
        {
            this.Client = client;
            this.Hash = Randomizer.NextHash();
        }
    }
}
