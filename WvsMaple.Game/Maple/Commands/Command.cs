using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Characters;

namespace WvsGame.Maple.Commands
{
    public abstract class Command
    {
        public abstract bool IsRestricted { get; }
        public abstract string Name { get; }
        public abstract string Parameters { get; }

        public abstract void Execute(Character caller, string[] args);

        public string CombineArgs(string[] args, int start = 0)
        {
            string result = string.Empty;

            for (int i = start; i < args.Length; i++)
            {
                result += args[i] + ' ';
            }

            return result.Trim();
        }

        public string CombineArgs(string[] args, int start, int length)
        {
            string result = string.Empty;

            for (int i = start; i < length; i++)
            {
                result += args[i] + ' ';
            }

            return result.Trim();
        }

        public void ShowSyntax(Character pCaller)
        {
            pCaller.Notify(string.Format("[Syntax] {0}{1} {2}", Constants.CommandIndicator, this.Name, this.Parameters.ClearFormatters()));
        }
    }
}
