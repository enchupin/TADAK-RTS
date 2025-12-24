using UnityEngine.InputSystem;
using UnityEngine;

public class BuildInputHandler : MonoBehaviour {
    private string userName = "Player1";
    private bool isConfirming = false; // 중복 설치 방지

    void Update() {

        // 마우스 위치 업데이트
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        // 디버깅용 코드
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red);



        // Raycast
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("Map"))) {
            BuildManager.Instance.UpdateGhost(hit.point, userName);
        }

        // 건설 확정
        if (Mouse.current.leftButton.wasPressedThisFrame && !isConfirming) {
            ConfirmBuild();
        }

        // 건설 모드 취소
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            BuildManager.Instance.ClearMode();
        }
    }

    private async void ConfirmBuild() {
        isConfirming = true;
        await BuildManager.Instance.ConfirmPlacement(userName);
        isConfirming = false;
    }
}