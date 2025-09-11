using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유니티의 InputManager 기반 사용자 입력을 처리하는 클래스
/// </summary>

public class InputManagerHandler : InputHandler
{
    public override event Action<Vector2> OnMoveInput;
    public override event Action OnDashInput;
    public override event Action OnFireInput;

    Vector2 _moveInput;

    private void Update()
    {
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        OnMoveInput?.Invoke(_moveInput);

        if (Input.GetMouseButtonDown(1))
        {
            OnDashInput?.Invoke();
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnFireInput?.Invoke();
        }
    }
}
    
