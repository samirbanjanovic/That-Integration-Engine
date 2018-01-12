using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThatIntegrationEngine.Core.Components;
using ThatLogger;
using ThatIntegrationEngine.SettingsElements;
using ThatIntegrationEngine.Core.Components.Adapters;

namespace ThatIntegrationEngine
{
    public sealed class Engine
        : IDisposable
    {
        /// <summary>
        /// 1-to-1 Mapping of Trigger (CronScheduler or DirectoryWatcher)
        /// to IProcess. A process can be related to multiple triggers; put a 
        /// trigger can only be associated with one action (Process)
        /// 
        /// int - trigger id
        /// type - type of process
        /// </summary>
        /// 
        private IDictionary<int, IList<int>> _processHandlerRelation;

        private IDictionary<int, IProcessDetails> _processDetails;

        private ICronSchedulerCollection _cronSchedulers;

        private IDirectoryWatcherCollection _directoryWatchers;

        private readonly ConcurrentBag<IProcess> _activeProcesses;

        private readonly ConcurrentQueue<IDirectoryWatcher> _failedWatchers;

        private readonly BackgroundWorker _failedWatchersWorker;



        private bool _hasStarted;

        private string _logSource;

        private readonly ILogger _logger;

        public Engine(IRelationLoader relationLoader)
            : this(relationLoader, null)
        {

        }

        // add second parameter that passes in the logger
        // Use ThatLogger and pass in interface ILogger
        public Engine(IRelationLoader relationLoader, ILogger logger)
        {
            this._logSource = Environment.MachineName;
            this._logger = logger ?? LoggerFactory.Current.GetFileLogger("tie.log");

            this._activeProcesses = new ConcurrentBag<IProcess>();

            this._failedWatchers = new ConcurrentQueue<IDirectoryWatcher>();

            this._hasStarted = false;

            this._failedWatchersWorker = new BackgroundWorker();
            this._failedWatchersWorker.DoWork += ProcessFailedWatchersQueue;

            InstanceId = Guid.NewGuid();

            LoadRelationMappings(relationLoader);
            SubscribeToHandlerEvents();
        }

        public Guid InstanceId { get; private set; }

        public void Start()
        {
            bool success = InternalPowerSwitch(true);

            if (success)
            {
                Task.Run(async () => await this._directoryWatchers.CheckForExistingFiles());
            }
        }

        public void Stop()
        {
            InternalPowerSwitch(false);
        }

        public void Restart(IRelationLoader relationLoader)
        {
            try
            {
                Stop();
                ClearResources();
                LoadRelationMappings(relationLoader);
                Start();
            }
            catch
            {
                // log exception 
                // dispose of resources
                // shut down
                Dispose();
                throw;
            }
        }

        private bool InternalPowerSwitch(bool onOff)
        {
            try
            {
                this._directoryWatchers?.ActiveSwitch(onOff);
                this._cronSchedulers?.ActiveSwitch(onOff);

                this._hasStarted = onOff;

                return true;
            }
            catch (Exception e)
            {
                // log exception 
                // dispose of resources
                // shut down
                Dispose();
                throw;
            }
        }

        private void SubscribeToHandlerEvents()
        {
            if (this._directoryWatchers != null)
            {
                // subscribe watchers to handlers
                foreach (var watcher in this._directoryWatchers)
                {
                    watcher.FileArrivedAsync += HandleFileArrivalAsync;
                    watcher.ErrorAsync += HandleWatcherErrorAsync;
                }
            }

            if (this._cronSchedulers != null)
            {
                foreach (var scheduler in this._cronSchedulers)
                {
                    scheduler.TimedActionAsync += HandleScheduledActionAsync;
                }
            }
        }

        private void HandleScheduledActionAsync(object sender, CronSchedulerEventArgs csarg)
        {
            var scheduler = sender as ICronScheduler;
            if (scheduler != null)
            {
                // load process type based on trigger (sender) id
                // dynamically create instance of the process and pass 
                // all expected information
                BeginProcessExecution(csarg);
            }
        }

        private void HandleFileArrivalAsync(object sender, DirWatcherEventArgs dwargs)
        {
            var watcher = sender as IDirectoryWatcher;
            if (watcher != null)
            {
                // load process type based on trigger (sender) id
                // dynamically create instance of the process and pass 
                // all expected information
                BeginProcessExecution(dwargs);
            }
        }

        private static readonly object _workerPad = new object();
        private void HandleWatcherErrorAsync(object sender, ErrorEventArgs e)
        {
            var watcher = sender as IDirectoryWatcher;
            if (watcher != null)
            {
                // log the error and enqueue for re-init
                this._failedWatchers.Enqueue(watcher);

                lock (_workerPad)
                {
                    if (!this._failedWatchersWorker.IsBusy)
                    {
                        _failedWatchersWorker.RunWorkerAsync();
                    }
                }
            }
        }

        private void ProcessFailedWatchersQueue(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            IDirectoryWatcher watcher = null;
            bool enqueueWatcher = true;

            do
            {
                if (this._failedWatchers.TryDequeue(out watcher))
                {
                    try
                    {
                        var errArgs = watcher.CheckMonitoredPath().Result;
                        if (errArgs == null)
                        {// no errors detected
                            watcher.InitializeWatcher();

                            watcher.FileArrivedAsync += HandleFileArrivalAsync;
                            watcher.ErrorAsync += HandleWatcherErrorAsync;

                            watcher.IsEnabled = true;

                            watcher.CheckForExistingFiles();

                            enqueueWatcher = false;
                        }
                    }
                    catch
                    {
                        // log error                        
                    }
                    finally
                    {
                        if (enqueueWatcher)
                        {
                            this._failedWatchers.Enqueue(watcher);
                        }
                        else
                        {
                            // log re init
                        }
                    }
                }
            } while (watcher != null);

        }

        private void BeginProcessExecution(HandlerEventArgs hargs)
        {
            IList<int> processIdList = null;

            if (this._processHandlerRelation.TryGetValue(hargs.TriggerId, out processIdList))
            {
                if (processIdList.Count > 1)
                {
                    try
                    {
                        int pi = 0;
                        var plist = new Task[processIdList.Count];
                        foreach (var pid in processIdList)
                        {
                            plist[pi++] = Task.Factory.StartNew(() => ExecuteProcess(this._processDetails[pid], hargs), TaskCreationOptions.LongRunning);
                        }

                        Task.WaitAll(plist);
                    }
                    catch (Exception e)
                    {
                        // log exception
                    }
                }
                else
                {// avoid parallelization if only one process is expected 
                 // to be executed
                 // execute on calling thread
                    var pd = this._processDetails[processIdList[0]];
                    ExecuteProcess(pd, hargs);
                }
            }
        }

        private void ExecuteProcess(IProcessDetails pd, HandlerEventArgs hargs)
        {
            IProcess process = null;
            IExecuteResults execRes = null;
            var transId = Guid.NewGuid();
            Stopwatch durWatch = null;
            long? duration = null;

            bool hasDuration = false;

            try
            {
                IArguments pargs = null;

                // if there are any constructors with 1
                var ctors = pd.ProcessType.GetConstructors()
                                           .Where(c => c.GetParameters().Count() == 1);

                if (ctors.Any())
                {
                    if (hargs is DirWatcherEventArgs)
                    {
                        var wa = (DirWatcherEventArgs)hargs;
                        pargs = new WatcherArguments(new TieFileInfo(wa.FullePath), pd.Id, pd.Name, DateTime.Now,
                                                     hargs.TriggerId, hargs.TriggerName, transId);

                        process = (TieProcess<WatcherArguments>)Activator.CreateInstance(pd.ProcessType, pargs);
                    }
                    else
                    {
                        var sa = (CronSchedulerEventArgs)hargs;
                        pargs = new SchedulerArguments(sa.QuartzCronExpression, pd.Id, pd.Name, DateTime.Now,
                                                       hargs.TriggerId, hargs.TriggerName, transId);


                        process = (TieProcess<SchedulerArguments>)Activator.CreateInstance(pd.ProcessType, pargs);
                    }
                }
                else
                {
                    // check if default constructor is present
                    var dctor = pd.ProcessType.GetConstructor(Type.EmptyTypes);

                    if (dctor != null)
                    {
                        process = (IProcess)Activator.CreateInstance(pd.ProcessType);
                    }
                }

                if (process != null)
                {
                    this._activeProcesses.Add(process);

                    durWatch = Stopwatch.StartNew();

                    execRes = process.Execute();

                    duration = durWatch.ElapsedMilliseconds;
                    durWatch.Stop();
                    
                    hasDuration = true;
                }
            }
            catch (Exception pex)
            {
                // log error
                if (execRes == null)
                {
                    execRes = new ExecuteResults(false, ExecutionState.Failed, transId);
                }

                execRes.Errors.Add(pex.ToString());
            }
            finally
            {
                if (!hasDuration)
                {
                    duration = durWatch?.ElapsedMilliseconds;
                }


                if (execRes == null)
                {// here for backwards compatibility
                    execRes = new ExecuteResults(true, ExecutionState.ProcessCompatExec, transId);
                }

                ProcessExecutionResults(execRes, pd);

                if (process != null)
                {
                    this._activeProcesses.TryTake(out process);
                    this._logger.LogMessage(new StatusMessage("tie", $"{this._activeProcesses.Count} active processes"), process.GetHashCode().ToString());

                    process.Dispose();
                    process = null;
                }
                else
                {
                    // log that process was null and no work was performed
                    // give hargs details
                }
            }
        }

        private void ProcessExecutionResults(IExecuteResults results, IProcessDetails pd)
        {
            // perform checks on process 
            // execution state
            switch (results.State)
            {
                case ExecutionState.Success:
                    // check if files were generated 
                    // and log?
                    break;
                case ExecutionState.SuccessWithErrors:
                    // log all errors
                    // check for file names
                    break;
                case ExecutionState.FailedCanRetry:
                    // log all errors;
                    // add to retry queue
                    break;
                case ExecutionState.Failed:
                    // log all errors
                    // treat as fatal error
                    // check if any files were generated before failure
                    break;
                case ExecutionState.ProcessCompatExec:
                    // log that the process was 
                    // executed using compatibility 
                    // from previous version
                    break;
            }
        }

        private void LoadRelationMappings(IRelationLoader relationLoader)
        {
            this._cronSchedulers = relationLoader.LoadAllCronSchedulers();
            this._directoryWatchers = relationLoader.LoadAllDirectoryWatchers();
            this._processDetails = relationLoader.LoadProcessDetails();
            this._processHandlerRelation = relationLoader.LoadHandlerTaskRelationships();
           
        }

        private void LoadSettingsFromAppConfig()
        {
            var configFile = (EngineSettingsSections)ConfigurationManager.GetSection("EngineSettings");

        }

        private void ClearResources()
        {
            this._directoryWatchers?.Dispose();
            this._cronSchedulers?.Dispose();
            this._processHandlerRelation?.Clear();
            this._processHandlerRelation = null;
        }

        public void Dispose()
        {
            ClearResources();
        }
    }
}
