using UnityEngine;

public enum OccupationState { Neutral, Capturing, Occupied }
public interface ICapturable {

    OccupationState State { get; }
    void UpdateCaptureProgress(string capturerUsername, float amount);
    bool IsOccupiedBy(string username);
}



public class CapturableMap : Map, ICapturable, IOwnable {

    public string OwnerName { get; set; }
    public bool IsOwnedBy(string username) {
        return (username == OwnerName);
    }
    public OccupationState State => OccupationState.Neutral;


    private void Awake() {
        sectorName = gameObject.name; // 오브젝트 이름을 섹터 이름으로 사용
        sectorID = gameObject.GetInstanceID();
    }

    public bool IsOccupiedBy(string username) {
        return State == OccupationState.Occupied && OwnerName == username;
    }

    public void UpdateCaptureProgress(string capturerUsername, float amount) {

        // 점령 진행
    }


}