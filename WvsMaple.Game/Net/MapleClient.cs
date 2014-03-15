using Common;
using Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using WvsGame.Maple;
using WvsGame.Maple.Characters;

namespace WvsGame.Net
{
    public class MapleClient : Session
    {
        public User User { get; private set; }
        public Character Character
        {
            get
            {
                return this.User.Character;
            }
            set
            {
                this.User.Character = value;
            }
        }

        public MapleClient(Socket socket)
            : base(socket)
        {
            this.User = new User(this);

            this.SendHandshake(Constants.MapleVersion, Constants.PatchLocation, Constants.Localisation);
        }

        public override void OnPacketInbound(Packet pPacket)
        {
            try
            {
                if (this.User.IsAssigned)
                {
                    switch ((ClientMessages)pPacket.OperationCode)
                    {
                        case ClientMessages.Hash:
                            break;

                        case ClientMessages.SetField:
                            this.Character.SetField(pPacket);
                            break;

                        case ClientMessages.CharacterMovement:
                            this.Character.Move(pPacket);
                            break;

                        case ClientMessages.CharacterSit:
                            this.Character.Sit(pPacket);
                            break;

                        case ClientMessages.GeneralChat:
                            this.Character.Talk(pPacket);
                            break;

                        case ClientMessages.NpcConverse:
                            this.Character.ControlledNpcs.Converse(pPacket);
                            break;

                        case ClientMessages.NpcResult:
                            this.Character.LastNpc.Operate(pPacket, this.Character);
                            break;

                        case ClientMessages.ItemMovement:
                            this.Character.Inventory.Operate(pPacket);
                            break;

                        case ClientMessages.DistributeAP:
                            this.Character.DistributeAP(pPacket);
                            break;

                        case ClientMessages.HealOverTime:
                            this.Character.HealOverTime(pPacket);
                            break;

                        case ClientMessages.DistributeSP:
                            this.Character.DistributeSP(pPacket);
                            break;

                        case ClientMessages.CharacterInformation:
                            this.Character.InformOnCharacter(pPacket);
                            break;

                        case ClientMessages.SetScriptedField:
                            this.Character.SetScriptedField(pPacket);
                            break;

                        case ClientMessages.QuestOperation:
                            this.Character.Quests.Operate(pPacket);
                            break;

                        case ClientMessages.NpcAction:
                            this.Character.ControlledNpcs.Act(pPacket);
                            break;

                        default:
                            MainForm.Instance.LogPacket(pPacket);
                            break;
                    }
                }
                else
                {
                    switch ((ClientMessages)pPacket.OperationCode)
                    {
                        case ClientMessages.MigrateIn:
                            this.OnMigrateIn(pPacket);
                            break;

                        default:
                            MainForm.Instance.LogPacket(pPacket);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                MainForm.Instance.Log(e.ToString());
            }
        }

        private void OnMigrateIn(Packet inPacket)
        {
            int characterId = inPacket.ReadInt();

            if (GameServer.Characters.ContainsKey(characterId))
            {
                this.Disconnect();

                return;
            }

            this.Character = new Character(characterId, this);
            this.Character.Load(true);

            GameServer.AddCharacter(this.Character);
            MainForm.Instance.Log("'{0}' joined.", this.Character.Name);
            GameServer.Center.RequestCharacterRegistration(this.Character);

            this.User.IsAssigned = true;
            this.Character.Initialize(-1);
        }

        public override void OnDisconnect()
        {
            if (this.User.IsAssigned && this.Character != null)
            {
                this.Character.Save();

                GameServer.RemoveCharacter(this.Character);
                MainForm.Instance.Log("'{0}' left.", this.Character.Name);

                if (!this.User.IsMigrating)
                {
                    GameServer.Center.RequestCharacterDeregistration(this.Character);
                }

                this.Character.Deinitialize();
                this.Character = null;
            }

            GameServer.RemoveUser(this.User);
        }
    }
}
