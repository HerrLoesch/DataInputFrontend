using NLog;

namespace DataInputt.Logging
{
    internal interface ILog
    {
        /// <summary>
        ///     Gets or sets the logging level. Only messages with a <see cref="ILogData.Severity" />
        ///     below this property value will be written to the logging target.
        /// </summary>
        LogLevel Severity { get; set; }

        /// <summary>
        ///     Adds a new log entry containing the information of passed <paramref name="data" />.
        /// </summary>
        void Write(ILogData data);

        /// <summary>
        ///     Configures the logger by reading the file according to passed path.
        /// </summary>
        /// <param name="configFile">The name of the configuration file.</param>
        void Configure(string configFile);
    }
}