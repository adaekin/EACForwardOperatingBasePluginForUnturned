using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ekin.EACFOB
{
    internal class CommandAllFobs : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "afobs";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer val = (UnturnedPlayer)caller;
            if (EACFOBPlugin.Instance.Fobs.Count == 0)
            {
                UnturnedChat.Say(caller, "There is no active fob", Color.yellow);
                return;
            }
            UnturnedChat.Say(caller, "=== FOB LIST ===", Color.yellow);
            int num = 0;
            foreach (var fob in EACFOBPlugin.Instance.Fobs)
            {
                UnturnedChat.Say(caller, "* " + fob.Value.Name + " Team: " + fob.Value.Type +  " ID: " + fob.Value.InstanceID, Color.white);
                num++;

            }
            UnturnedChat.Say(caller, num + " Fobs Listed", Color.blue);
        }
    }
}
