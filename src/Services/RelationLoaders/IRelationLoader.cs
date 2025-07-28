using System;
using System.Collections.Generic;
using ThatIntegrationEngine.Core.Components;

namespace ThatIntegrationEngine
{
    public interface IRelationLoader
        : IDisposable
    {
        IDirectoryWatcherCollection LoadAllDirectoryWatchers();

        ICronSchedulerCollection LoadAllCronSchedulers();

        IDictionary<int, IProcessDetails> LoadProcessDetails();      

        IDictionary<int, IList<int>> LoadHandlerTaskRelationships();        
    }
}
