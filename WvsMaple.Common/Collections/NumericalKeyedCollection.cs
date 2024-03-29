﻿using System.Collections.ObjectModel;

namespace Common.Collections
{
    public abstract class NumericalKeyedCollection<TItem> : KeyedCollection<int, TItem>
    {
        public TItem GetAtIndex(int index)
        {
            return base.Items[index];
        }
    }
}
