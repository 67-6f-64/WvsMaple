using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using WvsCenter.Net;

namespace WvsCenter.Maple
{
    public class Server
    {
        public InteroperabilityClient Session { get; set; }

        public ServerType Type { get; set; }
        public string Name { get; set; }
        public ushort Port { get; set; }

        public IPAddress PublicIP { get; set; }
        public IPAddress PrivateIP { get; set; }

        public bool IsConnected { get; set; }
        public int Connections { get; set; }
        public byte WorldID { get; set; }
        public byte GameID { get; set; }
    }
}
