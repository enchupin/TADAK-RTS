using System;

public abstract class BaseUnit {
    public string ID;
    public string OwnerID;
    public Race Race;
    public int Wood;
    public int Rock;
    public float moveSpeed;
    public float AttackSpeed;
    public float AttackDamage;
    public float MaxHP;
    public float CurrentHP;

    public event Action<BaseUnit> OnDead;

    // 이동 예정
    public virtual void Die() {
        OnDead?.Invoke(this);
    }

}