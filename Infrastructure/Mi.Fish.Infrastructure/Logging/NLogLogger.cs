using System;
using System.Globalization;
using Castle.Core.Logging;

namespace Mi.Fish.Infrastructure.Logging
{
    [Serializable]
    public class NLogLogger : MarshalByRefObject, ILogger
    {
        public NLogLogger(NLog.ILogger logger)
        {
            Logger = logger;
        }

        internal NLogLogger()
        {
        }

        public bool IsDebugEnabled => Logger.IsEnabled(NLog.LogLevel.Debug);

        public bool IsErrorEnabled => Logger.IsEnabled(NLog.LogLevel.Error);

        public bool IsFatalEnabled => Logger.IsEnabled(NLog.LogLevel.Fatal);

        public bool IsInfoEnabled => Logger.IsEnabled(NLog.LogLevel.Info);

        public bool IsWarnEnabled => Logger.IsEnabled(NLog.LogLevel.Warn);

        protected internal NLogLoggerFactory Factory { get; set; }

        protected internal NLog.ILogger Logger { get; set; }

        public override string ToString()
        {
            return Logger.ToString();
        }

        public virtual Castle.Core.Logging.ILogger CreateChildLogger(string name)
        {
            return Factory.Create(Logger.Name + "." + name);
        }

        public void Debug(string message)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(message);
            }
        }

        public void Debug(Func<string> messageFactory)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(messageFactory);
            }
        }

        public void Debug(string message, Exception exception)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(exception, message);
            }
        }

        public void DebugFormat(string format, params Object[] args)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(CultureInfo.InvariantCulture, format, args);
            }
        }

        public void DebugFormat(Exception exception, string format, params Object[] args)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(exception, CultureInfo.InvariantCulture, format, args);
            }
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(formatProvider, format, args);
            }
        }

        public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (IsDebugEnabled)
            {
                Logger.Debug(exception, formatProvider, format, args);
            }
        }

        public void Error(string message)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(message);
            }
        }

        public void Error(Func<string> messageFactory)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(messageFactory);
            }
        }

        public void Error(string message, Exception exception)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(exception, message);
            }
        }

        public void ErrorFormat(string format, params Object[] args)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(CultureInfo.InvariantCulture, format, args);
            }
        }

        public void ErrorFormat(Exception exception, string format, params Object[] args)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(exception, CultureInfo.InvariantCulture, format, args);
            }
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(formatProvider, format, args);
            }
        }

        public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (IsErrorEnabled)
            {
                Logger.Error(exception, formatProvider, format, args);
            }
        }

        public void Fatal(string message)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(message);
            }
        }

        public void Fatal(Func<string> messageFactory)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(messageFactory);
            }
        }

        public void Fatal(string message, Exception exception)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(exception, message);
            }
        }

        public void FatalFormat(string format, params Object[] args)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(CultureInfo.InvariantCulture, format, args);
            }
        }

        public void FatalFormat(Exception exception, string format, params Object[] args)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(exception, CultureInfo.InvariantCulture, format, args);
            }
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(formatProvider, format, args);
            }
        }

        public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (IsFatalEnabled)
            {
                Logger.Fatal(exception, formatProvider, format, args);
            }
        }

        public void Info(string message)
        {
            if (IsInfoEnabled)
            {
                Logger.Info(message);
            }
        }

        public void Info(Func<string> messageFactory)
        {
            if (IsInfoEnabled)
            {
                Logger.Info(messageFactory);
            }
        }

        public void Info(string message, Exception exception)
        {
            if (IsInfoEnabled)
            {
                Logger.Info(exception, message);
            }
        }

        public void InfoFormat(string format, params Object[] args)
        {
            if (IsInfoEnabled)
            {
                Logger.Info(CultureInfo.InvariantCulture, format, args);
            }
        }

        public void InfoFormat(Exception exception, string format, params Object[] args)
        {
            if (IsInfoEnabled)
            {
                Logger.Info(exception, CultureInfo.InvariantCulture, format, args);
            }
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (IsInfoEnabled)
            {
                Logger.Info(formatProvider, format, args);
            }
        }

        public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (IsInfoEnabled)
            {
                Logger.Info(exception, formatProvider, format, args);
            }
        }

        public void Warn(string message)
        {
            if (IsWarnEnabled)
            {
                Logger.Warn(message);
            }
        }

        public void Warn(Func<string> messageFactory)
        {
            if (IsWarnEnabled)
            {
                Logger.Warn(messageFactory);
            }
        }

        public void Warn(string message, Exception exception)
        {
            if (IsWarnEnabled)
            {
                Logger.Warn(exception, message);
            }
        }

        public void WarnFormat(string format, params Object[] args)
        {
            if (IsWarnEnabled)
            {
                Logger.Warn(CultureInfo.InvariantCulture, format, args);
            }
        }

        public void WarnFormat(Exception exception, string format, params Object[] args)
        {
            if (IsWarnEnabled)
            {
                Logger.Warn(exception, CultureInfo.InvariantCulture, format, args);
            }
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (IsWarnEnabled)
            {
                Logger.Warn(formatProvider, format, args);
            }
        }

        public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params Object[] args)
        {
            if (IsWarnEnabled)
            {
                Logger.Warn(exception, formatProvider, format, args);
            }
        }
    }
}