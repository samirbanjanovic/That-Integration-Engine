using System.Configuration;

namespace ThatIntegrationEngine.SettingsElements
{
    public class EmailElement
        : ConfigurationElement
    {
        /// <summary>
        /// Value to be put in the "From" in email message
        /// </summary>
        [ConfigurationProperty("From", IsRequired = true)]
        public string From
        {
            get
            {
                return (string)this["From"];
            }
        }

        /// <summary>
        /// Email address for recipients.  Multiple addresses can be used using ';'
        /// as the delimiter        
        /// </summary>
        [ConfigurationProperty("Recipients", IsRequired = true)]
        public string Recipients
        {
            get
            {
               return(string)this["Recipients"];
            }
        }

        /// <summary>
        /// List of email address to receive exceptions when
        /// encountered by process or engine. Multiple addresses can be used using ';'
        /// as the delimiter        
        /// </summary>
        [ConfigurationProperty("ExceptionRecipients")]
        public string ExceptionRecipients
        {
            get
            {
                return (string)this["ExceptionRecipients"];
            }

        }
    }
}
