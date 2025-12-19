using NUnit.Framework;
using System.Diagnostics;
using System.Collections.Generic;


[System.Serializable]
public class BuildingJsonData {
    public string ID;
    public string Type;
    // 유닛 생산 건물 "Unit"
    // 자원 건물 "Resource"
    public Race Race;
    public int Wood;
    public int Rock;
    public float MaxHealth;
    public float BuildTime;

    // 유닛 생산용
    public List<string> ProducibleUnits;

    // 자원 생산용
    public string ResourceType;
    public int AmountPerTick;
}

[System.Serializable]
public class BuildingDataWrapper {
    public List<BuildingJsonData> buildingDatabase_json;
}