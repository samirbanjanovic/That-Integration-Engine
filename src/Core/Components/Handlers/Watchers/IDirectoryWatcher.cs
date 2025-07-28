using System;
using System.IO;
using System.Threading.Tasks;

namespace ThatIntegrationEngine.Core.Components
{
    public interface IDirectoryWatcher
        : IHandler
    {
        string Directory { get; }

        string FileFilter { get; }

        bool IsEnabled { get; set; }

        bool IsRemoteLocation { get; }

        void InitializeWatcher();


        event EventHandler<ErrorEventArgs> ErrorAsync;
        event EventHandler<DirWatcherEventArgs> FileArrivedAsync;

        void CheckForExistingFiles();

        Task<ErrorEventArgs> CheckMonitoredPath();
    }
}