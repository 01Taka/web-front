using UnityEngine;
using System.Diagnostics;
using System.Reflection; // これを追加

public static class MyLogger
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    // 通常のログメソッド（リフレクションなし）
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    private static void InternalLog(object message, LogLevel level, Object context)
    {
        string prefix = $"[{level}] ";
        string logMessage;

        if (message is System.Collections.IEnumerable enumerable && !(message is string))
        {
            logMessage = string.Join(", ", GetStringArrayFromEnumerable(enumerable));
        }
        else
        {
            logMessage = message?.ToString() ?? "null";
        }

        switch (level)
        {
            case LogLevel.Info:
                UnityEngine.Debug.Log(prefix + logMessage, context);
                break;
            case LogLevel.Warning:
                UnityEngine.Debug.LogWarning(prefix + logMessage, context);
                break;
            case LogLevel.Error:
                UnityEngine.Debug.LogError(prefix + logMessage, context);
                break;
        }
    }

    // リフレクションを使用するログメソッド
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    private static void InternalLogReflective(object message, LogLevel level, Object context)
    {
        string prefix = $"[{level}] (Reflective) ";
        string logMessage = "null";

        if (message != null)
        {
            logMessage = GetStringFromObjectWithReflection(message);
        }

        switch (level)
        {
            case LogLevel.Info:
                UnityEngine.Debug.Log(prefix + logMessage, context);
                break;
            case LogLevel.Warning:
                UnityEngine.Debug.LogWarning(prefix + logMessage, context);
                break;
            case LogLevel.Error:
                UnityEngine.Debug.LogError(prefix + logMessage, context);
                break;
        }
    }

    // IEnumerableから文字列の配列を取得するヘルパーメソッド
    private static string[] GetStringArrayFromEnumerable(System.Collections.IEnumerable enumerable)
    {
        var list = new System.Collections.Generic.List<string>();
        foreach (var item in enumerable)
        {
            list.Add(item?.ToString() ?? "null");
        }
        return list.ToArray();
    }

    // オブジェクトの内容をリフレクションで文字列化するヘルパーメソッド
    private static string GetStringFromObjectWithReflection(object obj)
    {
        System.Type type = obj.GetType();
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var members = new System.Collections.Generic.List<string>();

        foreach (var field in fields)
        {
            members.Add($"{field.Name}: {field.GetValue(obj)}");
        }
        foreach (var prop in properties)
        {
            if (prop.CanRead)
            {
                members.Add($"{prop.Name}: {prop.GetValue(obj)}");
            }
        }
        return $"{type.Name} {{ {string.Join(", ", members)} }}";
    }

    // --- 公開メソッド ---

    // 通常の公開ログメソッド
    public static void Log(object message, Object context = null)
    {
        InternalLog(message, LogLevel.Info, context);
    }

    public static void LogWarning(object message, Object context = null)
    {
        InternalLog(message, LogLevel.Warning, context);
    }

    public static void LogError(object message, Object context = null)
    {
        InternalLog(message, LogLevel.Error, context);
    }

    // リフレクションを使用する公開ログメソッド
    public static void LogReflective(object message, Object context = null)
    {
        InternalLogReflective(message, LogLevel.Info, context);
    }

    public static void LogWarningReflective(object message, Object context = null)
    {
        InternalLogReflective(message, LogLevel.Warning, context);
    }

    public static void LogErrorReflective(object message, Object context = null)
    {
        InternalLogReflective(message, LogLevel.Error, context);
    }
}