using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ThatIntegrationEngine.Core.Components
{
    [Serializable]
    public class Arguments
        : IArguments
    {
        internal Arguments() { }

        /// <summary>
        /// Process arguments
        /// </summary>
        /// <param name="nid">Notifier Id</param>
        /// <param name="nn">Notifier Name</param>
        /// <param name="pid">Process Id</param>
        /// <param name="pn">Process Name</param>
        /// <param name="toe">Time Of Event</param>
        /// <param name="tid">Trigger Id</param>
        /// <param name="tn">Trigger Name</param>
        /// <param name="trid">Transaction Id</param>
        public Arguments(int pid, string pn, DateTime toe, int tid, string tn, Guid trid)
        {
            ProcessId = pid;
            ProcessName = pn;
            TimeOfEvent = toe;
            TriggerId = tid;
            TriggerName = tn;
            TransactionId = trid;
        }

        protected Arguments(SerializationInfo info, StreamingContext context)
        {
            ProcessId = info.GetInt32(nameof(ProcessId));
            ProcessName = info.GetString(nameof(ProcessName));
            TimeOfEvent = info.GetDateTime(nameof(TimeOfEvent));
            TriggerId = info.GetInt32(nameof(TriggerId));
            TriggerName = info.GetString(nameof(TriggerName));
            TransactionId = Guid.Parse(info.GetString(nameof(TransactionId)));
        }

        public int ProcessId { get; set; }

        public string ProcessName { get; set; }

        public DateTime TimeOfEvent { get; set; }

        public Guid TransactionId { get; set; }

        public int TriggerId { get; set; }

        public string TriggerName { get; set; }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ProcessId), ProcessId);
            info.AddValue(nameof(ProcessName), ProcessName);
            info.AddValue(nameof(TimeOfEvent), TimeOfEvent);
            info.AddValue(nameof(TransactionId), TransactionId);
            info.AddValue(nameof(TriggerId), TriggerId);
            info.AddValue(nameof(TriggerName), TriggerName);
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
