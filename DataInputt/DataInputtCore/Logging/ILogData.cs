using NLog;

namespace DataInputt.Logging
{
    /// <summary>
    ///     Specifies the information to log.
    /// </summary>
    internal interface ILogData
    {
        /// <summary>
        ///     Gets or sets the message to write to the logging target.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        ///     Gets or sets the severity of the log entry.
        /// </summary>
        LogLevel Severity { get; set; }
    }
}