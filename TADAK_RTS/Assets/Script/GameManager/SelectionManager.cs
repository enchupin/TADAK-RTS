using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour {
    public static SelectionManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private string myPlayerID = "Player1"; // 현재 플레이어 ID
    [SerializeField] private LayerMask unitLayer; // 유닛 레이어

    private List<UnitController> selectedUnits = new List<UnitController>();
    private Vector2 startMousePos;
    private bool isDragging = false;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update() {
        HandleSelection();
        HandleMovementCommand();
    }

    private void HandleSelection() {
        // 드래그 시작
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            startMousePos = Mouse.current.position.ReadValue();
            isDragging = true;
        }

        // 드래그 중 시각적 피드백 구현

        // 선택 완료
        if (Mouse.current.leftButton.wasReleasedThisFrame) {
            Vector2 endMousePos = Mouse.current.position.ReadValue();
            isDragging = false;

            // 드래그 거리가 짧으면 단일 클릭으로 간주
            // 추후 테스트 후 삭제 고려
            if (Vector2.Distance(startMousePos, endMousePos) < 5f) {
                SingleSelect(endMousePos);
            } else {
                DragSelect(startMousePos, endMousePos);
            }
        }

    }

    // 단일 선택
    private void SingleSelect(Vector2 mousePos) {
        ClearSelection();

        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, unitLayer)) {
            UnitController unit = hit.collider.GetComponent<UnitController>();
            if (unit != null && IsMyUnit(unit)) {
                SelectUnit(unit);
            }
        }
    }

    // 드래그 범위 선택
    private void DragSelect(Vector2 startPos, Vector2 endPos) {
        ClearSelection();

        // 드래그 사각형 영역 계산
        Rect selectionRect = GetScreenRect(startPos, endPos);




        // 맵 상의 모든 UnitController를 검사 (성능 최적화가 필요하면 레이어 기반 Overlap 사용 가능)
        UnitController[] allUnits = Object.FindObjectsByType<UnitController>(FindObjectsSortMode.None);



        foreach (var unit in allUnits) {
            // 유닛의 월드 좌표를 스크린 좌표로 변환하여 사각형 안에 있는지 확인
            Vector2 unitScreenPos = Camera.main.WorldToScreenPoint(unit.transform.position);
            if (selectionRect.Contains(unitScreenPos) && IsMyUnit(unit)) {
                SelectUnit(unit);
            }
        }
    }

    // 우클릭 이동 명령
    private void HandleMovementCommand() {
        if (Mouse.current.rightButton.wasPressedThisFrame && selectedUnits.Count > 0) {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("Map"))) {
                foreach (var unit in selectedUnits) {
                    // UnitController 내부의 이동 메서드 호출 (예: _movement.MoveTo(hit.point))
                    // unit.GetComponent<IMovement>().MoveTo(hit.point); // 구조에 맞게 호출
                    Debug.Log($"{unit.name} 이동 명령: {hit.point}");
                }
            }
        }
    }

    private bool IsMyUnit(UnitController unit) {
        // BaseUnit의 OwnerID 확인 구현


        return true;
    }

    private void SelectUnit(UnitController unit) {
        if (!selectedUnits.Contains(unit)) {
            selectedUnits.Add(unit);
            // 유닛에게 선택되었음을 알리는 시각적 효과 구현
        }
    }

    private void ClearSelection() {
        selectedUnits.Clear();
        // 모든 유닛의 선택 효과 해제
    }

    // 유틸리티 : 드래그 영역Rect 생성
    private Rect GetScreenRect(Vector2 screenPos1, Vector2 screenPos2) {
        var topLeft = Vector2.Min(screenPos1, screenPos2);
        var bottomRight = Vector2.Max(screenPos1, screenPos2);
        return Rect.MinMaxRect(topLeft.x, screenPos1.y, bottomRight.x, screenPos2.y);
    }
}