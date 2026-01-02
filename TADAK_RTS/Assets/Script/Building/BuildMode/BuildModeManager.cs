using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

public class BuildModeManager : MonoBehaviour {
    public static BuildModeManager Instance { get; private set; }

    [SerializeField] private LayerMask mapLayer;
    [SerializeField] private Material ghostMaterial; // 인스펙터에서 Transparent 설정된 머티리얼 할당



    private string userName = "Player1";

    // Preview(Ghost) 관련 변수
    private GameObject ghostObject;
    private Renderer[] ghostRenderers;
    private bool isConfirming = false;

    // 데이터 관련
    private BuildingJsonData selectedData;
    private GameObject loadedPrefab;
    private GameObject constructionPrefab;
    private readonly IPlacementValidator validator = new OccupationValidator();

    // 빌드 모드 활성화 여부
    public bool IsInBuildMode => ghostObject != null;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        GameDataBase.Initialize("Orc");
    }

    private void Update() {
        // 빌드 모드가 아니면 아무것도 하지 않음
        if (!IsInBuildMode) return;

        HandleBuildInput();
    }

    private void HandleBuildInput() {

        // 추후 LayerMask.GetMask("Map")를 mapLayer로 변경
        RaycastHit hit = MouseProvider.GetHitInfo(LayerMask.GetMask("Map"));

        if (hit.collider != null) {
            UpdateGhost(hit.point);
        }
        if (Mouse.current.leftButton.wasPressedThisFrame && !isConfirming) {
            _ = ConfirmPlacement(); // 비동기 호출
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            ClearMode();
        }
    }

    public async Task StartBuildMode(string id) {
        try {
            selectedData = GameDataBase.GetBuilding(id);
            if (selectedData == null) return;

            loadedPrefab = await ResourceManager.Instance.GetBuildingPrefab(id);
            constructionPrefab = await ResourceManager.Instance.GetBuildingPrefab("Orc_Construction");

            if (loadedPrefab != null) {
                CreateGhost(loadedPrefab);
            }
        } catch (System.Exception e) {
            Debug.LogError($"[BuildModeManager] 시작 오류: {e.Message}");
        }
    }

    private void CreateGhost(GameObject prefab) {
        ClearGhost();
        ghostObject = Instantiate(prefab);
        ghostObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        ghostRenderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (var r in ghostRenderers) {
            r.material = ghostMaterial;
        }
    }

    private void UpdateGhost(Vector3 position) {
        ghostObject.transform.position = position;
        bool isValid = validator.IsValid(position, userName);
        Color targetColor = isValid ? new Color(0, 1f, 0, 0.3f) : new Color(1f, 0, 0, 0.3f);
        foreach (var r in ghostRenderers) {
            r.material.color = targetColor;
        }
    }

    private void ClearGhost() {
        if (ghostObject != null) {
            Destroy(ghostObject);
            ghostObject = null;
            ghostRenderers = null;
        }
    }

    public async Task ConfirmPlacement() {
        if (!IsInBuildMode || selectedData == null) return;

        Vector3 pos = ghostObject.transform.position;

        if (!validator.IsValid(pos, userName)) return;
        if (!PlayerResourcesManager.Instance.ConsumeResources(0, selectedData.Wood, selectedData.Rock)) return;
        isConfirming = true;

        Instantiate(constructionPrefab, pos, Quaternion.Euler(-90f, 0f, 0f));

        ClearMode();
        isConfirming = false;
        await Task.CompletedTask;
    }

    public void ClearMode() {
        if (ghostObject != null) {
            Destroy(ghostObject);
            ghostObject = null;
            ghostRenderers = null;
        }
        selectedData = null;
        isConfirming = false;
    }
}