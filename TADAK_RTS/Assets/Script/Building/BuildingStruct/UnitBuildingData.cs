using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛 생산 건물 인터페이스
/// </summary>
public interface IUnitProducer {
    List<string> ProducibleUnits { get; }
    UnitProductionQueue ProductionQueue { get; }
    void RequestProduction(string unitID);
}

/// <summary>
/// 유닛 생산 건물 클래스
/// BuildingController를 상속받아 유닛 생산 기능 추가
/// </summary>
public class UnitBuildingData : BuildingController, IUnitProducer {
    
    private UnitProductionQueue _productionQueue;
    
    // IUnitProducer 구현
    public List<string> ProducibleUnits => _buildingData.ProducibleUnits;
    public UnitProductionQueue ProductionQueue => _productionQueue;

    protected override void Start() {
        base.Start(); // 부모 클래스의 데이터 로드 호출
        
        // 생산 큐 초기화
        if (_buildingData.ProducibleUnits != null && _buildingData.ProducibleUnits.Count > 0) {
            _productionQueue = gameObject.AddComponent<UnitProductionQueue>();
            _productionQueue.Initialize(_buildingData);
            Debug.Log($"[UnitBuildingData] '{buildingID}' 생산 큐 초기화 완료. 생산 가능 유닛: {string.Join(", ", _buildingData.ProducibleUnits)}");
        }
    }

    // ISelectable 구현 - 선택 시 생산 패널 표시
    public override void SingleSelectEntityInfo() {
        base.SingleSelectEntityInfo();
        
        if (_productionQueue != null && UnitProductionPanelUI.Instance != null) {
            UnitProductionPanelUI.Instance.Show(this);
        }
    }

    // IUnitProducer 구현 - 유닛 생산 요청
    public void RequestProduction(string unitID) {
        if (_productionQueue != null) {
            _productionQueue.AddToQueue(unitID);
        } else {
            Debug.LogWarning($"[UnitBuildingData] 생산 큐가 초기화되지 않았습니다.");
        }
    }
}