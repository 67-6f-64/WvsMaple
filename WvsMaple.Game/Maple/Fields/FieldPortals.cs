using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WvsGame.Maple.Fields
{
    public class FieldPortals : FieldObjects<Portal>
    {
        public FieldPortals(Field parent) : base(parent) { }

        protected override int GetKeyForItem(Portal item)
        {
            return item.ID;
        }

        public Portal this[string label]
        {
            get
            {
                foreach (Portal loopPortal in this)
                {
                    if (loopPortal.Label.ToLower() == label.ToLower())
                    {
                        return loopPortal;
                    }
                }

                throw new KeyNotFoundException();
            }
        }
    }
}
