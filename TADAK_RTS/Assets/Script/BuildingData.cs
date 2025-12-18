using NUnit.Framework;
using System.Diagnostics;
using System.Collections.Generic;

public enum Race { Human, Elf, Beastman, Undead }

// 건물 데이터 클래스
public abstract class BuildingData {
    public string ID { get; protected set; }

    public string Type { get; protected set; }
    public Race Race { get; protected set; }
    public int Wood { get; protected set; }
    public int Rock { get; protected set; }
    public float MaxHealth { get; protected set; }
    public float BuildTime { get; protected set; }


}


public class BaseBuildingData : BuildingData {
    public BaseBuildingData(string id, Race race, string type, int wood, int rock, float hp, float buildTime) {
        ID = id; Type = type; Race = race; Wood = wood; Rock = rock; MaxHealth = hp; BuildTime = buildTime;
    }
}



// 유닛 생산 건물 인터페이스
public interface IUnitProducer {
    List<string> ProducibleUnits { get; }
    void Produce(string unitName);
}

// 유닛 생산 건물 데이터 클래스
public class UnitBuildingData : BuildingData, IUnitProducer {
    public List<string> ProducibleUnits { get; private set; }

    public UnitBuildingData(string id, Race race, string type, int wood, int rock, float hp, float time, List<string> units) {
        ID = id; Race = race; Type = type; Wood = wood; Rock = rock; MaxHealth = hp; BuildTime = time;
        ProducibleUnits = units;
    }

    public void Produce(string unitName) {
        UnityEngine.Debug.Log($"{ID}에서 {unitName} 생산을 시작합니다.");
    }
}



// 자원 건물 인터페이스
public interface IResourceGenerator {
    string ResourceType { get; }
    int AmountPerTick { get; }
}
// 자원 채취 건물용 데이터 클래스
public class ResourceBuildingData : BuildingData, IResourceGenerator {
    public string ResourceType { get; private set; }
    public int AmountPerTick { get; private set; }

    public ResourceBuildingData(string id, Race race, string type, int wood, int rock, float hp, float time, string resType, int amount) {
        ID = id; Race = race; Type = type; Wood = wood; Rock = rock; MaxHealth = hp; BuildTime = time;
        ResourceType = resType; AmountPerTick = amount;
    }
}