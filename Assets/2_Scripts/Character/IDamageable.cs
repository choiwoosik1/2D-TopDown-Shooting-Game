using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Damage를 입을 수 있는 객체들을 위한 interface
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// 체력이 변경될 때 발생하는 이벤트(Current, Max)
    /// </summary>
    event Action<float, float> OnHpChanged;

    /// <summary>
    /// 캐릭터가 사망될 때 발생하는 이벤트
    /// </summary>
    event Action OnDead;

    /// <summary>
    /// 피해를 입히는 함수
    /// </summary>
    /// <param name="damage">Damage 양</param>
    void TakeHit(float damage);

}
