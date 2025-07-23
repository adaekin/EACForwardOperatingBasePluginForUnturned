using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ekin.EACFOB
{
    public class TeleportRequest
    {
        public UnturnedPlayer Player;

        public Vector3 StartPosition;

        public FobData TargetFob;

        public float StartTime;

        public bool IsActive;

        public TeleportRequest(UnturnedPlayer player, FobData targetFob)
        {
            Player = player;
            StartPosition = player.Position;
            TargetFob = targetFob;
            StartTime = Time.time;
            IsActive = true;
        }
    }
}
