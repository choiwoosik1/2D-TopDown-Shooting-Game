using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.ParticleSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Dash : MonoBehaviour
{
    [Header("--- 대쉬 ---")]
    [SerializeField] float _maxSpeed;               // 최고 속도
    [SerializeField] float _duration;               // 전체 구르기 시간
    [SerializeField] AnimationCurve _speedCurve;    // 시간에 따라 어떻게 변할지 지정
    [SerializeField] float _coolDown;               // 구르기 쿨 타임

    [Header("--- 효과 ----")]
    [SerializeField] Animator _anim;


    float _coolTimer;                               // 구르기 타이머


    Rigidbody2D _rigid;
    bool _isRolling = false;

    public bool IsRolling => _isRolling;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();

        // 곡선 그리기
        _speedCurve = new AnimationCurve(

            // 0초 일때 0
            new Keyframe(0f, 0f),

            // 0.2초 부근에 1.2
            new Keyframe(0.2f, 1.2f),

            // 1초에 다시 0
            new Keyframe(1f, 0f));
    }

    private void Update()
    {
        // 만약 쿨타임이 0보다 크면 0이 될 때까지 계속 줄임
        if (_coolTimer > Utils.Epsilon)
            _coolTimer -= Time.deltaTime;
    }

    public void Roll()
    {
        // 무한 구르기 불가
        if (_isRolling) return;

        if (_coolTimer > Utils.Epsilon) return;

        // 마우스 위치 벡터 변환
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mousePoint - transform.position;
        
        // 만약 마우스 위치가 현재 캐릭터와 너무 가깝다면 무시
        if (dir.magnitude < Utils.Epsilon * Utils.Epsilon) return;

        dir.Normalize();
        StartCoroutine(CoRoll(dir));
    }

    IEnumerator CoRoll(Vector2 dir)
    {
        // 코루틴 시작시 _isRolling true로 변환
        _isRolling = true;
        float t = 0.0f;

        _anim.SetTrigger(AnimatorParameters.Roll);

        while (t < _duration)
        {
            t += Time.deltaTime;
            float alpha = t / _duration;
            float speed = _maxSpeed * _speedCurve.Evaluate(alpha);

            // MovePosition으로 Rigid를 dir 방향으로 speed * Time.deltaTime만큼 이동시킴
            _rigid.MovePosition(_rigid.position + dir * speed * Time.deltaTime);
            
            // 프레임 단위
            yield return null;
        }

        _isRolling = false;
        _coolTimer = _coolDown;
        
    }
}
