using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터 클래스(이동, 점프, 공격 등등 구현)

public class PlayScene : MonoBehaviour
{
    [Header("---- 컴포넌트 참조 ----")]
    [SerializeField] Player _player;
    [SerializeField] InputHandler _inputHandler;
    [SerializeField] Enemy _enemy;
    [SerializeField] Transform _target;

    private void Start()
    {
        _player.Initialize();
        _enemy.Initialize(_target);

        _inputHandler.OnMoveInput += OnMoveInput;
        _inputHandler.OnDashInput += OnDashInput;
        _inputHandler.OnFireInput += OnFireInput;
    }

    void OnMoveInput(Vector2 inputVec)
    {
        _player.Move(inputVec);
    }

    void OnDashInput()
    {
        _player.Dash();
    }

    void OnFireInput()
    {
        _player.Fire();
    }
}
