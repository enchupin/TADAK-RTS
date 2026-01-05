using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour {
    public static SelectionManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private LayerMask clickLayer; // 클릭 레이어
    [SerializeField] private LayerMask mapLayer;
    private string myPlayerID = "Player1"; // 현재 플레이어 ID


    private Texture2D selectionTexture; // 드래그 범위
    private Vector2 mousePos; // 마우스 위치
    private bool isDragging = false; // 드래그 중인지 판별



    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializeDragTexture();
    }

    private void Update() {
        // 좌클릭 선택 명령
        HandleSelectionInput();

        HandleCommandInput();
    }


    // 단일 선택
    private void SingleSelect(Vector2 mousePos) {
        // 선택된 유닛 초기화
        SelectedUnits.Instance.Clear();

        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit entityHit, 1000f, clickLayer)) {
            ISelectable entity = entityHit.collider.GetComponent<ISelectable>();
            // 선택 성공 시 단일 개체 UI 호출
            entity.SingleSelectEntityInfo();


        }
    }




    // 다중 선택 (유닛만 가능)
    private void DragSelect(Vector2 startPos, Vector2 endPos) {
        // 선택된 유닛 초기화
        SelectedUnits.Instance.Clear();

        // 드래그 영역 생성
        Rect selectionRect = GetDragRect(startPos, endPos);

        // 맵 상의 모든 UnitController를 검사 (성능 최적화를 위해 추후 레이어 기반 Overlap으로 변경 예정)
        UnitController[] allUnits = Object.FindObjectsByType<UnitController>(FindObjectsSortMode.None);

        // 맵 상의 모든 유닛에 대하여
        foreach (var unit in allUnits) {
            // 유닛의 월드 좌표를 스크린 좌표로 변환
            Vector2 unitScreenPos = Camera.main.WorldToScreenPoint(unit.transform.position);
            // 선택된 유닛이 드래그 범위에 있는지 확인, 내꺼면 선택
            if (selectionRect.Contains(unitScreenPos) && IsOwnedByMe(unit)) {
                // 선택된 유닛 데이터 추가
                SelectedUnits.Instance.Add(unit);
            }
        }

        // 선택된 유닛 전부 보여주는 UI 구현



    }

    // UnitController.cs로 옮겨야하지 않나 생각중
    private bool IsOwnedByMe(UnitController entity) {
        // 내 소유인지 판별하는 로직 구현

        return true;
    }



    // 우클릭 이동 명령
    private void HandleCommandInput() {
        if (Mouse.current.rightButton.wasPressedThisFrame && SelectedUnits.Instance.Count > 0) {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, mapLayer)) {
                SelectedUnits.Instance.ExecuteMove(hit.point);
            }
        }
    }




    /// <summary>
    ///  이하 드래그 관련 메서드
    /// </summary>

    // 마우스 인풋 관리
    private void HandleSelectionInput() {
        // 드래그 시작
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            mousePos = Mouse.current.position.ReadValue();
            isDragging = true;
        }

        // 선택 완료
        if (Mouse.current.leftButton.wasReleasedThisFrame) {
            isDragging = false;
            Vector2 endMousePos = Mouse.current.position.ReadValue();
            

            // 드래그 거리가 짧으면 단일 클릭으로 간주
            // 추후 테스트 후 삭제 고려
            if (Vector2.Distance(mousePos, endMousePos) < 5f) {
                SingleSelect(endMousePos);
            } else {
                DragSelect(mousePos, endMousePos);
            }

            // 선택이 끝난 후 UI 업데이트 요청 구현
        }
    }

    // 드래그 범위 표시
    private void OnGUI() {
        if (isDragging) {
            var rect = GetDragRect(mousePos, Mouse.current.position.ReadValue());
            rect.y = Screen.height - rect.y - rect.height;
            GUI.DrawTexture(rect, selectionTexture);
        }
    }

    // 드래그 범위 표시를 위한 이미지 생성
    private void InitializeDragTexture() {
        selectionTexture = new Texture2D(1, 1);
        Color32 fixedColor = new Color32(204, 204, 255, 77);
        selectionTexture.SetPixel(0, 0, fixedColor);
        selectionTexture.Apply();
    }

    // 드래그 영역Rect 생성
    private Rect GetDragRect(Vector2 screenPos1, Vector2 screenPos2) {
        var topLeft = Vector2.Min(screenPos1, screenPos2);
        var bottomRight = Vector2.Max(screenPos1, screenPos2);
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }


}