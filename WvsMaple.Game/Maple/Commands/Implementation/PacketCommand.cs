using Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WvsGame.Maple.Commands.Implementation
{
    class PacketCommand : Command
    {
        public override bool IsRestricted
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "packet"; }
        }

        public override string Parameters
        {
            get { return "[ hex string ]"; }
        }

        public override void Execute(Characters.Character caller, string[] args)
        {
            if (args.Length == 0)
            {
                ShowSyntax(caller);
            }
            else
            {
                using (Packet outPacket = new Packet())
                {
                    foreach (string s in args)
                    {
                        outPacket.WriteHexString(s);
                    }

                    caller.Notify("[Command] Sent: " + outPacket.ToString());

                    caller.Client.Send(outPacket);
                }
            }
        }
    }
}
