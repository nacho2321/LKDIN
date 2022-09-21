using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace LkdinConnection
{
    public class SettingsManager
    {
        public string ReadSettings(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key] ?? string.Empty;
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error leyendo la configuracion");
                return string.Empty;
            }
        }
    }
}
