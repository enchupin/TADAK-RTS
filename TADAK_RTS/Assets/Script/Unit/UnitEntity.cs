using UnityEngine;

public class UnitEntity : MonoBehaviour
{
    public BaseUnit Data { get; private set; }



    public void Init(BaseUnit unitData) {
        this.Data = unitData;

        // OnDead 구독
        if (this.Data is IDead deadable) {
            deadable.OnDead += (unit) => OnDestroyVisual();
        }
    }

    public void TakeDamage(float damage) {
        Data.TakeDamage(damage);
    }

    private void OnDestroyVisual() {
        Destroy(gameObject);
    }

}
