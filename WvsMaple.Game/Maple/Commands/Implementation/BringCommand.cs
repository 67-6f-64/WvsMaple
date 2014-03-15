﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WvsGame.Maple.Characters;
using WvsGame.Net;

namespace WvsGame.Maple.Commands.Implementation
{
    public class BringCommand : Command
    {
        public override bool IsRestricted
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "bring"; }
        }

        public override string Parameters
        {
            get { return "[ name ]"; }
        }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                Character victim = GameServer.GetCharacter(args[0]);

                if (victim == null)
                {
                    caller.Notify("[Command] Unable to locate '" + args[0] + ".");
                    return;
                }

                victim.SetField(caller.Field.MapleID, caller.ClosestSpawnPoint.ID);
            }
        }
    }
}
