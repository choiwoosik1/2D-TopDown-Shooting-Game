using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager
{
    private readonly Dictionary<string, Object> _cache = new();

    public T LoadResource<T>(string path) where T : Object
    {
        if (_cache.TryGetValue(path, out var obj)) return obj as T;

        var loaded = Resources.Load<T>(path);
        if (loaded != null) _cache[path] = loaded;
        else Debug.LogError($"[ResourcesManager]를 찾을 수 없음 Resources/{path}");
        return loaded;
    }

    public void ClearCache() => _cache.Clear();
}
