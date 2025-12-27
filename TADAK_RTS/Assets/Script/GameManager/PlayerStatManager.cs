using UnityEngine;

public class PlayerStatsManager : MonoBehaviour {
    public static PlayerStatsManager Instance { get; private set; }

    public int Wood { get; private set; }
    public int Rock { get; private set; }
    public int Food { get; private set; }
    public int CurrentPopulation { get; private set; }
    public int MaxPopulation { get; private set; }

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 자원 사용 가능 여부 확인
    public bool CanAfford(int foodCost, int woodCost, int rockCost) {
        return Wood >= woodCost && Rock >= rockCost && Food >= foodCost;
    }

    // 자원 소비
    public void ConsumeResources(int food, int wood, int rock) {
        if (!CanAfford(food, wood, rock)) {

            // 생산 불가 이벤트 구현
            return;
        }
        Food -= food;
        Wood -= wood;
        Rock -= rock;
        UpdateUI(); // UI 업데이트 호출
    }

    // 자원 획득
    public void ProduceResources(int food, int wood, int rock) {
        Food += food;
        Wood += wood;
        Rock += rock;
        UpdateUI(); // UI 업데이트 호출
    }

    // 인구수 체크
    public bool CanAddUnit() => CurrentPopulation < MaxPopulation;

    private void UpdateUI() {
        // UI 매니저를 통해 화면에 반영
    }
}