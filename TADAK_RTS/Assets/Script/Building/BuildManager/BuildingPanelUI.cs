using UnityEngine;

public class BuildingPanelUI : MonoBehaviour {
    [SerializeField] private GameObject buildPanel;

    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            buildPanel.SetActive(!buildPanel.activeSelf);
        }
    }

    // 버튼에 연결될 메서드
    public async void OnClickBuildingButton(string buildingID) {
        await BuildModeManager.Instance.StartBuildMode(buildingID);
        buildPanel.SetActive(false); // 선택 후 패널 닫기
    }
}