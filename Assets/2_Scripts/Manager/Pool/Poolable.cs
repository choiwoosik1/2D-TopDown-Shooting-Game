using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pool에서 나온 객체가 자기 Pool로 되돌아가게 할 수 있는 Component
/// </summary>
public class Poolable : MonoBehaviour
{
    Pool _pool;
    public bool InPool { get; private set; } = false;

    // 람다 형식으로 축약한 것
    // public void Setpool(Pool pool){_pool = pool}과 같은 의미임
    public void SetPool(Pool pool) => _pool = pool;

    // Pool에서 Pop된 후 Active State로 표시
    public void MarkRent() => InPool = false;

    public void ReturnToPool()
    {
        if(_pool != null && !InPool)
        {
            InPool = true;

            // 반환 직전 정리
            var resets = GetComponents<IPoolReset>();    
            for (int i = 0; i < resets.Length; i++)
            {
                resets[i].OnReturn();
            }
            _pool.Push(gameObject);
        }

        else if(_pool == null)
        {
            // 예외 상황 : Pool 정보가 없으면 안전하게 파괴
            Object.Destroy(gameObject);
        }
    }
}
