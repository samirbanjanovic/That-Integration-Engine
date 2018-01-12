using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThatIntegrationEngine.Core.Components
{
    [Serializable]
    public class HandlerEventArgs
        : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tid">Trigger (Handler) Id</param>
        /// <param name="tn">Trigger (Handler) Name</param>        
        public HandlerEventArgs(int tid, string tn)
        {
            TriggerId = tid;
            TriggerName = tn;

            DateTimeOfEvent = DateTime.Now;
        }

        public int TriggerId { get; private set; }

        public string TriggerName { get; private set; }

        public DateTime DateTimeOfEvent { get; private set; }
    }
}
