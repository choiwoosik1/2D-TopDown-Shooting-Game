using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Combat(전투)가 가능한 캐릭터들의 모델 클래스
/// HP, 공격력, 방어력 등 Runtime Data 관리
/// </summary>
public class CombatCharacterModel : MonoBehaviour, IDamageable, IAttackable
{
    [Header("---- 이동 ----")]
    [SerializeField] float _moveSpeed;

    [Header("---- 피격 ----")]
    [SerializeField] float _currentHp;
    [SerializeField] float _maxHp;

    [Header("---- 공격 ----")]
    [SerializeField] float _damage;

    public float Damage => _damage;

    public event Action<float, float> OnHpChanged;
    public event Action OnDead;
    public event Action<float> OnSpeedChanged;

    public void Initialize(float moveSpeed, float currentHp, float maxHp, float damage)
    {
        _moveSpeed = moveSpeed;
        _currentHp = currentHp;
        _maxHp = maxHp;
        _damage = damage;

        OnHpChanged?.Invoke(_currentHp, _maxHp);
        OnSpeedChanged?.Invoke(_moveSpeed);
    }

    public void Hit(IDamageable damageable)
    {
        damageable.TakeHit(_damage);
    }

    public void TakeHit(float damage)
    {
        // Damage에 따른 체력 계산
        _currentHp = Mathf.Max(_currentHp - damage, 0);

        // 체력 변경 이벤트 발행
        OnHpChanged?.Invoke(_currentHp, _maxHp);

        // 사망 시 이벤트 발행
        if(_currentHp <= 0)
        {
            OnDead?.Invoke();
        }
    }

    /// <summary>
    /// 체력을 회복 하는 함수
    /// </summary>
    /// <param name="amount">회복량</param>
    public void Heal(float amount)
    {
        // 회복량 계산
        amount = Mathf.Max(amount, 0);

        // amount에 따른 체력 계산
        _currentHp = Mathf.Min(_currentHp + amount, _maxHp);

        // 체력 변경 이벤트 발행
        OnHpChanged?.Invoke(_currentHp, _maxHp);
    }

    /// <summary>
    /// 최대 체력 증가 함수
    /// </summary>
    /// <param name="amount">최대 체력 증가량</param>
    public void AddMaxHp(float amount)
    {
        // 최대 체력 증가
        _maxHp += amount;

        // 최대 체력 증가에 따른 현재 체력 변경
        _currentHp = Mathf.Max(_currentHp, _currentHp + amount);

        // 체력 변경 이벤트 발행
        OnHpChanged.Invoke(_currentHp, _maxHp);
    }

    /// <summary>
    /// Damage 증가 함수
    /// </summary>
    /// <param name="amount"></param>
    public void AddDamage(float amount)
    {
        _damage += amount;
    }

    
}
