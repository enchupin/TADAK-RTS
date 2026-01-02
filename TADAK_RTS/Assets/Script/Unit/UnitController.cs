using System;
using UnityEngine;


// 실제 유닛에 붙일 스크립트
public class UnitController : MonoBehaviour, ISelectable
{
    private UnitSound _unitSound;
    private UnitHealth _health;
    private IMovement _movement;
    public UnitJsonData _unitData;

    public string OwnerId;

    public event Action<UnitController> OnDead;

    // 이동 예정
    public void Die()
    {

    }
}
