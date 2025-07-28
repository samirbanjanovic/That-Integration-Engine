using System;
using System.Configuration;


namespace ThatIntegrationEngine.SettingsElements
{
    /// <summary>
    /// Configuration section for File Processing Engine
    /// </summary>

    public class EngineSettingsSections
        : ConfigurationSection
    {
        /// <summary>
        /// Host name of machine, if none is given then
        /// assigned Windows Machine Name is used
        /// </summary>
        [ConfigurationProperty("HostName")]
        public string HostName
        {
            get
            {
                var hn = (string)this["HostName"];

                return string.IsNullOrWhiteSpace(hn?.Trim()) ? Environment.MachineName : hn;
            }
        }


        /// <summary>
        /// Global key for looking up connection string to database
        /// </summary>
        [ConfigurationProperty("DataSource")]
        public string DataSource
        {
            get
            {
                return (string)this["DataSource"];
            }
        }

        /// <summary>
        /// Allow any running process to finish before stopping the process
        /// </summary>
        [ConfigurationProperty("FinishProcessesBeforeStop", DefaultValue = true)]
        public bool FinishProcessesBeforeStop
        {
            get
            {
                return (bool)this["FinishProcessesBeforeStop"];
            }
        }

        /// <summary>
        /// Path pointing to location of assemblies of each process
        /// </summary>
        [ConfigurationProperty("ProcessesAssemblyRepo", IsRequired = true)]
        public string ProcessesAssemblyRepo
        {
            get
            {
                return (string)this["ProcessesAssemblyRepo"];
            }
        }

        /// <summary>
        /// Pauses loop that steps to next watcher 
        /// </summary>
        [ConfigurationProperty("RetryFailedProcessInSeconds", IsRequired = false, DefaultValue = 300)]
        public int RetryFailedProcessInSeconds
        {
            get
            {
                return (int)this["RetryFailedProcessInSeconds"];
            }
        }

        /// <summary>
        /// Stops to retry process execution after N times
        /// </summary>
        [ConfigurationProperty("StopProcessRetryAfterAttempt", IsRequired = false, DefaultValue = 3)]
        public int StopProcessRetryAfterAttempt
        {
            get
            {
                return (int)this["StopProcessRetryAfterAttempt"];
            }
        }

        /// <summary>
        /// Pauses loop that steps to next watcher 
        /// </summary>
        [ConfigurationProperty("PauseWatcherReinitSeconds", IsRequired = false, DefaultValue = 15)]
        public int PauseWatcherReinitSeconds
        {
            get
            {
                return (int)this["PauseWatcherReinitSeconds"];
            }
        }

        /// <summary>
        /// Try to to re-initialize watchers for given time frame in seconds
        /// </summary>
        [ConfigurationProperty("SystemTryWatcherReinitSeconds", IsRequired = false, DefaultValue = 1800)]
        public int SystemTryWatcherReinitSeconds
        {
            get
            {
                return (int)this["SystemTryWatcherReinitSeconds"];
            }
        }

        /// <summary>
        /// Pause the retry of initializing watchers if it hasn't succeeded so far
        /// </summary>
        [ConfigurationProperty("SystemPauseWatcherReinitSeconds", IsRequired = false, DefaultValue = 300)]
        public int SystemPauseWatcherReinitSeconds
        {
            get
            {
                return (int)this["SystemPauseWatcherReinitSeconds"];
            }
        }

        /// <summary>
        /// Set if the File Processing Engine instance should email 
        /// When an exception occurs
        /// </summary>
        [ConfigurationProperty("EmailOnEngineException", IsRequired = true)]
        public bool EmailOnEngineException
        {
            get
            {
                return (bool)this["EmailOnEngineException"];
            }
        }

        /// <summary>
        /// Set if the File Processing Engine instance should email 
        /// When an exception occurs
        /// </summary>
        [ConfigurationProperty("LogRemainingTaskCount", IsRequired = true)]
        public bool LogRemainingTaskCount
        {
            get
            {
                return (bool)this["LogRemainingTaskCount"];
            }
        }

        /// <summary>
        /// Set if the File Processing Engine instance should email 
        /// When an exception occurs
        /// </summary>
        [ConfigurationProperty("EmailOnProcessException", IsRequired = true)]
        public bool EmailOnProcessException
        {
            get
            {
                return (bool)this["EmailOnProcessException"];
            }
        }

        /// <summary>
        /// Should system send email when watcher raises Error
        /// </summary>
        [ConfigurationProperty("EmailOnWatcherError", IsRequired = true)]
        public bool EmailOnWatcherError
        {
            get
            {
                return (bool)this["EmailOnWatcherError"];
            }
        }

        /// <summary>
        /// Should Engine log initialization every process
        /// </summary>
        [ConfigurationProperty("LogProcessInit", IsRequired = true)]
        public bool LogProcessInit
        {
            get
            {
                return (bool)this["LogProcessInit"];
            }
        }

        /// <summary>
        /// Should Engine log initialization every process
        /// </summary>
        [ConfigurationProperty("LogProcessExec", IsRequired = true)]
        public bool LogProcessExec
        {
            get
            {
                return (bool)this["LogProcessExec"];
            }
        }

        /// <summary>
        /// Should engine wait for tasks to complete before shutting down
        /// </summary>
        [ConfigurationProperty("WaitTaskCompletion", IsRequired = true, DefaultValue = true)]
        public bool WaitTaskCompletion
        {
            get
            {
                return (bool)this["WaitTaskCompletion"];
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
