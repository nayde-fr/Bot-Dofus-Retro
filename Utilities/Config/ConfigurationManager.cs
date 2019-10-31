using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot_Dofus_1._29._1.Utilities.Config
{
    public static class ConfigurationManager
    {
        private static readonly string ConfigurationPath = Path.Combine(Directory.GetCurrentDirectory(), "Configuration.json");

        public static Configuration Configuration { get; private set; }

        public static void Load()
        {
            if (File.Exists(ConfigurationPath))
            {
                Configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigurationPath));
            }
            else
            {
                Configuration = new Configuration();
            }
        }

        public static void Save()
        {
            File.WriteAllText(ConfigurationPath,JsonConvert.SerializeObject(Configuration,Formatting.Indented));
        }

        public static void AddAccount(AccountConfig config)
        {
            Configuration.Accounts.Add(config);
        }

        public static void DeleteAccount(int index)
        {
            Configuration.Accounts.RemoveAt(index);
        }

        public static AccountConfig GetAccount(int index)
        {
            return Configuration.Accounts.ElementAtOrDefault(index);
        }

        public static AccountConfig GetAccount(string username)
        {
            return Configuration.Accounts.FirstOrDefault(a => a.Username == username);
        }
        

        public static bool ShouldDebug()
        {
            return Configuration.DebugPackets;
        }
    }
}
