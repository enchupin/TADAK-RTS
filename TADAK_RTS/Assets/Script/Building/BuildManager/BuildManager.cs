using UnityEngine;
using System.Threading.Tasks;

public class BuildManager : MonoBehaviour {
    public static BuildManager Instance { get; private set; }

    [SerializeField] private GameObject inputHandler;
    private BuildingData selectedData;
    private BuildPreview preview;
    private GameObject loadedPrefab;
    private readonly IPlacementValidator validator = new OccupationValidator();

    void Awake() => Instance = this;


    // async Task를 사용하여 안정성 및 추적 가능성 확보
    public async Task StartBuildMode(string id) {
        try {
            // 데이터와 에셋 로딩을 병렬로 처리하거나 순차적으로 처리
            // 데이터 가져오기 (JSON 기반)
            selectedData = BuildingDataBase.Get(id);
            if (selectedData == null) {
                Debug.LogError($"[BuildManager] {id}에 해당하는 데이터가 JSON에 없습니다.");
                return;
            }

            // 비동기 작업의 흐름 제어 (로딩 중 UI 등을 띄울 수 있음)
            loadedPrefab = await ResourceManager.Instance.GetBuildingPrefab(id);

            if (loadedPrefab != null) {
                InitializeBuildPreview(loadedPrefab);
            }
        } catch (System.Exception e) {
            Debug.LogError($"[BuildManager] 빌드 모드 시작 중 오류 발생: {e.Message}");
        }
    }

    public void UpdateGhost(Vector3 position, string user) {
        // preview(BuildPreview 객체)가 로드 완료되어 생성된 상태인지 확인
        if (preview == null) return;

        // 미리보기 위치 갱신
        preview.SetPosition(position);

        // 설치 가능 여부를 판단하여 시각적 피드백 제공 (초록색/빨간색 등)
        bool isValid = validator.IsValid(position, user);
        preview.UpdateVisual(isValid);
    }


    // 미리보기 생성 메서드
    private void InitializeBuildPreview(GameObject prefab) {
        preview?.Destroy();
        preview = new BuildPreview(prefab);
        inputHandler.SetActive(true);
    }

    // 건설 확정 메서드
    public async Task ConfirmPlacement(string user) {
        if (preview == null) return;

        Vector3 pos = preview.GetPosition();
        if (validator.IsValid(pos, user)) {

            // 나중에 Addressables.InstantiateAsync를 쓸 것에 대비해 async Task 타입으로 구현
            Instantiate(loadedPrefab, pos, Quaternion.identity);

            ClearMode();
        }

        // Task 반환을 위해 명시적으로 기다릴게 없다면 자동으로 완료된 Task 반환
        await Task.CompletedTask;
    }

    public void ClearMode() {
        preview?.Destroy();
        preview = null;
        selectedData = null;
        inputHandler.SetActive(false);
    }
}