using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 드래그 선택 영역의 시각적 렌더링을 담당
/// </summary>
public class SelectionRenderer : MonoBehaviour {
    private Texture2D selectionTexture;
    
    // 드래그 상태 (외부에서 설정)
    public bool IsDragging { get; set; }
    public Vector2 StartMousePos { get; set; }
    
    private void Awake() {
        InitializeDragTexture();
    }

    /// <summary>
    /// 드래그 범위 화면 표시
    /// </summary>
    private void OnGUI() {
        if (IsDragging) {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            var rect = GetDragRect(StartMousePos, currentMousePos);
            rect.y = Screen.height - rect.y - rect.height;
            GUI.DrawTexture(rect, selectionTexture);
        }
    }

    /// <summary>
    /// 드래그 범위 표시를 위한 반투명 텍스처 생성
    /// </summary>
    private void InitializeDragTexture() {
        selectionTexture = new Texture2D(1, 1);
        Color32 fixedColor = new Color32(204, 204, 255, 77);
        selectionTexture.SetPixel(0, 0, fixedColor);
        selectionTexture.Apply();
    }

    /// <summary>
    /// 두 스크린 좌표로부터 드래그 영역 Rect 생성
    /// </summary>
    public static Rect GetDragRect(Vector2 screenPos1, Vector2 screenPos2) {
        var topLeft = Vector2.Min(screenPos1, screenPos2);
        var bottomRight = Vector2.Max(screenPos1, screenPos2);
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
}
