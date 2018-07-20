using System;
using System.IO;
using Castle.Core.Logging;
using NLog;

namespace Mi.Fish.Infrastructure.Logging
{
    public class NLogLoggerFactory : AbstractLoggerFactory
    {
        internal const string DefaultConfigFileName = "NLog.config";

        public NLogLoggerFactory()
            : this(DefaultConfigFileName)
        {
        }

        public NLogLoggerFactory(string configFileName)
        {
            if (!File.Exists(configFileName))
            {
                throw new FileNotFoundException(configFileName);
            }

            NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(configFileName);
        }

        public override Castle.Core.Logging.ILogger Create(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            return new NLogLogger(LogManager.GetLogger(name));
        }

        public override Castle.Core.Logging.ILogger Create(string name, LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
        }
    }
}