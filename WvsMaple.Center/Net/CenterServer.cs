using Common;
using Common.IO;
using Common.Net;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using WvsCenter.Maple;

namespace WvsCenter.Net
{
    public static class CenterServer
    {
        public static ushort Port { get; private set; }
        public static ushort AdminPort { get; private set; }

        public static Dictionary<string, Server> Servers { get; private set; }
        public static Dictionary<byte, World> Worlds { get; private set; }

        public static Dictionary<int, Character> Characters { get; private set; }

        public static Acceptor CenterAcceptor { get; private set; }

        public static Server LoginServer
        {
            get
            {
                foreach (var server in Servers)
                {
                    if (server.Key.Contains("Login"))
                    {
                        return server.Value;
                    }
                }

                return null;
            }
        }

        public static void Initialize()
        {
            GMSKeys.Initialize();

            Servers = new Dictionary<string, Server>();
            Worlds = new Dictionary<byte, World>();

            Characters = new Dictionary<int, Character>();

            Configurate();
        }

        private static void Configurate()
        {
            using (Config config = new Config(@"..\DataSvr\" + Program.ConfigurationFile))
            {
                Port = config.GetUShort("", "port");
                AdminPort = config.GetUShort("", "adminPort");

                List<string> serverNames = config.GetBlocksFromBlock("", 1);

                foreach (string serverName in serverNames)
                {
                    ServerType type = ServerType.Unknown;

                    switch (serverName)
                    {
                        case "login":
                            type = ServerType.Login;
                            break;

                        case "game":
                            type = ServerType.Game;
                            break;

                        case "shop":
                            type = ServerType.Shop;
                            break;

                        case "mapgen":
                            type = ServerType.MapGen;
                            break;

                        case "claim":
                            type = ServerType.Claim;
                            break;

                        case "itc":
                            type = ServerType.ITC;
                            break;
                    }

                    if (type == ServerType.Unknown)
                    {
                        MessageBox.Show("Unable to parse block '" + serverName + "'.");
                        return;
                    }

                    if (type == ServerType.Claim)
                    {
                        Server server = new Server();

                        server.Type = type;
                        server.Name = "claim";
                        server.Port = config.GetUShort("claim", "port");
                        server.PublicIP = IPAddress.Parse(config.GetString("claim", "PublicIP"));
                        server.PrivateIP = IPAddress.Parse(config.GetString("claim", "PrivateIP"));

                        Servers.Add(server.Name, server);
                    }
                    else
                    {
                        List<string> serverBlock = config.GetBlocks(serverName, true);

                        foreach (string childServer in serverBlock)
                        {
                            Server server = new Server();

                            server.Type = type;
                            server.Name = childServer;
                            server.Port = config.GetUShort(childServer, "port");
                            server.PublicIP = IPAddress.Parse(config.GetString(childServer, "PublicIP"));
                            server.PrivateIP = IPAddress.Parse(config.GetString(childServer, "PrivateIP"));

                            Servers.Add(server.Name, server);
                        }
                    }
                }
            }

            CenterAcceptor = new Acceptor(Port);
            CenterAcceptor.OnClientAccepted += new Action<Socket>(OnClientConnected);
            CenterAcceptor.Start();
        }

        private static void OnClientConnected(Socket socket)
        {
            new InteroperabilityClient(socket);
        }

        public static void AddCharacter(Character character)
        {
            Characters.Add(character.ID, character);
        }

        public static void RemoveCharacter(Character character)
        {
            Characters.Remove(character.ID);
        }

        public static Character GetCharacter(int id)
        {
            return Characters.ContainsKey(id) ? Characters[id] : null;
        }
    }
}
