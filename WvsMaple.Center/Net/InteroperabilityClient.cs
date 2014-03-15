using Common;
using Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WvsCenter.Maple;

namespace WvsCenter.Net
{
    public class InteroperabilityClient : Session
    {
        public Server Server { get; private set; }

        public InteroperabilityClient(Socket socket) :
            base(socket)
        {
            this.SendHandshake(Constants.MapleVersion, Constants.PatchLocation, Constants.Localisation);
        }

        public override void OnPacketInbound(Packet pPacket)
        {
            switch ((InteroperabilityMessages)pPacket.OperationCode)
            {
                case InteroperabilityMessages.AllocationRequest:
                    this.OnAllocationRequest(pPacket);
                    break;

                case InteroperabilityMessages.ServerListRequest:
                    this.OnServerListRequest(pPacket);
                    break;

                case InteroperabilityMessages.CheckUserLimitRequest:
                    this.OnCheckUserLimitRequest(pPacket);
                    break;

                case InteroperabilityMessages.CharacterListRequest:
                    this.OnCharacterListRequest(pPacket);
                    break;

                case InteroperabilityMessages.CharacterListResponse:
                    this.OnCharacterListResponse(pPacket);
                    break;

                case InteroperabilityMessages.CharacterCreationRequest:
                    this.OnCharacterCreationRequest(pPacket);
                    break;

                case InteroperabilityMessages.CharacterCreationResponse:
                    this.OnCharacterCreationResponse(pPacket);
                    break;

                case InteroperabilityMessages.MigrateRequest:
                    this.OnMigrateRequest(pPacket);
                    break;

                case InteroperabilityMessages.CharacterRegistrationRequest:
                    this.OnCharacterRegistrationRequest(pPacket);
                    break;

                case InteroperabilityMessages.CharacterDeregistrationRequest:
                    this.OnCharacterDegistrationRequest(pPacket);
                    break;
            }
        }

        public override void OnDisconnect()
        {
            if (this.Server == null)
            {
                return;
            }

            if (this.Server.IsConnected)
            {
                if (this.Server.Name.Contains("Game"))
                {
                    if (CenterServer.Worlds.ContainsKey(this.Server.WorldID))
                    {
                        CenterServer.Worlds[this.Server.WorldID].GameServers.Remove(this.Server.GameID);
                    }
                }
                else if (this.Server.Name.Contains("Shop"))
                {
                    if (CenterServer.Worlds.ContainsKey(this.Server.WorldID))
                    {
                        CenterServer.Worlds[this.Server.WorldID].ShopServer = null;
                    }
                }
            }

            this.Server.Session = null;
        }

        private void OnAllocationRequest(Packet inPacket)
        {
            string name = inPacket.ReadString();

            if (!CenterServer.Servers.ContainsKey(name))
            {
                MainForm.Instance.Log("Disconnected unassigned Local Server '{0}'.", name);

                this.Disconnect();
                return;
            }

            Server server = CenterServer.Servers[name];

            if (server.IsConnected)
            {
                this.Disconnect();
                return;
            }

            this.Server = server;

            this.Server.Session = this;
            this.Server.Connections = 0;
            this.Server.IsConnected = true;

            this.Server.PublicIP = IPAddress.Parse(inPacket.ReadString());
            this.Server.Port = inPacket.ReadUShort();
            this.Server.Name = name;

            if (this.Server.Name.Contains("Login"))
            {
                this.Server.Type = ServerType.Login;

                byte worlds = inPacket.ReadByte();

                for (byte b = 0; b < worlds; b++)
                {
                    World world = new World(inPacket.ReadByte());

                    world.Name = inPacket.ReadString();
                    world.Channels = inPacket.ReadByte();

                    CenterServer.Worlds.Add(world.ID, world);
                }

                MainForm.Instance.Log("Login Server '{0}' assigned.", this.Server.Name);
            }
            else if (this.Server.Name.Contains("Game"))
            {
                this.Server.Type = ServerType.Game;

                byte worldId = inPacket.ReadByte();

                if (!CenterServer.Worlds.ContainsKey(worldId))
                {
                    MainForm.Instance.Log("Disconnected Game Server '{0}' with unknown World ID '{1}'.", name, worldId);

                    this.Disconnect();
                    return;
                }

                World world = CenterServer.Worlds[worldId];
                byte gameId = world.NextSlot();

                if (gameId == byte.MaxValue)
                {
                    MainForm.Instance.Log("Disconnected Game Server '{0}' because the world '{1}' is full.", name, world.Name);

                    this.Disconnect();
                    return;
                }

                world.GameServers.Add(gameId, this.Server);

                this.Server.WorldID = worldId;
                this.Server.GameID = gameId;

                using (Packet outPacket = new Packet(InteroperabilityMessages.AllocationResponse))
                {
                    outPacket.WriteByte(this.Server.GameID);

                    this.Send(outPacket);
                }

                MainForm.Instance.Log("Game Server '{0}' ({1}'s Channel {2}) assigned.", this.Server.Name, world.Name, this.Server.GameID);
            }

            MainForm.Instance.RefreshServerList();
        }

        private void OnServerListRequest(Packet inPacket)
        {
            string hash = inPacket.ReadString();

            foreach (KeyValuePair<byte, World> world in CenterServer.Worlds)
            {
                using (Packet outPacket = new Packet(InteroperabilityMessages.ServerListResponse))
                {
                    outPacket.WriteString(hash);
                    outPacket.WriteByte(world.Key);
                    outPacket.WriteString(world.Value.Name);
                    outPacket.WriteByte(2); // TODO: Variable for Flag.
                    outPacket.WriteString("Welcome to WvsMaple!"); // TODO: Variable for Event Message.
                    outPacket.WriteByte();
                    outPacket.WriteByte(world.Value.Channels);

                    for (byte b = 0; b < world.Value.Channels; b++)
                    {
                        outPacket.WriteString(string.Format("{0}-{1}", world.Value.Name, (b + 1)));

                        if (world.Value.GameServers.ContainsKey(b))
                        {
                            outPacket.WriteInt(world.Value.GameServers[b].Connections);
                        }
                        else
                        {
                            outPacket.WriteInt(9001);
                        }

                        outPacket.WriteByte(world.Key);
                        outPacket.WriteShort(b);
                    }

                    this.Send(outPacket);
                }
            }

            using (Packet outPacket = new Packet(InteroperabilityMessages.ServerListResponse))
            {
                outPacket.WriteString(hash);
                outPacket.WriteByte(byte.MaxValue);

                this.Send(outPacket);
            }
        }

        private void OnCheckUserLimitRequest(Packet inPacket)
        {
            string hash = inPacket.ReadString();
            byte worldId = inPacket.ReadByte();

            using (Packet p = new Packet(InteroperabilityMessages.CheckUserLimitResponse))
            {
                p.WriteString(hash);

                if (CenterServer.Worlds.ContainsKey(worldId))
                {
                    p.WriteByte(CenterServer.Worlds[worldId].Status);
                }
                else
                {
                    p.WriteByte(2);
                }

                this.Send(p);
            }
        }

        private void OnCharacterListRequest(Packet inPacket)
        {
            string hash = inPacket.ReadString();
            int accountId = inPacket.ReadInt();
            byte worldId = inPacket.ReadByte();
            byte channelId = inPacket.ReadByte();

            if (!CenterServer.Worlds.ContainsKey(worldId) || !CenterServer.Worlds[worldId].GameServers.ContainsKey(channelId))
            {
                using (Packet outPacket = new Packet(InteroperabilityMessages.CharacterListResponse))
                {
                    outPacket.WriteString(hash);
                    outPacket.WriteBool(false);

                    this.Send(outPacket);
                }

                return;
            }

            using (Packet outPacket = new Packet(InteroperabilityMessages.CharacterListRequest))
            {
                outPacket.WriteString(hash);
                outPacket.WriteInt(accountId);
                outPacket.WriteByte(worldId);

                CenterServer.Worlds[worldId].BroadcastRandom(outPacket);
            }
        }

        private void OnCharacterListResponse(Packet inPacket)
        {
            string hash = inPacket.ReadString();

            using (Packet outPacket = new Packet(InteroperabilityMessages.CharacterListResponse))
            {
                outPacket.WriteString(hash);
                outPacket.WriteBool(true);

                outPacket.WriteByte(inPacket.ReadByte());
                outPacket.WriteBytes(inPacket.ReadLeftoverBytes());

                CenterServer.LoginServer.Session.Send(outPacket);
            }
        }

        private void OnCharacterCreationRequest(Packet inPacket)
        {
            string hash = inPacket.ReadString();
            int accountId = inPacket.ReadInt();
            byte worldId = inPacket.ReadByte();
            byte[] data = inPacket.ReadLeftoverBytes();

            using (Packet outPacket = new Packet(InteroperabilityMessages.CharacterCreationRequest))
            {
                outPacket.WriteString(hash);
                outPacket.WriteInt(accountId);
                outPacket.WriteByte(worldId);
                outPacket.WriteBytes(data);

                CenterServer.Worlds[worldId].BroadcastRandom(outPacket);
            }
        }

        private void OnCharacterCreationResponse(Packet inPacket)
        {
            string hash = inPacket.ReadString();

            using (Packet outPacket = new Packet(InteroperabilityMessages.CharacterCreationResponse))
            {
                outPacket.WriteString(hash);
                outPacket.WriteBytes(inPacket.ReadLeftoverBytes());

                CenterServer.LoginServer.Session.Send(outPacket);
            }
        }

        private void OnMigrateRequest(Packet inPacket)
        {
            string hash = inPacket.ReadString();
            byte worldId = inPacket.ReadByte();
            byte channelId = inPacket.ReadByte();
            int characterId = inPacket.ReadInt();

            using (Packet outPacket = new Packet(InteroperabilityMessages.MigrateResponse))
            {
                outPacket.WriteString(hash);
                outPacket.WriteInt(characterId);

                if (!CenterServer.Worlds.ContainsKey(worldId))
                {
                    outPacket.WriteInt();
                    outPacket.WriteShort();

                    this.Send(outPacket);

                    return;
                }

                if (channelId != 50 && CenterServer.Worlds[worldId].GameServers.ContainsKey(channelId))
                {
                    Server server = CenterServer.Worlds[worldId].GameServers[channelId];

                    outPacket.WriteBytes(server.PublicIP.GetAddressBytes());
                    outPacket.WriteUShort(server.Port);

                    Character c = CenterServer.GetCharacter(characterId);

                    if (c != null)
                    {
                        c.IsMigrating = true;
                    }
                }
                else if (channelId == 50 && CenterServer.Worlds[worldId].ShopServer != null)
                {
                    Server server = CenterServer.Worlds[worldId].ShopServer;

                    outPacket.WriteBytes(server.PublicIP.GetAddressBytes());
                    outPacket.WriteUShort(server.Port);

                    Character c = CenterServer.GetCharacter(characterId);

                    if (c != null)
                    {
                        c.IsMigrating = true;
                        c.LastChannel = c.ChannelID;
                    }
                }
                else
                {
                    outPacket.WriteInt();
                    outPacket.WriteShort();
                }

                this.Send(outPacket);
            }
        }

        private void OnCharacterRegistrationRequest(Packet inPacket)
        {
            int characterId = inPacket.ReadInt();
            Character character = CenterServer.GetCharacter(characterId);

            if (character == null)
            {
                character = new Character();

                character.ID = characterId;
                character.Name = inPacket.ReadString();
                character.WorldID = this.Server.WorldID;
                character.ChannelID = this.Server.GameID;

                CenterServer.Characters.Add(character.ID, character);
            }
            else if (character.IsMigrating)
            {
                character.IsMigrating = false;
                character.ChannelID = this.Server.GameID;
            }

            character.Job = inPacket.ReadShort();
            character.Level = inPacket.ReadByte();
        }

        private void OnCharacterDegistrationRequest(Packet inPacket)
        {
            int characterId = inPacket.ReadInt();
            Character character = CenterServer.GetCharacter(characterId);

            if (character == null)
            {
                return;
            }

            CenterServer.Characters.Remove(characterId);
        }
    }
}
