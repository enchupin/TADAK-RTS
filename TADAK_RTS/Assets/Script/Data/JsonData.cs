using System.Collections.Generic;

public enum Race { Human, Elf, Beastman, Undead }

[System.Serializable]
public struct BuildingJsonData {
    public string ID;
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

[System.Serializable]
public struct UnitJsonData {
    public string ID;
    public Race Race;
    public int CostWood;
    public int CostRock;
    public int CostFood;
    public float MoveSpeed;
    public float AttackSpeed;
    public float AttackDamage;
    public float MaxHP;
    public float ProductionTime; // 생산 시간 (초), 기본값 5초
}

[System.Serializable]
public class UnitDataWrapper {
    public List<UnitJsonData> unitDatabase_json;
}