using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 개별 유닛 생산 버튼
/// 클릭 시 해당 유닛을 생산 큐에 추가
/// </summary>
public class UnitProductionButton : MonoBehaviour {
    
    [Header("UI References")]
    [SerializeField] private Button button; // 버튼
    [SerializeField] private Image iconImage; // 아이콘
    [SerializeField] private TextMeshProUGUI nameText; // 이름
    [SerializeField] private TextMeshProUGUI costText; // 비용
    
    private UnitJsonData _unitData; // 유닛 데이터
    private UnitProductionQueue _productionQueue; // 생산 큐
    
    private void Awake() {
        if (button == null) {
            button = GetComponent<Button>();
        }
        
        if (button != null) {
            button.onClick.AddListener(OnClick); // 버튼 클릭 이벤트 추가
        }
    }
    
    /// <summary>
    /// 버튼 초기화
    /// </summary>
    public void Initialize(UnitJsonData unitData, UnitProductionQueue queue) {
        _unitData = unitData;
        _productionQueue = queue;
        
        // UI 업데이트
        UpdateUI();
    }
    
    /// <summary>
    /// UI 요소 업데이트
    /// </summary>
    private void UpdateUI() {
        // 유닛 이름 표시 -> 안할수도
        if (nameText != null) {
            nameText.text = _unitData.ID;
        }
        
        // 비용 표시
        if (costText != null) {
            costText.text = $"Wood: {_unitData.CostWood} | Rock: {_unitData.CostRock}";
        }
        
        // 아이콘 로드 (추후 구현)
        // if (iconImage != null) {
        //     Sprite icon = Resources.Load<Sprite>($"Icons/Units/{_unitData.ID}");
        //     if (icon != null) iconImage.sprite = icon;
        // }
    }
    
    /// <summary>
    /// 버튼 클릭 처리
    /// </summary>
    private void OnClick() {
        if (_productionQueue == null) {
            Debug.LogWarning("[UnitProductionButton] 생산 큐가 연결되지 않았습니다.");
            return;
        }
        
        bool success = _productionQueue.AddToQueue(_unitData);
        
        if (success) {
            Debug.Log($"[UnitProductionButton] '{_unitData.ID}' 생산 요청 성공");
            // 클릭 효과음 재생 등 추가 가능
        } else {
            Debug.Log($"[UnitProductionButton] '{_unitData.ID}' 생산 요청 실패 (자원 부족)");
            // 실패 효과음 또는 UI 피드백 추가 가능
        }
    }
}
