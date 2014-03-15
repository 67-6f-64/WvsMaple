using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Data;

namespace WvsGame.Maple.Commands.Implementation
{
    class MapCommand : Command
    {
        public override bool IsRestricted
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "map"; }
        }

        public override string Parameters
        {
            get { return "[ id ]"; }
        }

        public override void Execute(Characters.Character caller, string[] args)
        {
            if (args.Length != 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                if (MapleData.CachedFields.Contains(int.Parse(args[0])))
                {
                    caller.SetField(int.Parse(args[0]));
                }
                else
                {
                    caller.Notify("[Command] Invalid map.");
                }
            }
        }
    }
}
