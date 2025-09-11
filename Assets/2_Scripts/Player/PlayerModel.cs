using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [Header("--- 설정 데이터 ---")]
    [SerializeField] PlayerData _data;

    [Header("---- 스탯 ----")]
    [SerializeField] float _maxHp;
    [SerializeField] float _currentHp;
    [SerializeField] float _speed;

    // 체력 변경 이벤트
    public event Action<float, float> OnHpChanged;

    // 이동 속도 변경 이벤트
    public event Action<float> OnSpeedChanged;

    // 사망 이벤트
    public event Action OnDeath;

    public float MaxHp => _maxHp;
    public float CurrentHp => _currentHp;
    public float Speed => _speed;

    public void Initialize()
    {
        _maxHp = _data.MaxHp;
        _currentHp = _maxHp;
        _speed = _data.Speed;

        OnHpChanged?.Invoke(_currentHp, _maxHp);
        OnSpeedChanged?.Invoke(_speed);
    }
}
