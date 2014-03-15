using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WvsGame.Maple.Fields
{
    public class FieldDrops : FieldObjects<Drop>
    {
        public FieldDrops(Field field) : base(field) { }

        protected override void InsertItem(int index, Drop item)
        {
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
        }
    }
}
