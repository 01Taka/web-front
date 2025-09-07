using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;

public class NetworkRunnerHandlerManager : MonoBehaviour
{
    [Header("Prefabs and Managers")]
    [SerializeField] private NetworkRunner _runnerPrefab;

    [Header("Game Settings")]
    [SerializeField] private string _gameSceneName = "GameScene";

    // NetworkRunnerHandler�̃v���n�u���C���X�y�N�^�[����ݒ�
    [SerializeField] private NetworkRunnerHandler _runnerHandlerPrefab;

    [SerializeField] private int _peerCount = 1;
    [SerializeField] private NetworkProjectConfigAsset _configAsset;

    private List<NetworkRunnerHandler> _activeRunners = new List<NetworkRunnerHandler>();

    public async Task StartGame(string sessionName)
    {
        Debug.Log($"StartSession: {sessionName}");
#if UNITY_EDITOR
        // Unity�G�f�B�^�ł̂ݕ����̃s�A������
        int peerCount = _configAsset.Config.PeerMode == NetworkProjectConfig.PeerModes.Single ? 1 : _peerCount;
#else
    // �r���h���ꂽ�Q�[���ł͏�ɒP��s�A�ɋ���
    int peerCount = 1;
#endif
        await StartMultiplePeers(sessionName, peerCount);
    }

    // �Q�[���Z�b�V�������J�n���邽�߂̌��J���\�b�h
    // �����Ńs�A�̐����w�肵�āA���ꂼ����N�����܂�
    public async Task StartMultiplePeers(string sessionName, int peerCount)
    {
        if (_runnerHandlerPrefab == null)
        {
            Debug.LogError("NetworkRunnerHandler prefab is not assigned.");
            return;
        }

        for (int i = 0; i < peerCount; i++)
        {
            await CreateAndStartPeer(sessionName);
        }
    }

    private async Task CreateAndStartPeer(string sessionName)
    {

        // �V���� NetworkRunnerHandler �C���X�^���X�𐶐�
        NetworkRunnerHandler newRunnerHandler = Instantiate(_runnerHandlerPrefab, transform);
        _activeRunners.Add(newRunnerHandler);

        // �e�s�A�̃Q�[���Z�b�V�������J�n
        // PlayerSpawner�������Ƃ��ēn���܂�
        await newRunnerHandler.StartGame(_gameSceneName, sessionName, _runnerPrefab);
    }

    private void OnApplicationQuit()
    {
        // �A�v���P�[�V�����I�����ɂ��ׂẴ����i�[���V���b�g�_�E��
        foreach (var handler in _activeRunners)
        {
            // �����ł͔񓯊���Shutdown��҂����Ɏ��s
            handler.GetComponent<NetworkRunner>()?.Shutdown();
        }
    }
}