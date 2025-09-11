using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyRigidbody : MonoBehaviour, EnemyMover
{
    Rigidbody2D _rigid;

    // Rigid가 null인 오류 상황 방지
    public Vector2 velocity => _rigid ? _rigid.velocity : Vector2.zero;

    private void Awake() => _rigid = GetComponent<Rigidbody2D>();

    /// <summary>
    /// 지정된 방향으로 지정된 속도만큼 움직이는 함수
    /// </summary>
    /// <param name="dir">지정된 방향</param>
    /// <param name="speed">지정된 속도</param>
    public void MoveDirection(Vector2 dir, float speed)
    {
        _rigid.velocity = (dir.sqrMagnitude > Utils.Epsilon) ? dir.normalized * speed : Vector2.zero;
    }

    /// <summary>
    /// 특정 좌표를 향해서 지정된 속도로 움직이는 함수
    /// </summary>
    /// <param name="worldPos">특정 좌표</param>
    /// <param name="speed">지정된 속도</param>
    public void MoveTowards(Vector2 worldPos, float speed)
    {
        Vector2 to = worldPos - _rigid.position;
        _rigid.velocity = (to.sqrMagnitude > Utils.Epsilon) ? to.normalized * speed : Vector2.zero;
    }

    /// <summary>
    /// 멈추는 함수
    /// </summary>
    public void Stop()
    {
        _rigid.velocity = Vector2.zero;
    }
}
