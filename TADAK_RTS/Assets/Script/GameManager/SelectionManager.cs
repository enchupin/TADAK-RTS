using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour {
    public static SelectionManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private string myPlayerID = "Player1"; // 현재 플레이어 ID
    [SerializeField] private LayerMask clickLayer; // 클릭 레이어
    private Texture2D selectionTexture; // 드래그 범위


    private List<UnitController> selectedUnits = new List<UnitController>(); // 다중 선택된 유닛

    private Vector2 mousePos; // 마우스 위치
    private bool isDragging = false; // 드래그 중인지 판별

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializeDragTexture();
    }

    private void Update() {
        // 좌클릭 선택 명령
        HandleSelection();


    }



    private void HandleSelection() {
        // 드래그 시작
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            mousePos = Mouse.current.position.ReadValue();
            isDragging = true;
        }

        // 선택 완료
        if (Mouse.current.leftButton.wasReleasedThisFrame) {
            Vector2 endMousePos = Mouse.current.position.ReadValue();
            isDragging = false;

            // 드래그 거리가 짧으면 단일 클릭으로 간주
            // 추후 테스트 후 삭제 고려
            if (Vector2.Distance(mousePos, endMousePos) < 5f) {
                SingleSelect(endMousePos);
            } else {
                DragSelect(mousePos, endMousePos);
            }
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


    // 단일 선택
    private void SingleSelect(Vector2 mousePos) {
        ClearSelection();

        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit entityHit, 1000f, clickLayer)) {
            ISelectable entity = entityHit.collider.GetComponent<ISelectable>();

            // 선택 성공 시 단일 개체 UI 호출
            SingleSelectEntityInfo(entity);

            // 내 유닛이면 이동을 위해 selectedUnits 리스트에 추가하는 기능 구현 필요

        }
    }


    // 드래그 영역Rect 생성
    private Rect GetDragRect(Vector2 screenPos1, Vector2 screenPos2) {
        var topLeft = Vector2.Min(screenPos1, screenPos2);
        var bottomRight = Vector2.Max(screenPos1, screenPos2);
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    // 다중 선택
    private void DragSelect(Vector2 startPos, Vector2 endPos) {
        ClearSelection();
        Rect selectionRect = GetDragRect(startPos, endPos);


        // 맵 상의 모든 UnitController를 검사 (성능 최적화를 위해 추후 레이어 기반 Overlap으로 변경 예정)
        UnitController[] allUnits = Object.FindObjectsByType<UnitController>(FindObjectsSortMode.None);

        // 맵 상의 모든 유닛에 대하여
        foreach (var unit in allUnits) {
            // 유닛의 월드 좌표를 스크린 좌표로 변환
            Vector2 unitScreenPos = Camera.main.WorldToScreenPoint(unit.transform.position);
            // 선택된 유닛이 드래그 범위에 있는지 확인, 내꺼면 선택
            if (selectionRect.Contains(unitScreenPos) && IsOwnedByMe(unit)) {
                SelectUnit(unit);
            }
        }


        if (selectedUnits == null) return;

        // 하나의 유닛이 선택되었으면 단일 개체 UI 호출
        else if (selectedUnits.Count == 1) {
            SingleSelectEntityInfo(selectedUnits[0]);

        }
        // 2개 이상의 유닛이 선택되었으면 다중 개체 UI 호출
        else if (selectedUnits.Count >= 2) {
            ShowAllUnits();
        }
    }




    // 단일 선택 UI
    private void SingleSelectEntityInfo(ISelectable selectedEntity) {

    }

    // 다중 선택 UI
    private void ShowAllUnits() {
        



    }




    // 우클릭 이동 명령
    private void HandleMovementCommand() {
        if (Mouse.current.rightButton.wasPressedThisFrame && selectedUnits.Count > 0) {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            // 맵에 우클릭을 찍었을 때
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("Map"))) {
                // selectedUnits에 있는 모든 유닛 개체에 대하여 동작
                foreach (var unit in selectedUnits) {
                    // 각 유닛의 IMovement 컴포넌트 Get
                    IMovement movement = unit.GetComponent<IMovement>();

                    if (movement != null) {
                        // 이동x
                    }


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


}