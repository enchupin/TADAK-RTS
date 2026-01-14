using UnityEngine;

/// <summary>
/// 기본 건물 컨트롤러
/// 모든 건물 타입의 베이스 클래스
/// </summary>
public class BuildingController : MonoBehaviour, ISelectable
{
    [Header("Building Settings")]
    [SerializeField] protected string buildingID;
    
    protected BuildingJsonData _buildingData;

    // 프로퍼티
    public BuildingJsonData BuildingData => _buildingData;
    public string BuildingID => buildingID;

    protected virtual void Start() {
        // 데이터베이스 초기화 (이미 초기화되어 있으면 무시됨)
        GameDataBase.Initialize("Human");
        
        // 건물 데이터 로드
        if (!string.IsNullOrEmpty(buildingID)) {
            _buildingData = GameDataBase.GetBuilding(buildingID);
        }
    }

    // ISelectable 인터페이스 구현 - 하위 클래스에서 override 가능
    public virtual void SingleSelectEntityInfo() {

    }
}
