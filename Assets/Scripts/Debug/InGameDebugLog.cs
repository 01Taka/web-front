using UnityEngine;
using TMPro;
using System.Text;

public class InGameDebugLog : MonoBehaviour
{
    public TMP_Text[] logTexts;
    private StringBuilder logBuilder = new StringBuilder();
    private const int maxLines = 100;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string color;

        // ログタイプごとに色分け
        switch (type)
        {
            case LogType.Warning:
                color = "yellow";
                break;
            case LogType.Error:
            case LogType.Exception:
                color = "red";
                break;
            case LogType.Assert:
                color = "magenta";
                break;
            default:
                color = "white";
                break;
        }

        string formattedLog = $"<color={color}>{logString}</color>";

        logBuilder.AppendLine(formattedLog);

        // 行数制限
        string[] lines = logBuilder.ToString().Split('\n');
        if (lines.Length > maxLines)
        {
            logBuilder = new StringBuilder(string.Join("\n", lines, lines.Length - maxLines, maxLines));
        }

        foreach (var logText in logTexts)
        {
            logText.text = logBuilder.ToString();
        }
    }
}
