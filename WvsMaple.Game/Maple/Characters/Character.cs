using Common;
using Common.Net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Commands;
using WvsGame.Maple.Data;
using WvsGame.Maple.Fields;
using WvsGame.Maple.Life;
using WvsGame.Maple.Scripting;
using WvsGame.Net;

namespace WvsGame.Maple.Characters
{
    public class Character : FieldObject, ISpawnable
    {
        public User User { get; private set; }
        public MapleClient Client { get; private set; }

        public int ID { get; private set; }
        public int AccountID { get; set; }
        public string Name { get; set; }
        public bool IsInitialized { get; private set; }
        public bool IsMigrating { get; set; }

        private DateTime LastHealHPOverTime = new DateTime();
        private DateTime LastHealMPOverTime = new DateTime();

        public CharacterInventory Inventory { get; private set; }
        public CharacterStatistics Statistics { get; private set; }
        public CharacterSkills Skills { get; private set; }
        public CharacterQuests Quests { get; private set; }
        public CharacterSavedLocations SavedLocations { get; private set; }

        public ControlledMobs ControlledMobs { get; private set; }
        public ControlledNpcs ControlledNpcs { get; private set; }

        public Npc LastNpc { get; set; }
        public NpcScript NpcSession { get; set; }

        public byte Gender { get; set; }
        public byte SpawnPoint { get; set; }
        public byte MaxBuddies { get; set; }
        public bool IsMaster { get; set; }
        public byte PortalCount { get; set; }

        private byte skin;
        private int hair;
        private int face;
        private int meso;

        public byte Skin
        {
            get
            {
                return skin;
            }
            set
            {
                skin = value;

                if (this.IsInitialized)
                {
                    this.UpdateStatistics(StatisticType.Skin);
                    this.UpdateLooks();
                }
            }
        }

        public int Face
        {
            get
            {
                return face;
            }
            set
            {
                face = value;

                if (this.IsInitialized)
                {
                    this.UpdateStatistics(StatisticType.Face);
                    this.UpdateLooks();
                }
            }
        }

        public int Hair
        {
            get
            {
                return hair;
            }
            set
            {
                hair = value;

                if (this.IsInitialized)
                {
                    this.UpdateStatistics(StatisticType.Hair);
                    this.UpdateLooks();
                }
            }
        }

        public int Meso
        {
            get
            {
                return meso;
            }
            set
            {
                meso = value;

                if (this.IsInitialized)
                {
                    this.UpdateStatistics(StatisticType.Meso);
                }
            }
        }

        public new Field Field
        {
            get
            {
                if (this.IsInitialized)
                {
                    return base.Field;
                }
                else
                {
                    try
                    {
                        return MapleData.CachedFields[GameServer.Database.Fetch("characters", "MapID", "ID = '{0}'", this.ID)];
                    }
                    catch
                    {
                        return MapleData.CachedFields[0];
                    }
                }
            }
            set
            {
                base.Field = value;
            }
        }

        public Portal ClosestPortal
        {
            get
            {
                Portal closestPortal = null;
                double shortestDistance = double.PositiveInfinity;

                foreach (Portal loopPortal in (this.Field.Portals))
                {
                    double distance = loopPortal.Position.DistanceFrom(this.Position);

                    if (distance < shortestDistance)
                    {
                        closestPortal = loopPortal;
                        shortestDistance = distance;
                    }
                }

                return closestPortal;
            }
        }

        public Portal ClosestSpawnPoint
        {
            get
            {
                Portal closestPortal = this.Field.Portals[0];
                double shortestDistance = double.PositiveInfinity;

                foreach (Portal loopPortal in (this.Field.Portals))
                {
                    if (loopPortal.IsSpawnPoint)
                    {
                        double distance = loopPortal.Position.DistanceFrom(this.Position);

                        if (distance < shortestDistance)
                        {
                            closestPortal = loopPortal;
                            shortestDistance = distance;
                        }
                    }
                }

                return closestPortal;
            }
        }

        private bool IsAssigned { get; set; }

        public Character(int id = 0, MapleClient client = null)
        {
            this.ID = id;
            this.Client = client;

            if (this.Client != null)
            {
                this.User = this.Client.User;
            }

            this.Inventory = new CharacterInventory(this);
            this.Statistics = new CharacterStatistics(this);
            this.Skills = new CharacterSkills(this);
            this.Quests = new CharacterQuests(this);
            this.SavedLocations = new CharacterSavedLocations(this);

            this.ControlledMobs = new ControlledMobs(this);
            this.ControlledNpcs = new ControlledNpcs(this);

            this.Position = new Position(0, 0);
        }

        public void Initialize(short flags)
        {
            using (Packet outPacket = new Packet(ServerMessages.SetField))
            {
                outPacket.WriteInt(GameServer.ID);
                outPacket.WriteByte(); // NOTE: Unknown.
                outPacket.WriteBool(true); // NOTE: Is connecting.

                new CharacterRandom().Append(outPacket);
                outPacket.WriteInt(); // NOTE: Unknown, randomly generated.
                outPacket.WriteShort(flags);

                this.Statistics.Append(outPacket);
                this.Inventory.Append(outPacket);
                this.Skills.Append(outPacket);
                this.Quests.Append(outPacket);

                outPacket.WriteShort(); // Mini Games Records.
                outPacket.WriteShort(); // Buddy List Records.

                for (int i = 0; i <= 5; i++)
                {
                    outPacket.WriteInt(999999999);
                }

                outPacket.WriteLong();
                outPacket.WriteLong();
                outPacket.WriteLong();
                outPacket.WriteLong();
                outPacket.WriteLong();
                outPacket.WriteLong();

                this.Client.Send(outPacket);
            }

            this.Field.Characters.Add(this);

            this.IsInitialized = true;
        }

        public void Deinitialize()
        {
            this.IsInitialized = false;

            this.Field.Characters.Remove(this);
        }

        public void Append(Packet outPacket)
        {
            this.AppendStatistics(outPacket);
            this.AppendApperance(outPacket);

            outPacket.WriteInt(); // NOTE: Unknown.
            outPacket.WriteBool(false); // TODO: Ranking.
        }

        public void AppendStatistics(Packet outPacket)
        {
            outPacket.WriteInt(this.ID);
            outPacket.WriteString(this.Name, 13);
            outPacket.WriteByte(this.Gender);
            outPacket.WriteByte(this.Skin);
            outPacket.WriteInt(this.Face);
            outPacket.WriteInt(this.Hair);
            outPacket.WriteLong(); // NOTE: Pet's Unique ID.
            outPacket.WriteByte(this.Statistics.Level);
            outPacket.WriteShort(this.Statistics.Job);
            outPacket.WriteShort(this.Statistics.Strength);
            outPacket.WriteShort(this.Statistics.Dexterity);
            outPacket.WriteShort(this.Statistics.Intelligence);
            outPacket.WriteShort(this.Statistics.Luck);
            outPacket.WriteShort(this.Statistics.CurrentHP);
            outPacket.WriteShort(this.Statistics.MaxHP);
            outPacket.WriteShort(this.Statistics.CurrentMP);
            outPacket.WriteShort(this.Statistics.MaxMP);
            outPacket.WriteShort(this.Statistics.AvailableAP);
            outPacket.WriteShort(this.Statistics.AvailableSP);
            outPacket.WriteInt(this.Statistics.Experience);
            outPacket.WriteShort(this.Statistics.Fame);

            outPacket.WriteInt(this.Field.MapleID);
            outPacket.WriteByte(this.SpawnPoint);
        }

        public void AppendApperance(Packet outPacket)
        {
            outPacket.WriteByte(this.Gender);
            outPacket.WriteByte(this.Skin);
            outPacket.WriteInt(this.Face);
            outPacket.WriteByte();
            outPacket.WriteInt(this.Hair);

            Dictionary<byte, int> visibleLayer = new Dictionary<byte, int>();
            Dictionary<byte, int> hiddenLayer = new Dictionary<byte, int>();

            foreach (Item item in this.Inventory.GetEquipped())
            {
                byte position = item.AbsoluteSlot;

                if (position < 100 && !visibleLayer.ContainsKey(position))
                {
                    visibleLayer[position] = item.MapleID;
                }
                else if (position > 100 && position != 111)
                {
                    position -= 100;

                    if (visibleLayer.ContainsKey(position))
                    {
                        hiddenLayer[position] = visibleLayer[position];
                    }

                    visibleLayer[position] = item.MapleID;
                }
                else if (visibleLayer.ContainsKey(position))
                {
                    hiddenLayer[position] = item.MapleID;
                }
            }

            foreach (KeyValuePair<byte, int> entry in visibleLayer)
            {
                outPacket.WriteByte(entry.Key);
                outPacket.WriteInt(entry.Value);
            }

            outPacket.WriteByte(byte.MaxValue);

            foreach (KeyValuePair<byte, int> entry in hiddenLayer)
            {
                outPacket.WriteByte(entry.Key);
                outPacket.WriteInt(entry.Value);
            }

            outPacket.WriteByte(byte.MaxValue);

            outPacket.WriteInt();
        }

        public void SetField(Packet inPacket)
        {
            inPacket.ReadByte(); // Hmmm?

            FieldTrasnferMode mode = (FieldTrasnferMode)inPacket.ReadInt();


            switch (mode)
            {
                case FieldTrasnferMode.Portal:

                    string portalLabel = inPacket.ReadString();
                    Portal portal = this.Field.Portals[portalLabel];

                    if (portal == null)
                    {
                        return;
                    }

                    this.SetField(portal.DestinationFieldID, portal.Link.ID);

                    break;
            }
        }

        public void SetField(int fieldId, byte portalId = 0)
        {
            this.Field.Characters.Remove(this);
            this.Field = MapleData.CachedFields[fieldId];
            this.SpawnPoint = portalId;
            this.PortalCount++;

            using (Packet outPacket = new Packet(ServerMessages.SetField))
            {
                outPacket.WriteInt(GameServer.ID);
                outPacket.WriteByte(this.PortalCount); // TODO: Portal Count.
                outPacket.WriteBool(false); // NOTE: Is not connecting.
                outPacket.WriteInt(this.Field.MapleID);
                outPacket.WriteByte(this.SpawnPoint);
                outPacket.WriteShort(this.Statistics.CurrentHP);
                outPacket.WriteLong();
                outPacket.WriteLong();
                outPacket.WriteLong();

                this.Client.Send(outPacket);
            }

            this.Field.Characters.Add(this);
        }

        public void SetScriptedField(Packet inPacket)
        {
            string portalLabel = inPacket.ReadString();
            Portal portal = this.Field.Portals[portalLabel];

            if (portal == null || portal.DestinationFieldID != 999999999)
            {
                return;
            }

            this.Release();

            if (GameServer.Scripts[ScriptType.Portal].ContainsKey(portal.Label))
            {
                PortalScript script = (PortalScript)Activator.CreateInstance(GameServer.Scripts[ScriptType.Portal][portal.Label]);

                script.Initiate(this);
                script.Run();
            }
            else
            {
                MainForm.Instance.Log("Character '{0}' tried to go through an unimplemented Portal '{1}'.", this.Name, portal.Label);
            }
        }

        public void Notify(string text, NotificationType type = NotificationType.RedText)
        {
            using (Packet p = new Packet(ServerMessages.BroadcastMsg))
            {
                p.WriteByte((byte)(type));

                if (type == NotificationType.RedText || type == NotificationType.Notice)
                {
                    p.WriteString(text);
                }
                else if (type == NotificationType.Header)
                {
                    p.WriteBool(text.Length == 0 ? false : true);
                    p.WriteString(text);
                }

                this.Client.Send(p);
            }
        }

        public void Release()
        {
            using (Packet outPacket = new Packet(ServerMessages.InventoryOperation))
            {
                outPacket.WriteBool(true); // NOTE: Unknown.
                outPacket.WriteBytes(0, 0); // NOTE: Unknown.

                this.Client.Send(outPacket);
            }
        }

        public void UpdateLooks()
        {

        }

        public void UpdateStatistics(StatisticType type)
        {
            using (Packet outPacket = new Packet(ServerMessages.StatChanged))
            {
                outPacket.WriteBool(false); // NOTE: Reaction From Item.
                outPacket.WriteInt((int)type);

                switch (type)
                {
                    case StatisticType.Skin:
                        outPacket.WriteByte(this.Skin);
                        break;

                    case StatisticType.Face:
                        outPacket.WriteInt(this.Face);
                        break;

                    case StatisticType.Hair:
                        outPacket.WriteInt(this.Hair);
                        break;

                    case StatisticType.Level:
                        outPacket.WriteByte(this.Statistics.Level);
                        break;

                    case StatisticType.Job:
                        outPacket.WriteShort(this.Statistics.Job);
                        break;

                    case StatisticType.Strength:
                        outPacket.WriteShort(this.Statistics.Strength);
                        break;

                    case StatisticType.Dexterity:
                        outPacket.WriteShort(this.Statistics.Dexterity);
                        break;

                    case StatisticType.Intelligence:
                        outPacket.WriteShort(this.Statistics.Intelligence);
                        break;

                    case StatisticType.Luck:
                        outPacket.WriteShort(this.Statistics.Luck);
                        break;

                    case StatisticType.CurrentHP:
                        outPacket.WriteShort(this.Statistics.CurrentHP);
                        break;

                    case StatisticType.MaxHP:
                        outPacket.WriteShort(this.Statistics.MaxHP);
                        break;

                    case StatisticType.CurrentMP:
                        outPacket.WriteShort(this.Statistics.CurrentMP);
                        break;

                    case StatisticType.MaxMP:
                        outPacket.WriteShort(this.Statistics.MaxMP);
                        break;

                    case StatisticType.AvailableAP:
                        outPacket.WriteShort(this.Statistics.AvailableAP);
                        break;

                    case StatisticType.AvailableSP:
                        outPacket.WriteShort(this.Statistics.AvailableSP);
                        break;

                    case StatisticType.Fame:
                        outPacket.WriteShort(this.Statistics.Fame);
                        break;

                    case StatisticType.Meso:
                        outPacket.WriteInt(this.Meso);
                        break;

                    case StatisticType.Experience:
                        outPacket.WriteInt(this.Statistics.Experience);
                        break;
                }

                this.Client.Send(outPacket);
            }
        }

        public void Move(Packet inPacket)
        {
            byte portalCount = inPacket.ReadByte();

            if (portalCount != this.PortalCount)
            {
                MainForm.Instance.Log("Character '{0}' is moving with an invalid Portal Count '{1}'.", this.Name, portalCount);
                return;
            }

            Movements movements = Movements.Parse(inPacket);

            foreach (Movement movement in movements)
            {
                if (movement is AbsoluteMovement)
                {
                    this.Position = ((AbsoluteMovement)movement).Position;
                }

                if (!(movement is EquipmentMovement))
                {
                    this.Stance = movement.NewStance;
                }
            }

            inPacket.Reset(2);

            using (Packet p = new Packet(ServerMessages.CharacterMovement))
            {
                p.WriteInt(this.ID);
                p.WriteBytes(inPacket.ReadLeftoverBytes());

                this.Field.Broadcast(p, this);
            }
        }

        public void Sit(Packet inPacket)
        {
            short seatId = inPacket.ReadShort();
            Seat seat = this.Field.Seats[seatId];

            if (seat == null)
            {
                this.Release();
                return;
            }

            if (seat.IsTaken)
            {
                this.Release();
                return;
            }

            seat.IsTaken = true;

            using (Packet outPacket = new Packet(ServerMessages.CharacterSit))
            {

            }
        }

        public void Talk(Packet inPacket)
        {
            string text = inPacket.ReadString();

            this.Talk(text);
        }

        public void Talk(string text)
        {
            text = text.Replace("{", "{{").Replace("}", "}}");

            if (text.StartsWith(Constants.CommandIndicator) || text.StartsWith(Constants.PlayerCommandIndicator))
            {
                CommandFactory.Execute(this, text);
            }
            else
            {
                using (Packet outPacket = new Packet(ServerMessages.GeneralChat))
                {
                    outPacket.WriteInt(this.ID);
                    outPacket.WriteBool(this.IsMaster);
                    outPacket.WriteString(text);

                    this.Field.Broadcast(outPacket);
                }
            }
        }

        public void DistributeAP(Packet inPacket)
        {
            if (this.Statistics.AvailableAP == 0)
            {
                return;
            }

            uint flag = inPacket.ReadUInt();

            switch (flag)
            {
                case 0x40:

                    this.Statistics.Strength++;

                    break;

                case 0x80:

                    this.Statistics.Dexterity++;

                    break;

                case 0x100:

                    this.Statistics.Intelligence++;

                    break;

                case 0x200:

                    this.Statistics.Luck++;

                    break;

                case 0x800:

                    this.Statistics.MaxHP++;

                    break;

                case 0x2000:

                    this.Statistics.MaxMP++;

                    break;
            }

            this.Statistics.AvailableAP--;
            this.Release();
        }

        public void HealOverTime(Packet inPacket)
        {

        }

        public void DistributeSP(Packet inPacket)
        {
            if (this.Statistics.AvailableSP < 0)
            {
                this.Release();
                return;
            }

            int skillId = inPacket.ReadInt();

            if (!MapleData.CachedSkills.ContainsKey(skillId))
            {
                this.Release();
                return;
            }

            if (!this.Skills.Contains(skillId))
            {
                this.Skills.Add(new Skill(skillId));
            }

            Skill skill = this.Skills[skillId];

            // TODO: Check for skill requirements.

            if (skill.CurrentLevel + 1 <= skill.MaxLevel)
            {
                skill.CurrentLevel++;
            }

            this.Statistics.AvailableSP--;
            this.Release();
        }

        public void InformOnCharacter(Packet inPacket)
        {
            int id = inPacket.ReadInt();

            if (!this.Field.Characters.Contains(id))
            {
                return;
            }

            this.InformOnCharacter(this.Field.Characters[id]);
        }

        public void InformOnCharacter(Character character)
        {
            using (Packet outPacket = new Packet(ServerMessages.CharacterInfo))
            {
                outPacket.WriteInt(character.ID);
                outPacket.WriteByte(character.Statistics.Level);
                outPacket.WriteShort(character.Statistics.Job);
                outPacket.WriteShort(character.Statistics.Job);
                outPacket.WriteString("TestGuild");
                outPacket.WriteBool(false); // TODO: Pet.
                outPacket.WriteByte(); // TODO: WishList Count.

                this.Client.Send(outPacket);
            }
        }

        public void Load(bool initialize = true)
        {
            using (MySqlDataReader reader = GameServer.Database.RunQuery("SELECT * FROM characters WHERE ID = '{0}'", this.ID))
            {
                if (reader.Read())
                {
                    this.ID = reader.GetInt32("ID");
                    this.IsAssigned = true;

                    this.AccountID = reader.GetInt32("AccountID");
                    this.Name = reader.GetString("Name");
                    this.Gender = reader.GetByte("Gender");
                    this.Skin = reader.GetByte("Skin");
                    this.Face = reader.GetInt32("Face");
                    this.Hair = reader.GetInt32("Hair");
                    this.SpawnPoint = reader.GetByte("SpawnPoint");
                    this.Meso = reader.GetInt32("Meso");

                    this.Inventory.MaxSlots[ItemType.Equipment] = reader.GetByte("EquipmentSlots");
                    this.Inventory.MaxSlots[ItemType.Usable] = reader.GetByte("UsableSlots");
                    this.Inventory.MaxSlots[ItemType.Setup] = reader.GetByte("SetupSlots");
                    this.Inventory.MaxSlots[ItemType.Etcetera] = reader.GetByte("EtceteraSlots");
                    this.Inventory.MaxSlots[ItemType.Cash] = reader.GetByte("CashSlots");
                }
            }

            this.IsMaster = GameServer.Database.Fetch("accounts", "IsMaster", "ID = '{0}'", this.AccountID); // TODO: Move elsewhere.

            this.Statistics.Load();
            this.Inventory.Load();

            if (initialize)
            {
                this.Skills.Load();
                this.Quests.Load();
                this.SavedLocations.Load();
            }
        }

        public void Save()
        {
            if (this.IsInitialized)
            {
                this.SpawnPoint = this.ClosestSpawnPoint.ID;
            }

            if (this.IsAssigned)
            {
                string query = "UPDATE characters SET ";

                query += "Skin ='" + this.Skin + "', ";
                query += "Face = '" + this.Face + "', ";
                query += "Hair = '" + this.Hair + "', ";
                query += "MapID = '" + this.Field.MapleID + "', ";
                query += "SpawnPoint = '" + this.SpawnPoint + "', ";
                query += "Meso = '" + this.meso + "' ";

                query += "WHERE ID = '" + this.ID + "'";

                GameServer.Database.RunQuery(query);
            }
            else
            {
                StringBuilder query = new StringBuilder();

                query.Append("INSERT INTO characters (AccountID, WorldID, Name, Gender, Skin, Face, Hair, MapID, SpawnPoint, EquipmentSlots, UsableSlots, SetupSlots, EtceteraSlots, CashSlots, Meso) VALUES (");
                query.AppendFormat("'{0}', ", this.AccountID);
                query.AppendFormat("'{0}', ", 0); // TODO: Variable for this.

                query.AppendFormat("'{0}', ", this.Name);
                query.AppendFormat("'{0}', ", this.Gender);
                query.AppendFormat("'{0}', ", this.Skin);
                query.AppendFormat("'{0}', ", this.Face);
                query.AppendFormat("'{0}', ", this.Hair);
                query.AppendFormat("'{0}', ", this.Field.MapleID);
                query.AppendFormat("'{0}', ", this.SpawnPoint);
                query.AppendFormat("'{0}', ", this.Inventory.MaxSlots[ItemType.Equipment]);
                query.AppendFormat("'{0}', ", this.Inventory.MaxSlots[ItemType.Usable]);
                query.AppendFormat("'{0}', ", this.Inventory.MaxSlots[ItemType.Setup]);
                query.AppendFormat("'{0}', ", this.Inventory.MaxSlots[ItemType.Etcetera]);
                query.AppendFormat("'{0}', ", this.Inventory.MaxSlots[ItemType.Cash]);
                query.AppendFormat("'{0}')", this.Meso);

                GameServer.Database.RunQuery(query.ToString());

                this.ID = GameServer.Database.GetLastInsertId();
            }

            this.Statistics.Save();
            this.Inventory.Save();
            this.Skills.Save();
            this.Quests.Save();
            this.SavedLocations.Save();
        }

        public Packet GetCreatePacket()
        {
            return this.GetSpawnPacket();
        }

        public Packet GetSpawnPacket()
        {
            Packet outPacket = new Packet(ServerMessages.UserEnterField);

            return outPacket;
        }

        public Packet GetDestroyPacket()
        {
            Packet p = new Packet(ServerMessages.UserLeaveField);

            p.WriteInt(this.ID);

            return p;
        }

        public override int ObjectID
        {
            get
            {
                return this.ID;
            }
            set
            {
                base.ObjectID = value;
            }
        }
    }
}
