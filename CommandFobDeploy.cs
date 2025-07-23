using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ekin.EACFOB
{
    public class CommandFobDeploy : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "fobcreate";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer uplayer = (UnturnedPlayer)caller;
            float time = Time.time;
            var INST = EACFOBPlugin.Instance;
            if(INST.CanDeployFob)
            {if (INST.LastFobKurTime.ContainsKey(uplayer))
            {
                float num = INST.LastFobKurTime[uplayer];
                if (time - num < (float)INST.Configuration.Instance.createfobcooldown && !uplayer.IsAdmin)// COOLDOWN CONFIGE ATILACAK
                {
                    int num2 = (int)(EACFOBPlugin.Instance.Configuration.Instance.createfobcooldown - (time - num));
                    UnturnedChat.Say(caller, string.Format("You have to wait {0} seconds for establishing next fob.", num2));
                    return;
                }
            }
            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, "How to use: /fobcreate <team> <name>");
                UnturnedChat.Say(caller, "Teams: " + INST.Configuration.Instance.team1type + ", " + INST.Configuration.Instance.team2type );
                return;
            }
            string text = command[0].ToLower();
            string key = command[1].ToLower();
            if (text != INST.Configuration.Instance.team1type.ToLower() && text != INST.Configuration.Instance.team2type.ToLower())
            {
                UnturnedChat.Say(caller, "Invalid Team");
                UnturnedChat.Say(caller, "Teams: " + INST.Configuration.Instance.team1type + ", " + INST.Configuration.Instance.team2type);
                    return;
            }
            if (!IRocketPlayerExtension.HasPermission(uplayer, text)) // AND A2 TEAM
            {
                UnturnedChat.Say(caller, "You dont have " + text + " Permission.");
                return;
            }
            if (INST.Fobs.ContainsKey(key))
            {
                UnturnedChat.Say(caller, "There is a FOB with this name.");
                return;
            }
            Vector3 position = uplayer.Position;
            Quaternion rotation = uplayer.Player.transform.rotation;
            
            ItemBarricadeAsset bar1 = (ItemBarricadeAsset)Assets.find((EAssetType)1, INST.Configuration.Instance.fobobjectid);
            if (!INST.Configuration.Instance.samefob)
            {
                if(text == INST.Configuration.Instance.team1type) bar1 = (ItemBarricadeAsset)Assets.find((EAssetType)1, INST.Configuration.Instance.team1fobobjectid);
                if(text == INST.Configuration.Instance.team2type) bar1 = (ItemBarricadeAsset)Assets.find((EAssetType)1, INST.Configuration.Instance.team2fobobjectid);


            }
            if (bar1 == null)
            {
                UnturnedChat.Say(caller, "Failed to find Flag Asset, check configuration.");
                return;
            }
            Barricade bar2 = new Barricade(bar1);
            Vector3 pos = new Vector3(position.x, position.y + INST.Configuration.Instance.fobobjectYadjust, position.z);
            Transform tran = BarricadeManager.dropBarricade(bar2, null, pos, 0f, 0f, 0f, uplayer.CSteamID.m_SteamID, 99);
            BarricadeDrop barD = null;
            float inf = float.MaxValue;
            BarricadeRegion[,] regions = BarricadeManager.regions;
            foreach (BarricadeRegion uplayer7 in regions)
            {
                foreach (BarricadeDrop drop in uplayer7.drops)
                {
                    float dis = Vector3.Distance(drop.model.position, position);
                    if (dis < inf)
                    {
                        inf = dis;
                        barD = drop;
                    }
                }
            }
                if (barD != null && inf < 2f + INST.Configuration.Instance.fobobjectYadjust)
                {
                    INST.Fobs[key] = new FobData(position, (ushort)barD.instanceID, command[1], uplayer.CharacterName, text.ToUpper());
                    INST.LastFobKurTime[uplayer] = time;
                    UnturnedChat.Say(caller, text.ToUpper() + " FOB " + command[1] + " succsefully established!");
                }
                else
                {
                    UnturnedChat.Say(caller, text.ToUpper() + " FOB " + command[1] + " failed to established!");
                }
            }
            else
            {
                UnturnedChat.Say(caller, "Deploying fob is not active");
            }
        }
    }
}
