using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Door 스폰 포인트 정보
/// Cell 인덱스와 해당 Cell 내에서의 상대 위치, 방향을 정의
/// </summary>
[System.Serializable]
public class DoorSpawnPoint
{
    [Tooltip("방의 Cell 리스트 내 인덱스 (0부터 시작)")]
    public int cellIndex;
    
    [Tooltip("해당 Cell 기준 Door의 로컬 위치 오프셋")]
    public Vector2 positionOffset;
    
    [Tooltip("Door의 방향")]
    public EdgeDirection direction;

    public DoorSpawnPoint(int cellIndex, Vector2 offset, EdgeDirection dir)
    {
        this.cellIndex = cellIndex;
        this.positionOffset = offset;
        this.direction = dir;
    }
}

/// <summary>
/// Room의 타입, 모양, Prefab Variation, Door 위치를 정의하는 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "Room", menuName = "GameSettings/Map/Room")]
public class RoomScriptable : ScriptableObject
{
    [Header("---- Room 기본 정보 ----")]
    public RoomType _roomType;
    public RoomShape _roomShape;

    [Header("---- Room 타일 패턴 ----")]
    [Tooltip("방이 차지하는 타일의 상대 패턴 (정규화된 인덱스)")]
    public int[] occupiedTiles;

    [Header("---- Room Prefab Variations ----")]
    [Tooltip("같은 모양/타입의 다양한 방 프리팹들 (랜덤 선택됨)")]
    public GameObject[] _roomVariations;

    [Header("---- Door 스폰 포인트 ----")]
    [Tooltip("이 방의 Door들이 스폰될 위치 정보 리스트")]
    public DoorSpawnPoint[] doorSpawnPoints;

#if UNITY_EDITOR
    /// <summary>
    /// Inspector에서 RoomShape에 따라 기본 Door 포인트 자동 생성 (에디터 전용)
    /// </summary>
    [ContextMenu("Generate Default Door Points")]
    private void GenerateDefaultDoorPoints()
    {
        List<DoorSpawnPoint> points = new List<DoorSpawnPoint>();

        switch (_roomShape)
        {
            case RoomShape.OneByOne:
                points.Add(new DoorSpawnPoint(0, new Vector2(0, 4f), EdgeDirection.Up));
                points.Add(new DoorSpawnPoint(0, new Vector2(0, -4f), EdgeDirection.Down));
                points.Add(new DoorSpawnPoint(0, new Vector2(-9.4f, 0), EdgeDirection.Left));
                points.Add(new DoorSpawnPoint(0, new Vector2(9.4f, 0), EdgeDirection.Right));
                break;

            case RoomShape.OneByTwo:
                // Cell A (index 0)
                points.Add(new DoorSpawnPoint(0, new Vector2(0f, 9.4f), EdgeDirection.Up));
                points.Add(new DoorSpawnPoint(0, new Vector2(-9.4f, 5.1f), EdgeDirection.Left));
                points.Add(new DoorSpawnPoint(0, new Vector2(9.4f, 5.1f), EdgeDirection.Right));
                // Cell B (index 1)
                points.Add(new DoorSpawnPoint(1, new Vector2(0f, -9.4f), EdgeDirection.Down));
                points.Add(new DoorSpawnPoint(1, new Vector2(-9.4f, -5.1f), EdgeDirection.Left));
                points.Add(new DoorSpawnPoint(1, new Vector2(9.4f, -5.1f), EdgeDirection.Right));
                break;

            case RoomShape.TwoByOne:
                // Cell A (index 0)
                points.Add(new DoorSpawnPoint(0, new Vector2(-10.8f, 4.21f), EdgeDirection.Up));
                points.Add(new DoorSpawnPoint(0, new Vector2(-20f, 0f), EdgeDirection.Left));
                points.Add(new DoorSpawnPoint(0, new Vector2(-10.8f, -4.21f), EdgeDirection.Down));
                // Cell B (index 1)
                points.Add(new DoorSpawnPoint(1, new Vector2(10.8f, 4.21f), EdgeDirection.Up));
                points.Add(new DoorSpawnPoint(1, new Vector2(10.8f, -4.21f), EdgeDirection.Down));
                points.Add(new DoorSpawnPoint(1, new Vector2(20f, 0f), EdgeDirection.Right));
                break;

            case RoomShape.TwoByTwo:
                // Cell A (index 0) - 좌상
                points.Add(new DoorSpawnPoint(0, new Vector2(-10.65f, 9.26f), EdgeDirection.Up));
                points.Add(new DoorSpawnPoint(0, new Vector2(-20f, 5.21f), EdgeDirection.Left));
                // Cell B (index 1) - 우상
                points.Add(new DoorSpawnPoint(1, new Vector2(10.65f, 9.26f), EdgeDirection.Up));
                points.Add(new DoorSpawnPoint(1, new Vector2(20f, 5.21f), EdgeDirection.Right));
                // Cell C (index 2) - 좌하
                points.Add(new DoorSpawnPoint(2, new Vector2(-10.65f, -9.26f), EdgeDirection.Down));
                points.Add(new DoorSpawnPoint(2, new Vector2(-20f, -5.21f), EdgeDirection.Left));
                // Cell D (index 3) - 우하
                points.Add(new DoorSpawnPoint(3, new Vector2(10.65f, -9.26f), EdgeDirection.Down));
                points.Add(new DoorSpawnPoint(3, new Vector2(20f, -5.21f), EdgeDirection.Right));
                break;

            case RoomShape.LShape:
                // L-Shape는 여러 변형이 있으므로 기본 패턴만 제공
                // 실제 사용 시 각 RoomScriptable별로 수동 조정 필요
                Debug.LogWarning("L-Shape는 다양한 변형이 있어 자동 생성이 제한됩니다. 수동으로 조정하세요.");
                points.Add(new DoorSpawnPoint(0, new Vector2(-10.64f, 9.41f), EdgeDirection.Up));
                points.Add(new DoorSpawnPoint(0, new Vector2(-20f, 5f), EdgeDirection.Left));
                points.Add(new DoorSpawnPoint(1, new Vector2(10.64f, 9.41f), EdgeDirection.Up));
                points.Add(new DoorSpawnPoint(1, new Vector2(20f, 5.26f), EdgeDirection.Right));
                points.Add(new DoorSpawnPoint(2, new Vector2(-10.64f, -9.41f), EdgeDirection.Down));
                break;
        }

        doorSpawnPoints = points.ToArray();
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[{name}] {points.Count}개의 기본 Door 포인트가 생성되었습니다.");
    }
#endif
}
