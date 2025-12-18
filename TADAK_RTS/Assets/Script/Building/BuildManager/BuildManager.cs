using UnityEngine;

public class BuildManager : MonoBehaviour {
    public static BuildManager Instance { get; private set; } // 싱글톤

    [SerializeField] private BuildInputHandler inputHandler; // 핸들러 연결
    private BuildingData selectedData;
    private BuildPreview preview;
    private IPlacementValidator validator = new OccupationValidator();

    void Awake() {
        Instance = this;
    }

    public void StartBuildMode(string id) { // 건물 id 전달
        selectedData = BuildingDataBase.Get(id);
        GameObject prefab = Resources.Load<GameObject>($"Buildings/{id}");
        preview = new BuildPreview(prefab);
        inputHandler.enabled = true;
    }

    public void UpdateGhost(Vector3 position, string user) {
        if (preview == null) return;

        preview.SetPosition(position);
        preview.UpdateVisual(validator.IsValid(position, user));
    }

    public void ConfirmPlacement(string user) {
        Vector3 pos = preview.GetPosition();
        if (validator.IsValid(pos, user)) {

            // 실제 건물 생성 코드

            ClearMode();
        }
    }

    public void ClearMode() {
        preview?.Destroy();
        preview = null;
        selectedData = null;
        inputHandler.enabled = false;
    }
}