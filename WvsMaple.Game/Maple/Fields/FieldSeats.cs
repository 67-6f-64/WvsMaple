using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WvsGame.Maple.Fields
{
    public class FieldSeats : FieldObjects<Seat>
    {
        public FieldSeats(Field parent) : base(parent) { }

        protected override void InsertItem(int index, Seat item)
        {
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
        }
    }
}
