using System;
using System.Collections.Generic;

namespace ThatIntegrationEngine.Core.Components
{
    public interface ICronSchedulerCollection
        : IEnumerable<ICronScheduler>
        , IDisposable
    {
        ICronScheduler this[int id] { get; }

        void Add(ICronScheduler cs);

        ICronScheduler Add(int id, string n, string cronExpression, bool asyncEvents = true);

        ICronScheduler GetScheduler(int id);

        void ActiveSwitch(bool on);
    }
}