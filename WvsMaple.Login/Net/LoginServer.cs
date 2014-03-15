using Common.Data;
using Common.IO;
using Common.Net;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WvsLogin.Interoperability;
using WvsLogin.Maple;

namespace WvsLogin.Net
{
    public static class LoginServer
    {
        public static string Name { get; private set; }

        public static ushort Port { get; private set; }
        public static ushort AdminPort { get; private set; }

        public static IPAddress PublicIP { get; private set; }
        public static IPAddress PrivateIP { get; private set; }

        public static Dictionary<byte, World> Worlds { get; private set; }
        public static Dictionary<string, User> Users { get; private set; }

        public static Database Database { get; private set; }
        public static CenterServer Center { get; private set; }
        public static Acceptor LoginAcceptor { get; private set; }

        public static void Initialize()
        {
            GMSKeys.Initialize();

            Worlds = new Dictionary<byte, World>();
            Users = new Dictionary<string, User>();

            Configurate();
            ConnectCenter();
        }

        private static void Configurate()
        {
            using (Config config = new Config(@"..\DataSvr\" + Program.ConfigurationFile))
            {
                Name = Program.ConfigurationFile.Replace(".img", "");
                Port = config.GetUShort("", "port");
                AdminPort = config.GetUShort("", "adminPort");

                IPAddress ipAddress;
                string ipString = config.GetString("", "PublicIP");

                if (!IPAddress.TryParse(ipString, out ipAddress))
                {
                    ipAddress = Dns.GetHostEntry(ipString).AddressList[0];
                }

                PublicIP = ipAddress;

                PrivateIP = IPAddress.Parse(config.GetString("", "PrivateIP"));

                List<string> worldNames = config.GetBlocks("center", true);

                foreach (string worldName in worldNames)
                {
                    World world = new World();

                    world.Name = worldName;
                    world.ID = config.GetByte(worldName, "world");
                    world.Channels = config.GetByte(worldName, "channelNo");

                    Worlds.Add(world.ID, world);
                }
            }

            using (Config config = new Config(@"..\DataSvr\Database.img"))
            {
                string host = config.GetString("", "Host");
                string schema = config.GetString("", "Schema");
                string username = config.GetString("", "Username");
                string password = config.GetString("", "Password");

                Database = new Database(username, password, schema, host);
            }

            LoginAcceptor = new Acceptor(Port);
            LoginAcceptor.OnClientAccepted += new Action<Socket>(OnClientConnected);
            LoginAcceptor.Start();
        }

        private static void OnClientConnected(Socket socket)
        {
            new MapleClient(socket);
        }

        public static void AddUser(User user)
        {
            Users.Add(user.Hash, user);
        }

        public static void RemoveUser(User user)
        {
            Users.Remove(user.Hash);
        }

        public static User GetUser(string hash)
        {
            return Users.ContainsKey(hash) ? Users[hash] : null;
        }

        private static void ConnectCenter()
        {
            Center = new CenterServer(PrivateIP.ToString(), 8383); // NOTE: Is a constant value needed for this?
            Center.Connect();
        }
    }
}
