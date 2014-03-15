using Common.Net;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WvsCenter.Maple
{
    public class World
    {
        public byte ID { get; set; }
        public string Name { get; set; }
        public byte Channels { get; set; }

        public byte Status
        {
            get
            {
                if (this.Load >= 200)
                {
                    return 1;
                }
                else if (this.Load >= 400)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int Load
        {
            get
            {
                int load = 0;

                foreach (KeyValuePair<byte, Server> gameServer in this.GameServers)
                {
                    load += gameServer.Value.Connections;
                }

                return load;
            }
        }

        public Dictionary<byte, Server> GameServers { get; private set; }
        public Server ShopServer { get; set; }

        public World(byte id)
        {
            this.ID = id;
            this.GameServers = new Dictionary<byte, Server>();
        }

        public void Broadcast(Packet outPacket)
        {
            foreach (KeyValuePair<byte, Server> gameServer in this.GameServers)
            {
                gameServer.Value.Session.Send(outPacket);
            }
        }

        public void BroadcastRandom(Packet outPacket)
        {
            byte randomGameId = (byte)Randomizer.Next(this.GameServers.Count - 1);

            this.GameServers[randomGameId].Session.Send(outPacket);
        }

        public byte NextSlot()
        {
            for (byte b = 0; b < this.Channels; b++)
            {
                if (this.GameServers.ContainsKey(b))
                {
                    continue;
                }

                return b;
            }

            return byte.MaxValue;
        }
    }
}
