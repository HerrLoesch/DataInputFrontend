using System;

namespace DataInputt.Logging
{
    /// <summary>
    ///     Extends the <see cref="LogData" /> class with additional information required for the <see cref="FileLog" /> class.
    /// </summary>
    /// <seealso cref="DataInputt.Logging.ILogData" />
    internal class FileLogData : LogData
    {
        public DateTime Timestamp { get; set; }
    }
}