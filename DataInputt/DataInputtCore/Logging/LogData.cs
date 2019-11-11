using NLog;

namespace DataInputt.Logging
{
    /// <summary>
    ///     Default implementation of <see cref="ILogData" />
    /// </summary>
    /// <seealso cref="DataInputt.Logging.ILogData" />
    internal class LogData : ILogData
    {
        /// <inheritdoc />
        public string Message { get; set; }

        /// <inheritdoc />
        public LogLevel Severity { get; set; }
    }
}