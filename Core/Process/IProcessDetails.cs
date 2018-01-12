using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThatIntegrationEngine.Core.Components
{
    public interface IProcessDetails
    {
        int Id { get; }

        string Name { get;}

        Type ProcessType { get; }

        double SystemImpact { get; }

        double AverageExecutionTime { get; }
    }
}
