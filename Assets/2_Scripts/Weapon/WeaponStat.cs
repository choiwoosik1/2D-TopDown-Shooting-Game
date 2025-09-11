using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무기 스탯 종류
/// </summary>
public enum WeaponStatType
{
    BonusDamage,                 // 데미지
    BulletSpeed,            // 총알 속도
    ShootingRange,          // 사정 거리
    BulletCount,            // 총알 수
    AttackCount,            // 공격 횟수
    BulletDuration,        // 총알 지속 시간
    FireDelay,              // 연발 사이 시간 간격
    CoolTimer               // 쿨타임

}

/// <summary>
/// 하나의 스탯 종류에 수치를 정하는 클래스
/// </summary>
[System.Serializable]       // 객체의 변수들을 Inspector View에서 설정하게 해줌
public class WeaponStat
{
    [SerializeField] WeaponStatType _statType;       // 어떤 스탯에 대한 데이터인지
    [SerializeField] float _value;                   // 해당 스탯에 대한 값

    public WeaponStatType StatType => _statType;
    public float Value => _value;

}
