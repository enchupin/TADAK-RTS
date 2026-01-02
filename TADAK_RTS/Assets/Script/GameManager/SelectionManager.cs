using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour {
    public static SelectionManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private string myPlayerID = "Player1"; // 현재 플레이어 ID
    [SerializeField] private LayerMask clickLayer; // 클릭 레이어
    private Texture2D selectionTexture; // 드래그 범위


    private List<UnitController> selectedUnits = new List<UnitController>();
    private Vector2 startMousePos;
    private bool isDragging = false;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializeDragTexture();
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


    private void OnGUI() {
        if (isDragging) {
            var rect = GetScreenRect(startMousePos, Mouse.current.position.ReadValue());
            rect.y = Screen.height - rect.y - rect.height;
            GUI.DrawTexture(rect, selectionTexture);
        }
    }


    private void InitializeDragTexture() {
        selectionTexture = new Texture2D(1, 1);
        selectionTexture = new Texture2D(1, 1);
        Color32 fixedColor = new Color32(204, 204, 255, 77);
        selectionTexture.SetPixel(0, 0, fixedColor);
        selectionTexture.Apply();
    }

    // 단일 선택
    private void SingleSelect(Vector2 mousePos) {
        ClearSelection();

        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit entityHit, 1000f, clickLayer)) {
            ISelectable entity = entityHit.collider.GetComponent<ISelectable>();

            // 선택 성공
            entity.SingleSelectEntityInfo();
        }
    }


    // 드래그 범위 선택
    private void DragSelect(Vector2 startPos, Vector2 endPos) {
        ClearSelection();
        Rect selectionRect = GetScreenRect(startPos, endPos);


        // 맵 상의 모든 UnitController를 검사 (성능 최적화가 필요하면 레이어 기반 Overlap 사용 가능)
        UnitController[] allUnits = Object.FindObjectsByType<UnitController>(FindObjectsSortMode.None);


        foreach (var unit in allUnits) {
            // 유닛의 월드 좌표를 스크린 좌표로 변환하여 사각형 안에 있는지 확인
            Vector2 unitScreenPos = Camera.main.WorldToScreenPoint(unit.transform.position);
            if (selectionRect.Contains(unitScreenPos) && IsOwnedByMe(unit)) {
                SelectUnit(unit);
            }
        }


        if (selectedUnits == null)
            return;

        else if (selectedUnits.Count == 1) {


        }
        ShowAllUnits();


    }


    private void ShowAllUnits() {
        // 전체 UI 구현



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

    private bool IsOwnedByMe(Component entity) {
        // 내 소유인지 판별하는 로직 구현


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
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
}