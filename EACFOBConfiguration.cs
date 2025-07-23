using Rocket.API;

namespace Ekin.EACFOB
{
    public class EACFOBConfiguration : IRocketPluginConfiguration
    {
        public ushort teleportcooldown { get; set; }
        public ushort createfobcooldown { get; set; }
        public ushort fobobjectid { get; set; }
        public bool samefob { get; set; }
        public ushort team1fobobjectid { get; set; }
        public ushort team2fobobjectid { get; set; }
        public float fobobjectYadjust { get; set; }
        public string team1permission { get; set; }
        public string team2permission { get; set; }
        public string team1type { get; set; }
        public string team2type { get; set; }

        public void LoadDefaults()
        {
            teleportcooldown = 20;
            createfobcooldown = 600;
            fobobjectid = 1232;
            samefob = false;
            team1fobobjectid = 1233;
            team2fobobjectid = 1234;
            fobobjectYadjust = +1;
            team1permission = "usa";
            team2permission = "russia";
            team1type = "usa";
            team2type = "russia";

        }
    }
}
