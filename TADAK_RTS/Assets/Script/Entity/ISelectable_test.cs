using UnityEngine;

public interface ISelectable_test {
    EntityData GetData(); // 데이터 반환
    void OnSelect();      // 선택되었을 때 실행 (예: 외곽선 활성화)
    void OnDeselect();    // 선택 해제되었을 때 실행
    Transform GetTransform();
}