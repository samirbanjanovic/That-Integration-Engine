using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThatIntegrationEngine.Core.Components
{
    public class DirectoryWatcherCollection
        : IDirectoryWatcherCollection
    {
        private IDictionary<int, IDirectoryWatcher> _dirWatchers;

        public DirectoryWatcherCollection()
        {
            this._dirWatchers = new Dictionary<int, IDirectoryWatcher>();
        }

        public virtual IDirectoryWatcher this[int id]
        {
            get
            {
                return this._dirWatchers[id];
            }
        }

        public virtual IDirectoryWatcher GetWatcher(int id)
        {
            return this[id];
        }

        public virtual void ActiveSwitch(bool on)
        {
            foreach (var dw in this)
            {
                if (!(dw.IsEnabled == on))
                {
                    dw.IsEnabled = on;
                }
            }
        }

        // TO DO: Write "ADD" function
        public virtual void Add(IDirectoryWatcher dw)
        {
            if (dw == null)
            {
                throw new ArgumentNullException(nameof(dw));
            }
            
            var comp = this._dirWatchers.Values.Where(x => x.Directory == dw.Directory);

            if (comp.Any())
            {
                if ((dw.FileFilter == "*.*" || dw.FileFilter == ".*"))
                {
                    throw new Exception($"Cannot add a universal filter to an already watched path {dw.Directory}");
                }

                if (comp.Any(p => p.FileFilter == "*.*" || p.FileFilter == ".*"))
                {
                    throw new Exception($"Cannot add a filter to a path that has a universal filter {dw.Directory}");
                }

                if (comp.Any(p => p.FileFilter.Equals(dw.FileFilter, StringComparison.CurrentCultureIgnoreCase)))
                {
                    throw new Exception($"Given filter, {dw.FileFilter}, already exists for {dw.Directory}");
                }
            }

            dw.InitializeWatcher();
            this._dirWatchers.Add(dw.Id, dw);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="n">Name</param>
        /// <param name="dir">Directory being watched</param>
        /// <param name="filter">File Filter to apply</param>
        /// <param name="isrl">Is dir remote/network location</param>
        /// <returns>Directory watcher that was generated and added</returns>
        public virtual IDirectoryWatcher Add(int id, string n, string dir, string filter, bool isrl, bool asyncEvents = true)
        {
            var dw = new DirectoryWatcher(id, n, dir, filter, isrl, asyncEvents);
            Add(dw);

            return dw;
        }

        public virtual async Task CheckForExistingFiles()
        {
            var tasks = new List<Task>();
            foreach(var watcher in this)
            {
                tasks.Add(Task.Factory.StartNew(() => watcher.CheckForExistingFiles()));
            }

            await Task.WhenAll(tasks);
        }

        public IEnumerator<IDirectoryWatcher> GetEnumerator()
        {
            foreach (var dw in this._dirWatchers.Values)
            {
                yield return dw;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var dw in this)
                    {
                        if (dw.IsEnabled)
                        {
                            dw.IsEnabled = false;
                        }

                        dw.Dispose();
                    }

                    this._dirWatchers.Clear();
                    this._dirWatchers = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }


        #endregion

    }
}
