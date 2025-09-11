using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolReset
{
    // Pool에서 꺼내올 때 호출
    void OnRent();

    // Pool로 반환 직전에 호출
    void OnReturn();
}
