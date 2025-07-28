using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ThatIntegrationEngine.Core.Components
{    
    public interface IHandler
        : IDisposable
    {
        int Id { get; }
        string Name { get; }
        
        bool AsyncEvents { get; }
    }
}
