using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class InGameDebugLog : MonoBehaviour
{
    [SerializeField] private TMP_Text[] _logTexts;
    [SerializeField] private TMP_FontAsset[] _fontAssets;
    [SerializeField] private int _maxLines = 100;

    // 外部からテキスト更新を制御するためのフラグ
    [SerializeField] private bool _isUpdatingText = false;

    // ログの行を効率的に管理するためのQueue
    private Queue<string> _logLines = new Queue<string>();

    // UI更新用のStringBuilder
    private StringBuilder _uiBuilder = new StringBuilder();

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    /// <summary>
    /// ログのテキスト更新を有効にする。
    /// </summary>
    public void EnableTextUpdate()
    {
        _isUpdatingText = true;
        // 有効化時にUIを最新の状態に更新
        UpdateLogText();
    }

    /// <summary>
    /// ログのテキスト更新を無効にする。
    /// </summary>
    public void DisableTextUpdate()
    {
        _isUpdatingText = false;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string color;

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

        string sanitizedLog = SanitizeTextForFont(logString);
        string formattedLog = $"<color={color}>{sanitizedLog}</color>";

        _logLines.Enqueue(formattedLog);

        if (_logLines.Count > _maxLines)
        {
            _logLines.Dequeue();
        }

        // テキスト更新が有効な場合のみ、UIを更新する
        if (_isUpdatingText)
        {
            UpdateLogText();
        }
    }

    /// <summary>
    /// UIのテキストを更新する内部メソッド。
    /// </summary>
    private void UpdateLogText()
    {
        _uiBuilder.Clear();
        foreach (var line in _logLines)
        {
            _uiBuilder.AppendLine(line);
        }

        foreach (var logText in _logTexts)
        {
            // 非アクティブなGameObjectへの更新をスキップ
            if (logText.gameObject.activeInHierarchy)
            {
                logText.text = _uiBuilder.ToString();
            }
        }
    }

    private string SanitizeTextForFont(string text)
    {
        var sb = new StringBuilder();
        foreach (char c in text)
        {
            if (IsCharacterSupportedByAnyFont(c))
            {
                sb.Append(c);
            }
            else
            {
                sb.Append('*');
            }
        }
        return sb.ToString();
    }

    private bool IsCharacterSupportedByAnyFont(char c)
    {
        foreach (var fontAsset in _fontAssets)
        {
            if (fontAsset != null && fontAsset.HasCharacter(c))
            {
                return true;
            }
        }
        return false;
    }
}