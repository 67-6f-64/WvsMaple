using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WvsGame.Maple.Characters;

namespace WvsGame.Maple.Commands
{
    public static class CommandFactory
    {
        public static CommandCollection Commands { get; private set; }

        public static void Initialize()
        {
            CommandFactory.Commands = new CommandCollection();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(typeof(Command)))
                {
                    CommandFactory.Commands.Add((Command)Activator.CreateInstance(type));
                }
            }
        }

        public static void Execute(Character pCaller, string pText)
        {
            string[] Splitted = pText.Split(' ');
            Splitted[0] = Splitted[0].ToLower();
            string CommandName = "";
            if (pText.StartsWith(Constants.CommandIndicator))
                CommandName = Splitted[0].TrimStart(Constants.CommandIndicator.ToCharArray());
            else if (pText.StartsWith(Constants.PlayerCommandIndicator))
                CommandName = Splitted[0].TrimStart(Constants.PlayerCommandIndicator.ToCharArray());
            else
                CommandName = Splitted[0];

            string[] args = new string[Splitted.Length - 1];

            for (int i = 1; i < Splitted.Length; i++)
            {
                args[i - 1] = Splitted[i];
            }

            if (CommandFactory.Commands.Contains(CommandName))
            {
                Command command = CommandFactory.Commands[CommandName];

                if ((command.IsRestricted && pText.StartsWith(Constants.CommandIndicator)) || (!command.IsRestricted && pText.StartsWith(Constants.PlayerCommandIndicator)))
                {
                    if ((command.IsRestricted && pCaller.IsMaster) || !command.IsRestricted)
                    {
                        try
                        {
                            command.Execute(pCaller, args);
                        }
                        catch (Exception e)
                        {
                            pCaller.Notify("[Command] Unknown error: " + e.Message);
                            MainForm.Instance.Log("{0} error by {1}: ", true, e, command.GetType().Name, pCaller.Name);
                        }
                    }
                    else
                    {
                        pCaller.Notify("[Command] Restricted command.");
                    }
                }
                else
                {
                    pCaller.Notify("[Command] Invalid command.");
                }
            }
            else
            {
                pCaller.Notify("[Command] Invalid command.");
            }
        }
    }
}
