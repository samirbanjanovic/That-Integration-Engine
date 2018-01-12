using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThatIntegrationEngine.Core.Components;

namespace TIE.Export
{
    public class CsvExport : TieProcess<SchedulerArguments>
    {
        public CsvExport(SchedulerArguments args) : base(args)
        {
        }

        public override IExecuteResults Execute()
        {
            var content = $"{Arguments.ProcessName},{Arguments.TransactionId}" + "\r\n";

            File.WriteAllText("u:\\drops\\test\\tie\\export_" +
                             $"{DateTime.Now.ToFileTime()}{this.GetHashCode()}.csv", content);

            return new ExecuteResults(true, ExecutionState.Success, Arguments.TransactionId);
        }
    }
}
