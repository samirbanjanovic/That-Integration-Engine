using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Linq;

namespace ThatIntegrationEngine.Core.Components
{
    [Serializable]
    public class ExecuteResults 
        : IExecuteResults

    {
        public ExecuteResults(bool didCompleteWork, ExecutionState state, Guid transactionId)
        {
            Errors = new List<string>();
            FileNames = new List<string>();

            DidWorkComplete = didCompleteWork;
            State = state;
            TransactionId = transactionId;
        }

        protected ExecuteResults(SerializationInfo info, StreamingContext context)
        {
            DidWorkComplete = info.GetBoolean(nameof(DidWorkComplete));
            
            Errors = info.GetString(nameof(Errors)).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            FileNames = info.GetString(nameof(FileNames)).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            State = (ExecutionState)Enum.Parse(typeof(ExecutionState), info.GetString(nameof(State)));
            TransactionId = new Guid(info.GetString(nameof(TransactionId)));
        }

        public bool DidWorkComplete { get; private set; }

        public IList<string> Errors { get; private set;  }

        public IList<string> FileNames { get; private set; }

        public ExecutionState State { get; private set; }

        public Guid TransactionId { get; private set; }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var err = string.Join(Environment.NewLine, Errors);
            var fns = string.Join(Environment.NewLine, FileNames);

            info.AddValue(nameof(DidWorkComplete), DidWorkComplete);
            info.AddValue(nameof(Errors), err);
            info.AddValue(nameof(FileNames), fns);
            info.AddValue(nameof(State), State.ToString());
            info.AddValue(nameof(TransactionId), TransactionId.ToString());
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            GetObjectData(info, context);
        }
    }
}