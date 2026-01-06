using UnityEngine;

public class BaseEntity : MonoBehaviour, ISelectable_test {
    [SerializeField] protected EntityData data; // SO 할당
    protected bool isSelected = false;

    public EntityData GetData() => data;

    public virtual void OnSelect() {
        isSelected = true;
        // 외곽선(Outline) 효과 활성화 등
    }

    public virtual void OnDeselect() {
        isSelected = false;
        // 외곽선 효과 비활성화
    }

    public Transform GetTransform() => transform;
}