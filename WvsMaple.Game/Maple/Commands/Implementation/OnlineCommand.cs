using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WvsGame.Net;

namespace WvsGame.Maple.Commands.Implementation
{
    class OnlineCommand : Command
    {
        public override bool IsRestricted
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "online"; }
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
                caller.Notify("[Online]");

                foreach (var loopCharacter in GameServer.Characters)
                {
                    caller.Notify("   -" + loopCharacter.Value.Name);
                }
            }
        }
    }
}
