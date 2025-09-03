using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class InGameDebugLog : MonoBehaviour
{
    [SerializeField] private TMP_Text[] _logTexts;
    [SerializeField] private TMP_FontAsset[] _fontAssets;
    [SerializeField] private int _maxLines = 100;

    // �O������e�L�X�g�X�V�𐧌䂷�邽�߂̃t���O
    [SerializeField] private bool _isUpdatingText = false;

    // ���O�̍s�������I�ɊǗ����邽�߂�Queue
    private Queue<string> _logLines = new Queue<string>();

    // UI�X�V�p��StringBuilder
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
    /// ���O�̃e�L�X�g�X�V��L���ɂ���B
    /// </summary>
    public void EnableTextUpdate()
    {
        _isUpdatingText = true;
        // �L��������UI���ŐV�̏�ԂɍX�V
        UpdateLogText();
    }

    /// <summary>
    /// ���O�̃e�L�X�g�X�V�𖳌��ɂ���B
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

        // �e�L�X�g�X�V���L���ȏꍇ�̂݁AUI���X�V����
        if (_isUpdatingText)
        {
            UpdateLogText();
        }
    }

    /// <summary>
    /// UI�̃e�L�X�g���X�V����������\�b�h�B
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
            // ��A�N�e�B�u��GameObject�ւ̍X�V���X�L�b�v
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