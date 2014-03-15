using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WvsGame.Maple.Characters;

namespace WvsGame.Maple.Events
{
    public static class EventFactory
    {
        public static EventCollection Events { get; private set; }
        public static Dictionary<string, Dictionary<string, object>> Properties { get; private set; }

        public static void Initialize()
        {
            EventFactory.Events = new EventCollection();
            EventFactory.Properties = new Dictionary<string, Dictionary<string, object>>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(typeof(Event)))
                {
                    Event evt = (Event)Activator.CreateInstance(type);

                    Events.Add(evt);
                    Properties.Add(evt.Name, new Dictionary<string, object>());

                    if (evt.IsBackground)
                    {
                        evt.Initiate();
                    }
                }
            }
        }

        public static void Initiate(string name, Character character)
        {
            Event evt = EventFactory.Events[name];

            if (evt == null)
            {
                return;
            }

            evt.Initiate(character);
        }

        public static object GetProperty(string name, string key)
        {
            Dictionary<string, object> properties = EventFactory.Properties[name];

            if (properties == null)
            {
                return null;
            }

            return properties[key];
        }

        public static void SetProperty(string name, string key, string value)
        {
            Dictionary<string, object> properties = EventFactory.Properties[name];

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
    }
}
