using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WvsGame.Maple.Fields
{
    public class FieldKites : FieldObjects<Kite>
    {
        public FieldKites(Field parent) : base(parent) { }

        protected override void InsertItem(int index, Kite item)
        {
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
        }
    }
}
