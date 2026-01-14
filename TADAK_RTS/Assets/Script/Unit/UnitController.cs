using System;
using UnityEngine;

public class UnitController : MonoBehaviour, ISelectable
{
    private UnitSound _unitSound;
    private UnitHealth _health;
    private IMovement _movement; 

    [SerializeField] public UnitJsonData _unitData; // 인스펙터 확인용 public
    public string OwnerId;

    [Header("테스트 설정")]
    public string testUnitID = "Worker";

    public event Action<UnitController> OnDead;

    private void Awake()
    {
        _movement = GetComponent<IMovement>();
        _unitSound = GetComponent<UnitSound>();
        _health = GetComponent<UnitHealth>();
    }

    // ★ [테스트용] 게임 시작하자마자 스스로 데이터를 로드함!
    private void Start()
    {

        // 2. DB에서 데이터 가져오기
        UnitJsonData data = GameDataBase.GetUnit(testUnitID);

        // 3. 데이터가 잘 왔는지 확인
        if (string.IsNullOrEmpty(data.ID))
        {
            Debug.LogError($"[오류] '{testUnitID}' 데이터를 찾을 수 없습니다! ID를 확인하거나 DB 로드를 확인하세요.");
            return;
        }

        // 4. 초기화 실행! (이제 여기서 호출됨)
        Initialize(data, "Player1");
    }

    public void Initialize(UnitJsonData data, string ownerID)
    {
        _unitData = data;
        this.OwnerId = ownerID;

        // 이동 부품에 속도값 전달!
        if (_movement != null)
        {
            _movement.Initialize(_unitData.MoveSpeed);
        }
    }

    public void CommandMove(Vector3 destination)
    {
        if (_movement != null)
        {
            _movement.MoveTo(destination);
        }
    }

    // ISelectable 인터페이스 구현
    public void SingleSelectEntityInfo()
    {
        Debug.Log($"[유닛 선택됨] {name} (ID: {_unitData.ID})");
        // 나중에 여기에 UI 띄우는 코드 추가
    }


}
