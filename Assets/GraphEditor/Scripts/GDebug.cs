using UnityEngine;

/// <summary>
/// Обертка для методов класса Debug.
/// </summary>
/// 
//TODO: обернуть в #if UNITY_EDITOR ?
public class GDebug : MonoBehaviour
{
    private const string PLUGIN_NAME = "GraphEditor: ";

    /// <summary>
    /// Вывести сообщение об ошибке
    /// </summary>
    public static void LogError(object message, Object context)
    {
        if (GraphEditor.IsDebug)
        {
            message = PLUGIN_NAME + message;
            Debug.LogError(message, context);
        }
    }
    /// <summary>
    /// Вывести сообщение об ошибке
    /// </summary>
    public static void LogError(object message)
    {
        if (GraphEditor.IsDebug)
        {
            message = PLUGIN_NAME + message;
            Debug.LogError(message);
        }
    }

    /// <summary>
    /// Вывести сообщение
    /// </summary>
    public static void Log(object message, Object context)
    {
        if (GraphEditor.IsDebug)
        {
            message = PLUGIN_NAME + message;
            Debug.Log(message, context);
        }
    }
    /// <summary>
    /// Вывести сообщение
    /// </summary>
    public static void Log(object message)
    {
        if (GraphEditor.IsDebug)
        {
            message = PLUGIN_NAME + message;
            Debug.Log(message);
        }
    }

    /// <summary>
    /// Вывести предупреждение
    /// </summary>
    public static void LogWarning(object message, Object context)
    {
        if (GraphEditor.IsDebug)
        {
            message = PLUGIN_NAME + message;
            Debug.LogError(message, context);
        }
    }
    /// <summary>
    /// Вывести предупреждение
    /// </summary>
    public static void LogWarning(object message)
    {
        if (GraphEditor.IsDebug)
        {
            message = PLUGIN_NAME + message;
            Debug.LogError(message);
        }
    }
}
