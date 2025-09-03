using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// シフトキーが押されている間のみ、アルファベットキーによる隠しコマンド入力を受け付けるスクリプト。
/// </summary>
public class HiddenCommandSystem : MonoBehaviour
{
    // 隠しコマンドとイベントをペアで管理するためのクラス
    [System.Serializable]
    public class CommandEvent
    {
        [Tooltip("例: 'log', 'setting'")]
        public string commandString;
        [Tooltip("このコマンドが成功したときに呼び出されるUnityEvent")]
        public UnityEvent onCommandSuccess;
    }

    // インスペクターで設定可能な隠しコマンドとイベントの配列
    [SerializeField]
    private CommandEvent[] _commandEvents;

    // 入力中のコマンド文字列
    private string _currentInput = "";

    // キーボードデバイスへの参照
    private Keyboard _keyboard;

    void Awake()
    {
        // キーボードデバイスを取得
        _keyboard = Keyboard.current;
        if (_keyboard == null)
        {
            Debug.LogError("Keyboard device not found!");
            enabled = false; // スクリプトを無効化
        }
    }

    void Update()
    {
        // キーボードがなければ何もしない
        if (_keyboard == null) return;

        // シフトキーの状態をチェック
        if (!_keyboard.leftShiftKey.isPressed && !_keyboard.rightShiftKey.isPressed)
        {
            // シフトキーが離されたら入力をリセット
            if (_currentInput.Length > 0)
            {
                _currentInput = "";
                Debug.Log("シフトキーが離されたため、入力をリセットしました。");
            }
            return; // シフトが押されていない場合は、これ以降の処理を行わない
        }

        // シフトキーが押されている場合のみ、アルファベットキーをチェック
        foreach (var key in _keyboard.allKeys)
        {
            // キーがアルファベットキーであり、かつ押された瞬間に処理
            if (key.keyCode >= Key.A && key.keyCode <= Key.Z && key.wasPressedThisFrame)
            {
                // キー名を取得して小文字に変換
                string keyName = key.displayName.ToLower();
                _currentInput += keyName;

                // コマンドが一致するかチェック
                CheckForCommandMatch();
            }
        }
    }

    /// <summary>
    /// 現在の入力文字列が隠しコマンドに一致するかをチェックし、一致した場合はイベントを起動します。
    /// </summary>
    private void CheckForCommandMatch()
    {
        bool foundMatch = false;
        foreach (var commandEvent in _commandEvents)
        {
            // 現在の入力がコマンド文字列の先頭と一致するか確認
            if (commandEvent.commandString.StartsWith(_currentInput))
            {
                foundMatch = true;
                if (_currentInput == commandEvent.commandString)
                {
                    Debug.Log($"Entered Command of {commandEvent.commandString}");
                    commandEvent.onCommandSuccess.Invoke();
                    _currentInput = ""; // 成功後、入力をリセット
                    return;
                }
            }
        }

        if (!foundMatch)
        {
            // どのコマンドの途中とも一致しない場合、入力をリセット
            _currentInput = "";
        }
    }
}