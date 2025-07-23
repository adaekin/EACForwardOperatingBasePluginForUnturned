using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using static Rocket.Unturned.Events.UnturnedPlayerEvents;
using static SDG.Unturned.DamageTool;
using Steamworks;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Rocket.Core;
using System.Xml;
using Rocket.Core.Steam;

namespace Ekin.EACFOB
{
    public class EACFOBPlugin : RocketPlugin<EACFOBConfiguration>
    {
        public static EACFOBPlugin Instance { get; set; }
        public Dictionary<string, FobData> Fobs = new Dictionary<string, FobData>();


        //public List<TeleportRequest> TeleportRequests = new List<TeleportRequest>();
        public Dictionary<UnturnedPlayer, TeleportRequest> TeleportRequests = new Dictionary<UnturnedPlayer, TeleportRequest>();


        public Dictionary<UnturnedPlayer, float> LastFobKurTime = new Dictionary<UnturnedPlayer, float>();

        public bool CanDeployFob = true;

        public static bool IsBarricadeWithInstanceIdPresent(ushort instanceId)
        {
            BarricadeRegion[,] regions = BarricadeManager.regions;
            foreach (BarricadeRegion val in regions)
            {
                foreach (BarricadeDrop drop in val.drops)
                {
                    if (drop.instanceID == instanceId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override void Load()
        {
            Rocket.Core.Logging.Logger.Log("EACFOB Plugin loaded!", ConsoleColor.White);
            Rocket.Core.Logging.Logger.Log("For Help: discord: realekin | https://discord.gg/tJPd32ESMD", ConsoleColor.White);
            ((MonoBehaviour)this).InvokeRepeating("CheckTeleportRequests", 1f, 1f);
            ((MonoBehaviour)this).InvokeRepeating("CheckFobBarricades", 1f, 1f);
            UnturnedPlayerEvents.OnPlayerDeath += new PlayerDeath(OnPlayerDeath);
            DamageTool.damagePlayerRequested += new DamagePlayerHandler(OnPlayerDamaged);
            Level.onLevelLoaded += levelloaded;


            Instance = this;
        }
        private void levelloaded(int level)
        {
            UnturnedChat.Say("EAC FOB Cleaning Started...");

            int deleted = 0;

            foreach (var region in BarricadeManager.regions)
            {
                for (int i = region.drops.Count - 1; i >= 0; i--)
                {
                    var drop = region.drops[i];

                    ushort id = drop.asset.id;
                    if (id == Configuration.Instance.fobobjectid ||
                        id == Configuration.Instance.team1fobobjectid ||
                        id == Configuration.Instance.team2fobobjectid)
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
        protected override void Unload()
        {
            Rocket.Core.Logging.Logger.Log("EACFOB Plugin unloaded", ConsoleColor.White);
            ((MonoBehaviour)this).CancelInvoke("CheckTeleportRequests");
            ((MonoBehaviour)this).CancelInvoke("CheckFobBarricades");
            foreach (var fob in Fobs.Values)
            {

                foreach (BarricadeRegion region in BarricadeManager.regions)
                {
                    foreach (var drop in region.drops)
                    {
                        if (drop.instanceID == fob.InstanceID)
                        {

                            if (BarricadeManager.tryGetRegion(drop.model.transform, out byte x, out byte y, out ushort plant, out BarricadeRegion region2))
                            {
                                BarricadeManager.destroyBarricade(region2, x, y, plant, (ushort)region2.drops.IndexOf(drop));
                                CheckFobBarricades();
                                Fobs.Remove(fob.Name);
                                UnturnedChat.Say("FOB " + fob.Name + " (" + fob.Type + ") destroyed!", new Color(1f, 0.5f, 0f));
                            }

                        }
                    }
                }
            }
            UnturnedPlayerEvents.OnPlayerDeath -= new PlayerDeath(OnPlayerDeath);
            DamageTool.damagePlayerRequested -= new DamagePlayerHandler(OnPlayerDamaged);
            
        }

        private void CheckFobBarricades()
        {
            List<string> list = new List<string>();
            foreach (FobData fob in Fobs.Values)
            {
                if (!IsBarricadeWithInstanceIdPresent(fob.InstanceID))
                {
                    list.Add(fob.Name);
                }
            }
            foreach (string fobs in list)
            {
                string team = Fobs[fobs].Type;
                Fobs.Remove(fobs);
                UnturnedChat.Say("FOB " + fobs + " (" + team + ") destroyed!", new Color(1f, 0.5f, 0f));
            }
        }

        private void CheckTeleportRequests()
        {
            List<UnturnedPlayer> list = new List<UnturnedPlayer>();
            foreach (TeleportRequest value in TeleportRequests.Values)
            {
                if (!value.IsActive)
                {
                    continue;
                }
                if (value.Player == null || !((Behaviour)value.Player.Player).isActiveAndEnabled)
                {
                    list.Add(value.Player);
                    continue;
                }
                if (value.Player.Dead)
                {
                    UnturnedChat.Say((IRocketPlayer)(object)value.Player, "Teleporation canceled for dying", Color.white);
                    list.Add(value.Player);
                    continue;
                }
                if (Time.time - value.StartTime >= Configuration.Instance.teleportcooldown)
                {
                    Vector3 location = value.TargetFob.Location;
                    location.y += 1f;
                    value.Player.Player.teleportToLocationUnsafe(location, 0);
                    UnturnedChat.Say((IRocketPlayer)(object)value.Player, "You have teleported to "+ value.TargetFob.Name + " FOB!", Color.white);
                    list.Add(value.Player);
                    continue;
                }
                float num3 = Vector3.Distance(value.StartPosition, value.Player.Position);
                if (num3 > 2f)
                {
                    UnturnedChat.Say((IRocketPlayer)(object)value.Player, "Teleportation canceled for moving.", Color.white);
                    list.Add(value.Player);
                    continue;
                }
                int num4 = Configuration.Instance.teleportcooldown - (int)(Time.time - value.StartTime);
                if (num4 % 5 == 0 && num4 > 0)
                {
                    UnturnedChat.Say((IRocketPlayer)(object)value.Player, string.Format("Teleporting to FOB... {0} seconds left.", num4), Color.white);
                }
            }
            foreach (UnturnedPlayer item in list)
            {
                TeleportRequests.Remove(item);
            }
        }

        public void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            if (TeleportRequests.ContainsKey(player))
            {
                TeleportRequests.Remove(player);
                UnturnedChat.Say((IRocketPlayer)(object)player, "Teleporation canceled for dying", Color.white);
            }
        }

        public void OnPlayerDamaged(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            //IL_0046: Unknown result type (might be due to invalid IL or missing references)
            if (parameters.player != null)
            {
                UnturnedPlayer val = UnturnedPlayer.FromPlayer(parameters.player);
                if (val != null && TeleportRequests.ContainsKey(val))
                {
                    
                    TeleportRequests.Remove(val);
                    UnturnedChat.Say((IRocketPlayer)(object)val, "Teleportation canceled for taking damage", Color.white);
                }
            }
        }
    }
}