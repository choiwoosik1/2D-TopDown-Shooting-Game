using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prefab Key(경로)별로 Pool을 생성/공유 하는 Manager
/// "Prefabs/{Key}"를 Resources에서 Load
/// </summary>
public class PoolManager : MonoBehaviour
{
    // Singleton 패턴. 게임 전체에 PoolManager는 단 하나만 존재하도록 보장
    // 어떤 Script에서든 PoolManager.Instance로 이 Script에 즉시 접근할 수 있도록 함
    public static PoolManager Instance { get; private set; }

    const string _prefabPath = "Prefabs/{0}";
    [SerializeField] int _defaultSize = 30;

    public ResourcesManager _resourcesManager { get; private set; }

    private readonly Dictionary<string, Pool> _poolMap = new();

    private void Awake()
    {
        // 혹시 자신 말고 다른 PoolManager가 있으면 스스로를 파괴함. Scene을 여러번 Load해도
        // PoolManage가 여러 개 생기는 것을 방지함.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 자신을 유일한 PoolManager로 Instance에 등록
        Instance = this;

        // Scene이 바뀌어도 이 오브젝트는 파괴되지 않고 계속 유지되도록. Pooling된 오브젝트를 계속 관리해야 하므로 필수적
        DontDestroyOnLoad(gameObject);

        // 별도로 주입 안 하면 기본 생성
        // A ?? B -> (A != null) ? A : B
        // _resourcesManager가 null이라면 new ResourcesManager를 할당한다
        // null이 아니라면 아무것도 안함
        _resourcesManager ??= new ResourcesManager();
    }

    public void Initialize(ResourcesManager rm)
    {
        // rm이 null이 아니라면 rm을 그대로 넣고 null이라면 새로운 ResourcesManager를 할당함
        // 보호장치의 역할을 함
        _resourcesManager = rm ?? new ResourcesManager();
    }

    public Pool GetPool(string key, int size = -1)
    {
        // 만약 _poolMap 딕셔너리에 key에 해당하는 값을 찾아보고 있다면 existing에 값을 넣어 반환
        if(_poolMap.TryGetValue(key, out var existing)) return existing;

        // 실제 리소스 경로
        string path = string.Format(_prefabPath, key);

        // 완성된 경로에 있는 Prefab 원본을 불러와 prefab에 저장
        GameObject prefab = _resourcesManager.LoadResource<GameObject>(path);
        if (prefab == null) return null;

        // 풀링된 오브젝트를 담아둘 빈 게임 오브젝트를 생성.
        var parent = new GameObject($"Pool_{key}").transform;
        
        // Scene이 바뀌어도 파괴 안되도록
        DontDestroyOnLoad(parent.gameObject);

        var pool = new Pool(prefab, parent, size > 0 ? size : _defaultSize);
        _poolMap[key] = pool;
        return pool;
    }

    public GameObject GetFromPool(string key)
    {
        var pool = GetPool(key);
        return pool != null ? pool.Pop() : null;
    }

    public GameObject GetFromPool(string key, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        var pool = GetPool(key);
        return pool != null ? pool.Pop(pos, rot, parent) : null;

    }
}
