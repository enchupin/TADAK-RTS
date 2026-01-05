using System.Collections.Generic;
using UnityEngine;

public class SelectBuilding : MonoBehaviour
{
    public static SelectBuilding Instance { get; private set; }

    private BuildingController selectedBuilding = new BuildingController();



    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ClearSelectBuilding() {

        if (selectedBuilding != null) {
            // 건물 선택 시점에 건물에 추가된 시각적 효과가 있다면 해제하는 기능 구현
        }
        selectedBuilding = null;
    }












}
