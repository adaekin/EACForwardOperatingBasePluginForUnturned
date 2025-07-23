using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ekin.EACFOB
{
    internal class CommandClearFobandBarricades : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "FobCleanAll";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {

            var INST = EACFOBPlugin.Instance;
            UnturnedChat.Say(caller,"Fob cleaning started.", Color.magenta);

            int deleted = 0;

            foreach (var region in BarricadeManager.regions)
            {
                for (int i = region.drops.Count - 1; i >= 0; i--)
                {
                    var drop = region.drops[i];

                    ushort id = drop.asset.id;
                    if (id == INST.Configuration.Instance.fobobjectid ||
                        id == INST.Configuration.Instance.team1fobobjectid ||
                        id == INST.Configuration.Instance.team2fobobjectid)
                    {
                        if (BarricadeManager.tryGetRegion(drop.model.transform, out byte x, out byte y, out ushort plant, out BarricadeRegion correctRegion))
                        {
                            int index = correctRegion.drops.FindIndex(d => d.instanceID == drop.instanceID);
                            if (index != -1)
                            {
                                BarricadeManager.destroyBarricade(correctRegion, x, y, plant, (ushort)index);
                                deleted++;
                            }
                        }
                    }
                }
            }

            UnturnedChat.Say($"Fob cleaning complated, {deleted} Fob/OldBarricade deleted.", Color.cyan);
        }
    }
}
