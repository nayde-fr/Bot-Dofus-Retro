using System.Collections.Generic;

namespace Bot_Dofus_1._29._1.Utilities.Config
{
    public class ServerInfo
    {
        public List<RealmInfo> RealmInfos { get; set; } = new List<RealmInfo>();
        public string LoginIp { get; set; } = "34.251.172.139";
        public short LoginPort { get; set; } = 443;
        public string Name { get; set; }

        public int Id { get; set; }
    }
}
