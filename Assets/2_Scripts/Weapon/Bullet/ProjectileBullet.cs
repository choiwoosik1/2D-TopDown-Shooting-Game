using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 일정 방향을 날아가 적에게 닿으면 데미지를 입히는 클래스
/// </summary>

[RequireComponent(typeof(Poolable))]
public class ProjectileBullet : Bullet
{
    [SerializeField] float _speed;                  // 이동 속력
    [SerializeField] protected float _duration;     // 총알 지속 시간
    [SerializeField] float _knockBackForce;         // 밀려나는 힘

    Vector3 _dir;                                   // 이동 방향
    protected float _timer;                         // 타이머
    protected Poolable _poolable;

    protected virtual void Awake()
    {
        _poolable = GetComponent<Poolable>();
    }

    void OnEnable() => _timer = 0.0f;

    /// <summary>
    /// 이동 속력을 설정하는 함수
    /// </summary>
    /// <param name="speed"></param>
    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    /// <summary>
    /// 지속 시간을 설정하는 함수
    /// </summary>
    /// <param name="duration"></param>
    public void SetDuration(float duration) 
    {
        _duration = duration;
    }

    /// <summary>
    /// 이동 방향을 설정하는 함수
    /// </summary>
    /// <param name="dir"></param>
    public void SetDirection(Vector3 dir)
    {
        _dir = dir.normalized;

        // 총알이 앞을 보도록 회전
        transform.right = _dir;
    }

    // virtual로 설정하였을 때 자식 클래스에서 override 안하면 부모 클래스 함수 실행
    // 만약 자식 클래스에서 override 하였을 떄 부모 로직도 사용하고 싶다면
    // base.FixedUpdate
    protected virtual void FixedUpdate()
    {
        transform.Translate(_dir * _speed * Time.fixedDeltaTime, Space.World);

        // 타이머가 지속 시간보다 커지면 총알 오브젝트 파괴
        _timer += Time.fixedDeltaTime;
        if(_timer > _duration)
        {
            Despawn();
        }
    }

    protected void Despawn()
    {
        if(_poolable != null ) _poolable.ReturnToPool();
        else Destroy(gameObject);
    }

    protected override void Attack(IDamageable target)
    {
        base.Attack(target);

        // IDamageable은 interface일 뿐 게임 오브젝트 기능 X
        // target을 MonoBehavior 타입으로 바꾸기. Enemy나 Player는 MonoBehavior이기때문에 가능
        Rigidbody2D Rb = (target as MonoBehaviour)?.GetComponent<Rigidbody2D>();
        if(Rb != null)
        {
            Vector2 knockBackDir = (Rb.transform.position -transform.position).normalized;
            Rb.AddForce(knockBackDir * _knockBackForce, ForceMode2D.Impulse);
        }

        _poolable.ReturnToPool();

    }

}
