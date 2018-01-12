using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThatIntegrationEngine.Core.Components
{
    public interface IDirectoryWatcherCollection
        : IEnumerable<IDirectoryWatcher>
        , IDisposable
    {
        IDirectoryWatcher this[int id] { get; }
    
        void Add(IDirectoryWatcher dw);

        IDirectoryWatcher Add(int id, string n, string dir, string filter, bool isrl, bool asyncResults);

        IDirectoryWatcher GetWatcher(int id);

        void ActiveSwitch(bool on);

        Task CheckForExistingFiles();
    }
}