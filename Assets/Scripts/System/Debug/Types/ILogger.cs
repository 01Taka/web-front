using UnityEngine;

public interface ILogger
{
    /// <summary>
    /// ��񃍃O���o�͂��܂��B
    /// </summary>
    void LogInfo(object message, Object context = null);

    /// <summary>
    /// �x�����O���o�͂��܂��B
    /// </summary>
    void LogWarning(object message, Object context = null);

    /// <summary>
    /// �G���[���O���o�͂��܂��B
    /// </summary>
    void LogError(object message, Object context = null);
}