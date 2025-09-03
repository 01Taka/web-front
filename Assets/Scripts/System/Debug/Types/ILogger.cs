using UnityEngine;

public interface ILogger
{
    /// <summary>
    /// 情報ログを出力します。
    /// </summary>
    void LogInfo(object message, Object context = null);

    /// <summary>
    /// 警告ログを出力します。
    /// </summary>
    void LogWarning(object message, Object context = null);

    /// <summary>
    /// エラーログを出力します。
    /// </summary>
    void LogError(object message, Object context = null);
}