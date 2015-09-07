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
        if (false
            || level == LogLevel.Debug
            || level == LogLevel.Info
            || level == LogLevel.Warning
            || level == LogLevel.Error
            || level == LogLevel.Fatal
            )
        {
            Debug.Log(message);
        }
        
    }

	
}
