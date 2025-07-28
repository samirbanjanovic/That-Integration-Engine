using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ThatIntegrationEngine.Core.Components;

namespace TIE.Export
{
    public class XmlExport
        : TieProcess<SchedulerArguments>
    {
        public XmlExport(SchedulerArguments args) : base(args)
        {
        }

        public override IExecuteResults Execute()
        {
            var xdoc = new XDocument(
                            new XElement("XPort",
                                new XElement("PName", Arguments.ProcessName),
                                new XElement("TransId", Arguments.TransactionId)));

            var content = xdoc.ToString();

            File.WriteAllText($"u:\\drops\\test\\tie\\export_{DateTime.Now.ToFileTime()}{this.GetHashCode()}.xml", content);

            return new ExecuteResults(true, ExecutionState.Success, Arguments.TransactionId);
        }
    }
}