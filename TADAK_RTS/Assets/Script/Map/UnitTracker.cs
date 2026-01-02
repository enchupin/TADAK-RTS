using System.Collections.Generic;
using System.Linq;

public interface IUnitMeasurable {
    void AddUnit(UnitController unit);
    void RemoveUnit(UnitController unit);
    IEnumerable<UnitSummary> GetUnitSummaries();
    List<UnitController> UnitsInRange { get; }
}


public class UnitTracker : IUnitMeasurable {
    private List<UnitController> unitsInRange = new List<UnitController>();
    public List<UnitController> UnitsInRange => unitsInRange;

    // 유닛 수 변경 이벤트 발생 시 외부에 전달
    public event System.Action OnRegistryChanged;

    public void AddUnit(UnitController unit) {
        if (!unitsInRange.Contains(unit)) {
            unit.OnDead += RemoveUnit;
            unitsInRange.Add(unit);
            OnRegistryChanged?.Invoke();
        }
    }

    public void RemoveUnit(UnitController unit) {
        if (unitsInRange.Contains(unit)) {
            unit.OnDead -= RemoveUnit;
            unitsInRange.Remove(unit);
            OnRegistryChanged?.Invoke();
        }
    }

    public IEnumerable<UnitSummary> GetUnitSummaries() {
        return unitsInRange
            .GroupBy(u => new { u.OwnerId, u._unitData.ID })
            .Select(group => new UnitSummary {
                Owner = group.Key.OwnerId,
                UnitName = group.Key.ID,
                Count = group.Count()
            });
    }
}