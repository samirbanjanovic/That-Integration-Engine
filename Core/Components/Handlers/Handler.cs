using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThatIntegrationEngine.Core.Components
{
    public class Handler
        : IHandler
    {
        public Handler(int id, string n, bool asyncEvents)
        {
            Id = id;
            Name = n;
            AsyncEvents = asyncEvents;
        }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public bool AsyncEvents { get; private set; }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
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
