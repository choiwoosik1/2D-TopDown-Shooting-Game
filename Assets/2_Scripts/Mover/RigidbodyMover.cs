using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Rigidbody를 이용한 Mover Script


[RequireComponent(typeof(Rigidbody2D))]
public class RigidbodyMover : Mover
{
    public override event UnityAction<Vector3> OnMoved;

    [SerializeField] SpriteRenderer _renderer;

    Rigidbody2D _rigid;

    Vector3 _velocity;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rigid.velocity = _velocity;

        // 이벤트 알림
        OnMoved?.Invoke(_velocity);
    }

    public override void Move(Vector3 direction)
    {

        // 만약 속도의 크기가 작다면
        if(direction.magnitude < Utils.Epsilon)
        {
            // 현재 속도 0으로 설정
            _velocity = Vector3.zero;
        }
        else
        {
            // 현재 속도값 설정
            _velocity = direction.normalized * _moveSpeed;

            if(_velocity.x < 0)
            {
                _renderer.flipX = true;
            }
            else
            {
                _renderer.flipX = false;
            }
        }
    }

}
    
