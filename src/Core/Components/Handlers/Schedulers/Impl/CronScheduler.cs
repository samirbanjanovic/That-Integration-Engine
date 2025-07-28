using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ThatIntegrationEngine.Core.Components
{
    public class CronScheduler
        : Handler
        , ICronScheduler
    {
        public sealed class CronSchedulerQuartzJob
            : IJob
        {
            public void Execute(IJobExecutionContext context)
            {
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                var cronSched = (CronScheduler)dataMap["csInstance"];

                var x = new CronExpression(cronSched.CronExpression);
                
                var args = new CronSchedulerEventArgs(new CronExpression(cronSched.CronExpression), cronSched.Id, cronSched.Name);

                // This is where we raise the event to activate our process
                if(cronSched.AsyncEvents)
                {
                    cronSched.OnTimedActionAsync(cronSched, args);
                }
                else
                {
                    cronSched.OnTimedAction(cronSched, args);
                }                
            }
        }

        public event EventHandler<CronSchedulerEventArgs> TimedActionAsync;

        public event EventHandler<CronSchedulerEventArgs> TimedAction;

        public CronScheduler(int id, string n, string cronExpression, bool asyncEvents)
            : base(id, n, asyncEvents)
        {
            CronExpression = cronExpression;
        }

        public string CronExpression { get; private set; }


        // Use beginInvoke so we utilize a ThreadPool instead of occupying
        // a scheduler worker thread; this will ensure we don't starve
        // the scheduler and delay triggering of other timers
        protected virtual void OnTimedActionAsync(object s, CronSchedulerEventArgs e)
        {
            EventHandler<CronSchedulerEventArgs> handler = TimedActionAsync;
            handler?.BeginInvoke(this, e, cs =>
            {
                var delg = (EventHandler<CronSchedulerEventArgs>)cs.AsyncState;
                delg.EndInvoke(cs);
            },
            handler);
        }

        protected virtual void OnTimedAction(object s, CronSchedulerEventArgs e)
        {
            EventHandler<CronSchedulerEventArgs> handler = TimedActionAsync;
            handler?.Invoke(this, e);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            TimedActionAsync = null;
            TimedAction = null;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
