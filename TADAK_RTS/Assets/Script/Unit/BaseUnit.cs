using System;

public interface IDead {
    // 유닛이 죽었을 때 외부에 알릴 이벤트
    event Action<BaseUnit> OnDead;
    void Die();
}
public abstract class BaseUnit : IOwnable, IDead {
    public string UnitName { get; set; }
    public string OwnerName { get; set; }
    public Race Race { get; set; }

    public event Action<BaseUnit> OnDead;
    public virtual void Die() {
        OnDead?.Invoke(this);
    }



    public bool IsOwnedBy(string name) => OwnerName == name;
}