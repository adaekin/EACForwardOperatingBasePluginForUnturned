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
    public class CommandFobs : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "fobs";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer val = (UnturnedPlayer)caller;
            if(EACFOBPlugin.Instance.Fobs.Count == 0)
            {
                UnturnedChat.Say(caller, "There is no active fob");
            }
            
            bool t1perm = IRocketPlayerExtension.HasPermission(caller, EACFOBPlugin.Instance.Configuration.Instance.team1permission);
            bool t2perm = IRocketPlayerExtension.HasPermission(caller, EACFOBPlugin.Instance.Configuration.Instance.team2permission);
            if(!t1perm && !t2perm)
            {
                UnturnedChat.Say(caller, "You dont have permissions to see fobs.");
                return;
            }
            UnturnedChat.Say(caller, "=== FOB LIST ===");
            int num = 0;
            foreach (var fob in EACFOBPlugin.Instance.Fobs)
            {
                bool flag = false;
                if(fob.Value.Type == EACFOBPlugin.Instance.Configuration.Instance.team1type.ToUpper() && t1perm)
                {
                    flag = true;
                }
                else if(fob.Value.Type == EACFOBPlugin.Instance.Configuration.Instance.team2type.ToUpper() && t2perm)
                {
                    flag = true;
                }
                if (flag)
                {
                    UnturnedChat.Say(caller, "* " + fob.Value.Name);
                    num++;
                }
                
            }
            if (num == 0)
            {
                UnturnedChat.Say(caller, "There is no active fob that you can see.");
            }
        }
    }
}
