using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WvsGame.Maple.Commands.Implementation
{
    class MesoCommand : Command
    {
        public override bool IsRestricted
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "meso"; }
        }

        public override string Parameters
        {
            get { return "[ amount ]"; }
        }

        public override void Execute(Characters.Character caller, string[] args)
        {
            if (args.Length != 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                int amount = 0;

                if (int.TryParse(args[0], out amount))
                {
                    caller.Meso = amount;
                }
                else
                {
                    caller.Notify("[Command] Invalid amount.");
                }
            }
        }
    }
}
