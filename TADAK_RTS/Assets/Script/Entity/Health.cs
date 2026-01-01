using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public float CurrentHp { get; protected set; }
    public float MaxHp { get; protected set; }

    // 체력이 변할 때 UI 등에 알릴 이벤트
    public System.Action<float, float> OnHpChanged;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public virtual void Initialize(float maxHp) {
        MaxHp = maxHp;
        CurrentHp = maxHp;
    }
    public virtual void TakeDamage(float amount) {
        CurrentHp -= amount;
        CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

        // UI에 알림
        OnHpChanged?.Invoke(CurrentHp, MaxHp);

        if (CurrentHp <= 0) Die();
    }

    protected abstract void Die();

}
