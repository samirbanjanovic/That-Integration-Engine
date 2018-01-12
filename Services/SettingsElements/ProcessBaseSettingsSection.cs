using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ThatIntegrationEngine.SettingsElements
{
    /// <summary>
    /// Configuration section class to be used if Processes want to use config file as well
    /// </summary>
    public class ProcessBaseSettingsSection
        : ConfigurationSection
    {
        /// <summary>
        /// Stores data source name to look up for processes
        /// </summary>
        [ConfigurationProperty("DataSource", IsRequired = false)]
        public string DataSource
        {
            get
            {
                return (string)this["DataSource"];
            }
        }

        /// <summary>
        /// Configuration settings for emailing errors, updates, etc.
        /// </summary>
        [ConfigurationProperty("EmailSettings")]
        public EmailElement EmailSettings
        {
            get
            {
                return (EmailElement)this["EmailSettings"];
            }
        }
    }
}
