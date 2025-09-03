using UnityEngine;
using TMPro;
using System;
using System.Threading.Tasks;

public class RoomUIManager : MonoBehaviour
{
    // シリアライズフィールドとしてUIコンポーネントと参加処理クラスの参照をアサイン
    [SerializeField] private TMP_InputField _sessionInputField;
    [SerializeField] private TMP_Text _roomIdText;
    [SerializeField] private NetworkRunnerHandler _networkRunnerHandler;

    [Header("Buttons")]
    [SerializeField] private UnityEngine.UI.Button _joinButton;
    [SerializeField] private UnityEngine.UI.Button _createButton;

    /// <summary>
    /// UnityのStartメソッドで初期設定を行います。
    /// </summary>
    private void Start()
    {
        // アプリケーション開始時に時刻に基づいたセッションIDを生成して表示
        GenerateSessionIdByTimeOfDay();

        // InputField の onValueChanged イベントに関数を登録
        _sessionInputField.onValueChanged.AddListener(OnInputFieldValueChanged);

        // 初回起動時にボタンの状態を更新
        UpdateButtonStates();
    }

    /// <summary>
    /// InputField の入力値が変更されたときに呼ばれるメソッド。
    /// </summary>
    /// <param name="value">現在の入力値</param>
    private void OnInputFieldValueChanged(string value)
    {
        UpdateButtonStates();
    }

    /// <summary>
    /// ボタンの有効・無効をリアルタイムで更新するメソッド。
    /// </summary>
    private void UpdateButtonStates()
    {
        // 参加ボタンの有効性をチェック
        string inputId = _sessionInputField.text;
        bool isJoinValid = IsValidJoinId(inputId);
        _joinButton.interactable = isJoinValid;
    }

    /// <summary>
    /// UIボタンの有効・無効を切り替えるメソッド。
    /// </summary>
    /// <param name="interactable">ボタンを有効にするか無効にするか</param>
    private void SetButtonsInteractable(bool interactable)
    {
        _joinButton.interactable = interactable;
        _createButton.interactable = interactable;
    }

    /// <summary>
    /// 入力されたIDが参加条件を満たしているか確認するヘルパーメソッド。
    /// </summary>
    /// <param name="id">ユーザーが入力したID</param>
    /// <returns>有効なIDであればtrue、そうでなければfalse</returns>
    private bool IsValidJoinId(string id)
    {
        // 入力値が5桁の数値であるかを確認
        if (id.Length != 5 || !int.TryParse(id, out int userValue))
        {
            return false;
        }

        // 入力値が過去の秒数であるかを確認
        int currentSecondsOfDay = (int)(DateTime.UtcNow - DateTime.UtcNow.Date).TotalSeconds;
        if (userValue >= currentSecondsOfDay)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 現在時刻の「その日の午前0時からの秒数」を基にセッションIDを生成し、表示するメソッド。
    /// </summary>
    public void GenerateSessionIdByTimeOfDay()
    {
        // 現在時刻の、その日中の秒数を計算 (0～86399)
        int uniqueId = (int)(DateTime.UtcNow - DateTime.UtcNow.Date).TotalSeconds;

        string newId = uniqueId.ToString("D5"); // 5桁のゼロ埋め
        _roomIdText.text = newId;
    }

    /// <summary>
    /// UIのボタンから呼ばれ、InputFieldのIDで参加を試みるメソッド。
    /// </summary>
    public void OnJoinButton()
    {
        string inputId = _sessionInputField.text;

        // ボタンが有効な状態で押された場合のみ処理を実行
        if (IsValidJoinId(inputId))
        {
            // 今日の0時からの秒数を計算
            int secondsSinceEpochToday = (int)(DateTime.UtcNow.Date - new DateTime(1970, 1, 1)).TotalSeconds;

            // ユーザーの入力値に今日の秒数を足して、一意なセッションIDを生成
            int userValue = int.Parse(inputId);
            string sessionId = (secondsSinceEpochToday + userValue).ToString();

            // 参加処理クラスのメソッドを呼び出し、ボタンを無効化
            SetButtonsInteractable(false);

            Debug.Log($"Entered ID: {userValue}");
            _ = HandleJoinAttempt(sessionId);
        }
    }

    /// <summary>
    /// UIのボタンから呼ばれ、ランダム生成されたIDで参加を試みるメソッド。
    /// </summary>
    public void OnCreateButton()
    {
        string inputId = _roomIdText.text;

        if (string.IsNullOrEmpty(inputId) || !int.TryParse(inputId, out int generatedValue))
        {
            Debug.LogError("The session ID is not generated.");
            return;
        }

        // 今日の0時からの秒数を計算
        int secondsSinceEpochToday = (int)(DateTime.UtcNow.Date - new DateTime(1970, 1, 1)).TotalSeconds;

        // 生成された値に今日の秒数を足して、一意なセッションIDを生成
        string sessionId = (secondsSinceEpochToday + generatedValue).ToString();

        // 参加処理クラスのメソッドを呼び出し、ボタンを無効化
        SetButtonsInteractable(false);
        _ = HandleJoinAttempt(sessionId);
    }

    /// <summary>
    /// 参加処理を待ち、完了後にボタンを有効化する非同期ヘルパーメソッド。
    /// </summary>
    private async Task HandleJoinAttempt(string sessionId)
    {
        Debug.Log($"Joined the Session with {sessionId}");
        try
        {
            await _networkRunnerHandler.StartGame(sessionId);
        }
        finally
        {
            // 処理が完了したらボタンを有効化
            SetButtonsInteractable(true);
        }
    }
}