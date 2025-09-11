using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// 적 캐릭터를 담당하는 클래스
/// 동작 - 공격, 이동, 피해 etc
/// </summary>

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("---- 타겟 ----")]
    [SerializeField] Transform _target;

    [Header("---- 컴포넌트 참조 ----")]
    [SerializeField] CombatCharacterModel _model;
    [SerializeField] Animator _anim;
    [SerializeField] Collider2D _collider;
    [SerializeField] PlayerData _enemyData;

    [Header("---- 이동 모듈 ----")]
    [SerializeField] EnemyRigidbody _monoBehavior;   // Inspector에 EnemyRigidbody 할당
    public EnemyMover _mover { get; private set; }

    [Header("---- 무기 ----")]
    [SerializeField] FiringWeapon _weapon;

    [Header("---- AI ----")]
    [SerializeField] float _thinkSpan;              // AI 판단 간격
    [SerializeField] float _roamDistance;           // 배회 거리
    [SerializeField] float _roamSpan;               // 배회 간격(초)
    [SerializeField] float _traceDistance;          // 추적 거리
    [SerializeField] float _attackDistance;         // 공격 거리
    [SerializeField] float _attackSpan;             // 공격 간격

    [Header("---- 사망 ----")]
    [SerializeField] float _deadDuration;

    // 상태 & 런타임
    public Vector2 HomePos { get; set; }
    public bool HomeSet { get; set; }
    float _nextRoam, _nextAttack;
    Vector2 _roamDir;

    public PlayerData EnemyData => _enemyData;
    public Transform Target => _target;
    public float RoamDistance => _roamDistance;
    public float RoamSpan => _roamSpan;
    public float AttackDistance => _attackDistance;


    /// <summary>
    /// 적 캐릭터 제거 이벤트 변수
    /// </summary>
    public event Action<Enemy> OnRemoved;

    /// <summary>
    /// 적 캐릭터 상체 객체들
    /// </summary>
    EnemyState[] _states = new EnemyState[(int)EnemyStateType.Count];

    /// <summary>
    /// 현재 상태
    /// </summary>
    EnemyState _currentState;

    /// <summary>
    /// 현재 속도
    /// </summary>
    float _currentSpeed;

    Coroutine _calculateStateRoutine;

    /// <summary>
    /// Enemy 컴포넌트 초기화 함수
    /// EnemySpawner에서 초기화
    /// </summary>
    public void Initialize(Transform target)
    {
        _target = target;

        _model.Initialize(_enemyData.Speed, _enemyData.CurrentHp, _enemyData.MaxHp, _enemyData.Damage);

        _collider.enabled = true;

        _mover = (EnemyMover)_monoBehavior;

        _states[(int)EnemyStateType.Idle] = new IdleState(this);
        _states[(int)EnemyStateType.Trace] = new TraceState(this);
        _states[(int)EnemyStateType.Combat] = new CombatState(this);
        _states[(int)EnemyStateType.Dead] = new DeadState(this);

        _currentState = _states[(int)EnemyStateType.Idle];
        _currentState.Enter();

        _model.OnDead += OnDead;

        if(_weapon != null) _weapon.SetModel(_model);

        if (_weapon is EnemyFiringWeapon efw) efw.SetTarget(_target);

        if (_calculateStateRoutine != null)
        {
            StopCoroutine(_calculateStateRoutine);
            _calculateStateRoutine = null;
        }
        _calculateStateRoutine = StartCoroutine(CalculateStatRoutine());
    }

    void OnDisable()
    {
        _model.OnDead -= OnDead;

        if (_calculateStateRoutine != null)
        {
            StopCoroutine(_calculateStateRoutine);
            _calculateStateRoutine = null;
        }
    }

    IEnumerator CalculateStatRoutine()
    {
        while (_currentState.StateType != EnemyStateType.Dead)
        {
            CalculateState();
            yield return new WaitForSeconds(_thinkSpan);
        }
        _calculateStateRoutine = null;
    }

    private void Update()
    {
        if(_currentState == null)
        {
            return;
        }

        _currentState.Update();

        // 현재 속도 및 애니메이션
        // _mover가 null이 아니라면 velocity에 접근하고, null이면 전체 결과 null로
        // velocity의 크기가 null이면 0.0f 사용
        _currentSpeed = _mover?.velocity.magnitude ?? 0.0f;
        _anim.SetFloat(AnimatorParameters.MoveSpeed, _currentSpeed);

        // 좌우 플립
        var vx = _mover?.velocity.x ?? 0.0f;
        if (MathF.Abs(vx) > Utils.Epsilon)
        {
            transform.localScale = new Vector3(MathF.Sign(vx), 1.0f, 1.0f);
        }
    }

    // ---- 상태 판단 ---- //
    void CalculateState()
    {
        if (_currentState.StateType == EnemyStateType.Dead) return;

        float dist = _target ? Vector2.Distance(transform.position, _target.position) : float.MaxValue;

        if (_target == null || dist > _traceDistance)
            ChangeState(EnemyStateType.Idle);

        else if (dist > _attackDistance)
            ChangeState(EnemyStateType.Trace);

        else
            ChangeState(EnemyStateType.Combat);
    }

    public void ChangeState(EnemyStateType stateType)
    {
        int stateIndex = (int)stateType;

        // 기존 상태가 새로 바꾸려는 상태와 같거나 Dead 상태일 경우 return
        if (_currentState.StateType == stateType || _currentState.StateType == EnemyStateType.Dead) return;

        // 없는 상태로 바꾸려는 경우 return
        else if (stateIndex < 0 || stateIndex >= _states.Length) return;

        // 기존 상태 종료
        _currentState.Exit();

        // 새 상태 적용
        _currentState = _states[stateIndex];

        // 새 상태 실행
        _currentState.Enter();
    }

    /// <summary>
    /// 공격을 실행하는 함수
    /// </summary>
    public void Attack()
    {
        if (Time.time < _nextAttack) return;
        _nextAttack = Time.time + _attackSpan;

        _anim.SetTrigger(AnimatorParameters.Fire);

        if(_weapon != null) _weapon.Attack();
    }
    
    void OnDead()
    {
        if(_calculateStateRoutine != null)
        {
            StopCoroutine(_calculateStateRoutine);
            _calculateStateRoutine = null;
        }

        ChangeState(EnemyStateType.Dead);
        _anim.SetTrigger(AnimatorParameters.Death);
        _collider.enabled = false;
        _mover?.Stop();
    }

    public IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(_deadDuration);
        Destroy(gameObject);
    }

    // 디버그
    private void OnDrawGizmosSelected()
    {
        // home이 안 잡혔으면 현재 위치 기준
        Vector3 center = Application.isPlaying && HomeSet? (Vector3)HomePos : transform.position;

        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(center, _roamDistance);
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, _traceDistance);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, _attackDistance);
        
    }
}
