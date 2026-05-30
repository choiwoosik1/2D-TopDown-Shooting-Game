using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Door Prefab 데이터 구조
/// 각 방향별로 Open/Close 상태의 Prefab을 가짐
/// </summary>
[System.Serializable]
public class DoorPrefabData
{
    [Tooltip("닫힌 상태의 Door Prefab")]
    public GameObject closedPrefab;
    
    [Tooltip("열린 상태의 Door Prefab")]
    public GameObject openPrefab;
}

/// <summary>
/// Door의 시각적 Prefab을 RoomType과 방향별로 관리하는 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "Door", menuName = "GameSettings/Map/Door")]
public class DoorScriptable : ScriptableObject
{
    [Header("---- Door 타입 ----")]
    public RoomType _roomType;

    [Header("---- 방향별 Door Prefab (Open/Close) ----")]
    public DoorPrefabData _upDoor = new DoorPrefabData();
    public DoorPrefabData _downDoor = new DoorPrefabData();
    public DoorPrefabData _leftDoor = new DoorPrefabData();
    public DoorPrefabData _rightDoor = new DoorPrefabData();

    /// <summary>
    /// 방향에 따른 Door Prefab 데이터 반환
    /// </summary>
    public DoorPrefabData GetDoorPrefabData(EdgeDirection direction)
    {
        switch (direction)
        {
            case EdgeDirection.Up:
                return _upDoor;
            case EdgeDirection.Down:
                return _downDoor;
            case EdgeDirection.Left:
                return _leftDoor;
            case EdgeDirection.Right:
                return _rightDoor;
            default:
                return null;
        }
    }

    /// <summary>
    /// 방향과 상태에 따른 Door Prefab 반환 (Legacy 지원)
    /// </summary>
    public GameObject GetDoorPrefab(EdgeDirection direction, bool isOpen = false)
    {
        var data = GetDoorPrefabData(direction);
        if (data == null) return null;
        
        return isOpen ? data.openPrefab : data.closedPrefab;
    }
}
