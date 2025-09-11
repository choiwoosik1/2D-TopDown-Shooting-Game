using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 단일 prefab전용 Pool이며, Pop/Push로 대여/반납
/// </summary>
public class Pool
{
    // Pop으로 나가기 전 Prefab 보관 스택
    readonly Stack<GameObject> _pool;
    
    // 원본 Prefab
    readonly GameObject _prefab;

    // Stack을 보관하는 창고. 생성되는 prefab들은 _parent의 자식 오브젝트로 모아짐
    readonly Transform _parent;

    /// <summary>
    /// initialize만큼 CreatePoolObj 함수를 실행하여 prefab을 모아둠
    /// </summary>
    /// <param name="prefab">원본 Prefab</param>
    /// <param name="parent">위치</param>
    /// <param name="initialize">초기 수</param>
    public Pool(GameObject prefab, Transform parent, int initialize)
    {
        _prefab = prefab;
        _parent = parent;
        _pool = new Stack<GameObject>(initialize);

        for(int i = 0; i < initialize; i++)
        {
            CreatePoolObj();
        }
    }

    void CreatePoolObj()
    {
        // 원본 Prefab 복제해서 새 게임 오브젝트 생성
        // instantiate에 Object가 붙은 이유는 이 클래스는 Monobehavior를 상속받지 않기 때문임
        GameObject go = Object.Instantiate(_prefab);

        // 새 게임 오브젝트의 부모를 Pool의 부모로 설정
        // false인 이유는 불필요한 좌표 계산을 생략한 것(성능 미세하게 상승)
        go.transform.SetParent(_parent, false);

        // 새 게임 오브젝트 비활성화
        go.SetActive(false);

        // 새 게임 오브젝트에서 Poolable Component를 가져옴
        var poolable = go.GetComponent<Poolable>();

        // 만약 Poolable Component가 없으면 만듦
        if(poolable == null) poolable = go.AddComponent<Poolable>();

        // 생성된 Object(poolable)에게 해당 인스턴스(Pool) 출신인 것을 알리며
        // 나중에 Object가 스스로 어느 Pool로 돌아가야하는지 알 수 있게됨
        poolable.SetPool(this);

        _pool.Push(go);
    }

    /// <summary>
    /// Pool에서 게임 오브젝트를 대여 해주는 함수
    /// 만약 Pool이 비어있다면 새로 생성해서 반환
    /// </summary>
    public GameObject Pop()
    {
        GameObject go;

        // Pool에 남은 Game Object가 있는 경우
        if(_pool.Count > 0 )
        {
            go = _pool.Pop();
        }

        // 없는 경우 새로 Instantiate하여 빌려줌
        else
        {
            go = Object.Instantiate(_prefab);
            go.transform.SetParent(_parent, false);

            var p = go.GetComponent<Poolable>();
            if(p == null) p = go.AddComponent<Poolable>();
            p.SetPool(this);
        }

        // 대여 표시
        var poolable = go.GetComponent<Poolable>();
        poolable.MarkRent();

        // IPoolReset라는 인터페이스를 가진 모든 컴포넌트 찾은 후 초기화
        var resets = go.GetComponents<IPoolReset>();
        for (int i = 0; i < resets.Length; i++) resets[i].OnRent();

        // 준비가 끝난 오브젝트를 활성화 후 리턴
        go.SetActive(true);
        return go;
    }

    /// <summary>
    /// Pop()함수와 동일한 기능. 위치, 방향을 한 번에 설정
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject Pop(Vector3 pos, Quaternion rot, Transform parent)
    {
        var go = Pop();
        var t = go.transform;
        if (parent != null) t.SetParent(parent, false);

        // Prefab의 위치와 방향을 설정
        t.SetPositionAndRotation(pos, rot);
        return go;
    }

    /// <summary>
    /// Pool로 게임 오브젝트를 반납하는 함수
    /// </summary>
    /// <param name="go"></param>
    public void Push(GameObject go)
    {
        go.transform.SetParent(_parent);
        go.SetActive(false);
        _pool.Push(go);
    }
}
