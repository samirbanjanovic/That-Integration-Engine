using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ThatIntegrationEngine.Core.Components
{
    public interface IExecuteResults
        : ISerializable
    {
        bool DidWorkComplete { get; }

        IList<string> Errors { get; }

        IList<string> FileNames { get; }

        ExecutionState State { get; }

        Guid TransactionId { get; }
    }
}