using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WvsGame.Net;

namespace WvsGame.Maple.Commands.Implementation
{
    class WarpCommand : Command
    {
        public override bool IsRestricted
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "warp"; }
        }

        public override string Parameters
        {
            get { return "[ name ]"; }
        }

        public override void Execute(Characters.Character caller, string[] args)
        {
            if (args.Length != 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                Characters.Character victim = GameServer.GetCharacter(args[0]);

                if (victim == null)
                {
                    caller.Notify("[Command] Unable to locate '" + args[0] + ".");
                    return;
                }

                caller.SetField(victim.Field.MapleID, victim.ClosestSpawnPoint.ID);
            }
        }
    }
}
