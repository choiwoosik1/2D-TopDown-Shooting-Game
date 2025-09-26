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

    /// <summary>
    /// 읽기 전용 Property들
    /// </summary>
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

        // Model 초기화
        _model.Initialize(_enemyData.Speed, _enemyData.CurrentHp, _enemyData.MaxHp, _enemyData.Damage);

        _collider.enabled = true;

        _mover = (EnemyMover)_monoBehavior;

        // 상태 객체 생성 및 테이블에 등록
        _states[(int)EnemyStateType.Idle] = new IdleState(this);
        _states[(int)EnemyStateType.Trace] = new TraceState(this);
        _states[(int)EnemyStateType.Combat] = new CombatState(this);
        _states[(int)EnemyStateType.Dead] = new DeadState(this);

        // 초기화 시 Idle 상태로
        _currentState = _states[(int)EnemyStateType.Idle];
        // 진입
        _currentState.Enter();

        // 이벤트 구독
        _model.OnDead += OnDead;

        // Weapon 초기화
        if (_weapon != null) 
        {
            _weapon.SetModel(_model);
        }


        if (_weapon is EnemyFiringWeapon efw) efw.SetTarget(_target);

        // 상태 판단 루프(생존 중에만 동작) 재시작 안전 처리
        if (_calculateStateRoutine != null)
        {
            StopCoroutine(_calculateStateRoutine);
            _calculateStateRoutine = null;
        }
        _calculateStateRoutine = StartCoroutine(CalculateStatRoutine());
    }

    void OnDisable()
    {
        // 이벤트 구독 해제
        _model.OnDead -= OnDead;

        // 코루틴 정지
        if (_calculateStateRoutine != null)
        {
            StopCoroutine(_calculateStateRoutine);
            _calculateStateRoutine = null;
        }
    }

    // 일정 간격(_thinkSpan)으로 상태 판단
    IEnumerator CalculateStatRoutine()
    {
        // Dead 상태이면 코루틴 종료
        while (_currentState.StateType != EnemyStateType.Dead)
        {
            // 현재 상황 분석 및 상태 전환
            CalculateState();
            yield return new WaitForSeconds(_thinkSpan);
        }
        _calculateStateRoutine = null;
    }

    private void Update()
    {
        // 아직 상태 준비 안되면 아무것도 하지 않음
        // 클래스간 Start, Update 시작 순서에 대한 오류 방지
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
        // x 속도의 부호로 좌/우를 결정
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

        // 타겟이 없으면 매우 먼 거리로 간주
        float dist = _target ? Vector2.Distance(transform.position, _target.position) : float.MaxValue;

        // 1) 타겟이 없거나 거리가 너무 멀면 Idle
        if (_target == null || dist > _traceDistance)
            ChangeState(EnemyStateType.Idle);

        // 추적 범위 내지만 공격 거리 밖이면 Trace
        else if (dist > _attackDistance)
            ChangeState(EnemyStateType.Trace);

        // 공격 거리 이내면 Combat
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
        // 쿨타임 아직 안됐으면 리턴
        if (Time.time < _nextAttack) return;
        _nextAttack = Time.time + _attackSpan;

        // 공격 애니메이션
        _anim.SetTrigger(AnimatorParameters.Fire);

        // 무기 발사
        if(_weapon != null) _weapon.Attack();
    }
    
    void OnDead()
    {
        // 상태 판단 루프 정지
        if(_calculateStateRoutine != null)
        {
            StopCoroutine(_calculateStateRoutine);
            _calculateStateRoutine = null;
        }

        // Dead 상태로 전환
        ChangeState(EnemyStateType.Dead);

        // 사망 애니메이션
        _anim.SetTrigger(AnimatorParameters.Death);

        // 피격/충돌 불가
        _collider.enabled = false;

        // 이동 정지
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
