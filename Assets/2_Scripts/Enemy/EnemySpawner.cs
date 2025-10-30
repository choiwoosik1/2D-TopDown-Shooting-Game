using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 1~3명의 랜덤한 Enemy 생성
/// 
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("---- PoolManager----")]
    [SerializeField] PoolManager _poolManager;

    [Header("---- 적 생성 ----")]
    [SerializeField] string _enemyPrefabPath;
    [SerializeField] Transform _target;
    [SerializeField] int _maxSpawnCount;
    [SerializeField] Vector2 _spawnRange;

    [Header("---- 적 생성 목록(읽기 전용) ----")]
    [SerializeField] List<Enemy> _enemies = new();

    bool _alreadySpawned;
    bool _roomCleared;

    private void Start()
    {
        _poolManager = PoolManager.Instance;
    }

    void OnDisable()
    {
        // 이벤트 정리
        for(int i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i] != null) _enemies[i].OnRemoved -= OnEnemyRemoved;
        }
        _enemies.Clear();
    }

    public void Spawn()
    {
        if (_alreadySpawned || _roomCleared) 
        {
            return; 
        }

        // string.IsNullOrWhiteSpace : 문자열이 null이거나 공백로 이루어져있는지 확인
        if (_poolManager == null || string.IsNullOrWhiteSpace(_enemyPrefabPath)) 
        {
            return; 
        }

        if(_target == null) return;

        _alreadySpawned = true;

        // 1~3 랜덤한 수의 적 생성
        int spawnCount = Random.Range(1, _maxSpawnCount + 1);

        for(int i = 0; i < spawnCount; i++)
        {
            // _target 기준 Position 설정
            Vector2 pos2 = (Vector2)_target.position +
                new Vector2(Random.Range(-_spawnRange.x, _spawnRange.x), Random.Range(-_spawnRange.y, _spawnRange.y));

            var pos = new Vector3(pos2.x, pos2.y, _target.position.z);

            var go = _poolManager.GetFromPool(_enemyPrefabPath, pos, Quaternion.identity, transform);

            if (go == null) return;

            var enemy = go.GetComponent<Enemy>();

            if (enemy == null) return;

            enemy.Initialize(_target);
            _enemies.Add(enemy);
            enemy.OnRemoved += OnEnemyRemoved;

        }
    }

    void OnEnemyRemoved(Enemy enemy)
    {
        if(enemy != null) enemy.OnRemoved -= OnEnemyRemoved;
        _enemies.Remove(enemy);

        // 모든 적 제거시 방 클리어 -> 재생성 X
        if(_enemies.Count == 0)
        {
            _roomCleared = true;
        }
    }
}
