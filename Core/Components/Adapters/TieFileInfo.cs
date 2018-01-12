using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThatIntegrationEngine.Core.Components.Adapters
{
    public class TieFileInfo
    {
        public const double RETRY_DURATION = 600;

        private readonly FileInfo _fileInfo;

        public TieFileInfo(string fileName)
        {
            this._fileInfo = new FileInfo(fileName);
        }

        public virtual byte[] ReadAllBytes(double retryDurationInS = RETRY_DURATION)
        {
            return FileReader.ReadAllBytes(this._fileInfo.FullName, retryDurationInS);
        }

        public virtual string ReadAllText(double retryDurationInS = RETRY_DURATION)
        {
            return FileReader.ReadAllText(this._fileInfo.FullName, retryDurationInS);
        }

        public virtual string[] ReadAllLines(double retryDurationInS = RETRY_DURATION)
        {
            return FileReader.ReadAllLines(this._fileInfo.FullName, retryDurationInS);
        }

        public string FullName { get { return this._fileInfo.FullName; } }

        public string Name { get { return this._fileInfo.Name; } }

        public long Length { get { return this._fileInfo.Length; } }

        public DateTime CreationTime { get { return this._fileInfo.CreationTime; } }

        public DirectoryInfo Directory { get { return this._fileInfo.Directory; } }

        public string DirectoryName { get { return this._fileInfo.DirectoryName; } }

        public bool Exists { get { return this._fileInfo.Exists; } }

        public string Extension { get { return this._fileInfo.Extension; } }

        public void Refresh()
        {
            this._fileInfo.Refresh();
        }
    }
}
