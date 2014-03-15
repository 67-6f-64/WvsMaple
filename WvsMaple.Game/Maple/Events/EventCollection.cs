using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace WvsGame.Maple.Events
{
    public class EventCollection : KeyedCollection<string, Event>
    {
        protected override string GetKeyForItem(Event item)
        {
            return item.Name;
        }
    }
}
