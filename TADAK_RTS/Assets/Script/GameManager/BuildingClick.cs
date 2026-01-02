using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingClick : MonoBehaviour
{
    [SerializeField] private LayerMask buildingLayer;
    // [SerializeField] private BuildingInfoUI buildingUI;

    void Update() {
        // 빌드 모드 중에는 클릭 로직 방지
        if (BuildModeManager.Instance.IsInBuildMode) return;

        if (Mouse.current.leftButton.wasPressedThisFrame) {
            var hit = MouseProvider.GetHitInfo(buildingLayer);
            if (hit.collider != null) {
                var building = hit.collider.GetComponent<BuildingController>();
                if (building != null) {
                // UI 띄우기
                
                }
            } else {
                // UI 숨기기
            }
        }
    }



}
