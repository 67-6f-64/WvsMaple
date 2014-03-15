using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WvsGame.Maple.Commands.Implementation
{
    class PositionCommand : Command
    {
        public override bool IsRestricted
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "position"; }
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
                caller.Notify("X: " + caller.Position.X + ", Y: " + caller.Position.Y);
            }
        }
    }
}
