using System;

namespace ThatIntegrationEngine.Core.Components
{
    public interface ICronScheduler 
        : IHandler
    {
        string CronExpression { get; }

        event EventHandler<CronSchedulerEventArgs> TimedActionAsync;

        event EventHandler<CronSchedulerEventArgs> TimedAction;

    }
}