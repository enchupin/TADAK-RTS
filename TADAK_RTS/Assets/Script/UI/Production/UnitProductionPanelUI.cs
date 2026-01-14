using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 유닛 생산 패널 UI 관리
/// 생산 건물 선택 시 표시되며, 생산 가능한 유닛 목록과 큐 상태를 보여줌
/// </summary>
public class UnitProductionPanelUI : MonoBehaviour {
    
    public static UnitProductionPanelUI Instance { get; private set; }
    
    [Header("UI References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject unitButtonPrefab;
    
    [Header("Queue Display")]
    [SerializeField] private Transform queueContainer;
    [SerializeField] private GameObject queueItemPrefab;
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI currentProductionText;
    
    [Header("Building Info")]
    [SerializeField] private TextMeshProUGUI buildingNameText;
    
    private IUnitProducer _currentProducer;
    private UnitProductionQueue _currentQueue;
    private List<GameObject> _spawnedButtons = new List<GameObject>();
    private List<GameObject> _spawnedQueueItems = new List<GameObject>();
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        
        // 시작 시 패널 숨김
        if (panelRoot != null) {
            panelRoot.SetActive(false);
        }
    }
    
    private void Update() {
        // 진행률 바 업데이트
        if (_currentQueue != null && _currentQueue.IsProducing && progressBar != null) {
            progressBar.fillAmount = _currentQueue.CurrentProgress;
        }
    }
    
    /// <summary>
    /// 생산 패널 표시 (IUnitProducer를 구현한 건물용)
    /// </summary>
    public void Show(UnitBuildingData building) {
        if (building == null) return;
        
        _currentProducer = building;
        _currentQueue = building.ProductionQueue;
        
        if (_currentQueue == null) {
            Debug.LogWarning("[UnitProductionPanelUI] 이 건물은 유닛 생산 기능이 없습니다.");
            return;
        }
        
        // 패널 표시
        if (panelRoot != null) {
            panelRoot.SetActive(true);
        }
        
        // 건물 이름 표시
        if (buildingNameText != null) {
            buildingNameText.text = building.BuildingData.ID;
        }
        
        // 기존 버튼 제거
        ClearButtons();
        
        // 생산 가능한 유닛 버튼 생성
        CreateUnitButtons();
        
        // 큐 UI 업데이트
        RefreshQueueDisplay();
        
        // 큐 변경 이벤트 구독
        _currentQueue.OnQueueChanged += RefreshQueueDisplay;
        
        Debug.Log($"[UnitProductionPanelUI] '{building.BuildingData.ID}' 생산 패널 표시");
    }
    
    /// <summary>
    /// 생산 패널 숨김
    /// </summary>
    public void Hide() {
        if (panelRoot != null) {
            panelRoot.SetActive(false);
        }
        
        // 이벤트 구독 해제
        if (_currentQueue != null) {
            _currentQueue.OnQueueChanged -= RefreshQueueDisplay;
        }
        
        _currentProducer = null;
        _currentQueue = null;
        
        ClearButtons();
        ClearQueueItems();
    }
    
    /// <summary>
    /// 유닛 버튼 생성
    /// </summary>
    private void CreateUnitButtons() {
        if (_currentProducer == null || buttonContainer == null || unitButtonPrefab == null) return;
        
        List<string> producibleUnits = _currentProducer.ProducibleUnits;
        if (producibleUnits == null || producibleUnits.Count == 0) return;
        
        foreach (string unitID in producibleUnits) {
            UnitJsonData unitData = GameDataBase.GetUnit(unitID);
            if (string.IsNullOrEmpty(unitData.ID)) continue;
            
            // 버튼 생성
            GameObject buttonObj = Instantiate(unitButtonPrefab, buttonContainer);
            _spawnedButtons.Add(buttonObj);
            
            // 버튼 초기화
            UnitProductionButton button = buttonObj.GetComponent<UnitProductionButton>();
            if (button != null) {
                button.Initialize(unitData, _currentQueue);
            }
        }
    }
    
    /// <summary>
    /// 큐 상태 UI 갱신
    /// </summary>
    private void RefreshQueueDisplay() {
        if (_currentQueue == null) return;
        
        ClearQueueItems();
        
        // 현재 생산 중인 유닛 표시
        if (currentProductionText != null) {
            if (_currentQueue.IsProducing && _currentQueue.CurrentProduction != null) {
                currentProductionText.text = $"생산 중: {_currentQueue.CurrentProduction.UnitData.ID}";
            } else {
                currentProductionText.text = "대기 중";
            }
        }
        
        // 프로그레스 바 초기화
        if (progressBar != null) {
            progressBar.fillAmount = _currentQueue.CurrentProgress;
        }
        
        // 큐 아이템 표시
        if (queueContainer != null && queueItemPrefab != null) {
            for (int i = 0; i < _currentQueue.Queue.Count; i++) {
                ProductionItem item = _currentQueue.Queue[i];
                GameObject queueItemObj = Instantiate(queueItemPrefab, queueContainer);
                _spawnedQueueItems.Add(queueItemObj);
                
                // 큐 아이템 UI 설정
                TextMeshProUGUI itemText = queueItemObj.GetComponentInChildren<TextMeshProUGUI>();
                if (itemText != null) {
                    itemText.text = item.UnitData.ID;
                }
                
                // 취소 버튼 설정
                Button cancelButton = queueItemObj.GetComponentInChildren<Button>();
                if (cancelButton != null) {
                    int index = i;
                    cancelButton.onClick.AddListener(() => OnCancelClicked(index));
                }
            }
        }
    }
    
    /// <summary>
    /// 취소 버튼 클릭 처리
    /// </summary>
    private void OnCancelClicked(int index) {
        if (_currentQueue != null) {
            _currentQueue.CancelProduction(index);
        }
    }
    
    /// <summary>
    /// 생성된 버튼 정리
    /// </summary>
    private void ClearButtons() {
        foreach (var button in _spawnedButtons) {
            if (button != null) Destroy(button);
        }
        _spawnedButtons.Clear();
    }
    
    /// <summary>
    /// 생성된 큐 아이템 정리
    /// </summary>
    private void ClearQueueItems() {
        foreach (var item in _spawnedQueueItems) {
            if (item != null) Destroy(item);
        }
        _spawnedQueueItems.Clear();
    }
}
