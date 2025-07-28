using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThatIntegrationEngine.Core.Components
{ 
    public class ProcessDetails
        : IProcessDetails
    {
        public ProcessDetails(int id, string nm, Type pt, double si, double axt)
        {
            Id = id;
            Name = nm;
            ProcessType = pt;
            SystemImpact = si;
            AverageExecutionTime = axt;
        }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public Type ProcessType { get; private set; }

        public double SystemImpact { get; private set; }

        public double AverageExecutionTime { get; private set; }
    }
}
