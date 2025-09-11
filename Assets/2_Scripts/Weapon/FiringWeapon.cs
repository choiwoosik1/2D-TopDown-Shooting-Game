using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 플레이어가 좌클릭을 사용해 무기를 발사하는 추상 클래스
/// </summary>

public abstract class FiringWeapon : Weapon
{
    [SerializeField] protected Transform _firePoint;
    [SerializeField] Animator _anim;
    
    protected CombatCharacterModel _model;
    float _lastAttackTime;

    protected override void Start()
    {
        base.Start();

        _lastAttackTime = -_coolTimer;      // 게임 시작 시 바로 쏠 수 있도록 초기화
    }

    /// <summary>
    /// 쿨타임 체크 후 공격 실행
    /// </summary>
    public void Attack()
    {
        // 쿨타임 안 지났으면 함수 바로 종료
        if (Time.time < _lastAttackTime + _coolTimer)
            return;

        // 쿨타임 통과 : 마지막 공격 시간을 현재 시간으로 갱신
        _lastAttackTime = Time.time;

        // 발사
        SpawnBullet();
        _anim.SetTrigger(AnimatorParameters.Fire);
    }

    /// <summary>
    /// 설정된 수만큼 총알을 연발로 발사하는 Coroutine
    /// </summary>
    /// <returns></returns>
    IEnumerator FireRoutine()
    {
        for (int i = 0; i < _bulletCount; i++)
        {
            SpawnBullet();

            // 연발 시간동안 대기
            yield return new WaitForSeconds(_fireDelay);
        }
    }

    protected abstract void SpawnBullet();

    protected virtual Vector3 GetBulletDirection()
    {
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mousePoint - transform.position;
        dir.Normalize();

        return dir;
    }

    public void SetModel(CombatCharacterModel model)
    {
        _model = model;
    }
}
