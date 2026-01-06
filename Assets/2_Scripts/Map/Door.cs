using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prefab으로 사용할 Door 컴포넌트
/// Open/Close 상태별 Prefab을 전환하여 문 상태를 표현
/// </summary>
public class Door : MonoBehaviour
{
    [Header("---- Door 설정 ----")]
    [SerializeField] private EdgeDirection _direction;
    [SerializeField] private RoomType _roomType;
    
    [Header("---- Door 상태 ----")]
    [SerializeField] private bool _isOpen = false;
    [SerializeField] private bool _isLocked = false;

    [Header("---- Visual Prefab ----")]
    private GameObject _closedPrefab;
    private GameObject _openPrefab;
    
    private GameObject _closedInstance;
    private GameObject _openInstance;

    public EdgeDirection Direction => _direction;
    public RoomType RoomType => _roomType;
    public bool IsOpen => _isOpen;
    public bool IsLocked => _isLocked;

    /// <summary>
    /// Door 초기화 (생성 시 호출)
    /// </summary>
    public void Initialize(EdgeDirection direction, RoomType roomType, GameObject closedPrefab, GameObject openPrefab)
    {
        _direction = direction;
        _roomType = roomType;
        _closedPrefab = closedPrefab;
        _openPrefab = openPrefab;

        // 초기 상태는 닫힌 상태
        _isOpen = false;
        
        // 시각적 Prefab 생성
        SpawnVisuals();
        
        // 초기 상태 적용
        UpdateVisualState();
    }

    /// <summary>
    /// Open/Close 시각적 요소 생성
    /// </summary>
    private void SpawnVisuals()
    {
        // 닫힌 문 Prefab 생성
        if (_closedPrefab != null)
        {
            _closedInstance = Instantiate(_closedPrefab, transform);
            _closedInstance.transform.localPosition = Vector3.zero;
            _closedInstance.transform.localRotation = Quaternion.identity;
            _closedInstance.transform.localScale = Vector3.one;
            _closedInstance.name = "Door_Closed";
        }

        // 열린 문 Prefab 생성
        if (_openPrefab != null)
        {
            _openInstance = Instantiate(_openPrefab, transform);
            _openInstance.transform.localPosition = Vector3.zero;
            _openInstance.transform.localRotation = Quaternion.identity;
            _openInstance.transform.localScale = Vector3.one;
            _openInstance.name = "Door_Open";
        }
    }

    /// <summary>
    /// 현재 상태에 맞는 Visual만 활성화
    /// </summary>
    private void UpdateVisualState()
    {
        if (_closedInstance != null)
        {
            _closedInstance.SetActive(!_isOpen);
        }

        if (_openInstance != null)
        {
            _openInstance.SetActive(_isOpen);
        }
    }

    /// <summary>
    /// 문 열기
    /// </summary>
    public void Open()
    {
        if (_isLocked)
        {
            Debug.Log($"[Door] 문이 잠겨있습니다. ({_direction})");
            return;
        }
        
        _isOpen = true;
        UpdateVisualState();
    }

    /// <summary>
    /// 문 닫기
    /// </summary>
    public void Close()
    {
        _isOpen = false;
        UpdateVisualState();
    }

    /// <summary>
    /// 문 잠금
    /// </summary>
    public void Lock()
    {
        _isLocked = true;
        
        // 잠글 때는 자동으로 닫기
        if (_isOpen)
        {
            Close();
        }
    }

    /// <summary>
    /// 문 잠금 해제
    /// </summary>
    public void Unlock()
    {
        _isLocked = false;
    }

    /// <summary>
    /// 문 상태 토글
    /// </summary>
    public void Toggle()
    {
        if (_isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Scene에서 Door 방향 및 상태 표시 (디버깅용)
    /// </summary>
    private void OnDrawGizmos()
    {
        // 상태별 색상
        // 잠김(빨강) > 열림(초록) > 닫힘(노랑)
        Gizmos.color = _isLocked ? Color.red : (_isOpen ? Color.green : Color.yellow);
        
        Vector3 arrowDirection = Vector3.zero;
        switch (_direction)
        {
            case EdgeDirection.Up:
                arrowDirection = Vector3.up;
                break;
            case EdgeDirection.Down:
                arrowDirection = Vector3.down;
                break;
            case EdgeDirection.Left:
                arrowDirection = Vector3.left;
                break;
            case EdgeDirection.Right:
                arrowDirection = Vector3.right;
                break;
        }

        // 방향 화살표
        Gizmos.DrawRay(transform.position, arrowDirection * 0.5f);
        
        // 상태 표시 (크기로 구분)
        float size = _isLocked ? 0.3f : (_isOpen ? 0.25f : 0.2f);
        Gizmos.DrawWireSphere(transform.position, size);
    }
#endif
}
