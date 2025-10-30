using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 방 유형
/// </summary>
public enum RoomType
{
    Regular,
    Item,
    Shop,
    Boss,
}

/// <summary>
/// 방 형태
/// </summary>
public enum RoomShape
{
    OneByOne,
    OneByTwo,
    TwoByOne,
    TwoByTwo,
    LShape,
}

public class Cell : MonoBehaviour
{
    public RoomType _roomType;
    public RoomShape _roomShape;

    [Header("---- 좌표 및 인덱스 ----")]
    public int index;
    public int Value;
    public List<int> _cellList = new List<int>();

    [Header("---- 미니맵 용 ----")]
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] SpriteRenderer _roomSprite;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    public SpriteRenderer RoomSprite => _roomSprite;

    public void SetSpecialRoomSprite(Sprite icon)
    {
        _spriteRenderer.sprite = icon;
    }

    public void SetRoomSprite(Sprite icon)
    {
        _roomSprite.sprite = icon;
    }

    public void SetRoomType(RoomType roomType)
    {
        _roomType = roomType;
    }

    public void SetMinimapVisible(bool visible)
    {
        if(_spriteRenderer) _spriteRenderer.enabled = visible;
        if(_roomSprite) _roomSprite.enabled = visible;
    }

    public void SetRoomShape(RoomShape roomShape)
    {
        _roomShape = roomShape;
    }

    public void RotateCell(List<int> connectedCells)
    {
        connectedCells.Sort();

        index = connectedCells[0];

        if(connectedCells.Contains(index + 1) && connectedCells.Contains(index + 10))
        {
            ApplyRotation(-90);
        }

        if(connectedCells.Contains(index + 1) && connectedCells.Contains(index + 11))
        {
            ApplyRotation(180);
        }

        if(connectedCells.Contains(index + 9) &&  connectedCells.Contains(index + 10))
        {
            ApplyRotation(90);
        }
    }

    /// <summary>
    /// L자형 방에 적용되는 회전 함수
    /// </summary>
    /// <param name="angle"></param>
    public void ApplyRotation(float angle)
    {
        // angle의 값만큼 z축으로 회전
        transform.rotation = Quaternion.Euler(0,0,angle);
    }
}