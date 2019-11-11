using System;

namespace DataInputt.Logging
{
    /// <summary>
    ///     Encapsulates an <see cref="ILog"/>-implementation that uses <see cref="NLog"/> as logging framework.
    /// </summary>
    /// <seealso cref="ILog" />
    internal class NLogFacade : ILog, IDisposable
    {
        public NLog.LogLevel Severity { get; set; }

        public NLogFacade(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("Given filename is invalid", nameof(filename));
            }

            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget{FileName = filename, Name = "logfile"};

            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", NLog.LogLevel.Debug, logfile));
            NLog.LogManager.Configuration = config;
        }

        public void Write(ILogData data)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Log(data.Severity, data.Message);
        }

        public void Configure(string configFile)
        {
            NLog.LogManager.LoadConfiguration(configFile);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                NLog.LogManager.Shutdown();
            }
        }
    }
}
