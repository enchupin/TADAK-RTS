using System; // Action 사용을 위해 필요
using UnityEngine;

// 실제 유닛에 붙일 스크립트
public class UnitController : MonoBehaviour, ISelectable
{
    private UnitSound _unitSound;
    private UnitHealth _health;
    private IMovement _movement;
    public UnitJsonData _unitData;

    public string OwnerId = "P1"; // 임시 테스트 용

    public event Action<UnitController> OnDead;

    // [테스트를 위해 추가한 변수]
    [Header("테스트용 설정")]
    [Tooltip("여기에 Worker 같은 유닛 ID를 적으세요")]
    [SerializeField] private string _testUnitID;

    private void Awake()
    {
        _unitSound = GetComponent<UnitSound>();
        _health = GetComponent<UnitHealth>();
        _movement = GetComponent<IMovement>();
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(_testUnitID))
        {
            Initialize(_testUnitID, OwnerId);

            Invoke("TestMove", 1.0f);
        }
    }

    // 초기화 함수
    public void Initialize(string unitID, string ownerID)
    {
        _unitData = GameDataBase.GetUnit(unitID);

        if (string.IsNullOrEmpty(_unitData.ID))
        {
            Debug.LogError($"[UnitController] '{unitID}' 데이터를 DB에서 찾을 수 없습니다. (GameDataBase 초기화 되었나요?)");
            return;
        }

        this.OwnerId = ownerID;

        if (_movement != null)
        {
            _movement.Initialize(_unitData.moveSpeed);
        }

        gameObject.name = $"{unitID}_{ownerID}";
        Debug.Log($"[{gameObject.name}] 초기화 완료! 이속: {_unitData.moveSpeed}");
    }

    public void CommandMove(Vector3 destination)
    {
        if (_movement != null)
        {
            _movement.MoveTo(destination);
        }
    }

    private void TestMove()
    {
        Debug.Log(">> [테스트] (10, 0, 10)으로 이동 명령!");
        CommandMove(new Vector3(10, 0, 10));
    }

    public void Die()
    {
        OnDead?.Invoke(this);
    }
}