using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBullet : ProjectileBullet
{
    [SerializeField] int _count;        // 총알의 공격 횟수

    public void SetCount(int count)
    {
        _count = count;
    }

    protected override void Attack(Enemy enemy)
    {
        base.Attack(enemy);

        // 공격 횟수 모두 차감시 제거
        _count--;
        if( _count <= 0)
        {
            //Destroy(gameObject);
            _poolable.ReturnToPool();
        }
    }

}
