using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 캐릭터 Move Interface
/// </summary>
public interface EnemyMover
{
    Vector2 velocity { get; }

    void MoveDirection(Vector2 dir, float speed);

    void MoveTowards(Vector2 worldPos, float speed);

    void Stop();
}
