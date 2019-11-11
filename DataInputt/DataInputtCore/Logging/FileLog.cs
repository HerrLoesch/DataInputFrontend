using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace DataInputt.Logging
{
    internal class FileLog : ILog, IDisposable
    {
        private readonly FileStream fs;
        private readonly StreamWriter sw;
        private readonly EventWaitHandle doLogging = new AutoResetEvent(false);
        private readonly Thread loggingThread;
        private readonly ConcurrentQueue<FileLogData> messageQueue;
        private readonly EventWaitHandle terminateEvent = new ManualResetEvent(false);
        private readonly bool useUtcTimestamps;
        private bool isDisposed;

        public FileLog(string filename, NLog.LogLevel severity, bool utc, bool asyncLogging)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("Given filename is invalid", nameof(filename));
            }

            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            // Create logging target
            fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
            sw = new StreamWriter(fs);

            if (asyncLogging)
            {
                loggingThread = new Thread(WorkerThreadMethod);
                messageQueue = new ConcurrentQueue<FileLogData>();
                loggingThread.Start();
            }

            // Configuration
            useUtcTimestamps = utc;
            Severity = severity;
        }

        public void Write(ILogData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!(data is FileLogData fileLoggingData))
            {
                throw new InvalidOperationException("Given data is not compatible with this logger");
            }

            fileLoggingData.Timestamp = useUtcTimestamps ? DateTime.UtcNow : DateTime.Now;

            if (this.loggingThread != null)
            {
                // Asynchronous logging
                messageQueue.Enqueue(fileLoggingData);
                doLogging.Set();
            }
            else
            {
                sw.WriteLine($"{DateTime.Now}: {data.Message}");
                sw.Flush();
            }
        }

        public void Configure(string configFile)
        {
            throw new NotImplementedException();
        }

        public NLog.LogLevel Severity { get; set; }

        private void WorkerThreadMethod()
        {
            WaitHandle[] events = {terminateEvent, doLogging};
            while (true)
            {
                var result = WaitHandle.WaitAny(events);
                switch (result)
                {
                    case 0: // terminateEvent
                        return;

                    case 1: // doLogging
                        while (messageQueue.TryDequeue(out var data))
                        {
                            sw.WriteLine($"{data.Timestamp}: {data.Message}");
                            sw.Flush();
                        }

                        break;
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed == false)
            {
                if (disposing)
                {
                    this.terminateEvent.Set();
                    this.loggingThread.Join(5000);
                    this.sw.Close();
                    this.fs.Close();
                    this.terminateEvent.Close();
                    this.doLogging.Close();
                }

                this.isDisposed = true;
            }
        }
    }
}