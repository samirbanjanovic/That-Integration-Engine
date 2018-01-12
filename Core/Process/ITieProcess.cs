using System;

namespace ThatIntegrationEngine.Core.Components
{
    public interface ITieProcess<TArgs> 
        : IProcess
    {
        TArgs Arguments { get; }
    }

    public interface IProcess
        : IDisposable
    {
        IExecuteResults Execute();
    }
}