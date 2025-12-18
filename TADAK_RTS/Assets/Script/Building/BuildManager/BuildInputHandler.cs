using UnityEngine;

public class BuildInputHandler : MonoBehaviour {

    private string userName = "Player1"; // 임시 이름

    void Update() {
        // 위치 갱신 요청
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Map"))) {
            BuildManager.Instance.UpdateGhost(hit.point, userName);
        }

        // 설치 요청
        if (Input.GetMouseButtonDown(0)) {
            BuildManager.Instance.ConfirmPlacement(userName);
        }

        // 취소 요청
        if (Input.GetKeyDown(KeyCode.Escape)) {
            BuildManager.Instance.ClearMode();
        }
    }
}