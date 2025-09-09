using UnityEngine;
using System.Collections.Generic;
using System;

public enum ExplosionType
{
    Default,
    Blue,
    Big
}

[Serializable]
public class ExplosionSettings
{
    public float explosionDuration = 0.5f;
    public Vector3 maxScale = new Vector3(2f, 2f, 1f);
    public Color startColor = Color.yellow;
    public Color endColor = new Color(1f, 1f, 1f, 0f);
    public Sprite explosionSprite;
}

public class ExplosionEffectPoolManager : MonoBehaviour
{
    public static ExplosionEffectPoolManager Instance { get; private set; }

    [Serializable]
    public class ExplosionSettingsEntry
    {
        public ExplosionType explosionType;
        public ExplosionSettings settings;
    }

    [Header("Pool Settings")]
    [SerializeField] private ExplosionEffect explosionPrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private Transform poolParent;

    [Header("Explosion Settings")]
    [SerializeField] private List<ExplosionSettingsEntry> allExplosionSettings;

    private ObjectPool<ExplosionEffect> _pool;
    private Dictionary<ExplosionType, ExplosionSettings> _settingsDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // ExplosionType → Settings に変換して検索を高速化
        _settingsDict = new Dictionary<ExplosionType, ExplosionSettings>();
        foreach (var entry in allExplosionSettings)
        {
            if (!_settingsDict.ContainsKey(entry.explosionType))
            {
                _settingsDict.Add(entry.explosionType, entry.settings);
            }
        }

        // 汎用プール作成
        _pool = new ObjectPool<ExplosionEffect>(explosionPrefab, poolSize, poolParent);
    }

    public void PlayExplosion(Vector3 position, float sizeRatio, ExplosionType type)
    {
        if (!_settingsDict.TryGetValue(type, out var settings))
        {
            Debug.LogError($"Explosion type '{type}' not found in settings.");
            return;
        }

        ExplosionEffect effect = _pool.Get();
        effect.transform.position = position;
        effect.Initialize(settings);
        effect.SetSizeRaito(sizeRatio);

        // ExplosionEffect が終了したら自分で ReturnToPool を呼ぶ
        effect.OnReturnToPool = () => _pool.ReturnToPool(effect);
    }
}
