using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

public class BuildModeManager : MonoBehaviour {
    public static BuildModeManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private LayerMask mapLayer;
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
    private bool isInBuildMode => ghostObject != null;

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
        if (!isInBuildMode) return;

        HandleBuildInput();
    }

    private void HandleBuildInput() {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        // 추후 LayerMask.GetMask("Map")를 mapLayer로 변경
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("Map"))) {
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
    }

    private void UpdateGhost(Vector3 position) {
        ghostObject.transform.position = position;
        bool isValid = validator.IsValid(position, userName);
        Color targetColor = isValid ? new Color(0, 255f, 0, 0.5f) : new Color(255f, 0, 0, 0.5f);
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
        if (!isInBuildMode || selectedData == null) return;

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