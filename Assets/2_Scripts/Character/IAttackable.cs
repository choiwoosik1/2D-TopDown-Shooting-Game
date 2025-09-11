using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격 가능한 객체들을 위한 interface
/// </summary>
public interface IAttackable
{
    /// <summary>
    /// 지정된 대상에게 Damage를 입히는 함수
    /// </summary>
    /// <param name="damageable"></param>
    void Hit(IDamageable damageable);
}
