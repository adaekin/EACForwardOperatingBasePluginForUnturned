using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekin.EACFOB
{
    public class CommandFob : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "fob";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer val = (UnturnedPlayer)caller;
            if(command.Length == 0)
            {
                UnturnedChat.Say(caller, "How to use: /fob <name>");
                return;
            }
            string key = command[0].ToLower();
            var INST = EACFOBPlugin.Instance;
            if (!INST.Fobs.ContainsKey(key))
            {
                UnturnedChat.Say(caller, command[0] + " FOB not found");
                return;
            }
            FobData fobdata = INST.Fobs[key];
            bool flag = IRocketPlayerExtension.HasPermission(caller, INST.Configuration.Instance.team1permission);
            bool flag2 = IRocketPlayerExtension.HasPermission(caller, INST.Configuration.Instance.team2permission);
            bool flag3 = false;
            if(fobdata.Type == INST.Configuration.Instance.team1type.ToUpper() && flag)
            {
                flag3 = true;
            }
            else if (fobdata.Type == INST.Configuration.Instance.team2type.ToUpper() && flag2)
            {
                flag3 = true;
            }
            if (!flag3)
            {
                UnturnedChat.Say(caller, "You dont have permission to access " + fobdata.Type + " fob");
                return;
            }
            if(INST.TeleportRequests.ContainsKey(val))
            {
                UnturnedChat.Say(caller, "You already teleporting to another FOB!");
                return;
            }
            TeleportRequest value = new TeleportRequest(val, fobdata);
            INST.TeleportRequests[val] = value;
            UnturnedChat.Say(caller, "Teleporting to " + fobdata.Name + ", wait for " + INST.Configuration.Instance.teleportcooldown + " seconds.");

        }
    }
}
