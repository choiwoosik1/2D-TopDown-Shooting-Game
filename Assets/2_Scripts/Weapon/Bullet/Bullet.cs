using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float _damage;                         // 데미지
    [SerializeField] protected LayerMask _targetLayerMask;  // 충돌할 레이어
    

    /// <summary>
    /// 데미지를 설정하는 함수
    /// </summary>
    /// <param name="damage"></param>
    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 게임 오브젝트 Layer가 _targetLayerMask에 포함되면
        if (_targetLayerMask.Contains(collision.gameObject.layer))
        {
            // 충돌한 게임 오브젝트에서 IDamageable 인터페이스 가져오기
            IDamageable target = collision.GetComponentInParent<IDamageable>();

            // Enemy 컴포넌트가 존재한다면
            if(target != null)
            {
                Attack(target);
            }

        }
    }

    /// <summary>
    /// 적을 공격하는 함수
    /// </summary>
    /// <param name="enemy"></param>
    protected virtual void Attack(IDamageable target)
    {
        // 적이 데미지 입는 함수
        target.TakeHit(_damage);
    }
}
