using UnityEngine;
public interface ICapturable {
    CaptureState CurrentState { get; }
    void UpdateCaptureProgress(string capturerUsername, float amount);
    bool IsOccupiedBy(string username);
}

public class CapturableMap : Map, ICapturable {

    [SerializeField] private CaptureState currentState = new CaptureState();
    public CaptureState CurrentState => currentState;

    // 로직 클래스 래핑
    private CaptureProcessor _processor = new CaptureProcessor();

    private void Awake() {
        sectorName = gameObject.name; // 오브젝트 이름을 섹터 이름으로 사용
        sectorID = gameObject.GetInstanceID();
    }


    public bool IsOccupiedBy(string username) {
        return CurrentState.State == OccupationState.Occupied && CurrentState.Owner == username;
    }

    public void UpdateCaptureProgress(string capturerUsername, float amount) {
        _processor.Process(CurrentState, capturerUsername, amount);
    }


}