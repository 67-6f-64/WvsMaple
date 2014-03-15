using Common.Net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple;
using WvsGame.Maple.Characters;
using WvsGame.Net;

namespace WvsGame.Interoperability
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
            try
            {
                switch ((InteroperabilityMessages)pPacket.OperationCode)
                {
                    case InteroperabilityMessages.AllocationResponse:
                        this.OnAllocationResponse(pPacket);
                        break;

                    case InteroperabilityMessages.CharacterListRequest:
                        this.OnCharacterListRequest(pPacket);
                        break;

                    case InteroperabilityMessages.CharacterCreationRequest:
                        this.OnCharacterCreationRequest(pPacket);
                        break;
                }
            }
            catch (Exception ex)
            {
                MainForm.Instance.Log("Caught an exception :: Interoperability: " + Environment.NewLine + ex.ToString());
            }
        }

        public override void OnDisconnect()
        {
            base.OnDisconnect();
        }

        private void Allocate()
        {
            using (Packet outPacket = new Packet(InteroperabilityMessages.AllocationRequest))
            {
                outPacket.WriteString(GameServer.Name);
                outPacket.WriteString(GameServer.PublicIP.ToString());
                outPacket.WriteUShort(GameServer.Port);
                outPacket.WriteByte(GameServer.WorldID);
                outPacket.WriteString(GameServer.WorldName);

                this.Send(outPacket);
            }
        }

        public void RequestCharacterRegistration(Character character)
        {
            using (Packet outPacket = new Packet(InteroperabilityMessages.CharacterRegistrationRequest))
            {
                outPacket.WriteInt(character.ID);
                outPacket.WriteString(character.Name);
                outPacket.WriteShort(character.Statistics.Job);
                outPacket.WriteByte(character.Statistics.Level);

                this.Send(outPacket);
            }
        }

        public void RequestCharacterDeregistration(Character character)
        {
            using (Packet outPacket = new Packet(InteroperabilityMessages.CharacterDeregistrationRequest))
            {
                outPacket.WriteInt(character.ID);

                this.Send(outPacket);
            }
        }

        private void OnAllocationResponse(Packet inPacket)
        {
            GameServer.ID = inPacket.ReadByte();
        }

        private void OnCharacterListRequest(Packet inPacket)
        {
            string hash = inPacket.ReadString();
            int accountId = inPacket.ReadInt();
            byte worldId = inPacket.ReadByte();

            using (Packet outPacket = new Packet(InteroperabilityMessages.CharacterListResponse))
            {
                outPacket.WriteString(hash);

                List<int> characterIds = new List<int>();

                using (MySqlDataReader reader = GameServer.Database.RunQuery("SELECT ID FROM characters WHERE AccountID = '{0}' AND WorldID = '{1}'", accountId, worldId))
                {
                    while (reader.Read())
                    {
                        characterIds.Add(reader.GetInt32(0));
                    }
                }

                outPacket.WriteByte((byte)characterIds.Count);

                foreach (int id in characterIds)
                {
                    Character character = new Character(id);
                    character.Load(false);

                    character.Append(outPacket);
                }

                this.Send(outPacket);
            }
        }

        private void OnCharacterCreationRequest(Packet inPacket)
        {
            string hash = inPacket.ReadString();
            int accountId = inPacket.ReadInt();
            byte worldId = inPacket.ReadByte();

            string name = inPacket.ReadString();
            int face = inPacket.ReadInt();
            int hair = (inPacket.ReadInt() + inPacket.ReadInt());
            int skin = inPacket.ReadInt();

            int topId = inPacket.ReadInt();
            int bottomId = inPacket.ReadInt();
            int shoesId = inPacket.ReadInt();
            int weaponId = inPacket.ReadInt();

            byte strength = inPacket.ReadByte();
            byte dexterity = inPacket.ReadByte();
            byte intelligence = inPacket.ReadByte();
            byte luck = inPacket.ReadByte();

            Character character = new Character();

            character.AccountID = accountId;
            // TODO: worldId.
            character.Statistics.Level = 1;
            character.Statistics.Strength = strength;
            character.Statistics.Dexterity = dexterity;
            character.Statistics.Intelligence = intelligence;
            character.Statistics.Luck = luck;
            character.Statistics.Job = 0;
            character.Statistics.Fame = 0;
            character.Statistics.AvailableAP = 0;
            character.Statistics.AvailableSP = 0;

            character.Statistics.CurrentHP = 50;
            character.Statistics.MaxHP = 50;
            character.Statistics.CurrentMP = 5;
            character.Statistics.MaxMP = 5;
            character.Meso = 0;
            character.SpawnPoint = 0;
            character.MaxBuddies = 20;

            character.Name = name;
            character.Face = face;
            character.Hair = hair;
            character.Skin = 0;
            character.Gender = 0; // TODO.

            character.Inventory.Add(new Item(topId, equipped: true));
            character.Inventory.Add(new Item(bottomId, equipped: true));
            character.Inventory.Add(new Item(shoesId, equipped: true));
            character.Inventory.Add(new Item(weaponId, equipped: true));

            bool charOk = true;

            if (GameServer.Database.Exists("characters", "Name = '{0}'", name)) // Not containing forbidden name.
            {
                charOk = false;
            }

            if (charOk)
            {
                character.Save();

                using (Packet outPacket = new Packet(InteroperabilityMessages.CharacterCreationResponse))
                {
                    outPacket.WriteString(hash);
                    outPacket.WriteBool(true);
                    character.Append(outPacket);

                    this.Send(outPacket);
                }
            }
            else
            {
                using (Packet outPacket = new Packet(InteroperabilityMessages.CharacterCreationResponse))
                {
                    outPacket.WriteString(hash);
                    outPacket.WriteBool(false);

                    this.Send(outPacket);
                }
            }
        }
    }
}
