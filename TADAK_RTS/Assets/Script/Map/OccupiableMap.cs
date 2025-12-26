
using UnityEngine;

public enum OccupancyState { Neutral, Occupying, Occupied }
public interface IOccupiable {
    OccupancyState State { get; set; }
    bool IsOccupiedBy(string username);
}

public class OccupiableMap : Map, IOccupiable, IOwnable {
    private OccupyProcessor _occupyProcessor;
    public string OwnerName { get; set; }
    public OccupancyState State { get; set; }
    public bool IsOccupiedBy(string username) => State == OccupancyState.Occupied && OwnerName == username;
    public bool IsOwnedBy(string username) => username == OwnerName;


    private void Awake() {
        sectorName = gameObject.name; // 오브젝트 이름을 섹터 이름으로 사용
        sectorID = gameObject.GetInstanceID();
        _occupyProcessor = new OccupyProcessor(this);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<BaseUnit>(out var unit)) {
            _occupyProcessor.UnitRegistry.AddUnit(unit);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.TryGetComponent<BaseUnit>(out var unit)) {
            _occupyProcessor.UnitRegistry.RemoveUnit(unit);
        }
    }

}