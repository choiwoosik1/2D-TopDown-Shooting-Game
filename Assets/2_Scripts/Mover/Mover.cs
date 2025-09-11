using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Mover : MonoBehaviour
{
    /// <summary>
    /// 이동 속력
    /// </summary>
    [SerializeField] protected float _moveSpeed;

    public abstract event UnityAction<Vector3> OnMoved;

    /// <summary>
    /// 방향대로 이동 시키는 함수
    /// </summary>
    /// <param name="direction"></param>
    public abstract void Move(Vector3 direction);

    public virtual void SetMoveSpeed(float MoveSpeed)
    {
        _moveSpeed = MoveSpeed;
    }
}
