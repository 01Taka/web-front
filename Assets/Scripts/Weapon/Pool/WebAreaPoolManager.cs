using UnityEngine;

public class WebAreaPoolManager : MonoBehaviour
{
    public static WebAreaPoolManager Instance;

    [SerializeField] private WebArea webAreaPrefab;
    [SerializeField] private int initialPoolSize = 10;

    private ObjectPool<WebArea> _webAreaPool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _webAreaPool = new ObjectPool<WebArea>(webAreaPrefab, initialPoolSize, transform);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public WebArea GetWebArea()
    {
        return _webAreaPool.Get();
    }

    public void ReturnWebArea(WebArea webArea)
    {
        _webAreaPool.ReturnToPool(webArea);
    }
}