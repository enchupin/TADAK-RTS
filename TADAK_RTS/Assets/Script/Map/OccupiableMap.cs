
public enum OccupationState { Neutral, Capturing, Occupied }
public interface IOccupiable {
    OccupationState State { get; set; }
    bool IsOccupiedBy(string username);
}

public class OccupiableMap : Map, IOccupiable, IOwnable {
    private OccupyProcessor _occupyProcessor;
    public string OwnerName { get; set; }
    public OccupationState State { get; set; }
    public bool IsOwnedBy(string username) {
        return (username == OwnerName);
    }




    private void Awake() {
        sectorName = gameObject.name; // 오브젝트 이름을 섹터 이름으로 사용
        sectorID = gameObject.GetInstanceID();
        _occupyProcessor = new OccupyProcessor(this);
        unitTracker.OnRegistryChanged += HandleUnitCountChanged;
    }

    public bool IsOccupiedBy(string username) {
        return State == OccupationState.Occupied && OwnerName == username;
    }

    public void UpdateOccupyProgress(string capturerUsername, float amount) {

        // 점령 진행
    }

    private void HandleUnitCountChanged() {
        // 점령 로직 등을 실행
    }

}