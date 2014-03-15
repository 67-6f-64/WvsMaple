using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WvsGame.Maple
{
    public class InventoryFullException : Exception
    {
        public override string Message
        {
            get
            {
                return "The inventory is full.";
            }
        }
    }
}
