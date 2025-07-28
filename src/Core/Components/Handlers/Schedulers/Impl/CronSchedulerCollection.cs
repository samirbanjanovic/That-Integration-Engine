using Quartz;
using Quartz.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ThatIntegrationEngine.Core.Components
{
    public class CronSchedulerCollection
        : ICronSchedulerCollection
    {
        private readonly IDictionary<int, ICronScheduler> _cronSchedulers;

        protected IScheduler _quartzScheduler;

        public CronSchedulerCollection(bool waitAllJobs)
        {
            this._cronSchedulers = new Dictionary<int, ICronScheduler>();
            this._quartzScheduler = new StdSchedulerFactory().GetScheduler();
            WaitAllJobs = waitAllJobs;
        }

        public bool WaitAllJobs { get; private set; }

        public virtual ICronScheduler this[int id]
        {
            get
            {
                return this._cronSchedulers[id];
            }
        }

        public virtual ICronScheduler GetScheduler(int id)
        {
            return this[id];
        }

        public virtual void ActiveSwitch(bool on)
        {
            if (on && !this._quartzScheduler.IsStarted)
            {
                this._quartzScheduler.Start();
            }

            if (!on && !this._quartzScheduler.InStandbyMode)
            {
                this._quartzScheduler.Standby();
            }
        }


        public virtual void Add(ICronScheduler cs)
        {
            string jobName = "quartzJob_" + cs.Id;

            IJobDetail jobDetail = JobBuilder
                            .Create<CronScheduler.CronSchedulerQuartzJob>()
                            .WithIdentity(jobName)
                            .Build();

            jobDetail.JobDataMap["csInstance"] = cs;

            // using CRON string stored in database create the trigger
            ITrigger trigger = TriggerBuilder.Create()
                                             .WithIdentity(cs.Id.ToString())
                                             .WithCronSchedule(cs.CronExpression)
                                             .Build();
          
            this._cronSchedulers.Add(cs.Id, cs);
            this._quartzScheduler.ScheduleJob(jobDetail, trigger);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="n">Name</param>
        /// <param name="nid">Notifier Id</param>
        /// <param name="nn">Notifier Name</param>
        /// <param name="cronExpression">Cron expression for scheduling</param>
        /// <returns>Created instance of CronScheduler</returns>
        public virtual ICronScheduler Add(int id, string n, string cronExpression, bool asyncEvents = true)
        {
            var cs = new CronScheduler(id, n, cronExpression, asyncEvents);
            Add(cs);

            return cs;
        }

        public virtual IEnumerator<ICronScheduler> GetEnumerator()
        {
            foreach (var cs in this._cronSchedulers.Values)
            {
                yield return cs;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this._quartzScheduler != null && !this._quartzScheduler.IsShutdown)
                    {
                        this._quartzScheduler.Shutdown(this.WaitAllJobs);
                        this._quartzScheduler.Clear();
                    }

                    if (this._cronSchedulers != null)
                    {
                        foreach (var sch in this)
                        {
                            sch.Dispose();
                        }

                        this._quartzScheduler.Clear();
                    }
                }


                disposedValue = true;
            }
        }

        public void Dispose()
        {

            Dispose(true);

        }
        #endregion
    }
}
