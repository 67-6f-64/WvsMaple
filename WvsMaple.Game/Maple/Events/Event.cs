using Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Characters;
using WvsGame.Maple.Data;
using WvsGame.Maple.Fields;
using WvsGame.Maple.Life;

namespace WvsGame.Maple.Events
{
    public abstract class Event
    {
        public abstract string Name { get; }
        public abstract bool IsBackground { get; }

        public abstract void Initiate();
        public abstract void Initiate(Character participant);

        public object Get(string key)
        {
            Dictionary<string, object> properties = EventFactory.Properties[this.Name];

            if (properties == null)
            {
                return null;
            }

            return properties[key];
        }

        public void Set(string key, object value)
        {
            Dictionary<string, object> properties = EventFactory.Properties[this.Name];

            if (properties == null)
            {
                return;
            }

            if (properties.ContainsKey(key))
            {
                properties[key] = value;
            }
            else
            {
                properties.Add(key, value);
            }
        }

        public Field GetField(int fieldId)
        {
            return MapleData.CachedFields[fieldId];
        }

        public void Spawn(int fieldId, int mapleId, int amount, Position position)
        {
            for (int i = 0; i < amount; i++)
            {
                MapleData.CachedFields[fieldId].Mobs.Add(new Mob(mapleId)
                {
                    Position = position
                });
            }
        }

        public void Warp(int fieldId, int destinationId)
        {
            Field field = MapleData.CachedFields[fieldId];
            List<Character> toWarp = new List<Character>();

            lock (field.Characters)
            {
                foreach (Character character in field.Characters)
                {
                    toWarp.Add(character);
                }
            }

            foreach (Character character in toWarp)
            {
                character.SetField(destinationId);
            }
        }

        public void Broadcast(int fieldId, Packet outPacket)
        {
            MapleData.CachedFields[fieldId].Broadcast(outPacket);
        }
    }
}
