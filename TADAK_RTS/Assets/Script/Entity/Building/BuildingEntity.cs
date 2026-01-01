using UnityEngine;

public class BuildingEntity : MonoBehaviour {


    private BaseBuilding _data;
    private Health _health;

    private void Awake() {


        _health = GetComponent<Health>();
    }




    // 데이터 초기화
    public void Setup(BaseBuilding data) {
        _data = data;

        // 체력 컴포넌트 초기화 (데이터에서 MaxHealth를 가져와 전달)
        if (_health != null) {
            _health.Initialize(_data.MaxHealth);
        }

    }



    // 클릭했을 때
    private void OnMouseDown() {
        if (_data != null) {



        }
    }



}