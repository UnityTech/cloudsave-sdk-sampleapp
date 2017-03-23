using System;

namespace UnitySocialCloudSave.Utils
{
    internal class UnityDebugLogger : InternalLogger
    {
        public UnityDebugLogger(Type type) : base(type)
        {
        }

        public override void Error(Exception exception, string messageFormat, params object[] args)
        {
            if (exception != null)
                UnityEngine.Debug.LogException(exception);

            if (!string.IsNullOrEmpty(messageFormat))
                UnityEngine.Debug.LogError(string.Format(messageFormat, args));
        }

        public override void Debug(Exception exception, string messageFormat, params object[] args)
        {
            if (exception != null)
                UnityEngine.Debug.LogException(exception);

            if (!string.IsNullOrEmpty(messageFormat))
                UnityEngine.Debug.Log(string.Format(messageFormat, args));
        }

        public override void DebugFormat(string messageFormat, params object[] args)
        {
            if (!string.IsNullOrEmpty(messageFormat))
                UnityEngine.Debug.Log(string.Format(messageFormat, args));
        }
    }
}