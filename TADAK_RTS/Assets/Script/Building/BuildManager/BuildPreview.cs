using UnityEngine;

public class BuildPreview {
    private GameObject ghostObject; // 선택된 건물
    private Renderer[] renderers; // 건물 별 매쉬

    public BuildPreview(GameObject prefab) {
        ghostObject = Object.Instantiate(prefab);
        ghostObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        renderers = ghostObject.GetComponentsInChildren<Renderer>();
    }

    public void SetPosition(Vector3 position) {
        if (ghostObject == null)
            return;
        ghostObject.transform.position = position;
    }

    public void UpdateVisual(bool canPlace) {
        // 가능하면 초록, 아니면 빨강으로 설정
        Color targetColor = canPlace ? new Color(0, 255f, 0, 0.5f) : new Color(255f, 0, 0, 0.5f);
        foreach (var r in renderers) {
            r.material.color = targetColor;
        }
    }

    public void Destroy() => Object.Destroy(ghostObject);
    public Vector3 GetPosition() => ghostObject.transform.position;
}