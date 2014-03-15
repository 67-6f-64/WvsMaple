using Common.Net;
using Common.Threading;
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

        public Dictionary<string, Field> Fields {get; set;}

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

        public void Register(Action method, int delay)
        {
            Delay.Execute(delay, () => {
                method();
            });
        }

        public void AddField(string label, int fieldId)
        {
            if (this.Fields.ContainsKey(label))
            {
                return;
            }

            this.Fields.Add(label, MapleData.CachedFields[fieldId]);
        }

        public void RemoveField(string label)
        {
            if (!this.Fields.ContainsKey(label))
            {
                return;
            }

            this.Fields.Remove(label);
        }

        public void Warp(string labelFrom, string labelTo)
        {
            Field fieldFrom = this.Fields[labelFrom];
            Field fieldTo = this.Fields[labelTo];

            List<Character> toWarp = new List<Character>();

            lock (fieldFrom.Characters)
            {
                foreach (Character character in fieldFrom.Characters)
                {
                    toWarp.Add(character);
                }
            }

            foreach (Character character in toWarp)
            {
                character.SetField(fieldTo.MapleID);
            }
        }

        public void SetMusic(string label)
        {

        }

        public void Broadcast(int fieldId, Packet outPacket)
        {
            MapleData.CachedFields[fieldId].Broadcast(outPacket);
        }
    }
}
