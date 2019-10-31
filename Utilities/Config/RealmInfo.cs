using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot_Dofus_1._29._1.Utilities.Config
{
    public class RealmInfo
    {
        public RealmInfo()
        {
            
        }

        public RealmInfo(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
