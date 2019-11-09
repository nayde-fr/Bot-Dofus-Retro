using System;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Bot_Dofus_1._29._1.Utilities.Config
{
    public static class ConfigurationManager
    {
        private static readonly string ConfigurationPath = Path.Combine(Directory.GetCurrentDirectory(), "Configuration.json");
        public static Configuration Configuration { get; private set; }

        public static void Load()
        {
            Configuration = File.Exists(ConfigurationPath) ? JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigurationPath)) : new Configuration();
        }

        public static void Save()
        {
            File.WriteAllText(ConfigurationPath,JsonConvert.SerializeObject(Configuration,Formatting.Indented));
        }

        public static void AddAccount(AccountConfig config)
        {
            if (Configuration.Accounts.Capacity > 0)
            {
                if (Configuration.Accounts.FirstOrDefault(accCfg => accCfg.Username.Contains(config.Username)) != null)
                {
                    var dialogResult = MessageBox.Show("Username already exists within accounts config, would you like to add it anyways?", "Already Exists", MessageBoxButtons.YesNo);
                    switch (dialogResult)
                    {
                        case DialogResult.Yes:
                            Configuration.Accounts.Add(config);
                            return;
                        case DialogResult.No:
                            return;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            Configuration.Accounts.Add(config);
        }

        public static void DeleteAccount(int index)
        {
            if (Configuration.Accounts.ElementAtOrDefault(index) == null)
            {
                MessageBox.Show("Account not found!", "Not Found", MessageBoxButtons.OK);
                return;
            }

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
