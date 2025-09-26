using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 무기의 공통 기능을 담당하는 클래스
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    [Header("---- 스탯 데이터 ----")]
    [SerializeField] protected WeaponData _data;        // 무기 데이터

    protected float _bonusDamage;                            // 총알의 Bonus Damage
    protected float _bulletSpeed;                       // 총알 속도
    protected float _shootingRange;                     // 사정 거리
    protected float _bulletCount;                       // 총알 수
    protected int _attackCount;                         // 공격 횟수
    protected float _bulletDuration;                    // 총알 지속 시간
    protected float _fireDelay;                         // 연발 사이 시간 간격
    protected float _coolTimer;                         // 쿨타임

    // 무기의 최종 스탯을 저장하는 Dictionary
    // 스탯 종류를 Key로 사용하여 값을 찾을 수 있음
    Dictionary<WeaponStatType, float> _stats = new Dictionary<WeaponStatType, float>();

    protected virtual void Start()
    {
        Initialize();
    }

    /// <summary>
    /// WeaponData로부터 스탯을 받아와 초기화 하는 함수
    /// </summary>
    void Initialize()
    {
        if(_data == null)
        {
            Debug.LogError($"{gameObject.name}에 WeaponData가 할당되지 않았음");
            return;
        }

        // 새로운 계산을 위해 딕셔너리 초기화
        _stats.Clear();

        // WeaponData의 Stat 리스트를 순회하여 딕셔너리에 값 채우기
        foreach(var stat in _data.Stat)
        {
            // Add와 동일하나 조금 더 안전함.
            _stats[stat.StatType] = stat.Value;
            //_stats.Add(stat.StatType, stat.Value);
        }

        _bonusDamage = GetStat(WeaponStatType.BonusDamage);
        _bulletSpeed = GetStat(WeaponStatType.BulletSpeed);
        _bulletCount = GetStat(WeaponStatType.BulletCount);
        _shootingRange = GetStat(WeaponStatType.ShootingRange);
        _bulletCount = GetStat(WeaponStatType.BulletCount);
        _attackCount = Mathf.RoundToInt(GetStat(WeaponStatType.AttackCount));
        _bulletDuration = GetStat(WeaponStatType.BulletDuration);
        _fireDelay = GetStat(WeaponStatType.FireDelay);
        _coolTimer = GetStat(WeaponStatType.CoolTimer);
    }


    /// <summary>
    /// 딕셔너리에서 스탯 값을 가져오는 함수
    /// </summary>
    /// <param name="statType">가져올 스탯 종류</param>
    /// <returns>해당 스탯의 값을 가져옴. 만약 없다면 0을 리턴</returns>
    public float GetStat(WeaponStatType statType)
    {
        // 딕셔너리에 해당 Key가 있는지 확인하고 있으면 value 리턴
        if(_stats.TryGetValue(statType, out float vaule))
        {
            return vaule;
        }

        return 0.0f;
    }
}
