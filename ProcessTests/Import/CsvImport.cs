using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThatIntegrationEngine.Core.Components;

namespace TIE.Import
{
    public class CsvImport : TieProcess<WatcherArguments>
    {
        public CsvImport(WatcherArguments args) : base(args)
        {
        }


        public override IExecuteResults Execute()
        {

            var content = $"{Arguments.File.ReadAllText()}\r\n" +
                          $"{Arguments.ProcessName},{Arguments.TransactionId}" + "\r\n";

            File.WriteAllText("u:\\drops\\test\\tie\\import_" + 
                              $"{DateTime.Now.ToFileTime()}{this.GetHashCode()}.csv", content);

            return new ExecuteResults(false, ExecutionState.Success, Arguments.TransactionId);      
        }
    }
}
