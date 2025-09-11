using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사용자 입력을 처리하는 추상 클래스

public abstract class InputHandler : MonoBehaviour
{
    /// <summary>
    /// 이동 입력 이벤트
    /// </summary>
    public abstract event Action<Vector2> OnMoveInput;

    /// <summary>
    /// 대쉬 입력 이벤트
    /// </summary>
    public abstract event Action OnDashInput;

    public abstract event Action OnFireInput;
}