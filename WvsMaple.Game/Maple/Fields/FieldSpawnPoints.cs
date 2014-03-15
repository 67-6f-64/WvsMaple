using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace WvsGame.Maple.Fields
{
    public class FieldSpawnPoints : KeyedCollection<int, SpawnPoint>
    {
        public FieldSpawnPoints(Field parent) : base() { }

        protected override int GetKeyForItem(SpawnPoint item)
        {
            return item.ID;
        }
    }
}
