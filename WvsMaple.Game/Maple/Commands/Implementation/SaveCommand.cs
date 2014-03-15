using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WvsGame.Maple.Commands.Implementation
{
    class SaveCommand : Command
    {
        public override bool IsRestricted
        {
            get { return false; }
        }

        public override string Name
        {
            get { return "save"; }
        }

        public override string Parameters
        {
            get { return string.Empty; }
        }

        public override void Execute(Characters.Character caller, string[] args)
        {
            if (args.Length != 0)
            {
                ShowSyntax(caller);
            }
            else
            {
                caller.Save();
            }
        }
    }
}
