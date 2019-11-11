using System;
using System.IO;

namespace DataInputt.Logging
{
    internal class CsvLogger: IDisposable
    {
        private readonly FileStream fs;
        private readonly StreamWriter sw;

        public CsvLogger(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("Given filename is invalid", nameof(filename));
            }

            // Create logging target
            fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
            sw = new StreamWriter(fs);
        }

        public void Log(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            sw.WriteLine($"{DateTime.Now}: {message}");
            sw.Flush();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.sw.Close();
            this.fs.Close();
        }
    }
}
