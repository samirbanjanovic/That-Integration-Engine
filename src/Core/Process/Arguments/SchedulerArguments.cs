using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using System.Xml.Serialization;

namespace ThatIntegrationEngine.Core.Components
{
    [Serializable]
    public class SchedulerArguments
        : Arguments
    {
        internal SchedulerArguments() { }

        /// <summary>
        /// Arguments passed by a scheduler
        /// </summary>
        /// <param name="ce">Cron expression details</param>
        /// <param name="nid">Notifier Id</param>
        /// <param name="nn">Notifier Name</param>
        /// <param name="pid">Process Id</param>
        /// <param name="pn">Process Name</param>
        /// <param name="toe">Time Of Event</param>
        /// <param name="tid">Trigger Id</param>
        /// <param name="tn">Trigger Name</param>
        /// <param name="trid">Transaction Id</param>
        public SchedulerArguments(CronExpression ce,
            int pid, string pn, DateTime toe, int tid, string tn, Guid trid)
            : base(pid, pn, toe, tid, tn, trid)
        {
            CronString = ce.CronExpressionString;
            CronExpression = ce;
        }

        protected SchedulerArguments(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            CronString = info.GetString(nameof(CronString));
            CronExpression = new CronExpression(CronString);
        }
        
        public string CronString { get; set; }
        
        [NonSerialized]
        private CronExpression _cronExpression;
    
        [XmlIgnore]
        public CronExpression CronExpression
        {
            get { return this._cronExpression; }
            set { this._cronExpression = value; }
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);            
            info.AddValue(nameof(CronString), CronString);            
        }
    }
}
