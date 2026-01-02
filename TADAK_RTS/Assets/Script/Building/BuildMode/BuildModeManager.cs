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

    // 구조체 데이터가 유효한지 확인하는 도우미 프로퍼티
    private bool IsDataSelected => !string.IsNullOrEmpty(selectedData.ID);



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


    // 클릭 관리
    private void HandleBuildInput() {

        // 추후 LayerMask.GetMask("Map")를 mapLayer로 변경
        RaycastHit hit = MouseProvider.GetHitInfo(LayerMask.GetMask("Map"));
        // 잔상 위치 업데이트
        if (hit.collider != null) {
            UpdateGhost(hit.point);
        }
        // 건설 확정
        if (Mouse.current.leftButton.wasPressedThisFrame && !isConfirming) {
            _ = ConfirmPlacement(); // 비동기 호출
        }
        // 건설모드 취소
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            ClearMode();
        }
    }

    // 빌드모드 시작
    public async Task StartBuildMode(string id) {
        try {
            selectedData = GameDataBase.GetBuilding(id); // 아이디를 기준으로 딕셔너리에서 데이터 가져옴
            if (!IsDataSelected) return;

            loadedPrefab = await ResourceManager.Instance.GetBuildingPrefab(id); // 건물 잔상 프리팹
            constructionPrefab = await ResourceManager.Instance.GetBuildingPrefab("Orc_Construction"); // 건설중 프리팹

            if (loadedPrefab != null) { // 예외
                CreateGhost(loadedPrefab);
            }
        } catch (System.Exception e) {
            Debug.LogError($"[BuildModeManager] 시작 오류: {e.Message}");
        }
    }

    // 잔상 프리팹 로드
    private void CreateGhost(GameObject prefab) {
        ClearGhost();
        ghostObject = Instantiate(prefab);
        ghostObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        ghostRenderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (var r in ghostRenderers) {
            r.material = ghostMaterial;
        }
    }

    // 잔상 위치 업데이트
    private void UpdateGhost(Vector3 position) {
        ghostObject.transform.position = position;
        bool isValid = validator.IsValid(position, userName);
        Color targetColor = isValid ? new Color(0, 1f, 0, 0.3f) : new Color(1f, 0, 0, 0.3f);
        foreach (var r in ghostRenderers) {
            r.material.color = targetColor;
        }
    }

    // 잔상 프리팹 제거
    private void ClearGhost() {
        if (ghostObject != null) {
            Destroy(ghostObject);
            ghostObject = null;
            ghostRenderers = null;
        }
    }

    // 건설 확정
    public async Task ConfirmPlacement() {
        if (!IsInBuildMode || !IsDataSelected) return;

        Vector3 pos = ghostObject.transform.position; // 잔상 위치에 건설

        if (!validator.IsValid(pos, userName)) return;
        if (!PlayerResourcesManager.Instance.ConsumeResources(0, selectedData.Wood, selectedData.Rock)) return;
        isConfirming = true;

        // 프리팹 생성
        Instantiate(constructionPrefab, pos, Quaternion.Euler(-90f, 0f, 0f));

        // 건설모드 종료
        ClearMode();
        isConfirming = false;
        await Task.CompletedTask;
    }

    // 건설모드 종료
    public void ClearMode() {
        if (ghostObject != null) {
            Destroy(ghostObject);
            ghostObject = null;
            ghostRenderers = null;
        }
        selectedData = default;
        isConfirming = false;
    }
}