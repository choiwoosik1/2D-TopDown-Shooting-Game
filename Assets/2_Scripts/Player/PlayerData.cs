using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PlayerData", menuName = "GameSettings/Hero/HeroData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] float _maxHp;          // 최대 체력
    [SerializeField] float _currentHp;      // 현재 체력
    [SerializeField] float _speed;          // 이동 속도
    [SerializeField] float _damage;         // 캐릭터 기본 공격력

    public float MaxHp => _maxHp;
    public float CurrentHp => _currentHp;
    public float Speed => _speed;
    public float Damage => _damage;
}
