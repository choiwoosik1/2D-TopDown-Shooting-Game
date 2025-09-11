using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Mover _mover;
    [SerializeField] Dash _dash;
    [SerializeField] CombatCharacterModel _model;
    [SerializeField] FiringWeapon _weapon;
    [SerializeField] PlayerData _data;

    [Header("--- 애니메이션 관련 ---")]
    [SerializeField] Animator _animator;

    public void Initialize()
    {
        _weapon.SetModel(_model);
        _model.OnSpeedChanged += _mover.SetMoveSpeed;
        _model.Initialize(_data.Speed, _data.CurrentHp, _data.MaxHp, _data.Damage);

        _mover.OnMoved += OnMoved;
    }

    /// <summary>
    /// 방향대로 이동시키는 함수
    /// </summary>
    /// <param name="direction">방향</param>
    public void Move(Vector3 direction)
    {
        // 구르기 중일 때 Move를 return 시키면서
        // 구를 때마다 속도가 달랐던 버그를 방지
        if (_dash.IsRolling) return;

        _mover.Move(direction);
    }

    /// <summary>
    /// 움직이는 애니메이션 실행하는 함수
    /// </summary>
    /// <param name="velocity"></param>
    void OnMoved(Vector3 velocity)
    {
        _animator.SetFloat(AnimatorParameters.MoveSpeed, velocity.magnitude);
    }

    /// <summary>
    /// 마우스 방향으로 구르는 함수
    /// </summary>
    public void Dash()
    {
        _dash.Roll();
    }

    /// <summary>
    /// 마우스 방향으로 총을 쏘는 함수
    /// </summary>
    public void Fire()
    {
        _weapon.Attack();
    }

}
