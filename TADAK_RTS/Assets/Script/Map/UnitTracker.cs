using System.Collections.Generic;
using System.Linq;

public interface IUnitMeasurable {
    void AddUnit(BaseUnit unit);
    void RemoveUnit(BaseUnit unit);
    IEnumerable<UnitSummary> GetUnitSummaries();
    List<BaseUnit> UnitsInRange { get; }
}


public class UnitTracker : IUnitMeasurable {
    private List<BaseUnit> unitsInRange = new List<BaseUnit>();
    public List<BaseUnit> UnitsInRange => unitsInRange;

    // 유닛 수 변경 이벤트 발생 시 외부에 전달
    public event System.Action OnRegistryChanged;

    public void AddUnit(BaseUnit unit) {
        if (!unitsInRange.Contains(unit)) {
            unit.OnDead += RemoveUnit;
            unitsInRange.Add(unit);
            OnRegistryChanged?.Invoke();
        }
    }

    public void RemoveUnit(BaseUnit unit) {
        if (unitsInRange.Contains(unit)) {
            unit.OnDead -= RemoveUnit;
            unitsInRange.Remove(unit);
            OnRegistryChanged?.Invoke();
        }
    }

    public IEnumerable<UnitSummary> GetUnitSummaries() {
        return unitsInRange
            .GroupBy(u => new { u.OwnerID, u.ID })
            .Select(group => new UnitSummary {
                Owner = group.Key.OwnerID,
                UnitName = group.Key.ID,
                Count = group.Count()
            });
    }
}