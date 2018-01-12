using System;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Timers;

namespace ThatIntegrationEngine.Core.Components
{
    public class DirectoryWatcher
        : Handler
        , IDirectoryWatcher
    {
        public const double DIR_CHECK_TIMER = 5000;

        public event EventHandler<DirWatcherEventArgs> FileArrivedAsync;

        public event EventHandler<ErrorEventArgs> ErrorAsync;

        public event EventHandler<DirWatcherEventArgs> FileArrived;

        public event EventHandler<ErrorEventArgs> Error;

        protected FileSystemWatcher _fsw;

        protected Timer _dirCheckTimer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="n">Name</param>
        /// <param name="dir">Directory being watched</param>
        /// <param name="filter">File Filter to apply</param>
        /// <param name="isrl">Is dir remote/network location</param>
        public DirectoryWatcher(int id, string n, string dir, string filter, bool isrl, bool asyncEvents = true)
            : base(id, n, asyncEvents)
        {
            Directory = dir;
            FileFilter = filter;
            IsRemoteLocation = isrl;         
        }

        public string Directory { get; private set; }

        public string FileFilter { get; private set; }

        public bool IsRemoteLocation { get; private set; }
        
        public virtual bool IsEnabled
        {
            get
            {
                return this._fsw.EnableRaisingEvents;                
            }
            set
            {
                this._fsw.EnableRaisingEvents = value;
                this._dirCheckTimer.Enabled = value;
                this._dirCheckTimer.AutoReset = true;
            }
        }

        public virtual void CheckForExistingFiles()
        {
            var files = System.IO.Directory.EnumerateFiles(Directory, FileFilter, SearchOption.TopDirectoryOnly).ToList();

            if (files.Count > 0)
            {
                for (int n = 0; n < files.Count; n++)
                {
                    var fn = Path.GetFileName(files[n]);

                    OnCreatedAsync(this, new FileSystemEventArgs(WatcherChangeTypes.All, Directory, fn));
                }
            }
        }

        public virtual void InitializeWatcher()
        {
            // initialize the FileSystemWatcher and configure using received data
            this._fsw = new FileSystemWatcher(Directory, FileFilter);
            this._fsw.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName;

            this._dirCheckTimer = new Timer(DIR_CHECK_TIMER);

            if (AsyncEvents)
            {
                this._fsw.Created += OnCreatedAsync;
                this._fsw.Renamed += OnRenamedAsync;                
            }            
            else
            {
                this._fsw.Created += OnCreated;
                this._fsw.Renamed += OnRenamed;
            }
            
            this._dirCheckTimer.Elapsed += CheckDirectoryAvailability;
        }

        protected virtual void CheckDirectoryAvailability(object sender, ElapsedEventArgs e)
        {
            // needs to be addressed to allow for 
            // synchronous flow
            Task.Run(async () => 
            {
                // disable timer so multiple  
                // checks aren't performed at the same time if the first check takes too long
                this._dirCheckTimer.Enabled = false;

                var res = await CheckMonitoredPath();

                return res;
            }).ContinueWith(res =>
                    {
                        ErrorEventArgs errAargs = res.Result;
                        if (errAargs != null)
                        {
                            IsEnabled = false;

                            if (AsyncEvents)
                            {
                                OnErrorAsync(this, errAargs);
                            }
                            else
                            {
                                OnError(this, errAargs);
                            }
                        }
                        else
                        {//re-enable the timer
                            this._dirCheckTimer.Enabled = true;
                        }
                    });
        }

        protected virtual void OnCreatedAsync(object sender, FileSystemEventArgs args)
        {
            EventHandler<DirWatcherEventArgs> handler = FileArrivedAsync;
            handler?.BeginInvoke(this, new DirWatcherEventArgs(args.FullPath, WatcherChangeTypes.Created, Id, Name)
            , cs =>
                {
                    var delg = (EventHandler<DirWatcherEventArgs>)cs.AsyncState;
                    delg.EndInvoke(cs);
                }
            , handler);
        }

        protected virtual void OnRenamedAsync(object sender, RenamedEventArgs args)
        {
            if (Operators.LikeString(args.Name, FileFilter, CompareMethod.Binary))
            {
                EventHandler<DirWatcherEventArgs> handler = FileArrivedAsync;
                handler?.BeginInvoke(this, new DirWatcherEventArgs(args.FullPath, WatcherChangeTypes.Created, Id, Name)
                , cs =>
                {
                    var delg = (EventHandler<DirWatcherEventArgs>)cs.AsyncState;
                    delg.EndInvoke(cs);
                }
                , handler);
            }
        }

        protected virtual void OnErrorAsync(object sender, ErrorEventArgs args)
        {
            EventHandler<ErrorEventArgs> handler = ErrorAsync;
            handler?.BeginInvoke(this, args
            , cs =>
            {
                var delg = (EventHandler<ErrorEventArgs>)cs.AsyncState;
                delg.EndInvoke(cs);
            }
            , handler);
        }

        protected virtual void OnCreated(object sender, FileSystemEventArgs args)
        {
            EventHandler<DirWatcherEventArgs> handler = FileArrived;
            handler?.Invoke(this, new DirWatcherEventArgs(args.FullPath, WatcherChangeTypes.Created, Id, Name));
        }

        protected virtual void OnRenamed(object sender, RenamedEventArgs args)
        {
            if (Operators.LikeString(args.Name, FileFilter, CompareMethod.Binary))
            {
                OnCreated(sender, args);
            }
        }

        protected virtual void OnError(object sender, ErrorEventArgs args)
        {
            EventHandler<ErrorEventArgs> handler = Error;
            handler?.Invoke(this, args);
        }

        public virtual async Task<ErrorEventArgs> CheckMonitoredPath()
        {// Execution of method begins from a timer (every 5 seconds)
            // executes on a task and raises error if it didn't succeed
            // otherwise it continues to work without issues            
            IPStatus ipStatus = IPStatus.Success;
            ErrorEventArgs errArgs = null;
            bool exists = false;
            Exception pex = null;

            if (IsRemoteLocation)
            {// attempt to ping the server

                PingReply pr = null;
                var host = "";

                using (var ping = new Ping())
                {                    
                    try
                    {
                        pr = await ping.SendPingAsync(host);
                        
                        // add code that accounts for roundtrip time
                        // if time is excessive

                        ipStatus = pr.Status;

                    }
                    catch (Exception x)
                    {
                        pex = x;
                    }
                    finally
                    {
                        if (ipStatus != IPStatus.Success)
                        {
                            if(pex == null)
                            {
                                pex = new Exception($"Lost connection to remote path {host}");                                
                            }

                            errArgs = new ErrorEventArgs(pex);
                        }
                    }
                }
            }

            if (ipStatus != IPStatus.Success)
            {
                return errArgs;
            }

            try
            {
                exists = System.IO.Directory.Exists(Directory);
            }
            catch (Exception dex)
            {
                pex = dex;                
            }
            finally
            {
                if (!exists)
                {
                    if (pex == null)
                    {
                        pex = new Exception($"Directory {Directory} doesn't exist!");
                    }

                    errArgs = new ErrorEventArgs(pex);
                }
            }

            return errArgs;            
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.FileArrivedAsync = null;
            this.ErrorAsync = null;

            this.FileArrived = null;
            this.Error = null;

            this._fsw.EnableRaisingEvents = false;
            this._fsw.Dispose();
            this._fsw = null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
