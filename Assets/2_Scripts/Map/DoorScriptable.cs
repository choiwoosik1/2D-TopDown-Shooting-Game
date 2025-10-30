using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Door", menuName = "GameSettings/Map/Door")]
public class DoorScriptable : ScriptableObject
{
    public RoomType _roomType;
    public GameObject _upDoor;
    public GameObject _downDoor;
    public GameObject _leftDoor;
    public GameObject _rightDoor;
}
