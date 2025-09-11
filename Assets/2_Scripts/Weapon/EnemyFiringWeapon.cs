using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 전용 원거리 무기 : 타겟 Transform을 향해 발사
/// Pooling/SpwanBullet 부분은 GunWeapon 그대로 재사용
/// 쿨타임/연사 로직은 FiringWeapon.Attack을 사용
/// 총알 스폰은 Pool 사용
/// 방향 계산은 타겟 기준
/// </summary>
public class EnemyFiringWeapon : GunWeapon
{
    [Header("---- Aim ----")]
    [SerializeField] Transform _target;

    public void SetTarget(Transform target) => _target = target;

    protected override Vector3 GetBulletDirection()
    {
        if (_target != null)
        {
            Vector3 origin = _firePoint ? _firePoint.position : transform.position;
            Vector2 dir = (Vector2)_target.position - (Vector2)origin;
            if(dir.sqrMagnitude > Utils.Epsilon) return dir.normalized;
        }

        return base.GetBulletDirection();
    }
}
