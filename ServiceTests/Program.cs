
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using ThatIntegrationEngine;
using ThatIntegrationEngine.Core.Components;
using ThatIntegrationEngine.Core.Components.Adapters;
using ThatLogger;

namespace TIETester
{
    class Program
    {
        static void Main(string[] args)
        {
            var hPath = @"Y:\src\ThatIntegrationEngine\Core\ExampleHandlers.xml";
            var pdPath = @"Y:\src\ThatIntegrationEngine\Core\ExampleProcessDetailRelations.xml";
            var relPath = @"Y:\src\ThatIntegrationEngine\Core\ExampleTriggerProcessRelations.xml";

            var loader = new FromXMLRelationLoader(hPath, pdPath, relPath);

            var engine = new Engine(loader);
            engine.Start();
            Console.ReadLine();
        }
    }
}
