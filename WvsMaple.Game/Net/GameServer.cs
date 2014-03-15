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
using WvsGame.Interoperability;
using WvsGame.Maple;
using WvsGame.Maple.Characters;
using WvsGame.Maple.Scripting;

namespace WvsGame.Net
{
    public static class GameServer
    {
        public static int ID { get; set; }

        public static string Name { get; private set; }

        public static string WorldName { get; private set; }
        public static byte WorldID { get; private set; }

        public static ushort Port { get; private set; }

        public static IPAddress PublicIP { get; private set; }
        public static IPAddress PrivateIP { get; private set; }

        public static IPAddress CenterIP { get; private set; }
        public static ushort CenterPort { get; private set; }

        public static Dictionary<string, User> Users { get; private set; }
        public static Dictionary<int, Character> Characters { get; private set; }
        public static Dictionary<ScriptType, Dictionary<string, Type>> Scripts { get; private set; }


        public static Database Database { get; private set; }
        public static CenterServer Center { get; private set; }
        public static Acceptor GameAcceptor { get; private set; }

        public static void Initialize()
        {
            GMSKeys.Initialize();

            Users = new Dictionary<string, User>();
            Characters = new Dictionary<int, Character>();
            Scripts = new Dictionary<ScriptType, Dictionary<string, Type>>();

            Configurate();
            ConnectCenter();
        }

        private static void Configurate()
        {
            using (Config config = new Config(@"..\DataSvr\" + Program.ConfigurationFile))
            {
                // NOTE: Properties.
                
                Name = Program.ConfigurationFile.Replace(".img", "");
                Port = config.GetUShort("", "port");
                WorldID = config.GetByte("", "gameWorldId");

                IPAddress ipAddress;
                string ipString = config.GetString("", "PublicIP");

                if (!IPAddress.TryParse(ipString, out ipAddress))
                {
                    ipAddress = Dns.GetHostEntry(ipString).AddressList[0];
                }

                PublicIP = ipAddress;

                PrivateIP = IPAddress.Parse(config.GetString("", "PrivateIP"));

                // NOTE: Center Block.

                CenterIP = IPAddress.Parse(config.GetString("center", "ip"));
                CenterPort = config.GetUShort("center", "port");
                WorldName = config.GetString("center", "worldName");

                // NOTE: Hacking Auto Block Block.
            }

            using (Config config = new Config(@"..\DataSvr\Database.img"))
            {
                string host = config.GetString("", "Host");
                string schema = config.GetString("", "Schema");
                string username = config.GetString("", "Username");
                string password = config.GetString("", "Password");

                Database = new Database(username, password, schema, host);
            }

            GameAcceptor = new Acceptor(Port);
            GameAcceptor.OnClientAccepted += new Action<Socket>(OnClientConnected);
            GameAcceptor.Start();
        }

        private static void OnClientConnected(Socket socket)
        {
            new MapleClient(socket);
        }

        public static void AddUser(User user)
        {
            Users.Add(user.Hash, user);
        }

        public static void AddCharacter(Character character)
        {
            Characters.Add(character.ID, character);
        }

        public static void RemoveUser(User user)
        {
            Users.Remove(user.Hash);
        }

        public static void RemoveCharacter(Character character)
        {
            Characters.Remove(character.ID);
        }

        public static User GetUser(string hash)
        {
            return Users.ContainsKey(hash) ? Users[hash] : null;
        }

        public static Character GetCharacter(int id)
        {
            return Characters.ContainsKey(id) ? Characters[id] : null;
        }

        private static void ConnectCenter()
        {
            Center = new CenterServer(CenterIP.ToString(), CenterPort);
            Center.Connect();
        }
    }
}
