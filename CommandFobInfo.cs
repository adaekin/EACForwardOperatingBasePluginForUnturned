using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Ekin.EACFOB
{
    internal class CommandFobInfo : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "fobinfo";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, "How to use: /fobinfo <fobname/id>");
                return;
            }
            var INST = EACFOBPlugin.Instance;
            string fobname = command[0];

            if (INST.Fobs.ContainsKey(fobname))
            {
                var fobinfo = INST.Fobs[fobname];
                UnturnedChat.Say(caller, fobname + " INFO:", Color.yellow);
                UnturnedChat.Say(caller, "Fob Created by : " + fobinfo.Owner, Color.white);
                UnturnedChat.Say(caller, "Fob Team : " + fobinfo.Type, Color.white);
                UnturnedChat.Say(caller, "Fob ID : " + fobinfo.InstanceID, Color.white);
                UnturnedChat.Say(caller, "Fob Location : " + fobinfo.Location, Color.white);
                return;
            }
            var match = INST.Fobs.FirstOrDefault(pair => pair.Value.InstanceID.ToString() == fobname);
            if (!match.Equals(default(KeyValuePair<string, FobData>)))
            {
                var fobinfo = match.Value;

                UnturnedChat.Say(caller,"Listed By ID:", Color.gray);
                UnturnedChat.Say(caller, fobinfo.Name + " INFO:",Color.yellow);
                UnturnedChat.Say(caller, "Fob Created by : " + fobinfo.Owner, Color.white);
                UnturnedChat.Say(caller, "Fob Team : " + fobinfo.Type, Color.white);
                UnturnedChat.Say(caller, "Fob ID : " + fobinfo.InstanceID, Color.white);
                UnturnedChat.Say(caller, "Fob Location : " + fobinfo.Location, Color.white);
                return;
            }
            else
            {
                UnturnedChat.Say(caller, "Failed to find " + fobname + " Fob.", Color.red);
                return;
            }
        }
    }
}
