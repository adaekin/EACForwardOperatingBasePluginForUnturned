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
    internal class CommandDisableFobDeploy : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "fobdisable";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            bool isdeployfobenable = EACFOBPlugin.Instance.CanDeployFob;
            if (isdeployfobenable)
            {
                UnturnedChat.Say( "Deploying fob is now deactive", Color.red);
                EACFOBPlugin.Instance.CanDeployFob = false;
            }
            else
            {
                UnturnedChat.Say( "Deploying fob is now active", Color.blue);
                EACFOBPlugin.Instance.CanDeployFob = true;
            }
        }
    }
}
