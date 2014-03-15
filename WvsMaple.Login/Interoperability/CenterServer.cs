using Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsLogin.Maple;
using WvsLogin.Net;

namespace WvsLogin.Interoperability
{
    public class CenterServer : Session
    {
        public CenterServer(string ip, ushort port) : base(ip, port, "Center") { }

        public override void OnHandshakeInbound(Packet pPacket)
        {
            this.Allocate();
        }

        public override void OnPacketInbound(Packet pPacket)
        {
            switch ((InteroperabilityMessages)pPacket.OperationCode)
            {
                case InteroperabilityMessages.ServerListResponse:
                    this.OnServerListResponse(pPacket);
                    break;

                case InteroperabilityMessages.CheckUserLimitResponse:
                    this.OnCheckUserLimitResponse(pPacket);
                    break;

                case InteroperabilityMessages.CharacterListResponse:
                    this.OnCharacterListResponse(pPacket);
                    break;

                case InteroperabilityMessages.CharacterCreationResponse:
                    this.OnCharacterCreationResponse(pPacket);
                    break;

                case InteroperabilityMessages.MigrateResponse:
                    this.OnMigrateResponse(pPacket);
                    break;

                default:
                    MainForm.Instance.LogPacket(pPacket);
                    break;
            }
        }

        public override void OnDisconnect()
        {
            base.OnDisconnect();
        }

        private void Allocate()
        {
            using (Packet p = new Packet(InteroperabilityMessages.AllocationRequest))
            {
                p.WriteString(LoginServer.Name);
                p.WriteString(LoginServer.PublicIP.ToString());
                p.WriteUShort(LoginServer.Port);
                p.WriteByte((byte)LoginServer.Worlds.Count);

                foreach (KeyValuePair<byte, World> world in LoginServer.Worlds)
                {
                    p.WriteByte(world.Key);
                    p.WriteString(world.Value.Name);
                    p.WriteByte(world.Value.Channels);
                }

                this.Send(p);
            }
        }

        public void RequestServerList(string hash)
        {
            using (Packet p = new Packet(InteroperabilityMessages.ServerListRequest))
            {
                p.WriteString(hash);

                this.Send(p);
            }
        }

        public void RequestCheckUserLimit(string hash, byte worldId)
        {
            using (Packet p = new Packet(InteroperabilityMessages.CheckUserLimitRequest))
            {
                p.WriteString(hash);
                p.WriteByte(worldId);

                this.Send(p);
            }
        }

        public void RequestCharacterList(string hash, int accountId, byte worldId, byte channelId)
        {
            using (Packet p = new Packet(InteroperabilityMessages.CharacterListRequest))
            {
                p.WriteString(hash);
                p.WriteInt(accountId);
                p.WriteByte(worldId);
                p.WriteByte(channelId);

                this.Send(p);
            }
        }

        public void RequestCharacterCreation(string hash, int accountId, byte worldId, byte[] data)
        {
            using (Packet outPacket = new Packet(InteroperabilityMessages.CharacterCreationRequest))
            {
                outPacket.WriteString(hash);
                outPacket.WriteInt(accountId);
                outPacket.WriteByte(worldId);
                outPacket.WriteBytes(data);

                this.Send(outPacket);
            }
        }

        public void RequestMigration(string hash, byte worldId, byte channelId, int characterId)
        {
            using (Packet outPacket = new Packet(InteroperabilityMessages.MigrateRequest))
            {
                outPacket.WriteString(hash);
                outPacket.WriteByte(worldId);
                outPacket.WriteByte(channelId);
                outPacket.WriteInt(characterId);

                this.Send(outPacket);
            }
        }

        private void OnServerListResponse(Packet inPacket)
        {
            string hash = inPacket.ReadString();
            User user = LoginServer.GetUser(hash);

            if (user != null)
            {
                using (Packet outPacket = new Packet(ServerMessages.WorldInformation))
                {
                    outPacket.WriteBytes(inPacket.ReadLeftoverBytes());

                    user.Client.Send(outPacket);
                }
            }
        }

        private void OnCheckUserLimitResponse(Packet inPacket)
        {
            string hash = inPacket.ReadString();
            User user = LoginServer.GetUser(hash);

            if (user != null)
            {
                using (Packet outPacket = new Packet(ServerMessages.CheckUserLimitResult))
                {
                    outPacket.WriteShort(inPacket.ReadByte());

                    user.Client.Send(outPacket);
                }
            }
        }

        private void OnCharacterListResponse(Packet inPacket)
        {
            string hash = inPacket.ReadString();
            User user = LoginServer.GetUser(hash);

            if (user != null)
            {
                bool online = inPacket.ReadBool();

                if (online)
                {
                    using (Packet outPacket = new Packet(ServerMessages.SelectWorldResult))
                    {
                        outPacket.WriteByte();
                        outPacket.WriteByte(inPacket.ReadByte());
                        outPacket.WriteBytes(inPacket.ReadLeftoverBytes());

                        user.Client.Send(outPacket);
                    }
                }
                else
                {
                    using (Packet outPacket = new Packet(ServerMessages.SelectWorldResult))
                    {
                        outPacket.WriteByte(9);

                        user.Client.Send(outPacket);
                    }
                }
            }
        }

        private void OnCharacterCreationResponse(Packet inPacket)
        {
            string hash = inPacket.ReadString();
            User user = LoginServer.GetUser(hash);

            if (user != null)
            {
                using (Packet outPacket = new Packet(ServerMessages.CreateNewCharacterResult))
                {
                    outPacket.WriteBool(!inPacket.ReadBool());
                    outPacket.WriteBytes(inPacket.ReadLeftoverBytes());

                    user.Client.Send(outPacket);
                }
            }
        }

        private void OnMigrateResponse(Packet inPacket)
        {
            string hash = inPacket.ReadString();
            User user = LoginServer.GetUser(hash);

            if (user != null)
            {
                int characterId = inPacket.ReadInt();
                byte[] ip = inPacket.ReadBytes(4);
                ushort port = inPacket.ReadUShort();

                using (Packet outPacket = new Packet(ServerMessages.SelectCharacterResult))
                {
                    outPacket.WriteShort(); // NOTE: Status. Otherwise will show error connecting.
                    outPacket.WriteBytes(ip);
                    outPacket.WriteUShort(port);
                    outPacket.WriteInt(characterId);
                    outPacket.WriteByte(); // NOTE: Unknown.
                    outPacket.WriteInt(); // NOTE: Unknown.

                    user.Client.Send(outPacket);
                }
            }
        }
    }
}
