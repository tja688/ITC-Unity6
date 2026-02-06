using UnityEngine;

namespace UnityCodeIntel.Editor
{
    public static class CILogger
    {
        public static void Log(string message)
        {
            if (CodeIntelManager.DebugEnabled)
                Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            if (CodeIntelManager.DebugEnabled)
                Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            if (CodeIntelManager.DebugEnabled)
                Debug.LogError(message);
        }
        
        public static void LogException(System.Exception e)
        {
            if (CodeIntelManager.DebugEnabled)
                Debug.LogException(e);
        }
    }
}
