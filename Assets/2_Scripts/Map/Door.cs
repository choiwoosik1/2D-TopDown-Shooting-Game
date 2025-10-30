using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject _doorPrefab;

    public void SetPrefab(GameObject door)
    {
        _doorPrefab = door;

    }
}
