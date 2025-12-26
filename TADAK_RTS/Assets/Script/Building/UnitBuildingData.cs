
// 유닛 생산 건물 인터페이스
using System.Collections.Generic;

public interface IUnitProducer {
    List<string> ProducibleUnits { get; }
    void Produce(string unitName);
}

// 유닛 생산 건물 데이터 클래스
public class UnitBuildingData : BaseBuilding, IUnitProducer {
    public List<string> ProducibleUnits { get; private set; }

    public UnitBuildingData(string id, string type, Race race, int wood, int rock, float hp, float time, List<string> units) {
        ID = id; Race = race; Type = type; Wood = wood; Rock = rock; MaxHealth = hp; BuildTime = time;
        ProducibleUnits = units;
    }

    public void Produce(string unitName) {
        UnityEngine.Debug.Log($"{ID}에서 {unitName} 생산을 시작합니다.");
    }
}