using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem; // 추가



public class BuildInputHandler : MonoBehaviour {
    private string userName = "Player1";
    void Update() {

        // 마우스 위치 업데이트
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Map"))) {
            BuildManager.Instance.UpdateGhost(hit.point, userName);
        }

        // 건설 확정
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            // _ = 키워드로 "실행 후 기다리지 않음"을 명시
            _ = BuildManager.Instance.ConfirmPlacement(userName);
        }

        // 건설 모드 취소
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            BuildManager.Instance.ClearMode();
        }
    }
}