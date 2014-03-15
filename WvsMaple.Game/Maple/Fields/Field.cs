using Common;
using Common.Net;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Characters;

namespace WvsGame.Maple.Fields
{
    public class Field : MarshalByRefObject
    {
        public int MapleID { get; set; }
        public int ReturnMapID { get; set; }
        public int ForcedReturnMapID { get; set; }
        public sbyte RegenerationRate { get; set; }
        public byte DecreaseHP { get; set; }
        public ushort DamagePerSecond { get; set; }
        public int ProtectorItemID { get; set; }
        public bool HasShip { get; set; }
        public byte RequiredLevel { get; set; }
        public int TimeLimit { get; set; }
        public double SpawnRate { get; set; }
        public bool IsTown { get; set; }
        public bool HasClock { get; set; }
        public bool IsEverlasting { get; set; }
        public bool DisablesTownScroll { get; set; }
        public bool IsSwim { get; set; }
        public bool ShufflesReactors { get; set; }
        public string UniqueShuffledReactor { get; set; }
        public bool IsShop { get; set; }
        public bool NoPartyLeaderPass { get; set; }

        public FieldCharacters Characters { get; private set; }
        public FieldDrops Drops { get; private set; }
        public FieldMobs Mobs { get; private set; }
        public FieldNpcs Npcs { get; private set; }
        public FieldPortals Portals { get; private set; }
        public FieldSpawnPoints SpawnPoints { get; private set; }
        public FieldSeats Seats { get; private set; }
        public FieldKites Kites { get; private set; }

        public LoopingID ObjectIDs { get; private set; }

        public Field()
        {
            this.Characters = new FieldCharacters(this);
            this.Drops = new FieldDrops(this);
            this.Mobs = new FieldMobs(this);
            this.Npcs = new FieldNpcs(this);
            this.Portals = new FieldPortals(this);
            this.SpawnPoints = new FieldSpawnPoints(this);
            this.Seats = new FieldSeats(this);
            this.Kites = new FieldKites(this);

            this.ObjectIDs = new LoopingID();
        }

        public void Broadcast(Packet outPacket, Character ignored = null)
        {
            lock (this.Characters)
            {
                foreach (Character character in this.Characters)
                {
                    if (character != ignored)
                    {
                        character.Client.Send(outPacket);
                    }
                }
            }
        }

        public void ShowEffect(FieldEffect effect, Character character = null)
        {
            if (effect == FieldEffect.ShipArrive || effect == FieldEffect.ShipLeave)
            {
                using (Packet outPacket = new Packet(ServerMessages.ContiState))
                {
                    outPacket.WriteShort((short)(effect == FieldEffect.ShipArrive ? 0 : 3));

                    if (character == null)
                    {
                        this.Broadcast(outPacket);
                    }
                    else
                    {
                        character.Client.Send(outPacket);
                    }
                }
            }
        }

        public int AssignObjectID()
        {
            lock (this.ObjectIDs)
            {
                return this.ObjectIDs.NextValue();
            }
        }
    }
}
