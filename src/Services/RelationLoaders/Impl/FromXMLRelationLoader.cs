using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ThatIntegrationEngine.Core.Components;

namespace ThatIntegrationEngine
{
    public sealed class FromXMLRelationLoader
        : IRelationLoader
    {
        private string _handlersPath;
        private string _processDetailsPath;
        private string _relationsPath;

        public FromXMLRelationLoader(string handlersPath, string processDetailsPath, string relationsPath)
        {
            this._handlersPath = handlersPath;
            this._processDetailsPath = processDetailsPath;
            this._relationsPath = relationsPath;
        }

        private IEnumerable<XElement> _xhandlers;
        public IEnumerable<XElement> XHandlers
        {
            get
            {  
                return this._xhandlers ?? (this._xhandlers = XDocument.Load(this._handlersPath).Root.Elements("Handler"));
            }
        }

        public ICronSchedulerCollection LoadAllCronSchedulers()
        {
            var xschedulers = this.XHandlers
                                   .Where(x => x.Elements("H_Type").First().Value == "Scheduler");

            var schedCollection = new CronSchedulerCollection(true);
            if (xschedulers.Any())
            {                
                foreach (var xs in xschedulers)
                {
                    int id = (int)xs.Element("Id");
                    var n = (string)xs.Element("Name");
                    var ae = bool.Parse((string)xs.Element("AsyncEvents"));
                    var cs = (string)xs.Element("SchedulerDetails").Element("CronExpression").Value;
                                        
                    schedCollection.Add(id, n, cs, ae);
                }
            }

            return schedCollection;
        }

        public IDirectoryWatcherCollection LoadAllDirectoryWatchers()
        {
            var xschedulers = this.XHandlers
                                  .Where(x => x.Elements("H_Type").First().Value == "Watcher");


            var watchCollcetion = new DirectoryWatcherCollection();
            foreach (var xs in xschedulers)
            {
                int id = (int)xs.Element("Id");
                var n = (string)xs.Element("Name");
                var ae = bool.Parse((string)xs.Element("AsyncEvents"));
                var dir = (string)xs.Element("WatcherDetails").Element("Directory");
                var filter = (string)xs.Element("WatcherDetails").Element("FileFilter");
                var irl = bool.Parse((string)xs.Element("WatcherDetails").Element("IsRemoteLocation"));

                watchCollcetion.Add(id, n, dir, filter, irl);
            }


            return watchCollcetion;
        }

        public IDictionary<int, IProcessDetails> LoadProcessDetails()
        {
            // generate all IProcessDetails
            var xmlProcDetList = XDocument.Load(this._processDetailsPath)
                               .Root
                               .Elements();

            var pdict = new Dictionary<int, IProcessDetails>();
            foreach (var xpd in xmlProcDetList)
            {
                int id = (int)xpd.Element("Id");
                string name = (string)xpd.Element("Name");

                string assemblyPath = (string)xpd.Element("DotNetObjectDetails").Element("AssemblyPath");
                string objectName = (string)xpd.Element("DotNetObjectDetails").Element("FullyQualifiedObjectName");

                Type procType = LoadTypeFromAssembly(assemblyPath, objectName);

                double weight = double.Parse((string)xpd.Element("Weight"));
                double avgDuration = double.Parse((string)xpd.Element("AverageDurationMS"));

                var pd = new ProcessDetails(id, name, procType, weight, avgDuration);

                pdict.Add(pd.Id, pd);
            }

            return pdict;
        }

        public IDictionary<int, IList<int>> LoadHandlerTaskRelationships()
        { 
            // establish relations
            var relations = XDocument.Load(this._relationsPath)
                                     .Root
                                     .Elements();

            var rdict = new Dictionary<int, IList<int>>();
            foreach(var rel in relations)
            {
                var tid = int.Parse(rel.Attribute("id").Value);
                var pd = new List<int>();
                foreach(var pid in rel.Elements())
                {
                    var processID = int.Parse(pid.Value);
                    pd.Add(processID);
                }
                rdict.Add(tid, pd);
            }

            return rdict;
        }

        private Type LoadTypeFromAssembly(string assemblyPath, string objectName)
        {
            var assembly = Assembly.LoadFrom(assemblyPath);
            var type = assembly.GetType(objectName);

            return type;
        }

        public void Dispose()
        {
            this._xhandlers = null;
        }
    }
}
