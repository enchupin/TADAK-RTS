
public enum OccupationState { Neutral, Capturing, Occupied }
public interface ICapturable {
    OccupationState State { get; set; }
    bool IsOccupiedBy(string username);
}

public class CapturableMap : Map, ICapturable, IOwnable {
    private CaptureProcessor _captureProcessor;
    public string OwnerName { get; set; }
    public OccupationState State { get; set; }
    public bool IsOwnedBy(string username) {
        return (username == OwnerName);
    }
    

    private void Awake() {
        sectorName = gameObject.name; // 오브젝트 이름을 섹터 이름으로 사용
        sectorID = gameObject.GetInstanceID();
        _captureProcessor = new CaptureProcessor(this);
    }

    public bool IsOccupiedBy(string username) {
        return State == OccupationState.Occupied && OwnerName == username;
    }

    public void UpdateCaptureProgress(string capturerUsername, float amount) {

        // 점령 진행
    }


}