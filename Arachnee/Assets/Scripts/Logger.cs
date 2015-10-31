using UnityEngine;

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
    Fatal
}

public class Logger : MonoBehaviour 
{
    public static void Trace (string message, LogLevel level)
    {
        if (level == LogLevel.Debug)
        {
            Debug.Log("Debug: " + message);
        }
        else if (level == LogLevel.Info)
        {
            Debug.Log("Info: " + message);
        }
        else if (level == LogLevel.Warning)
        {
            Debug.LogWarning("Warning: " + message);
        }
        else if (level == LogLevel.Error)
        {
            Debug.LogError("ERROR: " + message);
        }
        else
        {
            Debug.LogError("FATAL: " + message);
        }
    }

	
}
