using System.Collections;
using System.Linq;
using NLog;
using NLog.Fluent;

namespace NzbDrone.Common.Instrumentation.Extensions
{
    public static class SentryLoggerExtensions
    {
        public static readonly Logger SentryLogger = LogManager.GetLogger("Sentry");

        public static LogEventBuilder SentryFingerprint(this LogEventBuilder logBuilder, params string[] fingerprint)
        {
            return logBuilder.Property("Sentry", fingerprint);
        }

        public static LogEventBuilder WriteSentryDebug(this LogEventBuilder logBuilder, params string[] fingerprint)
        {
            return LogSentryMessage(logBuilder, LogLevel.Debug, fingerprint);
        }

        public static LogEventBuilder WriteSentryInfo(this LogEventBuilder logBuilder, params string[] fingerprint)
        {
            return LogSentryMessage(logBuilder, LogLevel.Info, fingerprint);
        }

        public static LogEventBuilder WriteSentryWarn(this LogEventBuilder logBuilder, params string[] fingerprint)
        {
            return LogSentryMessage(logBuilder, LogLevel.Warn, fingerprint);
        }

        public static LogEventBuilder WriteSentryError(this LogEventBuilder logBuilder, params string[] fingerprint)
        {
            return LogSentryMessage(logBuilder, LogLevel.Error, fingerprint);
        }

        private static LogEventBuilder LogSentryMessage(LogEventBuilder logBuilder, LogLevel level, string[] fingerprint)
        {
            SentryLogger.ForLogEvent(level)
                        .CopyLogEvent(logBuilder.LogEvent)
                        .SentryFingerprint(fingerprint)
                        .Log();

            return logBuilder.Property("Sentry", null);
        }

        private static LogEventBuilder CopyLogEvent(this LogEventBuilder logBuilder, LogEventInfo logEvent)
        {
            return logBuilder.TimeStamp(logEvent.TimeStamp)
                             .Message(logEvent.Message, logEvent.Parameters)
                             .Properties(logEvent.Properties.ToDictionary(v => v.Key, v => v.Value))
                             .Exception(logEvent.Exception);
        }
    }
}
