using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적에게 GunBullet을 발사하는 클래스
/// </summary>
public class GunWeapon : FiringWeapon
{
    //[Header("---- 총알 Prefab ----")]
    //[SerializeField] GunBullet _bulletPrefab;

    [Header("---- 풀링 key ----")]
    [SerializeField] string _prefabPath = "Arrow";

    [Header("---- 타겟 감지 ----")]
    [SerializeField] LayerMask _layerMask;

    PoolManager _poolManager;

    protected override void Start()
    {
        base.Start();
        _poolManager = PoolManager.Instance;
        if (_poolManager == null) Debug.LogError("PoolManager를 찾을 수 없음");
    }

    protected override void SpawnBullet()
    {
        //GunBullet bullet = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
        GameObject go = _poolManager.GetFromPool(_prefabPath, _firePoint.position, _firePoint.rotation);
        if (go == null) return;

        var bullet = go.GetComponent<ProjectileBullet>();

        // FinalDamage 계산(기본 캐릭터 공격력 + 무기)
        // 기본 캐릭터 공격력
        float playerDamage = _model.Damage;

        // 무기 공격력
        float weaponDamage = _bonusDamage;

        // 최종 공격력
        float finalDamage = playerDamage + weaponDamage;


        // 초기화
        bullet.SetDamage(finalDamage);
        bullet.SetDirection(GetBulletDirection());
        bullet.SetDuration(_bulletDuration);
        bullet.SetSpeed(_bulletSpeed);

        // 공격횟수가 필요없는 단발 Weapon
        // bullet.SetCount(_attackCount);
    }
}
