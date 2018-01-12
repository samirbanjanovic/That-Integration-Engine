using System;
using System.Runtime.Serialization;

namespace ThatIntegrationEngine.Core.Components
{
    public interface IArguments
        : ISerializable
    {
        int TriggerId { get; set; }

        string TriggerName { get; set; }

        int ProcessId { get; set; }

        string ProcessName { get; set; }

        DateTime TimeOfEvent { get; set; }

        Guid TransactionId { get; set; }
    }
}
