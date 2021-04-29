using System;
using System.Text;
using Intel.Dal;

using System.Configuration;
using System.Windows.Forms;


namespace EH_Project_DalHost
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            if (ConfigurationManager.AppSettings["installed"] == "false")
                firstInstall();
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FacebookForm());
            }
        }

        private static void firstInstall()
        {
            TEEConnector te = new TEEConnector();

            try
            {
                te.DALCreatSession();

                string userid = "{c7cdf2ed-cc9e-45c9-b042-9ca240e207a8}";
                string userName = "haleli";
                te.WriteKeyToTEE(userid, userName);
            }
            finally
            {
                te.DALCloseSession();
            }

            UpdateAppSettings("installed", "true");
        }

        public static void UpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                settings[key].Value = value;
                
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }
}