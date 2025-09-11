using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy 상태 패턴(State Pattern)

public enum EnemyStateType
{
    Idle,
    Trace,
    Combat,
    Dead,
    Count           // 상태 종료 카운트 수
}

public abstract class EnemyState
{
    protected Enemy _enemy;
    
    /// <summary>
    /// 상태 종류를 반환
    /// </summary>
    public abstract EnemyStateType StateType { get; }

    /// <summary>
    /// Enemy 상태 객체 생성자
    /// </summary>
    /// <param name="enemy">상태를 적용할 Enemy 객체</param>
    public EnemyState(Enemy enemy)
    {
        _enemy = enemy;
    }
    
    /// <summary>
    /// 상태 진입 시 호출되는 함수
    /// </summary>
    public abstract void Enter();
    
    /// <summary>
    /// 상태 유지 시 매 프레임 호출되는 함수
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// 상태 종료 시 호출되는 함수
    /// </summary>
    public abstract void Exit();
}

public class IdleState : EnemyState
{
    float _nextTurn;
    Vector2 _dir;

    public IdleState(Enemy enemy) : base(enemy)
    {
    }

    public override EnemyStateType StateType => EnemyStateType.Idle;

    public override void Enter()
    {
        _enemy._mover?.Stop();
        _nextTurn = 0.0f;

        if (!_enemy.HomeSet)
        {
            _enemy.HomePos = _enemy.transform.position;
            _enemy.HomeSet = true;
        }
    }

    public override void Update()
    {
        Vector2 pos = _enemy.transform.position;
        Vector2 toCenter = _enemy.HomePos - pos;
        
        float dist =toCenter.magnitude;
        float R = _enemy.RoamDistance;
        float innerBand = Mathf.Max(0.2f, R - 0.5f);

        // 반경 밖이면 센터 복귀
        if(dist > R)
        {
            _enemy._mover?.MoveDirection(toCenter.normalized, _enemy.EnemyData.Speed);
            return;
        }

        // 랜덤 방향 전환
        if(Time.time >= _nextTurn)
        {
            _dir = Random.insideUnitCircle.normalized;
            _nextTurn = Time.time + _enemy.RoamSpan;
        }

        // 경계 밴드에서는 바깥 방향 금지 (안쪽 Or 접선 성분만)
        if(dist > innerBand)
        {
            Vector2 inWard = toCenter.normalized;   // 안쪽
            Vector2 outWard = -inWard;              // 바깥쪽

            // 바깥 방향 성분이면 안쪽으로 꺾기
            if (Vector2.Dot(_dir, outWard) > Utils.Epsilon)
                _dir = inWard;
        }

        _enemy._mover?.MoveDirection(_dir, _enemy.EnemyData.Speed);
    }

    public override void Exit()
    {
        _enemy._mover?.Stop();
    }


}

public class TraceState : EnemyState
{
    public TraceState(Enemy enemy) : base(enemy)
    {
    }

    public override EnemyStateType StateType => EnemyStateType.Trace;

    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        _enemy._mover?.Stop();
    }

    public override void Update()
    {
        if (!_enemy.Target) { _enemy.ChangeState(EnemyStateType.Idle); return; }
        _enemy._mover?.MoveTowards(_enemy.Target.position, _enemy.EnemyData.Speed);
    }
}

public class CombatState : EnemyState
{
    public CombatState(Enemy enemy) : base(enemy)
    {
    }

    public override EnemyStateType StateType => EnemyStateType.Combat;

    public override void Enter()
    {
        _enemy._mover?.Stop();
    }

    public override void Exit()
    {
        _enemy._mover?.Stop();
    }

    public override void Update()
    {
        if (!_enemy.Target) { _enemy.ChangeState(EnemyStateType.Idle); return; }

        Vector2 me = _enemy.transform.position;
        Vector2 toT = (Vector2)_enemy.Target.position - me;
        float d = toT.magnitude;

        // 공격 거리 밖 -> 접근 / 이내 정지 후 사격
        if(d > _enemy.AttackDistance)
        {
            _enemy._mover?.MoveDirection(toT.normalized, _enemy.EnemyData.Speed);
        }

        else
        {
            _enemy._mover?.Stop();
            _enemy.Attack();
        }
    }
}

public class DeadState : EnemyState
{
    public DeadState(Enemy enemy) : base(enemy)
    {
    }

    public override EnemyStateType StateType => EnemyStateType.Dead;

    public override void Enter()
    {
        _enemy.StartCoroutine(_enemy.DelayedDestroy());
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
    }
}