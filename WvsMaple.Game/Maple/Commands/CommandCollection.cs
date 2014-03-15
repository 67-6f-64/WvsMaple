using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace WvsGame.Maple.Commands
{
    public class CommandCollection : KeyedCollection<string, Command>
    {
        protected override string GetKeyForItem(Command pItem)
        {
            return pItem.Name.ToLower();
        }
    }
}
