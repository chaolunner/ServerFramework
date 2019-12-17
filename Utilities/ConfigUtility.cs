using System.Configuration;
using System.IO;
using System;

namespace ServerFramework
{
    class ConfigUtility
    {
        private const string AppSettingsStr = "appSettings";
        private const string ConnectionStringsStr = "ConnectionStrings";
        private static string AppConfigPath = AppDomain.CurrentDomain.BaseDirectory + "../../App.config";

        public static ConnectionStringSettings GetConnectionStringsConfig(string name)
        {
            return ConfigurationManager.ConnectionStrings[name];
        }

        public static void SetConnectionStringsConfig(string name, string connectionString, string providerName)
        {
            bool isModified = false;
            if (ConfigurationManager.ConnectionStrings[name] != null)
            {
                isModified = true;
            }

            ConnectionStringSettings settings =
                new ConnectionStringSettings(name, connectionString, providerName);
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (isModified)
            {
                config.ConnectionStrings.ConnectionStrings.Remove(name);
            }

            config.ConnectionStrings.ConnectionStrings.Add(settings);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(ConnectionStringsStr);
        }

        public static string GetAppConfig(string key)
        {
            for (int i = 0; i < ConfigurationManager.AppSettings.Count; i++)
            {
                if (ConfigurationManager.AppSettings.GetKey(i) == key)
                {
                    return ConfigurationManager.AppSettings[key];
                }
            }
            return null;
        }

        public static void SetAppConfig(string key, string value)
        {
            bool isModified = false;
            for (int i = 0; i < ConfigurationManager.AppSettings.Count; i++)
            {
                if (ConfigurationManager.AppSettings.GetKey(i) == key)
                {
                    isModified = true;
                    break;
                }
            }

            Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (isModified)
            {
                config.AppSettings.Settings.Remove(key);
            }

            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(AppSettingsStr);
        }

        public static void UpdateRawConfig()
        {
            File.Delete(AppConfigPath);
            File.Copy(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, AppConfigPath);
        }
    }
}
