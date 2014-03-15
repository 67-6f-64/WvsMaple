using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WvsGame.Maple.Characters;
using WvsGame.Maple.Data;

namespace WvsGame.Maple.Commands.Implementation
{
    class ItemCommand : Command
    {
        public override bool IsRestricted { get { return true; } }
        public override string Name { get { return "item"; } }
        public override string Parameters { get { return "{ id } [ quantity ]"; } }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length < 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                short quantity = 0;
                bool isQuantitySpecified;

                if (args.Length > 1)
                {
                    isQuantitySpecified = short.TryParse(args[args.Length - 1], out quantity);
                }
                else
                {
                    isQuantitySpecified = false;
                }

                if (quantity < 1)
                {
                    quantity = 1;
                }

                int itemID = -1;


                if (int.TryParse(args[0], out itemID) && MapleData.CachedItems.Contains(itemID))
                {
                    caller.Inventory.Add(new Item(itemID, quantity));
                }
                else
                {
                    caller.Notify("[Command] Invalid item.");
                }
            }
        }
    }
}
