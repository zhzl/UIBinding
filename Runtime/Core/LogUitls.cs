using System.Diagnostics;

namespace UIBinding
{
    public interface ILogger
    {
        void Log(string message);
        void Warning(string message);
        void Error(string message);
    }

    internal static class LogUtils
    {
        public static ILogger Logger = null;

        [Conditional("DEBUG")]
        public static void Log(string message)
        {
            message = $"[UIBinding]{message}";

            if (Logger != null)
            {
                Logger.Log(message);
            }
            else
            {
                UnityEngine.Debug.Log(message);
            }
        }

        public static void Warning(string message)
        {
            message = $"[UIBinding]{message}";

            if (Logger != null)
            {
                Logger.Warning(message);
            }
            else
            {
                UnityEngine.Debug.LogWarning(message);
            }
        }

        public static void Error(string message)
        {
            message = $"[UIBinding]{message}";

            if (Logger != null)
            {
                Logger.Error(message);
            }
            else
            {
                UnityEngine.Debug.LogError(message);
            }
        }
    }
}