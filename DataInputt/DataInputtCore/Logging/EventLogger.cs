using System;
using System.Diagnostics;

namespace DataInputt.Logging
{
    internal class EventLogger : ILog
    {
        private readonly string eventSource;

        public NLog.LogLevel Severity { get; set; }

        public EventLogger(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentException("No event source specified", nameof(source));
            }

            eventSource = source;
        }

        public void Write(ILogData data)
        {
            if (!(data is EventLoggingData eventLoggingData))
            {
                throw new ArgumentException(nameof(data));
            }

            if (EventLog.SourceExists(this.eventSource))
            {
                EventLog.CreateEventSource(new EventSourceCreationData(this.eventSource, "Application"));
            }

            EventLogEntryType foo = EventLogEntryType.Information;
            if (data.Severity == NLog.LogLevel.Debug || data.Severity == NLog.LogLevel.Info || data.Severity == NLog.LogLevel.Trace)
            {
                foo = EventLogEntryType.Information;
            }
            else if (data.Severity == NLog.LogLevel.Warn)
            {
                foo = EventLogEntryType.Warning;
            }
            else if (data.Severity == NLog.LogLevel.Error || data.Severity == NLog.LogLevel.Fatal)
            {
                foo = EventLogEntryType.Error;
            }

            EventLog.WriteEntry(this.eventSource, eventLoggingData.Message, foo, eventLoggingData.EventId);

        }

        public void Configure(string configFile)
        {
            throw new NotImplementedException();
        }
    }
}