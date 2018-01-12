using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThatIntegrationEngine.Core.Components
{
    [Serializable]
    public class CronSchedulerEventArgs
        : HandlerEventArgs
    {
        public CronSchedulerEventArgs(CronExpression ce, int tid, string tn)
            : base(tid, tn)
        {
            QuartzCronExpression = ce;
        }

        public CronExpression QuartzCronExpression { get; private set; }
    }
}
