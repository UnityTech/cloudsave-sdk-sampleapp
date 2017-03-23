using System;
using System.Collections.Generic;

namespace UnitySocialCloudSave.Utils
{
    internal class Logger
    {
        internal delegate void onConfigChangedHandler();
        internal static onConfigChangedHandler onConfigChanged;
        private static LoggingConfig _loggingConfig = new LoggingConfig();
        internal static LoggingConfig LoggingConfig { get { return _loggingConfig; } set { _loggingConfig = value; } }
        private static IDictionary<Type, Logger> _loggers = new Dictionary<Type, Logger>();
        private static Logger _emptyLogger = new Logger();

        private List<InternalLogger> _loggerList;    // For now, only UnityDebugLogger

        private Logger()
        {
            _loggerList = new List<InternalLogger>();
        }

        private Logger(Type type)
        {
            _loggerList = new List<InternalLogger>();
            UnityDebugLogger unityLogger = new UnityDebugLogger(type);

            _loggerList.Add(unityLogger);
            ConfigureLoggers();
            onConfigChanged += ConfigureLoggers;
        }

        internal static void ChangingConfig()
        {
            onConfigChanged();
        }

        private void ConfigureLoggers()
        {
            foreach (InternalLogger internalLogger in _loggerList)
            {
                if (internalLogger is UnityDebugLogger)
                    internalLogger.IsEnabled = (LoggingConfig.LogTo & LogToOption.UnityLogger) ==
                                               LogToOption.UnityLogger;
            }
        }

        public static Logger GetLogger(Type type)
        {
#if DEBUG
            if (type == null)
                throw new ArgumentNullException("type");

            Logger logger;

            lock (_loggers)
            {
                if (!_loggers.TryGetValue(type, out logger))
                {
                    logger = new Logger(type);
                    _loggers[type] = logger;
                }
            }

            return logger;
#else
            return _emptyLogger;
#endif
        }

        public void Error(Exception exception, string messageFormat, params object[] args)
        {
            foreach (InternalLogger logger in _loggerList)
            {
                if (logger.IsEnabled)
                    logger.Error(exception, messageFormat, args);
            }
        }

        public void Debug(Exception exception, string messageFormat, params object[] args)
        {
            foreach (InternalLogger logger in _loggerList)
            {
                if (logger.IsEnabled)
                    logger.Debug(exception, messageFormat, args);
            }
        }

        public void DebugFormat(string messageFormat, params object[] args)
        {
            foreach (InternalLogger logger in _loggerList)
            {
                if (logger.IsEnabled)
                    logger.DebugFormat(messageFormat, args);
            }
        }
    }

    internal abstract class InternalLogger
    {
        public Type DeclaringType { get; private set; }

        public bool IsEnabled { get; set; }

        public InternalLogger(Type type)
        {
            DeclaringType = type;
            IsEnabled = true;
        }

        public abstract void Error(Exception exception, string messageFormat, params object[] args);

        public abstract void Debug(Exception exception, string messageFormat, params object[] args);

        public abstract void DebugFormat(string messageFormat, params object[] args);
    }

    #region Logging Config

    internal enum LogToOption
    {
        None = 0,

        UnityLogger = 1
    }

    internal enum LogHttpOption
    {
        Nerver = 0,

        OnError = 1,

        Always = 2
    }

    internal class LoggingConfig
    {

        internal LoggingConfig()
        {
            _logTo = LogToOption.UnityLogger;
            LogHttpOption = LogHttpOption.Always;
            LogInnerMostError = false;
        }

        private LogToOption _logTo;

        public LogToOption LogTo
        {
            get { return _logTo; }
            set { _logTo = value; Logger.ChangingConfig();}
        }

        public LogHttpOption LogHttpOption { get; set; }

        public bool LogInnerMostError { get; set; }
    }

    #endregion
}