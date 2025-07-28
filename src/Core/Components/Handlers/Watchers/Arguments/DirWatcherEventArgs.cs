using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThatIntegrationEngine.Core.Components
{
    [Serializable]
    public class DirWatcherEventArgs
        : HandlerEventArgs
    {
        public DirWatcherEventArgs(string fullPath, WatcherChangeTypes wct,
            int tid, string tn)
            : base(tid, tn)
        {
            FileWatcherChangeType = wct;
            FullePath = fullPath;
            Directory = Path.GetDirectoryName(FullePath);
            FileName = Path.GetFileName(fullPath);
        }

        /// <summary>
        /// Identifies if a file was created, deleted, changed, renamed, or all
        /// </summary>
        public WatcherChangeTypes FileWatcherChangeType { get; private set; }

        /// <summary>
        /// Fully qualified path to affected file
        /// </summary>
        public string FullePath { get; private set; }

        /// <summary>
        /// Directory affected file
        /// </summary>
        public string Directory { get; private set; }

        /// <summary>
        /// Affected file name
        /// </summary>
        public string FileName { get; private set; }
    }
}
