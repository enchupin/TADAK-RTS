using UnityEngine;
using System.Collections.Generic;

public class SelectedUnits : MonoBehaviour {
    public static SelectedUnits Instance { get; private set; }

    // 현재 선택된 유닛 리스트
    public List<UnitController> _selectedList = new List<UnitController>();
    public int Count => _selectedList.Count;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 선택된 유닛 초기화
    public void Clear() {

        foreach (var unit in _selectedList) {
            // 유닛 선택 시점에 유닛에 추가된 시각적 효과가 있다면 해제하는 기능 구현
        }
        _selectedList.Clear();
    }

    // 유닛 추가
    public void Add(UnitController unit) {
        if (!_selectedList.Contains(unit)) {
            _selectedList.Add(unit);
        }
    }

    public void ExecuteMove(Vector3 destination)
    {
        foreach (var unit in _selectedList)
        {
            if (unit != null)
            {
                unit.CommandMove(destination);
            }
        }
    }

    // 선택 상태에 따른 UI 업데이트
    public void UpdateSelectionUI() {
        if (_selectedList.Count == 0) return;
    }


}