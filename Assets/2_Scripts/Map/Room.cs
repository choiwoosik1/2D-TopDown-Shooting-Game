using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EdgeDirection
{
    Up,
    Down,
    Left,
    Right,
}

/// <summary>
/// Room 인스턴스를 관리하고 Door를 데이터 기반으로 생성하는 클래스
/// </summary>
public class Room : MonoBehaviour
{
    public GameObject _prefab;

    /// <summary>
    /// Room 설정 (Prefab 생성 및 Door 배치)
    /// </summary>
    public void SetUpRoom(Cell currentCell, RoomScriptable roomData)
    {
        // 랜덤한 방 Variation 선택
        var prefab = roomData._roomVariations[Random.Range(0, roomData._roomVariations.Length)];

        var spawnedRoom = Instantiate(prefab, transform);
        spawnedRoom.transform.localPosition = Vector3.zero;
        spawnedRoom.transform.localRotation = Quaternion.identity;
        spawnedRoom.transform.localScale = Vector3.one;

        // 데이터 기반 Door 생성
        SetUpDoorsFromData(currentCell, roomData);
    }

    /// <summary>
    /// RoomScriptable의 doorSpawnPoints 데이터를 기반으로 Door 생성
    /// </summary>
    private void SetUpDoorsFromData(Cell currentCell, RoomScriptable roomData)
    {
        var floorPlan = MapGenerator.instance.GetFloorPlan;
        var cellList = MapGenerator.instance.GetSpawnCells;

        // 각 DoorSpawnPoint에 대해 Door 생성 시도
        foreach (var spawnPoint in roomData.doorSpawnPoints)
        {
            // Cell 인덱스 유효성 검사
            if (spawnPoint.cellIndex < 0 || spawnPoint.cellIndex >= currentCell._cellList.Count)
            {
                Debug.LogWarning($"[Room] Invalid cell index {spawnPoint.cellIndex} in {roomData.name}");
                continue;
            }

            int fromCellIndex = currentCell._cellList[spawnPoint.cellIndex];

            TryPlaceDoor(
                fromCellIndex,
                spawnPoint.positionOffset,
                spawnPoint.direction,
                floorPlan,
                cellList,
                currentCell
            );
        }
    }

    /// <summary>
    /// 지정된 위치에 Door 배치 시도 (인접한 방이 있을 경우에만)
    /// </summary>
    void TryPlaceDoor(int fromIndex, Vector2 positionOffset, EdgeDirection direction, int[] floorPlan, List<Cell> cellList, Cell currentCell)
    {
        int neighbourIndex = fromIndex + GetOffset(direction);

        // 경계 체크
        if (neighbourIndex < 0 || neighbourIndex >= floorPlan.Length) return;

        // 인접 셀이 방으로 채워져 있는지 확인
        if (floorPlan[neighbourIndex] != 1) return;

        // 인접한 Cell 찾기
        var foundCell = cellList.FirstOrDefault(x => x._cellList.Contains(neighbourIndex));
        if (foundCell == null) return;

        // Door 타입 결정 (현재 방이 Regular면 인접 방 타입, 아니면 현재 방 타입)
        RoomType doorRoomType = currentCell._roomType == RoomType.Regular ? foundCell._roomType : currentCell._roomType;

        // Door Prefab 생성
        var doorInstance = Instantiate(RoomManager.instance._doorPrefab, transform);
        doorInstance.transform.position = (Vector2)transform.position + positionOffset;

        // Door ScriptableObject에서 Prefab 데이터 가져오기
        var doorScriptable = GetDoorScriptable(doorRoomType);
        if (doorScriptable == null)
        {
            Debug.LogWarning($"[Room] No DoorScriptable found for RoomType: {doorRoomType}");
            Destroy(doorInstance.gameObject);
            return;
        }

        var prefabData = doorScriptable.GetDoorPrefabData(direction);
        if (prefabData == null)
        {
            Debug.LogWarning($"[Room] No DoorPrefabData found for Direction: {direction}");
            Destroy(doorInstance.gameObject);
            return;
        }

        // Door 초기화 (Closed/Open Prefab 전달)
        doorInstance.Initialize(direction, doorRoomType, prefabData.closedPrefab, prefabData.openPrefab);
    }

    /// <summary>
    /// RoomType에 맞는 DoorScriptable 반환
    /// </summary>
    DoorScriptable GetDoorScriptable(RoomType roomType)
    {
        return RoomManager.instance._doors.FirstOrDefault(x => x._roomType == roomType);
    }

    /// <summary>
    /// 방향에 따른 1D 배열 오프셋 계산
    /// </summary>
    int GetOffset(EdgeDirection direction)
    {
        switch (direction)
        {
            case EdgeDirection.Up:
                return -10;

            case EdgeDirection.Down:
                return 10;

            case EdgeDirection.Left:
                return -1;

            case EdgeDirection.Right:
                return 1;
        }

        return 0;
    }
}
