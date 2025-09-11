using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] Transform _firePoint;

    float _firePointInitialX;
    bool _isFaceRight = true;

    void Start()
    {
        // �������� ���� ���� ���� �ʱ� X ��ġ�� ����
        if (_firePoint != null)
            _firePointInitialX = _firePoint.localPosition.x;
    }

    void Update()
    {
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool isLookRight = mousePoint.x > transform.position.x;

        if(isLookRight != _isFaceRight)
        {
            Flip(isLookRight);
        }

    }

    public void Flip(bool lookRight)
    {
        _isFaceRight = lookRight;

        _renderer.flipX = !lookRight;

        Vector3 firePoint = _firePoint.localPosition;

        firePoint.x = lookRight ? _firePointInitialX : -_firePointInitialX;

        _firePoint.localPosition = firePoint;
    }
}
