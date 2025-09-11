using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/Weapon/WeaponData", fileName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("---- 정보 ----")]
    [SerializeField] string _weaponName;
    [TextArea(3, 5)][SerializeField] string _description;

    [Header("---- 기본 스탯 ----")]
    [SerializeField] List<WeaponStat> _stat;        // 무기 설명
    [SerializeField] Sprite _iconSprite;            // 무기 이미지

    public string WeapomName => _weaponName;
    public string Description => _description;
    public Sprite IconSprite => _iconSprite;
    public List<WeaponStat> Stat => _stat;
}
