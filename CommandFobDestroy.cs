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
    internal class CommandFobDestroy : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "fobdestroy";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {

            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, "How to use: /fobdestroy <id>");
                return;
            }

            var INST = EACFOBPlugin.Instance;
            string fobid = command[0];

            var match = INST.Fobs.FirstOrDefault(pair => pair.Value.InstanceID.ToString() == fobid);
            if (match.Equals(default(KeyValuePair<string, FobData>)))
            {
                UnturnedChat.Say(caller, $"FOB ID {fobid} not found.");
                return;
            }

            var fobinfo = match.Value;
            bool destroyed = false;

            foreach (var region in BarricadeManager.regions)
            {
                for (int i = region.drops.Count - 1; i >= 0; i--)
                {
                    var drop = region.drops[i];

                    if (drop.instanceID == fobinfo.InstanceID)
                    {
                        if (BarricadeManager.tryGetRegion(drop.model.transform, out byte x, out byte y, out ushort plant, out BarricadeRegion region2))
                        {
                            int index = region2.drops.FindIndex(d => d.instanceID == drop.instanceID);
                            if (index != -1)
                            {
                                BarricadeManager.destroyBarricade(region2, x, y, plant, (ushort)index);
                                UnturnedChat.Say(caller, $"FOB {fobinfo.Name} ({fobinfo.Type}) destroyed!", new Color(1f, 0.5f, 0f));
                                destroyed = true;
                                break;
                            }
                        }
                    }
                }

                if (destroyed)
                    break;
            }

            if (!destroyed)
            {
                UnturnedChat.Say(caller, "Failed to Find Fob");
            }
            //UnturnedPlayer player = (UnturnedPlayer)caller;

            //if (command.Length == 0 || command.Length < 1)
            //{
            //    UnturnedChat.Say(caller, "How to use: /fobdestroy <id>");
            //    return;
            //}
            //var INST = EACFOBPlugin.Instance;
            //string fobid = command[0];
            //var match = INST.Fobs.FirstOrDefault(pair => pair.Value.InstanceID.ToString() == fobid);
            //if (!match.Equals(default(KeyValuePair<string, FobData>)))
            //{
            //    var fobinfo = match.Value;

            //    foreach (var fob in INST.Fobs.Values)
            //    {
            //        foreach (var region in BarricadeManager.regions)
            //        {
            //            foreach (var drop in region.drops)
            //            {
            //                if (drop.instanceID == fobinfo.InstanceID)
            //                {
            //                    if (BarricadeManager.tryGetRegion(drop.model.transform, out byte x, out byte y, out ushort plant, out BarricadeRegion region2))
            //                    {
            //                        BarricadeManager.destroyBarricade(region2, x, y, plant, (ushort)region2.drops.IndexOf(drop));
            //                        UnturnedChat.Say(caller, "FOB " + fob.Name + " (" + fob.Type + ") destroyed!", new Color(1f, 0.5f, 0f));
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    return;
            //}
        }
    }
}
