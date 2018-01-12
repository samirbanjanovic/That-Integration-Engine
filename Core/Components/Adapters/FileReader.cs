using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace ThatIntegrationEngine.Core.Components.Adapters
{
    /// <summary>
    /// Static class for reading a file.  
    /// </summary>
    public static class FileReader
    {
        public static byte[] ReadAllBytes(string filePath, double retryTime = 480)
        {
            if(retryTime <= 0)
                retryTime = 120;

            if(string.IsNullOrEmpty(filePath?.Trim()))
            {// verify that full path isn't null or empty                                    
                throw new ArgumentException(nameof(filePath));
            }

            var sw = new Stopwatch();
            byte[] buffer = null;

            sw.Start();
            while (sw.Elapsed.TotalSeconds < retryTime)
            {
                try
                {
                    buffer = System.IO.File.ReadAllBytes(filePath); // get file content
                    break;
                }
                catch(IOException)
                {// only handle IOException; let all other exceptions bubble up
                    System.Threading.Thread.Sleep(1000);
                }
            }

            sw.Stop();
            sw = null;

            return buffer;
        }

        public static string ReadAllText(string filePath, double retryTime = 480)
        {
            var utf8 = new UTF8Encoding(false);
            return utf8.GetString(FileReader.ReadAllBytes(filePath, retryTime));
        }

        public static string[] ReadAllLines(string filePath, double retryTime = 480)
        {
            if(retryTime <= 0)
                retryTime = 120;

            if(string.IsNullOrEmpty(filePath?.Trim()))
            {// verify that full path isn't null or empty                                    
                throw new ArgumentException(nameof(filePath));
            }

            var sw = new Stopwatch();
            string[] buffer = null;

            sw.Start();
            while(sw.Elapsed.TotalSeconds < retryTime)
            {
                try
                {
                    buffer = System.IO.File.ReadAllLines(filePath); // get file content
                    break;
                }
                catch(IOException)
                {// only handle IOException; let all other exceptions bubble up
                    System.Threading.Thread.Sleep(1000);
                }
            }

            sw.Stop();
            sw = null;

            return buffer;
        }
    }
}
