using System;
public interface IDamaged {
    float MaxHp { get; }
    float CurrentHp { get; }
    void TakeDamage(float damage);
}


public interface IDead : IDamaged {
    // 유닛이 죽었을 때 외부에 알릴 이벤트
    event Action<BaseUnit> OnDead;
    void Die();
}
public abstract class BaseUnit : IOwnable, IDead {
    public string UnitName { get; set; }
    public string OwnerName { get; set; }
    public float MaxHp { get; protected set; }
    public float CurrentHp { get; protected set; }

    public Race Race { get; set; }

    public event Action<BaseUnit> OnDead;
    public virtual void Die() {
        OnDead?.Invoke(this);
    }


    public virtual void TakeDamage(float damage) {
        if (CurrentHp <= 0) return;

        CurrentHp = MathF.Max(0, CurrentHp - damage);

        if (CurrentHp <= 0) {
            Die();
        }



    }



    public bool IsOwnedBy(string name) => OwnerName == name;
}