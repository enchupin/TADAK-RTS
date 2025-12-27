using UnityEngine;

public class PlayerResourcesManager : MonoBehaviour {
    public static PlayerResourcesManager Instance { get; private set; }

    // 테스트용 자원 설정
    public int Wood { get; private set; } = 10000;
    public int Rock { get; private set; } = 10000;
    public int Food { get; private set; } = 10000;
    public int CurrentPopulation { get; private set; } = 0;
    public int MaxPopulation { get; private set; } = 200;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }



    // 자원 사용 가능 여부 확인
    public bool CanAfford(int foodCost, int woodCost, int rockCost) {
        return Wood >= woodCost && Rock >= rockCost && Food >= foodCost;
    }

    // 자원 소비
    public bool ConsumeResources(int food, int wood, int rock) {
        if (!CanAfford(food, wood, rock)) {
            return false;
        }

        Food -= food;
        Wood -= wood;
        Rock -= rock;
        UpdateUI();
        return true;
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