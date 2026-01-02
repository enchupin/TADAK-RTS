using UnityEngine.InputSystem;
using UnityEngine;


public static class MouseProvider {
    public static RaycastHit GetHitInfo(LayerMask layerMask) {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out RaycastHit hit, 1000f, layerMask);
        return hit;
    }
}