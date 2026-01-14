using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛 생산 대기열 항목
/// </summary>
[System.Serializable]
public class ProductionItem {
    public UnitJsonData UnitData;
    public float RemainingTime;
    
    public ProductionItem(UnitJsonData unitData) {
        UnitData = unitData;
        // ProductionTime이 0이면 기본값 5초 사용
        RemainingTime = unitData.ProductionTime > 0 ? unitData.ProductionTime : 5f;
    }
}

/// <summary>
/// 유닛 생산 큐 관리 컴포넌트
/// 건물에 부착되어 유닛을 순차적으로 생산
/// </summary>
public class UnitProductionQueue : MonoBehaviour {
    
    [Header("생산 큐 상태")]
    [SerializeField] private List<ProductionItem> productionQueue = new List<ProductionItem>();
    [SerializeField] private bool isProducing = false;
    
    private BuildingJsonData _buildingData;
    private Transform _spawnPoint; // 스폰 위치
    
    // 이벤트
    public event Action<ProductionItem> OnProductionStarted; // 생산 시작 이벤트 발생
    public event Action<UnitJsonData> OnProductionComplete; // 생산 완료 이벤트 발생
    public event Action OnQueueChanged; // 큐 변경 이벤트 발생
    
    // 프로퍼티
    public IReadOnlyList<ProductionItem> Queue => productionQueue; // 큐 반환
    public bool IsProducing => isProducing; // 생산 중 여부
    public ProductionItem CurrentProduction => isProducing && productionQueue.Count > 0 ? productionQueue[0] : null; // 현재 생산 중인 유닛
    public float CurrentProgress { // 현재 생산 진행도
        get {
            if (CurrentProduction == null) return 0f; // 만약 현재 생산 중인 유닛이 없으면 0
            float totalTime = CurrentProduction.UnitData.ProductionTime > 0 ? CurrentProduction.UnitData.ProductionTime : 5f; // 기본값 5초
            return 1f - (CurrentProduction.RemainingTime / totalTime); // 현재 진행도 계산
        }
    }
    
    /// <summary>
    /// 건물 데이터로 초기화
    /// </summary>
    public void Initialize(BuildingJsonData buildingData) {
        _buildingData = buildingData;
        _spawnPoint = transform; // 기본적으로 건물 위치에서 생성
    }
    
    /// <summary>
    /// 스폰 위치 설정
    /// </summary>
    public void SetSpawnPoint(Transform spawnPoint) {
        _spawnPoint = spawnPoint;
    }
    
    private void Update() {
        if (!isProducing || productionQueue.Count == 0) return;
        
        // 현재 생산 중인 유닛 타이머 감소
        productionQueue[0].RemainingTime -= Time.deltaTime;
        
        // 생산 완료
        if (productionQueue[0].RemainingTime <= 0f) {
            CompleteProduction();
        }
    }
    
    /// <summary>
    /// 유닛을 생산 큐에 추가, AddToQueue 오버로딩, 아마 사용하지 않을 것 같음
    /// </summary>
    public bool AddToQueue(string unitID) {
        // 유닛 데이터 조회
        UnitJsonData unitData = GameDataBase.GetUnit(unitID);
        if (string.IsNullOrEmpty(unitData.ID)) { // 만약 유닛 ID가 없으면 false 반환
            Debug.LogWarning($"[UnitProductionQueue] 유닛 ID '{unitID}'를 찾을 수 없습니다.");
            return false;
        }
        
        return AddToQueue(unitData);
    }
    

    /// <summary>
    /// 유닛 데이터로 생산 큐에 추가
    /// </summary>
    public bool AddToQueue(UnitJsonData unitData) {
        // 자원 확인
        if (!PlayerResourcesManager.Instance.CanAfford(unitData.CostFood, unitData.CostWood, unitData.CostRock)) { // 만약 자원이 부족하면 false 반환
            Debug.Log($"[UnitProductionQueue] 자원이 부족합니다. (Wood: {unitData.CostWood}, Rock: {unitData.CostRock}, Food: {unitData.CostFood})");
            return false;
        }
        
        // 자원 소비
        PlayerResourcesManager.Instance.ConsumeResources(unitData.CostFood, unitData.CostWood, unitData.CostRock);
        
        // 큐에 추가
        ProductionItem item = new ProductionItem(unitData);
        productionQueue.Add(item);
        
        OnQueueChanged?.Invoke(); // 큐 변화 이벤트 발생
        
        // 생산 시작
        if (!isProducing) {
            StartNextProduction();
        }
        
        return true;
    }
    
    /// <summary>
    /// 다음 유닛 생산 시작
    /// </summary>
    private void StartNextProduction() {
        if (productionQueue.Count == 0) {
            isProducing = false;
            return;
        }
        
        isProducing = true;
        OnProductionStarted?.Invoke(productionQueue[0]); // 생산 시작 이벤트 발생
    }
    
    /// <summary>
    /// 현재 생산 완료 처리
    /// </summary>
    private void CompleteProduction() {
        if (productionQueue.Count == 0) return;
        
        UnitJsonData completedUnit = productionQueue[0].UnitData;
        productionQueue.RemoveAt(0);
        
        // 유닛 생성
        SpawnUnit(completedUnit);
        
        OnProductionComplete?.Invoke(completedUnit); // 생산 완료 이벤트 발생
        OnQueueChanged?.Invoke(); // 큐 변화 이벤트 발생
        
        // 다음 생산 시작
        if (productionQueue.Count > 0) {
            StartNextProduction();
        } else {
            isProducing = false;
        }
    }
    
    /// <summary>
    /// 유닛 스폰
    /// </summary>
    private void SpawnUnit(UnitJsonData unitData) {
        // 스폰 위치 계산 (건물 앞쪽 약간 떨어진 곳)
        Vector3 spawnPosition = _spawnPoint.position + _spawnPoint.forward * 2f;
        
        // TODO: 유닛 프리팹 로드 및 인스턴스화
        // 현재는 로그로만 출력
        Debug.Log($"[UnitProductionQueue] '{unitData.ID}' 유닛이 {spawnPosition}에 생성되었습니다.");
        
        // 추후 구현 예정:
        // GameObject unitPrefab = Resources.Load<GameObject>($"Prefabs/Units/{unitData.ID}");
        // if (unitPrefab != null) {
        //     GameObject unitObj = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
        //     UnitController controller = unitObj.GetComponent<UnitController>();
        //     controller.Initialize(unitData, "Player1");
        // }
    }
    
    /// <summary>
    /// 특정 인덱스의 생산 취소
    /// </summary>
    public bool CancelProduction(int index) {
        if (index < 0 || index >= productionQueue.Count) return false;
        
        ProductionItem canceledItem = productionQueue[index];
        
        // 자원 환불 (50%)
        int refundWood = canceledItem.UnitData.CostWood / 2;
        int refundRock = canceledItem.UnitData.CostRock / 2;
        int refundFood = canceledItem.UnitData.CostFood / 2;
        PlayerResourcesManager.Instance.ProduceResources(refundFood, refundWood, refundRock);
        
        productionQueue.RemoveAt(index);
        
        Debug.Log($"[UnitProductionQueue] '{canceledItem.UnitData.ID}' 생산 취소됨. 자원 50% 환불.");
        
        OnQueueChanged?.Invoke();
        
        // 첫 번째 항목이 취소되었으면 다음 생산 시작
        if (index == 0 && productionQueue.Count > 0) {
            StartNextProduction();
        } else if (productionQueue.Count == 0) {
            isProducing = false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 전체 큐 취소
    /// </summary>
    public void CancelAllProduction() {
        while (productionQueue.Count > 0) {
            CancelProduction(productionQueue.Count - 1);
        }
    }
}
