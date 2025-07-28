using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThatIntegrationEngine.Core.Components;

namespace ThatIntegrationEngine
{ 
    public sealed class ADORelationLoader
        : IRelationLoader
    {
        public ADORelationLoader(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; private set; }


        public ICronSchedulerCollection LoadAllCronSchedulers()
        {
            throw new NotImplementedException();
        }

        public IDirectoryWatcherCollection LoadAllDirectoryWatchers()
        {
            throw new NotImplementedException();
        }

        public IDictionary<int, IProcessDetails> LoadProcessDetails()
        {
            throw new NotImplementedException();
        }

        public IDictionary<int, IList<int>> LoadHandlerTaskRelationships()
        {
            throw new NotImplementedException();
        }


        public void Dispose()
        {
            ConnectionString = null;
        }        
    }
}
