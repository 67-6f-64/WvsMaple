using Common;
using Common.Net;

using WvsLogin.Maple;

using System;
using System.Net.Sockets;

namespace WvsLogin.Net
{
    public class MapleClient : Session
    {
        public User User { get; private set; }
        public Account Account { get; private set; }
        public byte WorldID { get; private set; }
        public byte ChannelID { get; private set; }
        public string[] MacAddresses { get; private set; }

        public MapleClient(Socket socket)
            : base(socket)
        {
            this.User = new User(this);

            MainForm.Instance.Log("'{0}' joined.", this.IP);

            LoginServer.AddUser(this.User);

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

                        case ClientMessages.WorldListRequest:
                            this.OnServerListRequest();
                            break;

                        case ClientMessages.CheckUserLimit:
                            this.OnCheckUserLimit(pPacket);
                            break;

                        case ClientMessages.SelectWorld:
                            this.OnSelectWorld(pPacket);
                            break;

                        case ClientMessages.CheckDuplicatedID:
                            this.OnCheckDuplicatedID(pPacket);
                            break;

                        case ClientMessages.CreateNewCharacter:
                            this.OnCreateNewCharacter(pPacket);
                            break;

                        case ClientMessages.SelectCharacter:
                            this.OnSelectCharacter(pPacket);
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
                        case ClientMessages.CheckPassword:
                            this.OnCheckPassword(pPacket);
                            break;

                        case ClientMessages.LicenceAgreement:
                            this.OnLicenceAgreement(pPacket);
                            break;

                        case ClientMessages.CheckPin:
                            this.OnCheckPin(pPacket);
                            break;

                        case ClientMessages.RegisterPin:
                            this.OnRegisterPin(pPacket);
                            break;

                        default:
                            MainForm.Instance.LogPacket(pPacket);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                MainForm.Instance.Log(e.ToString()); // TODO: Log to file.
            }
        }

        private void RespondLogin(LoginResponse response)
        {
            using (Packet outPacket = new Packet(ServerMessages.CheckPasswordResult))
            {
                if (response == LoginResponse.Valid)
                {
                    outPacket.WriteByte();
                    outPacket.WriteByte(); // NOTE: Unknown.
                    outPacket.WriteInt(); // NOTE: Unknown.
                    outPacket.WriteInt(this.Account.ID);
                    outPacket.WriteByte(); // TODO: Gender.
                    outPacket.WriteBool(this.Account.IsMaster); // NOTE: Enables admin commands/restrictions.
                    outPacket.WriteByte(); // NOTE: Unknown.
                    outPacket.WriteString(this.Account.Username);
                    outPacket.WriteByte(); // NOTE: Unknown.
                    outPacket.WriteBool(false); // NOTE: Quiet Ban Enabled.
                    outPacket.WriteLong(); // NOTE: Quiet Ban Expiration.
                }
                else
                {
                    outPacket.WriteByte((byte)response);
                    outPacket.WriteByte(); // NOTE: Unknown.
                    outPacket.WriteInt(); // NOTE: Unknown.
                }

                this.Send(outPacket);
            }
        }

        private void RespondPin(PinResponse response)
        {
            if (response == PinResponse.Valid)
            {
                this.User.IsAssigned = true;
            }

            using (Packet p = new Packet(ServerMessages.PinOperation))
            {
                p.WriteByte((byte)response);

                this.Send(p);
            }
        }

        private void OnCheckPassword(Packet inPacket)
        {
            string username = inPacket.ReadString();
            string password = inPacket.ReadString();

            if (!username.IsAlphaNumeric())
            {
                this.RespondLogin(LoginResponse.NotRegistered);
            }
            else
            {
                this.Account = new Account(this);

                try
                {
                    this.Account.Load(username);

                    if (!this.Account.LicenceAgreement)
                    {
                        this.RespondLogin(LoginResponse.LicenceAgreement);
                    }
                    else if (password != this.Account.Password)
                    {
                        this.RespondLogin(LoginResponse.IncorrectPassword);
                    }
                    else if (this.Account.IsBanned)
                    {
                        this.RespondLogin(LoginResponse.Banned);
                    }
                    else if (this.Account.IsLoggedIn)
                    {
                        this.RespondLogin(LoginResponse.AlreadyLoggedIn);
                    }
                    else
                    {
                        this.RespondLogin(LoginResponse.Valid);
                    }
                }
                catch (NoAccountException)
                {
                    this.RespondLogin(LoginResponse.NotRegistered);
                }
            }
        }

        private void OnLicenceAgreement(Packet inPacket)
        {
            bool agreement = inPacket.ReadBool();

            if (!agreement)
            {
                this.Disconnect();
                return;
            }

            this.RespondLogin(LoginResponse.Valid);
            this.Account.LicenceAgreement = true;
            this.Account.Save();
        }

        private void OnCheckPin(Packet inPacket)
        {
            byte alpha = inPacket.ReadByte();
            byte beta = 0;

            if (inPacket.Remaining > 0)
            {
                beta = inPacket.ReadByte();
            }

            if (alpha == 1 && beta == 1) // NOTE: Request login.
            {
                if (Constants.RequestPin)
                {
                    if (this.Account.Pin == string.Empty)
                    {
                        this.RespondPin(PinResponse.Register);
                    }
                    else
                    {
                        this.RespondPin(PinResponse.Request);
                    }
                }
                else
                {
                    this.Account.IsLoggedIn = true;
                    this.Account.Save();
                    this.RespondPin(PinResponse.Valid);
                }
            }
            else if (beta == 0)
            {
                inPacket.ReadInt(); // NOTE: Unknown.

                if (alpha != 0) // Not canceled.
                {
                    if (inPacket.ReadString() != this.Account.Pin)
                    {
                        this.RespondPin(PinResponse.Invalid);
                    }
                    else
                    {
                        if (alpha == 1) // NOTE: Request pin validation.
                        {
                            this.Account.IsLoggedIn = true;
                            this.Account.Save();
                            this.RespondPin(PinResponse.Valid);
                        }
                        else if (alpha == 2) // NOTE: Request new pin registration.
                        {
                            this.RespondPin(PinResponse.Register);
                        }
                        else
                        {
                            this.RespondPin(PinResponse.Error);
                        }
                    }
                }
            }
            else
            {
                this.RespondPin(PinResponse.Error);
            }
        }

        private void OnRegisterPin(Packet inPacket)
        {
            byte operation = inPacket.ReadByte();
            string pin = inPacket.ReadString();

            if (operation != 0) // NOTE: Not canceled. // TODO: Check if operation could be bool continue.
            {
                this.Account.Pin = pin;
                this.Account.Save();

                using (Packet outPacket = new Packet(ServerMessages.PinAssigned))
                {
                    outPacket.WriteByte();

                    this.Send(outPacket);
                }
            }
            else
            {
                this.RespondPin(PinResponse.Error);
            }
        }

        private void OnServerListRequest()
        {
            LoginServer.Center.RequestServerList(this.User.Hash);
        }

        private void OnCheckUserLimit(Packet inPacket)
        {
            byte worldId = inPacket.ReadByte();

            LoginServer.Center.RequestCheckUserLimit(this.User.Hash, worldId);
        }

        private void OnSelectWorld(Packet inPacket)
        {
            this.WorldID = inPacket.ReadByte();
            this.ChannelID = inPacket.ReadByte();

            LoginServer.Center.RequestCharacterList(this.User.Hash, this.Account.ID, this.WorldID, this.ChannelID);
        }

        private void OnCheckDuplicatedID(Packet inPacket)
        {
            string characterName = inPacket.ReadString();
            bool exists = LoginServer.Database.Exists("characters", "Name = '{0}'", characterName);

            using (Packet outPacket = new Packet(ServerMessages.CheckDuplicatedIDResult))
            {
                outPacket.WriteString(characterName);
                outPacket.WriteBool(exists);

                this.Send(outPacket);
            }
        }

        private void OnCreateNewCharacter(Packet inPacket)
        {
            byte[] characterData = inPacket.ReadLeftoverBytes();

            LoginServer.Center.RequestCharacterCreation(this.User.Hash, this.Account.ID, this.WorldID, characterData);
        }

        private void OnSelectCharacter(Packet inPacket)
        {
            int characterId = inPacket.ReadInt();

            LoginServer.Center.RequestMigration(this.User.Hash, this.WorldID, this.ChannelID, characterId);
        }

        public override void OnDisconnect()
        {
            MainForm.Instance.Log("'{0}' left.", this.IP);

            LoginServer.RemoveUser(this.User);
        }
    }
}
