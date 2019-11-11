namespace DataInputt.Logging
{
    internal class EventLoggingData : ILogData
    {
        public string Message { get; set; }

        public NLog.LogLevel Severity { get; set; }

        public int EventId { get; set; }
    }
}