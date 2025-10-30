using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // 맵 생성에 필요한 private 변수들
    int[] _floorPlan;           // 맵 전체를 1D 배열로 저장하는 공간
    public int[] GetFloorPlan => _floorPlan;

    [SerializeField] Transform _cells;           // Cell Prefab을 담을 곳
    int _floorPlanCount;        // 지금까지 생성된 방
    int _minRooms;              // 최소 방 개수
    int _maxRooms;              // 최대 방 개수
    List<int> _endRooms;        // 현재 생성된 방들 중 끝 방의 인덱스를 담는 리스트
    List<int> _bigRoomIndexes;  // 큰 방들의 인덱스를 담는 리스트

    int _bossRoomIndex;         // 보스 방 셀의 인덱스
    int _shopRoomIndex;         // 상점 방 셀의 인덱스
    int _itemRoomIndex;         // 아이템 방 셀의 인덱스

    public Cell CellPrefab;           // 실제 화면에 찍어줄 방 Prefab
    float _cellSize;            // 셀 사이 간격
    Queue<int> _cellQueue;      // 단계적 방 생성 시 사용할 큐
    List<Cell> _spawnCells;     // Scene에 실제로 Instantiate한 방 오브젝트

    public List<Cell> GetSpawnCells => _spawnCells;

    [Header("---- 스프라이트 참조 ----")]
    [SerializeField] Sprite _item;
    [SerializeField] Sprite _shop;
    [SerializeField] Sprite _boss;

    [Header("---- 변형 방들 ----")]
    [SerializeField] Sprite _largeRoom;
    [SerializeField] Sprite _verticalRoom;
    [SerializeField] Sprite _horizontalRoom;
    [SerializeField] Sprite _LShapeRoom;

    [Header("---- 디버그/미니맵 ----")]
    [SerializeField] bool _debugMiniMap;        // true면 미니맵 표시 false면 비활성화

    // 다른 스크립트에서 MapGenerator.instance로 접근 가능
    public static MapGenerator instance;

    /// <summary>
    /// 방 모양에 대한 읽기전용 Static 리스트
    /// </summary>
    static readonly List<int[]> _roomShapes = new()
    {
        // 1x2 크기의 가로 방 모양
        new int[] {-1},
        new int[] {1},

        // 2x1 크기의 세로 방 모양
        new int[] {10},
        new int[] {-10},

        // L자 모양 방
        new int[] {1, 10},
        new int[] {1, 11},
        new int[] {10, 11},

        new int[] {9, 10},
        new int[] {-1, 9},
        new int[] {-1, 10},

        new int[] {1, -10},
        new int[] {1, -9},
        new int[] {-9, -10},

        new int[] {-1, -10},
        new int[] {-1, -11},
        new int[] {-10, -11},

        // 2x2 크기의 사각형 방 모양
        new int[] {1, 10, 11},
        new int[] {1, -9, -10},
        new int[] {-1, 9, 10},
        new int[] {-1, -10, -11}
    };

    private void Start()
    {
        instance = this;


        _minRooms = 7;
        _maxRooms = 12;
        _cellSize = 1f;

        _spawnCells = new();

        SetUpDugeon();
    }

    void SetUpDugeon()
    {
        for(int i = 0; i < _spawnCells.Count; i++)
        {
            Destroy(_spawnCells[i].gameObject);
        }

        _spawnCells.Clear();

        _floorPlan = new int[100];
        _floorPlanCount = 0;
        _cellQueue = new Queue<int>();
        _endRooms = new List<int>();
        _bigRoomIndexes = new List<int>();

        // 시작 Cell
        VisitCell(45);

        // 생성 루프
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        // Queue가 빌 때까지 반복
        while(_cellQueue.Count > 0)
        {
            int index = _cellQueue.Dequeue();

            int x = index % 10;

            // created가 true면 현재 방은 다른 방을 잇는 통로 방임을 의미
            bool created = false;

            // 왼쪽 Cell 확인(즉, x가 가장 왼쪽에 있는 Cell인지)
            if (x > 1) created |= VisitCell(index - 1);

            // 오른쪽 Cell 확인(즉, x가 가장 오른쪽에 있는 Cell인지)
            if (x < 9) created |= VisitCell(index + 1);

            // 위쪽 Cell 확인(즉, index가 20보다 크면 위로 올라가도 범위 안임)
            if (index > 20) created |= VisitCell(index - 10);

            // 아래쪽 Cell 확인(즉, index가 70보다 작으면 아래로 내려가도 범위 안임)
            if(index < 70) created |= VisitCell(index + 10);

            if (created == false)
            {
                _endRooms.Add(index);
            }
        }

        if (_floorPlanCount < _minRooms)
        {
            SetUpDugeon();
            return;
        }

        CleanEndRoomslist();

        SetUpSpecialRooms();
    }

    /// <summary>
    /// 최종 방 목록을 업데이트하여 실수로 큰 방이 포함되지 않도록 함
    /// 큰 방은 EndRoom은 될 수는 있지만 specialRoom이 되기는 원치 않음
    /// </summary>
    void CleanEndRoomslist()
    {
        _endRooms.RemoveAll(item => _bigRoomIndexes.Contains(item) || GetNeighbourCount(item) > 1);
    }

    void SetUpSpecialRooms()
    {
        _bossRoomIndex = _endRooms.Count > 0 ? _endRooms[_endRooms.Count - 1] : -1;

        if(_bossRoomIndex != -1)
        {
            // 중복 방지
            _endRooms.RemoveAt(_endRooms.Count - 1);
        }

        _itemRoomIndex = RandomEndRoom();
        _shopRoomIndex = RandomEndRoom();

        if (_itemRoomIndex == -1 || _shopRoomIndex == -1 || _bossRoomIndex == -1)
        {
            SetUpDugeon();
            return;
        }

        UpdateSpecialRoomVisuals();
        RoomManager.instance.SetUpRooms(_spawnCells);
    }

    void UpdateSpecialRoomVisuals()
    {
        foreach(var cell in _spawnCells)
        {
            if(cell.index == _itemRoomIndex)
            {
                cell.SetSpecialRoomSprite(_item);
                cell.SetRoomType(RoomType.Item);
            }

            if(cell.index == _shopRoomIndex)
            {
                cell.SetSpecialRoomSprite(_shop);
                cell.SetRoomType(RoomType.Shop);
            }

            if(cell.index == _bossRoomIndex)
            {
                cell.SetSpecialRoomSprite(_boss);
                cell.SetRoomType(RoomType.Boss);
            }
        }
    }

    int RandomEndRoom()
    {
        // _endRooms가 비어있다면 선택할 수 없으므로 -1 리턴
        if (_endRooms.Count == 0) return -1;

        // 0 ~ _endRooms.Count 범위 내 랜덤한 정수 뽑기
        int randomRoom = Random.Range(0, _endRooms.Count);

        // _endRooms 리스트에서 해당 index의 실제 방 번호를 가져옴
        int index = _endRooms[randomRoom];

        // 방 선택 이후, 중복 방지를 위해 리스트에서 제거
        _endRooms.RemoveAt(randomRoom);

        // 선택된 방 index 반환
        return index;
    }

    int GetNeighbourCount(int index)
    {
        // 상 하 좌 우에 몇 개의 방이 있는지 확인 후 return
        return _floorPlan[index - 10] + _floorPlan[index - 1] + _floorPlan[index + 1] + _floorPlan[index + 10];
    }

    /// <summary>
    /// 셀을 방문하는 함수
    /// 몇 가지 확인에서 성공하면 현재 Cell 설정 전에 false 반환
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    bool VisitCell(int index)
    {
        // 비어 있는 셀이거나 이웃 수가 1보다 크거나 현재 생성된 방이 _maxRooms보다 크거나 50% 확률 확인
        if (_floorPlan[index] != 0 || GetNeighbourCount(index) > 1 || _floorPlanCount > _maxRooms || Random.value < 0.5f) return false;

        // 30% 확률로 큰 방이 배치됨.
        if (Random.value < 0.3f && index != 45)
        {
            foreach(var shape in _roomShapes.OrderBy(_=> Random.value))
            {
                if(TryPlaceRoom(index, shape))
                {
                    // 루프 취소 후 배치. 큰 방이 우선적으로 적용됨
                    return true;
                }
            }
        }

        // 모든 if 통과 후 Queue에 데이터 넣기
        _cellQueue.Enqueue(index);

        // 해당 방의 index 1로 바꾸고 현재 방 개수 + 1
        _floorPlan[index] = 1;
        _floorPlanCount++;

        // 방 생성
        SpawnRoom(index);

        return true;
    }

    void SpawnRoom(int index)
    {
        int x = index % 10;
        int y = index / 10;

        Vector2 position = new Vector2(x * _cellSize, -y * _cellSize);

        Cell newCell = Instantiate(CellPrefab, position, Quaternion.identity, _cells);

        newCell.Value = 1;

        newCell.index = index;

        newCell.SetRoomType(RoomType.Regular);

        newCell.SetRoomShape(RoomShape.OneByOne);

        newCell._cellList.Add(index);

        if (_debugMiniMap)
        {
            // GameScene에 미니맵 Sprite 활성화
            newCell.SetMinimapVisible(true);
        }
        else
        {
            // 비활성화
            newCell.SetMinimapVisible(false);
        }

        _spawnCells.Add(newCell);
    }

    bool TryPlaceRoom(int origin, int[] offsets)
    {
        // 이번에 만들 방(큰 방)을 구성할 index들을 담는 임시 리스트
        List<int> currentRoomIndex = new List<int>() { origin };

        foreach(var offset in offsets)
        {
            // 각 오프셋(EX +1, +10, -1, -10)을 origin에 더해 실제 타일 index 계산
            int currentRoomChecked = origin + offset;

            // 후보 칸 위 혹은 아래가 맵 밖이면 배치 불가
            if(currentRoomChecked - 10 < 0 || currentRoomChecked + 10 >= _floorPlan.Length)
            {
                return false;
            }

            // 이미 점유된 방이라면 false
            if (_floorPlan[currentRoomChecked] != 0)
            {
                return false;
            }

            // 후보 칸이 기준 칸이라면 스킵
            if (currentRoomChecked == origin) continue;

            // 후보 칸이 가장 왼쪽에 있는 경우 스킵
            if (currentRoomChecked % 10 == 0) continue;

            // 검증이 통과된 후보 칸을 큰 방 구성 List에 추가
            currentRoomIndex.Add(currentRoomChecked);
        }

        // 큰 방을 구성할 후보 칸을 찾지 못한 경우 false
        if (currentRoomIndex.Count == 1) return false;

        // 최종 확정된 큰 방 구성 Cell들을 실제 맵에 commit
        foreach(int index in currentRoomIndex)
        {
            // 점유되었음을 표시
            _floorPlan[index] = 1;

            // 생성된 방 수 증가
            _floorPlanCount++;

            // _cellQueue에 등록
            _cellQueue.Enqueue(index);

            // 큰 방 index 목록에 추가
            _bigRoomIndexes.Add(index);
        }

        SpawnLargeRoom(currentRoomIndex);

        return true;
    }

    /// <summary>
    /// 큰 방(2x2, 1x2, 2x1, LShape)을 구성하는 셀들의 index 목록을 받아 하나의 prefab으로 배치
    /// </summary>
    /// <param name="largeRoomIndexs"></param>
    void SpawnLargeRoom(List<int> largeRoomIndexes)
    {
        // 생성한 Prefab 핸들
        Cell newCell = null;

        // 그리드 좌표(x, y)의 합(평균이나 중심 계산에 사용)
        int combinedX = 0, combinedY = 0;

        // 셀 중심으로 이동하기 위한 half-cell offset
        float offset = _cellSize / 2f;

        // 전달된 모든 셀 index 순회
        for (int i = 0; i < largeRoomIndexes.Count; i++)
        {
            // 1D -> 2D : 열 index
            int x = largeRoomIndexes[i] % 10;
            // 1D -> 2D : 행 index
            int y = largeRoomIndexes[i] / 10;

            // x, y 좌표 누적
            combinedX += x;
            combinedY += y;
        }

        // 2x2 형태의 객실
        if (largeRoomIndexes.Count == 4)
        {
            // 평균(정수 나눗셈으로 바닥값) * _cellSize + (0.5cell) -> 2x2 블록의 정확한 중심
            // y는 위에서 아래로 증가하도록 음수로 매핑
            Vector2 position = new Vector2(combinedX / 4 * _cellSize + offset, -combinedY / 4 * _cellSize - offset);

            // prefab 생성
            newCell = Instantiate(CellPrefab, position, Quaternion.identity,_cells);
            newCell.SetRoomSprite(_largeRoom);
            newCell.SetRoomShape(RoomShape.TwoByTwo);
        }

        // L 형태의 객실
        if (largeRoomIndexes.Count == 3)
        {
            // 2x2 박스의 중심에 맞추기 위해 평균 + half-cell
            Vector2 position = new Vector2(combinedX / 3 * _cellSize + offset, -combinedY / 3 * _cellSize - offset);

            newCell = Instantiate(CellPrefab, position, Quaternion.identity, _cells);
            newCell.SetRoomSprite(_LShapeRoom);
            newCell.SetRoomShape(RoomShape.LShape);

            // 누락된 코너 위치를 기반으로 올바른 방향으로 회전
            newCell.RotateCell(largeRoomIndexes);
        }

        // 1x2 혹은 2x1 형태의 객실
        if (largeRoomIndexes.Count == 2)
        {
            // 수직 인접
            if (largeRoomIndexes[0] + 10 == largeRoomIndexes[1] || largeRoomIndexes[0] - 10 == largeRoomIndexes[1])
            {
                // 수직 모양이기에 x는 두 셀이 같으므로 평균 그대로. y는 서로 다르므로 중앙이 반칸 -> -offset
                Vector2 position = new Vector2(combinedX / 2 * _cellSize, -combinedY / 2 * _cellSize - offset);

                newCell = Instantiate(CellPrefab, position, Quaternion.identity, _cells);
                newCell.SetRoomSprite(_verticalRoom);
                newCell.SetRoomShape(RoomShape.TwoByOne);
            }

            // 수평 인접
            if (largeRoomIndexes[0] + 1 == largeRoomIndexes[1] || largeRoomIndexes[0] - 1 == largeRoomIndexes[1])
            {
                // 수평 모양이기에 x는 중앙이 반칸 -> +offset, y는 두 셀이 같으므로 평균
                Vector2 position = new Vector2(combinedX / 2 * _cellSize + offset, -combinedY / 2 * _cellSize);

                newCell = Instantiate(CellPrefab, position, Quaternion.identity, _cells);
                newCell.SetRoomSprite(_horizontalRoom);
                newCell.SetRoomShape(RoomShape.OneByTwo);
            }
        }

        newCell._cellList = largeRoomIndexes;
        newCell._cellList.Sort();

        // 관리 / 참조 를 위해 리스트에 추가
        _spawnCells.Add(newCell);
    }


}
